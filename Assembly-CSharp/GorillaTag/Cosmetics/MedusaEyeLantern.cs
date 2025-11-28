using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010AD RID: 4269
	public class MedusaEyeLantern : MonoBehaviour
	{
		// Token: 0x06006AD4 RID: 27348 RVA: 0x00230840 File Offset: 0x0022EA40
		private void Awake()
		{
			foreach (MedusaEyeLantern.EyeState eyeState in this.allStates)
			{
				this.allStatesDict.Add(eyeState.eyeState, eyeState);
			}
		}

		// Token: 0x06006AD5 RID: 27349 RVA: 0x00230878 File Offset: 0x0022EA78
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x06006AD6 RID: 27350 RVA: 0x00230885 File Offset: 0x0022EA85
		private void Start()
		{
			if (this.rotatingObjectTransform == null)
			{
				this.rotatingObjectTransform = base.transform;
			}
			this.initialRotation = this.rotatingObjectTransform.localRotation;
			this.SwitchState(MedusaEyeLantern.State.DORMANT);
		}

		// Token: 0x06006AD7 RID: 27351 RVA: 0x002308BC File Offset: 0x0022EABC
		private void Update()
		{
			if (!this.transferableParent.InHand() && this.currentState != MedusaEyeLantern.State.DORMANT)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
			if (!this.transferableParent.InHand())
			{
				return;
			}
			this.UpdateState();
			if (this.velocityTracker == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector;
			vector..ctor(averageVelocity.x, 0f, averageVelocity.z);
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			float num = Mathf.Clamp(-normalized.z, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			float num2 = Mathf.Clamp(normalized.x, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			this.targetRotation = this.initialRotation * Quaternion.Euler(num, 0f, num2);
			if (magnitude > this.sloshVelocityThreshold)
			{
				this.SwitchState(MedusaEyeLantern.State.SLOSHING);
			}
			if ((double)magnitude < 0.01)
			{
				this.targetRotation = this.initialRotation;
			}
			if (!this.EyeIsLockedOn())
			{
				this.rotatingObjectTransform.localRotation = Quaternion.Slerp(this.rotatingObjectTransform.localRotation, this.targetRotation, Time.deltaTime * this.rotationSmoothing);
			}
		}

		// Token: 0x06006AD8 RID: 27352 RVA: 0x00230A1A File Offset: 0x0022EC1A
		public void HandleOnNoOneInRange()
		{
			this.SwitchState(MedusaEyeLantern.State.RESET);
			this.resetTargetTime = Time.time;
			this.rotatingObjectTransform.localRotation = this.initialRotation;
		}

		// Token: 0x06006AD9 RID: 27353 RVA: 0x00230A3F File Offset: 0x0022EC3F
		public void HandleOnNewPlayerDetected(VRRig target, float distance)
		{
			this.targetRig = target;
			if (this.currentState != MedusaEyeLantern.State.SLOSHING)
			{
				this.SwitchState(MedusaEyeLantern.State.TRACKING);
			}
		}

		// Token: 0x06006ADA RID: 27354 RVA: 0x00230A58 File Offset: 0x0022EC58
		private void Sloshing()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector;
			vector..ctor(averageVelocity.x, 0f, averageVelocity.z);
			if ((double)vector.magnitude < 0.01)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
		}

		// Token: 0x06006ADB RID: 27355 RVA: 0x00230AAC File Offset: 0x0022ECAC
		private void FaceTarget()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 normalized = (this.targetRig.tagSound.transform.position - this.rotatingObjectTransform.position).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.z).normalized;
			Debug.DrawRay(this.rotatingObjectTransform.position, this.rotatingObjectTransform.forward * 0.3f, Color.blue);
			Debug.DrawRay(this.rotatingObjectTransform.position, normalized2 * 0.3f, Color.green);
			if (normalized2.sqrMagnitude > 0.001f)
			{
				float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(this.rotatingObjectTransform.forward.normalized, normalized2), -1f, 1f)) * 57.29578f;
				if (180f - num < this.targetHeadAngleThreshold && this.currentState == MedusaEyeLantern.State.TRACKING)
				{
					this.SwitchState(MedusaEyeLantern.State.WARMUP);
					return;
				}
				Quaternion quaternion = Quaternion.LookRotation(-normalized2, Vector3.up);
				this.rotatingObjectTransform.rotation = Quaternion.RotateTowards(this.rotatingObjectTransform.rotation, quaternion, this.lookAtTargetSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06006ADC RID: 27356 RVA: 0x00230C10 File Offset: 0x0022EE10
		private bool IsTargetLookingAtEye()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return false;
			}
			Transform transform = this.targetRig.tagSound.transform;
			Vector3 normalized = (this.rotatingObjectTransform.position - this.rotatingObjectTransform.forward * this.faceDistanceOffset - transform.position).normalized;
			float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.up.normalized, normalized), -1f, 1f)) * 57.29578f;
			Debug.DrawRay(transform.position, transform.up * 0.3f, Color.magenta);
			Debug.DrawRay(transform.position, normalized * 0.3f, Color.yellow);
			return num < this.lookAtEyeAngleThreshold;
		}

		// Token: 0x06006ADD RID: 27357 RVA: 0x00230CF8 File Offset: 0x0022EEF8
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case MedusaEyeLantern.State.SLOSHING:
				this.Sloshing();
				break;
			case MedusaEyeLantern.State.DORMANT:
				this.warmupCounter = 0f;
				this.petrificationStarted = float.PositiveInfinity;
				if (this.targetRig != null && (this.targetRig.transform.position - base.transform.position).IsShorterThan(this.distanceChecker.distanceThreshold))
				{
					this.SwitchState(MedusaEyeLantern.State.TRACKING);
				}
				break;
			case MedusaEyeLantern.State.TRACKING:
				this.FaceTarget();
				break;
			case MedusaEyeLantern.State.WARMUP:
				this.warmupCounter += Time.deltaTime;
				this.FaceTarget();
				if (this.warmupCounter > this.warmUpProgressTime)
				{
					this.SwitchState(MedusaEyeLantern.State.PRIMING);
					this.warmupCounter = 0f;
				}
				break;
			case MedusaEyeLantern.State.PRIMING:
				this.FaceTarget();
				if (this.IsTargetLookingAtEye())
				{
					UnityEvent<VRRig> onPetrification = this.OnPetrification;
					if (onPetrification != null)
					{
						onPetrification.Invoke(this.targetRig);
					}
					this.SwitchState(MedusaEyeLantern.State.PETRIFICATION);
					this.petrificationStarted = Time.time;
				}
				break;
			case MedusaEyeLantern.State.PETRIFICATION:
				if (Time.time - this.petrificationStarted > this.petrificationDuration)
				{
					this.SwitchState(MedusaEyeLantern.State.COOLDOWN);
				}
				break;
			case MedusaEyeLantern.State.COOLDOWN:
				if (Time.time - this.petrificationStarted > this.resetCooldown)
				{
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
					this.petrificationStarted = float.PositiveInfinity;
				}
				break;
			case MedusaEyeLantern.State.RESET:
				if (Time.time - this.resetTargetTime > this.resetTargetTimer)
				{
					this.resetTargetTime = float.PositiveInfinity;
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
				}
				break;
			}
			this.PlayHaptic(this.currentState);
		}

		// Token: 0x06006ADE RID: 27358 RVA: 0x00230EA8 File Offset: 0x0022F0A8
		private void SwitchState(MedusaEyeLantern.State newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			MedusaEyeLantern.EyeState eyeState;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(newState, ref eyeState))
			{
				UnityEvent onEnterState = eyeState.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
			}
			MedusaEyeLantern.EyeState eyeState2;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(this.lastState, ref eyeState2))
			{
				UnityEvent onExitState = eyeState2.onExitState;
				if (onExitState == null)
				{
					return;
				}
				onExitState.Invoke();
			}
		}

		// Token: 0x06006ADF RID: 27359 RVA: 0x00230F2C File Offset: 0x0022F12C
		private void PlayHaptic(MedusaEyeLantern.State state)
		{
			if (!this.transferableParent.IsMyItem())
			{
				return;
			}
			MedusaEyeLantern.EyeState eyeState;
			this.allStatesDict.TryGetValue(state, ref eyeState);
			if (this.currentState == MedusaEyeLantern.State.WARMUP)
			{
				float num = Mathf.Clamp01(this.warmupCounter / this.warmUpProgressTime);
				if (eyeState != null && eyeState.hapticStrength != null)
				{
					float amplitude = eyeState.hapticStrength.Evaluate(num);
					bool forLeftController = this.transferableParent.InLeftHand();
					GorillaTagger.Instance.StartVibration(forLeftController, amplitude, Time.deltaTime);
					return;
				}
			}
			else if (eyeState != null && eyeState.hapticStrength != null)
			{
				float amplitude2 = eyeState.hapticStrength.Evaluate(0.5f);
				bool forLeftController2 = this.transferableParent.InLeftHand();
				GorillaTagger.Instance.StartVibration(forLeftController2, amplitude2, Time.deltaTime);
			}
		}

		// Token: 0x06006AE0 RID: 27360 RVA: 0x00230FE5 File Offset: 0x0022F1E5
		private bool EyeIsLockedOn()
		{
			return this.currentState == MedusaEyeLantern.State.TRACKING || this.currentState == MedusaEyeLantern.State.WARMUP || this.currentState == MedusaEyeLantern.State.PRIMING;
		}

		// Token: 0x04007AF8 RID: 31480
		[SerializeField]
		private DistanceCheckerCosmetic distanceChecker;

		// Token: 0x04007AF9 RID: 31481
		[SerializeField]
		private TransferrableObject transferableParent;

		// Token: 0x04007AFA RID: 31482
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04007AFB RID: 31483
		[SerializeField]
		private Transform rotatingObjectTransform;

		// Token: 0x04007AFC RID: 31484
		[Space]
		[Header("Rotation Settings")]
		[SerializeField]
		private float maxRotationAngle = 50f;

		// Token: 0x04007AFD RID: 31485
		[SerializeField]
		private float sloshVelocityThreshold = 1f;

		// Token: 0x04007AFE RID: 31486
		[SerializeField]
		private float rotationSmoothing = 10f;

		// Token: 0x04007AFF RID: 31487
		[SerializeField]
		private float rotationSpeedMultiplier = 5f;

		// Token: 0x04007B00 RID: 31488
		[Space]
		[Header("Target Tracking Settings")]
		[SerializeField]
		private float lookAtEyeAngleThreshold = 90f;

		// Token: 0x04007B01 RID: 31489
		[SerializeField]
		private float targetHeadAngleThreshold = 5f;

		// Token: 0x04007B02 RID: 31490
		[SerializeField]
		private float lookAtTargetSpeed = 5f;

		// Token: 0x04007B03 RID: 31491
		[SerializeField]
		private float warmUpProgressTime = 3f;

		// Token: 0x04007B04 RID: 31492
		[SerializeField]
		private float resetCooldown = 5f;

		// Token: 0x04007B05 RID: 31493
		[SerializeField]
		private float faceDistanceOffset = 0.2f;

		// Token: 0x04007B06 RID: 31494
		[SerializeField]
		private float petrificationDuration = 0.2f;

		// Token: 0x04007B07 RID: 31495
		[Space]
		[Header("Eye State Settings")]
		public MedusaEyeLantern.EyeState[] allStates = new MedusaEyeLantern.EyeState[0];

		// Token: 0x04007B08 RID: 31496
		public UnityEvent<VRRig> OnPetrification;

		// Token: 0x04007B09 RID: 31497
		private Quaternion initialRotation;

		// Token: 0x04007B0A RID: 31498
		private Quaternion targetRotation;

		// Token: 0x04007B0B RID: 31499
		private MedusaEyeLantern.State currentState;

		// Token: 0x04007B0C RID: 31500
		private MedusaEyeLantern.State lastState;

		// Token: 0x04007B0D RID: 31501
		private float petrificationStarted = float.PositiveInfinity;

		// Token: 0x04007B0E RID: 31502
		private float warmupCounter;

		// Token: 0x04007B0F RID: 31503
		private Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState> allStatesDict = new Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState>();

		// Token: 0x04007B10 RID: 31504
		private VRRig targetRig;

		// Token: 0x04007B11 RID: 31505
		private float resetTargetTimer = 1f;

		// Token: 0x04007B12 RID: 31506
		private float resetTargetTime = float.PositiveInfinity;

		// Token: 0x020010AE RID: 4270
		[Serializable]
		public class EyeState
		{
			// Token: 0x04007B13 RID: 31507
			public MedusaEyeLantern.State eyeState;

			// Token: 0x04007B14 RID: 31508
			public AnimationCurve hapticStrength;

			// Token: 0x04007B15 RID: 31509
			public UnityEvent onEnterState;

			// Token: 0x04007B16 RID: 31510
			public UnityEvent onExitState;
		}

		// Token: 0x020010AF RID: 4271
		public enum State
		{
			// Token: 0x04007B18 RID: 31512
			SLOSHING,
			// Token: 0x04007B19 RID: 31513
			DORMANT,
			// Token: 0x04007B1A RID: 31514
			TRACKING,
			// Token: 0x04007B1B RID: 31515
			WARMUP,
			// Token: 0x04007B1C RID: 31516
			PRIMING,
			// Token: 0x04007B1D RID: 31517
			PETRIFICATION,
			// Token: 0x04007B1E RID: 31518
			COOLDOWN,
			// Token: 0x04007B1F RID: 31519
			RESET
		}
	}
}
