using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200048B RID: 1163
[RequireComponent(typeof(BezierSpline))]
public class ManipulatableSpinner : ManipulatableObject
{
	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001DBE RID: 7614 RVA: 0x0009C8E1 File Offset: 0x0009AAE1
	// (set) Token: 0x06001DBF RID: 7615 RVA: 0x0009C8E9 File Offset: 0x0009AAE9
	public float angle { get; private set; }

	// Token: 0x06001DC0 RID: 7616 RVA: 0x0009C8F2 File Offset: 0x0009AAF2
	private void Awake()
	{
		this.spline = base.GetComponent<BezierSpline>();
	}

	// Token: 0x06001DC1 RID: 7617 RVA: 0x0009C900 File Offset: 0x0009AB00
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
		Vector3 position = grabbingHand.transform.position;
		float num = this.FindPositionOnSpline(position);
		this.previousHandT = num;
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x0009C928 File Offset: 0x0009AB28
	protected override bool ShouldHandDetach(GameObject hand)
	{
		if (!this.spline.Loop && (this.currentHandT >= 0.99f || this.currentHandT <= 0.01f))
		{
			return true;
		}
		Vector3 position = hand.transform.position;
		Vector3 point = this.spline.GetPoint(this.currentHandT);
		return Vector3.SqrMagnitude(position - point) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001DC4 RID: 7620 RVA: 0x0009C998 File Offset: 0x0009AB98
	protected override void OnHeldUpdate(GameObject hand)
	{
		float angle = this.angle;
		Vector3 position = hand.transform.position;
		this.currentHandT = this.FindPositionOnSpline(position);
		float num = this.currentHandT - this.previousHandT;
		if (this.spline.Loop)
		{
			if (num > 0.5f)
			{
				num -= 1f;
			}
			else if (num < -0.5f)
			{
				num += 1f;
			}
		}
		this.angle += num;
		this.previousHandT = this.currentHandT;
		if (this.applyReleaseVelocity && this.currentHandT <= 0.99f && this.currentHandT >= 0.01f)
		{
			this.tVelocity = (this.angle - angle) / Time.deltaTime;
		}
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x0009CA54 File Offset: 0x0009AC54
	protected override void OnReleasedUpdate()
	{
		if (this.tVelocity != 0f)
		{
			this.angle += this.tVelocity * Time.deltaTime;
			if (Mathf.Abs(this.tVelocity) < this.lowSpeedThreshold)
			{
				this.tVelocity *= 1f - this.lowSpeedDrag * Time.deltaTime;
				return;
			}
			this.tVelocity *= 1f - this.releaseDrag * Time.deltaTime;
		}
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x0009CADC File Offset: 0x0009ACDC
	private float FindPositionOnSpline(Vector3 grabPoint)
	{
		int i = 0;
		int num = 200;
		float num2 = 0.001f;
		float num3 = 1f / (float)num;
		float3 @float = base.transform.InverseTransformPoint(grabPoint);
		float result = 0f;
		float num4 = float.PositiveInfinity;
		while (i < num)
		{
			float num5 = math.distancesq(this.spline.GetPointLocal(num2), @float);
			if (num5 < num4)
			{
				num4 = num5;
				result = num2;
			}
			num2 += num3;
			i++;
		}
		return result;
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x0009CB58 File Offset: 0x0009AD58
	public void SetAngle(float newAngle)
	{
		this.angle = newAngle;
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x0009CB61 File Offset: 0x0009AD61
	public void SetVelocity(float newVelocity)
	{
		this.tVelocity = newVelocity;
	}

	// Token: 0x040027BA RID: 10170
	public float breakDistance = 0.2f;

	// Token: 0x040027BB RID: 10171
	public bool applyReleaseVelocity;

	// Token: 0x040027BC RID: 10172
	public float releaseDrag = 1f;

	// Token: 0x040027BD RID: 10173
	public float lowSpeedThreshold = 0.12f;

	// Token: 0x040027BE RID: 10174
	public float lowSpeedDrag = 3f;

	// Token: 0x040027BF RID: 10175
	private BezierSpline spline;

	// Token: 0x040027C0 RID: 10176
	private float previousHandT;

	// Token: 0x040027C1 RID: 10177
	private float currentHandT;

	// Token: 0x040027C2 RID: 10178
	private float tVelocity;
}
