using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag.Cosmetics;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class RCHoverboard : RCVehicle
{
	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000C0B RID: 3083 RVA: 0x00040E23 File Offset: 0x0003F023
	// (set) Token: 0x06000C0C RID: 3084 RVA: 0x00040E2B File Offset: 0x0003F02B
	private float _MaxForwardSpeed
	{
		get
		{
			return this.m_maxForwardSpeed;
		}
		set
		{
			this.m_maxForwardSpeed = value;
			this._forwardAccel = value / math.max(0.01f, this.m_forwardAccelTime);
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000C0D RID: 3085 RVA: 0x00040E4C File Offset: 0x0003F04C
	// (set) Token: 0x06000C0E RID: 3086 RVA: 0x00040E54 File Offset: 0x0003F054
	private float _MaxTurnRate
	{
		get
		{
			return this.m_maxTurnRate;
		}
		set
		{
			this.m_maxTurnRate = value;
			this._turnAccel = value / math.max(1E-06f, this.m_turnAccelTime);
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000C0F RID: 3087 RVA: 0x00040E75 File Offset: 0x0003F075
	// (set) Token: 0x06000C10 RID: 3088 RVA: 0x00040E7D File Offset: 0x0003F07D
	private float _MaxTiltAngle
	{
		get
		{
			return this.m_maxTiltAngle;
		}
		set
		{
			this.m_maxTiltAngle = value;
			this._tiltAccel = value / math.max(1E-06f, this.m_tiltTime);
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00040EA0 File Offset: 0x0003F0A0
	protected override void Awake()
	{
		base.Awake();
		this._hasAudioSource = (this.m_audioSource != null);
		this._hasHoverSound = (this.m_hoverSound != null);
		this._MaxForwardSpeed = this.m_maxForwardSpeed;
		this._MaxTurnRate = this.m_maxTurnRate;
		this._MaxTiltAngle = this.m_maxTiltAngle;
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00040EFC File Offset: 0x0003F0FC
	protected override void AuthorityBeginDocked()
	{
		base.AuthorityBeginDocked();
		this._currentTurnRate = 0f;
		this._currentTiltAngle = 0f;
		float3 to = this._ProjectOnPlane(base.transform.forward, math.up());
		this._currentTurnAngle = this._SignedAngle(new float3(0f, 0f, 1f), to, new float3(0f, 1f, 0f));
		this._motorLevel = 0f;
		if (this._hasAudioSource)
		{
			this.m_audioSource.Stop();
			this.m_audioSource.volume = 0f;
		}
		if (this.connectedRemote == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00040FC0 File Offset: 0x0003F1C0
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized)
		{
			float num = math.length(this.activeInput.joystick);
			this._motorLevel = math.saturate(num);
			if (this.hasNetworkSync)
			{
				this.networkSync.syncedState.dataA = (byte)((uint)(this._motorLevel * 255f));
				return;
			}
		}
		else
		{
			this._motorLevel = 0f;
		}
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00041034 File Offset: 0x0003F234
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (this.localState == RCVehicle.State.Mobilized && this.hasNetworkSync)
		{
			this._motorLevel = (float)this.networkSync.syncedState.dataA / 255f;
			return;
		}
		this._motorLevel = 0f;
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x00041084 File Offset: 0x0003F284
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		switch (this.localState)
		{
		case RCVehicle.State.Disabled:
		case RCVehicle.State.DockedLeft:
		case RCVehicle.State.DockedRight:
		case RCVehicle.State.Crashed:
			break;
		case RCVehicle.State.Mobilized:
			if (this._hasAudioSource && this._hasHoverSound)
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.m_audioSource.volume = 0f;
					this.m_audioSource.clip = this.m_hoverSound;
					this.m_audioSource.loop = true;
					this.m_audioSource.GTPlay();
					return;
				}
				float target = math.lerp(this.m_hoverSoundVolumeMinMax.x, this.m_hoverSoundVolumeMinMax.y, this._motorLevel);
				float maxDelta = this.m_hoverSoundVolumeMinMax.y / this.m_hoverSoundVolumeRampTime * dt;
				this.m_audioSource.volume = this._MoveTowards(this.m_audioSource.volume, target, maxDelta);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x00041168 File Offset: 0x0003F368
	protected void FixedUpdate()
	{
		if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = this.m_inputThrustForward.Get(this.activeInput) - this.m_inputThrustBack.Get(this.activeInput);
		float num2 = this.m_inputTurn.Get(this.activeInput);
		float num3 = this.m_inputJump.Get(this.activeInput);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(base.transform.position, Vector3.down, ref raycastHit, 10f, this.raycastLayers, 2);
		bool flag2 = flag && raycastHit.distance <= this.m_hoverHeight + 0.1f;
		if (this.enableJumpInput && num3 > 0.001f && flag2 && !this._hasJumped)
		{
			this.rb.AddForce(Vector3.up * this.m_jumpForce, 1);
			this._hasJumped = true;
		}
		else if (num3 <= 0.001f)
		{
			this._hasJumped = false;
		}
		float target = num2 * this._MaxTurnRate;
		this._currentTurnRate = this._MoveTowards(this._currentTurnRate, target, this._turnAccel * fixedDeltaTime);
		this._currentTurnAngle += this._currentTurnRate * fixedDeltaTime;
		float target2 = math.lerp(-this.m_maxTiltAngle, this.m_maxTiltAngle, math.unlerp(-1f, 1f, num));
		this._currentTiltAngle = this._MoveTowards(this._currentTiltAngle, target2, this._tiltAccel * fixedDeltaTime);
		base.transform.rotation = quaternion.EulerXYZ(math.radians(new float3(this._currentTiltAngle, this._currentTurnAngle, 0f)));
		float3 @float = base.transform.forward;
		float num4 = math.dot(@float, this.rb.linearVelocity);
		float num5 = num * this.m_maxForwardSpeed;
		float num6 = (math.abs(num5) > 0.001f && ((num5 > 0f && num4 < num5) || (num5 < 0f && num4 > num5))) ? math.sign(num5) : 0f;
		this.rb.AddForce(@float * this._forwardAccel * num6 * this.rb.mass, 0);
		if (flag)
		{
			float num7 = math.saturate(this.m_hoverHeight - raycastHit.distance);
			float num8 = math.dot(this.rb.linearVelocity, Vector3.up);
			float num9 = num7 * this.m_hoverForce - num8 * this.m_hoverDamp;
			this.rb.AddForce(math.up() * num9, 0);
		}
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x00041434 File Offset: 0x0003F634
	protected void OnCollisionEnter(Collision collision)
	{
		GameObject gameObject = collision.collider.gameObject;
		bool flag = gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
		bool flag2 = gameObject.IsOnLayer(UnityLayer.GorillaHand);
		if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
		{
			Vector3 vector = Vector3.zero;
			if (flag2)
			{
				GorillaHandClimber component = gameObject.GetComponent<GorillaHandClimber>();
				if (component != null)
				{
					vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == 4).GetAverageVelocity(true, 0.15f, false);
				}
			}
			else if (collision.rigidbody != null)
			{
				vector = collision.rigidbody.linearVelocity;
			}
			if ((flag || vector.sqrMagnitude > 0.01f) && base.HasLocalAuthority)
			{
				this.AuthorityApplyImpact(vector, flag);
				if (this.networkSync != null)
				{
					this.networkSync.photonView.RPC("HitRCVehicleRPC", 1, new object[]
					{
						vector,
						flag
					});
				}
			}
		}
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x00041528 File Offset: 0x0003F728
	[MethodImpl(256)]
	private float _MoveTowards(float current, float target, float maxDelta)
	{
		if (math.abs(target - current) > maxDelta)
		{
			return current + math.sign(target - current) * maxDelta;
		}
		return target;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00041544 File Offset: 0x0003F744
	[MethodImpl(256)]
	private float _SignedAngle(float3 from, float3 to, float3 axis)
	{
		float3 @float = math.normalize(from);
		float3 float2 = math.normalize(to);
		float num = math.acos(math.dot(@float, float2));
		float num2 = math.sign(math.dot(math.cross(@float, float2), axis));
		return math.degrees(num) * num2;
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x00041585 File Offset: 0x0003F785
	[MethodImpl(256)]
	private float3 _ProjectOnPlane(float3 vector, float3 planeNormal)
	{
		return vector - math.dot(vector, planeNormal) * planeNormal;
	}

	// Token: 0x04000EB5 RID: 3765
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputTurn = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickX, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.1f, 0f, 0f, 1.25f, 0f, 0f),
		new Keyframe(0.9f, 1f, 1.25f, 0f, 0f, 0f),
		new Keyframe(1f, 1f, 0f, 0f, 0f, 0f)
	}));

	// Token: 0x04000EB6 RID: 3766
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustForward = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.Trigger, AnimationCurves.EaseInCirc);

	// Token: 0x04000EB7 RID: 3767
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputThrustBack = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.StickBack, new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
		new Keyframe(0.9f, 0f, 0f, 9.9999f, 0.5825f, 0.3767f),
		new Keyframe(1f, 1f, 9.9999f, 1f, 0f, 0f)
	}));

	// Token: 0x04000EB8 RID: 3768
	[SerializeField]
	private RCHoverboard._SingleInputOption m_inputJump = new RCHoverboard._SingleInputOption(RCHoverboard._EInputSource.PrimaryFaceButton, AnimationCurves.Linear);

	// Token: 0x04000EB9 RID: 3769
	[Tooltip("Desired hover height above ground from this transform's position.")]
	[SerializeField]
	private float m_hoverHeight = 0.2f;

	// Token: 0x04000EBA RID: 3770
	[Tooltip("Upward force to maintain hover when below hoverHeight.")]
	[SerializeField]
	private float m_hoverForce = 200f;

	// Token: 0x04000EBB RID: 3771
	[Tooltip("Damping factor to smooth out vertical movement.")]
	[SerializeField]
	private float m_hoverDamp = 5f;

	// Token: 0x04000EBC RID: 3772
	[SerializeField]
	private LayerMask raycastLayers = -1;

	// Token: 0x04000EBD RID: 3773
	[SerializeField]
	private bool enableJumpInput = true;

	// Token: 0x04000EBE RID: 3774
	[Tooltip("Upward impulse force for jump.")]
	[SerializeField]
	private float m_jumpForce = 3.5f;

	// Token: 0x04000EBF RID: 3775
	private bool _hasJumped;

	// Token: 0x04000EC0 RID: 3776
	[SerializeField]
	[HideInInspector]
	private float m_maxForwardSpeed = 6f;

	// Token: 0x04000EC1 RID: 3777
	[SerializeField]
	[Tooltip("Time (seconds) to reach max forward speed from zero.")]
	private float m_forwardAccelTime = 2f;

	// Token: 0x04000EC2 RID: 3778
	[SerializeField]
	[HideInInspector]
	private float m_maxTurnRate = 720f;

	// Token: 0x04000EC3 RID: 3779
	[Tooltip("Time (seconds) to reach max turning rate.")]
	[SerializeField]
	private float m_turnAccelTime = 0.75f;

	// Token: 0x04000EC4 RID: 3780
	[SerializeField]
	[HideInInspector]
	private float m_maxTiltAngle = 30f;

	// Token: 0x04000EC5 RID: 3781
	[Tooltip("Time (seconds) to reach max tilt angle.")]
	[SerializeField]
	private float m_tiltTime = 0.1f;

	// Token: 0x04000EC6 RID: 3782
	[Tooltip("Audio source for any motor or hover sound.")]
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000EC7 RID: 3783
	[Tooltip("Looping motor/hover sound clip.")]
	[SerializeField]
	private AudioClip m_hoverSound;

	// Token: 0x04000EC8 RID: 3784
	[Tooltip("Volume range for the hover sound (x = min, y = max).")]
	[SerializeField]
	private float2 m_hoverSoundVolumeMinMax = new float2(0.1f, 0.5f);

	// Token: 0x04000EC9 RID: 3785
	[Tooltip("Time it takes for the volume to reach max value.")]
	[SerializeField]
	private float m_hoverSoundVolumeRampTime = 1f;

	// Token: 0x04000ECA RID: 3786
	private bool _hasAudioSource;

	// Token: 0x04000ECB RID: 3787
	private bool _hasHoverSound;

	// Token: 0x04000ECC RID: 3788
	private float _forwardAccel;

	// Token: 0x04000ECD RID: 3789
	private float _turnAccel;

	// Token: 0x04000ECE RID: 3790
	private float _tiltAccel;

	// Token: 0x04000ECF RID: 3791
	private float _currentTurnRate;

	// Token: 0x04000ED0 RID: 3792
	private float _currentTurnAngle;

	// Token: 0x04000ED1 RID: 3793
	private float _currentTiltAngle;

	// Token: 0x04000ED2 RID: 3794
	private float _motorLevel;

	// Token: 0x020001C0 RID: 448
	private enum _EInputSource
	{
		// Token: 0x04000ED4 RID: 3796
		None,
		// Token: 0x04000ED5 RID: 3797
		StickX,
		// Token: 0x04000ED6 RID: 3798
		StickForward,
		// Token: 0x04000ED7 RID: 3799
		StickBack,
		// Token: 0x04000ED8 RID: 3800
		Trigger,
		// Token: 0x04000ED9 RID: 3801
		PrimaryFaceButton
	}

	// Token: 0x020001C1 RID: 449
	[Serializable]
	private struct _SingleInputOption
	{
		// Token: 0x06000C1C RID: 3100 RVA: 0x000417C6 File Offset: 0x0003F9C6
		public _SingleInputOption(RCHoverboard._EInputSource source, AnimationCurve remapCurve)
		{
			this.source = new GTOption<StringEnum<RCHoverboard._EInputSource>>(source);
			this.remapCurve = new GTOption<AnimationCurve>(remapCurve);
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x000417E8 File Offset: 0x0003F9E8
		public float Get(RCRemoteHoldable.RCInput input)
		{
			float num;
			switch (this.source.ResolvedValue.Value)
			{
			case RCHoverboard._EInputSource.None:
				num = 0f;
				break;
			case RCHoverboard._EInputSource.StickX:
				num = input.joystick.x;
				break;
			case RCHoverboard._EInputSource.StickForward:
				num = math.saturate(input.joystick.y);
				break;
			case RCHoverboard._EInputSource.StickBack:
				num = math.saturate(-input.joystick.y);
				break;
			case RCHoverboard._EInputSource.Trigger:
				num = input.trigger;
				break;
			case RCHoverboard._EInputSource.PrimaryFaceButton:
				num = (float)input.buttons;
				break;
			default:
				num = 0f;
				break;
			}
			float num2 = num;
			return this.remapCurve.ResolvedValue.Evaluate(math.abs(num2)) * math.sign(num2);
		}

		// Token: 0x04000EDA RID: 3802
		public GTOption<StringEnum<RCHoverboard._EInputSource>> source;

		// Token: 0x04000EDB RID: 3803
		public GTOption<AnimationCurve> remapCurve;
	}
}
