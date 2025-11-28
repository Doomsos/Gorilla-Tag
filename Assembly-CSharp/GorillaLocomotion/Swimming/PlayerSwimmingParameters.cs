using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F88 RID: 3976
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSwimmingParameters", order = 1)]
	public class PlayerSwimmingParameters : ScriptableObject
	{
		// Token: 0x0400731D RID: 29469
		[Header("Base Settings")]
		public float floatingWaterLevelBelowHead = 0.6f;

		// Token: 0x0400731E RID: 29470
		public float buoyancyFadeDist = 0.3f;

		// Token: 0x0400731F RID: 29471
		public bool extendBouyancyFromSpeed;

		// Token: 0x04007320 RID: 29472
		public float buoyancyExtensionDecayHalflife = 0.2f;

		// Token: 0x04007321 RID: 29473
		public float baseUnderWaterDampingHalfLife = 0.25f;

		// Token: 0x04007322 RID: 29474
		public float swimUnderWaterDampingHalfLife = 1.1f;

		// Token: 0x04007323 RID: 29475
		public AnimationCurve speedToBouyancyExtension = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007324 RID: 29476
		public Vector2 speedToBouyancyExtensionMinMax = Vector2.zero;

		// Token: 0x04007325 RID: 29477
		public float swimmingVelocityOutOfWaterDrainRate = 3f;

		// Token: 0x04007326 RID: 29478
		[Range(0f, 1f)]
		public float underwaterJumpsAsSwimVelocityFactor = 1f;

		// Token: 0x04007327 RID: 29479
		[Range(0f, 1f)]
		public float swimmingHapticsStrength = 0.5f;

		// Token: 0x04007328 RID: 29480
		[Header("Surface Jumping")]
		public bool allowWaterSurfaceJumps;

		// Token: 0x04007329 RID: 29481
		public float waterSurfaceJumpHandSpeedThreshold = 1f;

		// Token: 0x0400732A RID: 29482
		public float waterSurfaceJumpAmount;

		// Token: 0x0400732B RID: 29483
		public float waterSurfaceJumpMaxSpeed = 1f;

		// Token: 0x0400732C RID: 29484
		public AnimationCurve waterSurfaceJumpPalmFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400732D RID: 29485
		public AnimationCurve waterSurfaceJumpHandVelocityFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400732E RID: 29486
		[Header("Diving")]
		public bool applyDiveSteering;

		// Token: 0x0400732F RID: 29487
		public bool applyDiveDampingMultiplier;

		// Token: 0x04007330 RID: 29488
		public float diveDampingMultiplier = 1f;

		// Token: 0x04007331 RID: 29489
		[Tooltip("In degrees")]
		public float maxDiveSteerAnglePerStep = 1f;

		// Token: 0x04007332 RID: 29490
		public float diveVelocityAveragingWindow = 0.1f;

		// Token: 0x04007333 RID: 29491
		public bool applyDiveSwimVelocityConversion;

		// Token: 0x04007334 RID: 29492
		[Tooltip("In meters per second")]
		public float diveSwimVelocityConversionRate = 3f;

		// Token: 0x04007335 RID: 29493
		public float diveMaxSwimVelocityConversion = 3f;

		// Token: 0x04007336 RID: 29494
		public bool reduceDiveSteeringBelowVelocityPlane;

		// Token: 0x04007337 RID: 29495
		public float reduceDiveSteeringBelowPlaneFadeStartDist = 0.4f;

		// Token: 0x04007338 RID: 29496
		public float reduceDiveSteeringBelowPlaneFadeEndDist = 0.55f;

		// Token: 0x04007339 RID: 29497
		public AnimationCurve palmFacingToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400733A RID: 29498
		public Vector2 palmFacingToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400733B RID: 29499
		public AnimationCurve swimSpeedToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400733C RID: 29500
		public Vector2 swimSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400733D RID: 29501
		public AnimationCurve swimSpeedToMaxRedirectAngle = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400733E RID: 29502
		public Vector2 swimSpeedToMaxRedirectAngleMinMax = Vector2.zero;

		// Token: 0x0400733F RID: 29503
		public AnimationCurve handSpeedToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04007340 RID: 29504
		public Vector2 handSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04007341 RID: 29505
		public AnimationCurve handAccelToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x04007342 RID: 29506
		public Vector2 handAccelToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04007343 RID: 29507
		public AnimationCurve nonDiveDampingHapticsAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007344 RID: 29508
		public Vector2 nonDiveDampingHapticsAmountMinMax = Vector2.zero;
	}
}
