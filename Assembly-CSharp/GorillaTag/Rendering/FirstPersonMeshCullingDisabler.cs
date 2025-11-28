using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001079 RID: 4217
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		// Token: 0x060069D3 RID: 27091 RVA: 0x00226D2C File Offset: 0x00224F2C
		protected void Awake()
		{
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren == null)
			{
				return;
			}
			this.meshes = new Mesh[componentsInChildren.Length];
			this.xforms = new Transform[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.meshes[i] = componentsInChildren[i].mesh;
				this.xforms[i] = componentsInChildren[i].transform;
			}
		}

		// Token: 0x060069D4 RID: 27092 RVA: 0x00226D90 File Offset: 0x00224F90
		protected void OnEnable()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Transform transform = main.transform;
			Vector3 position = transform.position;
			Vector3 vector = Vector3.Normalize(transform.forward);
			float nearClipPlane = main.nearClipPlane;
			float num = (main.farClipPlane - nearClipPlane) / 2f + nearClipPlane;
			Vector3 vector2 = position + vector * num;
			for (int i = 0; i < this.meshes.Length; i++)
			{
				Vector3 vector3 = this.xforms[i].InverseTransformPoint(vector2);
				this.meshes[i].bounds = new Bounds(vector3, Vector3.one);
			}
		}

		// Token: 0x04007919 RID: 31001
		private Mesh[] meshes;

		// Token: 0x0400791A RID: 31002
		private Transform[] xforms;
	}
}
