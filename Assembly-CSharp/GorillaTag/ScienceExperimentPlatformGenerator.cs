using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02001003 RID: 4099
	public class ScienceExperimentPlatformGenerator : MonoBehaviourPun, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060067B4 RID: 26548 RVA: 0x0021D5CF File Offset: 0x0021B7CF
		private void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
			this.scienceExperimentManager = base.GetComponent<ScienceExperimentManager>();
		}

		// Token: 0x060067B5 RID: 26549 RVA: 0x0021D5E3 File Offset: 0x0021B7E3
		private void OnEnable()
		{
			if (((IGuidedRefReceiverMono)this).GuidedRefsWaitingToResolveCount > 0)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x060067B6 RID: 26550 RVA: 0x001338D3 File Offset: 0x00131AD3
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x060067B7 RID: 26551 RVA: 0x0021D5F5 File Offset: 0x0021B7F5
		// (set) Token: 0x060067B8 RID: 26552 RVA: 0x0021D5FD File Offset: 0x0021B7FD
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x060067B9 RID: 26553 RVA: 0x0021D608 File Offset: 0x0021B808
		void ITickSystemPost.PostTick()
		{
			double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble;
			this.UpdateTrails(currentTime);
			this.RemoveExpiredBubbles(currentTime);
			this.SpawnNewBubbles(currentTime);
			this.UpdateActiveBubbles(currentTime);
		}

		// Token: 0x060067BA RID: 26554 RVA: 0x0021D648 File Offset: 0x0021B848
		private void RemoveExpiredBubbles(double currentTime)
		{
			for (int i = this.activeBubbles.Count - 1; i >= 0; i--)
			{
				if (Mathf.Clamp01((float)(currentTime - this.activeBubbles[i].spawnTime) / this.activeBubbles[i].lifetime) >= 1f)
				{
					this.activeBubbles[i].bubble.Pop();
					this.activeBubbles.RemoveAt(i);
				}
			}
		}

		// Token: 0x060067BB RID: 26555 RVA: 0x0021D6C4 File Offset: 0x0021B8C4
		private void SpawnNewBubbles(double currentTime)
		{
			if (base.photonView.IsMine && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				int num = Mathf.Min((int)(this.rockCountVsLavaProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.bubbleCountMultiplier), this.maxBubbleCount) - this.activeBubbles.Count;
				if (this.activeBubbles.Count < this.maxBubbleCount)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnRockAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
			}
		}

		// Token: 0x060067BC RID: 26556 RVA: 0x0021D754 File Offset: 0x0021B954
		private void UpdateActiveBubbles(double currentTime)
		{
			if (this.liquidSurfacePlane == null)
			{
				return;
			}
			float y = this.liquidSurfacePlane.transform.position.y;
			float num = this.bubblePopWobbleAmplitude * Mathf.Sin(this.bubblePopWobbleFrequency * 0.5f * 3.1415927f * Time.time);
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = this.activeBubbles[i];
				float num2 = Mathf.Clamp01((float)(currentTime - bubbleData.spawnTime) / bubbleData.lifetime);
				float num3 = bubbleData.spawnSize * this.rockSizeVsLifetime.Evaluate(num2) * this.scaleFactor;
				bubbleData.position.y = y;
				bubbleData.bubble.body.gameObject.transform.localScale = Vector3.one * num3;
				bubbleData.bubble.body.MovePosition(bubbleData.position);
				float num4 = (float)((double)bubbleData.lifetime + bubbleData.spawnTime - currentTime);
				if (num4 < this.bubblePopAnticipationTime)
				{
					float num5 = Mathf.Clamp01(1f - num4 / this.bubblePopAnticipationTime);
					bubbleData.bubble.bubbleMesh.transform.localScale = Vector3.one * (1f + num5 * num);
				}
				this.activeBubbles[i] = bubbleData;
			}
		}

		// Token: 0x060067BD RID: 26557 RVA: 0x0021D8BC File Offset: 0x0021BABC
		private void UpdateTrails(double currentTime)
		{
			if (base.photonView.IsMine)
			{
				int num = (int)(this.trailCountVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailCountMultiplier) - this.trailHeads.Count;
				if (num > 0 && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
				{
					for (int i = 0; i < num; i++)
					{
						this.SpawnTrailAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
					}
				}
				else if (num < 0)
				{
					for (int j = 0; j > num; j--)
					{
						this.trailHeads.RemoveAt(0);
					}
				}
				float num2 = this.trailSpawnRateVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailSpawnRateMultiplier;
				float num3 = this.trailBubbleBoundaryRadiusVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.surfaceRadiusSpawnRange.y;
				for (int k = this.trailHeads.Count - 1; k >= 0; k--)
				{
					if ((float)(currentTime - this.trailHeads[k].spawnTime) > num2)
					{
						float num4 = -this.trailMaxTurnAngle;
						float num5 = this.trailMaxTurnAngle;
						float num6 = Vector3.SignedAngle(this.trailHeads[k].direction, this.trailHeads[k].position - this.liquidSurfacePlane.transform.position, Vector3.up);
						float num7 = num3 - Vector3.Distance(this.trailHeads[k].position, this.liquidSurfacePlane.transform.position);
						if (num7 < this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor)
						{
							float num8 = Mathf.InverseLerp(this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor, this.trailEdgeAvoidanceSpawnsMinMax.y * this.trailDistanceBetweenSpawns * this.scaleFactor, num7);
							if (num6 > 0f)
							{
								float num9 = num6 - 90f * num8;
								num5 = Mathf.Min(num5, num9);
								num4 = Mathf.Min(num4, num5 - this.trailMaxTurnAngle);
							}
							else
							{
								float num10 = num6 + 90f * num8;
								num4 = Mathf.Max(num4, num10);
								num5 = Mathf.Max(num5, num4 + this.trailMaxTurnAngle);
							}
						}
						Vector3 vector = Quaternion.AngleAxis(Random.Range(num4, num5), Vector3.up) * this.trailHeads[k].direction;
						Vector3 vector2 = this.trailHeads[k].position + vector * this.trailDistanceBetweenSpawns * this.scaleFactor - this.liquidSurfacePlane.transform.position;
						if (vector2.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
						{
							vector2 = vector2.normalized * this.surfaceRadiusSpawnRange.y;
						}
						Vector2 vector3;
						vector3..ctor(vector2.x, vector2.z);
						float num11 = this.trailBubbleSize;
						float num12 = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
						this.trailHeads.RemoveAt(k);
						base.photonView.RPC("SpawnSodaBubbleRPC", 1, new object[]
						{
							vector3,
							num11,
							num12,
							currentTime
						});
						this.SpawnSodaBubbleLocal(vector3, num11, num12, currentTime, true, vector);
					}
				}
			}
		}

		// Token: 0x060067BE RID: 26558 RVA: 0x0021DC54 File Offset: 0x0021BE54
		private void SpawnRockAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num2 = this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num3 = Random.Range(this.lifetimeRange.x, this.lifetimeRange.y) * num;
				float num4 = Random.Range(this.sizeRange.x, this.sizeRange.y * num2);
				float num5 = this.spawnRadiusMultiplierVsLavaProgress.Evaluate(lavaProgress);
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y) * num5;
				vector = this.GetSpawnPositionWithClearance(vector, num4 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				base.photonView.RPC("SpawnSodaBubbleRPC", 1, new object[]
				{
					vector,
					num4,
					num3,
					currentTime
				});
				this.SpawnSodaBubbleLocal(vector, num4, num3, currentTime, false, default(Vector3));
			}
		}

		// Token: 0x060067BF RID: 26559 RVA: 0x0021DD8C File Offset: 0x0021BF8C
		private void SpawnTrailAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
				float num2 = this.trailBubbleSize;
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y);
				vector = this.GetSpawnPositionWithClearance(vector, num2 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
				Vector3 direction = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.forward;
				base.photonView.RPC("SpawnSodaBubbleRPC", 1, new object[]
				{
					vector,
					num2,
					num,
					currentTime
				});
				this.SpawnSodaBubbleLocal(vector, num2, num, currentTime, true, direction);
			}
		}

		// Token: 0x060067C0 RID: 26560 RVA: 0x0021DE94 File Offset: 0x0021C094
		private void SpawnSodaBubbleLocal(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, bool addAsTrail = false, Vector3 direction = default(Vector3))
		{
			if (this.activeBubbles.Count < this.maxBubbleCount)
			{
				Vector3 position = this.liquidSurfacePlane.transform.position + new Vector3(surfacePosLocal.x, 0f, surfacePosLocal.y);
				ScienceExperimentPlatformGenerator.BubbleData bubbleData = new ScienceExperimentPlatformGenerator.BubbleData
				{
					position = position,
					spawnSize = spawnSize,
					lifetime = lifetime,
					spawnTime = spawnTime,
					isTrail = false
				};
				bubbleData.bubble = ObjectPools.instance.Instantiate(this.spawnedPrefab, bubbleData.position, Quaternion.identity, 0f, true).GetComponent<SodaBubble>();
				if (base.photonView.IsMine && addAsTrail)
				{
					bubbleData.direction = direction;
					bubbleData.isTrail = true;
					this.trailHeads.Add(bubbleData);
				}
				this.activeBubbles.Add(bubbleData);
			}
		}

		// Token: 0x060067C1 RID: 26561 RVA: 0x0021DF7C File Offset: 0x0021C17C
		[PunRPC]
		public void SpawnSodaBubbleRPC(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SpawnSodaBubbleRPC");
			if (info.Sender == PhotonNetwork.MasterClient)
			{
				if (!float.IsFinite(spawnSize) || !float.IsFinite(lifetime) || !double.IsFinite(spawnTime))
				{
					return;
				}
				float num = Mathf.Clamp01(this.scienceExperimentManager.RiseProgressLinear);
				ref surfacePosLocal.ClampThisMagnitudeSafe(this.surfaceRadiusSpawnRange.y);
				spawnSize = Mathf.Clamp(spawnSize, this.sizeRange.x, this.sizeRange.y * this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(num));
				lifetime = Mathf.Clamp(lifetime, this.lifetimeRange.x, this.lifetimeRange.y * this.rockLifetimeMultiplierVsLavaProgress.Evaluate(num));
				double num2 = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble;
				spawnTime = ((Mathf.Abs((float)(spawnTime - num2)) < 10f) ? spawnTime : num2);
				this.SpawnSodaBubbleLocal(surfacePosLocal, spawnSize, lifetime, spawnTime, false, default(Vector3));
			}
		}

		// Token: 0x060067C2 RID: 26562 RVA: 0x0021E07C File Offset: 0x0021C27C
		private Vector2 GetSpawnPositionWithClearance(Vector2 inputPosition, float inputSize, float maxDistance, Vector3 lavaSurfaceOrigin)
		{
			Vector2 vector = inputPosition;
			for (int i = 0; i < this.activeBubbles.Count; i++)
			{
				Vector3 vector2 = this.activeBubbles[i].position - lavaSurfaceOrigin;
				Vector2 vector3;
				vector3..ctor(vector2.x, vector2.z);
				Vector2 vector4 = vector - vector3;
				float num = (inputSize + this.activeBubbles[i].spawnSize * this.scaleFactor) * 0.5f;
				if (vector4.sqrMagnitude < num * num)
				{
					float magnitude = vector4.magnitude;
					if (magnitude > 0.001f)
					{
						Vector2 vector5 = vector4 / magnitude;
						vector += vector5 * (num - magnitude);
						if (vector.sqrMagnitude > maxDistance * maxDistance)
						{
							vector = vector.normalized * maxDistance;
						}
					}
				}
			}
			if (vector.sqrMagnitude > this.surfaceRadiusSpawnRange.y * this.surfaceRadiusSpawnRange.y)
			{
				vector = vector.normalized * this.surfaceRadiusSpawnRange.y;
			}
			return vector;
		}

		// Token: 0x060067C3 RID: 26563 RVA: 0x0021E18F File Offset: 0x0021C38F
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<ScienceExperimentPlatformGenerator>(this, "liquidSurfacePlane", ref this.liquidSurfacePlane_gRef);
			GuidedRefHub.ReceiverFullyRegistered<ScienceExperimentPlatformGenerator>(this);
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x060067C4 RID: 26564 RVA: 0x0021E1A8 File Offset: 0x0021C3A8
		// (set) Token: 0x060067C5 RID: 26565 RVA: 0x0021E1B0 File Offset: 0x0021C3B0
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x060067C6 RID: 26566 RVA: 0x0021E1B9 File Offset: 0x0021C3B9
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<ScienceExperimentPlatformGenerator, Transform>(this, ref this.liquidSurfacePlane, this.liquidSurfacePlane_gRef, target);
		}

		// Token: 0x060067C7 RID: 26567 RVA: 0x0021E1CE File Offset: 0x0021C3CE
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			if (!base.enabled)
			{
				return;
			}
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x060067C8 RID: 26568 RVA: 0x001338D3 File Offset: 0x00131AD3
		void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x060067CA RID: 26570 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060067CB RID: 26571 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x04007674 RID: 30324
		[SerializeField]
		private GameObject spawnedPrefab;

		// Token: 0x04007675 RID: 30325
		[SerializeField]
		private float scaleFactor = 0.03f;

		// Token: 0x04007676 RID: 30326
		[Header("Random Bubbles")]
		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		// Token: 0x04007677 RID: 30327
		[SerializeField]
		private Vector2 lifetimeRange = new Vector2(5f, 10f);

		// Token: 0x04007678 RID: 30328
		[SerializeField]
		private Vector2 sizeRange = new Vector2(0.5f, 2f);

		// Token: 0x04007679 RID: 30329
		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400767A RID: 30330
		[SerializeField]
		[FormerlySerializedAs("rockCountMultiplier")]
		private float bubbleCountMultiplier = 80f;

		// Token: 0x0400767B RID: 30331
		[SerializeField]
		private int maxBubbleCount = 100;

		// Token: 0x0400767C RID: 30332
		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400767D RID: 30333
		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400767E RID: 30334
		[SerializeField]
		private AnimationCurve spawnRadiusMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400767F RID: 30335
		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007680 RID: 30336
		[Header("Bubble Trails")]
		[SerializeField]
		private AnimationCurve trailSpawnRateVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007681 RID: 30337
		[SerializeField]
		private float trailSpawnRateMultiplier = 1f;

		// Token: 0x04007682 RID: 30338
		[SerializeField]
		private AnimationCurve trailBubbleLifetimeVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007683 RID: 30339
		[SerializeField]
		private AnimationCurve trailBubbleBoundaryRadiusVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007684 RID: 30340
		[SerializeField]
		private float trailBubbleLifetimeMultiplier = 6f;

		// Token: 0x04007685 RID: 30341
		[SerializeField]
		private float trailDistanceBetweenSpawns = 3f;

		// Token: 0x04007686 RID: 30342
		[SerializeField]
		private float trailMaxTurnAngle = 55f;

		// Token: 0x04007687 RID: 30343
		[SerializeField]
		private float trailBubbleSize = 1.5f;

		// Token: 0x04007688 RID: 30344
		[SerializeField]
		private AnimationCurve trailCountVsProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007689 RID: 30345
		[SerializeField]
		private float trailCountMultiplier = 12f;

		// Token: 0x0400768A RID: 30346
		[SerializeField]
		private Vector2 trailEdgeAvoidanceSpawnsMinMax = new Vector2(3f, 1f);

		// Token: 0x0400768B RID: 30347
		[Header("Feedback Effects")]
		[SerializeField]
		private float bubblePopAnticipationTime = 2f;

		// Token: 0x0400768C RID: 30348
		[SerializeField]
		private float bubblePopWobbleFrequency = 25f;

		// Token: 0x0400768D RID: 30349
		[SerializeField]
		private float bubblePopWobbleAmplitude = 0.01f;

		// Token: 0x0400768E RID: 30350
		[SerializeField]
		private Transform liquidSurfacePlane;

		// Token: 0x0400768F RID: 30351
		[SerializeField]
		private GuidedRefReceiverFieldInfo liquidSurfacePlane_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04007690 RID: 30352
		private List<ScienceExperimentPlatformGenerator.BubbleData> activeBubbles = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x04007691 RID: 30353
		private List<ScienceExperimentPlatformGenerator.BubbleData> trailHeads = new List<ScienceExperimentPlatformGenerator.BubbleData>();

		// Token: 0x04007692 RID: 30354
		private List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug> bubbleSpawnDebug = new List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug>();

		// Token: 0x04007693 RID: 30355
		private ScienceExperimentManager scienceExperimentManager;

		// Token: 0x02001004 RID: 4100
		private struct BubbleData
		{
			// Token: 0x04007696 RID: 30358
			public Vector3 position;

			// Token: 0x04007697 RID: 30359
			public Vector3 direction;

			// Token: 0x04007698 RID: 30360
			public float spawnSize;

			// Token: 0x04007699 RID: 30361
			public float lifetime;

			// Token: 0x0400769A RID: 30362
			public double spawnTime;

			// Token: 0x0400769B RID: 30363
			public bool isTrail;

			// Token: 0x0400769C RID: 30364
			public SodaBubble bubble;
		}

		// Token: 0x02001005 RID: 4101
		private struct BubbleSpawnDebug
		{
			// Token: 0x0400769D RID: 30365
			public Vector3 initialPosition;

			// Token: 0x0400769E RID: 30366
			public Vector3 initialDirection;

			// Token: 0x0400769F RID: 30367
			public Vector3 spawnPosition;

			// Token: 0x040076A0 RID: 30368
			public float minAngle;

			// Token: 0x040076A1 RID: 30369
			public float maxAngle;

			// Token: 0x040076A2 RID: 30370
			public float edgeCorrectionAngle;

			// Token: 0x040076A3 RID: 30371
			public double spawnTime;
		}
	}
}
