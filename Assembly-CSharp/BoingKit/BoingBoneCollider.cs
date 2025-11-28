using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200117E RID: 4478
	public class BoingBoneCollider : MonoBehaviour
	{
		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x06007110 RID: 28944 RVA: 0x0025024C File Offset: 0x0024E44C
		public Bounds Bounds
		{
			get
			{
				switch (this.Shape)
				{
				case BoingBoneCollider.Type.Sphere:
				{
					float num = VectorUtil.MinComponent(base.transform.localScale);
					return new Bounds(base.transform.position, 2f * num * this.Radius * Vector3.one);
				}
				case BoingBoneCollider.Type.Capsule:
				{
					float num2 = VectorUtil.MinComponent(base.transform.localScale);
					return new Bounds(base.transform.position, 2f * num2 * this.Radius * Vector3.one + this.Height * VectorUtil.ComponentWiseAbs(base.transform.rotation * Vector3.up));
				}
				case BoingBoneCollider.Type.Box:
					return new Bounds(base.transform.position, VectorUtil.ComponentWiseMult(base.transform.localScale, VectorUtil.ComponentWiseAbs(base.transform.rotation * this.Dimensions)));
				default:
					return default(Bounds);
				}
			}
		}

		// Token: 0x06007111 RID: 28945 RVA: 0x0025035C File Offset: 0x0024E55C
		public bool Collide(Vector3 boneCenter, float boneRadius, out Vector3 push)
		{
			switch (this.Shape)
			{
			case BoingBoneCollider.Type.Sphere:
			{
				float num = VectorUtil.MinComponent(base.transform.localScale);
				return Collision.SphereSphere(boneCenter, boneRadius, base.transform.position, num * this.Radius, out push);
			}
			case BoingBoneCollider.Type.Capsule:
			{
				float num2 = VectorUtil.MinComponent(base.transform.localScale);
				Vector3 headB = base.transform.TransformPoint(0.5f * this.Height * Vector3.up);
				Vector3 tailB = base.transform.TransformPoint(0.5f * this.Height * Vector3.down);
				return Collision.SphereCapsule(boneCenter, boneRadius, headB, tailB, num2 * this.Radius, out push);
			}
			case BoingBoneCollider.Type.Box:
			{
				Vector3 centerOffsetA = base.transform.InverseTransformPoint(boneCenter);
				Vector3 halfExtentB = 0.5f * VectorUtil.ComponentWiseMult(base.transform.localScale, this.Dimensions);
				if (!Collision.SphereBox(centerOffsetA, boneRadius, halfExtentB, out push))
				{
					return false;
				}
				push = base.transform.TransformVector(push);
				return true;
			}
			default:
				push = Vector3.zero;
				return false;
			}
		}

		// Token: 0x06007112 RID: 28946 RVA: 0x00250480 File Offset: 0x0024E680
		public void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.Dimensions.x = Mathf.Max(0f, this.Dimensions.x);
			this.Dimensions.y = Mathf.Max(0f, this.Dimensions.y);
			this.Dimensions.z = Mathf.Max(0f, this.Dimensions.z);
		}

		// Token: 0x06007113 RID: 28947 RVA: 0x00250503 File Offset: 0x0024E703
		public void OnDrawGizmos()
		{
			this.DrawGizmos();
		}

		// Token: 0x06007114 RID: 28948 RVA: 0x0025050C File Offset: 0x0024E70C
		public void DrawGizmos()
		{
			switch (this.Shape)
			{
			case BoingBoneCollider.Type.Sphere:
			{
				float num = VectorUtil.MinComponent(base.transform.localScale) * this.Radius;
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
				if (this.Shape == BoingBoneCollider.Type.Sphere)
				{
					Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
					Gizmos.DrawSphere(Vector3.zero, num);
				}
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(Vector3.zero, num);
				Gizmos.matrix = Matrix4x4.identity;
				return;
			}
			case BoingBoneCollider.Type.Capsule:
			{
				float num2 = VectorUtil.MinComponent(base.transform.localScale);
				float num3 = num2 * this.Radius;
				float num4 = 0.5f * num2 * this.Height;
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
				if (this.Shape == BoingBoneCollider.Type.Capsule)
				{
					Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
					Gizmos.DrawSphere(num4 * Vector3.up, num3);
					Gizmos.DrawSphere(num4 * Vector3.down, num3);
				}
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(num4 * Vector3.up, num3);
				Gizmos.DrawWireSphere(num4 * Vector3.down, num3);
				for (int i = 0; i < 4; i++)
				{
					float num5 = (float)i * MathUtil.HalfPi;
					Vector3 vector;
					vector..ctor(num3 * Mathf.Cos(num5), 0f, num3 * Mathf.Sin(num5));
					Gizmos.DrawLine(vector + num4 * Vector3.up, vector + num4 * Vector3.down);
				}
				Gizmos.matrix = Matrix4x4.identity;
				return;
			}
			case BoingBoneCollider.Type.Box:
			{
				Vector3 vector2 = VectorUtil.ComponentWiseMult(base.transform.localScale, this.Dimensions);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				if (this.Shape == BoingBoneCollider.Type.Box)
				{
					Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
					Gizmos.DrawCube(Vector3.zero, vector2);
				}
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(Vector3.zero, vector2);
				Gizmos.matrix = Matrix4x4.identity;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04008141 RID: 33089
		public BoingBoneCollider.Type Shape;

		// Token: 0x04008142 RID: 33090
		public float Radius = 0.1f;

		// Token: 0x04008143 RID: 33091
		public float Height = 0.25f;

		// Token: 0x04008144 RID: 33092
		public Vector3 Dimensions = new Vector3(0.1f, 0.1f, 0.1f);

		// Token: 0x0200117F RID: 4479
		public enum Type
		{
			// Token: 0x04008146 RID: 33094
			Sphere,
			// Token: 0x04008147 RID: 33095
			Capsule,
			// Token: 0x04008148 RID: 33096
			Box
		}
	}
}
