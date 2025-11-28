using System;
using GorillaGameModes;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000807 RID: 2055
public class HoldableHand : HoldableObject, IGorillaSliceableSimple
{
	// Token: 0x170004CE RID: 1230
	// (get) Token: 0x06003611 RID: 13841 RVA: 0x00125684 File Offset: 0x00123884
	public VRRig Rig
	{
		get
		{
			return this.myPlayer;
		}
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x0012568C File Offset: 0x0012388C
	private void Start()
	{
		if (this.myPlayer.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003615 RID: 13845 RVA: 0x001256C1 File Offset: 0x001238C1
	public void SliceUpdate()
	{
		this.interactionPoint.enabled = (GameMode.ActiveGameMode is GorillaGuardianManager);
	}

	// Token: 0x06003616 RID: 13846 RVA: 0x001256DC File Offset: 0x001238DC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && !this.myPlayer.creator.IsLocal && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
			this.myPlayer.netView.SendRPC("GrabbedByPlayer", this.myPlayer.Creator, new object[]
			{
				this.isBody,
				this.isLeftHand,
				flag
			});
			this.myPlayer.ApplyLocalGrabOverride(this.isBody, this.isLeftHand, grabbingHand.transform);
			EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
			this.ClearOtherGrabs(flag);
		}
	}

	// Token: 0x06003617 RID: 13847 RVA: 0x001257B4 File Offset: 0x001239B4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && !this.myPlayer.creator.IsLocal)
		{
			bool forLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 vector = Vector3.zero;
			if (gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				vector = GTPlayer.Instance.GetHandVelocityTracker(forLeftHand).GetAverageVelocity(true, 0.15f, false);
			}
			vector = Vector3.ClampMagnitude(vector, 20f);
			this.myPlayer.netView.SendRPC("DroppedByPlayer", this.myPlayer.Creator, new object[]
			{
				vector
			});
			this.myPlayer.ClearLocalGrabOverride();
			this.myPlayer.ApplyLocalTrajectoryOverride(vector);
			EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
		}
		return true;
	}

	// Token: 0x06003618 RID: 13848 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06003619 RID: 13849 RVA: 0x00125897 File Offset: 0x00123A97
	public override void DropItemCleanup()
	{
		this.myPlayer.ClearLocalGrabOverride();
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x001258A4 File Offset: 0x00123AA4
	private void ClearOtherGrabs(bool grabbedLeft)
	{
		IHoldableObject holdableObject = grabbedLeft ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment;
		if (this.isBody)
		{
			if (holdableObject == this.myPlayer.leftHolds || holdableObject == this.myPlayer.rightHolds)
			{
				EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
				return;
			}
		}
		else if (this.isLeftHand)
		{
			if (holdableObject == this.myPlayer.rightHolds || holdableObject == this.myPlayer.bodyHolds)
			{
				EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
				return;
			}
		}
		else if (holdableObject == this.myPlayer.leftHolds || holdableObject == this.myPlayer.bodyHolds)
		{
			EquipmentInteractor.instance.UpdateHandEquipment(null, !grabbedLeft);
		}
	}

	// Token: 0x04004578 RID: 17784
	[SerializeField]
	private VRRig myPlayer;

	// Token: 0x04004579 RID: 17785
	[SerializeField]
	private bool isBody;

	// Token: 0x0400457A RID: 17786
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x0400457B RID: 17787
	public InteractionPoint interactionPoint;
}
