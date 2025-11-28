using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200049B RID: 1179
[Serializable]
public class SubGrabPoint
{
	// Token: 0x06001E2B RID: 7723 RVA: 0x0009F74D File Offset: 0x0009D94D
	public virtual Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripPointLocalToAdvOriginLocal;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x0009F755 File Offset: 0x0009D955
	public virtual Quaternion GetRotationRelativeToObjectAnchor(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripRotation_ParentAnchorLocal;
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x0009F75D File Offset: 0x0009D95D
	public virtual Vector3 GetGrabPositionRelativeToGrabPointOrigin(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return this.gripPoint_AdvOriginLocal;
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x0009F768 File Offset: 0x0009D968
	public virtual void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		if (this.gripPoint == null)
		{
			return;
		}
		this.gripPoint_AdvOriginLocal = advancedGrabPointOrigin.InverseTransformPoint(this.gripPoint.position);
		this.gripRotation_AdvOriginLocal = Quaternion.Inverse(advancedGrabPointOrigin.rotation) * this.gripPoint.rotation;
		this.advAnchor_ParentAnchorLocal = Quaternion.Inverse(anchor.rotation) * grabPointAnchor.rotation;
		this.gripRotation_ParentAnchorLocal = Quaternion.Inverse(anchor.rotation) * this.gripPoint.rotation;
		this.gripPointLocalToAdvOriginLocal = advancedGrabPointOrigin.worldToLocalMatrix * this.gripPoint.localToWorldMatrix;
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x0009F815 File Offset: 0x0009DA15
	public Vector3 GetPositionOnObject(Transform transferableObject, SlotTransformOverride slotTransformOverride)
	{
		return transferableObject.TransformPoint(this.gripPoint_AdvOriginLocal);
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x0009F824 File Offset: 0x0009DA24
	public virtual Matrix4x4 GetTransformFromPositionState(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride, Transform targetDockXf)
	{
		Quaternion quaternion = advancedItemState.deltaRotation;
		if (!quaternion.IsValid())
		{
			quaternion = Quaternion.identity;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, quaternion, Vector3.one);
		Matrix4x4 matrix4x2 = this.GetTransformation_GripPointLocalToAdvOriginLocal(advancedItemState.preData, slotTransformOverride) * matrix4x.inverse;
		Matrix4x4 matrix4x3 = slotTransformOverride.AdvAnchorLocalToAdvOriginLocal * matrix4x2.inverse;
		return slotTransformOverride.AdvOriginLocalToParentAnchorLocal * matrix4x3;
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x0009F894 File Offset: 0x0009DA94
	public AdvancedItemState GetAdvancedItemStateFromHand(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		AdvancedItemState.PreData preData = this.GetPreData(objectTransform, handTransform, targetDock, slotTransformOverride);
		Matrix4x4 matrix4x = targetDock.localToWorldMatrix * slotTransformOverride.AdvOriginLocalToParentAnchorLocal * slotTransformOverride.AdvAnchorLocalToAdvOriginLocal;
		Matrix4x4 matrix4x2 = objectTransform.localToWorldMatrix * this.GetTransformation_GripPointLocalToAdvOriginLocal(preData, slotTransformOverride);
		Quaternion quaternion = (matrix4x.inverse * matrix4x2).rotation;
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 vector3 = quaternion * Vector3.forward;
		bool reverseGrip = false;
		Vector2 up = Vector2.up;
		float angle = 0f;
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
			quaternion = Quaternion.identity;
			break;
		case LimitAxis.YAxis:
			if (this.allowReverseGrip)
			{
				if (Vector3.Dot(vector, Vector3.up) < 0f)
				{
					Debug.Log("Using Reverse Grip");
					reverseGrip = true;
					vector = Vector3.down;
				}
				else
				{
					vector = Vector3.up;
				}
			}
			else
			{
				vector = Vector3.up;
			}
			vector2 = Vector3.Cross(vector, vector3);
			vector3 = Vector3.Cross(vector2, vector);
			up..ctor(vector3.z, vector3.x);
			quaternion = Quaternion.LookRotation(vector3, vector);
			break;
		case LimitAxis.XAxis:
			vector2 = Vector3.right;
			vector3 = Vector3.Cross(vector2, vector);
			vector = Vector3.Cross(vector3, vector2);
			break;
		case LimitAxis.ZAxis:
			vector3 = Vector3.forward;
			vector2 = Vector3.Cross(vector, vector3);
			vector = Vector3.Cross(vector3, vector2);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return new AdvancedItemState
		{
			preData = preData,
			limitAxis = this.limitAxis,
			angle = angle,
			reverseGrip = reverseGrip,
			angleVectorWhereUpIsStandard = up,
			deltaRotation = quaternion
		};
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x0009FA58 File Offset: 0x0009DC58
	public virtual AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			pointType = AdvancedItemState.PointType.Standard
		};
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x0009FA68 File Offset: 0x0009DC68
	public virtual float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		float num = Vector3.SqrMagnitude(this.gripPoint_AdvOriginLocal - vector);
		float num2;
		Vector3 vector2;
		(Quaternion.Inverse(objectTransform.rotation * this.gripRotation_AdvOriginLocal) * targetDock.rotation * this.advAnchor_ParentAnchorLocal).ToAngleAxis(ref num2, ref vector2);
		return num + Mathf.Abs(num2) * 0.0001f;
	}

	// Token: 0x04002866 RID: 10342
	[FormerlySerializedAs("transform")]
	public Transform gripPoint;

	// Token: 0x04002867 RID: 10343
	public LimitAxis limitAxis;

	// Token: 0x04002868 RID: 10344
	public bool allowReverseGrip;

	// Token: 0x04002869 RID: 10345
	private Vector3 gripPoint_AdvOriginLocal;

	// Token: 0x0400286A RID: 10346
	private Vector3 gripPointOffset_AdvOriginLocal;

	// Token: 0x0400286B RID: 10347
	public Quaternion gripRotation_AdvOriginLocal;

	// Token: 0x0400286C RID: 10348
	public Quaternion advAnchor_ParentAnchorLocal;

	// Token: 0x0400286D RID: 10349
	public Quaternion gripRotation_ParentAnchorLocal;

	// Token: 0x0400286E RID: 10350
	public Matrix4x4 gripPointLocalToAdvOriginLocal;
}
