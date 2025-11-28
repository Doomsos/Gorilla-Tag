using System;
using System.Text;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000742 RID: 1858
public class GRUIPromotionBot : MonoBehaviourTick
{
	// Token: 0x06002FFC RID: 12284 RVA: 0x00106268 File Offset: 0x00104468
	public string FormattedUserInfo()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return "ERROR";
		}
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(grplayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
		int num = 1000 + grplayer.ShiftCreditCapIncreases * 100;
		int num2 = grplayer.CurrentProgression.points - grplayer.CurrentProgression.redeemedPoints + item2;
		string text = (player != null) ? player.SanitizedNickName : "RANDO MONKE";
		this.cachedStringBuilder.Clear();
		this.cachedStringBuilder.Append("<color=#808080>EMPLOYEE:</color>     " + text + "\n");
		this.cachedStringBuilder.Append("<color=#808080>TITLE:</color>        " + titleNameAndGrade + "\n");
		this.cachedStringBuilder.Append(string.Format("<color=#808080>XP:</color>           {0}/{1}\n", num2, item));
		if (grplayer == GRPlayer.GetLocal())
		{
			this.cachedStringBuilder.Append(string.Format("<color=#808080>CREDITS:</color>      <color=#00ff00>⑭ {0}</color>\n", grplayer.ShiftCredits));
			this.cachedStringBuilder.Append(string.Format("<color=#808080>CREDIT LIMIT:</color> <color=#00a000>⑭ {0}</color>\n", num));
			if (this.reactor != null && this.reactor.toolProgression != null)
			{
				int numberOfResearchPoints = this.reactor.toolProgression.GetNumberOfResearchPoints();
				this.cachedStringBuilder.Append(string.Format("<color=#808080>JUICE:</color>        <color=purple>⑮ {0}</color>\n", numberOfResearchPoints));
			}
			if (ProgressionManager.Instance != null)
			{
				int shinyRocksTotal = ProgressionManager.Instance.GetShinyRocksTotal();
				this.cachedStringBuilder.Append(string.Format("<color=#808080>SHINY ROCKS:</color>  <color=white>⑯ {0}</color>\n", shinyRocksTotal));
			}
		}
		return this.cachedStringBuilder.ToString();
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x0010645C File Offset: 0x0010465C
	public bool ActivePlayerEligibleForPromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return false;
		}
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(grplayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		return item - item2 < grplayer.CurrentProgression.points - grplayer.CurrentProgression.redeemedPoints;
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x001064BC File Offset: 0x001046BC
	public void Init(GhostReactor _reactor)
	{
		this.reactor = _reactor;
		this.currentPlayerActorNumber = -1;
		this.currentState = GRUIPromotionBot.PromotionBotState.WaitingForLogin;
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x001064D3 File Offset: 0x001046D3
	public void Refresh()
	{
		this.RefreshPlayerData();
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x001064DC File Offset: 0x001046DC
	public override void Tick()
	{
		if (this.reactor == null || this.reactor.grManager == null || !this.reactor.grManager.IsAuthority())
		{
			return;
		}
		float time = Time.time;
		if (this.currentPlayerActorNumber != -1 && (this.timeLastDistanceCheck > time || time > this.timeLastDistanceCheck + this.timeBetweenDistanceChecks))
		{
			GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
			if (grplayer == null || (base.transform.position - grplayer.transform.position).sqrMagnitude > this.distanceForAutoLogout * this.distanceForAutoLogout)
			{
				this.SwitchState(GRUIPromotionBot.PromotionBotState.WaitingForLogin, false);
			}
		}
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x00106594 File Offset: 0x00104794
	public bool CheckIsActivePlayer()
	{
		Object @object = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		return @object == grplayer;
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x001065C0 File Offset: 0x001047C0
	public void UpPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState != GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease)
		{
			if (promotionBotState == GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits)
			{
				this.SwitchState(GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease, false);
				return;
			}
		}
		else
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePromotion, false);
		}
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x001065F8 File Offset: 0x001047F8
	public void DownPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState == GRUIPromotionBot.PromotionBotState.ChoosePromotion)
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease, false);
			return;
		}
		if (promotionBotState != GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease)
		{
			return;
		}
		this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x00106630 File Offset: 0x00104830
	public void YesPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		switch (this.currentState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			this.AttemptPromotion();
			return;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			this.AttemptPurchaseShiftCreditIncrease();
			return;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits, false);
			return;
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x00106688 File Offset: 0x00104888
	public void NoPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState - GRUIPromotionBot.PromotionBotState.ChoosePromotion > 2)
		{
			if (promotionBotState == GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits)
			{
				this.AttemptPurchaseShiftCreditRefillToMax();
				return;
			}
		}
		else
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.WaitingForLogin, false);
		}
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x001066C0 File Offset: 0x001048C0
	public void SwitchState(GRUIPromotionBot.PromotionBotState newState, bool fromRPC = false)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer2 == null)
		{
			return;
		}
		this.RefreshPlayerData();
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		this.currentState = newState;
		this.SetScreenVisibility();
		this.SetMenuText(newState);
		switch (newState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			if (this.ActivePlayerEligibleForPromotion())
			{
				this.descriptionText.text = "<color=#c0c0c0>     YOU ARE ELIGIBLE FOR A PROMOTION!\n     PRESS 'YES' TO CONTINUE</color>";
			}
			else
			{
				this.descriptionText.text = "<color=#c04040>     YOU ARE NOT ELIGIBLE FOR A PROMOTION\n     EARN MORE XP BY COMPLETING SHIFT GOALS</color>";
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			if (grplayer.ShiftCreditCapIncreases != grplayer.ShiftCreditCapIncreasesMax)
			{
				this.descriptionText.text = "<color=#c0c0c0>     INCREASE CREDIT LIMIT BY <color=#00ff00>⑭ 100</color>\n     FOR <color=purple>⑮ 2</color> JUICE?</color>";
			}
			else
			{
				this.descriptionText.text = "<color=#c0c0c0>     CREDIT LIMIT CAN'T BE INCREASED AT THIS TIME\n</color>";
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
			if (grplayer == null)
			{
				this.descriptionText.text = "No active player";
			}
			else
			{
				int purchaseToCreditCapAmount = this.GetPurchaseToCreditCapAmount();
				if (purchaseToCreditCapAmount > 0)
				{
					this.descriptionText.text = string.Format("<color=#c0c0c0>     PURCHASE <color=#00ff00>+⑭{0}</color> CREDITS\n     FOR <color=white>100 SHINY ROCKS?</color>", purchaseToCreditCapAmount);
				}
				else
				{
					this.descriptionText.text = "<color=#c0c0c0>     YOU ARE AT FULL CREDITS";
				}
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
		{
			int purchaseToCreditCapAmount2 = this.GetPurchaseToCreditCapAmount();
			this.descriptionText.text = string.Format("<color=#c0c0c0>     CONFIRM PURCHASE <color=#00ff00>+⑭{0}</color>\n     FOR <color=white>100 SHINY ROCKS?</color>", purchaseToCreditCapAmount2);
			break;
		}
		}
		if (this.currentState == GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits)
		{
			this.yesText.text = "<size=0.4>CANCEL</size>";
			this.noText.text = "<size=0.4>CONFIRM</size>";
		}
		else
		{
			if (this.yesText.text != "YES")
			{
				this.yesText.text = "YES";
			}
			if (this.noText.text != "NO")
			{
				this.noText.text = "NO";
			}
		}
		if (this.reactor != null && this.reactor.grManager != null && !fromRPC && (grplayer == grplayer2 || this.reactor.grManager.IsAuthority()))
		{
			this.reactor.grManager.PromotionBotActivePlayerRequest((int)this.currentState);
		}
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x001068E0 File Offset: 0x00104AE0
	public int GetPurchaseToCreditCapAmount()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		int shiftCredits = grplayer.ShiftCredits;
		int num = 1000 + grplayer.ShiftCreditCapIncreases * 100;
		return Math.Max(0, num - shiftCredits);
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x0010691C File Offset: 0x00104B1C
	public void CelebratePromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		this.particlesGO.SetActive(false);
		this.particlesGO.SetActive(true);
		this.levelUpSound.Play();
		this.popSound.Play();
		PlayerGameEvents.MiscEvent(GRUIPromotionBot.EVENT_PROMOTED, 1);
		grplayer.SendRankUpTelemetry(GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints));
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x00106990 File Offset: 0x00104B90
	public void SetMenuText(GRUIPromotionBot.PromotionBotState menuState)
	{
		switch (menuState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			this.menuText.text = "-> REQUEST PROMOTION\n   INCREASE CREDIT LIMIT\n   BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			this.menuText.text = "   REQUEST PROMOTION\n-> INCREASE CREDIT LIMIT\n   BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
			this.menuText.text = "   REQUEST PROMOTION\n   INCREASE CREDIT LIMIT\n-> BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		default:
			return;
		}
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x001069E8 File Offset: 0x00104BE8
	public void SetScreenVisibility()
	{
		this.startScreenText.gameObject.SetActive(this.currentState == GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.userInfo.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.menuText.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.descriptionText.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.purchaseSuccessText.gameObject.SetActive(false);
	}

	// Token: 0x0600300B RID: 12299 RVA: 0x00106A6A File Offset: 0x00104C6A
	public void RefreshPlayerData()
	{
		this.userInfo.text = this.FormattedUserInfo();
	}

	// Token: 0x0600300C RID: 12300 RVA: 0x00106A80 File Offset: 0x00104C80
	public void OnPurchaseCallback(bool success)
	{
		if (success)
		{
			this.purchaseSuccessText.text = "<color=#80ff80>     PURCHASE SUCCEEDED!</color>";
			this.RefreshPlayerData();
			this.purchaseSuccessText.gameObject.SetActive(true);
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded == null)
			{
				return;
			}
			onSucceeded.Invoke();
			return;
		}
		else
		{
			this.purchaseSuccessText.text = "<color=#ff8080>     FAILED PURCHASE. NO CHARGE.</color>";
			this.RefreshPlayerData();
			this.purchaseSuccessText.gameObject.SetActive(true);
			UnityEvent onFailed = this.scanner.onFailed;
			if (onFailed == null)
			{
				return;
			}
			onFailed.Invoke();
			return;
		}
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x001064D3 File Offset: 0x001046D3
	public void OnJuiceUpdated()
	{
		this.RefreshPlayerData();
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x00106B0C File Offset: 0x00104D0C
	public void OnGetShiftCredit(string mothershipId, int credit)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.mothershipId == mothershipId)
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x00106B44 File Offset: 0x00104D44
	public void OnShinyRocksUpdated()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.gamePlayer.IsLocal())
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x00106B7C File Offset: 0x00104D7C
	public void OnGetShiftCreditCapData(string mothershipId, int creditCap, int creditCapMax)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.mothershipId == mothershipId)
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x00106BB4 File Offset: 0x00104DB4
	public void AttemptPromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer && grplayer.AttemptPromotion() && this.reactor != null && this.reactor.grManager != null)
		{
			this.CelebratePromotion();
			this.RefreshPlayerData();
			this.RefreshActivePlayerBadge();
			string titleName = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			int grade = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints);
			this.purchaseSuccessText.text = string.Format("CONGRATULATIONS, {0} {1}!", titleName, grade);
			this.purchaseSuccessText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x00106C68 File Offset: 0x00104E68
	public void AttemptPurchaseShiftCreditIncrease()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			Debug.Log("AttemptPurchaseShiftCreditIncrease currentPlayer null");
			return;
		}
		if (grplayer.ShiftCreditCapIncreases == grplayer.ShiftCreditCapIncreasesMax)
		{
			return;
		}
		Debug.Log(string.Format("AttemptPurchaseShiftCreditIncrease currentPlayer ShiftCreditCapIncreases {0} ShiftCreditCapIncreasesMax {1}", grplayer.ShiftCreditCapIncreases, grplayer.ShiftCreditCapIncreasesMax));
		int num = 2;
		if (grplayer != null && grplayer.gamePlayer.IsLocal() && grplayer.ShiftCreditCapIncreases < grplayer.ShiftCreditCapIncreasesMax && this.reactor.toolProgression.GetNumberOfResearchPoints() >= num && ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.PurchaseShiftCreditCapIncrease();
		}
		this.RefreshPlayerData();
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x00106D24 File Offset: 0x00104F24
	public void AttemptPurchaseShiftCreditRefillToMax()
	{
		if (this.GetPurchaseToCreditCapAmount() == 0)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			Debug.Log("AttemptPurchaseShiftCreditIncrease currentPlayer null");
			return;
		}
		int num = 1000;
		int num2 = 100;
		int num3 = num + grplayer.ShiftCreditCapIncreases * num2;
		Debug.Log(string.Format("AttemptPurchaseShiftCreditIncrease currentPlayer ShiftCredits {0} ShiftCreditMax {1}", grplayer.ShiftCredits, num3));
		if (grplayer != null && grplayer.gamePlayer.IsLocal() && grplayer.ShiftCredits < num3)
		{
			int num4 = 100;
			if (ProgressionManager.Instance != null && ProgressionManager.Instance.GetShinyRocksTotal() >= num4)
			{
				ProgressionManager.Instance.PurchaseShiftCredit();
			}
		}
		this.RefreshPlayerData();
		this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x00106DE0 File Offset: 0x00104FE0
	public void PlayerSwipedID()
	{
		if (this.reactor == null || this.reactor.grManager == null)
		{
			return;
		}
		if (this.currentPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded == null)
			{
				return;
			}
			onSucceeded.Invoke();
			return;
		}
		else if (this.currentPlayerActorNumber != -1 && GRPlayer.Get(this.currentPlayerActorNumber) != null)
		{
			UnityEvent onFailed = this.scanner.onFailed;
			if (onFailed == null)
			{
				return;
			}
			onFailed.Invoke();
			return;
		}
		else
		{
			this.reactor.grManager.PromotionBotActivePlayerRequest(6);
			UnityEvent onSucceeded2 = this.scanner.onSucceeded;
			if (onSucceeded2 == null)
			{
				return;
			}
			onSucceeded2.Invoke();
			return;
		}
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x00106E90 File Offset: 0x00105090
	public void RefreshActivePlayerBadge()
	{
		if (this.currentPlayerActorNumber == -1)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && this.currentPlayerActorNumber != -1)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(this.currentPlayerActorNumber);
			if (netPlayerByID != null && grplayer.badge != null)
			{
				grplayer.badge.RefreshText(netPlayerByID);
			}
		}
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x00106EF4 File Offset: 0x001050F4
	public void SetActivePlayerStateChange(int actorNumber, int state)
	{
		if (state == 0)
		{
			this.RefreshActivePlayerBadge();
			actorNumber = -1;
		}
		bool flag = this.currentPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		bool flag2 = actorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		if (flag && !flag2)
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnPurchaseShiftCredit -= new Action<bool>(this.OnPurchaseCallback);
				ProgressionManager.Instance.OnPurchaseShiftCreditCapIncrease -= new Action<bool>(this.OnPurchaseCallback);
				ProgressionManager.Instance.OnInventoryUpdated -= new Action(this.OnJuiceUpdated);
				ProgressionManager.Instance.OnGetShiftCredit -= new Action<string, int>(this.OnGetShiftCredit);
				ProgressionManager.Instance.OnGetShiftCreditCapData -= new Action<string, int, int>(this.OnGetShiftCreditCapData);
			}
			if (CosmeticsController.instance != null)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnShinyRocksUpdated));
			}
		}
		else if (!flag && flag2)
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnPurchaseShiftCredit += new Action<bool>(this.OnPurchaseCallback);
				ProgressionManager.Instance.OnPurchaseShiftCreditCapIncrease += new Action<bool>(this.OnPurchaseCallback);
				ProgressionManager.Instance.OnInventoryUpdated += new Action(this.OnJuiceUpdated);
				ProgressionManager.Instance.OnGetShiftCredit += new Action<string, int>(this.OnGetShiftCredit);
				ProgressionManager.Instance.OnGetShiftCreditCapData += new Action<string, int, int>(this.OnGetShiftCreditCapData);
			}
			if (CosmeticsController.instance != null)
			{
				CosmeticsController instance2 = CosmeticsController.instance;
				instance2.OnGetCurrency = (Action)Delegate.Combine(instance2.OnGetCurrency, new Action(this.OnShinyRocksUpdated));
			}
		}
		this.currentPlayerActorNumber = actorNumber;
		this.SwitchState((GRUIPromotionBot.PromotionBotState)state, true);
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x001070C0 File Offset: 0x001052C0
	public int GetCurrentPlayerActorNumber()
	{
		return this.currentPlayerActorNumber;
	}

	// Token: 0x04003EEF RID: 16111
	private static string EVENT_PROMOTED = "GRPromoted";

	// Token: 0x04003EF0 RID: 16112
	private GhostReactor reactor;

	// Token: 0x04003EF1 RID: 16113
	public TMP_Text startScreenText;

	// Token: 0x04003EF2 RID: 16114
	public TMP_Text userInfo;

	// Token: 0x04003EF3 RID: 16115
	public TMP_Text menuText;

	// Token: 0x04003EF4 RID: 16116
	public TMP_Text descriptionText;

	// Token: 0x04003EF5 RID: 16117
	public TMP_Text yesText;

	// Token: 0x04003EF6 RID: 16118
	public TMP_Text noText;

	// Token: 0x04003EF7 RID: 16119
	public TMP_Text purchaseSuccessText;

	// Token: 0x04003EF8 RID: 16120
	public IDCardScanner scanner;

	// Token: 0x04003EF9 RID: 16121
	public GameObject particlesGO;

	// Token: 0x04003EFA RID: 16122
	public AudioSource levelUpSound;

	// Token: 0x04003EFB RID: 16123
	public AudioSource popSound;

	// Token: 0x04003EFC RID: 16124
	private string defaultText = "-N/A-\n-N/A-\n-N/A-\n-N/A-\n-N/A-\n\n-N/A-";

	// Token: 0x04003EFD RID: 16125
	private string promotionTextStr1 = "CONGRATULATIONS\n ";

	// Token: 0x04003EFE RID: 16126
	private string promotionTextStr2 = ".\n\nYOU ARE NOW A GRADE ";

	// Token: 0x04003EFF RID: 16127
	private string promotionTextStr3 = ".\n\nYOU MAY TAKE TWO UNPAID MINUTES TO CELEBRATE, THEN RETURN TO WORK.";

	// Token: 0x04003F00 RID: 16128
	private string inertButtonText = "-";

	// Token: 0x04003F01 RID: 16129
	private string buttonReturnText = "-RETURN-";

	// Token: 0x04003F02 RID: 16130
	private string requestPromotionText = "REQUEST PROMOTION";

	// Token: 0x04003F03 RID: 16131
	public const string newLine = "\n";

	// Token: 0x04003F04 RID: 16132
	public int currentPlayerActorNumber;

	// Token: 0x04003F05 RID: 16133
	public GRUIPromotionBot.PromotionBotState currentState;

	// Token: 0x04003F06 RID: 16134
	public float timeOutTime;

	// Token: 0x04003F07 RID: 16135
	public float distanceForAutoLogout = 2.5f;

	// Token: 0x04003F08 RID: 16136
	private StringBuilder cachedStringBuilder = new StringBuilder(512);

	// Token: 0x04003F09 RID: 16137
	private float timeLastDistanceCheck;

	// Token: 0x04003F0A RID: 16138
	private float timeBetweenDistanceChecks = 0.5f;

	// Token: 0x02000743 RID: 1859
	public enum PromotionBotState
	{
		// Token: 0x04003F0C RID: 16140
		WaitingForLogin,
		// Token: 0x04003F0D RID: 16141
		ChoosePromotion,
		// Token: 0x04003F0E RID: 16142
		ChooseCreditIncrease,
		// Token: 0x04003F0F RID: 16143
		ChoosePurchaseCredits,
		// Token: 0x04003F10 RID: 16144
		ConfirmPurchaseCredits,
		// Token: 0x04003F11 RID: 16145
		CelebratePromotion,
		// Token: 0x04003F12 RID: 16146
		TryingLogIn
	}
}
