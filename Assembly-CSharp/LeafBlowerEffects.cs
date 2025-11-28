using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class LeafBlowerEffects : MonoBehaviour, ISpawnable
{
	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06001000 RID: 4096 RVA: 0x00054441 File Offset: 0x00052641
	// (set) Token: 0x06001001 RID: 4097 RVA: 0x00054449 File Offset: 0x00052649
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06001002 RID: 4098 RVA: 0x00054452 File Offset: 0x00052652
	// (set) Token: 0x06001003 RID: 4099 RVA: 0x0005445A File Offset: 0x0005265A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001004 RID: 4100 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x00054464 File Offset: 0x00052664
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.headToleranceAngleCos = Mathf.Cos(0.017453292f * this.headToleranceAngle);
		this.squareHitAngleCos = Mathf.Cos(0.017453292f * this.squareHitAngle);
		this.fan = rig.cosmeticReferences.Get(this.fanRef).GetComponent<CosmeticFan>();
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x000544BB File Offset: 0x000526BB
	public void StartFan()
	{
		this.fan.Run();
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x000544C8 File Offset: 0x000526C8
	public void StopFan()
	{
		this.fan.Stop();
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x000544D5 File Offset: 0x000526D5
	public void UpdateEffects()
	{
		this.ProjectParticles();
		this.BlowFaces();
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x000544E4 File Offset: 0x000526E4
	public void ProjectParticles()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.gunBarrel.transform.position, this.gunBarrel.transform.forward, ref raycastHit, this.projectionRange, this.raycastLayers))
		{
			SpawnOnEnter component = raycastHit.collider.GetComponent<SpawnOnEnter>();
			if (component != null)
			{
				component.OnTriggerEnter(raycastHit.collider);
			}
			if (Vector3.Dot(raycastHit.normal, this.gunBarrel.transform.forward) < -this.squareHitAngleCos)
			{
				this.squareHitParticleSystem.transform.position = raycastHit.point;
				this.squareHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Stop(true, 1);
				}
				if (!this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Play(true);
					return;
				}
			}
			else
			{
				this.angledHitParticleSystem.transform.position = raycastHit.point;
				this.angledHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Stop(true, 1);
				}
				if (!this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Play(true);
					return;
				}
			}
		}
		else
		{
			this.StopEffects();
		}
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00054696 File Offset: 0x00052896
	public void StopEffects()
	{
		this.angledHitParticleSystem.Stop(true, 1);
		this.squareHitParticleSystem.Stop(true, 1);
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x000546B4 File Offset: 0x000528B4
	public void BlowFaces()
	{
		Vector3 position = this.gunBarrel.transform.position;
		Vector3 forward = this.gunBarrel.transform.forward;
		if (NetworkSystem.Instance.InRoom)
		{
			using (List<VRRig>.Enumerator enumerator = GorillaParent.instance.vrrigs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VRRig rig = enumerator.Current;
					this.TryBlowFace(rig, position, forward);
				}
				return;
			}
		}
		this.TryBlowFace(VRRig.LocalRig, position, forward);
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x0005474C File Offset: 0x0005294C
	private void TryBlowFace(VRRig rig, Vector3 origin, Vector3 directionNormalized)
	{
		Transform rigTarget = rig.head.rigTarget;
		Vector3 vector = rigTarget.position - origin;
		float num = Vector3.Dot(vector, directionNormalized);
		if (num < 0f || num > this.projectionRange)
		{
			return;
		}
		if ((vector - num * directionNormalized).IsLongerThan(this.projectionWidth))
		{
			return;
		}
		if (Vector3.Dot(-rigTarget.forward, vector.normalized) < this.headToleranceAngleCos)
		{
			return;
		}
		rig.GetComponent<GorillaMouthFlap>().EnableLeafBlower();
	}

	// Token: 0x040013F1 RID: 5105
	[SerializeField]
	private GameObject gunBarrel;

	// Token: 0x040013F2 RID: 5106
	[SerializeField]
	private float projectionRange;

	// Token: 0x040013F3 RID: 5107
	[SerializeField]
	private float projectionWidth;

	// Token: 0x040013F4 RID: 5108
	[SerializeField]
	private float headToleranceAngle;

	// Token: 0x040013F5 RID: 5109
	[SerializeField]
	private LayerMask raycastLayers;

	// Token: 0x040013F6 RID: 5110
	[SerializeField]
	private ParticleSystem angledHitParticleSystem;

	// Token: 0x040013F7 RID: 5111
	[SerializeField]
	private ParticleSystem squareHitParticleSystem;

	// Token: 0x040013F8 RID: 5112
	[SerializeField]
	private float squareHitAngle;

	// Token: 0x040013F9 RID: 5113
	[SerializeField]
	private CosmeticRefID fanRef;

	// Token: 0x040013FA RID: 5114
	private float headToleranceAngleCos;

	// Token: 0x040013FB RID: 5115
	private float squareHitAngleCos;

	// Token: 0x040013FC RID: 5116
	private CosmeticFan fan;
}
