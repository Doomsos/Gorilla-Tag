using System;
using UnityEngine;

// Token: 0x02000792 RID: 1938
public class GorillaIK : MonoBehaviour
{
	// Token: 0x060032D8 RID: 13016 RVA: 0x00112620 File Offset: 0x00110820
	private void Awake()
	{
		if (Application.isPlaying && !this.testInEditor)
		{
			this.dU = (this.leftUpperArm.position - this.leftLowerArm.position).magnitude;
			this.dL = (this.leftLowerArm.position - this.leftHand.position).magnitude;
			this.dMax = this.dU + this.dL - this.eps;
			this.initialUpperLeft = this.leftUpperArm.localRotation;
			this.initialLowerLeft = this.leftLowerArm.localRotation;
			this.initialUpperRight = this.rightUpperArm.localRotation;
			this.initialLowerRight = this.rightLowerArm.localRotation;
		}
	}

	// Token: 0x060032D9 RID: 13017 RVA: 0x001126F2 File Offset: 0x001108F2
	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
	}

	// Token: 0x060032DA RID: 13018 RVA: 0x001126FF File Offset: 0x001108FF
	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
	}

	// Token: 0x060032DB RID: 13019 RVA: 0x0011270C File Offset: 0x0011090C
	public void OverrideTargetPos(bool isLeftHand, Vector3 targetWorldPos)
	{
		if (isLeftHand)
		{
			this.hasLeftOverride = true;
			this.leftOverrideWorldPos = targetWorldPos;
			return;
		}
		this.hasRightOverride = true;
		this.rightOverrideWorldPos = targetWorldPos;
	}

	// Token: 0x060032DC RID: 13020 RVA: 0x0011272E File Offset: 0x0011092E
	public Vector3 GetShoulderLocalTargetPos_Left()
	{
		return this.leftUpperArm.parent.InverseTransformPoint(this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position);
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x0011275B File Offset: 0x0011095B
	public Vector3 GetShoulderLocalTargetPos_Right()
	{
		return this.rightUpperArm.parent.InverseTransformPoint(this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position);
	}

	// Token: 0x060032DE RID: 13022 RVA: 0x00112788 File Offset: 0x00110988
	public void ClearOverrides()
	{
		this.hasLeftOverride = false;
		this.hasRightOverride = false;
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x00112798 File Offset: 0x00110998
	private void ArmIK(ref Transform upperArm, ref Transform lowerArm, ref Transform hand, Quaternion initRotUpper, Quaternion initRotLower, Transform target)
	{
		upperArm.localRotation = initRotUpper;
		lowerArm.localRotation = initRotLower;
		float num = Mathf.Clamp((target.position - upperArm.position).magnitude, this.eps, this.dMax);
		float num2 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (lowerArm.position - upperArm.position).normalized), -1f, 1f));
		float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((upperArm.position - lowerArm.position).normalized, (hand.position - lowerArm.position).normalized), -1f, 1f));
		float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot((hand.position - upperArm.position).normalized, (target.position - upperArm.position).normalized), -1f, 1f));
		float num5 = Mathf.Acos(Mathf.Clamp((this.dL * this.dL - this.dU * this.dU - num * num) / (-2f * this.dU * num), -1f, 1f));
		float num6 = Mathf.Acos(Mathf.Clamp((num * num - this.dU * this.dU - this.dL * this.dL) / (-2f * this.dU * this.dL), -1f, 1f));
		Vector3 normalized = Vector3.Cross(hand.position - upperArm.position, lowerArm.position - upperArm.position).normalized;
		Vector3 normalized2 = Vector3.Cross(hand.position - upperArm.position, target.position - upperArm.position).normalized;
		Quaternion quaternion = Quaternion.AngleAxis((num5 - num2) * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized);
		Quaternion quaternion2 = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(lowerArm.rotation) * normalized);
		Quaternion quaternion3 = Quaternion.AngleAxis(num4 * 57.29578f, Quaternion.Inverse(upperArm.rotation) * normalized2);
		this.newRotationUpper = upperArm.localRotation * quaternion3 * quaternion;
		this.newRotationLower = lowerArm.localRotation * quaternion2;
		upperArm.localRotation = this.newRotationUpper;
		lowerArm.localRotation = this.newRotationLower;
		hand.rotation = target.rotation;
	}

	// Token: 0x04004128 RID: 16680
	public Transform headBone;

	// Token: 0x04004129 RID: 16681
	public Transform leftUpperArm;

	// Token: 0x0400412A RID: 16682
	public Transform leftLowerArm;

	// Token: 0x0400412B RID: 16683
	public Transform leftHand;

	// Token: 0x0400412C RID: 16684
	public Transform rightUpperArm;

	// Token: 0x0400412D RID: 16685
	public Transform rightLowerArm;

	// Token: 0x0400412E RID: 16686
	public Transform rightHand;

	// Token: 0x0400412F RID: 16687
	public Transform targetLeft;

	// Token: 0x04004130 RID: 16688
	public Transform targetRight;

	// Token: 0x04004131 RID: 16689
	public Transform targetHead;

	// Token: 0x04004132 RID: 16690
	public Quaternion initialUpperLeft;

	// Token: 0x04004133 RID: 16691
	public Quaternion initialLowerLeft;

	// Token: 0x04004134 RID: 16692
	public Quaternion initialUpperRight;

	// Token: 0x04004135 RID: 16693
	public Quaternion initialLowerRight;

	// Token: 0x04004136 RID: 16694
	public Quaternion newRotationUpper;

	// Token: 0x04004137 RID: 16695
	public Quaternion newRotationLower;

	// Token: 0x04004138 RID: 16696
	public float dU;

	// Token: 0x04004139 RID: 16697
	public float dL;

	// Token: 0x0400413A RID: 16698
	public float dMax;

	// Token: 0x0400413B RID: 16699
	public bool testInEditor;

	// Token: 0x0400413C RID: 16700
	public bool reset;

	// Token: 0x0400413D RID: 16701
	public bool testDefineRot;

	// Token: 0x0400413E RID: 16702
	public bool moveOnce;

	// Token: 0x0400413F RID: 16703
	public float eps;

	// Token: 0x04004140 RID: 16704
	public float upperArmAngle;

	// Token: 0x04004141 RID: 16705
	public float elbowAngle;

	// Token: 0x04004142 RID: 16706
	private bool hasLeftOverride;

	// Token: 0x04004143 RID: 16707
	private Vector3 leftOverrideWorldPos;

	// Token: 0x04004144 RID: 16708
	private bool hasRightOverride;

	// Token: 0x04004145 RID: 16709
	private Vector3 rightOverrideWorldPos;
}
