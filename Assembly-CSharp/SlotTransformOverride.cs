using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x0200049E RID: 1182
[Serializable]
public class SlotTransformOverride
{
	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001E41 RID: 7745 RVA: 0x0009FDFB File Offset: 0x0009DFFB
	// (set) Token: 0x06001E42 RID: 7746 RVA: 0x0009FE08 File Offset: 0x0009E008
	private XformOffset _EdXformOffsetRepresenationOf_overrideTransformMatrix
	{
		get
		{
			return new XformOffset(this.overrideTransformMatrix);
		}
		set
		{
			this.overrideTransformMatrix = Matrix4x4.TRS(value.pos, value.rot, value.scale);
		}
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x0009FE28 File Offset: 0x0009E028
	public void Initialize(Component component, Transform anchor)
	{
		if (!this.useAdvancedGrab)
		{
			return;
		}
		this.AdvOriginLocalToParentAnchorLocal = anchor.worldToLocalMatrix * this.advancedGrabPointOrigin.localToWorldMatrix;
		this.AdvAnchorLocalToAdvOriginLocal = this.advancedGrabPointOrigin.worldToLocalMatrix * this.advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint subGrabPoint in this.multiPoints)
		{
			if (subGrabPoint == null)
			{
				break;
			}
			subGrabPoint.InitializePoints(anchor, this.advancedGrabPointAnchor, this.advancedGrabPointOrigin);
		}
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x0009FED4 File Offset: 0x0009E0D4
	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x0009FEE8 File Offset: 0x0009E0E8
	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint subGrabPoint = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(subGrabPoint);
	}

	// Token: 0x04002878 RID: 10360
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	public Transform overrideTransform;

	// Token: 0x04002879 RID: 10361
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	[Delayed]
	public string overrideTransform_path;

	// Token: 0x0400287A RID: 10362
	public TransferrableObject.PositionState positionState;

	// Token: 0x0400287B RID: 10363
	public bool useAdvancedGrab;

	// Token: 0x0400287C RID: 10364
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	// Token: 0x0400287D RID: 10365
	public Transform advancedGrabPointAnchor;

	// Token: 0x0400287E RID: 10366
	public Transform advancedGrabPointOrigin;

	// Token: 0x0400287F RID: 10367
	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	// Token: 0x04002880 RID: 10368
	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	// Token: 0x04002881 RID: 10369
	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
