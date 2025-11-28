using System;
using UnityEngine;

// Token: 0x02000D02 RID: 3330
public class ZoneEntityBSP : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000787 RID: 1927
	// (get) Token: 0x060050D5 RID: 20693 RVA: 0x001A1930 File Offset: 0x0019FB30
	public VRRig entityRig
	{
		get
		{
			return this._entityRig;
		}
	}

	// Token: 0x17000788 RID: 1928
	// (get) Token: 0x060050D6 RID: 20694 RVA: 0x001A1938 File Offset: 0x0019FB38
	public GTZone currentZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return GTZone.none;
			}
			return zoneDef.zoneId;
		}
	}

	// Token: 0x17000789 RID: 1929
	// (get) Token: 0x060050D7 RID: 20695 RVA: 0x001A194C File Offset: 0x0019FB4C
	public GTSubZone currentSubZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return GTSubZone.none;
			}
			return zoneDef.subZoneId;
		}
	}

	// Token: 0x1700078A RID: 1930
	// (get) Token: 0x060050D8 RID: 20696 RVA: 0x001A1960 File Offset: 0x0019FB60
	public GroupJoinZoneAB GroupZone
	{
		get
		{
			ZoneDef zoneDef = this.currentNode;
			if (zoneDef == null)
			{
				return default(GroupJoinZoneAB);
			}
			return zoneDef.groupZoneAB;
		}
	}

	// Token: 0x060050D9 RID: 20697 RVA: 0x001A1986 File Offset: 0x0019FB86
	private void Start()
	{
		if (!this._entityRig.isOfflineVRRig)
		{
			this._emitTelemetry = false;
		}
		this.SliceUpdate();
	}

	// Token: 0x060050DA RID: 20698 RVA: 0x001A19A2 File Offset: 0x0019FBA2
	public virtual void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x060050DB RID: 20699 RVA: 0x001A19AB File Offset: 0x0019FBAB
	public virtual void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x060050DC RID: 20700 RVA: 0x001A19B4 File Offset: 0x0019FBB4
	public void SliceUpdate()
	{
		if (this.isUpdateDisabled)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraphBSP.Instance.FindZoneAtPoint(base.transform.position);
		if (!zoneDef.IsSameZone(this.currentNode))
		{
			this.lastExitedNode = this.currentNode;
			this.currentNode = zoneDef;
			this.lastEnteredNode = zoneDef;
			if (this._emitTelemetry)
			{
				ZoneDef zoneDef2 = this.lastEnteredNode;
				if (zoneDef2 != null && zoneDef2.trackEnter)
				{
					GorillaTelemetry.EnqueueZoneEvent(this.lastEnteredNode, GTZoneEventType.zone_enter);
				}
				ZoneDef zoneDef3 = this.lastExitedNode;
				if (zoneDef3 != null && zoneDef3.trackExit)
				{
					GorillaTelemetry.EnqueueZoneEvent(this.lastExitedNode, GTZoneEventType.zone_exit);
					return;
				}
			}
		}
		else if (this._emitTelemetry)
		{
			ZoneDef zoneDef4 = this.currentNode;
			if (zoneDef4 != null && zoneDef4.trackStay)
			{
				GorillaTelemetry.EnqueueZoneEvent(this.currentNode, GTZoneEventType.zone_stay);
			}
		}
	}

	// Token: 0x060050DD RID: 20701 RVA: 0x001A1A79 File Offset: 0x0019FC79
	public void EnableZoneChanges()
	{
		this.isUpdateDisabled = false;
	}

	// Token: 0x060050DE RID: 20702 RVA: 0x001A1A82 File Offset: 0x0019FC82
	public void DisableZoneChanges()
	{
		this.isUpdateDisabled = true;
	}

	// Token: 0x0400601B RID: 24603
	[Space]
	[SerializeField]
	private bool _emitTelemetry = true;

	// Token: 0x0400601C RID: 24604
	[Space]
	[SerializeField]
	private VRRig _entityRig;

	// Token: 0x0400601D RID: 24605
	[Space]
	[NonSerialized]
	public ZoneDef currentNode;

	// Token: 0x0400601E RID: 24606
	[NonSerialized]
	public ZoneDef lastEnteredNode;

	// Token: 0x0400601F RID: 24607
	[NonSerialized]
	public ZoneDef lastExitedNode;

	// Token: 0x04006020 RID: 24608
	private bool isUpdateDisabled;
}
