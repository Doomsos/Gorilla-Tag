using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200019C RID: 412
public class SnowballThrowable : HoldableObject
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000B0D RID: 2829 RVA: 0x0003C1A2 File Offset: 0x0003A3A2
	// (set) Token: 0x06000B0E RID: 2830 RVA: 0x0003C1AA File Offset: 0x0003A3AA
	public XformOffset SpawnOffset
	{
		get
		{
			return this.spawnOffset;
		}
		set
		{
			this.spawnOffset = value;
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000B0F RID: 2831 RVA: 0x0003C1B3 File Offset: 0x0003A3B3
	internal int ProjectileHash
	{
		get
		{
			return PoolUtils.GameObjHashCode(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab);
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0003C1E0 File Offset: 0x0003A3E0
	protected virtual void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.awakeHasBeenCalled = true;
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.randModelIndex = -1;
		foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
		{
			if (randomProjectileThrowable != null)
			{
				RandomProjectileThrowable randomProjectileThrowable2 = randomProjectileThrowable;
				randomProjectileThrowable2.OnDestroyRandomProjectile = (UnityAction<bool>)Delegate.Combine(randomProjectileThrowable2.OnDestroyRandomProjectile, new UnityAction<bool>(this.HandleOnDestroyRandomProjectile));
			}
		}
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0003C2AC File Offset: 0x0003A4AC
	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0003C2CC File Offset: 0x0003A4CC
	public virtual void OnEnable()
	{
		if (this.targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.targetRig.isOfflineVRRig)
		{
			if (this.targetRig.netView != null && this.targetRig.netView.IsMine)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
			this.ApplyColor(throwableProjectileColor);
			if (this.randomModelSelection)
			{
				foreach (RandomProjectileThrowable randomProjectileThrowable in this.localModels)
				{
					randomProjectileThrowable.gameObject.SetActive(false);
				}
				this.randModelIndex = this.targetRig.GetRandomThrowableModelIndex();
				this.EnableRandomModel(this.randModelIndex, true);
			}
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnDisable()
	{
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00002789 File Offset: 0x00000989
	protected new virtual void OnDestroy()
	{
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0003C3DC File Offset: 0x0003A5DC
	public void SetSnowballActiveLocal(bool enabled)
	{
		if (!this.awakeHasBeenCalled)
		{
			this.Awake();
		}
		if (!this.OnEnableHasBeenCalled)
		{
			this.OnEnable();
		}
		if (this.isLeftHanded)
		{
			this.targetRig.LeftThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : -1);
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : -1);
		}
		bool flag = !base.gameObject.activeSelf && enabled;
		base.gameObject.SetActive(enabled);
		if (flag && this.pickupSoundBankPlayer != null)
		{
			this.pickupSoundBankPlayer.Play();
		}
		if (this.randomModelSelection)
		{
			if (enabled)
			{
				this.EnableRandomModel(this.GetRandomModelIndex(), true);
			}
			else
			{
				this.EnableRandomModel(this.randModelIndex, false);
			}
			this.targetRig.SetRandomThrowableModelIndex(this.randModelIndex);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(enabled ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = enabled ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white;
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0003C504 File Offset: 0x0003A704
	private int GetRandomModelIndex()
	{
		if (this.localModels.Count == 0)
		{
			return -1;
		}
		this.randModelIndex = Random.Range(0, this.localModels.Count);
		if ((float)Random.Range(1, 100) <= this.localModels[this.randModelIndex].spawnChance * 100f)
		{
			return this.randModelIndex;
		}
		return this.GetRandomModelIndex();
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0003C56C File Offset: 0x0003A76C
	private void EnableRandomModel(int index, bool enable)
	{
		if (this.randModelIndex >= 0 && this.randModelIndex < this.localModels.Count)
		{
			this.localModels[this.randModelIndex].gameObject.SetActive(enable);
			if (enable && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
			{
				this.destroyTimer = 0f;
			}
			return;
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0003C5E0 File Offset: 0x0003A7E0
	protected virtual void LateUpdateLocal()
	{
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].ForceDestroy)
		{
			this.localModels[this.randModelIndex].ForceDestroy = false;
			if (this.localModels[this.randModelIndex].gameObject.activeSelf)
			{
				this.PerformSnowballThrowAuthority();
			}
		}
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].autoDestroyAfterSeconds > 0f)
		{
			this.destroyTimer += Time.deltaTime;
			if (this.destroyTimer > this.localModels[this.randModelIndex].autoDestroyAfterSeconds)
			{
				if (this.localModels[this.randModelIndex].gameObject.activeSelf)
				{
					this.PerformSnowballThrowAuthority();
				}
				this.destroyTimer = -1f;
			}
		}
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x00002789 File Offset: 0x00000989
	protected void LateUpdateReplicated()
	{
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00002789 File Offset: 0x00000989
	protected void LateUpdateShared()
	{
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0003C6DF File Offset: 0x0003A8DF
	private Transform Anchor()
	{
		return base.transform.parent;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0003C6EC File Offset: 0x0003A8EC
	private void AnchorToHand()
	{
		BodyDockPositions myBodyDockPositions = this.targetRig.myBodyDockPositions;
		Transform transform = this.Anchor();
		if (this.isLeftHanded)
		{
			transform.parent = myBodyDockPositions.leftHandTransform;
		}
		else
		{
			transform.parent = myBodyDockPositions.rightHandTransform;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		base.transform.localPosition = this.spawnOffset.pos;
		base.transform.localRotation = this.spawnOffset.rot;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0003C770 File Offset: 0x0003A970
	protected void LateUpdate()
	{
		if (this.IsMine())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0003C78E File Offset: 0x0003A98E
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.OnSnowballRelease();
		return true;
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0003C7A3 File Offset: 0x0003A9A3
	protected virtual void OnSnowballRelease()
	{
		this.PerformSnowballThrowAuthority();
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0003C7AC File Offset: 0x0003A9AC
	protected virtual void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.linearVelocity;
		}
		Vector3 vector2 = this.velocityEstimator.linearVelocity - vector;
		float magnitude = vector2.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			vector2 *= num / magnitude;
		}
		Vector3 velocity = vector2 + vector;
		Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, velocity, x, this.randomizeColor, throwableProjectileColor);
		this.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count)
		{
			if (this.localModels[this.randModelIndex].ForceDestroy || this.localModels[this.randModelIndex].destroyAfterRelease)
			{
				slingshotProjectile.DestroyAfterRelease();
			}
			else if (this.localModels[this.randModelIndex].moveOverPassedLifeTime)
			{
				float num2 = Time.time - this.localModels[this.randModelIndex].TimeEnabled;
				float remainingLifeTime = slingshotProjectile.GetRemainingLifeTime();
				if (remainingLifeTime > num2)
				{
					float newLifeTime = remainingLifeTime - num2;
					slingshotProjectile.UpdateRemainingLifeTime(newLifeTime);
				}
				else
				{
					slingshotProjectile.UpdateRemainingLifeTime(0f);
				}
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			RoomSystem.SendLaunchProjectile(position, velocity, this.isLeftHanded ? RoomSystem.ProjectileSource.LeftHand : RoomSystem.ProjectileSource.RightHand, slingshotProjectile.myProjectileCount, this.randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0003C9B4 File Offset: 0x0003ABB4
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomColour, Color colour)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(component, velocity, location, scale);
		component.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileCount, scale, randomColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		component.OnImpact += this.OnProjectileImpact;
		return component;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0003CA58 File Offset: 0x0003AC58
	protected virtual SlingshotProjectile SpawnProjectile()
	{
		return ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].GetProjectilePrefab() : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0003CA90 File Offset: 0x0003AC90
	protected virtual void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		if (hitPlayer != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
			{
				instance.OnWaterBalloonHitPlayer(hitPlayer);
			}
		}
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0003CADC File Offset: 0x0003ACDC
	private void ApplyColor(Color newColor)
	{
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty(ShaderProps._BaseColor))
						{
							material.SetColor(ShaderProps._BaseColor, newColor);
						}
						if (material.HasProperty(ShaderProps._Color))
						{
							material.SetColor(ShaderProps._Color, newColor);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x0003CB69 File Offset: 0x0003AD69
	public override void DropItemCleanup()
	{
		if (base.gameObject.activeSelf)
		{
			this.OnSnowballRelease();
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x0003CB7E File Offset: 0x0003AD7E
	private void HandleOnDestroyRandomProjectile(bool enable)
	{
		this.SetSnowballActiveLocal(enable);
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0003CB88 File Offset: 0x0003AD88
	public SnowballThrowable()
	{
		List<int> list = new List<int>();
		list.Add(32);
		this.matDataIndexes = list;
		this.linSpeedMultiplier = 1f;
		this.maxLinSpeed = 12f;
		this.randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);
		this.destroyTimer = -1f;
		base..ctor();
	}

	// Token: 0x04000D77 RID: 3447
	[GorillaSoundLookup]
	public List<int> matDataIndexes;

	// Token: 0x04000D78 RID: 3448
	[Tooltip("prefab to spawn from global object pools when thrown")]
	public GameObject projectilePrefab;

	// Token: 0x04000D79 RID: 3449
	public SoundBankPlayer pickupSoundBankPlayer;

	// Token: 0x04000D7A RID: 3450
	public bool isLeftHanded;

	// Token: 0x04000D7B RID: 3451
	[Tooltip("This needs to match the index of the projectilePrefab on the Local Gorilla Player's BodyDockPositions LeftHandThrowables or RightHandThrowables list\nCheck the array in play mode to find the index")]
	public int throwableMakerIndex;

	// Token: 0x04000D7C RID: 3452
	[Tooltip("Multiplier is applied to hand speed to get launch speed of the projectile")]
	public float linSpeedMultiplier;

	// Token: 0x04000D7D RID: 3453
	[Tooltip("Maximum launch speed of the projectile")]
	public float maxLinSpeed;

	// Token: 0x04000D7E RID: 3454
	[Space]
	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	// Token: 0x04000D7F RID: 3455
	public GTColor.HSVRanges randomColorHSVRanges;

	// Token: 0x04000D80 RID: 3456
	[Tooltip("Check this part only if we want to randomize the prefab meshes and projectile")]
	public bool randomModelSelection;

	// Token: 0x04000D81 RID: 3457
	public List<RandomProjectileThrowable> localModels;

	// Token: 0x04000D82 RID: 3458
	[Tooltip("projectile identifier sent out by the PlayerGameEvents.LaunchedProjectile event. Uses prefab name if empty")]
	public string throwEventName;

	// Token: 0x04000D83 RID: 3459
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000D84 RID: 3460
	protected VRRig targetRig;

	// Token: 0x04000D85 RID: 3461
	protected bool isOfflineRig;

	// Token: 0x04000D86 RID: 3462
	private bool awakeHasBeenCalled;

	// Token: 0x04000D87 RID: 3463
	private bool OnEnableHasBeenCalled;

	// Token: 0x04000D88 RID: 3464
	private Renderer[] renderers;

	// Token: 0x04000D89 RID: 3465
	protected int randModelIndex;

	// Token: 0x04000D8A RID: 3466
	private float destroyTimer;

	// Token: 0x04000D8B RID: 3467
	private XformOffset spawnOffset;
}
