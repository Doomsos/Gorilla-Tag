using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class StickyHand : MonoBehaviour, ISpawnable
{
	// Token: 0x17000194 RID: 404
	// (get) Token: 0x0600109E RID: 4254 RVA: 0x00056C73 File Offset: 0x00054E73
	// (set) Token: 0x0600109F RID: 4255 RVA: 0x00056C7B File Offset: 0x00054E7B
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x060010A0 RID: 4256 RVA: 0x00056C84 File Offset: 0x00054E84
	// (set) Token: 0x060010A1 RID: 4257 RVA: 0x00056C8C File Offset: 0x00054E8C
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060010A2 RID: 4258 RVA: 0x00056C98 File Offset: 0x00054E98
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.isLocal = rig.isLocal;
		this.flatHand.enabled = false;
		this.defaultLocalPosition = this.stringParent.transform.InverseTransformPoint(this.rb.transform.position);
		int num = (this.CosmeticSelectedSide == ECosmeticSelectSide.Left) ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00056D10 File Offset: 0x00054F10
	private void Update()
	{
		if (this.isLocal)
		{
			if (this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringDetachLength))
			{
				this.Unstick();
			}
			else if (!this.rb.isKinematic && (this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringTeleportLength))
			{
				this.rb.transform.position = this.stringParent.transform.TransformPoint(this.defaultLocalPosition);
			}
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.rb.isKinematic);
			return;
		}
		if (GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex) != this.rb.isKinematic)
		{
			if (this.rb.isKinematic)
			{
				this.Unstick();
				return;
			}
			this.Stick();
		}
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00056E3E File Offset: 0x0005503E
	private void Stick()
	{
		this.thwackSound.Play();
		this.flatHand.enabled = true;
		this.regularHand.enabled = false;
		this.rb.isKinematic = true;
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x00056E6F File Offset: 0x0005506F
	private void Unstick()
	{
		this.schlupSound.Play();
		this.rb.isKinematic = false;
		this.flatHand.enabled = false;
		this.regularHand.enabled = true;
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x00056EA0 File Offset: 0x000550A0
	private void OnCollisionStay(Collision collision)
	{
		if (!this.isLocal || this.rb.isKinematic)
		{
			return;
		}
		if ((this.rb.transform.position - this.stringParent.transform.position).IsLongerThan(this.stringMaxAttachLength))
		{
			return;
		}
		this.Stick();
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		this.rb.transform.rotation = Quaternion.LookRotation(normal, this.rb.transform.up);
		Vector3 vector = this.rb.transform.position - point;
		vector -= Vector3.Dot(vector, normal) * normal;
		this.rb.transform.position = point + vector + this.surfaceOffsetDistance * normal;
	}

	// Token: 0x040014B7 RID: 5303
	[SerializeField]
	private MeshRenderer flatHand;

	// Token: 0x040014B8 RID: 5304
	[SerializeField]
	private MeshRenderer regularHand;

	// Token: 0x040014B9 RID: 5305
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x040014BA RID: 5306
	[SerializeField]
	private GameObject stringParent;

	// Token: 0x040014BB RID: 5307
	[SerializeField]
	private float surfaceOffsetDistance;

	// Token: 0x040014BC RID: 5308
	[SerializeField]
	private float stringMaxAttachLength;

	// Token: 0x040014BD RID: 5309
	[SerializeField]
	private float stringDetachLength;

	// Token: 0x040014BE RID: 5310
	[SerializeField]
	private float stringTeleportLength;

	// Token: 0x040014BF RID: 5311
	[SerializeField]
	private SoundBankPlayer thwackSound;

	// Token: 0x040014C0 RID: 5312
	[SerializeField]
	private SoundBankPlayer schlupSound;

	// Token: 0x040014C1 RID: 5313
	private VRRig myRig;

	// Token: 0x040014C2 RID: 5314
	private bool isLocal;

	// Token: 0x040014C3 RID: 5315
	private int stateBitIndex;

	// Token: 0x040014C4 RID: 5316
	private Vector3 defaultLocalPosition;
}
