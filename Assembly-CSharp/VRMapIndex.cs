using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200044E RID: 1102
[Serializable]
public class VRMapIndex : VRMap
{
	// Token: 0x06001BF4 RID: 7156 RVA: 0x00094CB8 File Offset: 0x00092EB8
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x00094D2C File Offset: 0x00092F2C
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.triggerValue = ControllerInputPoller.TriggerFloat(this.vrTargetNode);
		this.triggerTouch = ControllerInputPoller.TriggerTouch(this.vrTargetNode);
		this.calcT = 0.1f * this.triggerTouch;
		this.calcT += 0.9f * this.triggerValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x00094D9C File Offset: 0x00092F9C
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.currentAngle3 = Mathf.Lerp(this.currentAngle3, this.calcT, lerpValue);
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
			}
			this.myTempInt = (int)(this.currentAngle3 * 10.1f);
			if (this.myTempInt != this.lastAngle3)
			{
				this.lastAngle3 = this.myTempInt;
				this.fingerBone3.localRotation = this.angle3Table[this.lastAngle3];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
			this.fingerBone3.localRotation = Quaternion.Lerp(this.fingerBone3.localRotation, Quaternion.Lerp(this.startingAngle3Quat, this.closedAngle3Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x040025B9 RID: 9657
	public InputFeatureUsage inputAxis;

	// Token: 0x040025BA RID: 9658
	public float triggerTouch;

	// Token: 0x040025BB RID: 9659
	public float triggerValue;

	// Token: 0x040025BC RID: 9660
	public Transform fingerBone1;

	// Token: 0x040025BD RID: 9661
	public Transform fingerBone2;

	// Token: 0x040025BE RID: 9662
	public Transform fingerBone3;

	// Token: 0x040025BF RID: 9663
	public Vector3 closedAngle1;

	// Token: 0x040025C0 RID: 9664
	public Vector3 closedAngle2;

	// Token: 0x040025C1 RID: 9665
	public Vector3 closedAngle3;

	// Token: 0x040025C2 RID: 9666
	public Vector3 startingAngle1;

	// Token: 0x040025C3 RID: 9667
	public Vector3 startingAngle2;

	// Token: 0x040025C4 RID: 9668
	public Vector3 startingAngle3;

	// Token: 0x040025C5 RID: 9669
	public Quaternion closedAngle1Quat;

	// Token: 0x040025C6 RID: 9670
	public Quaternion closedAngle2Quat;

	// Token: 0x040025C7 RID: 9671
	public Quaternion closedAngle3Quat;

	// Token: 0x040025C8 RID: 9672
	public Quaternion startingAngle1Quat;

	// Token: 0x040025C9 RID: 9673
	public Quaternion startingAngle2Quat;

	// Token: 0x040025CA RID: 9674
	public Quaternion startingAngle3Quat;

	// Token: 0x040025CB RID: 9675
	private int lastAngle1;

	// Token: 0x040025CC RID: 9676
	private int lastAngle2;

	// Token: 0x040025CD RID: 9677
	private int lastAngle3;

	// Token: 0x040025CE RID: 9678
	private InputDevice myInputDevice;

	// Token: 0x040025CF RID: 9679
	public Quaternion[] angle1Table;

	// Token: 0x040025D0 RID: 9680
	public Quaternion[] angle2Table;

	// Token: 0x040025D1 RID: 9681
	public Quaternion[] angle3Table;

	// Token: 0x040025D2 RID: 9682
	private float currentAngle1;

	// Token: 0x040025D3 RID: 9683
	private float currentAngle2;

	// Token: 0x040025D4 RID: 9684
	private float currentAngle3;

	// Token: 0x040025D5 RID: 9685
	private int myTempInt;
}
