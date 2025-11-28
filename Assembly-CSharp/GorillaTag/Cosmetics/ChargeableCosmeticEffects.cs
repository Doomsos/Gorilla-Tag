using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010C0 RID: 4288
	public class ChargeableCosmeticEffects : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006B7E RID: 27518 RVA: 0x00234966 File Offset: 0x00232B66
		private bool HasFractionals()
		{
			return this.continuousProperties.Count > 0 || this.whileCharging.GetPersistentEventCount() > 0;
		}

		// Token: 0x06006B7F RID: 27519 RVA: 0x00234986 File Offset: 0x00232B86
		private void Awake()
		{
			this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
			this.hasFractionalsCached = this.HasFractionals();
		}

		// Token: 0x06006B80 RID: 27520 RVA: 0x002349A6 File Offset: 0x00232BA6
		public void SetMaxChargeSeconds(float s)
		{
			this.maxChargeSeconds = s;
			this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
			this.SetChargeTime(this.chargeTime);
		}

		// Token: 0x06006B81 RID: 27521 RVA: 0x002349CD File Offset: 0x00232BCD
		public void SetChargeState(bool state)
		{
			if (this.isCharging != state)
			{
				TickSystem<object>.AddTickCallback(this);
				this.isCharging = state;
			}
		}

		// Token: 0x06006B82 RID: 27522 RVA: 0x002349E5 File Offset: 0x00232BE5
		public void StartCharging()
		{
			this.SetChargeState(true);
		}

		// Token: 0x06006B83 RID: 27523 RVA: 0x002349EE File Offset: 0x00232BEE
		public void StopCharging()
		{
			this.SetChargeState(false);
		}

		// Token: 0x06006B84 RID: 27524 RVA: 0x002349F7 File Offset: 0x00232BF7
		public void ToggleCharging()
		{
			this.SetChargeState(!this.isCharging);
		}

		// Token: 0x06006B85 RID: 27525 RVA: 0x00234A08 File Offset: 0x00232C08
		public void SetChargeTime(float t)
		{
			if (t >= this.maxChargeSeconds)
			{
				if (this.chargeTime < this.maxChargeSeconds)
				{
					this.RunMaxCharge();
					return;
				}
			}
			else if (t <= 0f)
			{
				if (this.chargeTime > 0f)
				{
					this.RunNoCharge();
					return;
				}
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
				this.chargeTime = t;
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
				}
			}
		}

		// Token: 0x06006B86 RID: 27526 RVA: 0x00234A6A File Offset: 0x00232C6A
		public void SetChargeFrac(float f)
		{
			this.SetChargeTime(f * this.maxChargeSeconds);
		}

		// Token: 0x06006B87 RID: 27527 RVA: 0x00234A7A File Offset: 0x00232C7A
		public void EmptyCharge()
		{
			this.SetChargeTime(0f);
		}

		// Token: 0x06006B88 RID: 27528 RVA: 0x00234A87 File Offset: 0x00232C87
		public void FillCharge()
		{
			this.SetChargeTime(this.maxChargeSeconds);
		}

		// Token: 0x06006B89 RID: 27529 RVA: 0x00234A95 File Offset: 0x00232C95
		public void EmptyAndStop()
		{
			this.isCharging = false;
			this.EmptyCharge();
		}

		// Token: 0x06006B8A RID: 27530 RVA: 0x00234AA4 File Offset: 0x00232CA4
		public void FillAndStop()
		{
			this.StopCharging();
			this.FillCharge();
		}

		// Token: 0x06006B8B RID: 27531 RVA: 0x00234AB2 File Offset: 0x00232CB2
		public void EmptyAndStart()
		{
			this.StartCharging();
			this.EmptyCharge();
		}

		// Token: 0x06006B8C RID: 27532 RVA: 0x00234AC0 File Offset: 0x00232CC0
		public void FillAndStart()
		{
			this.isCharging = true;
			this.FillCharge();
		}

		// Token: 0x06006B8D RID: 27533 RVA: 0x00234AD0 File Offset: 0x00232CD0
		private void OnEnable()
		{
			if ((this.chargeTime <= 0f && this.isCharging) || (this.chargeTime >= this.maxChargeSeconds && !this.isCharging) || (this.chargeTime > 0f && this.chargeTime < this.maxChargeSeconds))
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006B8E RID: 27534 RVA: 0x00018787 File Offset: 0x00016987
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006B8F RID: 27535 RVA: 0x00234B2C File Offset: 0x00232D2C
		private void RunMaxCharge()
		{
			if (this.isCharging)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.chargeTime = this.maxChargeSeconds;
			UnityEvent unityEvent = this.onMaxCharge;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(1f);
			}
			this.continuousProperties.ApplyAll(1f);
		}

		// Token: 0x06006B90 RID: 27536 RVA: 0x00234B94 File Offset: 0x00232D94
		private void RunNoCharge()
		{
			if (!this.isCharging)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.chargeTime = 0f;
			UnityEvent unityEvent = this.onNoCharge;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(0f);
			}
			this.continuousProperties.ApplyAll(0f);
		}

		// Token: 0x06006B91 RID: 27537 RVA: 0x00234BFC File Offset: 0x00232DFC
		private void RunChargeFrac()
		{
			float num = this.masterChargeRemapCurve.Evaluate(this.chargeTime * this.inverseMaxChargeSeconds);
			UnityEvent<float> unityEvent = this.whileCharging;
			if (unityEvent != null)
			{
				unityEvent.Invoke(num);
			}
			this.continuousProperties.ApplyAll(num);
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06006B92 RID: 27538 RVA: 0x00234C40 File Offset: 0x00232E40
		// (set) Token: 0x06006B93 RID: 27539 RVA: 0x00234C48 File Offset: 0x00232E48
		public bool TickRunning { get; set; }

		// Token: 0x06006B94 RID: 27540 RVA: 0x00234C54 File Offset: 0x00232E54
		public void Tick()
		{
			if (this.isCharging && this.chargeTime < this.maxChargeSeconds)
			{
				this.chargeTime += Time.deltaTime * this.chargeGainSpeed;
				if (this.chargeTime >= this.maxChargeSeconds)
				{
					this.RunMaxCharge();
					return;
				}
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
					return;
				}
			}
			else if (!this.isCharging && this.chargeTime > 0f)
			{
				this.chargeTime -= Time.deltaTime * this.chargeLossSpeed;
				if (this.chargeTime <= 0f)
				{
					this.RunNoCharge();
					return;
				}
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
				}
			}
		}

		// Token: 0x04007BF4 RID: 31732
		[SerializeField]
		private float maxChargeSeconds = 1f;

		// Token: 0x04007BF5 RID: 31733
		[SerializeField]
		private float chargeGainSpeed = 1f;

		// Token: 0x04007BF6 RID: 31734
		[SerializeField]
		private float chargeLossSpeed = 1f;

		// Token: 0x04007BF7 RID: 31735
		[Tooltip("This will remap the internal charge output to whatever you set. The remapped value will be output by 'whileCharging' and the 'continuousProperties' (keep in mind that the remapped value will then be used as an INPUT for the curves on each ContinuousProperty).\n\nIt should start at (0,0) and end at (1,1).\n\nDisabled if there are no ContinuousProperties and no whileCharging event callbacks.")]
		[SerializeField]
		private AnimationCurve masterChargeRemapCurve = AnimationCurves.Linear;

		// Token: 0x04007BF8 RID: 31736
		[SerializeField]
		private bool isCharging;

		// Token: 0x04007BF9 RID: 31737
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x04007BFA RID: 31738
		[SerializeField]
		private UnityEvent<float> whileCharging;

		// Token: 0x04007BFB RID: 31739
		[SerializeField]
		private UnityEvent onMaxCharge;

		// Token: 0x04007BFC RID: 31740
		[SerializeField]
		private UnityEvent onNoCharge;

		// Token: 0x04007BFD RID: 31741
		private float chargeTime;

		// Token: 0x04007BFE RID: 31742
		private float inverseMaxChargeSeconds;

		// Token: 0x04007BFF RID: 31743
		private bool hasFractionalsCached;
	}
}
