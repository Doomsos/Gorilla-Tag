using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200041F RID: 1055
public class SlingshotProjectile : MonoBehaviour
{
	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06001A02 RID: 6658 RVA: 0x0008ACDC File Offset: 0x00088EDC
	// (set) Token: 0x06001A03 RID: 6659 RVA: 0x0008ACE4 File Offset: 0x00088EE4
	public Vector3 launchPosition { get; private set; }

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06001A04 RID: 6660 RVA: 0x0008ACF0 File Offset: 0x00088EF0
	// (remove) Token: 0x06001A05 RID: 6661 RVA: 0x0008AD28 File Offset: 0x00088F28
	public event SlingshotProjectile.ProjectileImpactEvent OnImpact;

	// Token: 0x06001A06 RID: 6662 RVA: 0x0008AD60 File Offset: 0x00088F60
	public void Launch(Vector3 position, Vector3 velocity, NetPlayer player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (this.launchSoundBankPlayer != null)
		{
			this.launchSoundBankPlayer.Play();
		}
		this.particleLaunched = true;
		this.timeCreated = Time.time;
		this.launchPosition = position;
		Transform transform = base.transform;
		transform.position = position;
		transform.localScale = Vector3.one * scale;
		base.GetComponent<Collider>().contactOffset = 0.01f * scale;
		RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
		if (component != null)
		{
			component.objectRadiusForWaterCollision = 0.02f * scale;
		}
		this.projectileRigidbody.isKinematic = false;
		this.projectileRigidbody.useGravity = false;
		this.forceComponent.enabled = true;
		this.forceComponent.force = Physics.gravity * this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f);
		this.projectileRigidbody.linearVelocity = velocity;
		this.projectileOwner = player;
		this.myProjectileCount = projectileCount;
		this.projectileRigidbody.position = position;
		this.ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
		this.remainingLifeTime = this.lifeTime;
		if (this.forceComponent)
		{
			this.forceComponent.enabled = true;
			this.forceComponent.force = Physics.gravity * this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f);
			if (this.useForwardForce)
			{
				this.forceComponent.force += this.projectileRigidbody.linearVelocity.normalized * this.forwardForceMultiplier;
			}
		}
		this.isSettled = false;
		UnityEvent<NetPlayer> onLaunch = this.OnLaunch;
		if (onLaunch == null)
		{
			return;
		}
		onLaunch.Invoke(this.projectileOwner);
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x0008AF4C File Offset: 0x0008914C
	protected void Awake()
	{
		if (this.playerImpactEffectPrefab == null)
		{
			this.playerImpactEffectPrefab = this.surfaceImpactEffectPrefab;
		}
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
		this.remainingLifeTime = this.lifeTime;
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x0008AFC4 File Offset: 0x000891C4
	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * this.initialScale;
		this.projectileRigidbody.useGravity = true;
		if (this.forceComponent)
		{
			this.forceComponent.force = Vector3.zero;
		}
		this.OnImpact = null;
		this.aoeKnockbackConfig = default(SlingshotProjectile.AOEKnockbackConfig?);
		this.impactSoundVolumeOverride = default(float?);
		this.impactSoundPitchOverride = default(float?);
		this.impactEffectScaleMultiplier = 1f;
		this.projectileRigidbody.isKinematic = false;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x0008B068 File Offset: 0x00089268
	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		if (prefab == null)
		{
			return;
		}
		Vector3 position2 = position + normal * this.impactEffectOffset;
		GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
		Vector3 localScale = base.transform.localScale;
		gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
		gameObject.transform.up = normal;
		GorillaColorizableBase component = gameObject.GetComponent<GorillaColorizableBase>();
		if (component != null)
		{
			component.SetColor(this.teamColor);
		}
		SurfaceImpactFX component2 = gameObject.GetComponent<SurfaceImpactFX>();
		if (component2 != null)
		{
			component2.SetScale(localScale.x * this.impactEffectScaleMultiplier);
		}
		SoundBankPlayer component3 = gameObject.GetComponent<SoundBankPlayer>();
		if (component3 != null && !component3.playOnEnable)
		{
			component3.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
		}
		if (this.spawnWorldEffects != null)
		{
			this.spawnWorldEffects.RequestSpawn(position, normal);
		}
		UnityEvent<Vector3> onImapctEvent = this.OnImapctEvent;
		if (onImapctEvent == null)
		{
			return;
		}
		onImapctEvent.Invoke(position);
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x0008B168 File Offset: 0x00089368
	public void CheckForAOEKnockback(Vector3 impactPosition, float impactSpeed)
	{
		if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
		{
			Vector3 vector = GTPlayer.Instance.HeadCenterPosition - impactPosition;
			if (vector.sqrMagnitude < this.aoeKnockbackConfig.Value.aeoOuterRadius * this.aoeKnockbackConfig.Value.aeoOuterRadius)
			{
				float magnitude = vector.magnitude;
				Vector3 direction = (magnitude > 0.001f) ? (vector / magnitude) : Vector3.up;
				float num = Mathf.InverseLerp(this.aoeKnockbackConfig.Value.aeoOuterRadius, this.aoeKnockbackConfig.Value.aeoInnerRadius, magnitude);
				float num2 = Mathf.InverseLerp(0f, this.aoeKnockbackConfig.Value.impactVelocityThreshold, impactSpeed);
				GTPlayer.Instance.ApplyKnockback(direction, this.aoeKnockbackConfig.Value.knockbackVelocity * num * num2, false);
				this.impactEffectScaleMultiplier = Mathf.Lerp(1f, this.impactEffectScaleMultiplier, num2);
				if (this.impactSoundVolumeOverride != null)
				{
					this.impactSoundVolumeOverride = new float?(Mathf.Lerp(this.impactSoundVolumeOverride.Value * 0.5f, this.impactSoundVolumeOverride.Value, num2));
				}
				float num3 = Mathf.Lerp(this.aoeKnockbackConfig.Value.aeoInnerRadius, this.aoeKnockbackConfig.Value.aeoOuterRadius, 0.25f);
				if (this.aoeKnockbackConfig.Value.playerProximityEffect != PlayerEffect.NONE && vector.sqrMagnitude < num3 * num3)
				{
					RoomSystem.SendPlayerEffect(PlayerEffect.SNOWBALL_IMPACT, NetworkSystem.Instance.LocalPlayer);
				}
			}
		}
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x0008B30C File Offset: 0x0008950C
	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			this.teamColor = overrideColor;
		}
		else
		{
			this.teamColor = (blueTeam ? this.blueColor : (orangeTeam ? this.orangeColor : this.defaultColor));
		}
		this.blueBall.enabled = blueTeam;
		this.orangeBall.enabled = orangeTeam;
		this.defaultBall.enabled = (!blueTeam && !orangeTeam);
		this.teamRenderer = (blueTeam ? this.blueBall : (orangeTeam ? this.orangeBall : this.defaultBall));
		this.ApplyColor(this.teamRenderer, (this.colorizeBalls || shouldOverrideColor) ? this.teamColor : Color.white);
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x0008B3BA File Offset: 0x000895BA
	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
		SlingshotProjectileManager.RegisterSP(this);
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x0008B3D4 File Offset: 0x000895D4
	protected void OnDisable()
	{
		this.particleLaunched = false;
		SlingshotProjectileManager.UnregisterSP(this);
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x0008B3E4 File Offset: 0x000895E4
	public void InvokeUpdate()
	{
		if (this.particleLaunched || this.dontDestroyOnHit)
		{
			if (Time.time > this.timeCreated + this.GetRemainingLifeTime())
			{
				this.DestroyAfterRelease();
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
		if (this.dontDestroyOnHit)
		{
			this.SettleProjectile();
		}
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x0008B475 File Offset: 0x00089675
	public void DestroyAfterRelease()
	{
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
		this.Deactivate();
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x0008B499 File Offset: 0x00089699
	public float GetRemainingLifeTime()
	{
		return this.remainingLifeTime;
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x0008B4A1 File Offset: 0x000896A1
	public void UpdateRemainingLifeTime(float newLifeTime)
	{
		this.remainingLifeTime = newLifeTime;
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x0008B4AC File Offset: 0x000896AC
	public float GetDistanceTraveled()
	{
		return (base.transform.position - this.launchPosition).magnitude;
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x0008B4D8 File Offset: 0x000896D8
	private void SettleProjectile()
	{
		if (!this.isSettled)
		{
			int value = this.floorLayerMask.value;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, ref raycastHit, 0.1f, value, 1) && Vector3.Angle(raycastHit.normal, Vector3.up) < 40f)
			{
				if (this.forceComponent)
				{
					this.forceComponent.force = Vector3.zero;
				}
				this.projectileRigidbody.angularVelocity = Vector3.zero;
				this.projectileRigidbody.linearVelocity = Vector3.zero;
				this.projectileRigidbody.isKinematic = true;
				base.transform.position = raycastHit.point + Vector3.up * this.placementOffset;
				this.isSettled = true;
				return;
			}
		}
		else if (this.keepRotationUpright)
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.up, Vector3.up).normalized, Vector3.up);
			base.transform.rotation = rotation;
		}
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x0008B5F0 File Offset: 0x000897F0
	protected void OnCollisionEnter(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.collider.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(ref slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeHit(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x0008B688 File Offset: 0x00089888
	protected void OnCollisionStay(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(ref slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeCollisionStay(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x0008B71C File Offset: 0x0008991C
	protected void OnTriggerExit(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(ref slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerExit(this, other);
		}
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x0008B74C File Offset: 0x0008994C
	protected void OnTriggerEnter(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(ref slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerEnter(this, other);
		}
		if (this.projectileOwner == NetworkSystem.Instance.LocalPlayer)
		{
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = (componentInParent != null) ? componentInParent.creator : null;
			if (netPlayer == null)
			{
				return;
			}
			SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, base.transform.position, netPlayer);
			}
			if (NetworkSystem.Instance.LocalPlayer == netPlayer)
			{
				return;
			}
			if (component && !component.LocalCanHit(NetworkSystem.Instance.LocalPlayer, netPlayer))
			{
				return;
			}
			if (component && GameMode.ActiveNetworkHandler)
			{
				GameMode.ActiveNetworkHandler.SendRPC("RPC_ReportSlingshotHit", false, new object[]
				{
					(netPlayer as PunNetPlayer).PlayerRef,
					base.transform.position,
					this.myProjectileCount
				});
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
			if (this.m_sendNetworkedImpact)
			{
				RoomSystem.SendImpactEffect(base.transform.position, this.teamColor.r, this.teamColor.g, this.teamColor.b, this.teamColor.a, this.myProjectileCount);
			}
			this.Deactivate();
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		VRRig vrrig;
		if (attachedRigidbody.IsNotNull() && attachedRigidbody.gameObject.TryGetComponent<VRRig>(ref vrrig))
		{
			UnityEvent<VRRig> onHitPlayer = this.OnHitPlayer;
			if (onHitPlayer == null)
			{
				return;
			}
			onHitPlayer.Invoke(vrrig);
		}
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x0008B919 File Offset: 0x00089B19
	private void ApplyColor(Renderer rend, Color color)
	{
		if (!rend)
		{
			return;
		}
		this.matPropBlock.SetColor(ShaderProps._BaseColor, color);
		this.matPropBlock.SetColor(ShaderProps._Color, color);
		rend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x0400238C RID: 9100
	public NetPlayer projectileOwner;

	// Token: 0x0400238D RID: 9101
	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	// Token: 0x0400238E RID: 9102
	[Tooltip("if left empty, the default player impact that is set in Room System Setting will be played")]
	public GameObject playerImpactEffectPrefab;

	// Token: 0x0400238F RID: 9103
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x04002390 RID: 9104
	[SerializeField]
	private SoundBankPlayer launchSoundBankPlayer;

	// Token: 0x04002391 RID: 9105
	[SerializeField]
	private bool dontDestroyOnHit;

	// Token: 0x04002392 RID: 9106
	[SerializeField]
	private LayerMask floorLayerMask;

	// Token: 0x04002393 RID: 9107
	[SerializeField]
	private float placementOffset = 0.01f;

	// Token: 0x04002394 RID: 9108
	[SerializeField]
	private bool keepRotationUpright = true;

	// Token: 0x04002395 RID: 9109
	public float lifeTime = 20f;

	// Token: 0x04002396 RID: 9110
	public float gravityMultiplier = 1f;

	// Token: 0x04002397 RID: 9111
	public bool useForwardForce;

	// Token: 0x04002398 RID: 9112
	public float forwardForceMultiplier = 0.1f;

	// Token: 0x04002399 RID: 9113
	public Color defaultColor = Color.white;

	// Token: 0x0400239A RID: 9114
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x0400239B RID: 9115
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x0400239C RID: 9116
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	// Token: 0x0400239D RID: 9117
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	// Token: 0x0400239E RID: 9118
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	// Token: 0x0400239F RID: 9119
	public bool colorizeBalls;

	// Token: 0x040023A0 RID: 9120
	public bool faceDirectionOfTravel = true;

	// Token: 0x040023A1 RID: 9121
	private bool particleLaunched;

	// Token: 0x040023A2 RID: 9122
	private float timeCreated;

	// Token: 0x040023A4 RID: 9124
	private Rigidbody projectileRigidbody;

	// Token: 0x040023A5 RID: 9125
	private Color teamColor = Color.white;

	// Token: 0x040023A6 RID: 9126
	private Renderer teamRenderer;

	// Token: 0x040023A7 RID: 9127
	public int myProjectileCount;

	// Token: 0x040023A8 RID: 9128
	private float initialScale;

	// Token: 0x040023A9 RID: 9129
	private Vector3 previousPosition;

	// Token: 0x040023AA RID: 9130
	[HideInInspector]
	public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

	// Token: 0x040023AB RID: 9131
	[HideInInspector]
	public float? impactSoundVolumeOverride;

	// Token: 0x040023AC RID: 9132
	[HideInInspector]
	public float? impactSoundPitchOverride;

	// Token: 0x040023AD RID: 9133
	[HideInInspector]
	public float impactEffectScaleMultiplier = 1f;

	// Token: 0x040023AE RID: 9134
	private ConstantForce forceComponent;

	// Token: 0x040023AF RID: 9135
	public bool m_sendNetworkedImpact = true;

	// Token: 0x040023B1 RID: 9137
	public UnityEvent<NetPlayer> OnLaunch;

	// Token: 0x040023B2 RID: 9138
	public UnityEvent<Vector3> OnImapctEvent;

	// Token: 0x040023B3 RID: 9139
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040023B4 RID: 9140
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x040023B5 RID: 9141
	public UnityEvent<VRRig> OnHitPlayer;

	// Token: 0x040023B6 RID: 9142
	private float remainingLifeTime;

	// Token: 0x040023B7 RID: 9143
	private bool isSettled;

	// Token: 0x040023B8 RID: 9144
	private float distanceTraveled;

	// Token: 0x02000420 RID: 1056
	[Serializable]
	public struct AOEKnockbackConfig
	{
		// Token: 0x040023B9 RID: 9145
		public bool applyAOEKnockback;

		// Token: 0x040023BA RID: 9146
		[Tooltip("Full knockback velocity is imparted within the inner radius")]
		public float aeoInnerRadius;

		// Token: 0x040023BB RID: 9147
		[Tooltip("Partial knockback velocity is imparted between the inner and outer radius")]
		public float aeoOuterRadius;

		// Token: 0x040023BC RID: 9148
		public float knockbackVelocity;

		// Token: 0x040023BD RID: 9149
		[Tooltip("The required impact velocity to achieve full knockback velocity")]
		public float impactVelocityThreshold;

		// Token: 0x040023BE RID: 9150
		[SerializeField]
		public PlayerEffect playerProximityEffect;
	}

	// Token: 0x02000421 RID: 1057
	// (Invoke) Token: 0x06001A1B RID: 6683
	public delegate void ProjectileImpactEvent(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
}
