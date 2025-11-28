using System;
using CustomMapSupport;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x0200093A RID: 2362
public class CustomMapsGorillaZipline : GorillaZipline
{
	// Token: 0x06003C59 RID: 15449 RVA: 0x0013ECC8 File Offset: 0x0013CEC8
	public bool GenerateZipline(BezierSpline splineRef)
	{
		this.spline = base.GetComponent<BezierSpline>();
		if (this.spline.IsNull())
		{
			return false;
		}
		this.spline.BuildSplineFromPoints(splineRef.GetControlPoints(), this.ConvertControlPointModes(splineRef.GetControlPointModes()), splineRef.Loop);
		if (this.segmentsRoot == null)
		{
			return false;
		}
		this.ziplineDistance = 0f;
		float num = 0f;
		int num2 = 0;
		Transform transform = null;
		while (num < 1f)
		{
			float num3 = this.segmentDistance;
			if (num2 == 0)
			{
				num3 /= 2f;
			}
			base.FindTFromDistance(ref num, num3, 5000);
			if (num < 1f || this.spline.Loop)
			{
				Vector3 point = this.spline.GetPoint(num);
				GameObject gameObject = Object.Instantiate<GameObject>(this.segmentPrefab);
				gameObject.transform.SetParent(this.segmentsRoot);
				gameObject.transform.position = point;
				gameObject.transform.LookAt(point + this.spline.GetDirection(num));
				gameObject.transform.position -= gameObject.transform.forward * 0.5f;
				if (num2 > 0)
				{
					transform.LookAt(gameObject.transform);
				}
				gameObject.GetComponent<GorillaClimbableRef>().climb = this.slideHelper;
				this.ziplineDistance += this.segmentDistance;
				transform = gameObject.transform;
			}
			num2++;
		}
		return true;
	}

	// Token: 0x06003C5A RID: 15450 RVA: 0x0013EE4C File Offset: 0x0013D04C
	private BezierControlPointMode[] ConvertControlPointModes(BezierControlPointMode[] refModes)
	{
		BezierControlPointMode[] array = new BezierControlPointMode[refModes.Length];
		for (int i = 0; i < refModes.Length; i++)
		{
			switch (refModes[i])
			{
			case 0:
				array[i] = BezierControlPointMode.Free;
				break;
			case 1:
				array[i] = BezierControlPointMode.Aligned;
				break;
			case 2:
				array[i] = BezierControlPointMode.Mirrored;
				break;
			}
		}
		return array;
	}

	// Token: 0x06003C5B RID: 15451 RVA: 0x0013EE99 File Offset: 0x0013D099
	protected override void Start()
	{
		GorillaClimbable slideHelper = this.slideHelper;
		slideHelper.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(slideHelper.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(base.OnBeforeClimb));
	}

	// Token: 0x06003C5C RID: 15452 RVA: 0x0013EEC4 File Offset: 0x0013D0C4
	public void Init(GTObjectPlaceholder ziplinePlaceholder)
	{
		if (ziplinePlaceholder.PlaceholderObject != 8)
		{
			return;
		}
		this.segmentDistance = ziplinePlaceholder.ziplineSegmentGenerationOffset;
		this.spline = base.gameObject.GetComponent<BezierSpline>();
		if (this.spline == null)
		{
			this.spline = base.gameObject.AddComponent<BezierSpline>();
		}
		this.spline.BuildSplineFromPoints(ziplinePlaceholder.spline.GetControlPoints(), this.ConvertControlPointModes(ziplinePlaceholder.spline.GetControlPointModes()), ziplinePlaceholder.spline.Loop);
		for (int i = 0; i < ziplinePlaceholder.ziplineSegments.Count; i++)
		{
			ziplinePlaceholder.ziplineSegments[i].transform.SetParent(this.segmentsRoot, true);
			ziplinePlaceholder.ziplineSegments[i].AddComponent<GorillaClimbableRef>().climb = this.slideHelper;
			this.ziplineDistance += this.segmentDistance;
		}
	}
}
