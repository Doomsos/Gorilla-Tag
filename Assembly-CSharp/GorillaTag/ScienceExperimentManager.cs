using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CjLib;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTag
{
	// Token: 0x02000FF9 RID: 4089
	[NetworkBehaviourWeaved(76)]
	public class ScienceExperimentManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x0600675D RID: 26461 RVA: 0x0021A270 File Offset: 0x00218470
		private bool RefreshWaterAvailable
		{
			get
			{
				return this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Erupting || (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising && this.riseProgress < this.lavaProgressToDisableRefreshWater) || (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining && this.riseProgress < this.lavaProgressToEnableRefreshWater);
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x0600675E RID: 26462 RVA: 0x0021A2D4 File Offset: 0x002184D4
		public ScienceExperimentManager.RisingLiquidState GameState
		{
			get
			{
				return this.reliableState.state;
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x0600675F RID: 26463 RVA: 0x0021A2E1 File Offset: 0x002184E1
		public float RiseProgress
		{
			get
			{
				return this.riseProgress;
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06006760 RID: 26464 RVA: 0x0021A2E9 File Offset: 0x002184E9
		public float RiseProgressLinear
		{
			get
			{
				return this.riseProgressLinear;
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06006761 RID: 26465 RVA: 0x0021A2F4 File Offset: 0x002184F4
		private int PlayerCount
		{
			get
			{
				int result = 1;
				GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
				if (gorillaGameManager != null && gorillaGameManager.currentNetPlayerArray != null)
				{
					result = gorillaGameManager.currentNetPlayerArray.Length;
				}
				return result;
			}
		}

		// Token: 0x06006762 RID: 26466 RVA: 0x0021A324 File Offset: 0x00218524
		protected override void Awake()
		{
			base.Awake();
			if (ScienceExperimentManager.instance == null)
			{
				ScienceExperimentManager.instance = this;
				NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
				this.riseTimeLookup = new float[]
				{
					this.riseTimeFast,
					this.riseTimeMedium,
					this.riseTimeSlow,
					this.riseTimeExtraSlow
				};
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
				GorillaGameManager.OnTouch += this.OnPlayerTagged;
				RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
				RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
				this.rotatingRings = new ScienceExperimentManager.RotatingRingState[this.ringParent.childCount];
				for (int i = 0; i < this.rotatingRings.Length; i++)
				{
					this.rotatingRings[i].ringTransform = this.ringParent.GetChild(i);
					this.rotatingRings[i].initialAngle = 0f;
					this.rotatingRings[i].resultingAngle = 0f;
				}
				this.gameAreaTriggerNotifier.CompositeTriggerEnter += this.OnColliderEnteredVolume;
				this.gameAreaTriggerNotifier.CompositeTriggerExit += this.OnColliderExitedVolume;
				this.liquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
				this.liquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				this.entryLiquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
				this.entryLiquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				if (this.bottleLiquidVolume != null)
				{
					this.bottleLiquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
					this.bottleLiquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				}
				if (this.refreshWaterVolume != null)
				{
					this.refreshWaterVolume.ColliderEnteredWater += this.OnColliderEnteredRefreshWater;
					this.refreshWaterVolume.ColliderExitedWater += this.OnColliderExitedRefreshWater;
				}
				if (this.sodaWaterProjectileTriggerNotifier != null)
				{
					this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter += this.OnProjectileEnteredSodaWater;
				}
				float num = Vector3.Distance(this.drainBlockerClosedPosition.position, this.drainBlockerOpenPosition.position);
				this.drainBlockerSlideSpeed = num / this.drainBlockerSlideTime;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06006763 RID: 26467 RVA: 0x0000B3F9 File Offset: 0x000095F9
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006764 RID: 26468 RVA: 0x0000B40D File Offset: 0x0000960D
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006765 RID: 26469 RVA: 0x0021A5C4 File Offset: 0x002187C4
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			GorillaGameManager.OnTouch -= this.OnPlayerTagged;
			if (this.gameAreaTriggerNotifier != null)
			{
				this.gameAreaTriggerNotifier.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
				this.gameAreaTriggerNotifier.CompositeTriggerExit -= this.OnColliderExitedVolume;
			}
			if (this.liquidVolume != null)
			{
				this.liquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.liquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.entryLiquidVolume != null)
			{
				this.entryLiquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.entryLiquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.bottleLiquidVolume != null)
			{
				this.bottleLiquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.bottleLiquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.refreshWaterVolume != null)
			{
				this.refreshWaterVolume.ColliderEnteredWater -= this.OnColliderEnteredRefreshWater;
				this.refreshWaterVolume.ColliderExitedWater -= this.OnColliderExitedRefreshWater;
			}
			if (this.sodaWaterProjectileTriggerNotifier != null)
			{
				this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter -= this.OnProjectileEnteredSodaWater;
			}
		}

		// Token: 0x06006766 RID: 26470 RVA: 0x0021A73C File Offset: 0x0021893C
		public void InitElements(ScienceExperimentSceneElements elements)
		{
			this.elements = elements;
			this.fizzParticleEmission = elements.sodaFizzParticles.emission;
			elements.sodaFizzParticles.gameObject.SetActive(false);
			elements.sodaEruptionParticles.gameObject.SetActive(false);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		}

		// Token: 0x06006767 RID: 26471 RVA: 0x0021A79E File Offset: 0x0021899E
		public void DeInitElements()
		{
			this.elements = null;
		}

		// Token: 0x06006768 RID: 26472 RVA: 0x0021A7A8 File Offset: 0x002189A8
		public Transform GetElement(ScienceExperimentElementID elementID)
		{
			switch (elementID)
			{
			case ScienceExperimentElementID.Platform1:
				return this.rotatingRings[0].ringTransform;
			case ScienceExperimentElementID.Platform2:
				return this.rotatingRings[1].ringTransform;
			case ScienceExperimentElementID.Platform3:
				return this.rotatingRings[2].ringTransform;
			case ScienceExperimentElementID.Platform4:
				return this.rotatingRings[3].ringTransform;
			case ScienceExperimentElementID.Platform5:
				return this.rotatingRings[4].ringTransform;
			case ScienceExperimentElementID.LiquidMesh:
				return this.liquidMeshTransform;
			case ScienceExperimentElementID.EntryChamberLiquidMesh:
				return this.entryWayLiquidMeshTransform;
			case ScienceExperimentElementID.EntryChamberBridgeQuad:
				return this.entryWayBridgeQuadTransform;
			case ScienceExperimentElementID.DrainBlocker:
				return this.drainBlocker;
			default:
				Debug.LogError(string.Format("Unhandled ScienceExperiment element ID! {0}", elementID));
				return null;
			}
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06006769 RID: 26473 RVA: 0x0021A86D File Offset: 0x00218A6D
		// (set) Token: 0x0600676A RID: 26474 RVA: 0x0021A875 File Offset: 0x00218A75
		bool ITickSystemTick.TickRunning { get; set; }

		// Token: 0x0600676B RID: 26475 RVA: 0x0021A880 File Offset: 0x00218A80
		void ITickSystemTick.Tick()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.unscaledTimeAsDouble);
			this.lastInfrequentUpdateTime = ((this.lastInfrequentUpdateTime > this.currentTime) ? this.currentTime : this.lastInfrequentUpdateTime);
			if (this.currentTime > this.lastInfrequentUpdateTime + (double)this.infrequentUpdatePeriod)
			{
				this.InfrequentUpdate();
				this.lastInfrequentUpdateTime = (double)((float)this.currentTime);
			}
			if (base.IsMine)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagRiseProgressOffset = Mathf.MoveTowards(this.localLagRiseProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLiquid(this.riseProgress + this.localLagRiseProgressOffset);
			this.UpdateRotatingRings(this.ringRotationProgress);
			this.UpdateRefreshWater();
			this.UpdateDrainBlocker(this.currentTime);
			this.DisableObjectsInContactWithLava(this.liquidMeshTransform.localScale.z);
			this.UpdateEffects();
			if (this.debugDrawPlayerGameState)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					NetPlayer netPlayer = null;
					if (NetworkSystem.Instance.InRoom)
					{
						netPlayer = NetworkSystem.Instance.GetPlayer(this.inGamePlayerStates[i].playerId);
					}
					else if (this.inGamePlayerStates[i].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
					{
						netPlayer = NetworkSystem.Instance.LocalPlayer;
					}
					RigContainer rigContainer;
					if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						float num = 0.03f;
						DebugUtil.DrawSphere(rigContainer.Rig.transform.position + Vector3.up * 0.5f * num, 0.16f * num, 12, 12, this.inGamePlayerStates[i].touchedLiquid ? Color.red : Color.green, true, DebugUtil.Style.SolidColor);
					}
				}
			}
		}

		// Token: 0x0600676C RID: 26476 RVA: 0x0021AAA0 File Offset: 0x00218CA0
		private void InfrequentUpdate()
		{
			this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
			if (base.IsMine)
			{
				for (int i = this.inGamePlayerCount - 1; i >= 0; i--)
				{
					int playerId = this.inGamePlayerStates[i].playerId;
					bool flag = false;
					for (int j = 0; j < this.allPlayersInRoom.Length; j++)
					{
						if (this.allPlayersInRoom[j].ActorNumber == playerId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (i < this.inGamePlayerCount - 1)
						{
							this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						}
						this.inGamePlayerStates[this.inGamePlayerCount - 1] = default(ScienceExperimentManager.PlayerGameState);
						this.inGamePlayerCount--;
					}
				}
			}
			if (this.optPlayersOutOfRoomGameMode)
			{
				for (int k = 0; k < this.allPlayersInRoom.Length; k++)
				{
					bool flag2 = false;
					for (int l = 0; l < this.inGamePlayerCount; l++)
					{
						if (this.allPlayersInRoom[k].ActorNumber == this.inGamePlayerStates[l].playerId)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						GameMode.OptOut(this.allPlayersInRoom[k]);
					}
					else
					{
						GameMode.OptIn(this.allPlayersInRoom[k]);
					}
				}
			}
		}

		// Token: 0x0600676D RID: 26477 RVA: 0x0021ABEC File Offset: 0x00218DEC
		private bool PlayerInGame(Player player)
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600676E RID: 26478 RVA: 0x0021AC28 File Offset: 0x00218E28
		private void UpdateReliableState(double currentTime, ref ScienceExperimentManager.SyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
			{
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() > 0 && syncData.activationProgress > 1.0)
				{
					syncData.state = ScienceExperimentManager.RisingLiquidState.Erupting;
					syncData.stateStartTime = currentTime;
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.activationProgress = 1.0;
					return;
				}
				float num = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num);
				return;
			}
			case ScienceExperimentManager.RisingLiquidState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.fullyDrainedWaitTime)
				{
					this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Rising;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() <= 0)
				{
					this.UpdateWinner();
					syncData.stateStartLiquidProgressLinear = Mathf.Clamp01((float)((currentTime - syncData.stateStartTime) / (double)this.riseTime));
					syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.stateStartLiquidProgressLinear = 1f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Full;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Full:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() <= 0 || currentTime > syncData.stateStartTime + (double)this.maxFullTime)
				{
					this.UpdateWinner();
					syncData.stateStartLiquidProgressLinear = 1f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
				if (currentTime > syncData.stateStartTime + (double)this.preDrainWaitTime)
				{
					syncData.state = ScienceExperimentManager.RisingLiquidState.Draining;
					syncData.stateStartTime = currentTime;
					syncData.activationProgress = 0.0;
					for (int i = 0; i < this.rotatingRings.Length; i++)
					{
						float num2 = Mathf.Repeat(this.rotatingRings[i].resultingAngle, 360f);
						float num3 = Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
						float num4 = (Random.Range(0f, 1f) > 0.5f) ? 1f : -1f;
						this.rotatingRings[i].initialAngle = num2;
						this.rotatingRings[i].resultingAngle = num2 + num4 * num3;
					}
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Draining:
			{
				double num5 = (1.0 - (double)syncData.stateStartLiquidProgressLinear) * (double)this.drainTime;
				if (currentTime + num5 > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Drained;
					syncData.stateStartTime = currentTime;
					syncData.activationProgress = 0.0;
				}
				break;
			}
			}
		}

		// Token: 0x0600676F RID: 26479 RVA: 0x0021AF04 File Offset: 0x00219104
		private void UpdateLocalState(double currentTime, ScienceExperimentManager.SyncData syncData)
		{
			switch (syncData.state)
			{
			default:
				this.riseProgressLinear = 0f;
				this.riseProgress = 0f;
				if (!this.debugRandomizingRings)
				{
					this.ringRotationProgress = 1f;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
			{
				double num = (currentTime - syncData.stateStartTime) / (double)this.riseTime;
				this.riseProgressLinear = Mathf.Clamp01((float)num);
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = 1f;
				return;
			}
			case ScienceExperimentManager.RisingLiquidState.Full:
				this.riseProgressLinear = 1f;
				this.riseProgress = 1f;
				this.ringRotationProgress = 1f;
				return;
			case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
				this.riseProgressLinear = syncData.stateStartLiquidProgressLinear;
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = 1f;
				return;
			case ScienceExperimentManager.RisingLiquidState.Draining:
			{
				double num2 = (1.0 - (double)syncData.stateStartLiquidProgressLinear) * (double)this.drainTime;
				double num3 = (currentTime + num2 - syncData.stateStartTime) / (double)this.drainTime;
				this.riseProgressLinear = Mathf.Clamp01((float)(1.0 - num3));
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = (float)(currentTime - syncData.stateStartTime) / (this.drainTime * syncData.stateStartLiquidProgressLinear);
				break;
			}
			}
		}

		// Token: 0x06006770 RID: 26480 RVA: 0x0021B070 File Offset: 0x00219270
		private void UpdateLiquid(float fillProgress)
		{
			float num = Mathf.Lerp(this.minScale, this.maxScale, fillProgress);
			this.liquidMeshTransform.localScale = new Vector3(1f, 1f, num);
			bool active = this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Full || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.PreDrainDelay || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining;
			this.liquidMeshTransform.gameObject.SetActive(active);
			if (this.entryWayLiquidMeshTransform != null)
			{
				float num2 = 0f;
				float num3;
				float num4;
				if (num < this.entryLiquidScaleSyncOpeningBottom.y)
				{
					num3 = this.entryLiquidScaleSyncOpeningBottom.x;
					num4 = this.entryBridgeQuadMinMaxZHeight.x;
				}
				else if (num < this.entryLiquidScaleSyncOpeningTop.y)
				{
					float num5 = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningBottom.y, this.entryLiquidScaleSyncOpeningTop.y, num);
					num3 = Mathf.Lerp(this.entryLiquidScaleSyncOpeningBottom.x, this.entryLiquidScaleSyncOpeningTop.x, num5);
					num4 = Mathf.Lerp(this.entryBridgeQuadMinMaxZHeight.x, this.entryBridgeQuadMinMaxZHeight.y, num5);
					num2 = this.entryBridgeQuadMaxScaleY * Mathf.Sin(num5 * 3.1415927f);
				}
				else
				{
					float num6 = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningTop.y, 0.6f * this.maxScale, num);
					num3 = Mathf.Lerp(this.entryLiquidScaleSyncOpeningTop.x, this.entryLiquidMaxScale, num6);
					num4 = this.entryBridgeQuadMinMaxZHeight.y;
				}
				this.entryWayLiquidMeshTransform.localScale = new Vector3(this.entryWayLiquidMeshTransform.localScale.x, this.entryWayLiquidMeshTransform.localScale.y, num3);
				this.entryWayBridgeQuadTransform.localScale = new Vector3(this.entryWayBridgeQuadTransform.localScale.x, num2, this.entryWayBridgeQuadTransform.localScale.z);
				this.entryWayBridgeQuadTransform.localPosition = new Vector3(this.entryWayBridgeQuadTransform.localPosition.x, this.entryWayBridgeQuadTransform.localPosition.y, num4);
			}
		}

		// Token: 0x06006771 RID: 26481 RVA: 0x0021B294 File Offset: 0x00219494
		private void UpdateRotatingRings(float rotationProgress)
		{
			for (int i = 0; i < this.rotatingRings.Length; i++)
			{
				float num = Mathf.Lerp(this.rotatingRings[i].initialAngle, this.rotatingRings[i].resultingAngle, rotationProgress);
				this.rotatingRings[i].ringTransform.rotation = Quaternion.AngleAxis(num, Vector3.up);
			}
		}

		// Token: 0x06006772 RID: 26482 RVA: 0x0021B300 File Offset: 0x00219500
		private void UpdateDrainBlocker(double currentTime)
		{
			if (this.reliableState.state != ScienceExperimentManager.RisingLiquidState.Draining)
			{
				this.drainBlocker.position = this.drainBlockerClosedPosition.position;
				return;
			}
			float num = (float)(currentTime - this.reliableState.stateStartTime);
			float num2 = (1f - this.reliableState.stateStartLiquidProgressLinear) * this.drainTime;
			if (this.drainTime - (num + num2) < this.drainBlockerSlideTime)
			{
				this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerClosedPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
				return;
			}
			this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerOpenPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
		}

		// Token: 0x06006773 RID: 26483 RVA: 0x0021B3D4 File Offset: 0x002195D4
		private void UpdateEffects()
		{
			switch (this.reliableState.state)
			{
			case ScienceExperimentManager.RisingLiquidState.Drained:
				this.hasPlayedEruptionEffects = false;
				this.hasPlayedDrainEffects = false;
				this.eruptionAudioSource.GTStop();
				this.drainAudioSource.GTStop();
				this.rotatingRingsAudioSource.GTStop();
				if (this.elements != null)
				{
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.elements.sodaFizzParticles.gameObject.SetActive(true);
					if (this.reliableState.activationProgress > 0.0010000000474974513)
					{
						this.fizzParticleEmission.rateOverTimeMultiplier = Mathf.Lerp(this.sodaFizzParticleEmissionMinMax.x, this.sodaFizzParticleEmissionMinMax.y, (float)this.reliableState.activationProgress);
						return;
					}
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Erupting:
				if (!this.hasPlayedEruptionEffects)
				{
					this.eruptionAudioSource.loop = true;
					this.eruptionAudioSource.GTPlay();
					this.hasPlayedEruptionEffects = true;
					if (this.elements != null)
					{
						this.elements.sodaEruptionParticles.gameObject.SetActive(true);
						this.fizzParticleEmission.rateOverTimeMultiplier = this.sodaFizzParticleEmissionMinMax.y;
						return;
					}
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
				if (this.elements != null)
				{
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
					return;
				}
				break;
			default:
				if (this.elements != null)
				{
					this.elements.sodaFizzParticles.gameObject.SetActive(false);
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
				}
				this.hasPlayedEruptionEffects = false;
				this.hasPlayedDrainEffects = false;
				this.eruptionAudioSource.GTStop();
				this.drainAudioSource.GTStop();
				this.rotatingRingsAudioSource.GTStop();
				return;
			case ScienceExperimentManager.RisingLiquidState.Draining:
				this.hasPlayedEruptionEffects = false;
				this.eruptionAudioSource.GTStop();
				if (this.elements != null)
				{
					this.elements.sodaFizzParticles.gameObject.SetActive(false);
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
				}
				if (!this.hasPlayedDrainEffects)
				{
					this.drainAudioSource.loop = true;
					this.drainAudioSource.GTPlay();
					this.rotatingRingsAudioSource.loop = true;
					this.rotatingRingsAudioSource.GTPlay();
					this.hasPlayedDrainEffects = true;
				}
				break;
			}
		}

		// Token: 0x06006774 RID: 26484 RVA: 0x0021B670 File Offset: 0x00219870
		private void DisableObjectsInContactWithLava(float lavaScale)
		{
			if (this.elements == null)
			{
				return;
			}
			Plane plane;
			plane..ctor(this.liquidSurfacePlane.up, this.liquidSurfacePlane.position);
			if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				for (int i = 0; i < this.elements.disableByLiquidList.Count; i++)
				{
					if (!plane.GetSide(this.elements.disableByLiquidList[i].target.position + this.elements.disableByLiquidList[i].heightOffset * Vector3.up))
					{
						this.elements.disableByLiquidList[i].target.gameObject.SetActive(false);
					}
				}
				return;
			}
			if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining)
			{
				for (int j = 0; j < this.elements.disableByLiquidList.Count; j++)
				{
					if (plane.GetSide(this.elements.disableByLiquidList[j].target.position + this.elements.disableByLiquidList[j].heightOffset * Vector3.up))
					{
						this.elements.disableByLiquidList[j].target.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06006775 RID: 26485 RVA: 0x0021B7DC File Offset: 0x002199DC
		private void UpdateWinner()
		{
			float num = -1f;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLiquid)
				{
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
					break;
				}
				if (this.inGamePlayerStates[i].touchedLiquidAtProgress > num)
				{
					num = this.inGamePlayerStates[i].touchedLiquidAtProgress;
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
				}
			}
			this.RefreshWinnerName();
		}

		// Token: 0x06006776 RID: 26486 RVA: 0x0021B870 File Offset: 0x00219A70
		private void RefreshWinnerName()
		{
			NetPlayer playerFromId = this.GetPlayerFromId(this.lastWinnerId);
			if (playerFromId != null)
			{
				this.lastWinnerName = playerFromId.NickName;
				return;
			}
			this.lastWinnerName = "None";
		}

		// Token: 0x06006777 RID: 26487 RVA: 0x0021B8A5 File Offset: 0x00219AA5
		private NetPlayer GetPlayerFromId(int id)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				return NetworkSystem.Instance.GetPlayer(id);
			}
			if (id == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return NetworkSystem.Instance.LocalPlayer;
			}
			return null;
		}

		// Token: 0x06006778 RID: 26488 RVA: 0x0021B8E0 File Offset: 0x00219AE0
		private void UpdateRefreshWater()
		{
			if (this.refreshWaterVolume != null)
			{
				if (this.RefreshWaterAvailable && !this.refreshWaterVolume.gameObject.activeSelf)
				{
					this.refreshWaterVolume.gameObject.SetActive(true);
					return;
				}
				if (!this.RefreshWaterAvailable && this.refreshWaterVolume.gameObject.activeSelf)
				{
					this.refreshWaterVolume.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06006779 RID: 26489 RVA: 0x0021B954 File Offset: 0x00219B54
		private void ResetGame()
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
				playerGameState.touchedLiquid = false;
				playerGameState.touchedLiquidAtProgress = -1f;
				this.inGamePlayerStates[i] = playerGameState;
			}
		}

		// Token: 0x0600677A RID: 26490 RVA: 0x0021B9A0 File Offset: 0x00219BA0
		public void RestartGame()
		{
			if (base.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
				this.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : ((double)Time.time));
				this.reliableState.stateStartLiquidProgressLinear = 0f;
				this.reliableState.activationProgress = 1.0;
				this.ResetGame();
			}
		}

		// Token: 0x0600677B RID: 26491 RVA: 0x0021BA28 File Offset: 0x00219C28
		public void DebugErupt()
		{
			if (base.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
				this.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : ((double)Time.time));
				this.reliableState.stateStartLiquidProgressLinear = 0f;
				this.reliableState.activationProgress = 1.0;
			}
		}

		// Token: 0x0600677C RID: 26492 RVA: 0x0021BAAC File Offset: 0x00219CAC
		public void RandomizeRings()
		{
			for (int i = 0; i < this.rotatingRings.Length; i++)
			{
				float num = Mathf.Repeat(this.rotatingRings[i].resultingAngle, 360f);
				float num2 = Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
				float num3 = (Random.Range(0f, 1f) > 0.5f) ? 1f : -1f;
				this.rotatingRings[i].initialAngle = num;
				float num4 = num + num3 * num2;
				if (this.rotatingRingQuantizeAngles)
				{
					num4 = Mathf.Round(num4 / this.rotatingRingAngleSnapDegrees) * this.rotatingRingAngleSnapDegrees;
				}
				this.rotatingRings[i].resultingAngle = num4;
			}
			if (this.rotateRingsCoroutine != null)
			{
				base.StopCoroutine(this.rotateRingsCoroutine);
			}
			this.rotateRingsCoroutine = base.StartCoroutine(this.RotateRingsCoroutine());
		}

		// Token: 0x0600677D RID: 26493 RVA: 0x0021BB9E File Offset: 0x00219D9E
		private IEnumerator RotateRingsCoroutine()
		{
			if (this.debugRotateRingsTime > 0.01f)
			{
				float routineStartTime = Time.time;
				this.ringRotationProgress = 0f;
				this.debugRandomizingRings = true;
				while (this.ringRotationProgress < 1f)
				{
					this.ringRotationProgress = (Time.time - routineStartTime) / this.debugRotateRingsTime;
					yield return null;
				}
			}
			this.debugRandomizingRings = false;
			this.ringRotationProgress = 1f;
			yield break;
		}

		// Token: 0x0600677E RID: 26494 RVA: 0x0021BBB0 File Offset: 0x00219DB0
		public bool GetMaterialIfPlayerInGame(int playerActorNumber, out int materialIndex)
		{
			int i = 0;
			while (i < this.inGamePlayerCount)
			{
				if (this.inGamePlayerStates[i].playerId == playerActorNumber)
				{
					if (this.inGamePlayerStates[i].touchedLiquid)
					{
						materialIndex = 12;
						return true;
					}
					materialIndex = 0;
					return true;
				}
				else
				{
					i++;
				}
			}
			materialIndex = 0;
			return false;
		}

		// Token: 0x0600677F RID: 26495 RVA: 0x0021BC04 File Offset: 0x00219E04
		private void OnPlayerTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (base.IsMine)
			{
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == taggedPlayer.ActorNumber)
					{
						num = i;
					}
					else if (this.inGamePlayerStates[i].playerId == taggingPlayer.ActorNumber)
					{
						num2 = i;
					}
					if (num != -1 && num2 != -1)
					{
						break;
					}
				}
				if (num == -1 || num2 == -1)
				{
					return;
				}
				switch (this.tagBehavior)
				{
				case ScienceExperimentManager.TagBehavior.None:
					break;
				case ScienceExperimentManager.TagBehavior.Infect:
					if (this.inGamePlayerStates[num2].touchedLiquid && !this.inGamePlayerStates[num].touchedLiquid)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[num];
						playerGameState.touchedLiquid = true;
						playerGameState.touchedLiquidAtProgress = this.riseProgressLinear;
						this.inGamePlayerStates[num] = playerGameState;
						return;
					}
					break;
				case ScienceExperimentManager.TagBehavior.Revive:
					if (!this.inGamePlayerStates[num2].touchedLiquid && this.inGamePlayerStates[num].touchedLiquid)
					{
						ScienceExperimentManager.PlayerGameState playerGameState2 = this.inGamePlayerStates[num];
						playerGameState2.touchedLiquid = false;
						playerGameState2.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[num] = playerGameState2;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06006780 RID: 26496 RVA: 0x0021BD48 File Offset: 0x00219F48
		private void OnColliderEnteredVolume(Collider collider)
		{
			VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				this.PlayerEnteredGameArea(component.creator.ActorNumber);
			}
		}

		// Token: 0x06006781 RID: 26497 RVA: 0x0021BD88 File Offset: 0x00219F88
		private void OnColliderExitedVolume(Collider collider)
		{
			VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				this.PlayerExitedGameArea(component.creator.ActorNumber);
			}
		}

		// Token: 0x06006782 RID: 26498 RVA: 0x0021BDC8 File Offset: 0x00219FC8
		private void OnColliderEnteredSoda(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				if (base.IsMine)
				{
					this.PlayerTouchedLava(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerTouchedLavaRPC", 2, Array.Empty<object>());
			}
		}

		// Token: 0x06006783 RID: 26499 RVA: 0x00002789 File Offset: 0x00000989
		private void OnColliderExitedSoda(WaterVolume volume, Collider collider)
		{
		}

		// Token: 0x06006784 RID: 26500 RVA: 0x0021BE1C File Offset: 0x0021A01C
		private void OnColliderEnteredRefreshWater(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				if (base.IsMine)
				{
					this.PlayerTouchedRefreshWater(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerTouchedRefreshWaterRPC", 2, Array.Empty<object>());
			}
		}

		// Token: 0x06006785 RID: 26501 RVA: 0x00002789 File Offset: 0x00000989
		private void OnColliderExitedRefreshWater(WaterVolume volume, Collider collider)
		{
		}

		// Token: 0x06006786 RID: 26502 RVA: 0x0021BE6F File Offset: 0x0021A06F
		private void OnProjectileEnteredSodaWater(SlingshotProjectile projectile, Collider collider)
		{
			if (projectile.gameObject.CompareTag(this.mentoProjectileTag))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x06006787 RID: 26503 RVA: 0x0021BE98 File Offset: 0x0021A098
		private void AddLavaRock(int playerId)
		{
			if (base.IsMine && this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (!this.inGamePlayerStates[i].touchedLiquid)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
					this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				}
			}
		}

		// Token: 0x06006788 RID: 26504 RVA: 0x0021BF0C File Offset: 0x0021A10C
		public void OnWaterBalloonHitPlayer(NetPlayer hitPlayer)
		{
			bool flag = false;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == hitPlayer.ActorNumber)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (hitPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.ValidateLocalPlayerWaterBalloonHit(hitPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("ValidateLocalPlayerWaterBalloonHitRPC", 1, new object[]
				{
					hitPlayer.ActorNumber
				});
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06006789 RID: 26505 RVA: 0x0021BF89 File Offset: 0x0021A189
		// (set) Token: 0x0600678A RID: 26506 RVA: 0x0021BFB3 File Offset: 0x0021A1B3
		[Networked]
		[NetworkedWeaved(0, 76)]
		private unsafe ScienceExperimentManager.ScienceManagerData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ScienceExperimentManager.ScienceManagerData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ScienceExperimentManager.ScienceManagerData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x0600678B RID: 26507 RVA: 0x0021BFE0 File Offset: 0x0021A1E0
		public override void WriteDataFusion()
		{
			ScienceExperimentManager.ScienceManagerData data = new ScienceExperimentManager.ScienceManagerData((int)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.stateStartLiquidProgressLinear, this.reliableState.activationProgress, (int)this.nextRoundRiseSpeed, this.riseTime, this.lastWinnerId, this.inGamePlayerCount, this.inGamePlayerStates, this.rotatingRings);
			this.Data = data;
		}

		// Token: 0x0600678C RID: 26508 RVA: 0x0021C04C File Offset: 0x0021A24C
		public override void ReadDataFusion()
		{
			int num = this.lastWinnerId;
			ScienceExperimentManager.RiseSpeed riseSpeed = this.nextRoundRiseSpeed;
			this.reliableState.state = (ScienceExperimentManager.RisingLiquidState)this.Data.reliableState;
			this.reliableState.stateStartTime = this.Data.stateStartTime;
			this.reliableState.stateStartLiquidProgressLinear = this.Data.stateStartLiquidProgressLinear.ClampSafe(0f, 1f);
			this.reliableState.activationProgress = this.Data.activationProgress.GetFinite();
			this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed)this.Data.nextRoundRiseSpeed;
			this.riseTime = this.Data.riseTime.GetFinite();
			this.lastWinnerId = this.Data.lastWinnerId;
			this.inGamePlayerCount = Mathf.Clamp(this.Data.inGamePlayerCount, 0, 10);
			for (int i = 0; i < 10; i++)
			{
				this.inGamePlayerStates[i].playerId = this.Data.playerIdArray[i];
				this.inGamePlayerStates[i].touchedLiquid = this.Data.touchedLiquidArray[i];
				this.inGamePlayerStates[i].touchedLiquidAtProgress = this.Data.touchedLiquidAtProgressArray[i].ClampSafe(0f, 1f);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				this.rotatingRings[j].initialAngle = this.Data.initialAngleArray[j].GetFinite();
				this.rotatingRings[j].resultingAngle = this.Data.resultingAngleArray[j].GetFinite();
			}
			float num2 = this.riseProgress;
			this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
			this.localLagRiseProgressOffset = num2 - this.riseProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
		}

		// Token: 0x0600678D RID: 26509 RVA: 0x0021C274 File Offset: 0x0021A474
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext((int)this.reliableState.state);
			stream.SendNext(this.reliableState.stateStartTime);
			stream.SendNext(this.reliableState.stateStartLiquidProgressLinear);
			stream.SendNext(this.reliableState.activationProgress);
			stream.SendNext((int)this.nextRoundRiseSpeed);
			stream.SendNext(this.riseTime);
			stream.SendNext(this.lastWinnerId);
			stream.SendNext(this.inGamePlayerCount);
			for (int i = 0; i < 10; i++)
			{
				stream.SendNext(this.inGamePlayerStates[i].playerId);
				stream.SendNext(this.inGamePlayerStates[i].touchedLiquid);
				stream.SendNext(this.inGamePlayerStates[i].touchedLiquidAtProgress);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				stream.SendNext(this.rotatingRings[j].initialAngle);
				stream.SendNext(this.rotatingRings[j].resultingAngle);
			}
		}

		// Token: 0x0600678E RID: 26510 RVA: 0x0021C3CC File Offset: 0x0021A5CC
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			int num = this.lastWinnerId;
			ScienceExperimentManager.RiseSpeed riseSpeed = this.nextRoundRiseSpeed;
			this.reliableState.state = (ScienceExperimentManager.RisingLiquidState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = ((double)stream.ReceiveNext()).GetFinite();
			this.reliableState.stateStartLiquidProgressLinear = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.reliableState.activationProgress = ((double)stream.ReceiveNext()).GetFinite();
			this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed)((int)stream.ReceiveNext());
			this.riseTime = ((float)stream.ReceiveNext()).GetFinite();
			this.lastWinnerId = (int)stream.ReceiveNext();
			this.inGamePlayerCount = (int)stream.ReceiveNext();
			this.inGamePlayerCount = Mathf.Clamp(this.inGamePlayerCount, 0, 10);
			for (int i = 0; i < 10; i++)
			{
				this.inGamePlayerStates[i].playerId = (int)stream.ReceiveNext();
				this.inGamePlayerStates[i].touchedLiquid = (bool)stream.ReceiveNext();
				this.inGamePlayerStates[i].touchedLiquidAtProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				this.rotatingRings[j].initialAngle = ((float)stream.ReceiveNext()).GetFinite();
				this.rotatingRings[j].resultingAngle = ((float)stream.ReceiveNext()).GetFinite();
			}
			float num2 = this.riseProgress;
			this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
			this.localLagRiseProgressOffset = num2 - this.riseProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
		}

		// Token: 0x0600678F RID: 26511 RVA: 0x0021C5B4 File Offset: 0x0021A7B4
		private void PlayerEnteredGameArea(int pId)
		{
			if (base.IsMine)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == pId)
					{
						flag = true;
						break;
					}
				}
				if (!flag && this.inGamePlayerCount < 10)
				{
					bool touchedLiquid = false;
					this.inGamePlayerStates[this.inGamePlayerCount] = new ScienceExperimentManager.PlayerGameState
					{
						playerId = pId,
						touchedLiquid = touchedLiquid,
						touchedLiquidAtProgress = -1f
					};
					this.inGamePlayerCount++;
					if (this.optPlayersOutOfRoomGameMode)
					{
						GameMode.OptOut(pId);
					}
				}
			}
		}

		// Token: 0x06006790 RID: 26512 RVA: 0x0021C658 File Offset: 0x0021A858
		private void PlayerExitedGameArea(int playerId)
		{
			if (base.IsMine)
			{
				int i = 0;
				while (i < this.inGamePlayerCount)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						this.inGamePlayerCount--;
						if (this.optPlayersOutOfRoomGameMode)
						{
							GameMode.OptIn(playerId);
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x06006791 RID: 26513 RVA: 0x0021C6CE File Offset: 0x0021A8CE
		[PunRPC]
		public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(info.Sender.ActorNumber);
		}

		// Token: 0x06006792 RID: 26514 RVA: 0x0021C6EC File Offset: 0x0021A8EC
		[Rpc(7, 1)]
		public unsafe void RPC_PlayerTouchedLava(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != 4)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								ptr.Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, 0, 0);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(photonMessageInfoWrapped.Sender.ActorNumber);
		}

		// Token: 0x06006793 RID: 26515 RVA: 0x0021C850 File Offset: 0x0021AA50
		private void PlayerTouchedLava(int playerId)
		{
			if (base.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						if (!playerGameState.touchedLiquid)
						{
							playerGameState.touchedLiquidAtProgress = this.riseProgressLinear;
						}
						playerGameState.touchedLiquid = true;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06006794 RID: 26516 RVA: 0x0021C8C2 File Offset: 0x0021AAC2
		[PunRPC]
		private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
		}

		// Token: 0x06006795 RID: 26517 RVA: 0x0021C8E0 File Offset: 0x0021AAE0
		[Rpc(7, 1)]
		private unsafe void RPC_PlayerTouchedRefreshWater(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != 4)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
								int num2 = 8;
								ptr.Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, 0, 0);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(photonMessageInfoWrapped.Sender.ActorNumber);
		}

		// Token: 0x06006796 RID: 26518 RVA: 0x0021CA44 File Offset: 0x0021AC44
		private void PlayerTouchedRefreshWater(int playerId)
		{
			if (base.IsMine && this.RefreshWaterAvailable)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLiquid = false;
						playerGameState.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06006797 RID: 26519 RVA: 0x0021CAB5 File Offset: 0x0021ACB5
		[PunRPC]
		private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ValidateLocalPlayerWaterBalloonHitRPC");
			if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.ValidateLocalPlayerWaterBalloonHit(playerId);
			}
		}

		// Token: 0x06006798 RID: 26520 RVA: 0x0021CADC File Offset: 0x0021ACDC
		[Rpc(InvokeLocal = false)]
		private unsafe void RPC_ValidateLocalPlayerWaterBalloonHit(int playerId, RpcInfo info = default(RpcInfo))
		{
			if (this.InvokeRpc)
			{
				this.InvokeRpc = false;
				GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "ValidateLocalPlayerWaterBalloonHitRPC");
				if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
				{
					this.ValidateLocalPlayerWaterBalloonHit(playerId);
				}
				return;
			}
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", num);
					}
					else if (base.Runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3);
						int num2 = 8;
						*(int*)(ptr2 + num2) = playerId;
						num2 += 4;
						ptr.Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
				}
			}
		}

		// Token: 0x06006799 RID: 26521 RVA: 0x0021CC38 File Offset: 0x0021AE38
		private void ValidateLocalPlayerWaterBalloonHit(int playerId)
		{
			if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber && !GTPlayer.Instance.InWater)
			{
				if (base.IsMine)
				{
					this.PlayerHitByWaterBalloon(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerHitByWaterBalloonRPC", 2, new object[]
				{
					PhotonNetwork.LocalPlayer.ActorNumber
				});
			}
		}

		// Token: 0x0600679A RID: 26522 RVA: 0x0021CCAA File Offset: 0x0021AEAA
		[PunRPC]
		private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x0600679B RID: 26523 RVA: 0x0021CCC0 File Offset: 0x0021AEC0
		[Rpc(7, 1)]
		private unsafe void RPC_PlayerHitByWaterBalloon(int playerId, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != 4)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							num += 4;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 4);
								int num2 = 8;
								*(int*)(ptr2 + num2) = playerId;
								num2 += 4;
								ptr.Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, 0, 0);
						goto IL_12;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x0600679C RID: 26524 RVA: 0x0021CE38 File Offset: 0x0021B038
		private void PlayerHitByWaterBalloon(int playerId)
		{
			if (base.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLiquid = false;
						playerGameState.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x0600679D RID: 26525 RVA: 0x0021CEA1 File Offset: 0x0021B0A1
		public void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			this.PlayerExitedGameArea(otherPlayer.ActorNumber);
		}

		// Token: 0x0600679E RID: 26526 RVA: 0x0021CEB0 File Offset: 0x0021B0B0
		public void OnLeftRoom()
		{
			this.inGamePlayerCount = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
				{
					this.inGamePlayerStates[0] = this.inGamePlayerStates[i];
					this.inGamePlayerCount = 1;
					return;
				}
			}
		}

		// Token: 0x0600679F RID: 26527 RVA: 0x0021CF18 File Offset: 0x0021B118
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!Utils.PlayerInRoom(this.inGamePlayerStates[i].playerId))
				{
					this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
					this.inGamePlayerCount--;
					i--;
				}
			}
		}

		// Token: 0x060067A1 RID: 26529 RVA: 0x0021D194 File Offset: 0x0021B394
		[CompilerGenerated]
		private int <UpdateReliableState>g__GetAlivePlayerCount|105_0()
		{
			int num = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLiquid)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060067A2 RID: 26530 RVA: 0x0021D1CC File Offset: 0x0021B3CC
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060067A3 RID: 26531 RVA: 0x0021D1E4 File Offset: 0x0021B3E4
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x060067A4 RID: 26532 RVA: 0x0021D1F8 File Offset: 0x0021B3F8
		[NetworkRpcWeavedInvoker(1, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerTouchedLava@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerTouchedLava(info);
		}

		// Token: 0x060067A5 RID: 26533 RVA: 0x0021D23C File Offset: 0x0021B43C
		[NetworkRpcWeavedInvoker(2, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerTouchedRefreshWater@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerTouchedRefreshWater(info);
		}

		// Token: 0x060067A6 RID: 26534 RVA: 0x0021D280 File Offset: 0x0021B480
		[NetworkRpcWeavedInvoker(3, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_ValidateLocalPlayerWaterBalloonHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int playerId = num2;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_ValidateLocalPlayerWaterBalloonHit(playerId, info);
		}

		// Token: 0x060067A7 RID: 26535 RVA: 0x0021D2E0 File Offset: 0x0021B4E0
		[NetworkRpcWeavedInvoker(4, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerHitByWaterBalloon@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int playerId = num2;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerHitByWaterBalloon(playerId, info);
		}

		// Token: 0x040075F8 RID: 30200
		public static volatile ScienceExperimentManager instance;

		// Token: 0x040075F9 RID: 30201
		[SerializeField]
		private ScienceExperimentManager.TagBehavior tagBehavior = ScienceExperimentManager.TagBehavior.Infect;

		// Token: 0x040075FA RID: 30202
		[SerializeField]
		private float minScale = 1f;

		// Token: 0x040075FB RID: 30203
		[SerializeField]
		private float maxScale = 10f;

		// Token: 0x040075FC RID: 30204
		[SerializeField]
		private float riseTimeFast = 30f;

		// Token: 0x040075FD RID: 30205
		[SerializeField]
		private float riseTimeMedium = 60f;

		// Token: 0x040075FE RID: 30206
		[SerializeField]
		private float riseTimeSlow = 120f;

		// Token: 0x040075FF RID: 30207
		[SerializeField]
		private float riseTimeExtraSlow = 240f;

		// Token: 0x04007600 RID: 30208
		[SerializeField]
		private float preDrainWaitTime = 3f;

		// Token: 0x04007601 RID: 30209
		[SerializeField]
		private float maxFullTime = 5f;

		// Token: 0x04007602 RID: 30210
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04007603 RID: 30211
		[SerializeField]
		private float fullyDrainedWaitTime = 3f;

		// Token: 0x04007604 RID: 30212
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x04007605 RID: 30213
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007606 RID: 30214
		[SerializeField]
		private float lavaProgressToDisableRefreshWater = 0.18f;

		// Token: 0x04007607 RID: 30215
		[SerializeField]
		private float lavaProgressToEnableRefreshWater = 0.08f;

		// Token: 0x04007608 RID: 30216
		[SerializeField]
		private float entryLiquidMaxScale = 5f;

		// Token: 0x04007609 RID: 30217
		[SerializeField]
		private Vector2 entryLiquidScaleSyncOpeningTop = Vector2.zero;

		// Token: 0x0400760A RID: 30218
		[SerializeField]
		private Vector2 entryLiquidScaleSyncOpeningBottom = Vector2.zero;

		// Token: 0x0400760B RID: 30219
		[SerializeField]
		private float entryBridgeQuadMaxScaleY = 0.0915f;

		// Token: 0x0400760C RID: 30220
		[SerializeField]
		private Vector2 entryBridgeQuadMinMaxZHeight = new Vector2(0.245f, 0.337f);

		// Token: 0x0400760D RID: 30221
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400760E RID: 30222
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400760F RID: 30223
		[SerializeField]
		public GameObject waterBalloonPrefab;

		// Token: 0x04007610 RID: 30224
		[SerializeField]
		private Vector2 rotatingRingRandomAngleRange = Vector2.zero;

		// Token: 0x04007611 RID: 30225
		[SerializeField]
		private bool rotatingRingQuantizeAngles;

		// Token: 0x04007612 RID: 30226
		[SerializeField]
		private float rotatingRingAngleSnapDegrees = 9f;

		// Token: 0x04007613 RID: 30227
		[SerializeField]
		private float drainBlockerSlideTime = 4f;

		// Token: 0x04007614 RID: 30228
		[SerializeField]
		private Vector2 sodaFizzParticleEmissionMinMax = new Vector2(30f, 100f);

		// Token: 0x04007615 RID: 30229
		[SerializeField]
		private float infrequentUpdatePeriod = 3f;

		// Token: 0x04007616 RID: 30230
		[SerializeField]
		private bool optPlayersOutOfRoomGameMode;

		// Token: 0x04007617 RID: 30231
		[SerializeField]
		private bool debugDrawPlayerGameState;

		// Token: 0x04007618 RID: 30232
		private ScienceExperimentSceneElements elements;

		// Token: 0x04007619 RID: 30233
		private NetPlayer[] allPlayersInRoom;

		// Token: 0x0400761A RID: 30234
		private ScienceExperimentManager.RotatingRingState[] rotatingRings = new ScienceExperimentManager.RotatingRingState[0];

		// Token: 0x0400761B RID: 30235
		private const int maxPlayerCount = 10;

		// Token: 0x0400761C RID: 30236
		private ScienceExperimentManager.PlayerGameState[] inGamePlayerStates = new ScienceExperimentManager.PlayerGameState[10];

		// Token: 0x0400761D RID: 30237
		private int inGamePlayerCount;

		// Token: 0x0400761E RID: 30238
		private int lastWinnerId = -1;

		// Token: 0x0400761F RID: 30239
		private string lastWinnerName = "None";

		// Token: 0x04007620 RID: 30240
		private List<ScienceExperimentManager.PlayerGameState> sortedPlayerStates = new List<ScienceExperimentManager.PlayerGameState>();

		// Token: 0x04007621 RID: 30241
		private ScienceExperimentManager.SyncData reliableState;

		// Token: 0x04007622 RID: 30242
		private ScienceExperimentManager.RiseSpeed nextRoundRiseSpeed = ScienceExperimentManager.RiseSpeed.Slow;

		// Token: 0x04007623 RID: 30243
		private float riseTime = 120f;

		// Token: 0x04007624 RID: 30244
		private float riseProgress;

		// Token: 0x04007625 RID: 30245
		private float riseProgressLinear;

		// Token: 0x04007626 RID: 30246
		private float localLagRiseProgressOffset;

		// Token: 0x04007627 RID: 30247
		private double lastInfrequentUpdateTime = -10.0;

		// Token: 0x04007628 RID: 30248
		private string mentoProjectileTag = "ScienceCandyProjectile";

		// Token: 0x04007629 RID: 30249
		private double currentTime;

		// Token: 0x0400762A RID: 30250
		private double prevTime;

		// Token: 0x0400762B RID: 30251
		private float ringRotationProgress = 1f;

		// Token: 0x0400762C RID: 30252
		private float drainBlockerSlideSpeed;

		// Token: 0x0400762D RID: 30253
		private float[] riseTimeLookup;

		// Token: 0x0400762E RID: 30254
		[Header("Scene References")]
		public Transform ringParent;

		// Token: 0x0400762F RID: 30255
		public Transform liquidMeshTransform;

		// Token: 0x04007630 RID: 30256
		public Transform liquidSurfacePlane;

		// Token: 0x04007631 RID: 30257
		public Transform entryWayLiquidMeshTransform;

		// Token: 0x04007632 RID: 30258
		public Transform entryWayBridgeQuadTransform;

		// Token: 0x04007633 RID: 30259
		public Transform drainBlocker;

		// Token: 0x04007634 RID: 30260
		public Transform drainBlockerClosedPosition;

		// Token: 0x04007635 RID: 30261
		public Transform drainBlockerOpenPosition;

		// Token: 0x04007636 RID: 30262
		public WaterVolume liquidVolume;

		// Token: 0x04007637 RID: 30263
		public WaterVolume entryLiquidVolume;

		// Token: 0x04007638 RID: 30264
		public WaterVolume bottleLiquidVolume;

		// Token: 0x04007639 RID: 30265
		public WaterVolume refreshWaterVolume;

		// Token: 0x0400763A RID: 30266
		public CompositeTriggerEvents gameAreaTriggerNotifier;

		// Token: 0x0400763B RID: 30267
		public SlingshotProjectileHitNotifier sodaWaterProjectileTriggerNotifier;

		// Token: 0x0400763C RID: 30268
		public AudioSource eruptionAudioSource;

		// Token: 0x0400763D RID: 30269
		public AudioSource drainAudioSource;

		// Token: 0x0400763E RID: 30270
		public AudioSource rotatingRingsAudioSource;

		// Token: 0x0400763F RID: 30271
		private ParticleSystem.EmissionModule fizzParticleEmission;

		// Token: 0x04007640 RID: 30272
		private bool hasPlayedEruptionEffects;

		// Token: 0x04007641 RID: 30273
		private bool hasPlayedDrainEffects;

		// Token: 0x04007643 RID: 30275
		[SerializeField]
		private float debugRotateRingsTime = 10f;

		// Token: 0x04007644 RID: 30276
		private Coroutine rotateRingsCoroutine;

		// Token: 0x04007645 RID: 30277
		private bool debugRandomizingRings;

		// Token: 0x04007646 RID: 30278
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 76)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private ScienceExperimentManager.ScienceManagerData _Data;

		// Token: 0x02000FFA RID: 4090
		public enum RisingLiquidState
		{
			// Token: 0x04007648 RID: 30280
			Drained,
			// Token: 0x04007649 RID: 30281
			Erupting,
			// Token: 0x0400764A RID: 30282
			Rising,
			// Token: 0x0400764B RID: 30283
			Full,
			// Token: 0x0400764C RID: 30284
			PreDrainDelay,
			// Token: 0x0400764D RID: 30285
			Draining
		}

		// Token: 0x02000FFB RID: 4091
		private enum RiseSpeed
		{
			// Token: 0x0400764F RID: 30287
			Fast,
			// Token: 0x04007650 RID: 30288
			Medium,
			// Token: 0x04007651 RID: 30289
			Slow,
			// Token: 0x04007652 RID: 30290
			ExtraSlow
		}

		// Token: 0x02000FFC RID: 4092
		private enum TagBehavior
		{
			// Token: 0x04007654 RID: 30292
			None,
			// Token: 0x04007655 RID: 30293
			Infect,
			// Token: 0x04007656 RID: 30294
			Revive
		}

		// Token: 0x02000FFD RID: 4093
		[Serializable]
		public struct PlayerGameState
		{
			// Token: 0x04007657 RID: 30295
			public int playerId;

			// Token: 0x04007658 RID: 30296
			public bool touchedLiquid;

			// Token: 0x04007659 RID: 30297
			public float touchedLiquidAtProgress;
		}

		// Token: 0x02000FFE RID: 4094
		private struct SyncData
		{
			// Token: 0x0400765A RID: 30298
			public ScienceExperimentManager.RisingLiquidState state;

			// Token: 0x0400765B RID: 30299
			public double stateStartTime;

			// Token: 0x0400765C RID: 30300
			public float stateStartLiquidProgressLinear;

			// Token: 0x0400765D RID: 30301
			public double activationProgress;
		}

		// Token: 0x02000FFF RID: 4095
		private struct RotatingRingState
		{
			// Token: 0x0400765E RID: 30302
			public Transform ringTransform;

			// Token: 0x0400765F RID: 30303
			public float initialAngle;

			// Token: 0x04007660 RID: 30304
			public float resultingAngle;
		}

		// Token: 0x02001000 RID: 4096
		[Serializable]
		private struct DisableByLiquidData
		{
			// Token: 0x04007661 RID: 30305
			public Transform target;

			// Token: 0x04007662 RID: 30306
			public float heightOffset;
		}

		// Token: 0x02001001 RID: 4097
		[NetworkStructWeaved(76)]
		[StructLayout(2, Size = 304)]
		private struct ScienceManagerData : INetworkStruct
		{
			// Token: 0x170009B9 RID: 2489
			// (get) Token: 0x060067A8 RID: 26536 RVA: 0x0021D340 File Offset: 0x0021B540
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
			[NetworkedWeaved(10, 10)]
			public NetworkArray<int> playerIdArray
			{
				get
				{
					return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerIdArray), 10, ElementReaderWriterInt32.GetInstance());
				}
			}

			// Token: 0x170009BA RID: 2490
			// (get) Token: 0x060067A9 RID: 26537 RVA: 0x0021D368 File Offset: 0x0021B568
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterBoolean))]
			[NetworkedWeaved(20, 10)]
			public NetworkArray<bool> touchedLiquidArray
			{
				get
				{
					return new NetworkArray<bool>(Native.ReferenceToPointer<FixedStorage@10>(ref this._touchedLiquidArray), 10, ElementReaderWriterBoolean.GetInstance());
				}
			}

			// Token: 0x170009BB RID: 2491
			// (get) Token: 0x060067AA RID: 26538 RVA: 0x0021D390 File Offset: 0x0021B590
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterSingle))]
			[NetworkedWeaved(30, 10)]
			public NetworkArray<float> touchedLiquidAtProgressArray
			{
				get
				{
					return new NetworkArray<float>(Native.ReferenceToPointer<FixedStorage@10>(ref this._touchedLiquidAtProgressArray), 10, ElementReaderWriterSingle.GetInstance());
				}
			}

			// Token: 0x170009BC RID: 2492
			// (get) Token: 0x060067AB RID: 26539 RVA: 0x0021D3B8 File Offset: 0x0021B5B8
			[Networked]
			[Capacity(5)]
			[NetworkedWeavedLinkedList(5, 1, typeof(ElementReaderWriterSingle))]
			[NetworkedWeaved(40, 18)]
			public NetworkLinkedList<float> initialAngleArray
			{
				get
				{
					return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage@18>(ref this._initialAngleArray), 5, ElementReaderWriterSingle.GetInstance());
				}
			}

			// Token: 0x170009BD RID: 2493
			// (get) Token: 0x060067AC RID: 26540 RVA: 0x0021D3DC File Offset: 0x0021B5DC
			[Networked]
			[Capacity(5)]
			[NetworkedWeavedLinkedList(5, 1, typeof(ElementReaderWriterSingle))]
			[NetworkedWeaved(58, 18)]
			public NetworkLinkedList<float> resultingAngleArray
			{
				get
				{
					return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage@18>(ref this._resultingAngleArray), 5, ElementReaderWriterSingle.GetInstance());
				}
			}

			// Token: 0x060067AD RID: 26541 RVA: 0x0021D400 File Offset: 0x0021B600
			public ScienceManagerData(int reliableState, double stateStartTime, float stateStartLiquidProgressLinear, double activationProgress, int nextRoundRiseSpeed, float riseTime, int lastWinnerId, int inGamePlayerCount, ScienceExperimentManager.PlayerGameState[] playerStates, ScienceExperimentManager.RotatingRingState[] rings)
			{
				this.reliableState = reliableState;
				this.stateStartTime = stateStartTime;
				this.stateStartLiquidProgressLinear = stateStartLiquidProgressLinear;
				this.activationProgress = activationProgress;
				this.nextRoundRiseSpeed = nextRoundRiseSpeed;
				this.riseTime = riseTime;
				this.lastWinnerId = lastWinnerId;
				this.inGamePlayerCount = inGamePlayerCount;
				foreach (ScienceExperimentManager.RotatingRingState rotatingRingState in rings)
				{
					this.initialAngleArray.Add(rotatingRingState.initialAngle);
					this.resultingAngleArray.Add(rotatingRingState.resultingAngle);
				}
				int[] array = new int[10];
				bool[] array2 = new bool[10];
				float[] array3 = new float[10];
				for (int j = 0; j < 10; j++)
				{
					array[j] = playerStates[j].playerId;
					array2[j] = playerStates[j].touchedLiquid;
					array3[j] = playerStates[j].touchedLiquidAtProgress;
				}
				this.playerIdArray.CopyFrom(array, 0, array.Length);
				this.touchedLiquidArray.CopyFrom(array2, 0, array2.Length);
				this.touchedLiquidAtProgressArray.CopyFrom(array3, 0, array3.Length);
			}

			// Token: 0x04007663 RID: 30307
			[FieldOffset(0)]
			public int reliableState;

			// Token: 0x04007664 RID: 30308
			[FieldOffset(4)]
			public double stateStartTime;

			// Token: 0x04007665 RID: 30309
			[FieldOffset(12)]
			public float stateStartLiquidProgressLinear;

			// Token: 0x04007666 RID: 30310
			[FieldOffset(16)]
			public double activationProgress;

			// Token: 0x04007667 RID: 30311
			[FieldOffset(24)]
			public int nextRoundRiseSpeed;

			// Token: 0x04007668 RID: 30312
			[FieldOffset(28)]
			public float riseTime;

			// Token: 0x04007669 RID: 30313
			[FieldOffset(32)]
			public int lastWinnerId;

			// Token: 0x0400766A RID: 30314
			[FieldOffset(36)]
			public int inGamePlayerCount;

			// Token: 0x0400766B RID: 30315
			[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(40)]
			private FixedStorage@10 _playerIdArray;

			// Token: 0x0400766C RID: 30316
			[FixedBufferProperty(typeof(NetworkArray<bool>), typeof(UnityArraySurrogate@ElementReaderWriterBoolean), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(80)]
			private FixedStorage@10 _touchedLiquidArray;

			// Token: 0x0400766D RID: 30317
			[FixedBufferProperty(typeof(NetworkArray<float>), typeof(UnityArraySurrogate@ElementReaderWriterSingle), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(120)]
			private FixedStorage@10 _touchedLiquidAtProgressArray;

			// Token: 0x0400766E RID: 30318
			[FixedBufferProperty(typeof(NetworkLinkedList<float>), typeof(UnityLinkedListSurrogate@ElementReaderWriterSingle), 5, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(160)]
			private FixedStorage@18 _initialAngleArray;

			// Token: 0x0400766F RID: 30319
			[FixedBufferProperty(typeof(NetworkLinkedList<float>), typeof(UnityLinkedListSurrogate@ElementReaderWriterSingle), 5, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(232)]
			private FixedStorage@18 _resultingAngleArray;
		}
	}
}
