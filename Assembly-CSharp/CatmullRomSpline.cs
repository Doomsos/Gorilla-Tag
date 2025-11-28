using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C7B RID: 3195
public class CatmullRomSpline : MonoBehaviour
{
	// Token: 0x06004E14 RID: 19988 RVA: 0x001949EC File Offset: 0x00192BEC
	private void RefreshControlPoints()
	{
		this.controlPoints.Clear();
		this.controlPointsTransformationMatricies.Clear();
		for (int i = 0; i < this.controlPointTransforms.Length; i++)
		{
			this.controlPoints.Add(this.controlPointTransforms[i].position);
			this.controlPointsTransformationMatricies.Add(this.controlPointTransforms[i].localToWorldMatrix);
		}
	}

	// Token: 0x06004E15 RID: 19989 RVA: 0x00194A52 File Offset: 0x00192C52
	private void Awake()
	{
		this.RefreshControlPoints();
	}

	// Token: 0x06004E16 RID: 19990 RVA: 0x00194A5C File Offset: 0x00192C5C
	public static Vector3 Evaluate(List<Vector3> controlPoints, float t)
	{
		if (controlPoints.Count < 1)
		{
			return Vector3.zero;
		}
		if (controlPoints.Count < 2)
		{
			return controlPoints[0];
		}
		if (controlPoints.Count < 3)
		{
			return Vector3.Lerp(controlPoints[0], controlPoints[1], t);
		}
		if (controlPoints.Count >= 4)
		{
			float num = t * (float)(controlPoints.Count - 3);
			int num2 = Mathf.FloorToInt(num);
			float t2 = num - (float)num2;
			int num3 = num2;
			if (num3 >= controlPoints.Count - 3)
			{
				num3 = controlPoints.Count - 4;
				t2 = 1f;
			}
			return CatmullRomSpline.CatmullRom(t2, controlPoints[num3], controlPoints[num3 + 1], controlPoints[num3 + 2], controlPoints[num3 + 3]);
		}
		if (t < 0.5f)
		{
			return Vector3.Lerp(controlPoints[0], controlPoints[1], t * 2f);
		}
		return Vector3.Lerp(controlPoints[1], controlPoints[2], (t - 0.5f) * 2f);
	}

	// Token: 0x06004E17 RID: 19991 RVA: 0x00194B4E File Offset: 0x00192D4E
	public Vector3 Evaluate(float t)
	{
		return CatmullRomSpline.Evaluate(this.controlPoints, t);
	}

