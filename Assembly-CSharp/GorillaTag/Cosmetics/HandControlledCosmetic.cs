using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010FD RID: 4349
	public class HandControlledCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006CEB RID: 27883 RVA: 0x0023C4D4 File Offset: 0x0023A6D4
		public void Awake()
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			this.initialRotation = base.transform.localRotation;
			base.enabled = false;
			if (this.debugRelativePositionTransform1 != null)
			{
				Object.Destroy(this.debugRelativePositionTransform1.gameObject);
			}
			if (this.debugRelativePositionTransform2 != null)
			{
				Object.Destroy(this.debugRelativePositionTransform2.gameObject);
			}
		}

		// Token: 0x06006CEC RID: 27884 RVA: 0x0023C544 File Offset: 0x0023A744
		private void SetControlIndicatorPoints()
		{
			if (this.myRig.isOfflineVRRig && this.controllingHand != null && this.controlIndicatorCurve != null && this.controlIndicatorCurve.points != null)
			{
				this.controlIndicatorCurve.points[0] = this.controllingHand.position;
				this.controlIndicatorCurve.points[1] = this.controlIndicatorCurve.points[0] + this.myRig.scaleFactor * this.controllingHand.up;
				this.controlIndicatorCurve.points[2] = base.transform.position;
			}
		}

		// Token: 0x06006CED RID: 27885 RVA: 0x0023C60A File Offset: 0x0023A80A
		private Vector3 GetRelativeHandPosition()
		{
			return this.controllingHand.TransformPoint(this.handPositionOffset) - this.myRig.bodyTransform.position;
		}

		// Token: 0x06006CEE RID: 27886 RVA: 0x0023C634 File Offset: 0x0023A834
		public void StartControl(bool leftHand, float flexValue)
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.lowAngleLimits = this.activeSettings.angleLimits;
			this.highAngleLimits = 360f * Vector3.one - this.lowAngleLimits;
			this.handRotationOffset = (leftHand ? this.leftHandRotation : this.rightHandRotation);
			this.controllingHand = (leftHand ? this.myRig.leftHand.rigTarget.transform : this.myRig.rightHand.rigTarget.transform);
			this.startHandRelativePosition = this.GetRelativeHandPosition();
			this.startHandInverseRotation = Quaternion.Inverse(this.controllingHand.rotation * this.handRotationOffset);
			this.isActive = true;
			this.SetControlIndicatorPoints();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006CEF RID: 27887 RVA: 0x0023C714 File Offset: 0x0023A914
		public void StopControl()
		{
			this.localEuler = base.transform.localRotation.eulerAngles;
			this.isActive = false;
			this.SetControlIndicatorPoints();
		}

		// Token: 0x06006CF0 RID: 27888 RVA: 0x00002789 File Offset: 0x00000989
		public void OnEnable()
		{
		}

		// Token: 0x06006CF1 RID: 27889 RVA: 0x0023C747 File Offset: 0x0023A947
		public void OnDisable()
		{
			base.transform.localRotation = this.initialRotation;
			this.StopControl();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006CF2 RID: 27890 RVA: 0x0023C766 File Offset: 0x0023A966
		private float ReverseClampDegrees(float value, float low, float high)
		{
			value = Mathf.Repeat(value, 360f);
			if (value <= low || value >= high)
			{
				return value;
			}
			if (value >= 180f)
			{
				return high;
			}
			return low;
		}

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06006CF3 RID: 27891 RVA: 0x0023C78A File Offset: 0x0023A98A
		// (set) Token: 0x06006CF4 RID: 27892 RVA: 0x0023C792 File Offset: 0x0023A992
		public bool TickRunning { get; set; }

		// Token: 0x06006CF5 RID: 27893 RVA: 0x0023C79C File Offset: 0x0023A99C
		public void Tick()
		{
			if (this.isActive)
			{
				HandControlledCosmetic.RotationControl rotationControl = this.activeSettings.rotationControl;
				if (rotationControl != HandControlledCosmetic.RotationControl.Angle)
				{
					if (rotationControl == HandControlledCosmetic.RotationControl.Translation)
					{
						Vector3 relativeHandPosition = this.GetRelativeHandPosition();
						Vector3 vector;
						vector..ctor(relativeHandPosition.x, 0f, relativeHandPosition.z);
						float num = Vector3.SignedAngle(new Vector3(this.startHandRelativePosition.x, 0f, this.startHandRelativePosition.z), vector, Vector3.up);
						float num2 = 50f * (this.startHandRelativePosition.y - relativeHandPosition.y) / this.myRig.scaleFactor;
						float num3 = Vector3.Distance(this.startHandRelativePosition, relativeHandPosition) / this.myRig.scaleFactor;
						this.localEuler += Time.deltaTime * new Vector3(this.activeSettings.verticalSensitivity.Evaluate(num3) * num2, this.activeSettings.horizontalSensitivity.Evaluate(num3) * num, 0f);
						this.startHandRelativePosition = Vector3.MoveTowards(this.startHandRelativePosition, relativeHandPosition, Time.deltaTime * this.activeSettings.inputDecayCurve.Evaluate(num3));
					}
				}
				else
				{
					Quaternion quaternion = this.controllingHand.rotation * this.handRotationOffset;
					Quaternion quaternion2 = this.startHandInverseRotation * quaternion;
					this.localEuler += this.activeSettings.inputSensitivity * quaternion2.eulerAngles;
					float num4 = 1f - Mathf.Exp(-this.activeSettings.inputDecaySpeed * Time.deltaTime);
					this.startHandInverseRotation = Quaternion.Slerp(this.startHandInverseRotation, Quaternion.Inverse(quaternion), num4);
				}
				for (int i = 0; i < 3; i++)
				{
					this.localEuler[i] = this.ReverseClampDegrees(this.localEuler[i], this.lowAngleLimits[i], this.highAngleLimits[i]);
				}
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(this.localEuler), 1f - Mathf.Exp(-this.activeSettings.rotationSpeed * Time.deltaTime));
				return;
			}
			Quaternion localRotation = Quaternion.Slerp(base.transform.localRotation, this.initialRotation, 1f - Mathf.Exp(-this.inactiveSettings.rotationSpeed * Time.deltaTime));
			base.transform.localRotation = localRotation;
			this.localEuler = localRotation.eulerAngles;
		}

		// Token: 0x04007E05 RID: 32261
		[SerializeField]
		private HandControlledSettingsSO activeSettings;

		// Token: 0x04007E06 RID: 32262
		[SerializeField]
		private HandControlledSettingsSO inactiveSettings;

		// Token: 0x04007E07 RID: 32263
		[SerializeField]
		private Vector3 handPositionOffset;

		// Token: 0x04007E08 RID: 32264
		[SerializeField]
		private Quaternion rightHandRotation;

		// Token: 0x04007E09 RID: 32265
		[SerializeField]
		private Quaternion leftHandRotation;

		// Token: 0x04007E0A RID: 32266
		private Quaternion handRotationOffset;

		// Token: 0x04007E0B RID: 32267
		[SerializeField]
		private BezierCurve controlIndicatorCurve;

		// Token: 0x04007E0C RID: 32268
		[SerializeField]
		private Transform debugRelativePositionTransform1;

		// Token: 0x04007E0D RID: 32269
		[SerializeField]
		private Transform debugRelativePositionTransform2;

		// Token: 0x04007E0E RID: 32270
		private VRRig myRig;

		// Token: 0x04007E0F RID: 32271
		private Transform controllingHand;

		// Token: 0x04007E10 RID: 32272
		private Vector3 startHandRelativePosition;

		// Token: 0x04007E11 RID: 32273
		private Vector3 lowAngleLimits;

		// Token: 0x04007E12 RID: 32274
		private Vector3 highAngleLimits;

		// Token: 0x04007E13 RID: 32275
		private Vector3 localEuler;

		// Token: 0x04007E14 RID: 32276
		private Quaternion startHandInverseRotation;

		// Token: 0x04007E15 RID: 32277
		private Quaternion initialRotation;

		// Token: 0x04007E16 RID: 32278
		private bool isActive;

		// Token: 0x020010FE RID: 4350
		public enum RotationControl
		{
			// Token: 0x04007E19 RID: 32281
			Angle,
			// Token: 0x04007E1A RID: 32282
			Translation
		}
	}
}
