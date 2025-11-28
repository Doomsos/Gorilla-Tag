using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000FD1 RID: 4049
	[Serializable]
	public struct XformOffset
	{
		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x060066A4 RID: 26276 RVA: 0x00216DC9 File Offset: 0x00214FC9
		// (set) Token: 0x060066A5 RID: 26277 RVA: 0x00216DD1 File Offset: 0x00214FD1
		[Tooltip("The rotation of the offset relative to the parent bone.")]
		public Quaternion rot
		{
			get
			{
				return this._rotQuat;
			}
			set
			{
				this._rotQuat = value;
			}
		}

		// Token: 0x060066A6 RID: 26278 RVA: 0x00216DDA File Offset: 0x00214FDA
		public XformOffset(Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = scale;
		}

		// Token: 0x060066A7 RID: 26279 RVA: 0x00216DFE File Offset: 0x00214FFE
		public XformOffset(Vector3 pos, Vector3 rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = scale;
		}

		// Token: 0x060066A8 RID: 26280 RVA: 0x00216E21 File Offset: 0x00215021
		public XformOffset(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = Vector3.one;
		}

		// Token: 0x060066A9 RID: 26281 RVA: 0x00216E49 File Offset: 0x00215049
		public XformOffset(Vector3 pos, Vector3 rot)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = Vector3.one;
		}

		// Token: 0x060066AA RID: 26282 RVA: 0x00216E70 File Offset: 0x00215070
		public XformOffset(Transform parentXform, Transform childXform)
		{
			this.pos = parentXform.InverseTransformPoint(childXform.position);
			this._rotQuat = Quaternion.Inverse(parentXform.rotation) * childXform.rotation;
			this._rotEulerAngles = this._rotQuat.eulerAngles;
			this.scale = childXform.lossyScale.SafeDivide(parentXform.lossyScale);
		}

		// Token: 0x060066AB RID: 26283 RVA: 0x00216ED4 File Offset: 0x002150D4
		public XformOffset(Matrix4x4 matrix)
		{
			this.pos = matrix.GetPosition();
			this.scale = matrix.lossyScale;
			if (Vector3.Dot(Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)), matrix.GetColumn(2)) < 0f)
			{
				this.scale = -this.scale;
			}
			Matrix4x4 matrix4x = matrix;
			matrix4x.SetColumn(0, matrix4x.GetColumn(0) / this.scale.x);
			matrix4x.SetColumn(1, matrix4x.GetColumn(1) / this.scale.y);
			matrix4x.SetColumn(2, matrix4x.GetColumn(2) / this.scale.z);
			this._rotQuat = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
			this._rotEulerAngles = this._rotQuat.eulerAngles;
		}

		// Token: 0x060066AC RID: 26284 RVA: 0x00216FDC File Offset: 0x002151DC
		public bool Approx(XformOffset other)
		{
			return this.pos.Approx(other.pos, 1E-05f) && this._rotQuat.Approx(other._rotQuat, 1E-06f) && this.scale.Approx(other.scale, 1E-05f);
		}

		// Token: 0x04007551 RID: 30033
		[Tooltip("The position of the offset relative to the parent bone.")]
		public Vector3 pos;

		// Token: 0x04007552 RID: 30034
		[FormerlySerializedAs("_edRotQuat")]
		[FormerlySerializedAs("rot")]
		[HideInInspector]
		[SerializeField]
		private Quaternion _rotQuat;

		// Token: 0x04007553 RID: 30035
		[FormerlySerializedAs("_edRotEulerAngles")]
		[FormerlySerializedAs("_edRotEuler")]
		[HideInInspector]
		[SerializeField]
		private Vector3 _rotEulerAngles;

		// Token: 0x04007554 RID: 30036
		[Tooltip("The scale of the offset relative to the parent bone.")]
		public Vector3 scale;

		// Token: 0x04007555 RID: 30037
		public static readonly XformOffset Identity = new XformOffset
		{
			_rotQuat = Quaternion.identity,
			scale = Vector3.one
		};
	}
}
