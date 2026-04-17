using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks;

[DefaultExecutionOrder(10000)]
public sealed class AlarmClockManager : MonoBehaviour
{
	[Serializable]
	public class AlarmClockData
	{
		public string Key;

		public GTZone[] Zones;

		public Transform SpawnPoint;
	}

	public const string SaveDataKey = "AlarmClock";

	[SerializeField]
	private string _loadingMessage = "";

	[SerializeField]
	private float _wrongWarpTolerance = 0.5f;

	[SerializeField]
	private AlarmClockData[] _clockData = new AlarmClockData[0];

	[SerializeField]
	private Transform _defaultSpawn;

	public UnityEvent OnWakeUp;

	private Transform _teleportTarget;

	private AlarmClockData _activeClockData;

	[CanBeNull]
	private AlarmClock _activeClock;

	public static AlarmClockManager Instance { get; private set; }

	public bool Initialized { get; private set; }

	public string ActiveKey { get; private set; } = "";

	private void Start()
	{
		if (Instance != null)
		{
			Debug.LogError("Duplicate instance of singleton class AlarmClockManager.");
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (_defaultSpawn == null)
		{
			Debug.LogError("No default spawn set in AlarmClockManager.");
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		ActiveKey = PlayerPrefs.GetString("AlarmClock");
		if (string.IsNullOrEmpty(ActiveKey))
		{
			Debug.Log("No alarm clock value found.");
			Initialized = true;
		}
		else
		{
			StartCoroutine(ConnectToPlayerSpawned());
		}
	}

	private IEnumerator ConnectToPlayerSpawned()
	{
		while (GorillaTagger.Instance == null || GorillaTagger.Instance.mainCamera == null)
		{
			yield return null;
		}
		PrivateUIRoom.ForceStartOverlay(_loadingMessage);
		yield return new WaitForSeconds(1f);
		while (ZoneManagement.instance == null || !ZoneManagement.instance.Initialized)
		{
			yield return null;
		}
		GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
	}

	public static void ToggleAlarmClock(AlarmClock clock)
	{
		Instance.ToggleAlarmClockInternal(clock);
	}

	private void ToggleAlarmClockInternal(AlarmClock clock)
	{
		if (ActiveKey == clock.Key)
		{
			_activeClock?.OnDeactivate?.Invoke();
			_activeClock = null;
			ActiveKey = "";
		}
		else
		{
			_activeClock?.OnDeactivate?.Invoke();
			_activeClock = clock;
			_activeClock?.OnActivate?.Invoke();
			ActiveKey = clock.Key;
		}
		PlayerPrefs.SetString("AlarmClock", ActiveKey);
		Debug.Log("Alarm clock data set to \"" + ActiveKey + "\".");
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	[UsedImplicitly]
	private bool AllUniqueClockKeys()
	{
		HashSet<string> hashSet = new HashSet<string>();
		AlarmClockData[] clockData = _clockData;
		foreach (AlarmClockData alarmClockData in clockData)
		{
			if (!hashSet.Add(alarmClockData.Key))
			{
				return false;
			}
		}
		return true;
	}

	private void OnPlayerSpawned()
	{
		Debug.Log("Loaded alarm clock value " + ActiveKey);
		if (!string.IsNullOrEmpty(ActiveKey))
		{
			AlarmClockData alarmClockData = _clockData.FirstOrDefault((AlarmClockData c) => c.Key == ActiveKey);
			if (alarmClockData != null)
			{
				_activeClockData = alarmClockData;
				_teleportTarget = alarmClockData.SpawnPoint;
				PrivateUIRoom.ForceStartOverlay(_loadingMessage);
				ZoneManagement.SetActiveZones(alarmClockData.Zones);
				StartCoroutine(WaitForSubscriptionData());
				return;
			}
		}
		Initialized = true;
	}

	private IEnumerator WaitForSubscriptionData()
	{
		while (!MothershipClientApiUnity.IsClientLoggedIn() || !SubscriptionManager.LocalSubscriptionDataInitialized)
		{
			yield return null;
		}
		StartCoroutine(SubscriptionManager.IsLocalSubscribed() ? ZoneLoad() : ClearUnsubPlayerData());
	}

	private IEnumerator ClearUnsubPlayerData()
	{
		while (!MothershipClientApiUnity.IsClientLoggedIn() || !SubscriptionManager.LocalSubscriptionDataInitialized)
		{
			yield return null;
		}
		if (SubscriptionManager.LocalSubscriptionStatus() != SubscriptionManager.SubscriptionStatus.Active)
		{
			Debug.Log("No subscription, warping home.");
			PlayerPrefs.SetString("AlarmClock", "");
			GTPlayer.Instance.TeleportTo(_defaultSpawn, matchDestinationRotation: true, maintainVelocity: false);
			Initialized = true;
			yield return null;
			PrivateUIRoom.StopForcedOverlay();
		}
	}

	private IEnumerator ZoneLoad()
	{
		yield return null;
		if (_activeClockData.Zones.Length > 1 || _activeClockData.Zones[0] != GTZone.forest)
		{
			while (ZoneManagement.instance.AnyActiveLoadOps())
			{
				yield return null;
			}
		}
		while (!GTPlayer.hasInstance)
		{
			yield return null;
		}
		yield return null;
		GTPlayer.Instance.TeleportTo(_teleportTarget, matchDestinationRotation: true, maintainVelocity: false);
		yield return null;
		int fixAttempts = 0;
		while ((GTPlayer.Instance.playerRigidBody.position - _teleportTarget.position).sqrMagnitude > _wrongWarpTolerance * _wrongWarpTolerance)
		{
			int num = fixAttempts + 1;
			fixAttempts = num;
			if (num > 10)
			{
				break;
			}
			Debug.Log("AlarmClockManager attempting wrong warp fix.");
			GTPlayer.Instance.TeleportTo(_teleportTarget, matchDestinationRotation: true, maintainVelocity: false);
			yield return null;
		}
		PrivateUIRoom.StopForcedOverlay();
		OnWakeUp?.Invoke();
		Initialized = true;
	}
}
