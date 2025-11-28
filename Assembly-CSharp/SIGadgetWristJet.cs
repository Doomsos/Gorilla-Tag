using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000F9 RID: 249
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetWristJet : SIGadget, I_SIDisruptable
{
	// Token: 0x17000066 RID: 102
	// (get) Token: 0x0600065A RID: 1626 RVA: 0x000242C9 File Offset: 0x000224C9
	private bool CanRecharge
	{
		get
		{
			return (!this.rechargeRequiresFloorTouch || this._floorTouched) && this.state != SIGadgetWristJet.State.Active;
		}
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x000242EC File Offset: 0x000224EC
	private void Awake()
	{
		this._maxSqrHorizontalSpeed = this.maxHorizontalSpeed * this.maxHorizontalSpeed;
		this._hasThrustLoopAudioSource = (this.m_thrustLoopAudioSource != null);
		this.m_warnFuelLowThreshold = ((this.m_warnFuelLowSound != null) ? this.m_warnFuelLowThreshold : -1f);
		this._hasInactiveStateVisual = (this.inactiveStateVisual != null);
		this._hasActiveStateVisual = (this.activeStateVisual != null);
		this._gaugeMatPropBlock = new MaterialPropertyBlock();
		if (this.m_gaugeMatSlots == null)
		{
			this.m_gaugeMatSlots = Array.Empty<GTRendererMatSlot>();
		}
		int num = 0;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			if (this.m_gaugeMatSlots[i].TryInitialize())
			{
				this.m_gaugeMatSlots[num] = this.m_gaugeMatSlots[i];
				num++;
			}
		}
		if (num != this.m_gaugeMatSlots.Length)
		{
			Array.Resize<GTRendererMatSlot>(ref this.m_gaugeMatSlots, num);
		}
		this.throttleFlapInitialRots = ((this.m_throttleFlapXforms != null) ? new Quaternion[this.m_throttleFlapXforms.Length] : Array.Empty<Quaternion>());
		for (int j = 0; j < this.throttleFlapInitialRots.Length; j++)
		{
			if (this.m_throttleFlapXforms[j] == null)
			{
				this.throttleFlapInitialRots = Array.Empty<Quaternion>();
				Debug.LogError("[SIGadgetWristJet]  ERROR!!!  Awake: Throttle indicator flaps will not animate because entry is null in " + string.Format("array at `{0}[{1}]`. Path={2}", "m_throttleFlapXforms", j, base.transform.GetPathQ()), this);
				return;
			}
			this.throttleFlapInitialRots[j] = this.m_throttleFlapXforms[j].localRotation;
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00024476 File Offset: 0x00022676
	private void Start()
	{
		this.gtPlayer = GTPlayer.Instance;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0002449A File Offset: 0x0002269A
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.m_warnFuelLowThreshold > 0f)
		{
			this.m_warnFuelLowSound.LoadAudioData();
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x000244BB File Offset: 0x000226BB
	protected override void OnDisable()
	{
		if (this.m_warnFuelLowThreshold > 0f && this.m_warnFuelLowSound.loadState != null)
		{
			this.m_warnFuelLowSound.UnloadAudioData();
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x000244E4 File Offset: 0x000226E4
	protected override void Update()
	{
		base.Update();
		if (this._hasThrustLoopAudioSource)
		{
			float num = (this.state == SIGadgetWristJet.State.Active) ? this.m_thrustLoopSoundVolume : 0f;
			float num2 = (this.state == SIGadgetWristJet.State.Active) ? this.m_thrustLoopAudioFadeInTime : this.m_thrustLoopAudioFadeOutTime;
			this.m_thrustLoopAudioSource.volume = Mathf.MoveTowards(this.m_thrustLoopAudioSource.volume, num, 1f / num2 * Time.unscaledDeltaTime);
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00024558 File Offset: 0x00022758
	private void FixedUpdate()
	{
		if (!this.IsEquippedLocal() && !this.activatedLocally)
		{
			return;
		}
		if (this.state == SIGadgetWristJet.State.Active && this.currentFuel > 0f && this.buttonActivatable.CheckInput(true, true, 0.25f, true, true))
		{
			this.gtPlayer.AddForce(-Physics.gravity * (this.gtPlayer.scale * this.gravityNegationPercent), 5);
			this._ApplyClampedThrust();
		}
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x000245D8 File Offset: 0x000227D8
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		bool flag = this.buttonActivatable.CheckInput(true, true, 0.25f, true, true);
		if (!this._floorTouched)
		{
			this._floorTouched = (this.gtPlayer.IsGroundedButt || this.gtPlayer.IsGroundedHand);
		}
		if (this._throttleControl)
		{
			Vector2 joystickInput = base.GetJoystickInput();
			if (Mathf.Abs(joystickInput.y) > 0.75f && Mathf.Abs(joystickInput.x) < 0.5f)
			{
				this._throttle = Mathf.Clamp01(this._throttle + joystickInput.y * this.throttleChangeSpeed * Time.deltaTime);
				this._currentBurnRate = Mathf.Lerp(this.minimumBurnRate, 1f, this._throttle);
				this.UpdateThrottleIndicator();
			}
		}
		switch (this.state)
		{
		case SIGadgetWristJet.State.Unactive:
			if (this.CanRecharge)
			{
				this.currentFuel = Mathf.Clamp(this.currentFuel + dt * this.fuelGainRate, 0f, this.fuelSize);
			}
			if (flag)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.Active);
			}
			break;
		case SIGadgetWristJet.State.Active:
			this.currentFuel = Mathf.Clamp(this.currentFuel - dt * this.fuelSpendRate * this._currentBurnRate, 0f, this.fuelSize);
			this._floorTouched = false;
			this.gtPlayer.ThrusterActiveAtFrame = Time.frameCount;
			if (flag && this.m_warnFuelLowThreshold > 0f)
			{
				float num = this.currentFuel / this.fuelSize;
				if (this._warnFuelLowSoundWasPlayed && num > this.m_warnFuelLowThreshold)
				{
					this._warnFuelLowSoundWasPlayed = false;
				}
				else if (!this._warnFuelLowSoundWasPlayed && num <= this.m_warnFuelLowThreshold)
				{
					this._warnFuelLowSoundWasPlayed = true;
					this.gameEntity.audioSource.GTPlayOneShot(this.m_warnFuelLowSound, this.m_warnFuelLowSoundVolume);
				}
			}
			if (!flag || this.currentFuel <= 0f)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.OutOfFuel);
			}
			break;
		case SIGadgetWristJet.State.OutOfFuel:
			if (!flag)
			{
				this.emptiedCooldownResetProgress += dt;
			}
			else if (this.currentFuel > 0f)
			{
				this.SetStateAuthority(SIGadgetWristJet.State.Active);
			}
			if (this.emptiedCooldownResetProgress > this.emptiedCooldown)
			{
				this.emptiedCooldownResetProgress = 0f;
				this.SetStateAuthority(SIGadgetWristJet.State.Unactive);
			}
			break;
		}
		float num2 = this.currentFuel / this.fuelSize;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			this._gaugeMatPropBlock.SetFloat(ShaderProps._EmissionDissolveProgress, num2);
			this.m_gaugeMatSlots[i].renderer.SetPropertyBlock(this._gaugeMatPropBlock, this.m_gaugeMatSlots[i].slot);
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00024878 File Offset: 0x00022A78
	private void UpdateThrottleIndicator()
	{
		for (int i = 0; i < this.throttleFlapInitialRots.Length; i++)
		{
			Quaternion quaternion = this.throttleFlapInitialRots[i] * this.m_throttleFlapMaxRotOffset;
			this.m_throttleFlapXforms[i].localRotation = Quaternion.Lerp(this.throttleFlapInitialRots[i], quaternion, this._throttle);
		}
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x000248D8 File Offset: 0x00022AD8
	private void _ApplyClampedThrust()
	{
		Vector3 rigidbodyVelocity = this.gtPlayer.RigidbodyVelocity;
		float num = this.jetForce * this._currentBurnRate;
		Vector3 vector = rigidbodyVelocity + base.transform.forward * (num * Time.fixedDeltaTime);
		Vector3 vector2;
		vector2..ctor(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude > this._maxSqrHorizontalSpeed)
		{
			float magnitude = new Vector3(rigidbodyVelocity.x, 0f, rigidbodyVelocity.z).magnitude;
			vector2 = Vector3.ClampMagnitude(vector2, Mathf.Max(this.maxHorizontalSpeed, magnitude));
		}
		Vector3 vector3 = vector2;
		vector3.y = ((vector.y > this.maxVerticalSpeed) ? Mathf.Max(this.maxVerticalSpeed, rigidbodyVelocity.y) : vector.y);
		this.gtPlayer.AddForce(vector3 - rigidbodyVelocity, 2);
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x000249C0 File Offset: 0x00022BC0
	private void OnEntityStateChanged(long oldState, long newState)
	{
		SIGadgetWristJet.State state = (SIGadgetWristJet.State)oldState;
		SIGadgetWristJet.State state2 = (SIGadgetWristJet.State)newState;
		if (state != state2)
		{
			this.SetState(state2);
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000249DC File Offset: 0x00022BDC
	private void SetStateAuthority(SIGadgetWristJet.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00024A00 File Offset: 0x00022C00
	private void SetState(SIGadgetWristJet.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetWristJet.State.Unactive:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(true);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(false);
				return;
			}
			break;
		case SIGadgetWristJet.State.Active:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(false);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(true);
				return;
			}
			break;
		case SIGadgetWristJet.State.OutOfFuel:
			if (this._hasInactiveStateVisual)
			{
				this.inactiveStateVisual.SetActive(true);
			}
			if (this._hasActiveStateVisual)
			{
				this.activeStateVisual.SetActive(false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00024AB4 File Offset: 0x00022CB4
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._throttleControl = withUpgrades.Contains(SIUpgradeType.Thruster_Throttle_Control);
		if (this._throttleControl)
		{
			this.UpdateThrottleIndicator();
		}
		switch (this.jetType)
		{
		case SIGadgetWristJet.WristJetType.Jet:
			this.fuelSpendRate *= (withUpgrades.Contains(SIUpgradeType.Thruster_Jet_Duration) ? 0.8f : 1f);
			this.jetForce *= (withUpgrades.Contains(SIUpgradeType.Thruster_Jet_Accel) ? 1.2f : 1f);
			break;
		case SIGadgetWristJet.WristJetType.Propellor:
			this.fuelSpendRate *= (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Duration) ? 0.8f : 1f);
			this.maxVerticalSpeed *= (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Speed) ? 1.2f : 1f);
			this.maxHorizontalSpeed *= (withUpgrades.Contains(SIUpgradeType.Thruster_Prop_Speed) ? 1.2f : 1f);
			break;
		}
		AudioClip clip;
		if (this._hasThrustLoopAudioSource && this.m_thrustLoopSoundByUpgrade.TryGetActiveValue(withUpgrades, out clip))
		{
			this.m_thrustLoopAudioSource.clip = clip;
			this.m_thrustLoopAudioSource.Play();
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00024BDC File Offset: 0x00022DDC
	public void Disrupt(float disruptTime)
	{
		this.emptiedCooldownResetProgress = -disruptTime;
		this.SetState(SIGadgetWristJet.State.OutOfFuel);
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00024BF0 File Offset: 0x00022DF0
	public override void OnEntityInit()
	{
		this.emptiedCooldownResetProgress = 0f;
		if (this._hasInactiveStateVisual)
		{
			this.inactiveStateVisual.SetActive(true);
		}
		if (this._hasActiveStateVisual)
		{
			this.activeStateVisual.SetActive(false);
		}
		this.currentFuel = (this.fuelSize = 10f);
		this._throttle = (this._currentBurnRate = 1f);
	}

	// Token: 0x040007E4 RID: 2020
	private const string preLog = "[SIGadgetWristJet]  ";

	// Token: 0x040007E5 RID: 2021
	private const string preErr = "[SIGadgetWristJet]  ERROR!!!  ";

	// Token: 0x040007E6 RID: 2022
	private const string preErrBeta = "[SIGadgetWristJet]  ERROR!!!  (beta only log)  ";

	// Token: 0x040007E7 RID: 2023
	[SerializeField]
	private AudioSource m_thrustLoopAudioSource;

	// Token: 0x040007E8 RID: 2024
	private bool _hasThrustLoopAudioSource;

	// Token: 0x040007E9 RID: 2025
	[SerializeField]
	private SIUpgradeBasedGeneric<AudioClip> m_thrustLoopSoundByUpgrade;

	// Token: 0x040007EA RID: 2026
	[SerializeField]
	private float m_thrustLoopAudioFadeInTime = 0.1f;

	// Token: 0x040007EB RID: 2027
	[SerializeField]
	private float m_thrustLoopAudioFadeOutTime = 0.5f;

	// Token: 0x040007EC RID: 2028
	[SerializeField]
	private float m_thrustLoopSoundVolume = 0.33f;

	// Token: 0x040007ED RID: 2029
	[SerializeField]
	private AudioClip m_warnFuelLowSound;

	// Token: 0x040007EE RID: 2030
	[SerializeField]
	private float m_warnFuelLowThreshold = 0.5f;

	// Token: 0x040007EF RID: 2031
	[SerializeField]
	private float m_warnFuelLowSoundVolume = 0.05f;

	// Token: 0x040007F0 RID: 2032
	private bool _warnFuelLowSoundWasPlayed;

	// Token: 0x040007F1 RID: 2033
	[Tooltip("This renderer's material will have the `_EmissionDissolveProgress` property changed to visually communicate current fuel amount.")]
	[SerializeField]
	private GTRendererMatSlot[] m_gaugeMatSlots;

	// Token: 0x040007F2 RID: 2034
	public SIGadgetWristJet.WristJetType jetType;

	// Token: 0x040007F3 RID: 2035
	public GameButtonActivatable buttonActivatable;

	// Token: 0x040007F4 RID: 2036
	public GameObject inactiveStateVisual;

	// Token: 0x040007F5 RID: 2037
	private bool _hasInactiveStateVisual;

	// Token: 0x040007F6 RID: 2038
	[FormerlySerializedAs("jetFlame")]
	public GameObject activeStateVisual;

	// Token: 0x040007F7 RID: 2039
	private bool _hasActiveStateVisual;

	// Token: 0x040007F8 RID: 2040
	public float jetForce;

	// Token: 0x040007F9 RID: 2041
	public float fuelGainRate;

	// Token: 0x040007FA RID: 2042
	public float fuelSpendRate;

	// Token: 0x040007FB RID: 2043
	public float emptiedCooldown;

	// Token: 0x040007FC RID: 2044
	public float gravityNegationPercent;

	// Token: 0x040007FD RID: 2045
	public float maxVerticalSpeed;

	// Token: 0x040007FE RID: 2046
	public float maxHorizontalSpeed;

	// Token: 0x040007FF RID: 2047
	[SerializeField]
	private bool rechargeRequiresFloorTouch;

	// Token: 0x04000800 RID: 2048
	[SerializeField]
	private float throttleChangeSpeed = 2f;

	// Token: 0x04000801 RID: 2049
	[SerializeField]
	[Tooltip("Minimum proportion of thrust allowed with throttle control.")]
	[Range(0f, 1f)]
	private float minimumBurnRate = 0.33f;

	// Token: 0x04000802 RID: 2050
	[SerializeField]
	private Transform[] m_throttleFlapXforms;

	// Token: 0x04000803 RID: 2051
	private Quaternion[] throttleFlapInitialRots;

	// Token: 0x04000804 RID: 2052
	[SerializeField]
	private Quaternion m_throttleFlapMaxRotOffset = Quaternion.Euler(45f, 0f, 0f);

	// Token: 0x04000805 RID: 2053
	private float fuelSize;

	// Token: 0x04000806 RID: 2054
	private float currentFuel;

	// Token: 0x04000807 RID: 2055
	private SIGadgetWristJet.State state;

	// Token: 0x04000808 RID: 2056
	private GTPlayer gtPlayer;

	// Token: 0x04000809 RID: 2057
	private float emptiedCooldownResetProgress;

	// Token: 0x0400080A RID: 2058
	private bool _floorTouched;

	// Token: 0x0400080B RID: 2059
	private float _maxSqrHorizontalSpeed;

	// Token: 0x0400080C RID: 2060
	private const float kFUEL_CAPACITY = 10f;

	// Token: 0x0400080D RID: 2061
	private MaterialPropertyBlock _gaugeMatPropBlock;

	// Token: 0x0400080E RID: 2062
	private bool _throttleControl;

	// Token: 0x0400080F RID: 2063
	private float _throttle;

	// Token: 0x04000810 RID: 2064
	private float _currentBurnRate;

	// Token: 0x020000FA RID: 250
	private enum State
	{
		// Token: 0x04000812 RID: 2066
		Unactive,
		// Token: 0x04000813 RID: 2067
		Active,
		// Token: 0x04000814 RID: 2068
		OutOfFuel
	}

	// Token: 0x020000FB RID: 251
	public enum WristJetType
	{
		// Token: 0x04000816 RID: 2070
		Basic,
		// Token: 0x04000817 RID: 2071
		Jet,
		// Token: 0x04000818 RID: 2072
		Propellor
	}
}
