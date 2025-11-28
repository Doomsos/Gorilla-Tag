using System;
using CjLib;
using GorillaTag;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F8B RID: 3979
	[ExecuteAlways]
	public class UnderwaterCameraEffect : MonoBehaviour
	{
		// Token: 0x060063E1 RID: 25569 RVA: 0x00207A24 File Offset: 0x00205C24
		private void SetOffScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 0f, 1f);
			base.transform.localPosition = new Vector3(0f, -(this.frustumPlaneExtents.y + 0.04f), this.distanceFromCamera);
		}

		// Token: 0x060063E2 RID: 25570 RVA: 0x00207A90 File Offset: 0x00205C90
		private void SetFullScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 2f * (this.frustumPlaneExtents.y + 0.04f), 1f);
			base.transform.localPosition = new Vector3(0f, 0f, this.distanceFromCamera);
		}

		// Token: 0x060063E3 RID: 25571 RVA: 0x00207B00 File Offset: 0x00205D00
		private void OnEnable()
		{
			if (this.targetCamera == null)
			{
				this.targetCamera = Camera.main;
			}
			this.hasTargetCamera = (this.targetCamera != null);
			this.InitializeShaderProperties();
		}

		// Token: 0x060063E4 RID: 25572 RVA: 0x00207B34 File Offset: 0x00205D34
		private void Start()
		{
			this.player = GTPlayer.Instance;
			this.cachedAspectRatio = this.targetCamera.aspect;
			this.cachedFov = this.targetCamera.fieldOfView;
			this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			this.SetOffScreenPosition();
		}

		// Token: 0x060063E5 RID: 25573 RVA: 0x00207B88 File Offset: 0x00205D88
		private void LateUpdate()
		{
			if (!this.hasTargetCamera)
			{
				return;
			}
			if (!this.player)
			{
				return;
			}
			if (this.player.HeadOverlappingWaterVolumes.Count < 1)
			{
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				if (this.planeRenderer.enabled)
				{
					this.planeRenderer.enabled = false;
					this.SetOffScreenPosition();
				}
				if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
				{
					this.underwaterParticleEffect.UpdateParticleEffect(false, ref this.waterSurface);
				}
				return;
			}
			if (this.targetCamera.aspect != this.cachedAspectRatio || this.targetCamera.fieldOfView != this.cachedFov)
			{
				this.cachedAspectRatio = this.targetCamera.aspect;
				this.cachedFov = this.targetCamera.fieldOfView;
				this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			}
			bool flag = false;
			float num = float.MinValue;
			Vector3 position = this.targetCamera.transform.position;
			for (int i = 0; i < this.player.HeadOverlappingWaterVolumes.Count; i++)
			{
				WaterVolume.SurfaceQuery surfaceQuery;
				if (this.player.HeadOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(position, out surfaceQuery, false))
				{
					float num2 = Vector3.Dot(surfaceQuery.surfacePoint - position, surfaceQuery.surfaceNormal);
					if (num2 > num)
					{
						flag = true;
						num = num2;
						this.waterSurface = surfaceQuery;
					}
				}
			}
			if (flag)
			{
				Vector3 vector = this.targetCamera.transform.InverseTransformPoint(this.waterSurface.surfacePoint);
				Vector3 vector2 = this.targetCamera.transform.InverseTransformDirection(this.waterSurface.surfaceNormal);
				Plane p;
				p..ctor(vector2, vector);
				Plane p2;
				p2..ctor(Vector3.forward, -this.distanceFromCamera);
				Vector3 vector3;
				Vector3 vector4;
				if (this.IntersectPlanes(p2, p, out vector3, out vector4))
				{
					Vector3 normalized = Vector3.Cross(vector4, Vector3.forward).normalized;
					float num3 = Vector3.Dot(new Vector3(vector3.x, vector3.y, 0f), normalized);
					if (num3 > this.frustumPlaneExtents.y + 0.04f)
					{
						this.SetFullScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
					}
					else if (num3 < -(this.frustumPlaneExtents.y + 0.04f))
					{
						this.SetOffScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
					}
					else
					{
						float num4 = num3;
						num4 += this.GetFrustumCoverageDistance(-normalized) + 0.04f;
						float num5 = this.GetFrustumCoverageDistance(vector4) + 0.04f;
						num5 += this.GetFrustumCoverageDistance(-vector4) + 0.04f;
						base.transform.localScale = new Vector3(num5, num4, 1f);
						base.transform.localPosition = normalized * (num3 - num4 * 0.5f) + new Vector3(0f, 0f, this.distanceFromCamera);
						float num6 = Vector3.SignedAngle(Vector3.up, normalized, Vector3.forward);
						base.transform.localRotation = Quaternion.AngleAxis(num6, Vector3.forward);
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged);
					}
					if (this.debugDraw)
					{
						Vector3 vector5 = this.targetCamera.transform.TransformPoint(vector3);
						Vector3 vector6 = this.targetCamera.transform.TransformDirection(vector4);
						DebugUtil.DrawLine(vector5 - 2f * this.frustumPlaneExtents.x * vector6, vector5 + 2f * this.frustumPlaneExtents.x * vector6, Color.white, false);
					}
				}
				else if (new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint).GetSide(this.targetCamera.transform.position))
				{
					this.SetFullScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
				}
				else
				{
					this.SetOffScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				}
			}
			else
			{
				this.SetOffScreenPosition();
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
			}
			if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
			{
				this.underwaterParticleEffect.UpdateParticleEffect(flag, ref this.waterSurface);
			}
		}

		// Token: 0x060063E6 RID: 25574 RVA: 0x00207FC0 File Offset: 0x002061C0
		[DebugOption]
		private void InitializeShaderProperties()
		{
			Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
			Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
			float num = -Vector3.Dot(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
			Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(this.waterSurface.surfaceNormal.x, this.waterSurface.surfaceNormal.y, this.waterSurface.surfaceNormal.z, num));
		}

		// Token: 0x060063E7 RID: 25575 RVA: 0x00208040 File Offset: 0x00206240
		private void SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState state)
		{
			if (state != this.cameraOverlapWaterState || state == UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized)
			{
				this.cameraOverlapWaterState = state;
				switch (this.cameraOverlapWaterState)
				{
				case UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized:
				case UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater:
					Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.EnableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				}
			}
			if (this.cameraOverlapWaterState == UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged)
			{
				Plane plane;
				plane..ctor(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
				Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));
			}
		}

		// Token: 0x060063E8 RID: 25576 RVA: 0x00208120 File Offset: 0x00206320
		private void CalculateFrustumPlaneBounds(float fieldOfView, float aspectRatio)
		{
			float num = Mathf.Tan(0.017453292f * fieldOfView * 0.5f) * this.distanceFromCamera;
			float num2 = aspectRatio * num + 0.04f;
			float num3 = 1f / aspectRatio * num + 0.04f;
			this.frustumPlaneExtents = new Vector2(num2, num3);
			this.frustumPlaneCornersLocal[0] = new Vector3(-num2, -num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[1] = new Vector3(-num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[2] = new Vector3(num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[3] = new Vector3(num2, -num3, this.distanceFromCamera);
		}

		// Token: 0x060063E9 RID: 25577 RVA: 0x002081D8 File Offset: 0x002063D8
		private bool IntersectPlanes(Plane p1, Plane p2, out Vector3 point, out Vector3 direction)
		{
			direction = Vector3.Cross(p1.normal, p2.normal);
			float num = Vector3.Dot(direction, direction);
			if (num < Mathf.Epsilon)
			{
				point = Vector3.zero;
				return false;
			}
			point = Vector3.Cross(direction, p1.distance * p2.normal - p2.distance * p1.normal) / num;
			return true;
		}

		// Token: 0x060063EA RID: 25578 RVA: 0x0020826C File Offset: 0x0020646C
		private float GetFrustumCoverageDistance(Vector3 localDirection)
		{
			float num = float.MinValue;
			for (int i = 0; i < this.frustumPlaneCornersLocal.Length; i++)
			{
				float num2 = Vector3.Dot(this.frustumPlaneCornersLocal[i], localDirection);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x04007359 RID: 29529
		private const float edgeBuffer = 0.04f;

		// Token: 0x0400735A RID: 29530
		[SerializeField]
		private Camera targetCamera;

		// Token: 0x0400735B RID: 29531
		[SerializeField]
		private MeshRenderer planeRenderer;

		// Token: 0x0400735C RID: 29532
		[SerializeField]
		private UnderwaterParticleEffects underwaterParticleEffect;

		// Token: 0x0400735D RID: 29533
		[SerializeField]
		private float distanceFromCamera = 0.02f;

		// Token: 0x0400735E RID: 29534
		[SerializeField]
		[DebugOption]
		private bool debugDraw;

		// Token: 0x0400735F RID: 29535
		private float cachedAspectRatio = 1f;

		// Token: 0x04007360 RID: 29536
		private float cachedFov = 90f;

		// Token: 0x04007361 RID: 29537
		private readonly Vector3[] frustumPlaneCornersLocal = new Vector3[4];

		// Token: 0x04007362 RID: 29538
		private Vector2 frustumPlaneExtents;

		// Token: 0x04007363 RID: 29539
		private GTPlayer player;

		// Token: 0x04007364 RID: 29540
		private WaterVolume.SurfaceQuery waterSurface;

		// Token: 0x04007365 RID: 29541
		private const string kShaderKeyword_GlobalCameraTouchingWater = "_GLOBAL_CAMERA_TOUCHING_WATER";

		// Token: 0x04007366 RID: 29542
		private const string kShaderKeyword_GlobalCameraFullyUnderwater = "_GLOBAL_CAMERA_FULLY_UNDERWATER";

		// Token: 0x04007367 RID: 29543
		private int shaderParam_GlobalCameraOverlapWaterSurfacePlane = Shader.PropertyToID("_GlobalCameraOverlapWaterSurfacePlane");

		// Token: 0x04007368 RID: 29544
		private bool hasTargetCamera;

		// Token: 0x04007369 RID: 29545
		[DebugReadout]
		private UnderwaterCameraEffect.CameraOverlapWaterState cameraOverlapWaterState;

		// Token: 0x02000F8C RID: 3980
		private enum CameraOverlapWaterState
		{
			// Token: 0x0400736B RID: 29547
			Uninitialized,
			// Token: 0x0400736C RID: 29548
			OutOfWater,
			// Token: 0x0400736D RID: 29549
			PartiallySubmerged,
			// Token: 0x0400736E RID: 29550
			FullySubmerged
		}
	}
}
