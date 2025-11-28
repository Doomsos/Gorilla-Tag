using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001128 RID: 4392
	[RequireComponent(typeof(Collider))]
	public class CosmeticSwipeReactor : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006DDF RID: 28127 RVA: 0x0024125C File Offset: 0x0023F45C
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			if (this._rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this._rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = (this._rig != null && this._rig.isLocal);
			this.col = base.GetComponent<Collider>();
			switch (this.localSwipeAxis)
			{
			case CosmeticSwipeReactor.Axis.X:
				this.swipeDir = Vector3.right;
				return;
			case CosmeticSwipeReactor.Axis.Y:
				this.swipeDir = Vector3.up;
				return;
			case CosmeticSwipeReactor.Axis.Z:
				this.swipeDir = Vector3.forward;
				return;
			default:
				return;
			}
		}

		// Token: 0x06006DE0 RID: 28128 RVA: 0x00241314 File Offset: 0x0023F514
		private void OnTriggerEnter(Collider other)
		{
			if (!this.isLocal || !base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				if (component.isLeftHand)
				{
					this.handIndicatorL = component;
					Vector3 pos = base.transform.InverseTransformPoint(component.transform.position);
					this.ResetProgress(true, pos);
					this.handInTriggerL = true;
				}
				else
				{
					this.handIndicatorR = component;
					Vector3 pos2 = base.transform.InverseTransformPoint(component.transform.position);
					this.ResetProgress(false, pos2);
					this.handInTriggerR = true;
				}
			}
			if ((this.handInTriggerL || this.handInTriggerR) && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006DE1 RID: 28129 RVA: 0x002413C4 File Offset: 0x0023F5C4
		private void OnTriggerExit(Collider other)
		{
			if (!this.isLocal || !base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component != null)
			{
				if (component.isLeftHand)
				{
					this.handInTriggerL = false;
					if (this.resetCooldownOnTriggerExit)
					{
						this.isCoolingDownL = false;
						this.cooldownEndL = double.MinValue;
					}
				}
				else
				{
					this.handInTriggerR = false;
					if (this.resetCooldownOnTriggerExit)
					{
						this.isCoolingDownR = false;
						this.cooldownEndR = double.MinValue;
					}
				}
			}
			if (!this.handInTriggerL && !this.handInTriggerR && this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x06006DE2 RID: 28130 RVA: 0x00241464 File Offset: 0x0023F664
		// (set) Token: 0x06006DE3 RID: 28131 RVA: 0x0024146C File Offset: 0x0023F66C
		public bool TickRunning { get; set; }

		// Token: 0x06006DE4 RID: 28132 RVA: 0x00241478 File Offset: 0x0023F678
		public void Tick()
		{
			if (this.handInTriggerL)
			{
				this.ProcessHandMovement(this.handIndicatorL, this.startPosL, ref this.lastFramePosL, ref this.swipingUpL, ref this.distanceL, ref this.isCoolingDownL, ref this.cooldownEndL);
			}
			if (this.handInTriggerR)
			{
				this.ProcessHandMovement(this.handIndicatorR, this.startPosR, ref this.lastFramePosR, ref this.swipingUpR, ref this.distanceR, ref this.isCoolingDownR, ref this.cooldownEndR);
			}
			if (!this.handInTriggerL && !this.handInTriggerR && this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x06006DE5 RID: 28133 RVA: 0x00241514 File Offset: 0x0023F714
		private void ResetProgress(bool left, Vector3 pos)
		{
			if (left)
			{
				this.startPosL = pos;
				this.lastFramePosL = this.startPosL;
				this.distanceL = 0f;
				return;
			}
			this.startPosR = pos;
			this.lastFramePosR = this.startPosR;
			this.distanceR = 0f;
		}

		// Token: 0x06006DE6 RID: 28134 RVA: 0x00241564 File Offset: 0x0023F764
		private void ProcessHandMovement(GorillaTriggerColliderHandIndicator hand, Vector3 start, ref Vector3 last, ref bool swipingUp, ref float dist, ref bool isCoolingDown, ref double cooldownEndTime)
		{
			if (isCoolingDown)
			{
				if (Time.timeAsDouble < cooldownEndTime)
				{
					return;
				}
				isCoolingDown = false;
				cooldownEndTime = double.MinValue;
				this.ResetProgress(hand.isLeftHand, base.transform.InverseTransformPoint(hand.transform.position));
				return;
			}
			else
			{
				Vector3 vector = base.transform.InverseTransformPoint(hand.transform.position);
				float num = Mathf.Abs(this.GetAxisComponent(hand.currentVelocity));
				if (num < this.minimumVelocity * this._rig.scaleFactor || num > this.maximumVelocity * this._rig.scaleFactor)
				{
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				float num2 = this.GetAxisComponent(vector) - this.GetAxisComponent(last);
				if (num2 >= 0f && !swipingUp)
				{
					swipingUp = true;
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				if (num2 < 0f & swipingUp)
				{
					swipingUp = false;
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				if ((this.GetLateralMovement(start) - this.GetLateralMovement(vector)).sqrMagnitude > this.lateralMovementTolerance * this.lateralMovementTolerance)
				{
					this.ResetProgress(hand.isLeftHand, vector);
					return;
				}
				last = vector;
				dist += Mathf.Abs(num2);
				GorillaTagger.Instance.StartVibration(hand.isLeftHand, this.swipeHaptics.Evaluate(dist / this.swipeDistance), Time.deltaTime);
				if (dist >= this.swipeDistance)
				{
					if (swipingUp)
					{
						UnityEvent<bool> onSwipe = this.OnSwipe;
						if (onSwipe != null)
						{
							onSwipe.Invoke(hand.isLeftHand);
						}
						cooldownEndTime = Time.timeAsDouble + (double)this.swipeCooldown;
						isCoolingDown = true;
					}
					else
					{
						UnityEvent<bool> onReverseSwipe = this.OnReverseSwipe;
						if (onReverseSwipe != null)
						{
							onReverseSwipe.Invoke(hand.isLeftHand);
						}
						cooldownEndTime = Time.timeAsDouble + (double)this.swipeCooldown;
						isCoolingDown = true;
					}
					this.ResetProgress(hand.isLeftHand, vector);
				}
				return;
			}
		}

		// Token: 0x06006DE7 RID: 28135 RVA: 0x00241754 File Offset: 0x0023F954
		private float GetAxisComponent(Vector3 vec)
		{
			CosmeticSwipeReactor.Axis axis = this.localSwipeAxis;
			if (axis == CosmeticSwipeReactor.Axis.X)
			{
				return vec.x;
			}
			if (axis != CosmeticSwipeReactor.Axis.Y)
			{
				return vec.z;
			}
			return vec.y;
		}

		// Token: 0x06006DE8 RID: 28136 RVA: 0x00241788 File Offset: 0x0023F988
		private Vector2 GetLateralMovement(Vector3 vec)
		{
			CosmeticSwipeReactor.Axis axis = this.localSwipeAxis;
			if (axis == CosmeticSwipeReactor.Axis.X)
			{
				return new Vector2(vec.y, vec.z);
			}
			if (axis != CosmeticSwipeReactor.Axis.Y)
			{
				return new Vector2(vec.x, vec.y);
			}
			return new Vector2(vec.x, vec.z);
		}

		// Token: 0x04007F83 RID: 32643
		[SerializeField]
		private CosmeticSwipeReactor.Axis localSwipeAxis = CosmeticSwipeReactor.Axis.Y;

		// Token: 0x04007F84 RID: 32644
		private Vector3 swipeDir = Vector3.up;

		// Token: 0x04007F85 RID: 32645
		[Tooltip("Distance hand can move perpindicular to the swipe without cancelling the gesture")]
		[SerializeField]
		private float lateralMovementTolerance = 0.1f;

		// Token: 0x04007F86 RID: 32646
		[Tooltip("How far the hand has to move along the axis to count as a swipe\nThis distance must be contained within the trigger area")]
		[SerializeField]
		private float swipeDistance = 0.3f;

		// Token: 0x04007F87 RID: 32647
		[SerializeField]
		private float minimumVelocity = 0.1f;

		// Token: 0x04007F88 RID: 32648
		[SerializeField]
		private float maximumVelocity = 3f;

		// Token: 0x04007F89 RID: 32649
		[Tooltip("Delay after completing a swipe before starting the next")]
		[SerializeField]
		private float swipeCooldown = 0.25f;

		// Token: 0x04007F8A RID: 32650
		[SerializeField]
		private bool resetCooldownOnTriggerExit = true;

		// Token: 0x04007F8B RID: 32651
		[Tooltip("Amplitude of haptics from normalized swiped distance")]
		[SerializeField]
		private AnimationCurve swipeHaptics = AnimationCurve.EaseInOut(0f, 0.02f, 1f, 0.5f);

		// Token: 0x04007F8C RID: 32652
		public UnityEvent<bool> OnSwipe;

		// Token: 0x04007F8D RID: 32653
		public UnityEvent<bool> OnReverseSwipe;

		// Token: 0x04007F8E RID: 32654
		private VRRig _rig;

		// Token: 0x04007F8F RID: 32655
		private Collider col;

		// Token: 0x04007F90 RID: 32656
		private bool isLocal;

		// Token: 0x04007F91 RID: 32657
		private bool handInTriggerR;

		// Token: 0x04007F92 RID: 32658
		private bool handInTriggerL;

		// Token: 0x04007F93 RID: 32659
		private GorillaTriggerColliderHandIndicator handIndicatorR;

		// Token: 0x04007F94 RID: 32660
		private GorillaTriggerColliderHandIndicator handIndicatorL;

		// Token: 0x04007F95 RID: 32661
		private Vector3 startPosR;

		// Token: 0x04007F96 RID: 32662
		private Vector3 startPosL;

		// Token: 0x04007F97 RID: 32663
		private Vector3 lastFramePosR;

		// Token: 0x04007F98 RID: 32664
		private Vector3 lastFramePosL;

		// Token: 0x04007F99 RID: 32665
		private float distanceR;

		// Token: 0x04007F9A RID: 32666
		private float distanceL;

		// Token: 0x04007F9B RID: 32667
		private bool swipingUpL;

		// Token: 0x04007F9C RID: 32668
		private bool swipingUpR;

		// Token: 0x04007F9D RID: 32669
		private double cooldownEndL = double.MinValue;

		// Token: 0x04007F9E RID: 32670
		private double cooldownEndR = double.MinValue;

		// Token: 0x04007F9F RID: 32671
		private bool isCoolingDownL;

		// Token: 0x04007FA0 RID: 32672
		private bool isCoolingDownR;

		// Token: 0x02001129 RID: 4393
		public enum Axis
		{
			// Token: 0x04007FA3 RID: 32675
			X,
			// Token: 0x04007FA4 RID: 32676
			Y,
			// Token: 0x04007FA5 RID: 32677
			Z
		}
	}
}
