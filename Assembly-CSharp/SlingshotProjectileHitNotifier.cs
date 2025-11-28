using System;
using GorillaTag.GuidedRefs;
using UnityEngine;

// Token: 0x02000423 RID: 1059
public class SlingshotProjectileHitNotifier : BaseGuidedRefTargetMono
{
	// Token: 0x14000033 RID: 51
	// (add) Token: 0x06001A1F RID: 6687 RVA: 0x0008BA10 File Offset: 0x00089C10
	// (remove) Token: 0x06001A20 RID: 6688 RVA: 0x0008BA48 File Offset: 0x00089C48
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x06001A21 RID: 6689 RVA: 0x0008BA80 File Offset: 0x00089C80
	// (remove) Token: 0x06001A22 RID: 6690 RVA: 0x0008BAB8 File Offset: 0x00089CB8
	public event SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent OnPaperPlaneHit;

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x06001A23 RID: 6691 RVA: 0x0008BAF0 File Offset: 0x00089CF0
	// (remove) Token: 0x06001A24 RID: 6692 RVA: 0x0008BB28 File Offset: 0x00089D28
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06001A25 RID: 6693 RVA: 0x0008BB60 File Offset: 0x00089D60
	// (remove) Token: 0x06001A26 RID: 6694 RVA: 0x0008BB98 File Offset: 0x00089D98
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter;

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x06001A27 RID: 6695 RVA: 0x0008BBD0 File Offset: 0x00089DD0
	// (remove) Token: 0x06001A28 RID: 6696 RVA: 0x0008BC08 File Offset: 0x00089E08
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit;

	// Token: 0x06001A29 RID: 6697 RVA: 0x0008BC3D File Offset: 0x00089E3D
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x0008BC51 File Offset: 0x00089E51
	public void InvokeHit(PaperPlaneProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent onPaperPlaneHit = this.OnPaperPlaneHit;
		if (onPaperPlaneHit == null)
		{
			return;
		}
		onPaperPlaneHit(projectile, collider);
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x0008BC65 File Offset: 0x00089E65
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x0008BC79 File Offset: 0x00089E79
	public void InvokeTriggerEnter(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerEnter = this.OnProjectileTriggerEnter;
		if (onProjectileTriggerEnter == null)
		{
			return;
		}
		onProjectileTriggerEnter(projectile, collider);
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x0008BC8D File Offset: 0x00089E8D
	public void InvokeTriggerExit(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerExit = this.OnProjectileTriggerExit;
		if (onProjectileTriggerExit == null)
		{
			return;
		}
		onProjectileTriggerExit(projectile, collider);
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x0008BCA1 File Offset: 0x00089EA1
	private new void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
		this.OnProjectileTriggerEnter = null;
		this.OnProjectileTriggerExit = null;
	}

	// Token: 0x02000424 RID: 1060
	// (Invoke) Token: 0x06001A31 RID: 6705
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);

	// Token: 0x02000425 RID: 1061
	// (Invoke) Token: 0x06001A35 RID: 6709
	public delegate void PaperPlaneProjectileHitEvent(PaperPlaneProjectile projectile, Collider collider);

	// Token: 0x02000426 RID: 1062
	// (Invoke) Token: 0x06001A39 RID: 6713
	public delegate void ProjectileTriggerEvent(SlingshotProjectile projectile, Collider collider);
}
