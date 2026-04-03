using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

public class TitleDataDateRefActivation : MonoBehaviour, IGorillaSliceableSimple
{
	private enum ReadyState
	{
		None,
		Initializing,
		Ready,
		Crashed
	}

	[Serializable]
	private class TitleDataDateRefActivationTarget : IComparable<TitleDataDateRefActivationTarget>
	{
		[SerializeField]
		private int hrs;

		[SerializeField]
		private int min;

		[SerializeField]
		private int sec;

		[SerializeField]
		private GameObject gameObject;

		[SerializeField]
		private bool activationState;

		[SerializeField]
		private bool initialState;

		[SerializeField]
		private bool persistent = true;

		[SerializeField]
		private UnityEvent payload;

		[SerializeField]
		private bool lateDispatch;

		private DateTime dateTime = DateTime.MaxValue;

		private string gameObjectName
		{
			get
			{
				if (!(gameObject == null))
				{
					return (activationState ? "Activate " : "Deactivate ") + gameObject.name;
				}
				return "No Game Object";
			}
		}

		private string eventsLabel => $"Events [{payload.GetPersistentEventCount()}]" + (lateDispatch ? " *" : "");

		public DateTime ActivationTime => dateTime;

		public void Initialize(DateTime refTime)
		{
			dateTime = refTime.AddHours(hrs).AddMinutes(min).AddSeconds(sec);
			gameObject.SetActive(initialState);
		}

		public void Activate(DateTime now)
		{
			float num = (float)(now - dateTime).TotalSeconds;
			PersistLog.Log($"TitleDataDateRefActivationTarget :: Activate :: Time:{num}");
			Activate(num);
		}

		public void Activate()
		{
			Activate(0f);
		}

		private void Activate(float late)
		{
			if (gameObject != null && (persistent || late < 1f))
			{
				gameObject.SetActive(activationState);
				if (activationState)
				{
					Animator[] componentsInChildren = gameObject.GetComponentsInChildren<Animator>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						int fullPathHash = componentsInChildren[i].GetCurrentAnimatorStateInfo(0).fullPathHash;
						componentsInChildren[i].PlayInFixedTime(fullPathHash, 0, late);
					}
				}
			}
			if (lateDispatch || late < 1f)
			{
				payload?.Invoke();
			}
		}

		int IComparable<TitleDataDateRefActivationTarget>.CompareTo(TitleDataDateRefActivationTarget other)
		{
			return dateTime.CompareTo(other.dateTime);
		}
	}

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private TitleDataDateRefActivationTarget[] nodes;

	private ReadyState readyState;

	private List<TitleDataDateRefActivationTarget> nodeList = new List<TitleDataDateRefActivationTarget>();

	private async void Initialize()
	{
		if (readyState == ReadyState.Initializing || readyState == ReadyState.Ready)
		{
			return;
		}
		readyState = ReadyState.Initializing;
		if (titleDataKey.IsNullOrEmpty())
		{
			onTD("1/1/3001");
			return;
		}
		while (PlayFabTitleDataCache.Instance == null)
		{
			await Task.Yield();
		}
		PlayFabTitleDataCache.Instance.GetTitleData(titleDataKey, onTD, onTDError);
	}

	private void onTD(string s)
	{
		try
		{
			nodeList.Clear();
			DateTime refTime = DateTime.Parse(s);
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].Initialize(refTime);
				nodeList.Add(nodes[i]);
			}
			nodeList.Sort();
			readyState = ReadyState.Ready;
		}
		catch (Exception ex)
		{
			Debug.Log("TitleDataDateRefActivation :: onTD :: " + ex.Message + " :: " + ex.StackTrace);
			readyState = ReadyState.Crashed;
		}
	}

	private void onTDError(PlayFabError error)
	{
		Debug.Log($"TitleDataDateRefActivation :: onTDError :: {error}");
		readyState = ReadyState.Crashed;
	}

	private void OnEnable()
	{
		Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (readyState == ReadyState.Ready)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (serverTime.Year >= 2000 && nodeList.Count > 0 && nodeList[0].ActivationTime <= serverTime)
			{
				nodeList[0].Activate(serverTime);
				nodeList.RemoveAt(0);
			}
		}
	}

	public void ManuallyTriggerNode(int i)
	{
		nodes[i].Activate();
	}
}
