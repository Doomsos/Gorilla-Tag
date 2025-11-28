using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200049D RID: 1181
[Serializable]
public class SubSplineGrabPoint : SubLineGrabPoint
{
	// Token: 0x06001E3C RID: 7740 RVA: 0x0009FCE5 File Offset: 0x0009DEE5
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return CatmullRomSpline.Evaluate(this.controlPointsTransformsRelativeToGrabOrigin, advancedItemState.distAlongLine);
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x0009FCF8 File Offset: 0x0009DEF8
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		this.controlPointsRelativeToGrabOrigin = new List<Vector3>();
		foreach (Transform transform in this.spline.controlPointTransforms)
		{
			this.controlPointsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.InverseTransformPoint(transform.position));
			this.controlPointsTransformsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.worldToLocalMatrix * transform.localToWorldMatrix);
		}
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x0009FD6C File Offset: 0x0009DF6C
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		Vector3 worldPoint = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector;
		return new AdvancedItemState.PreData
		{
			distAlongLine = CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, worldPoint, out vector),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x0009FDA8 File Offset: 0x0009DFA8
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector2;
		CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, vector, out vector2);
		return Vector3.SqrMagnitude(vector2 - vector);
	}

	// Token: 0x04002875 RID: 10357
	public CatmullRomSpline spline;

	// Token: 0x04002876 RID: 10358
	public List<Vector3> controlPointsRelativeToGrabOrigin = new List<Vector3>();

	// Token: 0x04002877 RID: 10359
	public List<Matrix4x4> controlPointsTransformsRelativeToGrabOrigin = new List<Matrix4x4>();
}
