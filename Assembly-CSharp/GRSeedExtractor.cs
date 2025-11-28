using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020006F6 RID: 1782
public class GRSeedExtractor : MonoBehaviour
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x06002DA7 RID: 11687 RVA: 0x000F6953 File Offset: 0x000F4B53
	public bool StationOpen
	{
		get
		{
			return this.stationOpen;
		}
	}

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x06002DA8 RID: 11688 RVA: 0x000F695B File Offset: 0x000F4B5B
	public bool StationOpenForLocalPlayer
	{
		get
		{
			return this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x06002DA9 RID: 11689 RVA: 0x000F697E File Offset: 0x000F4B7E
	public int CurrentPlayerActorNumber
	{
		get
		{
			return this.currentPlayerActorNumber;
		}
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000F6988 File Offset: 0x000F4B88
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		this.coreDepositTriggerNotifier.TriggerEnterEvent += this.DepositorTriggerEntered;
		this.idCardScanner.OnPlayerCardSwipe += this.OnPlayerCardSwipe;
		for (int i = 0; i < this.maxVisualChaosSeedCount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.chaosSeedVisualPrefab, base.transform);
			gameObject.SetActive(false);
			this.chaosSeedVisuals.Add(gameObject);
		}
		this.UpdateOverdrivePurchaseButtons();
		base.enabled = false;
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000F6A34 File Offset: 0x000F4C34
	public void Init(GRToolProgressionManager progression, GhostReactor gr)
	{
		this.ghostReactor = gr;
		this.toolProgressionManager = progression;
		this.toolProgressionManager.OnProgressionUpdated += new Action(this.OnResearchPointsUpdated);
		ProgressionManager.Instance.OnJucierStatusUpdated += new Action<ProgressionManager.JuicerStatusResponse>(this.OnPlayerStatusReceived);
		ProgressionManager.Instance.OnPurchaseOverdrive += new Action<bool>(this.OnPurchaseOverdrive);
		ProgressionManager.Instance.OnChaosDepositSuccess += new Action<bool>(this.TryDepositSeedServerResponse);
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x000F6AA8 File Offset: 0x000F4CA8
	private void OnDisable()
	{
		this.ClearSeedVisuals();
		this.machineHumAudioSource.gameObject.SetActive(false);
		this.juicerSlowParticles.gameObject.SetActive(false);
		base.StopAllCoroutines();
		for (int i = 0; i < this.disableDuringOverdrive.Count; i++)
		{
			this.disableDuringOverdrive[i].gameObject.SetActive(true);
		}
		for (int j = 0; j < this.enableDuringOverdrive.Count; j++)
		{
			this.enableDuringOverdrive[j].gameObject.SetActive(false);
		}
		this.overdriveLightSpinnerOff.localRotation = this.overdriveLightSpinnerOn.localRotation;
		this.overdriveBeepAudioSource.Stop();
		this.overdriveActive = false;
		this.processingAmount = 0f;
		this.processingAmountVisual = 0f;
		this.overdriveAmount = 0f;
		this.overdriveAmountVisual = 0f;
		this.currentPlayerData = default(GRSeedExtractor.PlayerData);
		this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
		this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x000F6BF4 File Offset: 0x000F4DF4
	private void Update()
	{
		this.ValidateCurrentPlayer();
		if (this.stationOpen && this.shutterDoorOpenAmount < 1f)
		{
			float num = Time.time - this.currentPlayerData.latestRefreshTime;
			if (Time.time - this.stationOpenRequestTime >= 1f || num <= 5f)
			{
				float num2 = 1f / this.shutterDoorAnimTime;
				this.shutterDoorOpenAmount = Mathf.MoveTowards(this.shutterDoorOpenAmount, 1f, num2 * Time.deltaTime);
				Vector3 localPosition = this.shutterDoorParent.transform.localPosition;
				localPosition.y = Mathf.Lerp(this.shutterDoorLiftRange.x, this.shutterDoorLiftRange.y, this.shutterDoorOpenAmount);
				this.shutterDoorParent.transform.localPosition = localPosition;
			}
		}
		else if (!this.stationOpen && this.shutterDoorOpenAmount > 0f)
		{
			float num3 = 1f / this.shutterDoorAnimTime;
			this.shutterDoorOpenAmount = Mathf.MoveTowards(this.shutterDoorOpenAmount, 0f, num3 * Time.deltaTime);
			Vector3 localPosition2 = this.shutterDoorParent.transform.localPosition;
			localPosition2.y = Mathf.Lerp(this.shutterDoorLiftRange.x, this.shutterDoorLiftRange.y, this.shutterDoorOpenAmount);
			this.shutterDoorParent.transform.localPosition = localPosition2;
			if (this.shutterDoorOpenAmount <= 0f)
			{
				this.processingAmount = 0f;
				this.overdriveAmount = 0f;
			}
		}
		bool flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
		if (this.overdriveActive)
		{
			this.overdriveLightSpinnerOn.Rotate(Vector3.forward, 360f * this.overdriveLightSpinRate * Time.deltaTime, 1);
			this.overdriveAmountVisual = this.overdriveAmount;
			this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
			this.processingAmountVisual = this.processingAmount;
			this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
		}
		else
		{
			float num4 = 1f / this.overdriveFillTime;
			if (flag || this.overdriveAmount > this.overdriveAmountVisual || !this.stationOpen)
			{
				this.overdriveAmountVisual = Mathf.MoveTowards(this.overdriveAmountVisual, this.overdriveAmount, num4 * Time.deltaTime);
			}
			this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
			if (this.stationOpen)
			{
				float num5 = Mathf.Max(Time.time - this.currentPlayerData.latestRefreshTime, 0f);
				float num6 = this.currentPlayerData.coreProcessingPercentage + num5 / this.PROCESSING_TIME_SECONDS;
				this.processingAmount = Mathf.Clamp01(num6);
				this.estimatedJuiceTimeRemaining = (1f - this.processingAmount) * this.PROCESSING_TIME_SECONDS;
				if (this.StationOpenForLocalPlayer && num6 >= 1f && Time.time - this.lastServerRequestTime > this.timeBetweenServerRequests)
				{
					this.lastServerRequestTime = Time.time;
					ProgressionManager.Instance.GetJuicerStatus();
				}
			}
			if (flag)
			{
				this.machineHumAudioSource.gameObject.SetActive(true);
				this.juicerSlowParticles.gameObject.SetActive(true);
				this.processingAmountVisual = Mathf.MoveTowards(this.processingAmountVisual, this.processingAmount, num4 * Time.deltaTime);
			}
			else
			{
				this.processingAmountVisual = Mathf.MoveTowards(this.processingAmountVisual, 0f, num4 * Time.deltaTime);
				this.machineHumAudioSource.gameObject.SetActive(false);
				this.juicerSlowParticles.gameObject.SetActive(false);
			}
			this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
		}
		this.StepSeedVisualAnimation(Time.deltaTime);
		this.UpdateScreenDisplay();
		if (!this.stationOpen && this.shutterDoorOpenAmount <= 0f && this.overdriveAmountVisual <= 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x000F704C File Offset: 0x000F524C
	private void ValidateCurrentPlayer()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.CloseStation();
			return;
		}
		if (this.ghostReactor.grManager.IsAuthority() && this.stationOpen)
		{
			bool flag = false;
			NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				float num = 5f;
				if (rigContainer.Rig != null && rigContainer.Rig.OwningNetPlayer == player && (rigContainer.Rig.GetMouthPosition() - base.transform.position).sqrMagnitude < num * num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorCloseStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			}
		}
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x000F7124 File Offset: 0x000F5324
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component != null && component.OwningNetPlayer != null && component.OwningNetPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x000F717C File Offset: 0x000F537C
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component != null && component.OwningNetPlayer != null)
		{
			if (component.OwningNetPlayer.ActorNumber == this.currentPlayerActorNumber && this.stationOpen && this.ghostReactor.grManager.IsAuthority() && NetworkSystem.Instance.InRoom)
			{
				this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorCloseStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			}
			if (component.OwningNetPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.localPlayerData = default(GRSeedExtractor.PlayerData);
			}
		}
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000F722C File Offset: 0x000F542C
	public void OnPlayerCardSwipe(int playerActorNumber)
	{
		if (playerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorOpenStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			ProgressionManager.Instance.GetJuicerStatus();
		}
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x000F7284 File Offset: 0x000F5484
	public void DepositorTriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (this.ghostReactor == null || this.ghostReactor.grManager == null || other == null || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.ghostReactor.grManager.IsAuthority() && other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
			if (managerForZone != null && component != null && component.type == ProgressionManager.CoreType.ChaosSeed)
			{
				int netIdFromEntityId = managerForZone.GetNetIdFromEntityId(component.entity.id);
				int lastHeldByActorNumber = component.entity.lastHeldByActorNumber;
				bool player = NetworkSystem.Instance.GetPlayer(lastHeldByActorNumber) != null;
				float time = Time.time;
				if (player)
				{
					bool flag = false;
					for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
					{
						if (time - this.seedDepositsPending[i].Item3 > 5f || managerForZone.GetGameEntityFromNetId(this.seedDepositsPending[i].Item1) == null || NetworkSystem.Instance.GetPlayer(this.seedDepositsPending[i].Item2) == null)
						{
							this.seedDepositsPending.RemoveAt(i);
						}
						else if (this.seedDepositsPending[i].Item1 == netIdFromEntityId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						this.seedDepositsPending.Add(new ValueTuple<int, int, float, bool>(netIdFromEntityId, lastHeldByActorNumber, Time.time, false));
						this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorTryDepositSeed, lastHeldByActorNumber, netIdFromEntityId);
					}
				}
			}
		}
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x000F7435 File Offset: 0x000F5635
	public void OverdrivePurchaseButtonPressed()
	{
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchasePending = false;
		}
		else if (this.LocalPlayerCanPurchaseOverdrive())
		{
			this.overdrivePurchasePending = true;
		}
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x06002DB5 RID: 11701 RVA: 0x000F7460 File Offset: 0x000F5660
	private bool LocalPlayerCanPurchaseOverdrive()
	{
		if (Time.time - this.overdrivePurchaseTime > 5f)
		{
			this.overdriveServerConfirmationPending = false;
		}
		return this.StationOpenForLocalPlayer && !this.overdriveServerConfirmationPending && CosmeticsController.instance.CurrencyBalance >= 250 && this.localPlayerData.overdriveSupply <= 0f;
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x000F74C8 File Offset: 0x000F56C8
	public void OverdrivePurchaseConfirmButtonPressed()
	{
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchasePending = false;
			if (this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.overdriveServerConfirmationPending = true;
				this.overdrivePurchaseTime = Time.time;
				ProgressionManager.Instance.PurchaseOverdrive();
			}
		}
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x000F7528 File Offset: 0x000F5728
	public void OnPlayerStatusReceived(ProgressionManager.JuicerStatusResponse statusResponse)
	{
		if (statusResponse.MothershipId == GRPlayer.GetLocal().mothershipId && statusResponse.RefreshJuice)
		{
			this.toolProgressionManager.UpdateInventory();
		}
		this.PROCESSING_TIME_SECONDS = (float)statusResponse.CoreProcessingTimeSec;
		this.MAX_OVERDRIVE_USES = statusResponse.OverdriveCap / 100;
		float num = Mathf.Clamp01((float)statusResponse.OverdriveSupply / (float)statusResponse.OverdriveCap);
		int num2 = 0;
		bool flag = num < this.localPlayerData.overdriveSupply;
		bool flag2 = this.localPlayerData.overdriveSupply == 0f && this.localPlayerData.coreCount > statusResponse.CurrentCoreCount;
		if (statusResponse.CoresProcessedByOverdrive > 0 && (flag || flag2))
		{
			num2 = statusResponse.CoresProcessedByOverdrive;
		}
		this.localPlayerData.actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.localPlayerData.coreCount = statusResponse.CurrentCoreCount;
		this.localPlayerData.coreProcessingPercentage = Mathf.Clamp01(statusResponse.CoreProcessingPercent);
		this.localPlayerData.overdriveSupply = num;
		this.localPlayerData.coresProcessedByOverdrive = statusResponse.CoresProcessedByOverdrive;
		this.localPlayerData.coresPendingOverdriveProcessing = this.localPlayerData.coresPendingOverdriveProcessing + num2;
		this.localPlayerData.latestRefreshTime = Time.time;
		this.localPlayerData.researchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
		if (this.overdriveServerConfirmationPending && (this.localPlayerData.overdriveSupply > 0f || this.localPlayerData.coresProcessedByOverdrive > 0))
		{
			this.overdriveServerConfirmationPending = false;
		}
		if (this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			this.currentPlayerData = this.localPlayerData;
			this.ghostReactor.grManager.RequestApplySeedExtractorState(this.localPlayerData.coreCount, this.localPlayerData.coresProcessedByOverdrive, this.localPlayerData.researchPoints, this.localPlayerData.coreProcessingPercentage, this.localPlayerData.overdriveSupply);
			this.OnStateUpdated();
		}
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x000F7728 File Offset: 0x000F5928
	private void TryDepositSeedServerResponse(bool succeeded)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		int num = -1;
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < this.seedDepositsPending.Count; i++)
		{
			if (this.seedDepositsPending[i].Item2 == actorNumber)
			{
				num = this.seedDepositsPending[i].Item1;
			}
		}
		if (num == -1)
		{
			return;
		}
		if (succeeded)
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorDepositSeedSucceeded, actorNumber, num);
			this.RemovePendingSeedDeposit(num);
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			grplayer.SendSeedDepositedTelemetry(this.PROCESSING_TIME_SECONDS.ToString(), this.currentPlayerData.coreCount);
			grplayer.IncrementChaosSeedsCollected(1);
			return;
		}
		this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorDepositSeedFailed, actorNumber, num);
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x000F77F4 File Offset: 0x000F59F4
	public void CardSwipeSuccess()
	{
		this.idCardScanner.onSucceeded.Invoke();
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x000F7806 File Offset: 0x000F5A06
	public void CardSwipeFail()
	{
		this.idCardScanner.onFailed.Invoke();
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x000F7818 File Offset: 0x000F5A18
	public void TryDepositSeed(int playerActorNumber, int seedNetId)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(playerActorNumber);
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (player == null || managerForZone == null)
		{
			return;
		}
		this.depositorAudioSource.PlayOneShot(this.seedDepositAttemptAudio, this.seedDepositAttemptVolume);
		if (player.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			bool flag = false;
			float time = Time.time;
			for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
			{
				if (time - this.seedDepositsPending[i].Item3 > 5f || managerForZone.GetGameEntityFromNetId(this.seedDepositsPending[i].Item1) == null || NetworkSystem.Instance.GetPlayer(this.seedDepositsPending[i].Item2) == null)
				{
					this.seedDepositsPending.RemoveAt(i);
				}
				else if (this.seedDepositsPending[i].Item1 == seedNetId)
				{
					flag = true;
					if (this.seedDepositsPending[i].Item2 == NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.seedDepositsPending[i].Item4)
					{
						ValueTuple<int, int, float, bool> valueTuple = this.seedDepositsPending[i];
						valueTuple.Item4 = true;
						this.seedDepositsPending[i] = valueTuple;
						ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.ChaosSeed);
					}
				}
			}
			if (!flag)
			{
				this.seedDepositsPending.Add(new ValueTuple<int, int, float, bool>(seedNetId, playerActorNumber, Time.time, true));
				ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.ChaosSeed);
			}
		}
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x000F79B4 File Offset: 0x000F5BB4
	public bool ValidateSeedDepositSucceeded(int playerActorNumber, int entityNetId)
	{
		if (this.ghostReactor.grManager.IsAuthority())
		{
			bool result = false;
			for (int i = 0; i < this.seedDepositsPending.Count; i++)
			{
				if (this.seedDepositsPending[i].Item1 == entityNetId && this.seedDepositsPending[i].Item2 == playerActorNumber)
				{
					result = true;
				}
			}
			return result;
		}
		return false;
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x000F7A18 File Offset: 0x000F5C18
	public void SeedDepositSucceeded(int playerActorNumber, int entityNetId)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		this.depositorParticles.Play();
		this.depositorAudioSource.PlayOneShot(this.seedDepositAudio, this.seedDepositVolume);
		this.RemovePendingSeedDeposit(entityNetId);
		if (playerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
		if (!this.stationOpen && this.ghostReactor.grManager.IsAuthority())
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorOpenStation, playerActorNumber, 0);
		}
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x000F7AA5 File Offset: 0x000F5CA5
	public void SeedDepositFailed(int playerActorNumber, int entityNetId)
	{
		this.depositorAudioSource.PlayOneShot(this.seedDepositFailedAudio, this.seedDepositFailedVolume);
		this.RemovePendingSeedDeposit(entityNetId);
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x000F7AC8 File Offset: 0x000F5CC8
	private void RemovePendingSeedDeposit(int entityId)
	{
		for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
		{
			if (this.seedDepositsPending[i].Item1 == entityId)
			{
				this.seedDepositsPending.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000F7B10 File Offset: 0x000F5D10
	public void ApplyState(int playerActorNumber, int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply)
	{
		if (playerActorNumber == this.currentPlayerActorNumber)
		{
			if (this.currentPlayerData.actorNumber != playerActorNumber)
			{
				this.currentPlayerData = default(GRSeedExtractor.PlayerData);
			}
			coreCount = Mathf.Clamp(coreCount, 0, this.maxVisualChaosSeedCount);
			coresProcessedByOverdrive = Mathf.Clamp(coresProcessedByOverdrive, 0, this.MAX_OVERDRIVE_USES);
			coreProcessingPercentage = Mathf.Clamp(coreProcessingPercentage, 0f, 1f);
			overdriveSupply = Mathf.Clamp(overdriveSupply, 0f, 1f);
			bool flag = overdriveSupply < this.currentPlayerData.overdriveSupply;
			bool flag2 = this.currentPlayerData.overdriveSupply == 0f && this.currentPlayerData.coreCount > coreCount;
			if (playerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && coresProcessedByOverdrive > 0 && (flag || flag2))
			{
				this.currentPlayerData.coresPendingOverdriveProcessing = this.currentPlayerData.coresPendingOverdriveProcessing + coresProcessedByOverdrive;
			}
			this.currentPlayerData.actorNumber = playerActorNumber;
			this.currentPlayerData.coreCount = coreCount;
			this.currentPlayerData.coresProcessedByOverdrive = coresProcessedByOverdrive;
			this.currentPlayerData.coreProcessingPercentage = coreProcessingPercentage;
			this.currentPlayerData.overdriveSupply = overdriveSupply;
			this.currentPlayerData.latestRefreshTime = Time.time;
			this.currentPlayerData.researchPoints = researchPoints;
			this.OnStateUpdated();
		}
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x000F7C48 File Offset: 0x000F5E48
	public void OpenStation(int playerActorNumber)
	{
		if (NetworkSystem.Instance.GetPlayer(playerActorNumber) == null)
		{
			return;
		}
		if (!this.stationOpen)
		{
			this.doorAudioSource.PlayOneShot(this.doorOpenAudio, this.doorOpenVolume);
		}
		base.enabled = true;
		this.currentPlayerActorNumber = playerActorNumber;
		this.stationOpen = true;
		this.stationOpenRequestTime = Time.time;
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x000F7CA8 File Offset: 0x000F5EA8
	public void CloseStation()
	{
		if (this.stationOpen)
		{
			this.doorAudioSource.PlayOneShot(this.doorCloseAudio, this.doorCloseVolume);
		}
		this.currentPlayerActorNumber = -1;
		this.stationOpen = false;
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x06002DC3 RID: 11715 RVA: 0x000F7CE0 File Offset: 0x000F5EE0
	private void UpdateOverdrivePurchaseButtons()
	{
		if (!this.LocalPlayerCanPurchaseOverdrive())
		{
			this.overdrivePurchaseButton.myTmpText.text = "";
			this.overdrivePurchaseButton.buttonRenderer.material = this.defaultButtonMaterial;
			this.overdriveConfirmButton.myTmpText.text = "";
			this.overdriveConfirmButton.buttonRenderer.material = this.defaultButtonMaterial;
			return;
		}
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchaseButton.myTmpText.text = "CANCEL";
			this.overdrivePurchaseButton.buttonRenderer.material = this.redButtonMaterial;
			this.overdriveConfirmButton.myTmpText.text = "CONFIRM";
			this.overdriveConfirmButton.buttonRenderer.material = this.greenButtonMaterial;
			return;
		}
		this.overdrivePurchaseButton.myTmpText.text = "BUY";
		this.overdrivePurchaseButton.buttonRenderer.material = this.defaultButtonMaterial;
		this.overdriveConfirmButton.myTmpText.text = "";
		this.overdriveConfirmButton.buttonRenderer.material = this.defaultButtonMaterial;
	}

	// Token: 0x06002DC4 RID: 11716 RVA: 0x000F7E04 File Offset: 0x000F6004
	public void OnStateUpdated()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		if (player == null)
		{
			this.CloseStation();
		}
		this.UpdateOverdrivePurchaseButtons();
		if (this.stationOpen && player != null)
		{
			if (this.overdriveActive)
			{
				return;
			}
			if (this.currentPlayerData.coresPendingOverdriveProcessing > 0)
			{
				int coresPendingOverdriveProcessing = this.currentPlayerData.coresPendingOverdriveProcessing;
				this.currentPlayerData.coresPendingOverdriveProcessing = 0;
				if (this.StationOpenForLocalPlayer)
				{
					this.localPlayerData.coresPendingOverdriveProcessing = 0;
				}
				this.overdrivePurchaseAnimationRoutine = base.StartCoroutine(this.OverdrivePurchaseAnimationVisual(coresPendingOverdriveProcessing));
				return;
			}
			this.processingAmount = this.currentPlayerData.coreProcessingPercentage;
			this.overdriveAmount = this.currentPlayerData.overdriveSupply;
			int num = Mathf.Clamp(this.currentPlayerData.coreCount, 0, this.maxVisualChaosSeedCount) - this.seedProcessingStates.Count;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.DepositSeedVisual();
				}
				return;
			}
			if (num < 0)
			{
				for (int j = 0; j > num; j--)
				{
					this.CompleteSeedVisual();
				}
				return;
			}
		}
		else
		{
			this.screenText.text = "Player Data Lookup Failed.";
			this.overdriveAmount = 0f;
			this.processingAmount = 0f;
		}
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x000F7F38 File Offset: 0x000F6138
	private void DepositSeedVisual()
	{
		for (int i = 0; i < this.chaosSeedVisuals.Count; i++)
		{
			if (!this.chaosSeedVisuals[i].activeSelf)
			{
				GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = new GRSeedExtractor.SeedProcessingVisualState
				{
					poolIndex = i,
					rollAngle = 0f,
					speed = 0f,
					rampProgress = 0f,
					dropProgress = 0f
				};
				this.seedProcessingStates.Add(seedProcessingVisualState);
				this.chaosSeedVisuals[i].SetActive(true);
				this.chaosSeedVisuals[i].transform.localPosition = this.seedTubeStart.localPosition;
				this.chaosSeedVisuals[i].transform.localRotation = Quaternion.identity;
				this.chaosSeedVisuals[i].transform.localScale = Vector3.one * this.seedVisualScaleRange.y;
				this.seedTubeAudioSource.PlayOneShot(this.seedMovementAudio, this.seedMovementVolume);
				return;
			}
		}
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x000F8058 File Offset: 0x000F6258
	private void CompleteSeedVisual()
	{
		if (this.seedProcessingStates.Count > 0)
		{
			GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = this.seedProcessingStates[0];
			this.chaosSeedVisuals[seedProcessingVisualState.poolIndex].SetActive(false);
			this.seedProcessingStates.RemoveAt(0);
		}
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x000F80A4 File Offset: 0x000F62A4
	private void ClearSeedVisuals()
	{
		int count = this.seedProcessingStates.Count;
		for (int i = 0; i < count; i++)
		{
			this.CompleteSeedVisual();
		}
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x000F80D0 File Offset: 0x000F62D0
	private void UpdateScreenDisplay()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		if (player == null || !this.stationOpen)
		{
			return;
		}
		int num = (int)this.estimatedJuiceTimeRemaining;
		if (this.currentPlayerActorNumber != this.currentDisplayData.playerActorNumber || this.currentPlayerData.coreCount != this.currentDisplayData.coreCount || this.currentPlayerData.overdriveSupply != this.currentDisplayData.overdriveSupply || this.currentPlayerData.researchPoints != this.currentDisplayData.researchPoints || num != this.currentDisplayData.juiceSecondsLeft)
		{
			this.currentDisplayData.playerActorNumber = this.currentPlayerActorNumber;
			this.currentDisplayData.coreCount = this.currentPlayerData.coreCount;
			this.currentDisplayData.overdriveSupply = this.currentPlayerData.overdriveSupply;
			this.currentDisplayData.researchPoints = this.currentPlayerData.researchPoints;
			this.currentDisplayData.juiceSecondsLeft = num;
			this.UpdateScreenSB.Clear();
			this.UpdateScreenSB.Append(player.SanitizedNickName + "\n");
			this.UpdateScreenSB.Append(string.Format("JUICE: <color=purple>⑮ {0}</color>\n\n", this.currentDisplayData.researchPoints));
			if (this.currentDisplayData.coreCount > 0)
			{
				this.UpdateScreenSB.Append(string.Format("Processing {0} Seeds", this.currentDisplayData.coreCount));
				int num2 = this.currentDisplayData.juiceSecondsLeft % 3;
				if (num2 == 2)
				{
					this.UpdateScreenSB.Append(".");
				}
				else if (num2 == 1)
				{
					this.UpdateScreenSB.Append("..");
				}
				else
				{
					this.UpdateScreenSB.Append("...");
				}
				int num3 = num / 3600;
				int num4 = num / 60 % 60;
				int num5 = num % 60;
				if (num3 > 0)
				{
					this.UpdateScreenSB.Append(string.Format("\nNext <color=purple>⑮</color> in {0}:{1:00}:{2:00}\n", num3, num4, num5));
				}
				else
				{
					this.UpdateScreenSB.Append(string.Format("\nNext <color=purple>⑮</color> in {0}:{1:00}\n", num4, num5));
				}
			}
			else
			{
				this.UpdateScreenSB.Append("Deposit Chaos Seed\nFor Juice Processing\n");
			}
			this.screenText.text = this.UpdateScreenSB.ToString();
		}
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x000F8334 File Offset: 0x000F6534
	private void StepSeedVisualAnimation(float dt)
	{
		float magnitude = (this.seedTubeStart.position - this.seedTubeEnd.position).magnitude;
		float num = magnitude / this.seedVisualRollTime;
		for (int i = 0; i < this.seedProcessingStates.Count; i++)
		{
			GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = this.seedProcessingStates[i];
			float num2 = 2f;
			if (i > 0)
			{
				num2 = this.seedProcessingStates[i - 1].rampProgress - 2f * this.visualChaosSeedRadius / magnitude;
			}
			if (seedProcessingVisualState.rampProgress < 1f)
			{
				GameObject gameObject = this.chaosSeedVisuals[seedProcessingVisualState.poolIndex];
				seedProcessingVisualState.speed = Mathf.MoveTowards(seedProcessingVisualState.speed, num, num * dt);
				float num3 = seedProcessingVisualState.speed * dt;
				float num4 = num3 / magnitude;
				seedProcessingVisualState.rampProgress = Mathf.Clamp01(seedProcessingVisualState.rampProgress + num4);
				if (seedProcessingVisualState.rampProgress >= num2)
				{
					seedProcessingVisualState.rampProgress = num2;
					seedProcessingVisualState.speed = 0f;
					num3 = 0f;
				}
				gameObject.transform.localPosition = Vector3.Lerp(this.seedTubeStart.localPosition, this.seedTubeEnd.localPosition, seedProcessingVisualState.rampProgress);
				seedProcessingVisualState.rollAngle += num3 / this.visualChaosSeedRadius;
				gameObject.transform.localRotation = Quaternion.AngleAxis(seedProcessingVisualState.rollAngle * 57.29578f, Vector3.forward);
			}
			if (i == 0 && seedProcessingVisualState.rampProgress >= 1f)
			{
				GameObject gameObject2 = this.chaosSeedVisuals[seedProcessingVisualState.poolIndex];
				if (seedProcessingVisualState.dropProgress < 1f)
				{
					seedProcessingVisualState.dropProgress += 1f / this.seedVisualDropTime * dt;
					seedProcessingVisualState.rampProgress = 1f + seedProcessingVisualState.dropProgress;
					float num5 = this.tubeEndToProcessingPathY.Evaluate(seedProcessingVisualState.dropProgress);
					float num6 = this.tubeEndToProcessingPathX.Evaluate(seedProcessingVisualState.dropProgress);
					Vector3 localPosition = gameObject2.transform.localPosition;
					localPosition.y = Mathf.Lerp(this.seedTubeEnd.localPosition.y, this.seedProcessingPosition.localPosition.y, num5);
					localPosition.x = Mathf.Lerp(this.seedTubeEnd.localPosition.x, this.seedProcessingPosition.localPosition.x, num6);
					gameObject2.transform.localPosition = localPosition;
					float num7 = seedProcessingVisualState.speed * dt;
					seedProcessingVisualState.rollAngle += num7 / this.visualChaosSeedRadius;
					gameObject2.transform.localRotation = Quaternion.AngleAxis(seedProcessingVisualState.rollAngle * 57.29578f, Vector3.forward);
					if (seedProcessingVisualState.dropProgress >= 1f)
					{
						this.juicerAudioSource.PlayOneShot(this.seedDropAudio, this.seedDropVolume);
					}
				}
				if (seedProcessingVisualState.dropProgress >= 1f && !this.drainingProcessingBeaker)
				{
					gameObject2.transform.localScale = Vector3.one * Mathf.Lerp(this.seedVisualScaleRange.y, this.seedVisualScaleRange.x, this.processingAmountVisual);
				}
			}
			this.seedProcessingStates[i] = seedProcessingVisualState;
		}
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x000F867F File Offset: 0x000F687F
	private IEnumerator OverdrivePurchaseAnimationVisual(int coresToProcess)
	{
		this.overdriveActive = true;
		this.overdriveBeepAudioSource.loop = true;
		this.overdriveBeepAudioSource.volume = this.overdriveBeepingVolume;
		this.overdriveBeepAudioSource.clip = this.overdriveBeepingAudio;
		this.overdriveBeepAudioSource.Play();
		int num = Math.Min(coresToProcess + this.currentPlayerData.coreCount, this.maxVisualChaosSeedCount);
		while (this.seedProcessingStates.Count < num)
		{
			this.DepositSeedVisual();
		}
		for (int j = 0; j < this.disableDuringOverdrive.Count; j++)
		{
			this.disableDuringOverdrive[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < this.enableDuringOverdrive.Count; k++)
		{
			this.enableDuringOverdrive[k].gameObject.SetActive(true);
		}
		this.overdriveMeterAudioSource.PlayOneShot(this.overdriveFillAudio, this.overdriveFillVolume);
		float overdriveFillRate = 1f / this.overdriveFillTime;
		float maxOverdriveFill = Mathf.Clamp01(this.currentPlayerData.overdriveSupply + (float)coresToProcess / (float)this.MAX_OVERDRIVE_USES);
		while (this.overdriveAmount < maxOverdriveFill)
		{
			this.overdriveAmount = Mathf.MoveTowards(this.overdriveAmount, maxOverdriveFill, overdriveFillRate * Time.deltaTime);
			yield return null;
		}
		this.overdriveMeterAudioSource.Stop();
		int num6;
		for (int i = 0; i < coresToProcess; i = num6)
		{
			float waitForSeedDepositStartTime = Time.time;
			bool flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			while (!flag && Time.time - waitForSeedDepositStartTime < 3f)
			{
				yield return null;
				flag = (this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f);
			}
			this.juicerAudioSource.PlayOneShot(this.seedJuicingAudio, this.seedJuicingVolume);
			this.juicerOverdriveParticles.gameObject.SetActive(true);
			float num2 = Mathf.Clamp01(1f - this.processingAmount);
			float timeToProcess = num2 * this.overdriveProcessTime;
			float startingProcessingAmount = this.processingAmount;
			float num3 = num2 / (float)this.MAX_OVERDRIVE_USES;
			float startingOverdrive = this.overdriveAmount;
			float resultingOverdrive = Mathf.Clamp01(this.overdriveAmount - num3);
			float timeProcessing = 0f;
			while (timeProcessing < timeToProcess)
			{
				timeProcessing += Time.deltaTime;
				float num4 = timeProcessing / timeToProcess;
				this.overdriveAmount = Mathf.Lerp(startingOverdrive, resultingOverdrive, num4);
				this.processingAmount = Mathf.Lerp(startingProcessingAmount, 1f, num4);
				this.estimatedJuiceTimeRemaining = timeToProcess - timeProcessing;
				yield return null;
			}
			this.CompleteSeedVisual();
			this.juicerOverdriveParticles.gameObject.SetActive(false);
			this.drainingProcessingBeaker = true;
			float timeDepositing = 0f;
			while (timeDepositing < this.juiceDepositTime)
			{
				timeDepositing += Time.deltaTime;
				float num5 = timeDepositing / this.juiceDepositTime;
				this.processingAmount = Mathf.Lerp(1f, 0f, num5);
				yield return null;
			}
			this.drainingProcessingBeaker = false;
			num6 = i + 1;
		}
		if (this.currentPlayerData.coresPendingOverdriveProcessing == 0 && this.currentPlayerData.coreCount == 1)
		{
			if (this.seedProcessingStates.Count == 0)
			{
				this.DepositSeedVisual();
			}
			float timeDepositing = Time.time;
			bool flag2 = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			while (!flag2 && Time.time - timeDepositing < 3f)
			{
				yield return null;
				flag2 = (this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f);
			}
			float timeProcessing = 0f;
			float resultingOverdrive = this.processingAmount;
			float startingOverdrive = this.overdriveAmount;
			float startingProcessingAmount = Mathf.Clamp01(this.currentPlayerData.coreProcessingPercentage - resultingOverdrive) * this.overdriveProcessTime;
			while (timeProcessing < startingProcessingAmount)
			{
				timeProcessing += Time.deltaTime;
				float num7 = timeProcessing / startingProcessingAmount;
				this.processingAmount = Mathf.Clamp01(Mathf.Lerp(resultingOverdrive, this.currentPlayerData.coreProcessingPercentage, num7));
				this.overdriveAmount = Mathf.Clamp01(Mathf.Lerp(startingOverdrive, this.currentPlayerData.overdriveSupply, num7));
				yield return null;
			}
		}
		for (int l = 0; l < this.disableDuringOverdrive.Count; l++)
		{
			this.disableDuringOverdrive[l].gameObject.SetActive(true);
		}
		for (int m = 0; m < this.enableDuringOverdrive.Count; m++)
		{
			this.enableDuringOverdrive[m].gameObject.SetActive(false);
		}
		this.overdriveLightSpinnerOff.localRotation = this.overdriveLightSpinnerOn.localRotation;
		this.overdriveBeepAudioSource.Stop();
		this.overdriveActive = false;
		if (this.StationOpenForLocalPlayer)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
		this.OnStateUpdated();
		yield break;
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x000F8698 File Offset: 0x000F6898
	public void OnResearchPointsUpdated()
	{
		int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
		if (numberOfResearchPoints > this.localPlayerData.researchPoints)
		{
			GRPlayer.GetLocal().SendJuiceCollectedTelemetry(numberOfResearchPoints - this.localPlayerData.researchPoints, this.localPlayerData.coresProcessedByOverdrive);
		}
		this.localPlayerData.researchPoints = numberOfResearchPoints;
		if (this.StationOpenForLocalPlayer)
		{
			bool flag = this.currentPlayerData.researchPoints != this.localPlayerData.researchPoints;
			this.currentPlayerData.researchPoints = this.localPlayerData.researchPoints;
			if (flag)
			{
				this.ghostReactor.grManager.RequestApplySeedExtractorState(this.localPlayerData.coreCount, this.localPlayerData.coresProcessedByOverdrive, this.localPlayerData.researchPoints, this.localPlayerData.coreProcessingPercentage, this.localPlayerData.overdriveSupply);
				this.OnStateUpdated();
			}
		}
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x000F8778 File Offset: 0x000F6978
	public void OnPurchaseOverdrive(bool success)
	{
		this.overdriveServerConfirmationPending = false;
		if (!success)
		{
			return;
		}
		GRPlayer.GetLocal().SendOverdrivePurchasedTelemetry(250, this.localPlayerData.coreCount);
	}

	// Token: 0x04003B58 RID: 15192
	private float PROCESSING_TIME_SECONDS = 600f;

	// Token: 0x04003B59 RID: 15193
	private int MAX_OVERDRIVE_USES = 6;

	// Token: 0x04003B5A RID: 15194
	[SerializeField]
	private GTZone zone;

	// Token: 0x04003B5B RID: 15195
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04003B5C RID: 15196
	[SerializeField]
	private TriggerEventNotifier coreDepositTriggerNotifier;

	// Token: 0x04003B5D RID: 15197
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x04003B5E RID: 15198
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x04003B5F RID: 15199
	[SerializeField]
	private GameObject chaosSeedVisualPrefab;

	// Token: 0x04003B60 RID: 15200
	[Header("Overdrive Purchase Buttons")]
	[SerializeField]
	private GorillaPressableButton overdrivePurchaseButton;

	// Token: 0x04003B61 RID: 15201
	[SerializeField]
	private GorillaPressableButton overdriveConfirmButton;

	// Token: 0x04003B62 RID: 15202
	[SerializeField]
	private Material defaultButtonMaterial;

	// Token: 0x04003B63 RID: 15203
	[SerializeField]
	private Material redButtonMaterial;

	// Token: 0x04003B64 RID: 15204
	[SerializeField]
	private Material greenButtonMaterial;

	// Token: 0x04003B65 RID: 15205
	[Header("Shutter Door Visual")]
	[SerializeField]
	private Transform shutterDoorParent;

	// Token: 0x04003B66 RID: 15206
	[SerializeField]
	private Vector2 shutterDoorLiftRange = new Vector2(1.245f, 2.07f);

	// Token: 0x04003B67 RID: 15207
	[SerializeField]
	private float shutterDoorAnimTime;

	// Token: 0x04003B68 RID: 15208
	[Header("Seed Processing Visual")]
	[SerializeField]
	private Transform processingLiquidScaleParent;

	// Token: 0x04003B69 RID: 15209
	[SerializeField]
	[Range(0f, 1f)]
	public float processingAmount;

	// Token: 0x04003B6A RID: 15210
	private float processingAmountVisual;

	// Token: 0x04003B6B RID: 15211
	[SerializeField]
	private Transform seedTubeStart;

	// Token: 0x04003B6C RID: 15212
	[SerializeField]
	private Transform seedTubeEnd;

	// Token: 0x04003B6D RID: 15213
	[SerializeField]
	private Transform seedProcessingPosition;

	// Token: 0x04003B6E RID: 15214
	[SerializeField]
	private AnimationCurve tubeEndToProcessingPathY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003B6F RID: 15215
	[SerializeField]
	private AnimationCurve tubeEndToProcessingPathX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003B70 RID: 15216
	[SerializeField]
	private float visualChaosSeedRadius = 1f;

	// Token: 0x04003B71 RID: 15217
	[SerializeField]
	private int maxVisualChaosSeedCount = 6;

	// Token: 0x04003B72 RID: 15218
	[SerializeField]
	private float seedVisualRollTime = 2f;

	// Token: 0x04003B73 RID: 15219
	[SerializeField]
	private float seedVisualDropTime = 0.5f;

	// Token: 0x04003B74 RID: 15220
	[SerializeField]
	private Vector2 seedVisualScaleRange = new Vector2(0.1f, 1.25f);

	// Token: 0x04003B75 RID: 15221
	[Header("Overdrive Visual")]
	[SerializeField]
	private Transform overdriveLiquidScaleParent;

	// Token: 0x04003B76 RID: 15222
	[SerializeField]
	private Transform overdriveLightSpinnerOff;

	// Token: 0x04003B77 RID: 15223
	[SerializeField]
	private Transform overdriveLightSpinnerOn;

	// Token: 0x04003B78 RID: 15224
	[SerializeField]
	private List<Transform> enableDuringOverdrive = new List<Transform>();

	// Token: 0x04003B79 RID: 15225
	[SerializeField]
	private List<Transform> disableDuringOverdrive = new List<Transform>();

	// Token: 0x04003B7A RID: 15226
	[SerializeField]
	private float overdriveLightSpinRate = 1f;

	// Token: 0x04003B7B RID: 15227
	[SerializeField]
	[Range(0f, 1f)]
	public float overdriveAmount;

	// Token: 0x04003B7C RID: 15228
	private float overdriveAmountVisual;

	// Token: 0x04003B7D RID: 15229
	[Header("VFX")]
	[SerializeField]
	private ParticleSystem depositorParticles;

	// Token: 0x04003B7E RID: 15230
	[SerializeField]
	private ParticleSystem juicerSlowParticles;

	// Token: 0x04003B7F RID: 15231
	[SerializeField]
	private ParticleSystem juicerOverdriveParticles;

	// Token: 0x04003B80 RID: 15232
	[Header("Audio")]
	[SerializeField]
	private AudioSource depositorAudioSource;

	// Token: 0x04003B81 RID: 15233
	[SerializeField]
	private AudioSource doorAudioSource;

	// Token: 0x04003B82 RID: 15234
	[SerializeField]
	private AudioSource seedTubeAudioSource;

	// Token: 0x04003B83 RID: 15235
	[SerializeField]
	private AudioSource juicerAudioSource;

	// Token: 0x04003B84 RID: 15236
	[SerializeField]
	private AudioSource machineHumAudioSource;

	// Token: 0x04003B85 RID: 15237
	[SerializeField]
	private AudioSource overdriveMeterAudioSource;

	// Token: 0x04003B86 RID: 15238
	[SerializeField]
	private AudioSource overdriveBeepAudioSource;

	// Token: 0x04003B87 RID: 15239
	[SerializeField]
	private AudioClip seedDepositAudio;

	// Token: 0x04003B88 RID: 15240
	[SerializeField]
	private float seedDepositVolume = 0.5f;

	// Token: 0x04003B89 RID: 15241
	[SerializeField]
	private AudioClip seedDepositFailedAudio;

	// Token: 0x04003B8A RID: 15242
	[SerializeField]
	private float seedDepositFailedVolume = 0.5f;

	// Token: 0x04003B8B RID: 15243
	[SerializeField]
	private AudioClip seedDepositAttemptAudio;

	// Token: 0x04003B8C RID: 15244
	[SerializeField]
	private float seedDepositAttemptVolume = 0.5f;

	// Token: 0x04003B8D RID: 15245
	[SerializeField]
	private AudioClip seedMovementAudio;

	// Token: 0x04003B8E RID: 15246
	[SerializeField]
	private float seedMovementVolume = 0.5f;

	// Token: 0x04003B8F RID: 15247
	[SerializeField]
	private AudioClip seedDropAudio;

	// Token: 0x04003B90 RID: 15248
	[SerializeField]
	private float seedDropVolume = 0.5f;

	// Token: 0x04003B91 RID: 15249
	[SerializeField]
	private AudioClip seedJuicingAudio;

	// Token: 0x04003B92 RID: 15250
	[SerializeField]
	private float seedJuicingVolume = 0.5f;

	// Token: 0x04003B93 RID: 15251
	[SerializeField]
	private AudioClip doorOpenAudio;

	// Token: 0x04003B94 RID: 15252
	[SerializeField]
	private float doorOpenVolume = 0.5f;

	// Token: 0x04003B95 RID: 15253
	[SerializeField]
	private AudioClip doorCloseAudio;

	// Token: 0x04003B96 RID: 15254
	[SerializeField]
	private float doorCloseVolume = 0.5f;

	// Token: 0x04003B97 RID: 15255
	[SerializeField]
	private AudioClip processingHumAudio;

	// Token: 0x04003B98 RID: 15256
	[SerializeField]
	private float processingHumVolume = 0.5f;

	// Token: 0x04003B99 RID: 15257
	[SerializeField]
	private AudioClip overdriveFillAudio;

	// Token: 0x04003B9A RID: 15258
	[SerializeField]
	private float overdriveFillVolume = 0.5f;

	// Token: 0x04003B9B RID: 15259
	[SerializeField]
	private AudioClip overdriveEngineAudio;

	// Token: 0x04003B9C RID: 15260
	[SerializeField]
	private float overdriveEngineVolume = 0.5f;

	// Token: 0x04003B9D RID: 15261
	[SerializeField]
	private AudioClip overdriveBeepingAudio;

	// Token: 0x04003B9E RID: 15262
	[SerializeField]
	private float overdriveBeepingVolume = 0.5f;

	// Token: 0x04003B9F RID: 15263
	private GRSeedExtractor.PlayerData localPlayerData;

	// Token: 0x04003BA0 RID: 15264
	private GRSeedExtractor.PlayerData currentPlayerData;

	// Token: 0x04003BA1 RID: 15265
	private GRSeedExtractor.ScreenDisplayData currentDisplayData;

	// Token: 0x04003BA2 RID: 15266
	private bool stationOpen;

	// Token: 0x04003BA3 RID: 15267
	private float stationOpenRequestTime;

	// Token: 0x04003BA4 RID: 15268
	private int currentPlayerActorNumber = -1;

	// Token: 0x04003BA5 RID: 15269
	private float shutterDoorOpenAmount;

	// Token: 0x04003BA6 RID: 15270
	private List<GameObject> chaosSeedVisuals = new List<GameObject>();

	// Token: 0x04003BA7 RID: 15271
	private bool overdrivePurchasePending;

	// Token: 0x04003BA8 RID: 15272
	private bool overdriveServerConfirmationPending;

	// Token: 0x04003BA9 RID: 15273
	private float overdrivePurchaseTime;

	// Token: 0x04003BAA RID: 15274
	private bool overdriveActive;

	// Token: 0x04003BAB RID: 15275
	private bool drainingProcessingBeaker;

	// Token: 0x04003BAC RID: 15276
	private float estimatedJuiceTimeRemaining;

	// Token: 0x04003BAD RID: 15277
	private float processingLiquidFollowRate = Mathf.Exp(2f);

	// Token: 0x04003BAE RID: 15278
	private List<ValueTuple<int, int, float, bool>> seedDepositsPending = new List<ValueTuple<int, int, float, bool>>();

	// Token: 0x04003BAF RID: 15279
	private Coroutine overdrivePurchaseAnimationRoutine;

	// Token: 0x04003BB0 RID: 15280
	private List<GRSeedExtractor.SeedProcessingVisualState> seedProcessingStates = new List<GRSeedExtractor.SeedProcessingVisualState>();

	// Token: 0x04003BB1 RID: 15281
	private float timeBetweenServerRequests = 3f;

	// Token: 0x04003BB2 RID: 15282
	private float lastServerRequestTime;

	// Token: 0x04003BB3 RID: 15283
	private GhostReactor ghostReactor;

	// Token: 0x04003BB4 RID: 15284
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04003BB5 RID: 15285
	private StringBuilder UpdateScreenSB = new StringBuilder(256);

	// Token: 0x04003BB6 RID: 15286
	[Header("Debug Animation")]
	public int debugSeedCount;

	// Token: 0x04003BB7 RID: 15287
	public float debugSeedProcessingTime = 10f;

	// Token: 0x04003BB8 RID: 15288
	public float overdriveFillTime = 2f;

	// Token: 0x04003BB9 RID: 15289
	public float overdriveProcessTime = 1.5f;

	// Token: 0x04003BBA RID: 15290
	public float juiceDepositTime = 0.75f;

	// Token: 0x020006F7 RID: 1783
	public struct PlayerData
	{
		// Token: 0x04003BBB RID: 15291
		public int actorNumber;

		// Token: 0x04003BBC RID: 15292
		public int coreCount;

		// Token: 0x04003BBD RID: 15293
		public float coreProcessingPercentage;

		// Token: 0x04003BBE RID: 15294
		public float overdriveSupply;

		// Token: 0x04003BBF RID: 15295
		public int coresProcessedByOverdrive;

		// Token: 0x04003BC0 RID: 15296
		public int coresPendingOverdriveProcessing;

		// Token: 0x04003BC1 RID: 15297
		public int researchPoints;

		// Token: 0x04003BC2 RID: 15298
		public float latestRefreshTime;
	}

	// Token: 0x020006F8 RID: 1784
	private struct ScreenDisplayData
	{
		// Token: 0x04003BC3 RID: 15299
		public int playerActorNumber;

		// Token: 0x04003BC4 RID: 15300
		public int coreCount;

		// Token: 0x04003BC5 RID: 15301
		public float overdriveSupply;

		// Token: 0x04003BC6 RID: 15302
		public int researchPoints;

		// Token: 0x04003BC7 RID: 15303
		public int juiceSecondsLeft;
	}

	// Token: 0x020006F9 RID: 1785
	private struct SeedProcessingVisualState
	{
		// Token: 0x04003BC8 RID: 15304
		public int poolIndex;

		// Token: 0x04003BC9 RID: 15305
		public float speed;

		// Token: 0x04003BCA RID: 15306
		public float rollAngle;

		// Token: 0x04003BCB RID: 15307
		public float rampProgress;

		// Token: 0x04003BCC RID: 15308
		public float dropProgress;
	}
}
