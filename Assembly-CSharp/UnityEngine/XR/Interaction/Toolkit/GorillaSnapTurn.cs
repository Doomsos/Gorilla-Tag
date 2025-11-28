using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x02000D94 RID: 3476
	public class GorillaSnapTurn : LocomotionProvider, ITickSystemTick
	{
		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x0600554D RID: 21837 RVA: 0x001AD8AF File Offset: 0x001ABAAF
		// (set) Token: 0x0600554E RID: 21838 RVA: 0x001AD8B7 File Offset: 0x001ABAB7
		public bool TickRunning { get; set; }

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x0600554F RID: 21839 RVA: 0x001AD8C0 File Offset: 0x001ABAC0
		// (set) Token: 0x06005550 RID: 21840 RVA: 0x001AD8C8 File Offset: 0x001ABAC8
		public GorillaSnapTurn.InputAxes turnUsage
		{
			get
			{
				return this.m_TurnUsage;
			}
			set
			{
				this.m_TurnUsage = value;
			}
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x06005551 RID: 21841 RVA: 0x001AD8D1 File Offset: 0x001ABAD1
		// (set) Token: 0x06005552 RID: 21842 RVA: 0x001AD8D9 File Offset: 0x001ABAD9
		public List<XRController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x06005553 RID: 21843 RVA: 0x001AD8E2 File Offset: 0x001ABAE2
		// (set) Token: 0x06005554 RID: 21844 RVA: 0x001AD8EA File Offset: 0x001ABAEA
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x06005555 RID: 21845 RVA: 0x001AD8F3 File Offset: 0x001ABAF3
		// (set) Token: 0x06005556 RID: 21846 RVA: 0x001AD8FB File Offset: 0x001ABAFB
		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x06005557 RID: 21847 RVA: 0x001AD904 File Offset: 0x001ABB04
		// (set) Token: 0x06005558 RID: 21848 RVA: 0x001AD90C File Offset: 0x001ABB0C
		public float deadZone
		{
			get
			{
				return this.m_DeadZone;
			}
			set
			{
				this.m_DeadZone = value;
			}
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06005559 RID: 21849 RVA: 0x001AD915 File Offset: 0x001ABB15
		// (set) Token: 0x0600555A RID: 21850 RVA: 0x001AD91D File Offset: 0x001ABB1D
		public string turnType
		{
			get
			{
				return this.m_TurnType;
			}
			private set
			{
				this.m_TurnType = value;
			}
		}

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x0600555B RID: 21851 RVA: 0x001AD926 File Offset: 0x001ABB26
		// (set) Token: 0x0600555C RID: 21852 RVA: 0x001AD92E File Offset: 0x001ABB2E
		public int turnFactor
		{
			get
			{
				return this.m_TurnFactor;
			}
			private set
			{
				this.m_TurnFactor = value;
			}
		}

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x0600555D RID: 21853 RVA: 0x001AD937 File Offset: 0x001ABB37
		public static GorillaSnapTurn CachedSnapTurnRef
		{
			get
			{
				if (GorillaSnapTurn._cachedReference == null)
				{
					Debug.LogError("[SNAP_TURN] Tried accessing static cached reference, but was still null. Trying to find component in scene");
					GorillaSnapTurn._cachedReference = Object.FindAnyObjectByType<GorillaSnapTurn>();
				}
				return GorillaSnapTurn._cachedReference;
			}
		}

		// Token: 0x0600555E RID: 21854 RVA: 0x001AD95F File Offset: 0x001ABB5F
		protected override void Awake()
		{
			base.Awake();
			if (GorillaSnapTurn._cachedReference != null)
			{
				Debug.LogError("[SNAP_TURN] A [GorillaSnapTurn] component already exists in the scene");
				return;
			}
			GorillaSnapTurn._cachedReference = this;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x0600555F RID: 21855 RVA: 0x001AD98C File Offset: 0x001ABB8C
		public void Tick()
		{
			this.ValidateTurningOverriders();
			if (this.m_Controllers.Count > 0)
			{
				this.EnsureControllerDataListSize();
				for (int i = 0; i < this.m_Controllers.Count; i++)
				{
					XRController xrcontroller = this.m_Controllers[i];
					if (!(xrcontroller == null) && xrcontroller.enableInputActions)
					{
						float num = 0f;
						if (xrcontroller.controllerNode == 5)
						{
							num = ControllerInputPoller.instance.rightControllerPrimary2DAxis.x;
						}
						else if (xrcontroller.controllerNode == 4)
						{
							num = ControllerInputPoller.instance.leftControllerPrimary2DAxis.x;
						}
						if (num > this.deadZone)
						{
							this.StartTurn(this.m_TurnAmount);
						}
						else if (num < -this.deadZone)
						{
							this.StartTurn(-this.m_TurnAmount);
						}
						else
						{
							this.m_AxisReset = true;
						}
					}
				}
			}
			if (Mathf.Abs(this.m_CurrentTurnAmount) > 0f && base.TryPrepareLocomotion())
			{
				if (this.xrOrigin != null)
				{
					GTPlayer.Instance.Turn(this.m_CurrentTurnAmount);
				}
				this.m_CurrentTurnAmount = 0f;
				base.TryEndLocomotion();
			}
		}

		// Token: 0x06005560 RID: 21856 RVA: 0x001ADAB0 File Offset: 0x001ABCB0
		private void EnsureControllerDataListSize()
		{
			if (this.m_Controllers.Count != this.m_ControllersWereActive.Count)
			{
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.Add(false);
				}
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.RemoveAt(this.m_ControllersWereActive.Count - 1);
				}
			}
		}

		// Token: 0x06005561 RID: 21857 RVA: 0x001ADB2D File Offset: 0x001ABD2D
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x06005562 RID: 21858 RVA: 0x001ADB48 File Offset: 0x001ABD48
		private void StartTurn(float amount)
		{
			if (this.m_TimeStarted + this.m_DebounceTime > Time.time && !this.m_AxisReset)
			{
				return;
			}
			if (base.isLocomotionActive)
			{
				return;
			}
			if (this.turningOverriders.Count > 0)
			{
				return;
			}
			this.m_TimeStarted = Time.time;
			this.m_CurrentTurnAmount = amount;
			this.m_AxisReset = false;
		}

		// Token: 0x06005563 RID: 21859 RVA: 0x001ADBA4 File Offset: 0x001ABDA4
		public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
		{
			this.turnType = turnMode;
			this.turnFactor = turnSpeedFactor;
			if (turnMode == "SNAP")
			{
				this.m_DebounceTime = 0.5f;
				this.m_TurnAmount = 60f * this.ConvertedTurnFactor((float)turnSpeedFactor);
				return;
			}
			if (!(turnMode == "SMOOTH"))
			{
				this.m_DebounceTime = 0f;
				this.m_TurnAmount = 0f;
				return;
			}
			this.m_DebounceTime = 0f;
			this.m_TurnAmount = 360f * Time.fixedDeltaTime * this.ConvertedTurnFactor((float)turnSpeedFactor);
		}

		// Token: 0x06005564 RID: 21860 RVA: 0x001ADC37 File Offset: 0x001ABE37
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x06005565 RID: 21861 RVA: 0x001ADC56 File Offset: 0x001ABE56
		public void SetTurningOverride(ISnapTurnOverride caller)
		{
			if (!this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Add(caller);
			}
		}

		// Token: 0x06005566 RID: 21862 RVA: 0x001ADC73 File Offset: 0x001ABE73
		public void UnsetTurningOverride(ISnapTurnOverride caller)
		{
			if (this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Remove(caller);
			}
		}

		// Token: 0x06005567 RID: 21863 RVA: 0x001ADC90 File Offset: 0x001ABE90
		public void ValidateTurningOverriders()
		{
			foreach (ISnapTurnOverride snapTurnOverride in this.turningOverriders)
			{
				if (snapTurnOverride == null || !snapTurnOverride.TurnOverrideActive())
				{
					this.turningOverriders.Remove(snapTurnOverride);
				}
			}
		}

		// Token: 0x06005568 RID: 21864 RVA: 0x001ADCF4 File Offset: 0x001ABEF4
		public static void DisableSnapTurn()
		{
			Debug.Log("[SNAP_TURN] Disabling Snap Turn");
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			GorillaSnapTurn._cachedTurnFactor = PlayerPrefs.GetInt("turnFactor");
			GorillaSnapTurn._cachedTurnType = PlayerPrefs.GetString("stickTurning");
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode("NONE", 0);
		}

		// Token: 0x06005569 RID: 21865 RVA: 0x001ADD47 File Offset: 0x001ABF47
		public static void UpdateAndSaveTurnType(string mode)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetString("stickTurning", mode);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(mode, GorillaSnapTurn.CachedSnapTurnRef.turnFactor);
		}

		// Token: 0x0600556A RID: 21866 RVA: 0x001ADD86 File Offset: 0x001ABF86
		public static void UpdateAndSaveTurnFactor(int factor)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetInt("turnFactor", factor);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(GorillaSnapTurn.CachedSnapTurnRef.turnType, factor);
		}

		// Token: 0x0600556B RID: 21867 RVA: 0x001ADDC8 File Offset: 0x001ABFC8
		public static void LoadSettingsFromPlayerPrefs()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			string text = (Application.platform == 11) ? "NONE" : "SNAP";
			string @string = PlayerPrefs.GetString("stickTurning", text);
			int @int = PlayerPrefs.GetInt("turnFactor", 4);
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(@string, @int);
		}

		// Token: 0x0600556C RID: 21868 RVA: 0x001ADE20 File Offset: 0x001AC020
		public static void LoadSettingsFromCache()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(GorillaSnapTurn._cachedTurnType))
			{
				GorillaSnapTurn._cachedTurnType = ((Application.platform == 11) ? "NONE" : "SNAP");
			}
			string cachedTurnType = GorillaSnapTurn._cachedTurnType;
			int cachedTurnFactor = GorillaSnapTurn._cachedTurnFactor;
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(cachedTurnType, cachedTurnFactor);
		}

		// Token: 0x0400623C RID: 25148
		[Header("References")]
		[SerializeField]
		private XROrigin xrOrigin;

		// Token: 0x0400623D RID: 25149
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x0400623E RID: 25150
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x0400623F RID: 25151
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x04006240 RID: 25152
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x04006241 RID: 25153
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x04006242 RID: 25154
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x04006243 RID: 25155
		private float m_CurrentTurnAmount;

		// Token: 0x04006244 RID: 25156
		private float m_TimeStarted;

		// Token: 0x04006245 RID: 25157
		private bool m_AxisReset;

		// Token: 0x04006246 RID: 25158
		public float turnSpeed = 1f;

		// Token: 0x04006247 RID: 25159
		private HashSet<ISnapTurnOverride> turningOverriders = new HashSet<ISnapTurnOverride>();

		// Token: 0x04006248 RID: 25160
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x04006249 RID: 25161
		private static int _cachedTurnFactor;

		// Token: 0x0400624A RID: 25162
		private static string _cachedTurnType;

		// Token: 0x0400624B RID: 25163
		private string m_TurnType = "";

		// Token: 0x0400624C RID: 25164
		private int m_TurnFactor = 1;

		// Token: 0x0400624D RID: 25165
		[OnEnterPlay_SetNull]
		private static GorillaSnapTurn _cachedReference;

		// Token: 0x02000D95 RID: 3477
		public enum InputAxes
		{
			// Token: 0x0400624F RID: 25167
			Primary2DAxis,
			// Token: 0x04006250 RID: 25168
			Secondary2DAxis
		}
	}
}
