using System;
using UnityEngine;

// Token: 0x020005DE RID: 1502
public class FishingRod : TransferrableObject
{
	// Token: 0x060025C9 RID: 9673 RVA: 0x000C9C9C File Offset: 0x000C7E9C
	public override void OnActivate()
	{
		base.OnActivate();
		Transform transform = base.transform;
		Vector3 vector = transform.up + transform.forward * 640f;
		this.bobRigidbody.AddForce(vector, 1);
		this.line.tensionScale = 0.86f;
		this.ReelOut();
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000C9CF5 File Offset: 0x000C7EF5
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.line.tensionScale = 1f;
		this.ReelStop();
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000C9D13 File Offset: 0x000C7F13
	protected override void Start()
	{
		base.Start();
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000C9D27 File Offset: 0x000C7F27
	public void SetBobFloat(bool enable)
	{
		if (!this.bobRigidbody)
		{
			return;
		}
		this._bobFloatPlaneY = this.bobRigidbody.position.y;
		this._bobFloating = enable;
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000C9D54 File Offset: 0x000C7F54
	private void QuickReel()
	{
		if (this._lineResizing)
		{
			return;
		}
		this.bobCollider.enabled = false;
		this.ReelIn();
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000C9D74 File Offset: 0x000C7F74
	public bool IsFreeHandGripping()
	{
		bool flag = base.InLeftHand();
		Transform transform = flag ? this.rig.rightHandTransform : this.rig.leftHandTransform;
		float magnitude = (this.reelToSync.position - transform.position).magnitude;
		bool flag2 = this._grippingHand || magnitude <= 0.16f;
		this.disableStealing = flag2;
		if (!flag2)
		{
			return false;
		}
		VRMapThumb vrmapThumb = flag ? this.rig.rightThumb : this.rig.leftThumb;
		VRMapIndex vrmapIndex = flag ? this.rig.rightIndex : this.rig.leftIndex;
		VRMap vrmap = flag ? this.rig.rightMiddle : this.rig.leftMiddle;
		float calcT = vrmapThumb.calcT;
		float calcT2 = vrmapIndex.calcT;
		float calcT3 = vrmap.calcT;
		bool flag3 = calcT >= 0.1f && calcT2 >= 0.2f && calcT3 >= 0.2f;
		this._grippingHand = (flag3 ? transform : null);
		return flag3;
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000C9E8D File Offset: 0x000C808D
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (this._grippingHand)
		{
			this._grippingHand = null;
		}
		this.ResetLineLength(this.lineLengthMin * 1.32f);
		return true;
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x000C9EC4 File Offset: 0x000C80C4
	public void ReelIn()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, true);
		this._lineResizing = true;
		this._lineExpanding = false;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000C9F44 File Offset: 0x000C8144
	public void ReelOut()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, false);
		this._lineResizing = true;
		this._lineExpanding = true;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x000C9FC4 File Offset: 0x000C81C4
	public void ReelStop()
	{
		if (this._manualReeling)
		{
			this._localRotDelta = 0f;
		}
		else
		{
			FishingRod.SetHandleMotorUse(false, 0f, this.handleJoint, false);
		}
		this.bobCollider.enabled = true;
		if (this.line)
		{
			this.line.resizeScale = 1f;
		}
		this._lineResizing = false;
		this._lineExpanding = false;
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000CA030 File Offset: 0x000C8230
	private static void SetHandleMotorUse(bool useMotor, float spinRate, HingeJoint handleJoint, bool reverse)
	{
		JointMotor motor = handleJoint.motor;
		motor.force = (useMotor ? 1f : 0f) * spinRate;
		motor.targetVelocity = 16384f * (reverse ? -1f : 1f);
		handleJoint.motor = motor;
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000CA080 File Offset: 0x000C8280
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		this._manualReeling = (this._isGrippingHandle = this.IsFreeHandGripping());
		if (ControllerInputPoller.instance && ControllerInputPoller.PrimaryButtonPress(base.InLeftHand() ? 4 : 5))
		{
			this.QuickReel();
		}
		if (this._lineResetting && this._sinceReset.HasElapsed(this.line.resizeSpeed))
		{
			this.bobCollider.enabled = true;
			this._lineResetting = false;
		}
		this.handleTransform.localPosition = this.reelFreezeLocalPosition;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000CA113 File Offset: 0x000C8313
	private void ResetLineLength(float length)
	{
		if (!this.line)
		{
			return;
		}
		this._lineResetting = true;
		this.bobCollider.enabled = false;
		this.line.ForceTotalLength(length);
		this._sinceReset = TimeSince.Now();
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000CA150 File Offset: 0x000C8350
	private void FixedUpdate()
	{
		Transform transform = base.transform;
		this.handleRigidbody.useGravity = !this._manualReeling;
		if (this._bobFloating && this.bobRigidbody)
		{
			float y = this.bobRigidbody.position.y;
			float num = this.bobFloatForce * this.bobRigidbody.mass;
			float num2 = num * Mathf.Clamp01(this._bobFloatPlaneY - y);
			num += num2;
			if (y <= this._bobFloatPlaneY)
			{
				this.bobRigidbody.AddForce(0f, num, 0f);
			}
		}
		if (this._manualReeling)
		{
			if (this._isGrippingHandle && this._grippingHand)
			{
				this.reelTo.position = this._grippingHand.position;
			}
			Vector3 vector = this.reelFrom.InverseTransformPoint(this.reelTo.position);
			vector.x = 0f;
			vector.Normalize();
			vector *= 2f;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
			quaternion = (base.InRightHand() ? quaternion : Quaternion.Inverse(quaternion));
			this._localRotDelta = FishingRod.GetSignedDeltaYZ(ref this._lastLocalRot, ref quaternion);
			this._lastLocalRot = quaternion;
			Quaternion quaternion2 = transform.rotation * quaternion;
			this.handleRigidbody.MoveRotation(quaternion2);
		}
		else
		{
			this.reelTo.localPosition = transform.InverseTransformPoint(this.reelToSync.position);
		}
		if (!this.line)
		{
			return;
		}
		if (this._manualReeling)
		{
			this._lineResizing = (Mathf.Abs(this._localRotDelta) >= 0.001f);
			this._lineExpanding = (Mathf.Sign(this._localRotDelta) >= 0f);
		}
		if (!this._lineResizing)
		{
			return;
		}
		float num3 = this._manualReeling ? (Mathf.Abs(this._localRotDelta) * 0.66f * Time.fixedDeltaTime) : (this.lineResizeRate * this.lineCastFactor);
		this.line.resizeScale = this.lineCastFactor;
		float num4 = num3 * Time.fixedDeltaTime;
		float num5 = this.line.segmentTargetLength;
		if (this._manualReeling)
		{
			float num6 = 1f / ((float)this.line.segmentNumber + 0.0001f);
			float num7 = this.lineLengthMin * num6;
			float num8 = this.lineLengthMax * num6;
			num4 *= (this._lineExpanding ? 1f : -1f);
			num4 *= (base.InRightHand() ? -1f : 1f);
			float num9 = num5 + num4;
			if (num9 > num7 && num9 < num8)
			{
				num5 += num4;
			}
		}
		else if (this._lineExpanding)
		{
			if (num5 < this._targetSegmentMax)
			{
				num5 += num4;
			}
			else
			{
				this._lineResizing = false;
			}
		}
		else if (num5 > this._targetSegmentMin)
		{
			num5 -= num4;
		}
		else
		{
			this._lineResizing = false;
		}
		if (this._lineResizing)
		{
			this.line.segmentTargetLength = num5;
			return;
		}
		this.ReelStop();
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000CA448 File Offset: 0x000C8648
	private static float GetSignedDeltaYZ(ref Quaternion a, ref Quaternion b)
	{
		Vector3 forward = Vector3.forward;
		Vector3 vector = a * forward;
		Vector3 vector2 = b * forward;
		float num = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		float num2 = Mathf.Atan2(vector2.y, vector2.z) * 57.29578f;
		return Mathf.DeltaAngle(num, num2);
	}

	// Token: 0x04003181 RID: 12673
	public Transform handleTransform;

	// Token: 0x04003182 RID: 12674
	public HingeJoint handleJoint;

	// Token: 0x04003183 RID: 12675
	public Rigidbody handleRigidbody;

	// Token: 0x04003184 RID: 12676
	public BoxCollider handleCollider;

	// Token: 0x04003185 RID: 12677
	public Rigidbody bobRigidbody;

	// Token: 0x04003186 RID: 12678
	public Collider bobCollider;

	// Token: 0x04003187 RID: 12679
	public VerletLine line;

	// Token: 0x04003188 RID: 12680
	public GorillaVelocityEstimator tipTracker;

	// Token: 0x04003189 RID: 12681
	public Rigidbody tipBody;

	// Token: 0x0400318A RID: 12682
	[NonSerialized]
	public VRRig rig;

	// Token: 0x0400318B RID: 12683
	[Space]
	public Vector3 reelFreezeLocalPosition;

	// Token: 0x0400318C RID: 12684
	public Transform reelFrom;

	// Token: 0x0400318D RID: 12685
	public Transform reelTo;

	// Token: 0x0400318E RID: 12686
	public Transform reelToSync;

	// Token: 0x0400318F RID: 12687
	[Space]
	public float reelSpinRate = 1f;

	// Token: 0x04003190 RID: 12688
	public float lineResizeRate = 1f;

	// Token: 0x04003191 RID: 12689
	public float lineCastFactor = 3f;

	// Token: 0x04003192 RID: 12690
	public float lineLengthMin = 0.1f;

	// Token: 0x04003193 RID: 12691
	public float lineLengthMax = 8f;

	// Token: 0x04003194 RID: 12692
	[Space]
	[NonSerialized]
	private bool _bobFloating;

	// Token: 0x04003195 RID: 12693
	public float bobFloatForce = 8f;

	// Token: 0x04003196 RID: 12694
	public float bobStaticDrag = 3.2f;

	// Token: 0x04003197 RID: 12695
	public float bobDynamicDrag = 1.1f;

	// Token: 0x04003198 RID: 12696
	[NonSerialized]
	private float _bobFloatPlaneY;

	// Token: 0x04003199 RID: 12697
	[Space]
	[NonSerialized]
	private float _targetSegmentMin;

	// Token: 0x0400319A RID: 12698
	[NonSerialized]
	private float _targetSegmentMax;

	// Token: 0x0400319B RID: 12699
	[Space]
	[NonSerialized]
	private bool _manualReeling;

	// Token: 0x0400319C RID: 12700
	[NonSerialized]
	private bool _lineResizing;

	// Token: 0x0400319D RID: 12701
	[NonSerialized]
	private bool _lineExpanding;

	// Token: 0x0400319E RID: 12702
	[NonSerialized]
	private bool _lineResetting;

	// Token: 0x0400319F RID: 12703
	[NonSerialized]
	private TimeSince _sinceReset;

	// Token: 0x040031A0 RID: 12704
	[Space]
	[NonSerialized]
	private Quaternion _lastLocalRot = Quaternion.identity;

	// Token: 0x040031A1 RID: 12705
	[NonSerialized]
	private float _localRotDelta;

	// Token: 0x040031A2 RID: 12706
	[NonSerialized]
	private bool _isGrippingHandle;

	// Token: 0x040031A3 RID: 12707
	[NonSerialized]
	private Transform _grippingHand;

	// Token: 0x040031A4 RID: 12708
	private TimeSince _sinceGripLoss;
}
