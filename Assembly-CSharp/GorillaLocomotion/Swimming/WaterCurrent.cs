using System;
using System.Collections.Generic;
using AA;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F8E RID: 3982
	public class WaterCurrent : MonoBehaviour
	{
		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x060063EF RID: 25583 RVA: 0x002088BD File Offset: 0x00206ABD
		public float Speed
		{
			get
			{
				return this.currentSpeed;
			}
		}

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x060063F0 RID: 25584 RVA: 0x002088C5 File Offset: 0x00206AC5
		public float Accel
		{
			get
			{
				return this.currentAccel;
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x060063F1 RID: 25585 RVA: 0x002088CD File Offset: 0x00206ACD
		public float InwardSpeed
		{
			get
			{
				return this.inwardCurrentSpeed;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x060063F2 RID: 25586 RVA: 0x002088D5 File Offset: 0x00206AD5
		public float InwardAccel
		{
			get
			{
				return this.inwardCurrentAccel;
			}
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x002088E0 File Offset: 0x00206AE0
		public bool GetCurrentAtPoint(Vector3 worldPoint, Vector3 startingVelocity, float dt, out Vector3 currentVelocity, out Vector3 velocityChange)
		{
			float num = (this.fullEffectDistance + this.fadeDistance) * (this.fullEffectDistance + this.fadeDistance);
			bool result = false;
			velocityChange = Vector3.zero;
			currentVelocity = Vector3.zero;
			float num2 = 0.0001f;
			float magnitude = startingVelocity.magnitude;
			if (magnitude > num2)
			{
				Vector3 vector = startingVelocity / magnitude;
				float num3 = Spring.DamperDecayExact(magnitude, this.dampingHalfLife, dt, 1E-05f);
				Vector3 vector2 = vector * num3;
				velocityChange += vector2 - startingVelocity;
			}
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector3;
				float closestEvaluationOnSpline = catmullRomSpline.GetClosestEvaluationOnSpline(worldPoint, out vector3);
				Vector3 vector4 = catmullRomSpline.Evaluate(closestEvaluationOnSpline);
				Vector3 vector5 = vector4 - worldPoint;
				if (vector5.sqrMagnitude < num)
				{
					result = true;
					float magnitude2 = vector5.magnitude;
					float num4 = (magnitude2 > this.fullEffectDistance) ? (1f - Mathf.Clamp01((magnitude2 - this.fullEffectDistance) / this.fadeDistance)) : 1f;
					float t = Mathf.Clamp01(closestEvaluationOnSpline + this.velocityAnticipationAdjustment);
					Vector3 forwardTangent = catmullRomSpline.GetForwardTangent(t, 0.01f);
					if (this.currentSpeed > num2 && Vector3.Dot(startingVelocity, forwardTangent) < num4 * this.currentSpeed)
					{
						velocityChange += forwardTangent * (this.currentAccel * dt);
					}
					else if (this.currentSpeed < num2 && Vector3.Dot(startingVelocity, forwardTangent) > num4 * this.currentSpeed)
					{
						velocityChange -= forwardTangent * (this.currentAccel * dt);
					}
					currentVelocity += forwardTangent * num4 * this.currentSpeed;
					float num5 = Mathf.InverseLerp(this.inwardCurrentNoEffectRadius, this.inwardCurrentFullEffectRadius, magnitude2);
					if (num5 > num2)
					{
						vector3 = Vector3.ProjectOnPlane(vector5, forwardTangent);
						Vector3 normalized = vector3.normalized;
						if (this.inwardCurrentSpeed > num2 && Vector3.Dot(startingVelocity, normalized) < num5 * this.inwardCurrentSpeed)
						{
							velocityChange += normalized * (this.InwardAccel * dt);
						}
						else if (this.inwardCurrentSpeed < num2 && Vector3.Dot(startingVelocity, normalized) > num5 * this.inwardCurrentSpeed)
						{
							velocityChange -= normalized * (this.InwardAccel * dt);
						}
					}
					this.debugSplinePoint = vector4;
				}
			}
			this.debugCurrentVelocity = velocityChange.normalized;
			return result;
		}

		// Token: 0x060063F4 RID: 25588 RVA: 0x00208B94 File Offset: 0x00206D94
		private void Update()
		{
			if (this.debugDrawCurrentQueries)
			{
				DebugUtil.DrawSphere(this.debugSplinePoint, 0.15f, 12, 12, Color.green, false, DebugUtil.Style.Wireframe);
				DebugUtil.DrawArrow(this.debugSplinePoint, this.debugSplinePoint + this.debugCurrentVelocity, 0.1f, 0.1f, 12, 0.1f, Color.yellow, false, DebugUtil.Style.Wireframe);
			}
		}

		// Token: 0x060063F5 RID: 25589 RVA: 0x00208BF8 File Offset: 0x00206DF8
		private void OnDrawGizmosSelected()
		{
			int num = 16;
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector = catmullRomSpline.Evaluate(0f);
				for (int j = 1; j <= num; j++)
				{
					float t = (float)j / (float)num;
					Vector3 vector2 = catmullRomSpline.Evaluate(t);
					vector2 - vector;
					Quaternion rotation = Quaternion.LookRotation(catmullRomSpline.GetForwardTangent(t, 0.01f), Vector3.up);
					Gizmos.color = new Color(0f, 0.5f, 0.75f);
					this.DrawGizmoCircle(vector2, rotation, this.fullEffectDistance);
					Gizmos.color = new Color(0f, 0.25f, 0.5f);
					this.DrawGizmoCircle(vector2, rotation, this.fullEffectDistance + this.fadeDistance);
				}
			}
		}

		// Token: 0x060063F6 RID: 25590 RVA: 0x00208CE0 File Offset: 0x00206EE0
		private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
		{
			Vector3 vector = Vector3.right * radius;
			int num = 16;
			for (int i = 1; i <= num; i++)
			{
				float num2 = (float)i / (float)num * 2f * 3.1415927f;
				Vector3 vector2 = new Vector3(Mathf.Cos(num2), Mathf.Sin(num2), 0f) * radius;
				Gizmos.DrawLine(center + rotation * vector, center + rotation * vector2);
				vector = vector2;
			}
		}

		// Token: 0x04007377 RID: 29559
		[SerializeField]
		private List<CatmullRomSpline> splines = new List<CatmullRomSpline>();

		// Token: 0x04007378 RID: 29560
		[SerializeField]
		private float fullEffectDistance = 1f;

		// Token: 0x04007379 RID: 29561
		[SerializeField]
		private float fadeDistance = 0.5f;

		// Token: 0x0400737A RID: 29562
		[SerializeField]
		private float currentSpeed = 1f;

		// Token: 0x0400737B RID: 29563
		[SerializeField]
		private float currentAccel = 10f;

		// Token: 0x0400737C RID: 29564
		[SerializeField]
		private float velocityAnticipationAdjustment = 0.05f;

		// Token: 0x0400737D RID: 29565
		[SerializeField]
		private float inwardCurrentFullEffectRadius = 1f;

		// Token: 0x0400737E RID: 29566
		[SerializeField]
		private float inwardCurrentNoEffectRadius = 0.25f;

		// Token: 0x0400737F RID: 29567
		[SerializeField]
		private float inwardCurrentSpeed = 1f;

		// Token: 0x04007380 RID: 29568
		[SerializeField]
		private float inwardCurrentAccel = 10f;

		// Token: 0x04007381 RID: 29569
		[SerializeField]
		private float dampingHalfLife = 0.25f;

		// Token: 0x04007382 RID: 29570
		[SerializeField]
		private bool debugDrawCurrentQueries;

		// Token: 0x04007383 RID: 29571
		private Vector3 debugCurrentVelocity = Vector3.zero;

		// Token: 0x04007384 RID: 29572
		private Vector3 debugSplinePoint = Vector3.zero;
	}
}
