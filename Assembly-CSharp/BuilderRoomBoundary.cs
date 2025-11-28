using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class BuilderRoomBoundary : GorillaTriggerBox
{
	// Token: 0x0600243F RID: 9279 RVA: 0x000C2554 File Offset: 0x000C0754
	private void Awake()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter += this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit += this.OnExitedBoundary;
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000C25C8 File Offset: 0x000C07C8
	private void OnDestroy()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter -= this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit -= this.OnExitedBoundary;
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000C263C File Offset: 0x000C083C
	public void OnEnteredBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.rigRef.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (builderTable.isTableMutable)
		{
			this.rigRef.EnableBuilderResizeWatch(true);
		}
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000C26B8 File Offset: 0x000C08B8
	public void OnExitedBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		this.rigRef.EnableBuilderResizeWatch(false);
	}

	// Token: 0x04002FAB RID: 12203
	[SerializeField]
	private List<SizeChangerTrigger> enableOnEnterTrigger;

	// Token: 0x04002FAC RID: 12204
	[SerializeField]
	private SizeChangerTrigger disableOnExitTrigger;

	// Token: 0x04002FAD RID: 12205
	private VRRig rigRef;
}
