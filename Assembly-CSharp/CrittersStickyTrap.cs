using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class CrittersStickyTrap : CrittersToolThrowable
{
	// Token: 0x060002C3 RID: 707 RVA: 0x00010E53 File Offset: 0x0000F053
	public override void Initialize()
	{
		base.Initialize();
		this.TogglePhysics(!this.isStuck);
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00010E6A File Offset: 0x0000F06A
	public override void OnDisable()
	{
		base.OnDisable();
		this.isStuck = false;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00010E7C File Offset: 0x0000F07C
	public override void SetImpulse()
	{
		if (this.isOnPlayer || this.isSceneActor)
		{
			return;
		}
		this.localLastImpulse = this.lastImpulseTime;
		base.MoveActor(this.lastImpulsePosition, this.lastImpulseQuaternion, this.parentActorId >= 0, false, true);
		this.TogglePhysics(this.usesRB && this.parentActorId == -1 && !this.isStuck);
		if (!this.rb.isKinematic)
		{
			this.rb.linearVelocity = this.lastImpulseVelocity;
			this.rb.angularVelocity = this.lastImpulseAngularVelocity;
		}
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00010F18 File Offset: 0x0000F118
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.stickOnImpact)
			{
				this.rb.isKinematic = true;
				this.isStuck = true;
				this.updatedSinceLastFrame = true;
				base.UpdateImpulses(false, true);
			}
			CrittersStickyGoo crittersStickyGoo = (CrittersStickyGoo)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.StickyGoo, this.subStickyGooIndex);
			if (crittersStickyGoo == null)
			{
				return;
			}
			CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyDeployed, this.actorId, base.transform.position, Quaternion.LookRotation(hitNormal));
			Vector3 vector = base.transform.forward;
			vector -= hitNormal * Vector3.Dot(hitNormal, vector);
			crittersStickyGoo.MoveActor(hitPosition, Quaternion.LookRotation(vector, hitNormal), false, true, true);
			crittersStickyGoo.SetImpulseVelocity(Vector3.zero, Vector3.zero);
			base.UpdateImpulses(true, false);
		}
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0000DD33 File Offset: 0x0000BF33
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00010FF1 File Offset: 0x0000F1F1
	protected override void OnPickedUp()
	{
		if (this.isStuck)
		{
			this.isStuck = false;
			this.updatedSinceLastFrame = true;
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00011009 File Offset: 0x0000F209
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.isStuck);
	}

	// Token: 0x060002CA RID: 714 RVA: 0x00011024 File Offset: 0x0000F224
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		bool flag;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag)))
		{
			return false;
		}
		this.isStuck = flag;
		this.TogglePhysics(!this.isStuck);
		return true;
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00011061 File Offset: 0x0000F261
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.isStuck);
		return this.TotalActorDataLength();
	}

	// Token: 0x060002CC RID: 716 RVA: 0x000094F4 File Offset: 0x000076F4
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00011084 File Offset: 0x0000F284
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.isStuck = flag;
		this.TogglePhysics(!this.isStuck);
		return this.TotalActorDataLength();
	}

	// Token: 0x04000336 RID: 822
	[Header("Sticky Trap")]
	public bool stickOnImpact = true;

	// Token: 0x04000337 RID: 823
	public int subStickyGooIndex = -1;

	// Token: 0x04000338 RID: 824
	private bool isStuck;
}
