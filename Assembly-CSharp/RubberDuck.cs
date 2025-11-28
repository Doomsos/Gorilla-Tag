using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004F2 RID: 1266
public class RubberDuck : TransferrableObject
{
	// Token: 0x1700036D RID: 877
	// (get) Token: 0x06002084 RID: 8324 RVA: 0x000ACB73 File Offset: 0x000AAD73
	// (set) Token: 0x06002085 RID: 8325 RVA: 0x000ACB85 File Offset: 0x000AAD85
	public bool fxActive
	{
		get
		{
			return this.hasParticleFX && this._fxActive;
		}
		set
		{
			if (!this.hasParticleFX)
			{
				return;
			}
			this.pFXEmissionModule.enabled = value;
			this._fxActive = value;
		}
	}

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06002086 RID: 8326 RVA: 0x000ACBA3 File Offset: 0x000AADA3
	public int SqueezeSound
	{
		get
		{
			if (this.squeezeSoundBank.Length > 1)
			{
				return this.squeezeSoundBank[Random.Range(0, this.squeezeSoundBank.Length)];
			}
			if (this.squeezeSoundBank.Length == 1)
			{
				return this.squeezeSoundBank[0];
			}
			return this.squeezeSound;
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06002087 RID: 8327 RVA: 0x000ACBE0 File Offset: 0x000AADE0
	public int SqueezeReleaseSound
	{
		get
		{
			if (this.squeezeReleaseSoundBank.Length > 1)
			{
				return this.squeezeReleaseSoundBank[Random.Range(0, this.squeezeReleaseSoundBank.Length)];
			}
			if (this.squeezeReleaseSoundBank.Length == 1)
			{
				return this.squeezeReleaseSoundBank[0];
			}
			return this.squeezeReleaseSound;
		}
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000ACC20 File Offset: 0x000AAE20
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		if (this.skinRenderer == null)
		{
			this.skinRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}
		this.hasSkinRenderer = (this.skinRenderer != null);
		this.myThreshold = 0.7f;
		this.hysterisis = 0.3f;
		this.hasParticleFX = (this.particleFX != null);
		if (this.hasParticleFX)
		{
			this.pFXEmissionModule = this.particleFX.emission;
			this.pFXEmissionModule.rateOverTime = this.particleFXEmissionIdle;
		}
		this.fxActive = false;
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000ACCC0 File Offset: 0x000AAEC0
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnSqueezeActivate);
			this._events.Deactivate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnSqueezeDeactivate);
		}
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x000ACDB0 File Offset: 0x000AAFB0
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnSqueezeActivate);
			this._events.Deactivate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnSqueezeDeactivate);
			this._events.Dispose();
			this._events = null;
		}
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x000ACE27 File Offset: 0x000AB027
	private void OnSqueezeActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeActivateLocal();
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x000ACE4E File Offset: 0x000AB04E
	private void SqueezeActivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionSqueeze);
		if (this._sfxActivate && !this._sfxActivate.isPlaying)
		{
			this._sfxActivate.PlayNext(0f, 1f);
		}
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x000ACE8B File Offset: 0x000AB08B
	private void OnSqueezeDeactivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnSqueezeDeactivate");
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeDeactivateLocal();
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000ACEBE File Offset: 0x000AB0BE
	private void SqueezeDeactivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionIdle);
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x000ACECC File Offset: 0x000AB0CC
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		float num = 0f;
		if (base.InHand())
		{
			this.tempHandPos = ((base.myOnlineRig != null) ? base.myOnlineRig.ReturnHandPosition() : base.myRig.ReturnHandPosition());
			if (this.currentState == TransferrableObject.PositionState.InLeftHand)
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10000) / 1000f);
			}
			else
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10) / 1f);
			}
		}
		if (this.hasSkinRenderer)
		{
			this.skinRenderer.SetBlendShapeWeight(0, Mathf.Lerp(this.skinRenderer.GetBlendShapeWeight(0), num * 11.1f, this.blendShapeMaxWeight));
		}
		if (this.fxActive)
		{
			this.squeezeTimeElapsed += Time.deltaTime;
			this.pFXEmissionModule.rateOverTime = Mathf.Lerp(this.particleFXEmissionIdle, this.particleFXEmissionSqueeze, this.particleFXEmissionCooldownCurve.Evaluate(this.squeezeTimeElapsed));
			if (this.squeezeTimeElapsed > this.particleFXEmissionSqueeze)
			{
				this.fxActive = false;
			}
		}
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x000ACFE8 File Offset: 0x000AB1E8
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			RigContainer localRig = VRRigCache.Instance.localRig;
			int num = this.SqueezeSound;
			localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			if (localRig.netView)
			{
				localRig.netView.SendRPC("RPC_PlayHandTap", 1, new object[]
				{
					num,
					flag,
					0.33f
				});
			}
			GorillaTagger.Instance.StartVibration(flag, this.squeezeStrength, Time.deltaTime);
		}
		if (this._raiseActivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent activate = events.Activate;
				if (activate == null)
				{
					return;
				}
				activate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeActivateLocal();
			}
		}
	}

	// Token: 0x06002091 RID: 8337 RVA: 0x000AD0C4 File Offset: 0x000AB2C4
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			int num = this.SqueezeReleaseSound;
			Debug.Log("Squeezy Deactivate: " + num.ToString());
			VRRigCache.Instance.localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			RigContainer rigContainer;
			if (GorillaGameManager.instance && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
			{
				rigContainer.Rig.netView.SendRPC("RPC_PlayHandTap", 1, new object[]
				{
					num,
					flag,
					0.33f
				});
			}
			GorillaTagger.Instance.StartVibration(flag, this.releaseStrength, Time.deltaTime);
		}
		if (this._raiseDeactivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent deactivate = events.Deactivate;
				if (deactivate == null)
				{
					return;
				}
				deactivate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeDeactivateLocal();
			}
		}
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x000AD1D0 File Offset: 0x000AB3D0
	public void PlayParticleFX(float rate)
	{
		if (!this.hasParticleFX)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		if (!this.fxActive)
		{
			this.fxActive = true;
		}
		this.squeezeTimeElapsed = 0f;
		this.pFXEmissionModule.rateOverTime = rate;
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000AD224 File Offset: 0x000AB424
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x000AD22F File Offset: 0x000AB42F
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002B1B RID: 11035
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002B1C RID: 11036
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04002B1D RID: 11037
	private SkinnedMeshRenderer skinRenderer;

	// Token: 0x04002B1E RID: 11038
	[FormerlySerializedAs("duckieLerp")]
	public float blendShapeMaxWeight = 1f;

	// Token: 0x04002B1F RID: 11039
	private int tempHandPos;

	// Token: 0x04002B20 RID: 11040
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeSound = 75;

	// Token: 0x04002B21 RID: 11041
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeReleaseSound = 76;

	// Token: 0x04002B22 RID: 11042
	[GorillaSoundLookup]
	public int[] squeezeSoundBank;

	// Token: 0x04002B23 RID: 11043
	[GorillaSoundLookup]
	public int[] squeezeReleaseSoundBank;

	// Token: 0x04002B24 RID: 11044
	public float squeezeStrength = 0.05f;

	// Token: 0x04002B25 RID: 11045
	public float releaseStrength = 0.03f;

	// Token: 0x04002B26 RID: 11046
	public ParticleSystem particleFX;

	// Token: 0x04002B27 RID: 11047
	[Tooltip("The emission rate of the particle effect when not squeezed.")]
	public float particleFXEmissionIdle = 0.8f;

	// Token: 0x04002B28 RID: 11048
	[Tooltip("The emission rate of the particle effect when squeezed.")]
	public float particleFXEmissionSqueeze = 10f;

	// Token: 0x04002B29 RID: 11049
	[Tooltip("The animation of the particle effect returning to the idle emission rate. X axis is time, Y axis is the emission lerp value where 0 is idle, 1 is squeezed.")]
	public AnimationCurve particleFXEmissionCooldownCurve;

	// Token: 0x04002B2A RID: 11050
	private bool hasSkinRenderer;

	// Token: 0x04002B2B RID: 11051
	private ParticleSystem.EmissionModule pFXEmissionModule;

	// Token: 0x04002B2C RID: 11052
	private bool hasParticleFX;

	// Token: 0x04002B2D RID: 11053
	private float squeezeTimeElapsed;

	// Token: 0x04002B2E RID: 11054
	[SerializeField]
	private RubberDuckEvents _events;

	// Token: 0x04002B2F RID: 11055
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04002B30 RID: 11056
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04002B31 RID: 11057
	[SerializeField]
	private SoundEffects _sfxActivate;

	// Token: 0x04002B32 RID: 11058
	[SerializeField]
	private bool _fxActive;
}
