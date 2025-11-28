using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000450 RID: 1104
[Serializable]
public class VRMapThumb : VRMap
{
	// Token: 0x06001BFC RID: 7164 RVA: 0x000951D8 File Offset: 0x000933D8
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x0009522C File Offset: 0x0009342C
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == 4)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x00095320 File Offset: 0x00093520
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x040025F2 RID: 9714
	public InputFeatureUsage inputAxis;

	// Token: 0x040025F3 RID: 9715
	public bool primaryButtonTouch;

	// Token: 0x040025F4 RID: 9716
	public bool primaryButtonPress;

	// Token: 0x040025F5 RID: 9717
	public bool secondaryButtonTouch;

	// Token: 0x040025F6 RID: 9718
	public bool secondaryButtonPress;

	// Token: 0x040025F7 RID: 9719
	public Transform fingerBone1;

	// Token: 0x040025F8 RID: 9720
	public Transform fingerBone2;

	// Token: 0x040025F9 RID: 9721
	public Vector3 closedAngle1;

	// Token: 0x040025FA RID: 9722
	public Vector3 closedAngle2;

	// Token: 0x040025FB RID: 9723
	public Vector3 startingAngle1;

	// Token: 0x040025FC RID: 9724
	public Vector3 startingAngle2;

	// Token: 0x040025FD RID: 9725
	public Quaternion closedAngle1Quat;

	// Token: 0x040025FE RID: 9726
	public Quaternion closedAngle2Quat;

	// Token: 0x040025FF RID: 9727
	public Quaternion startingAngle1Quat;

	// Token: 0x04002600 RID: 9728
	public Quaternion startingAngle2Quat;

	// Token: 0x04002601 RID: 9729
	public Quaternion[] angle1Table;

	// Token: 0x04002602 RID: 9730
	public Quaternion[] angle2Table;

	// Token: 0x04002603 RID: 9731
	private float currentAngle1;

	// Token: 0x04002604 RID: 9732
	private float currentAngle2;

	// Token: 0x04002605 RID: 9733
	private int lastAngle1;

	// Token: 0x04002606 RID: 9734
	private int lastAngle2;

	// Token: 0x04002607 RID: 9735
	private InputDevice tempDevice;

	// Token: 0x04002608 RID: 9736
	private int myTempInt;
}
