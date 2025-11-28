using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001078 RID: 4216
	public class RendererCullerByTriggers : MonoBehaviour, IBuildValidation
	{
		// Token: 0x060069CF RID: 27087 RVA: 0x00226B78 File Offset: 0x00224D78
		protected void OnEnable()
		{
			this.camWasTouching = false;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
		}

		// Token: 0x060069D0 RID: 27088 RVA: 0x00226BD4 File Offset: 0x00224DD4
		protected void LateUpdate()
		{
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
			Vector3 position = this.mainCameraTransform.position;
			bool flag = false;
			foreach (Collider collider in this.colliders)
			{
				if (!(collider == null) && (collider.ClosestPoint(position) - position).sqrMagnitude < 0.010000001f)
				{
					flag = true;
					break;
				}
			}
			if (this.camWasTouching == flag)
			{
				return;
			}
			this.camWasTouching = flag;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = flag;
				}
			}
		}

		// Token: 0x060069D1 RID: 27089 RVA: 0x00226C94 File Offset: 0x00224E94
		public bool BuildValidationCheck()
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				if (this.renderers[i] == null)
				{
					Debug.LogError("rendererculllerbytriggers has null renderer", base.gameObject);
					return false;
				}
			}
			for (int j = 0; j < this.colliders.Length; j++)
			{
				if (this.colliders[j] == null)
				{
					Debug.LogError("rendererculllerbytriggers has null collider", base.gameObject);
					return false;
				}
			}
			return true;
		}

		// Token: 0x04007914 RID: 30996
		[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
		public Renderer[] renderers;

		// Token: 0x04007915 RID: 30997
		public Collider[] colliders;

		// Token: 0x04007916 RID: 30998
		private bool camWasTouching;

		// Token: 0x04007917 RID: 30999
		private const float cameraRadiusSq = 0.010000001f;

		// Token: 0x04007918 RID: 31000
		private Transform mainCameraTransform;
	}
}
