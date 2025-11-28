using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200049C RID: 1180
[Serializable]
public class SubLineGrabPoint : SubGrabPoint
{
	// Token: 0x06001E35 RID: 7733 RVA: 0x0009FAF8 File Offset: 0x0009DCF8
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		float distAlongLine = advancedItemState.distAlongLine;
		Vector3 vector = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), distAlongLine);
		Quaternion quaternion = Quaternion.Slerp(this.startPointRelativeTransformToGrabPointOrigin.rotation, this.endPointRelativeTransformToGrabPointOrigin.rotation, distAlongLine);
		return Matrix4x4.TRS(vector, quaternion, Vector3.one);
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x0009FB50 File Offset: 0x0009DD50
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		if (this.startPoint == null || this.endPoint == null)
		{
			return;
		}
		this.startPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.startPoint.position);
		this.endPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.endPoint.position);
		this.endPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.endPoint.localToWorldMatrix;
		this.startPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.startPoint.localToWorldMatrix;
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x0009FBE9 File Offset: 0x0009DDE9
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			distAlongLine = SubLineGrabPoint.<GetPreData>g__FindNearestFractionOnLine|8_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x0009FC20 File Offset: 0x0009DE20
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		float num = SubLineGrabPoint.<EvaluateScore>g__FindNearestFractionOnLine|9_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position);
		Vector3 vector = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), num);
		Vector3 vector2 = objectTransform.InverseTransformPoint(handTransform.position);
		return Vector3.SqrMagnitude(vector - vector2);
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x0009FC90 File Offset: 0x0009DE90
	[CompilerGenerated]
	internal static float <GetPreData>g__FindNearestFractionOnLine|8_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x0009FCCC File Offset: 0x0009DECC
	[CompilerGenerated]
	internal static float <EvaluateScore>g__FindNearestFractionOnLine|9_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x0400286F RID: 10351
	public Transform startPoint;

	// Token: 0x04002870 RID: 10352
	public Transform endPoint;

	// Token: 0x04002871 RID: 10353
	public Vector3 startPointRelativeToGrabPointOrigin;

	// Token: 0x04002872 RID: 10354
	public Vector3 endPointRelativeToGrabPointOrigin;

	// Token: 0x04002873 RID: 10355
	public Matrix4x4 startPointRelativeTransformToGrabPointOrigin;

	// Token: 0x04002874 RID: 10356
	public Matrix4x4 endPointRelativeTransformToGrabPointOrigin;
}
