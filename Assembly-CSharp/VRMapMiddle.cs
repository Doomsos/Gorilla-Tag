using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200044F RID: 1103
[Serializable]
public class VRMapMiddle : VRMap
{
	// Token: 0x06001BF8 RID: 7160 RVA: 0x00094F58 File Offset: 0x00093158
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x00094FCB File Offset: 0x000931CB
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.gripValue = ControllerInputPoller.GripFloat(this.vrTargetNode);
		this.calcT = 1f * this.gripValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x00095004 File Offset: 0x00093204
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

	// Token: 0x040025D6 RID: 9686
	public InputFeatureUsage inputAxis;

	// Token: 0x040025D7 RID: 9687
	public float gripValue;

	// Token: 0x040025D8 RID: 9688
	public Transform fingerBone1;

	// Token: 0x040025D9 RID: 9689
	public Transform fingerBone2;

	// Token: 0x040025DA RID: 9690
	public Transform fingerBone3;

	// Token: 0x040025DB RID: 9691
	public Vector3 closedAngle1;

	// Token: 0x040025DC RID: 9692
	public Vector3 closedAngle2;

	// Token: 0x040025DD RID: 9693
	public Vector3 closedAngle3;

	// Token: 0x040025DE RID: 9694
	public Vector3 startingAngle1;

	// Token: 0x040025DF RID: 9695
	public Vector3 startingAngle2;

	// Token: 0x040025E0 RID: 9696
	public Vector3 startingAngle3;

	// Token: 0x040025E1 RID: 9697
	public Quaternion closedAngle1Quat;

	// Token: 0x040025E2 RID: 9698
	public Quaternion closedAngle2Quat;

	// Token: 0x040025E3 RID: 9699
	public Quaternion closedAngle3Quat;

	// Token: 0x040025E4 RID: 9700
	public Quaternion startingAngle1Quat;

	// Token: 0x040025E5 RID: 9701
	public Quaternion startingAngle2Quat;

	// Token: 0x040025E6 RID: 9702
	public Quaternion startingAngle3Quat;

	// Token: 0x040025E7 RID: 9703
	public Quaternion[] angle1Table;

	// Token: 0x040025E8 RID: 9704
	public Quaternion[] angle2Table;

	// Token: 0x040025E9 RID: 9705
	public Quaternion[] angle3Table;

	// Token: 0x040025EA RID: 9706
	private int lastAngle1;

	// Token: 0x040025EB RID: 9707
	private int lastAngle2;

	// Token: 0x040025EC RID: 9708
	private int lastAngle3;

	// Token: 0x040025ED RID: 9709
	private float currentAngle1;

	// Token: 0x040025EE RID: 9710
	private float currentAngle2;

	// Token: 0x040025EF RID: 9711
	private float currentAngle3;

	// Token: 0x040025F0 RID: 9712
	private InputDevice tempDevice;

	// Token: 0x040025F1 RID: 9713
	private int myTempInt;
}
