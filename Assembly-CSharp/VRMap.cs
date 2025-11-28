using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200044D RID: 1101
[Serializable]
public class VRMap
{
	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001BEA RID: 7146 RVA: 0x00094A3B File Offset: 0x00092C3B
	// (set) Token: 0x06001BEB RID: 7147 RVA: 0x00094A48 File Offset: 0x00092C48
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Initialize()
	{
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x00094A58 File Offset: 0x00092C58
	public void MapOther(float lerpValue)
	{
		Vector3 vector;
		Quaternion quaternion;
		this.rigTarget.GetLocalPositionAndRotation(ref vector, ref quaternion);
		this.rigTarget.SetLocalPositionAndRotation(Vector3.Lerp(vector, this.syncPos, lerpValue), Quaternion.Lerp(quaternion, this.syncRotation, lerpValue));
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x00094A9C File Offset: 0x00092C9C
	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		Vector3 vector;
		Quaternion quaternion;
		this.rigTarget.GetPositionAndRotation(ref vector, ref quaternion);
		if (this.overrideTarget != null)
		{
			Vector3 vector2;
			Quaternion quaternion2;
			this.overrideTarget.GetPositionAndRotation(ref vector2, ref quaternion2);
			this.rigTarget.SetPositionAndRotation(vector2 + quaternion * this.trackingPositionOffset * ratio, quaternion2 * Quaternion.Euler(this.trackingRotationOffset));
		}
		else
		{
			if (!this.hasInputDevice && ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
			{
				this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
				this.hasInputDevice = true;
				if (this.vrTargetNode != 4 && this.vrTargetNode != 5)
				{
					this.hasInputDevice = this.myInputDevice.isValid;
				}
			}
			Quaternion quaternion3;
			Vector3 vector3;
			if (this.hasInputDevice && this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, ref quaternion3) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, ref vector3))
			{
				this.rigTarget.SetPositionAndRotation(vector3 + quaternion * this.trackingPositionOffset * ratio + playerOffsetTransform.position, quaternion3 * Quaternion.Euler(this.trackingRotationOffset));
				this.rigTarget.RotateAround(playerOffsetTransform.position, Vector3.up, playerOffsetTransform.eulerAngles.y);
			}
		}
		if (this.handholdOverrideTarget != null)
		{
			this.rigTarget.position = Vector3.MoveTowards(vector, this.handholdOverrideTarget.position - this.handholdOverrideTargetOffset + quaternion * this.trackingPositionOffset * ratio, Time.deltaTime * 2f);
		}
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x00094C50 File Offset: 0x00092E50
	public Vector3 GetExtrapolatedControllerPosition()
	{
		Vector3 vector;
		Quaternion quaternion;
		this.rigTarget.GetPositionAndRotation(ref vector, ref quaternion);
		return vector - quaternion * this.trackingPositionOffset * this.rigTarget.lossyScale.x;
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x00094C93 File Offset: 0x00092E93
	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void MapMyFinger(float lerpValue)
	{
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	// Token: 0x040025AC RID: 9644
	public XRNode vrTargetNode;

	// Token: 0x040025AD RID: 9645
	public Transform overrideTarget;

	// Token: 0x040025AE RID: 9646
	public Transform rigTarget;

	// Token: 0x040025AF RID: 9647
	public Vector3 trackingPositionOffset;

	// Token: 0x040025B0 RID: 9648
	public Vector3 trackingRotationOffset;

	// Token: 0x040025B1 RID: 9649
	public Transform headTransform;

	// Token: 0x040025B2 RID: 9650
	internal NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x040025B3 RID: 9651
	public Quaternion syncRotation;

	// Token: 0x040025B4 RID: 9652
	public float calcT;

	// Token: 0x040025B5 RID: 9653
	private InputDevice myInputDevice;

	// Token: 0x040025B6 RID: 9654
	private bool hasInputDevice;

	// Token: 0x040025B7 RID: 9655
	public Transform handholdOverrideTarget;

	// Token: 0x040025B8 RID: 9656
	public Vector3 handholdOverrideTargetOffset;
}
