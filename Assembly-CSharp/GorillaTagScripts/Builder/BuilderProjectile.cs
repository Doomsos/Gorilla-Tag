using System;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E5A RID: 3674
	public class BuilderProjectile : MonoBehaviour
	{
		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x06005BC1 RID: 23489 RVA: 0x001D72CC File Offset: 0x001D54CC
		// (set) Token: 0x06005BC2 RID: 23490 RVA: 0x001D72D4 File Offset: 0x001D54D4
		public Vector3 launchPosition { get; private set; }

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x06005BC3 RID: 23491 RVA: 0x001D72E0 File Offset: 0x001D54E0
		// (remove) Token: 0x06005BC4 RID: 23492 RVA: 0x001D7318 File Offset: 0x001D5518
		public event BuilderProjectile.ProjectileImpactEvent OnImpact;

		// Token: 0x06005BC5 RID: 23493 RVA: 0x001D7350 File Offset: 0x001D5550
		public void Launch(Vector3 position, Vector3 velocity, BuilderProjectileLauncher sourceObject, int projectileCount, float scale, int timeStamp)
		{
			this.particleLaunched = true;
			this.timeCreated = Time.time;
			this.projectileSource = sourceObject;
			float num = (NetworkSystem.Instance.ServerTimestamp - timeStamp) / 1000f;
			if (num >= this.lifeTime)
			{
				this.Deactivate();
				return;
			}
			this.timeCreated -= num;
			Vector3 vector = Vector3.ProjectOnPlane(velocity, Vector3.up);
			float num2 = 0.017453292f * Vector3.Angle(vector, velocity);
			float num3 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * 9.8f;
			Vector3 vector2 = num * Mathf.Cos(num2) * vector;
			float num4 = velocity.z * num * Mathf.Sin(num2) - 0.5f * num3 * num * num;
			this.launchPosition = position + vector2 + num4 * Vector3.down;
			Transform transform = base.transform;
			transform.position = position;
			transform.localScale = Vector3.one * scale;
			base.GetComponent<Collider>().contactOffset = 0.01f * scale;
			RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
			if (component != null)
			{
				component.objectRadiusForWaterCollision = 0.02f * scale;
			}
			this.projectileRigidbody.useGravity = false;
			Vector3 vector3 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * Physics.gravity;
			this.forceComponent.force = vector3;
			this.projectileRigidbody.linearVelocity = velocity + num * vector3;
			this.projectileId = projectileCount;
			this.projectileRigidbody.position = position;
			this.projectileSource.RegisterProjectile(this);
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x001D7511 File Offset: 0x001D5711
		protected void Awake()
		{
			this.projectileRigidbody = base.GetComponent<Rigidbody>();
			this.forceComponent = base.GetComponent<ConstantForce>();
			this.initialScale = base.transform.localScale.x;
		}

		// Token: 0x06005BC7 RID: 23495 RVA: 0x001D7544 File Offset: 0x001D5744
		public void Deactivate()
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			this.projectileRigidbody.useGravity = true;
			this.forceComponent.force = Vector3.zero;
			this.OnImpact = null;
			this.aoeKnockbackConfig = default(SlingshotProjectile.AOEKnockbackConfig?);
			this.impactSoundVolumeOverride = default(float?);
			this.impactSoundPitchOverride = default(float?);
			this.impactEffectScaleMultiplier = 1f;
			this.gravityMultiplier = 1f;
			ObjectPools.instance.Destroy(base.gameObject);
		}

		// Token: 0x06005BC8 RID: 23496 RVA: 0x001D75DC File Offset: 0x001D57DC
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 position2 = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
			Vector3 localScale = base.transform.localScale;
			gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(localScale.x * this.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
			}
		}

		// Token: 0x06005BC9 RID: 23497 RVA: 0x001D7684 File Offset: 0x001D5884
		public void ApplyHitKnockback(Vector3 hitNormal)
		{
			if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
			{
				Vector3 vector = Vector3.ProjectOnPlane(hitNormal, Vector3.up);
				vector.Normalize();
				Vector3 direction = 0.75f * vector + 0.25f * Vector3.up;
				direction.Normalize();
				GTPlayer instance = GTPlayer.Instance;
				instance.ApplyKnockback(direction, this.aoeKnockbackConfig.Value.knockbackVelocity, instance.scale < 0.9f);
			}
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x001D7714 File Offset: 0x001D5914
		private void OnEnable()
		{
			this.timeCreated = 0f;
			this.particleLaunched = false;
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x001D7728 File Offset: 0x001D5928
		protected void OnDisable()
		{
			this.particleLaunched = false;
			if (this.projectileSource != null)
			{
				this.projectileSource.UnRegisterProjectile(this);
			}
			this.projectileSource = null;
		}

		// Token: 0x06005BCC RID: 23500 RVA: 0x001D7754 File Offset: 0x001D5954
		public void UpdateProjectile()
		{
			if (this.particleLaunched)
			{
				if (Time.time > this.timeCreated + this.lifeTime)
				{
					this.Deactivate();
				}
				if (this.faceDirectionOfTravel)
				{
					Transform transform = base.transform;
					Vector3 position = transform.position;
					Vector3 vector = position - this.previousPosition;
					transform.rotation = ((vector.sqrMagnitude > 0f) ? Quaternion.LookRotation(vector) : transform.rotation);
					this.previousPosition = position;
				}
			}
		}

		// Token: 0x06005BCD RID: 23501 RVA: 0x001D77D0 File Offset: 0x001D59D0
		private void OnCollisionEnter(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06005BCE RID: 23502 RVA: 0x001D7888 File Offset: 0x001D5A88
		protected void OnCollisionStay(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06005BCF RID: 23503 RVA: 0x001D7940 File Offset: 0x001D5B40
		protected void OnTriggerEnter(Collider other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = (componentInParent != null) ? componentInParent.creator : null;
			if (netPlayer == null)
			{
				return;
			}
			if (netPlayer.IsLocal)
			{
				return;
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
			this.Deactivate();
		}

		// Token: 0x04006908 RID: 26888
		public BuilderProjectileLauncher projectileSource;

		// Token: 0x04006909 RID: 26889
		[Tooltip("Rotates to point along the Y axis after spawn.")]
		public GameObject surfaceImpactEffectPrefab;

		// Token: 0x0400690A RID: 26890
		[Tooltip("Distance from the surface that the particle should spawn.")]
		private float impactEffectOffset;

		// Token: 0x0400690B RID: 26891
		public float lifeTime = 20f;

		// Token: 0x0400690C RID: 26892
		public bool faceDirectionOfTravel = true;

		// Token: 0x0400690D RID: 26893
		private bool particleLaunched;

		// Token: 0x0400690E RID: 26894
		private float timeCreated;

		// Token: 0x04006910 RID: 26896
		private Rigidbody projectileRigidbody;

		// Token: 0x04006911 RID: 26897
		public int projectileId;

		// Token: 0x04006912 RID: 26898
		private float initialScale;

		// Token: 0x04006913 RID: 26899
		private Vector3 previousPosition;

		// Token: 0x04006914 RID: 26900
		[HideInInspector]
		public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

		// Token: 0x04006915 RID: 26901
		[HideInInspector]
		public float? impactSoundVolumeOverride;

		// Token: 0x04006916 RID: 26902
		[HideInInspector]
		public float? impactSoundPitchOverride;

		// Token: 0x04006917 RID: 26903
		[HideInInspector]
		public float impactEffectScaleMultiplier = 1f;

		// Token: 0x04006918 RID: 26904
		[HideInInspector]
		public float gravityMultiplier = 1f;

		// Token: 0x04006919 RID: 26905
		private ConstantForce forceComponent;

		// Token: 0x02000E5B RID: 3675
		// (Invoke) Token: 0x06005BD2 RID: 23506
		public delegate void ProjectileImpactEvent(BuilderProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
	}
}
