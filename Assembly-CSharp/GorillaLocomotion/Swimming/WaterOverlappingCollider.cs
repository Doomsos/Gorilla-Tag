using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F8F RID: 3983
	public struct WaterOverlappingCollider
	{
		// Token: 0x060063F8 RID: 25592 RVA: 0x00208E20 File Offset: 0x00207020
		public void PlayRippleEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float defaultRippleScale, float currentTime, WaterVolume volume)
		{
			this.lastRipplePosition = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			this.lastBoundingRadius = this.GetBoundingRadiusOnSurface(surfaceNormal);
			this.lastRippleScale = defaultRippleScale * this.lastBoundingRadius * 2f * this.scaleMultiplier;
			this.lastRippleTime = currentTime;
			ObjectPools.instance.Instantiate(rippleEffectPrefab, this.lastRipplePosition, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), this.lastRippleScale, true).GetComponent<WaterRippleEffect>().PlayEffect(volume);
		}

		// Token: 0x060063F9 RID: 25593 RVA: 0x00208EBC File Offset: 0x002070BC
		public void PlaySplashEffect(GameObject splashEffectPrefab, Vector3 splashPosition, float splashScale, bool bigSplash, bool enteringWater, WaterVolume volume)
		{
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right);
			ObjectPools.instance.Instantiate(splashEffectPrefab, splashPosition, quaternion, splashScale * this.scaleMultiplier, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, this.scaleMultiplier, volume);
			if (this.photonViewForRPC != null)
			{
				float time = Time.time;
				int num = -1;
				float num2 = time + 10f;
				for (int i = 0; i < WaterVolume.splashRPCSendTimes.Length; i++)
				{
					if (WaterVolume.splashRPCSendTimes[i] < num2)
					{
						num2 = WaterVolume.splashRPCSendTimes[i];
						num = i;
					}
				}
				if (time - 0.5f > num2)
				{
					WaterVolume.splashRPCSendTimes[num] = time;
					this.photonViewForRPC.SendRPC("RPC_PlaySplashEffect", 1, new object[]
					{
						splashPosition,
						quaternion,
						splashScale * this.scaleMultiplier,
						this.lastBoundingRadius,
						bigSplash,
						enteringWater
					});
				}
			}
		}

		// Token: 0x060063FA RID: 25594 RVA: 0x00208FDC File Offset: 0x002071DC
		public void PlayDripEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float dripScale)
		{
			Vector3 closestPositionOnSurface = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			float num = this.overrideBoundingRadius ? this.boundingRadiusOverride : this.lastBoundingRadius;
			Vector3 vector = Vector3.ProjectOnPlane(Random.onUnitSphere * num * 0.5f, surfaceNormal);
			ObjectPools.instance.Instantiate(rippleEffectPrefab, closestPositionOnSurface + vector, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), dripScale * this.scaleMultiplier, true);
		}

		// Token: 0x060063FB RID: 25595 RVA: 0x0020906B File Offset: 0x0020726B
		public Vector3 GetClosestPositionOnSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
		{
			return Vector3.ProjectOnPlane(this.collider.transform.position - surfacePoint, surfaceNormal) + surfacePoint;
		}

		// Token: 0x060063FC RID: 25596 RVA: 0x00209090 File Offset: 0x00207290
		private float GetBoundingRadiusOnSurface(Vector3 surfaceNormal)
		{
			if (this.overrideBoundingRadius)
			{
				this.lastBoundingRadius = this.boundingRadiusOverride;
				return this.boundingRadiusOverride;
			}
			Vector3 extents = this.collider.bounds.extents;
			Vector3 vector = Vector3.ProjectOnPlane(this.collider.transform.right * extents.x, surfaceNormal);
			Vector3 vector2 = Vector3.ProjectOnPlane(this.collider.transform.up * extents.y, surfaceNormal);
			Vector3 vector3 = Vector3.ProjectOnPlane(this.collider.transform.forward * extents.z, surfaceNormal);
			float sqrMagnitude = vector.sqrMagnitude;
			float sqrMagnitude2 = vector2.sqrMagnitude;
			float sqrMagnitude3 = vector3.sqrMagnitude;
			if (sqrMagnitude >= sqrMagnitude2 && sqrMagnitude >= sqrMagnitude3)
			{
				return vector.magnitude;
			}
			if (sqrMagnitude2 >= sqrMagnitude && sqrMagnitude2 >= sqrMagnitude3)
			{
				return vector2.magnitude;
			}
			return vector3.magnitude;
		}

		// Token: 0x04007385 RID: 29573
		public bool playBigSplash;

		// Token: 0x04007386 RID: 29574
		public bool playDripEffect;

		// Token: 0x04007387 RID: 29575
		public bool overrideBoundingRadius;

		// Token: 0x04007388 RID: 29576
		public float boundingRadiusOverride;

		// Token: 0x04007389 RID: 29577
		public float scaleMultiplier;

		// Token: 0x0400738A RID: 29578
		public Collider collider;

		// Token: 0x0400738B RID: 29579
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x0400738C RID: 29580
		public WaterVolume.SurfaceQuery lastSurfaceQuery;

		// Token: 0x0400738D RID: 29581
		public NetworkView photonViewForRPC;

		// Token: 0x0400738E RID: 29582
		public bool surfaceDetected;

		// Token: 0x0400738F RID: 29583
		public bool inWater;

		// Token: 0x04007390 RID: 29584
		public bool inVolume;

		// Token: 0x04007391 RID: 29585
		public float lastBoundingRadius;

		// Token: 0x04007392 RID: 29586
		public Vector3 lastRipplePosition;

		// Token: 0x04007393 RID: 29587
		public float lastRippleScale;

		// Token: 0x04007394 RID: 29588
		public float lastRippleTime;

		// Token: 0x04007395 RID: 29589
		public float lastInWaterTime;

		// Token: 0x04007396 RID: 29590
		public float nextDripTime;
	}
}
