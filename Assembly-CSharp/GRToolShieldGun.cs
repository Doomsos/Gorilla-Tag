using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000730 RID: 1840
public class GRToolShieldGun : MonoBehaviour
{
	// Token: 0x06002F6E RID: 12142 RVA: 0x00101DE1 File Offset: 0x000FFFE1
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x00101E14 File Offset: 0x00100014
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength1))
		{
			this.firingSound = this.upgrade1FiringSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength2))
		{
			this.firingSound = this.upgrade2FiringSound;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength3))
		{
			this.firingSound = this.upgrade3FiringSound;
		}
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x00101E65 File Offset: 0x00100065
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x00101E80 File Offset: 0x00100080
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x00101EB4 File Offset: 0x001000B4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolShieldGun.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Firing);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolShieldGun.State.Firing:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolShieldGun.State.Cooldown);
				return;
			}
			break;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolShieldGun.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x00101F9C File Offset: 0x0010019C
	private void OnUpdateRemote(float dt)
	{
		GRToolShieldGun.State state = (GRToolShieldGun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetStateAuthority(state);
		}
	}

	// Token: 0x06002F74 RID: 12148 RVA: 0x00101FC6 File Offset: 0x001001C6
	private void SetStateAuthority(GRToolShieldGun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x00101FE8 File Offset: 0x001001E8
	private void SetState(GRToolShieldGun.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolShieldGun.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolShieldGun.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolShieldGun.State.Firing:
			this.StartFiring();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolShieldGun.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x0010206C File Offset: 0x0010026C
	private void StartCharge()
	{
		if (this.chargeSound != null)
		{
			this.audioSource.PlayOneShot(this.chargeSound, this.chargeSoundVolume);
		}
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x001020BC File Offset: 0x001002BC
	private void StartFiring()
	{
		if (this.firingSound != null)
		{
			this.audioSource.PlayOneShot(this.firingSound, this.firingSoundVolume);
		}
		this.timeLastFired = Time.time;
		this.tool.UseEnergy();
		Vector3 position = this.firingTransform.position;
		Vector3 velocity = this.firingTransform.forward * this.projectileSpeed;
		float scale = GTPlayer.Instance.scale;
		int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		this.firedProjectile = ObjectPools.instance.Instantiate(hash, true).GetComponent<SlingshotProjectile>();
		this.firedProjectile.transform.localScale = Vector3.one * scale;
		if (this.projectileTrailPrefab != null)
		{
			int trailHash = PoolUtils.GameObjHashCode(this.projectileTrailPrefab);
			this.AttachTrail(trailHash, this.firedProjectile.gameObject, position, false, false);
		}
		Collider component = this.firedProjectile.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			for (int i = 0; i < this.colliders.Count; i++)
			{
				Physics.IgnoreCollision(this.colliders[i], component);
			}
		}
		if (this.IsHeldLocal())
		{
			this.firedProjectile.OnImpact += this.OnProjectileImpact;
		}
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		this.firedProjectile.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, 1, scale, true, this.projectileColor);
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x00102240 File Offset: 0x00100440
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam, false, default(Color));
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x00102294 File Offset: 0x00100494
	private void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		projectile.OnImpact -= this.OnProjectileImpact;
		GRPlayer grplayer = null;
		RigContainer rigContainer;
		if (hitPlayer != null && VRRigCache.Instance.TryGetVrrig(hitPlayer, out rigContainer) && rigContainer.Rig != null)
		{
			grplayer = rigContainer.Rig.GetComponent<GRPlayer>();
		}
		else if (this.allowAoeHits)
		{
			GRToolShieldGun.vrRigs.Clear();
			GRToolShieldGun.vrRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GRToolShieldGun.vrRigs);
			VRRig vrrig = null;
			float num = float.MaxValue;
			for (int i = 0; i < GRToolShieldGun.vrRigs.Count; i++)
			{
				float sqrMagnitude = (GRToolShieldGun.vrRigs[i].bodyTransform.position - impactPos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vrrig = GRToolShieldGun.vrRigs[i];
				}
			}
			if (vrrig != null)
			{
				grplayer = vrrig.GetComponent<GRPlayer>();
			}
		}
		if (grplayer != null)
		{
			int num2 = 0;
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength1))
			{
				num2 |= 1;
			}
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength2))
			{
				num2 |= 2;
			}
			if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.ShieldGunStrength3))
			{
				num2 |= 4;
			}
			this.gameEntity.manager.ghostReactorManager.RequestGrantPlayerShield(grplayer, this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ShieldSize), num2);
		}
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x001023F8 File Offset: 0x001005F8
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? 4 : 5) > 0.25f;
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x00102458 File Offset: 0x00100658
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x001024AC File Offset: 0x001006AC
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFired + this.cooldownMinimum);
	}

	// Token: 0x04003E01 RID: 15873
	public GameEntity gameEntity;

	// Token: 0x04003E02 RID: 15874
	public GRTool tool;

	// Token: 0x04003E03 RID: 15875
	public GRAttributes attributes;

	// Token: 0x04003E04 RID: 15876
	public GameObject projectilePrefab;

	// Token: 0x04003E05 RID: 15877
	public GameObject projectileTrailPrefab;

	// Token: 0x04003E06 RID: 15878
	public Transform firingTransform;

	// Token: 0x04003E07 RID: 15879
	public List<Collider> colliders;

	// Token: 0x04003E08 RID: 15880
	public float projectileSpeed = 25f;

	// Token: 0x04003E09 RID: 15881
	public Color projectileColor = new Color(0.25f, 0.25f, 1f);

	// Token: 0x04003E0A RID: 15882
	public bool allowAoeHits;

	// Token: 0x04003E0B RID: 15883
	public float aeoHitRadius = 0.5f;

	// Token: 0x04003E0C RID: 15884
	public float chargeDuration = 0.75f;

	// Token: 0x04003E0D RID: 15885
	public float flashDuration = 0.1f;

	// Token: 0x04003E0E RID: 15886
	public float cooldownDuration;

	// Token: 0x04003E0F RID: 15887
	public AudioSource audioSource;

	// Token: 0x04003E10 RID: 15888
	public AudioClip chargeSound;

	// Token: 0x04003E11 RID: 15889
	public float chargeSoundVolume = 0.5f;

	// Token: 0x04003E12 RID: 15890
	public AudioClip firingSound;

	// Token: 0x04003E13 RID: 15891
	public float firingSoundVolume = 0.5f;

	// Token: 0x04003E14 RID: 15892
	public AudioClip upgrade1FiringSound;

	// Token: 0x04003E15 RID: 15893
	public AudioClip upgrade2FiringSound;

	// Token: 0x04003E16 RID: 15894
	public AudioClip upgrade3FiringSound;

	// Token: 0x04003E17 RID: 15895
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x04003E18 RID: 15896
	private GRToolShieldGun.State state;

	// Token: 0x04003E19 RID: 15897
	private float stateTimeRemaining;

	// Token: 0x04003E1A RID: 15898
	private bool activatedLocally;

	// Token: 0x04003E1B RID: 15899
	private bool waitingForButtonRelease;

	// Token: 0x04003E1C RID: 15900
	private float timeLastFired;

	// Token: 0x04003E1D RID: 15901
	private float cooldownMinimum = 0.35f;

	// Token: 0x04003E1E RID: 15902
	private SlingshotProjectile firedProjectile;

	// Token: 0x04003E1F RID: 15903
	private static List<VRRig> vrRigs = new List<VRRig>(10);

	// Token: 0x02000731 RID: 1841
	private enum State
	{
		// Token: 0x04003E21 RID: 15905
		Idle,
		// Token: 0x04003E22 RID: 15906
		Charging,
		// Token: 0x04003E23 RID: 15907
		Firing,
		// Token: 0x04003E24 RID: 15908
		Cooldown,
		// Token: 0x04003E25 RID: 15909
		Count
	}
}
