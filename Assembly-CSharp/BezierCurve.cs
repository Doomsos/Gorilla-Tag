using System;
using UnityEngine;

// Token: 0x02000C79 RID: 3193
public class BezierCurve : MonoBehaviour
{
	// Token: 0x06004DF7 RID: 19959 RVA: 0x00193DB8 File Offset: 0x00191FB8
	public Vector3 GetPoint(float t)
	{
		Vector3 vector = (this.points.Length == 3) ? Bezier.GetPoint(this.points[0], this.points[1], this.points[2], t) : Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t);
		if (!this.referenceTransform)
		{
			return vector;
		}
		return this.referenceTransform.TransformPoint(vector);
	}

	// Token: 0x06004DF8 RID: 19960 RVA: 0x00193E50 File Offset: 0x00192050
	public Vector3 GetVelocity(float t)
	{
		Vector3 vector = (this.points.Length == 3) ? Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], t) : Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t);
		if (!this.referenceTransform)
		{
			return vector;
		}
		return this.referenceTransform.TransformPoint(vector) - this.referenceTransform.position;
	}

	// Token: 0x06004DF9 RID: 19961 RVA: 0x00193EF8 File Offset: 0x001920F8
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06004DFA RID: 19962 RVA: 0x00193F14 File Offset: 0x00192114
	public void Reset()
	{
		this.referenceTransform = base.transform;
		this.points = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}

	// Token: 0x04005D21 RID: 23841
	public Transform referenceTransform;

	// Token: 0x04005D22 RID: 23842
	public Vector3[] points;
}
