using System;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion.Climbing;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F92 RID: 3986
	[RequireComponent(typeof(Collider))]
	public class WaterVolume : BaseGuidedRefTargetMono, ITickSystemTick
	{
		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x060063FF RID: 25599 RVA: 0x0020925C File Offset: 0x0020745C
		// (set) Token: 0x06006400 RID: 25600 RVA: 0x00209264 File Offset: 0x00207464
		public bool TickRunning { get; set; }

		// Token: 0x140000B0 RID: 176
		// (add) Token: 0x06006401 RID: 25601 RVA: 0x00209270 File Offset: 0x00207470
		// (remove) Token: 0x06006402 RID: 25602 RVA: 0x002092A8 File Offset: 0x002074A8
		public event WaterVolume.WaterVolumeEvent ColliderEnteredVolume;

		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06006403 RID: 25603 RVA: 0x002092E0 File Offset: 0x002074E0
		// (remove) Token: 0x06006404 RID: 25604 RVA: 0x00209318 File Offset: 0x00207518
		public event WaterVolume.WaterVolumeEvent ColliderExitedVolume;

		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06006405 RID: 25605 RVA: 0x00209350 File Offset: 0x00207550
		// (remove) Token: 0x06006406 RID: 25606 RVA: 0x00209388 File Offset: 0x00207588
		public event WaterVolume.WaterVolumeEvent ColliderEnteredWater;

		// Token: 0x140000B3 RID: 179
		// (add) Token: 0x06006407 RID: 25607 RVA: 0x002093C0 File Offset: 0x002075C0
		// (remove) Token: 0x06006408 RID: 25608 RVA: 0x002093F8 File Offset: 0x002075F8
		public event WaterVolume.WaterVolumeEvent ColliderExitedWater;

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x06006409 RID: 25609 RVA: 0x0020942D File Offset: 0x0020762D
		public GTPlayer.LiquidType LiquidType
		{
			get
			{
				return this.liquidType;
			}
		}

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x0600640A RID: 25610 RVA: 0x00209435 File Offset: 0x00207635
		public WaterCurrent Current
		{
			get
			{
				return this.waterCurrent;
			}
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x0600640B RID: 25611 RVA: 0x0020943D File Offset: 0x0020763D
		public WaterParameters Parameters
		{
			get
			{
				return this.waterParams;
			}
		}

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x0600640C RID: 25612 RVA: 0x00209448 File Offset: 0x00207648
		private VRRig PlayerVRRig
		{
			get
			{
				if (this.playerVRRig == null)
				{
					GorillaTagger instance = GorillaTagger.Instance;
					if (instance != null)
					{
						this.playerVRRig = instance.offlineVRRig;
					}
				}
				return this.playerVRRig;
			}
		}

		// Token: 0x0600640D RID: 25613 RVA: 0x00209484 File Offset: 0x00207684
		public bool GetSurfaceQueryForPoint(Vector3 point, out WaterVolume.SurfaceQuery result, bool debugDraw = false)
		{
			result = default(WaterVolume.SurfaceQuery);
			if (!this.isStationary)
			{
				float num = float.MinValue;
				float num2 = float.MaxValue;
				for (int i = 0; i < this.volumeColliders.Count; i++)
				{
					float y = this.volumeColliders[i].bounds.max.y;
					float y2 = this.volumeColliders[i].bounds.min.y;
					if (y > num)
					{
						num = y;
					}
					if (y2 < num2)
					{
						num2 = y2;
					}
				}
				this.volumeMaxHeight = num;
				this.volumeMinHeight = num2;
			}
			Ray ray;
			ray..ctor(new Vector3(point.x, this.volumeMaxHeight, point.z), Vector3.down);
			Ray ray2;
			ray2..ctor(new Vector3(point.x, this.volumeMinHeight, point.z), Vector3.up);
			float num3 = this.volumeMaxHeight - this.volumeMinHeight;
			float num4 = float.MinValue;
			float num5 = float.MaxValue;
			bool flag = false;
			bool flag2 = false;
			float num6 = 0f;
			for (int j = 0; j < this.surfaceColliders.Count; j++)
			{
				bool enabled = this.surfaceColliders[j].enabled;
				this.surfaceColliders[j].enabled = true;
				RaycastHit hit;
				if (this.surfaceColliders[j].Raycast(ray, ref hit, num3) && hit.point.y > num4 && this.HitOutsideSurfaceOfMesh(ray.direction, this.surfaceColliders[j], hit))
				{
					num4 = hit.point.y;
					flag = true;
					result.surfacePoint = hit.point;
					result.surfaceNormal = hit.normal;
				}
				RaycastHit hit2;
				if (this.surfaceColliders[j].Raycast(ray2, ref hit2, num3) && hit2.point.y < num5 && this.HitOutsideSurfaceOfMesh(ray2.direction, this.surfaceColliders[j], hit2))
				{
					num5 = hit2.point.y;
					flag2 = true;
					num6 = hit2.point.y;
				}
				this.surfaceColliders[j].enabled = enabled;
			}
			if (!flag && this.surfacePlane != null)
			{
				flag = true;
				result.surfacePoint = point - Vector3.Dot(point - this.surfacePlane.position, this.surfacePlane.up) * this.surfacePlane.up;
				result.surfaceNormal = this.surfacePlane.up;
			}
			if (flag && flag2)
			{
				result.maxDepth = result.surfacePoint.y - num6;
			}
			else if (flag)
			{
				result.maxDepth = result.surfacePoint.y - this.volumeMinHeight;
			}
			else
			{
				result.maxDepth = this.volumeMaxHeight - this.volumeMinHeight;
			}
			if (debugDraw)
			{
				if (flag)
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num3, Color.green, false);
					DebugUtil.DrawSphere(result.surfacePoint, 0.001f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
				}
				else
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num3, Color.red, false);
				}
				if (flag2)
				{
					DebugUtil.DrawLine(ray2.origin, ray2.origin + ray2.direction * num3, Color.yellow, false);
					DebugUtil.DrawSphere(new Vector3(result.surfacePoint.x, num6, result.surfacePoint.z), 0.001f, 12, 12, Color.yellow, false, DebugUtil.Style.SolidColor);
				}
			}
			return flag;
		}

		// Token: 0x0600640E RID: 25614 RVA: 0x00209864 File Offset: 0x00207A64
		private bool HitOutsideSurfaceOfMesh(Vector3 castDir, MeshCollider meshCollider, RaycastHit hit)
		{
			if (!WaterVolume.meshTrianglesDict.TryGetValue(meshCollider.sharedMesh, ref this.sharedMeshTris))
			{
				this.sharedMeshTris = (int[])meshCollider.sharedMesh.triangles.Clone();
				WaterVolume.meshTrianglesDict.Add(meshCollider.sharedMesh, this.sharedMeshTris);
			}
			if (!WaterVolume.meshVertsDict.TryGetValue(meshCollider.sharedMesh, ref this.sharedMeshVerts))
			{
				this.sharedMeshVerts = (Vector3[])meshCollider.sharedMesh.vertices.Clone();
				WaterVolume.meshVertsDict.Add(meshCollider.sharedMesh, this.sharedMeshVerts);
			}
			Vector3 vector = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3]];
			Vector3 vector2 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 1]];
			Vector3 vector3 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 2]];
			Vector3 vector4 = meshCollider.transform.TransformDirection(Vector3.Cross(vector2 - vector, vector3 - vector).normalized);
			bool flag = Vector3.Dot(castDir, vector4) < 0f;
			if (this.debugDrawSurfaceCast)
			{
				Color color = flag ? Color.blue : Color.red;
				DebugUtil.DrawLine(hit.point, hit.point + vector4 * 0.3f, color, false);
			}
			return flag;
		}

		// Token: 0x0600640F RID: 25615 RVA: 0x002099D8 File Offset: 0x00207BD8
		private void DebugDrawMeshColliderHitTriangle(RaycastHit hit)
		{
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider != null)
			{
				Mesh sharedMesh = meshCollider.sharedMesh;
				int[] triangles = sharedMesh.triangles;
				Vector3[] vertices = sharedMesh.vertices;
				Vector3 vector = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
				Vector3 vector2 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
				Vector3 vector3 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
				Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
				float num = 0.2f;
				DebugUtil.DrawLine(vector, vector + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector2 + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector3, vector3 + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector, vector2, Color.blue, false);
				DebugUtil.DrawLine(vector, vector3, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector3, Color.blue, false);
			}
		}

		// Token: 0x06006410 RID: 25616 RVA: 0x00209B24 File Offset: 0x00207D24
		public bool RaycastWater(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, int layerMask)
		{
			if (this.triggerCollider != null)
			{
				return Physics.Raycast(new Ray(origin, direction), ref hit, distance, layerMask, 2);
			}
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x00209B50 File Offset: 0x00207D50
		public bool CheckColliderInVolume(Collider collider, out bool inWater, out bool surfaceDetected)
		{
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == collider)
				{
					inWater = this.persistentColliders[i].inWater;
					surfaceDetected = this.persistentColliders[i].surfaceDetected;
					return true;
				}
			}
			inWater = false;
			surfaceDetected = false;
			return false;
		}

		// Token: 0x06006412 RID: 25618 RVA: 0x00209BBB File Offset: 0x00207DBB
		protected override void Awake()
		{
			base.Awake();
			this.RefreshColliders();
		}

		// Token: 0x06006413 RID: 25619 RVA: 0x0001877F File Offset: 0x0001697F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006414 RID: 25620 RVA: 0x00209BCC File Offset: 0x00207DCC
		public void RefreshColliders()
		{
			this.triggerCollider = base.GetComponent<Collider>();
			if (this.volumeColliders == null || this.volumeColliders.Count < 1)
			{
				this.volumeColliders = new List<Collider>();
				this.volumeColliders.Add(base.gameObject.GetComponent<Collider>());
			}
			float num = float.MinValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.volumeColliders.Count; i++)
			{
				float y = this.volumeColliders[i].bounds.max.y;
				float y2 = this.volumeColliders[i].bounds.min.y;
				if (y > num)
				{
					num = y;
				}
				if (y2 < num2)
				{
					num2 = y2;
				}
			}
			this.volumeMaxHeight = num;
			this.volumeMinHeight = num2;
		}

		// Token: 0x06006415 RID: 25621 RVA: 0x00209C9C File Offset: 0x00207E9C
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				waterOverlappingCollider.inVolume = false;
				waterOverlappingCollider.playDripEffect = false;
				WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
				if (colliderExitedVolume != null)
				{
					colliderExitedVolume(this, waterOverlappingCollider.collider);
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
			this.RemoveCollidersOutsideVolume(Time.time);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006416 RID: 25622 RVA: 0x00209D1C File Offset: 0x00207F1C
		public void Tick()
		{
			if (this.persistentColliders.Count < 1)
			{
				return;
			}
			float time = Time.time;
			this.RemoveCollidersOutsideVolume(time);
			if (!this.CanPlayerSwim())
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				bool inWater = waterOverlappingCollider.inWater;
				if (waterOverlappingCollider.inVolume)
				{
					this.CheckColliderAgainstWater(ref waterOverlappingCollider, time);
				}
				else
				{
					waterOverlappingCollider.inWater = false;
				}
				this.TryRegisterOwnershipOfCollider(waterOverlappingCollider.collider, waterOverlappingCollider.inWater, waterOverlappingCollider.surfaceDetected);
				if (waterOverlappingCollider.inWater && !inWater)
				{
					this.OnWaterSurfaceEnter(ref waterOverlappingCollider);
				}
				else if (!waterOverlappingCollider.inWater && inWater)
				{
					this.OnWaterSurfaceExit(ref waterOverlappingCollider, time);
				}
				if (this.HasOwnershipOfCollider(waterOverlappingCollider.collider) && waterOverlappingCollider.surfaceDetected)
				{
					if (!waterOverlappingCollider.inWater)
					{
						this.ColliderOutOfWaterUpdate(ref waterOverlappingCollider, time);
					}
					else
					{
						this.ColliderInWaterUpdate(ref waterOverlappingCollider, time);
					}
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
		}

		// Token: 0x06006417 RID: 25623 RVA: 0x00209E1C File Offset: 0x0020801C
		private void RemoveCollidersOutsideVolume(float currentTime)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = this.persistentColliders.Count - 1; i >= 0; i--)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				if (waterOverlappingCollider.collider == null || !waterOverlappingCollider.collider.gameObject.activeInHierarchy || (!waterOverlappingCollider.inVolume && (!waterOverlappingCollider.playDripEffect || currentTime - waterOverlappingCollider.lastInWaterTime > this.waterParams.postExitDripDuration)) || !this.CanPlayerSwim())
				{
					this.UnregisterOwnershipOfCollider(waterOverlappingCollider.collider);
					GTPlayer instance = GTPlayer.Instance;
					if (waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider)
					{
						instance.OnExitWaterVolume(waterOverlappingCollider.collider, this);
					}
					this.persistentColliders.RemoveAt(i);
				}
			}
		}

		// Token: 0x06006418 RID: 25624 RVA: 0x00209EFC File Offset: 0x002080FC
		private void CheckColliderAgainstWater(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 position = persistentCollider.collider.transform.position;
			bool flag = true;
			if (persistentCollider.surfaceDetected && persistentCollider.scaleMultiplier > 0.99f && this.isStationary)
			{
				flag = ((position - Vector3.Dot(position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) * persistentCollider.lastSurfaceQuery.surfaceNormal - persistentCollider.lastSurfaceQuery.surfacePoint).sqrMagnitude > this.waterParams.recomputeSurfaceForColliderDist * this.waterParams.recomputeSurfaceForColliderDist);
			}
			if (flag)
			{
				WaterVolume.SurfaceQuery lastSurfaceQuery;
				if (this.GetSurfaceQueryForPoint(position, out lastSurfaceQuery, this.debugDrawSurfaceCast))
				{
					persistentCollider.surfaceDetected = true;
					persistentCollider.lastSurfaceQuery = lastSurfaceQuery;
				}
				else
				{
					persistentCollider.surfaceDetected = false;
					persistentCollider.lastSurfaceQuery = default(WaterVolume.SurfaceQuery);
				}
			}
			if (persistentCollider.surfaceDetected)
			{
				bool flag2 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.down * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.down * 10f)).y < persistentCollider.lastSurfaceQuery.surfacePoint.y;
				bool flag3 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.up * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.up * 10f)).y > persistentCollider.lastSurfaceQuery.surfacePoint.y - persistentCollider.lastSurfaceQuery.maxDepth;
				persistentCollider.inWater = (flag2 && flag3);
			}
			else
			{
				persistentCollider.inWater = false;
			}
			if (persistentCollider.inWater)
			{
				persistentCollider.lastInWaterTime = currentTime;
			}
		}

		// Token: 0x06006419 RID: 25625 RVA: 0x0020A0E4 File Offset: 0x002082E4
		private Vector3 GetColliderVelocity(ref WaterOverlappingCollider persistentCollider)
		{
			GTPlayer instance = GTPlayer.Instance;
			Vector3 result = Vector3.one * (this.waterParams.splashSpeedRequirement + 0.1f);
			if (persistentCollider.velocityTracker != null)
			{
				result = persistentCollider.velocityTracker.GetAverageVelocity(true, 0.1f, false);
			}
			else if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				result = instance.AveragedVelocity;
			}
			else if (persistentCollider.collider.attachedRigidbody != null && !persistentCollider.collider.attachedRigidbody.isKinematic)
			{
				result = persistentCollider.collider.attachedRigidbody.linearVelocity;
			}
			return result;
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x0020A19C File Offset: 0x0020839C
		private void OnWaterSurfaceEnter(ref WaterOverlappingCollider persistentCollider)
		{
			WaterVolume.WaterVolumeEvent colliderEnteredWater = this.ColliderEnteredWater;
			if (colliderEnteredWater != null)
			{
				colliderEnteredWater(this, persistentCollider.collider);
			}
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnEnterWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				Vector3 colliderVelocity = this.GetColliderVelocity(ref persistentCollider);
				bool flag = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, true, this);
				}
			}
		}

		// Token: 0x0600641B RID: 25627 RVA: 0x0020A2E8 File Offset: 0x002084E8
		private void OnWaterSurfaceExit(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			WaterVolume.WaterVolumeEvent colliderExitedWater = this.ColliderExitedWater;
			if (colliderExitedWater != null)
			{
				colliderExitedWater(this, persistentCollider.collider);
			}
			persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnExitWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				float num = Vector3.Dot(this.GetColliderVelocity(ref persistentCollider), persistentCollider.lastSurfaceQuery.surfaceNormal);
				bool flag = num > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = num > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, false, this);
				}
			}
		}

		// Token: 0x0600641C RID: 25628 RVA: 0x0020A454 File Offset: 0x00208654
		private void ColliderOutOfWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			if (currentTime < persistentCollider.lastInWaterTime + this.waterParams.postExitDripDuration && currentTime > persistentCollider.nextDripTime && persistentCollider.playDripEffect)
			{
				persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
				float dripScale = this.waterParams.rippleEffectScale * 2f * (this.waterParams.perDripDefaultRadius + Random.Range(-this.waterParams.perDripRadiusRandRange * 0.5f, this.waterParams.perDripRadiusRandRange * 0.5f));
				persistentCollider.PlayDripEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, dripScale);
			}
		}

		// Token: 0x0600641D RID: 25629 RVA: 0x0020A53C File Offset: 0x0020873C
		private void ColliderInWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 vector = Vector3.ProjectOnPlane(persistentCollider.collider.transform.position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) + persistentCollider.lastSurfaceQuery.surfacePoint;
			bool flag;
			if (persistentCollider.overrideBoundingRadius)
			{
				flag = ((persistentCollider.collider.transform.position - vector).sqrMagnitude < persistentCollider.boundingRadiusOverride * persistentCollider.boundingRadiusOverride);
			}
			else
			{
				flag = ((persistentCollider.collider.ClosestPointOnBounds(vector) - vector).sqrMagnitude < 0.001f);
			}
			if (flag)
			{
				float num = Mathf.Max(this.waterParams.minDistanceBetweenRipples, this.waterParams.defaultDistanceBetweenRipples * (persistentCollider.lastRippleScale / this.waterParams.rippleEffectScale));
				bool flag2 = (persistentCollider.lastRipplePosition - vector).sqrMagnitude > num * num;
				bool flag3 = currentTime - persistentCollider.lastRippleTime > this.waterParams.minTimeBetweenRipples;
				if (flag2 || flag3)
				{
					persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, currentTime, this);
					return;
				}
			}
			else
			{
				persistentCollider.lastRippleTime = currentTime;
			}
		}

		// Token: 0x0600641E RID: 25630 RVA: 0x0020A68C File Offset: 0x0020888C
		private void TryRegisterOwnershipOfCollider(Collider collider, bool isInWater, bool isSurfaceDetected)
		{
			WaterVolume waterVolume;
			if (WaterVolume.sharedColliderRegistry.TryGetValue(collider, ref waterVolume))
			{
				if (waterVolume != this)
				{
					bool flag;
					bool flag2;
					waterVolume.CheckColliderInVolume(collider, out flag, out flag2);
					if ((isSurfaceDetected && !flag2) || (isInWater && !flag))
					{
						WaterVolume.sharedColliderRegistry.Remove(collider);
						WaterVolume.sharedColliderRegistry.Add(collider, this);
						return;
					}
				}
			}
			else
			{
				WaterVolume.sharedColliderRegistry.Add(collider, this);
			}
		}

		// Token: 0x0600641F RID: 25631 RVA: 0x0020A6EE File Offset: 0x002088EE
		private void UnregisterOwnershipOfCollider(Collider collider)
		{
			if (WaterVolume.sharedColliderRegistry.ContainsKey(collider))
			{
				WaterVolume.sharedColliderRegistry.Remove(collider);
			}
		}

		// Token: 0x06006420 RID: 25632 RVA: 0x0020A70C File Offset: 0x0020890C
		private bool HasOwnershipOfCollider(Collider collider)
		{
			WaterVolume waterVolume;
			return WaterVolume.sharedColliderRegistry.TryGetValue(collider, ref waterVolume) && waterVolume == this;
		}

		// Token: 0x06006421 RID: 25633 RVA: 0x0020A734 File Offset: 0x00208934
		protected virtual bool CanPlayerSwim()
		{
			if (this.isMonkeblock && this.PlayerVRRig != null)
			{
				if (this.PlayerVRRig.scaleFactor < 0.5f)
				{
					return true;
				}
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(this.PlayerVRRig.zoneEntity.currentZone, out builderTable))
				{
					return !builderTable.isTableMutable;
				}
			}
			return true;
		}

		// Token: 0x06006422 RID: 25634 RVA: 0x0020A790 File Offset: 0x00208990
		public void OnTriggerEnter(Collider other)
		{
			if (!this.CanPlayerSwim())
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderEnteredVolume = this.ColliderEnteredVolume;
			if (colliderEnteredVolume != null)
			{
				colliderEnteredVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
					waterOverlappingCollider.inVolume = true;
					this.persistentColliders[i] = waterOverlappingCollider;
					return;
				}
			}
			WaterOverlappingCollider waterOverlappingCollider2 = new WaterOverlappingCollider
			{
				collider = other
			};
			waterOverlappingCollider2.inVolume = true;
			waterOverlappingCollider2.lastInWaterTime = Time.time - this.waterParams.postExitDripDuration - 10f;
			WaterSplashOverride component2 = other.GetComponent<WaterSplashOverride>();
			if (component2 != null)
			{
				if (component2.suppressWaterEffects)
				{
					return;
				}
				waterOverlappingCollider2.playBigSplash = component2.playBigSplash;
				waterOverlappingCollider2.playDripEffect = component2.playDrippingEffect;
				waterOverlappingCollider2.overrideBoundingRadius = component2.overrideBoundingRadius;
				waterOverlappingCollider2.boundingRadiusOverride = component2.boundingRadiusOverride;
				waterOverlappingCollider2.scaleMultiplier = (component2.scaleByPlayersScale ? GTPlayer.Instance.scale : 1f);
			}
			else
			{
				if (other.GetComponent<BuilderPieceCollider>() != null)
				{
					return;
				}
				waterOverlappingCollider2.playDripEffect = true;
				waterOverlappingCollider2.overrideBoundingRadius = false;
				waterOverlappingCollider2.scaleMultiplier = 1f;
				waterOverlappingCollider2.playBigSplash = false;
			}
			GTPlayer instance = GTPlayer.Instance;
			if (component != null)
			{
				waterOverlappingCollider2.velocityTracker = instance.GetHandVelocityTracker(component.isLeftHand);
				waterOverlappingCollider2.scaleMultiplier = instance.scale;
			}
			else
			{
				waterOverlappingCollider2.velocityTracker = other.GetComponent<GorillaVelocityTracker>();
			}
			if (this.PlayerVRRig != null && this.waterParams.sendSplashEffectRPCs && (component != null || waterOverlappingCollider2.collider == instance.headCollider || waterOverlappingCollider2.collider == instance.bodyCollider))
			{
				waterOverlappingCollider2.photonViewForRPC = this.PlayerVRRig.netView;
			}
			this.persistentColliders.Add(waterOverlappingCollider2);
		}

		// Token: 0x06006423 RID: 25635 RVA: 0x0020A9B0 File Offset: 0x00208BB0
		private void OnTriggerExit(Collider other)
		{
			if (!this.CanPlayerSwim())
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
			if (colliderExitedVolume != null)
			{
				colliderExitedVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
					waterOverlappingCollider.inVolume = false;
					this.persistentColliders[i] = waterOverlappingCollider;
				}
			}
		}

		// Token: 0x06006424 RID: 25636 RVA: 0x0020AA43 File Offset: 0x00208C43
		public void SetPropertiesFromPlaceholder(WaterVolumeProperties properties, List<Collider> waterVolumeColliders, WaterParameters parameters)
		{
			this.surfacePlane = properties.surfacePlane;
			this.surfaceColliders = properties.surfaceColliders;
			this.volumeColliders = waterVolumeColliders;
			this.liquidType = (GTPlayer.LiquidType)Math.Clamp(properties.liquidType - 1, 0, 1);
			this.waterParams = parameters;
		}

		// Token: 0x040073B3 RID: 29619
		[SerializeField]
		public Transform surfacePlane;

		// Token: 0x040073B4 RID: 29620
		[SerializeField]
		private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		// Token: 0x040073B5 RID: 29621
		[SerializeField]
		public List<Collider> volumeColliders = new List<Collider>();

		// Token: 0x040073B6 RID: 29622
		[SerializeField]
		private GTPlayer.LiquidType liquidType;

		// Token: 0x040073B7 RID: 29623
		[SerializeField]
		private WaterCurrent waterCurrent;

		// Token: 0x040073B8 RID: 29624
		[SerializeField]
		private WaterParameters waterParams;

		// Token: 0x040073B9 RID: 29625
		[SerializeField]
		[Tooltip("The water volume be placed in the scene (not spawned) and not moved for this to be true")]
		public bool isStationary = true;

		// Token: 0x040073BA RID: 29626
		[SerializeField]
		[Tooltip("Check scale of monke entering")]
		public bool isMonkeblock;

		// Token: 0x040073BC RID: 29628
		public const string WaterSplashRPC = "RPC_PlaySplashEffect";

		// Token: 0x040073BD RID: 29629
		public static float[] splashRPCSendTimes = new float[4];

		// Token: 0x040073BE RID: 29630
		private static Dictionary<Collider, WaterVolume> sharedColliderRegistry = new Dictionary<Collider, WaterVolume>(16);

		// Token: 0x040073BF RID: 29631
		private static Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(16);

		// Token: 0x040073C0 RID: 29632
		private static Dictionary<Mesh, Vector3[]> meshVertsDict = new Dictionary<Mesh, Vector3[]>(16);

		// Token: 0x040073C1 RID: 29633
		private int[] sharedMeshTris;

		// Token: 0x040073C2 RID: 29634
		private Vector3[] sharedMeshVerts;

		// Token: 0x040073C7 RID: 29639
		private VRRig playerVRRig;

		// Token: 0x040073C8 RID: 29640
		private float volumeMaxHeight;

		// Token: 0x040073C9 RID: 29641
		private float volumeMinHeight;

		// Token: 0x040073CA RID: 29642
		private bool debugDrawSurfaceCast;

		// Token: 0x040073CB RID: 29643
		private Collider triggerCollider;

		// Token: 0x040073CC RID: 29644
		private List<WaterOverlappingCollider> persistentColliders = new List<WaterOverlappingCollider>(16);

		// Token: 0x040073CD RID: 29645
		private GuidedRefTargetIdSO _guidedRefTargetId;

		// Token: 0x040073CE RID: 29646
		private Object _guidedRefTargetObject;

		// Token: 0x02000F93 RID: 3987
		public struct SurfaceQuery
		{
			// Token: 0x1700097F RID: 2431
			// (get) Token: 0x06006427 RID: 25639 RVA: 0x0020AAE3 File Offset: 0x00208CE3
			public Plane surfacePlane
			{
				get
				{
					return new Plane(this.surfaceNormal, this.surfacePoint);
				}
			}

			// Token: 0x040073CF RID: 29647
			public Vector3 surfacePoint;

			// Token: 0x040073D0 RID: 29648
			public Vector3 surfaceNormal;

			// Token: 0x040073D1 RID: 29649
			public float maxDepth;
		}

		// Token: 0x02000F94 RID: 3988
		// (Invoke) Token: 0x06006429 RID: 25641
		public delegate void WaterVolumeEvent(WaterVolume volume, Collider collider);
	}
}
