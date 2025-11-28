using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06003C12 RID: 15378 RVA: 0x0013D15B File Offset: 0x0013B35B
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x06003C13 RID: 15379 RVA: 0x0013D178 File Offset: 0x0013B378
	private void Update()
	{
		if (this.explosionStartTime != 0f)
		{
			float num = (Time.time - this.explosionStartTime) / this.totalExplosionTime * (this.maxExplosionScale - 0.25f) + 0.25f;
			base.gameObject.transform.localScale = new Vector3(num, num, num);
			if (base.photonView.IsMine && Time.time > this.explosionStartTime + this.totalExplosionTime)
			{
				PhotonNetwork.Destroy(PhotonView.Get(this));
			}
		}
	}

	// Token: 0x06003C14 RID: 15380 RVA: 0x0013D200 File Offset: 0x0013B400
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x06003C15 RID: 15381 RVA: 0x0013D24C File Offset: 0x0013B44C
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06003C16 RID: 15382 RVA: 0x0013D25B File Offset: 0x0013B45B
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", 0, null);
		}
	}

	// Token: 0x06003C17 RID: 15383 RVA: 0x0013D284 File Offset: 0x0013B484
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06003C18 RID: 15384 RVA: 0x0013D2A4 File Offset: 0x0013B4A4
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (base.photonView.IsMine)
		{
			if ((bool)base.photonView.InstantiationData[0])
			{
				base.transform.parent = GorillaPlaySpace.Instance.myVRRig.leftHandTransform;
				return;
			}
			base.transform.parent = GorillaPlaySpace.Instance.myVRRig.rightHandTransform;
		}
	}

	// Token: 0x06003C19 RID: 15385 RVA: 0x0013D307 File Offset: 0x0013B507
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04004C98 RID: 19608
	public float maxExplosionScale;

	// Token: 0x04004C99 RID: 19609
	public float totalExplosionTime;

	// Token: 0x04004C9A RID: 19610
	public float gravityStrength;

	// Token: 0x04004C9B RID: 19611
	private bool canExplode;

	// Token: 0x04004C9C RID: 19612
	private float explosionStartTime;
}
