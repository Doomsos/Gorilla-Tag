using System;
using System.Collections;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class PropHuntDebugHelper : MonoBehaviour
{
	// Token: 0x06000E58 RID: 3672 RVA: 0x0004C030 File Offset: 0x0004A230
	protected void Awake()
	{
		if (PropHuntDebugHelper.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		PropHuntDebugHelper.instance = this;
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0004C04C File Offset: 0x0004A24C
	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		this._propHuntManager = Object.FindAnyObjectByType<GorillaPropHuntGameManager>();
		if (this._propHuntManager != null)
		{
			Debug.Log("PropHuntDebugHelper :: Found number of props " + PropHuntPools.AllPropCosmeticIds.Length.ToString());
			this._cachedAllPropIDs = PropHuntPools.AllPropCosmeticIds;
			this._localPropHuntHandFollower = VRRig.LocalRig.GetComponent<PropHuntHandFollower>();
			this.UpdatePropsText();
		}
		yield break;
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0004C05C File Offset: 0x0004A25C
	public void UpdatePropsText()
	{
		string selectedPropID = this.GetSelectedPropID(this._selectedPropIndex);
		string text = string.Empty;
		if (this._selectedPropIndex != -1)
		{
			CosmeticSO cosmeticSO = this._allCosmetics.SearchForCosmeticSO(selectedPropID);
			if (cosmeticSO != null)
			{
				text = cosmeticSO.info.displayName;
			}
		}
		this._propsText.text = "Current Prop: " + this.GetCurrentPropInfo() + "\n" + string.Format("Selected Prop: {0} - {1} ({2}/{3})", new object[]
		{
			selectedPropID,
			text,
			this._selectedPropIndex,
			this._cachedAllPropIDs.Length
		});
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0004C0FD File Offset: 0x0004A2FD
	private string GetCurrentPropInfo()
	{
		return string.Empty;
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0004C104 File Offset: 0x0004A304
	private string GetSelectedPropID(int index)
	{
		if (index <= -1)
		{
			return "None";
		}
		return this._cachedAllPropIDs[index];
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0004C118 File Offset: 0x0004A318
	[ContextMenu("Prev Prop")]
	public void PrevProp()
	{
		this._selectedPropIndex--;
		if (this._selectedPropIndex < -1)
		{
			this._selectedPropIndex = this._cachedAllPropIDs.Length - 1;
		}
		string newPropId = (this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty;
		this.SendForcePropHandRPC(newPropId);
		this.UpdatePropsText();
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0004C178 File Offset: 0x0004A378
	[ContextMenu("Next Prop")]
	public void NextProp()
	{
		this._selectedPropIndex++;
		if (this._selectedPropIndex >= this._cachedAllPropIDs.Length)
		{
			this._selectedPropIndex = -1;
		}
		string newPropId = (this._selectedPropIndex > -1) ? this.GetSelectedPropID(this._selectedPropIndex) : string.Empty;
		this.SendForcePropHandRPC(newPropId);
		this.UpdatePropsText();
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00002789 File Offset: 0x00000989
	private void SendForcePropHandRPC(string newPropId)
	{
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00002789 File Offset: 0x00000989
	[ContextMenu("Toggle Round")]
	public void ToggleRound()
	{
	}

	// Token: 0x0400115C RID: 4444
	[OnEnterPlay_SetNull]
	public static PropHuntDebugHelper instance;

	// Token: 0x0400115D RID: 4445
	[SerializeField]
	private GorillaPropHuntGameManager _propHuntManager;

	// Token: 0x0400115E RID: 4446
	[SerializeField]
	private PropHuntHandFollower _localPropHuntHandFollower;

	// Token: 0x0400115F RID: 4447
	[SerializeField]
	private TextMeshPro _propsText;

	// Token: 0x04001160 RID: 4448
	[SerializeField]
	private AllCosmeticsArraySO _allCosmetics;

	// Token: 0x04001161 RID: 4449
	private string[] _cachedAllPropIDs;

	// Token: 0x04001162 RID: 4450
	private int _selectedPropIndex = -1;
}
