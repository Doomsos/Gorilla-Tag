using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C7D RID: 3197
public class LinearSpline : MonoBehaviour
{
	// Token: 0x06004E22 RID: 20002 RVA: 0x00195144 File Offset: 0x00193344
	private void RefreshControlPoints()
	{
		this.controlPoints.Clear();
		for (int i = 0; i < this.controlPointTransforms.Length; i++)
		{
			this.controlPoints.Add(this.controlPointTransforms[i].position);
		}
		this.totalDistance = 0f;
		this.distances.Clear();
		for (int j = 1; j < this.controlPoints.Count; j++)
		{
			float num = Vector3.Distance(this.controlPoints[j - 1], this.controlPoints[j]);
			this.distances.Add(num);
			this.totalDistance += num;
		}
		float num2 = Vector3.Distance(this.controlPoints[this.controlPoints.Count - 1], this.controlPoints[0]);
		this.distances.Add(num2);
		if (this.looping)
		{
			this.totalDistance += num2;
		}
		this.curveBoundaries.Clear();
		if (this.roundCorners)
		{
			for (int k = 0; k < this.controlPoints.Count; k++)
			{
				int num3 = (k > 0) ? (k - 1) : (this.controlPoints.Count - 1);
				int num4 = (k + 1) % this.controlPoints.Count;
				float num5 = Mathf.Min(Mathf.Min(this.cornerRadius, this.distances[num3 % this.distances.Count] * 0.5f), this.distances[k % this.distances.Count] * 0.5f);
				this.curveBoundaries.Add(new LinearSpline.CurveBoundary
				{
					start = Vector3.Lerp(this.controlPoints[num3], this.controlPoints[k], 1f - num5 / this.distances[num3 % this.distances.Count]),
					end = Vector3.Lerp(this.controlPoints[k], this.controlPoints[num4], num5 / this.distances[k])
				});
			}
		}
	}

	// Token: 0x06004E23 RID: 20003 RVA: 0x0019537E File Offset: 0x0019357E
	private void Awake()
	{
		this.RefreshControlPoints();
	}

	// Token: 0x06004E24 RID: 20004 RVA: 0x00195388 File Offset: 0x00193588
	public Vector3 Evaluate(float t)
	{
		if (this.controlPoints.Count < 1)
		{
			return Vector3.zero;
		}
		if (this.controlPoints.Count < 2)
		{
			return this.controlPoints[0];
		}
		if (this.controlPoints.Count < 3)
		{
			return Vector3.Lerp(this.controlPoints[0], this.controlPoints[1], t);
		}
		float num = Mathf.Clamp01(t) * this.totalDistance;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < this.distances.Count; i++)
		{
			if (this.looping || i != this.distances.Count - 1)
			{
				num2 = i;
				if (num - num4 <= this.distances[i])
				{
					num3 = Mathf.Clamp01((num - num4) / this.distances[i]);
					break;
				}
				num3 = 1f;
				num4 += this.distances[i];
			}
		}
		num2 %= this.controlPoints.Count;
		int num5 = (num2 + 1) % this.controlPoints.Count;
		if (this.roundCorners)
		{
			if (num3 > 0.5f && (this.looping || num2 < this.controlPoints.Count - 2))
			{
				int num6 = (num5 + 1) % this.controlPoints.Count;
				float num7 = Mathf.Min(Mathf.Min(this.cornerRadius, this.distances[num2] * 0.5f), this.distances[num5 % this.distances.Count] * 0.5f);
				float num8 = 1f - num7 / this.distances[num2];
				if (num3 > num8)
				{
					Vector3 start = this.curveBoundaries[num5].start;
					Vector3 end = this.curveBoundaries[num5].end;
					float num9 = 0.5f * Mathf.Clamp01((num3 - num8) / (1f - num8));
					Vector3 vector = Vector3.Lerp(start, this.controlPoints[num5], num9);
					Vector3 vector2 = Vector3.Lerp(this.controlPoints[num5], end, num9);
					return Vector3.Lerp(vector, vector2, num9);
				}
			}
			else if (num3 <= 0.5f && (this.looping || num2 > 0))
			{
				int num10 = (num2 > 0) ? (num2 - 1) : (this.controlPoints.Count - 1);
				float num11 = Mathf.Min(Mathf.Min(this.cornerRadius, this.distances[num2] * 0.5f), this.distances[num10 % this.distances.Count] * 0.5f) / this.distances[num2];
				if (num3 < num11)
				{
					Vector3 start2 = this.curveBoundaries[num2].start;
					Vector3 end2 = this.curveBoundaries[num2].end;
					float num12 = 0.5f + 0.5f * Mathf.Clamp01(num3 / num11);
					Vector3 vector3 = Vector3.Lerp(start2, this.controlPoints[num2], num12);
					Vector3 vector4 = Vector3.Lerp(this.controlPoints[num2], end2, num12);
					return Vector3.Lerp(vector3, vector4, num12);
				}
			}
		}
		return Vector3.Lerp(this.controlPoints[num2], this.controlPoints[num5], num3);
	}

	// Token: 0x06004E25 RID: 20005 RVA: 0x001956D4 File Offset: 0x001938D4
	public Vector3 GetForwardTangent(float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 vector = this.Evaluate(t);
		return (this.Evaluate(t + step) - vector).normalized;
	}

	// Token: 0x06004E26 RID: 20006 RVA: 0x0019571C File Offset: 0x0019391C
	private void OnDrawGizmosSelected()
	{
		this.RefreshControlPoints();
		Gizmos.color = Color.yellow;
		int num = this.gizmoResolution;
		Vector3 vector = this.Evaluate(0f);
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i / (float)num;
			Vector3 vector2 = this.Evaluate(t);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
		Vector3 vector3 = this.Evaluate(1f);
		Gizmos.DrawLine(vector, vector3);
	}

	// Token: 0x04005D30 RID: 23856
	public Transform[] controlPointTransforms = new Transform[0];

	// Token: 0x04005D31 RID: 23857
	public Transform debugTransform;

	// Token: 0x04005D32 RID: 23858
	public List<Vector3> controlPoints = new List<Vector3>();

	// Token: 0x04005D33 RID: 23859
	public List<float> distances = new List<float>();

	// Token: 0x04005D34 RID: 23860
	public List<LinearSpline.CurveBoundary> curveBoundaries = new List<LinearSpline.CurveBoundary>();

	// Token: 0x04005D35 RID: 23861
	public bool roundCorners;

	// Token: 0x04005D36 RID: 23862
	public float cornerRadius = 1f;

	// Token: 0x04005D37 RID: 23863
	public bool looping;

	// Token: 0x04005D38 RID: 23864
	public float testFloat;

	// Token: 0x04005D39 RID: 23865
	public int gizmoResolution = 128;

	// Token: 0x04005D3A RID: 23866
	public float totalDistance;

	// Token: 0x02000C7E RID: 3198
	public struct CurveBoundary
	{
		// Token: 0x04005D3B RID: 23867
		public Vector3 start;

		// Token: 0x04005D3C RID: 23868
		public Vector3 end;
	}
}
