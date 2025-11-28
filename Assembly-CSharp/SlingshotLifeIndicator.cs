using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class SlingshotLifeIndicator : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x060019F5 RID: 6645 RVA: 0x0008AB07 File Offset: 0x00088D07
	// (set) Token: 0x060019F6 RID: 6646 RVA: 0x0008AB0F File Offset: 0x00088D0F
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x060019F7 RID: 6647 RVA: 0x0008AB18 File Offset: 0x00088D18
	// (set) Token: 0x060019F8 RID: 6648 RVA: 0x0008AB20 File Offset: 0x00088D20
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060019F9 RID: 6649 RVA: 0x0008AB29 File Offset: 0x00088D29
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x0008AB32 File Offset: 0x00088D32
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x0008AB56 File Offset: 0x00088D56
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.Reset();
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x0008AB80 File Offset: 0x00088D80
	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(false);
		}
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x0008ABA8 File Offset: 0x00088DA8
	public void SliceUpdate()
	{
		if (!NetworkSystem.Instance.InRoom || (this.checkedBattle && !this.inBattle))
		{
			if (this.indicator1.activeSelf)
			{
				this.indicator1.SetActive(false);
			}
			if (this.indicator2.activeSelf)
			{
				this.indicator2.SetActive(false);
			}
			if (this.indicator3.activeSelf)
			{
				this.indicator3.SetActive(false);
			}
			return;
		}
		if (this.bMgr == null)
		{
			this.checkedBattle = true;
			this.inBattle = true;
			if (GorillaGameManager.instance == null)
			{
				return;
			}
			this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (this.bMgr == null)
			{
				this.inBattle = false;
				return;
			}
		}
		VRRig vrrig = this.myRig;
		if (((vrrig != null) ? vrrig.creator : null) == null)
		{
			return;
		}
		int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
		this.SetActive(this.indicator1, playerLives >= 1);
		this.SetActive(this.indicator2, playerLives >= 2);
		this.SetActive(this.indicator3, playerLives >= 3);
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x0008ACDD File Offset: 0x00088EDD
	public void OnLeftRoom()
	{
		this.Reset();
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x0008ACE5 File Offset: 0x00088EE5
	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	// Token: 0x04002383 RID: 9091
	private VRRig myRig;

	// Token: 0x04002384 RID: 9092
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x04002385 RID: 9093
	public bool checkedBattle;

	// Token: 0x04002386 RID: 9094
	public bool inBattle;

	// Token: 0x04002387 RID: 9095
	public GameObject indicator1;

	// Token: 0x04002388 RID: 9096
	public GameObject indicator2;

	// Token: 0x04002389 RID: 9097
	public GameObject indicator3;
}
