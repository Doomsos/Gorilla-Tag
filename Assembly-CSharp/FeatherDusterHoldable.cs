using System;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class FeatherDusterHoldable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000B51 RID: 2897 RVA: 0x0003D7AB File Offset: 0x0003B9AB
	public void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0003D7DB File Offset: 0x0003B9DB
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.lastWorldPos = base.transform.position;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0003D808 File Offset: 0x0003BA08
	public void SliceUpdate()
	{
		this.timeSinceLastSound += Time.deltaTime;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num = (position - this.lastWorldPos).sqrMagnitude / Time.deltaTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num >= this.collideMinSpeed * this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play();
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	// Token: 0x04000DD2 RID: 3538
	public LayerMask collisionLayer;

	// Token: 0x04000DD3 RID: 3539
	public float overlapSphereRadius = 0.08f;

	// Token: 0x04000DD4 RID: 3540
	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	// Token: 0x04000DD5 RID: 3541
	public ParticleSystem particleFx;

	// Token: 0x04000DD6 RID: 3542
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000DD7 RID: 3543
	[SerializeField]
	private float soundCooldown = 0.8f;

	// Token: 0x04000DD8 RID: 3544
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x04000DD9 RID: 3545
	private float initialRateOverTime;

	// Token: 0x04000DDA RID: 3546
	private float timeSinceLastSound;

	// Token: 0x04000DDB RID: 3547
	private Vector3 lastWorldPos;

	// Token: 0x04000DDC RID: 3548
	private Collider[] colliderResult = new Collider[1];
}
