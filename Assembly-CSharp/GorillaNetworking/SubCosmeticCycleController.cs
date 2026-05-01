using UnityEngine;

namespace GorillaNetworking;

public class SubCosmeticCycleController : MonoBehaviour
{
	[SerializeField]
	private bool syncCycleOverNetwork = true;

	private CosmeticCollectionDisplay display;

	private CosmeticCollectionDisplay Display
	{
		get
		{
			if (display == null)
			{
				display = GetComponentInChildren<CosmeticCollectionDisplay>();
			}
			return display;
		}
	}

	public CosmeticsController.CosmeticItem? ActiveCollectable => Display?.ActiveCollectable;

	public int ActiveIndex => Display?.ActiveIndex ?? 0;

	public int Count => Display?.Count ?? 0;

	public string GetAppliedCosmeticID()
	{
		return ActiveCollectable?.appliedCosmeticPlayFabID ?? string.Empty;
	}

	public void CycleForward()
	{
		if (!(Display == null) && Display.Count > 1)
		{
			int num = (Display.ActiveIndex + 1) % Display.Count;
			Display.SetActiveIndex(num);
			SendCycleRPC(num);
		}
	}

	public void CycleBackward()
	{
		if (!(Display == null) && Display.Count > 1)
		{
			int num = (Display.ActiveIndex - 1 + Display.Count) % Display.Count;
			Display.SetActiveIndex(num);
			SendCycleRPC(num);
		}
	}

	public void CycleRandom()
	{
		if (!(Display == null) && Display.Count > 1)
		{
			int num;
			do
			{
				num = Random.Range(0, Display.Count);
			}
			while (num == Display.ActiveIndex);
			Display.SetActiveIndex(num);
			SendCycleRPC(num);
		}
	}

	public void SetIndex(int index)
	{
		Display?.SetActiveIndex(index);
		SendCycleRPC(index);
	}

	private void SendCycleRPC(int newIndex)
	{
		if (syncCycleOverNetwork)
		{
			string text = Display?.ParentPlayFabID;
			if (!string.IsNullOrEmpty(text) && text.Length >= 5)
			{
				_ = NetworkSystem.Instance.InRoom;
			}
		}
	}
}
