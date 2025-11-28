using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class CrittersCage : CrittersActor
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000180 RID: 384 RVA: 0x00009D04 File Offset: 0x00007F04
	public Vector3 critterScale
	{
		get
		{
			if (this.subObjectIndex < this.critterScales.Length && this.subObjectIndex >= 0)
			{
				return this.critterScales[this.subObjectIndex];
			}
			return Vector3.one;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000181 RID: 385 RVA: 0x00009D36 File Offset: 0x00007F36
	public bool CanCatch
	{
		get
		{
			return this.heldByPlayer && !this.hasCritter && !this.inReleasingPosition && this._releaseCooldownEnd <= Time.time;
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00009D62 File Offset: 0x00007F62
	public void SetHasCritter(bool value)
	{
		if (this.hasCritter != value && !value)
		{
			this._releaseCooldownEnd = Time.time + this.releaseCooldown;
		}
		this.hasCritter = value;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00009D8F File Offset: 0x00007F8F
	public override void Initialize()
	{
		base.Initialize();
		this.hasCritter = false;
		this.heldByPlayer = false;
		this.inReleasingPosition = false;
		this.SetLidActive(true, false);
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00009DB4 File Offset: 0x00007FB4
	private void UpdateCageVisuals()
	{
		this.SetLidActive(!this.heldByPlayer || this.hasCritter, true);
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00009DD0 File Offset: 0x00007FD0
	private void SetLidActive(bool active, bool playAudio = true)
	{
		if (active != this._lidActive && playAudio)
		{
			this.sound.GTPlayOneShot(active ? this.openSound : this.closeSound, 1f);
		}
		this.lid.SetActive(active);
		this._lidActive = active;
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00009E21 File Offset: 0x00008021
	protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		base.RemoteGrabbedBy(grabbingActor);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00009E3C File Offset: 0x0000803C
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00009E5D File Offset: 0x0000805D
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
	{
		base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00009E79 File Offset: 0x00008079
	protected override void HandleRemoteReleased()
	{
		base.HandleRemoteReleased();
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00009E8E File Offset: 0x0000808E
	public override bool ShouldDespawn()
	{
		return base.ShouldDespawn() && !this.hasCritter;
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00009EA3 File Offset: 0x000080A3
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.hasCritter);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00009EC0 File Offset: 0x000080C0
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag))
		{
			return false;
		}
		this.SetHasCritter(flag);
		return true;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00009EF1 File Offset: 0x000080F1
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.hasCritter);
		return this.TotalActorDataLength();
	}

	// Token: 0x0600018E RID: 398 RVA: 0x000094F4 File Offset: 0x000076F4
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00009F14 File Offset: 0x00008114
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.SetHasCritter(flag);
		return this.TotalActorDataLength();
	}

	// Token: 0x040001BD RID: 445
	public Transform grabPosition;

	// Token: 0x040001BE RID: 446
	public Transform cagePosition;

	// Token: 0x040001BF RID: 447
	public float grabDistance;

	// Token: 0x040001C0 RID: 448
	[SerializeField]
	private Vector3[] critterScales = new Vector3[]
	{
		Vector3.one
	};

	// Token: 0x040001C1 RID: 449
	[SerializeField]
	private float releaseCooldown = 0.25f;

	// Token: 0x040001C2 RID: 450
	[SerializeField]
	private AudioSource sound;

	// Token: 0x040001C3 RID: 451
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x040001C4 RID: 452
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x040001C5 RID: 453
	public GameObject lid;

	// Token: 0x040001C6 RID: 454
	[NonSerialized]
	public bool heldByPlayer;

	// Token: 0x040001C7 RID: 455
	[NonSerialized]
	private bool hasCritter;

	// Token: 0x040001C8 RID: 456
	[NonSerialized]
	public bool inReleasingPosition;

	// Token: 0x040001C9 RID: 457
	private float _releaseCooldownEnd;

	// Token: 0x040001CA RID: 458
	private bool _lidActive;
}