	// Token: 0x06004E18 RID: 19992 RVA: 0x00194B5C File Offset: 0x00192D5C
	public static float GetClosestEvaluationOnSpline(List<Vector3> controlPoints, Vector3 worldPoint, out Vector3 linePoint)
	{
		float num = float.MaxValue;
		float num2 = 0f;
		int num3 = 0;
		linePoint = worldPoint;
		for (int i = 1; i < controlPoints.Count - 2; i++)
		{
			Vector3 vector = controlPoints[i];
			Vector3 vector2 = controlPoints[i + 1];
			Vector3 vector3 = vector2 - vector;
			float magnitude = vector3.magnitude;
			if ((double)magnitude > 1E-05)
			{
				Vector3 vector4 = vector3 / magnitude;
				float num4 = Vector3.Dot(worldPoint - vector, vector4);
				float sqrMagnitude;
				float num5;
				Vector3 vector5;
				if (num4 <= 0f)
				{
					sqrMagnitude = (worldPoint - vector).sqrMagnitude;
					num5 = 0f;
					vector5 = vector;
				}
				else if (num4 >= magnitude)
				{
					sqrMagnitude = (worldPoint - vector2).sqrMagnitude;
					num5 = 1f;
					vector5 = vector2;
				}
				else
				{
					sqrMagnitude = (worldPoint - (vector + vector4 * num4)).sqrMagnitude;
					num5 = num4 / magnitude;
					vector5 = vector + vector4 * num4;
				}
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					num2 = num5;
					num3 = i;
					linePoint = vector5;
				}
			}
		}
		return Mathf.Clamp01(((float)(num3 - 1) + num2) / (float)(controlPoints.Count - 3));
	}

	// Token: 0x06004E19 RID: 19993 RVA: 0x00194C9F File Offset: 0x00192E9F
	public float GetClosestEvaluationOnSpline(Vector3 worldPoint, out Vector3 linePoint)
	{
		return CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPoints, worldPoint, out linePoint);
	}

	// Token: 0x06004E1A RID: 19994 RVA: 0x00194CB0 File Offset: 0x00192EB0
	public static Vector3 GetForwardTangent(List<Vector3> controlPoints, float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 vector = CatmullRomSpline.Evaluate(controlPoints, t);
		return (CatmullRomSpline.Evaluate(controlPoints, t + step) - vector).normalized;
	}

	// Token: 0x06004E1B RID: 19995 RVA: 0x00194CF8 File Offset: 0x00192EF8
	public Vector3 GetForwardTangent(float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 vector = this.Evaluate(t);
		return (this.Evaluate(t + step) - vector).normalized;
	}

	// Token: 0x06004E1C RID: 19996 RVA: 0x00194D40 File Offset: 0x00192F40
	private static Vector3 CatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 vector = 2f * p1;
		Vector3 vector2 = p2 - p0;
		Vector3 vector3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 vector4 = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (vector + vector2 * t + vector3 * t * t + vector4 * t * t * t);
	}

	// Token: 0x06004E1D RID: 19997 RVA: 0x00194E04 File Offset: 0x00193004
	private void OnDrawGizmosSelected()
	{
		if (this.testFloat > 0f)
		{
			Vector3 vector = this.Evaluate(this.testFloat);
			Matrix4x4 matrix4x = CatmullRomSpline.Evaluate(this.controlPointsTransformationMatricies, this.testFloat);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.up * 0.2f);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.forward * 0.2f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.right * 0.2f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(vector, 0.01f);
			Gizmos.DrawRay(vector, this.GetForwardTangent(this.testFloat, 0.01f));
		}
		this.RefreshControlPoints();
		Gizmos.color = Color.yellow;
		int num = 128;
		Vector3 vector2 = this.Evaluate(0f);
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i / (float)num;
			Vector3 vector3 = this.Evaluate(t);
			Gizmos.DrawLine(vector2, vector3);
			vector2 = vector3;
		}
		if (this.debugTransform != null)
		{
			Vector3 vector4;
			float closestEvaluationOnSpline = this.GetClosestEvaluationOnSpline(this.debugTransform.position, out vector4);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.Evaluate(closestEvaluationOnSpline), 0.2f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(vector4, 0.25f);
			if (this.controlPoints.Count > 3)
			{
				Gizmos.color = Color.green;
				vector2 = this.controlPoints[1];
				for (int j = 2; j < this.controlPoints.Count - 2; j++)
				{
					Vector3 vector5 = this.controlPoints[j];
					Gizmos.DrawLine(vector2, vector5);
					vector2 = vector5;
				}
			}
		}
	}

	// Token: 0x06004E1E RID: 19998 RVA: 0x00194FE8 File Offset: 0x001931E8
	public static Matrix4x4 CatmullRom(float t, Matrix4x4 p0, Matrix4x4 p1, Matrix4x4 p2, Matrix4x4 p3)
	{
		Vector3 vector = CatmullRomSpline.CatmullRom(t, p0.GetColumn(3), p1.GetColumn(3), p2.GetColumn(3), p3.GetColumn(3));
		Quaternion quaternion = Quaternion.Slerp(p1.rotation, p2.rotation, t);
		Vector3 vector2 = Vector3.Lerp(p1.lossyScale, p2.lossyScale, t);
		return Matrix4x4.TRS(vector, quaternion, vector2);
	}

	// Token: 0x06004E1F RID: 19999 RVA: 0x00195060 File Offset: 0x00193260
	public static Matrix4x4 Evaluate(List<Matrix4x4> controlPoints, float t)
	{
		if (controlPoints.Count < 1)
		{
			return Matrix4x4.identity;
		}
		if (controlPoints.Count < 2)
		{
			return controlPoints[0];
		}
		if (controlPoints.Count < 4)
		{
			return controlPoints[0];
		}
		float num = t * (float)(controlPoints.Count - 3);
		int num2 = Mathf.FloorToInt(num);
		float t2 = num - (float)num2;
		int num3 = num2;
		if (num3 >= controlPoints.Count - 3)
		{
			num3 = controlPoints.Count - 4;
			t2 = 1f;
		}
		return CatmullRomSpline.CatmullRom(t2, controlPoints[num3], controlPoints[num3 + 1], controlPoints[num3 + 2], controlPoints[num3 + 3]);
	}

	// Token: 0x04005D29 RID: 23849
	public Transform[] controlPointTransforms = new Transform[0];

	// Token: 0x04005D2A RID: 23850
	public Transform debugTransform;

	// Token: 0x04005D2B RID: 23851
	public List<Vector3> controlPoints = new List<Vector3>();

	// Token: 0x04005D2C RID: 23852
	public List<Matrix4x4> controlPointsTransformationMatricies = new List<Matrix4x4>();

	// Token: 0x04005D2D RID: 23853
	public float testFloat;
}
