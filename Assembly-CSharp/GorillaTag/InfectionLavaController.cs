using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FF6 RID: 4086
	public class InfectionLavaController : MonoBehaviour, IGorillaSerializeableScene, IGorillaSerializeable, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06006732 RID: 26418 RVA: 0x00218FBD File Offset: 0x002171BD
		public static InfectionLavaController Instance
		{
			get
			{
				return InfectionLavaController.instance;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x06006733 RID: 26419 RVA: 0x00218FC4 File Offset: 0x002171C4
		public bool LavaCurrentlyActivated
		{
			get
			{
				return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06006734 RID: 26420 RVA: 0x00218FD4 File Offset: 0x002171D4
		public Plane LavaPlane
		{
			get
			{
				return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06006735 RID: 26421 RVA: 0x00218FF1 File Offset: 0x002171F1
		public Vector3 SurfaceCenter
		{
			get
			{
				return this.lavaSurfacePlaneTransform.position;
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06006736 RID: 26422 RVA: 0x00219000 File Offset: 0x00217200
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

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06006737 RID: 26423 RVA: 0x00219030 File Offset: 0x00217230
		private bool InCompetitiveQueue
		{
			get
			{
				return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
			}
		}

		// Token: 0x06006738 RID: 26424 RVA: 0x00219054 File Offset: 0x00217254
		private void Awake()
		{
			if (InfectionLavaController.instance.IsNotNull())
			{
				Object.Destroy(base.gameObject);
				return;
			}
			InfectionLavaController.instance = this;
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
			((IGuidedRefObject)this).GuidedRefInitialize();
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x00219105 File Offset: 0x00217305
		protected void OnEnable()
		{
			if (!this.guidedRefsFullyResolved)
			{
				return;
			}
			this.VerifyReferences();
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x0600673A RID: 26426 RVA: 0x0021911C File Offset: 0x0021731C
		void IGorillaSerializeableScene.OnSceneLinking(GorillaSerializerScene netObj)
		{
			this.networkObject = netObj;
		}

		// Token: 0x0600673B RID: 26427 RVA: 0x001338F3 File Offset: 0x00131AF3
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x0600673C RID: 26428 RVA: 0x00219128 File Offset: 0x00217328
		private void VerifyReferences()
		{
			this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
			}
		}

		// Token: 0x0600673D RID: 26429 RVA: 0x002191DC File Offset: 0x002173DC
		private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
		{
			if (obj != null)
			{
				return;
			}
			fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
			Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
			base.enabled = false;
		}

		// Token: 0x0600673E RID: 26430 RVA: 0x0021922C File Offset: 0x0021742C
		private void OnDestroy()
		{
			if (InfectionLavaController.instance == this)
			{
				InfectionLavaController.instance = null;
			}
			TickSystem<object>.RemovePostTickCallback(this);
			this.UpdateLava(0f);
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x0600673F RID: 26431 RVA: 0x002192A7 File Offset: 0x002174A7
		// (set) Token: 0x06006740 RID: 26432 RVA: 0x002192AF File Offset: 0x002174AF
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06006741 RID: 26433 RVA: 0x002192B8 File Offset: 0x002174B8
		void ITickSystemPost.PostTick()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
			if (this.networkObject.HasAuthority)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
			this.UpdateVolcanoActivationLava((float)this.reliableState.activationProgress);
			this.CheckLocalPlayerAgainstLava(this.currentTime);
		}

		// Token: 0x06006742 RID: 26434 RVA: 0x00219378 File Offset: 0x00217578
		private void JumpToState(InfectionLavaController.RisingLavaState state)
		{
			this.reliableState.state = state;
			switch (state)
			{
			case InfectionLavaController.RisingLavaState.Drained:
				for (int i = 0; i < this.volcanoEffects.Length; i++)
				{
					VolcanoEffects volcanoEffects = this.volcanoEffects[i];
					if (volcanoEffects != null)
					{
						volcanoEffects.SetDrainedState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Erupting:
				for (int j = 0; j < this.volcanoEffects.Length; j++)
				{
					VolcanoEffects volcanoEffects2 = this.volcanoEffects[j];
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.SetEruptingState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Rising:
				for (int k = 0; k < this.volcanoEffects.Length; k++)
				{
					VolcanoEffects volcanoEffects3 = this.volcanoEffects[k];
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.SetRisingState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Full:
				for (int l = 0; l < this.volcanoEffects.Length; l++)
				{
					VolcanoEffects volcanoEffects4 = this.volcanoEffects[l];
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.SetFullState();
					}
				}
				return;
			case InfectionLavaController.RisingLavaState.Draining:
				for (int m = 0; m < this.volcanoEffects.Length; m++)
				{
					VolcanoEffects volcanoEffects5 = this.volcanoEffects[m];
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.SetDrainingState();
					}
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x06006743 RID: 26435 RVA: 0x00219474 File Offset: 0x00217674
		private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
				if (syncData.activationProgress > 1.0)
				{
					float playerCount = (float)this.PlayerCount;
					float num = this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue;
					int num2 = Mathf.RoundToInt(playerCount * num);
					if (this.lavaActivationVoteCount >= num2)
					{
						for (int i = 0; i < this.lavaActivationVoteCount; i++)
						{
							this.lavaActivationVotePlayerIds[i] = 0;
						}
						this.lavaActivationVoteCount = 0;
						syncData.state = InfectionLavaController.RisingLavaState.Erupting;
						syncData.stateStartTime = currentTime;
						syncData.activationProgress = 1.0;
						for (int j = 0; j < this.volcanoEffects.Length; j++)
						{
							VolcanoEffects volcanoEffects = this.volcanoEffects[j];
							if (volcanoEffects != null)
							{
								volcanoEffects.SetEruptingState();
							}
						}
						return;
					}
				}
				else
				{
					float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
					double activationProgress = syncData.activationProgress;
					syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
					if (activationProgress > 0.0 && syncData.activationProgress <= 5E-324)
					{
						VolcanoEffects[] array = this.volcanoEffects;
						for (int k = 0; k < array.Length; k++)
						{
							array[k].OnVolcanoBellyEmpty();
						}
						return;
					}
				}
				break;
			case InfectionLavaController.RisingLavaState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Rising;
					syncData.stateStartTime = currentTime;
					for (int l = 0; l < this.volcanoEffects.Length; l++)
					{
						VolcanoEffects volcanoEffects2 = this.volcanoEffects[l];
						if (volcanoEffects2 != null)
						{
							volcanoEffects2.SetRisingState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Rising:
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Full;
					syncData.stateStartTime = currentTime;
					for (int m = 0; m < this.volcanoEffects.Length; m++)
					{
						VolcanoEffects volcanoEffects3 = this.volcanoEffects[m];
						if (volcanoEffects3 != null)
						{
							volcanoEffects3.SetFullState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Full:
				if (currentTime > syncData.stateStartTime + (double)this.fullTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Draining;
					syncData.stateStartTime = currentTime;
					for (int n = 0; n < this.volcanoEffects.Length; n++)
					{
						VolcanoEffects volcanoEffects4 = this.volcanoEffects[n];
						if (volcanoEffects4 != null)
						{
							volcanoEffects4.SetDrainingState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Draining:
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * Time.deltaTime);
				if (currentTime > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Drained;
					syncData.stateStartTime = currentTime;
					for (int num4 = 0; num4 < this.volcanoEffects.Length; num4++)
					{
						VolcanoEffects volcanoEffects5 = this.volcanoEffects[num4];
						if (volcanoEffects5 != null)
						{
							volcanoEffects5.SetDrainedState();
						}
					}
				}
				break;
			}
		}

		// Token: 0x06006744 RID: 26436 RVA: 0x00219760 File Offset: 0x00217960
		private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
		{
			switch (syncData.state)
			{
			default:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float time = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
				{
					if (volcanoEffects != null)
					{
						volcanoEffects.UpdateDrainedState(time);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Erupting:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num = (float)(currentTime - syncData.stateStartTime);
				float progress = Mathf.Clamp01(num / this.eruptTime);
				foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
				{
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.UpdateEruptingState(num, this.eruptTime - num, progress);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Rising:
			{
				float num2 = (float)(currentTime - syncData.stateStartTime) / this.riseTime;
				this.lavaProgressLinear = Mathf.Clamp01(num2);
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				float num3 = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
				{
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.UpdateRisingState(num3, this.riseTime - num3, this.lavaProgressLinear);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Full:
			{
				this.lavaProgressLinear = 1f;
				this.lavaProgressSmooth = 1f;
				float num4 = (float)(currentTime - syncData.stateStartTime);
				float progress2 = Mathf.Clamp01(this.fullTime / num4);
				foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
				{
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.UpdateFullState(num4, this.fullTime - num4, progress2);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Draining:
			{
				float num5 = (float)(currentTime - syncData.stateStartTime);
				float num6 = Mathf.Clamp01(num5 / this.drainTime);
				this.lavaProgressLinear = 1f - num6;
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
				{
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.UpdateDrainingState(num5, this.riseTime - num5, num6);
					}
				}
				return;
			}
			}
		}

		// Token: 0x06006745 RID: 26437 RVA: 0x002199A8 File Offset: 0x00217BA8
		private void UpdateLava(float fillProgress)
		{
			this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
			if (this.lavaMeshTransform != null)
			{
				this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
			}
		}

		// Token: 0x06006746 RID: 26438 RVA: 0x00219A14 File Offset: 0x00217C14
		private void UpdateVolcanoActivationLava(float activationProgress)
		{
			this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
			this.lavaActivationRenderer.material.SetColor(ShaderProps._BaseColor, this.lavaActivationGradient.Evaluate(activationProgress));
			this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
		}

		// Token: 0x06006747 RID: 26439 RVA: 0x00219A91 File Offset: 0x00217C91
		private void CheckLocalPlayerAgainstLava(double currentTime)
		{
			if (GTPlayer.Instance.InWater && GTPlayer.Instance.CurrentWaterVolume == this.lavaVolume)
			{
				this.LocalPlayerInLava(currentTime, false);
			}
		}

		// Token: 0x06006748 RID: 26440 RVA: 0x00219ABE File Offset: 0x00217CBE
		private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
			}
		}

		// Token: 0x06006749 RID: 26441 RVA: 0x00219AF8 File Offset: 0x00217CF8
		private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
		{
			GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
			if (gorillaGameManager != null && gorillaGameManager.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
			{
				this.lastTagSelfRPCTime = currentTime;
				GameMode.ReportHit();
			}
		}

		// Token: 0x0600674A RID: 26442 RVA: 0x00219B4A File Offset: 0x00217D4A
		public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (projectile.gameObject.CompareTag("LavaRockProjectile"))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x0600674B RID: 26443 RVA: 0x00219B70 File Offset: 0x00217D70
		private void AddLavaRock(int playerId)
		{
			if (this.networkObject.HasAuthority && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
				this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				this.AddVoteForVolcanoActivation(playerId);
				VolcanoEffects[] array = this.volcanoEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnStoneAccepted(this.reliableState.activationProgress);
				}
			}
		}

		// Token: 0x0600674C RID: 26444 RVA: 0x00219BEC File Offset: 0x00217DEC
		private void AddVoteForVolcanoActivation(int playerId)
		{
			if (this.networkObject.HasAuthority && this.lavaActivationVoteCount < 10)
			{
				bool flag = false;
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
					this.lavaActivationVoteCount++;
				}
			}
		}

		// Token: 0x0600674D RID: 26445 RVA: 0x00219C50 File Offset: 0x00217E50
		private void RemoveVoteForVolcanoActivation(int playerId)
		{
			if (this.networkObject.HasAuthority)
			{
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
						this.lavaActivationVoteCount--;
						return;
					}
				}
			}
		}

		// Token: 0x0600674E RID: 26446 RVA: 0x00219CAC File Offset: 0x00217EAC
		void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext((int)this.reliableState.state);
			stream.SendNext(this.reliableState.stateStartTime);
			stream.SendNext(this.reliableState.activationProgress);
			stream.SendNext(this.lavaActivationVoteCount);
			stream.SendNext(this.lavaActivationVotePlayerIds[0]);
			stream.SendNext(this.lavaActivationVotePlayerIds[1]);
			stream.SendNext(this.lavaActivationVotePlayerIds[2]);
			stream.SendNext(this.lavaActivationVotePlayerIds[3]);
			stream.SendNext(this.lavaActivationVotePlayerIds[4]);
			stream.SendNext(this.lavaActivationVotePlayerIds[5]);
			stream.SendNext(this.lavaActivationVotePlayerIds[6]);
			stream.SendNext(this.lavaActivationVotePlayerIds[7]);
			stream.SendNext(this.lavaActivationVotePlayerIds[8]);
			stream.SendNext(this.lavaActivationVotePlayerIds[9]);
		}

		// Token: 0x0600674F RID: 26447 RVA: 0x00219DCC File Offset: 0x00217FCC
		void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			InfectionLavaController.RisingLavaState risingLavaState = (InfectionLavaController.RisingLavaState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = ((double)stream.ReceiveNext()).GetFinite();
			this.reliableState.activationProgress = ((double)stream.ReceiveNext()).ClampSafe(0.0, 2.0);
			this.lavaActivationVoteCount = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[0] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[1] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[2] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[3] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[4] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[5] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[6] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[7] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[8] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[9] = (int)stream.ReceiveNext();
			float num = this.lavaProgressSmooth;
			if (risingLavaState != this.reliableState.state)
			{
				this.JumpToState(risingLavaState);
			}
			this.UpdateLocalState((double)((float)NetworkSystem.Instance.SimTime), this.reliableState);
			this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
		}

		// Token: 0x06006750 RID: 26448 RVA: 0x00219F3F File Offset: 0x0021813F
		public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
		{
			this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
		}

		// Token: 0x06006751 RID: 26449 RVA: 0x00219F50 File Offset: 0x00218150
		private void OnLeftRoom()
		{
			for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] != NetworkSystem.Instance.LocalPlayerID)
				{
					this.RemoveVoteForVolcanoActivation(this.lavaActivationVotePlayerIds[i]);
				}
			}
		}

		// Token: 0x06006752 RID: 26450 RVA: 0x00002789 File Offset: 0x00000989
		void IGorillaSerializeableScene.OnNetworkObjectDisable()
		{
		}

		// Token: 0x06006753 RID: 26451 RVA: 0x00002789 File Offset: 0x00000989
		void IGorillaSerializeableScene.OnNetworkObjectEnable()
		{
		}

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x06006754 RID: 26452 RVA: 0x00219F92 File Offset: 0x00218192
		// (set) Token: 0x06006755 RID: 26453 RVA: 0x00219F9A File Offset: 0x0021819A
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06006756 RID: 26454 RVA: 0x00219FA3 File Offset: 0x002181A3
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			this.guidedRefsFullyResolved = true;
			this.VerifyReferences();
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06006757 RID: 26455 RVA: 0x00219FB8 File Offset: 0x002181B8
		public void OnGuidedRefTargetDestroyed(int fieldId)
		{
			this.guidedRefsFullyResolved = false;
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06006758 RID: 26456 RVA: 0x00219FC8 File Offset: 0x002181C8
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaMeshTransform_gRef", ref this.lavaMeshTransform_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaSurfacePlaneTransform_gRef", ref this.lavaSurfacePlaneTransform_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaVolume_gRef", ref this.lavaVolume_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationRenderer_gRef", ref this.lavaActivationRenderer_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationStartPos_gRef", ref this.lavaActivationStartPos_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationEndPos_gRef", ref this.lavaActivationEndPos_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationProjectileHitNotifier_gRef", ref this.lavaActivationProjectileHitNotifier_gRef);
			GuidedRefHub.RegisterReceiverArray<InfectionLavaController, VolcanoEffects>(this, "volcanoEffects_gRefs", ref this.volcanoEffects, ref this.volcanoEffects_gRefs);
			GuidedRefHub.ReceiverFullyRegistered<InfectionLavaController>(this);
		}

		// Token: 0x06006759 RID: 26457 RVA: 0x0021A06C File Offset: 0x0021826C
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaMeshTransform, this.lavaMeshTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaSurfacePlaneTransform, this.lavaSurfacePlaneTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, WaterVolume>(this, ref this.lavaVolume, this.lavaVolume_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, MeshRenderer>(this, ref this.lavaActivationRenderer, this.lavaActivationRenderer_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationStartPos, this.lavaActivationStartPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationEndPos, this.lavaActivationEndPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, SlingshotProjectileHitNotifier>(this, ref this.lavaActivationProjectileHitNotifier, this.lavaActivationProjectileHitNotifier_gRef, target) || GuidedRefHub.TryResolveArrayItem<InfectionLavaController, VolcanoEffects>(this, this.volcanoEffects, this.volcanoEffects_gRefs, target);
		}

		// Token: 0x0600675B RID: 26459 RVA: 0x000743A9 File Offset: 0x000725A9
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600675C RID: 26460 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040075BF RID: 30143
		[OnEnterPlay_SetNull]
		private static InfectionLavaController instance;

		// Token: 0x040075C0 RID: 30144
		[SerializeField]
		private float lavaMeshMinScale = 3.17f;

		// Token: 0x040075C1 RID: 30145
		[Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
		[SerializeField]
		private float lavaMeshMaxScale = 8.941086f;

		// Token: 0x040075C2 RID: 30146
		[SerializeField]
		private float eruptTime = 3f;

		// Token: 0x040075C3 RID: 30147
		[SerializeField]
		private float riseTime = 10f;

		// Token: 0x040075C4 RID: 30148
		[SerializeField]
		private float fullTime = 240f;

		// Token: 0x040075C5 RID: 30149
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x040075C6 RID: 30150
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x040075C7 RID: 30151
		[SerializeField]
		private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040075C8 RID: 30152
		[Header("Volcano Activation")]
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageDefaultQueue = 0.42f;

		// Token: 0x040075C9 RID: 30153
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageCompetitiveQueue = 0.6f;

		// Token: 0x040075CA RID: 30154
		[SerializeField]
		private Gradient lavaActivationGradient;

		// Token: 0x040075CB RID: 30155
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040075CC RID: 30156
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040075CD RID: 30157
		[SerializeField]
		private float lavaActivationVisualMovementProgressPerSecond = 1f;

		// Token: 0x040075CE RID: 30158
		[SerializeField]
		private bool debugLavaActivationVotes;

		// Token: 0x040075CF RID: 30159
		[Header("Scene References")]
		[SerializeField]
		private Transform lavaMeshTransform;

		// Token: 0x040075D0 RID: 30160
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaMeshTransform_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075D1 RID: 30161
		[SerializeField]
		private Transform lavaSurfacePlaneTransform;

		// Token: 0x040075D2 RID: 30162
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaSurfacePlaneTransform_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075D3 RID: 30163
		[SerializeField]
		private WaterVolume lavaVolume;

		// Token: 0x040075D4 RID: 30164
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaVolume_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075D5 RID: 30165
		[SerializeField]
		private MeshRenderer lavaActivationRenderer;

		// Token: 0x040075D6 RID: 30166
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationRenderer_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075D7 RID: 30167
		[SerializeField]
		private Transform lavaActivationStartPos;

		// Token: 0x040075D8 RID: 30168
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationStartPos_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075D9 RID: 30169
		[SerializeField]
		private Transform lavaActivationEndPos;

		// Token: 0x040075DA RID: 30170
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationEndPos_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075DB RID: 30171
		[SerializeField]
		private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

		// Token: 0x040075DC RID: 30172
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationProjectileHitNotifier_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x040075DD RID: 30173
		[SerializeField]
		private VolcanoEffects[] volcanoEffects;

		// Token: 0x040075DE RID: 30174
		[SerializeField]
		private GuidedRefReceiverArrayInfo volcanoEffects_gRefs = new GuidedRefReceiverArrayInfo(true);

		// Token: 0x040075DF RID: 30175
		[DebugReadout]
		private InfectionLavaController.LavaSyncData reliableState;

		// Token: 0x040075E0 RID: 30176
		private int[] lavaActivationVotePlayerIds = new int[10];

		// Token: 0x040075E1 RID: 30177
		private int lavaActivationVoteCount;

		// Token: 0x040075E2 RID: 30178
		private float localLagLavaProgressOffset;

		// Token: 0x040075E3 RID: 30179
		[DebugReadout]
		private float lavaProgressLinear;

		// Token: 0x040075E4 RID: 30180
		[DebugReadout]
		private float lavaProgressSmooth;

		// Token: 0x040075E5 RID: 30181
		private double lastTagSelfRPCTime;

		// Token: 0x040075E6 RID: 30182
		private const string lavaRockProjectileTag = "LavaRockProjectile";

		// Token: 0x040075E7 RID: 30183
		private double currentTime;

		// Token: 0x040075E8 RID: 30184
		private double prevTime;

		// Token: 0x040075E9 RID: 30185
		private float activationProgessSmooth;

		// Token: 0x040075EA RID: 30186
		private float lavaScale;

		// Token: 0x040075EB RID: 30187
		private GorillaSerializerScene networkObject;

		// Token: 0x040075ED RID: 30189
		private bool guidedRefsFullyResolved;

		// Token: 0x02000FF7 RID: 4087
		public enum RisingLavaState
		{
			// Token: 0x040075F0 RID: 30192
			Drained,
			// Token: 0x040075F1 RID: 30193
			Erupting,
			// Token: 0x040075F2 RID: 30194
			Rising,
			// Token: 0x040075F3 RID: 30195
			Full,
			// Token: 0x040075F4 RID: 30196
			Draining
		}

		// Token: 0x02000FF8 RID: 4088
		private struct LavaSyncData
		{
			// Token: 0x040075F5 RID: 30197
			public InfectionLavaController.RisingLavaState state;

			// Token: 0x040075F6 RID: 30198
			public double stateStartTime;

			// Token: 0x040075F7 RID: 30199
			public double activationProgress;
		}
	}
}
