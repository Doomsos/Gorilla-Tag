using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004BE RID: 1214
public class ATM_Manager : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06001F4C RID: 8012 RVA: 0x000A59ED File Offset: 0x000A3BED
	public ATM_Manager.ATMStages CurrentATMStage
	{
		get
		{
			return this.currentATMStage;
		}
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x000A59F8 File Offset: 0x000A3BF8
	public void Awake()
	{
		if (ATM_Manager.instance)
		{
			Object.Destroy(this);
		}
		else
		{
			ATM_Manager.instance = this;
		}
		string defaultResult = "CREATOR CODE: ";
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out text, defaultResult))
		{
			Debug.LogError("[LOCALIZATION::ATM_MANAGER] Failed to get key for [ATM_CREATOR_CODE]");
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = text;
		}
		this.SwitchToStage(ATM_Manager.ATMStages.Unavailable);
		this.smallDisplays = new List<CreatorCodeSmallDisplay>();
		this.HookupToCreatorCodes();
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000A5AA4 File Offset: 0x000A3CA4
	public void Start()
	{
		Debug.Log("ATM COUNT: " + this.atmUIs.Count.ToString());
		Debug.Log("SMALL DISPLAY COUNT: " + this.smallDisplays.Count.ToString());
		GameEvents.OnGorrillaATMKeyButtonPressedEvent.AddListener(new UnityAction<GorillaATMKeyBindings>(this.PressButton));
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000A5B0C File Offset: 0x000A3D0C
	public void HookupToCreatorCodes()
	{
		CreatorCodes.InitializedEvent += new Action(this.CreatorCodesInitialized);
		CreatorCodes.OnCreatorCodeChangedEvent += new Action<string>(this.OnCreatorCodeChanged);
		CreatorCodes.OnCreatorCodeFailureEvent += new Action<string>(this.OnOnCreatorCodeFailureEvent);
		if (CreatorCodes.Intialized)
		{
			this.CreatorCodesInitialized();
		}
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x000A5B5C File Offset: 0x000A3D5C
	public void CreatorCodesInitialized()
	{
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode("atm_terminal_id"));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode("atm_terminal_id");
		}
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000A5C04 File Offset: 0x000A3E04
	public void OnCreatorCodeChanged(string id)
	{
		if (id != "atm_terminal_id")
		{
			return;
		}
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode("atm_terminal_id"));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode("atm_terminal_id");
		}
		string text = "CREATOR CODE:";
		CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus("atm_terminal_id");
		if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
		{
			if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
			{
				text += " VALID";
			}
		}
		else
		{
			text += " VALIDATING";
		}
		foreach (ATM_UI atm_UI2 in this.atmUIs)
		{
			atm_UI2.creatorCodeTitle.text = text;
		}
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000A5D34 File Offset: 0x000A3F34
	private void OnOnCreatorCodeFailureEvent(string id)
	{
		if (id != "atm_terminal_id")
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: INVALID";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE_INVALID", out text, atm_UI.atmText.text);
			atm_UI.creatorCodeTitle.text = text;
		}
		Debug.Log("ATM CODE FAILURE");
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000A5DCC File Offset: 0x000A3FCC
	public void OnCreatorCodeInvalid(string id)
	{
		if (id != "atm_terminal_id")
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000A5E34 File Offset: 0x000A4034
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x000A5E53 File Offset: 0x000A4053
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x000A5E66 File Offset: 0x000A4066
	private void OnLanguageChanged()
	{
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x000A5E74 File Offset: 0x000A4074
	public void PressButton(GorillaATMKeyBindings buttonPressed)
	{
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm && CreatorCodes.getCurrentCreatorCodeStatus("atm_terminal_id") != CreatorCodes.CreatorCodeStatus.Validating)
		{
			string defaultResult = "CREATOR CODE: ";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out text, defaultResult);
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.creatorCodeTitle.text = text;
			}
			if (buttonPressed == GorillaATMKeyBindings.delete)
			{
				CreatorCodes.DeleteCharacter("atm_terminal_id");
				return;
			}
			string id = "atm_terminal_id";
			string input;
			if (buttonPressed >= GorillaATMKeyBindings.delete)
			{
				input = buttonPressed.ToString();
			}
			else
			{
				int num = (int)buttonPressed;
				input = num.ToString();
			}
			CreatorCodes.AppendKey(id, input);
		}
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000A5F34 File Offset: 0x000A4134
	public void ProcessATMState(string currencyButton)
	{
		ATM_Manager.<ProcessATMState>d__55 <ProcessATMState>d__;
		<ProcessATMState>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ProcessATMState>d__.<>4__this = this;
		<ProcessATMState>d__.currencyButton = currencyButton;
		<ProcessATMState>d__.<>1__state = -1;
		<ProcessATMState>d__.<>t__builder.Start<ATM_Manager.<ProcessATMState>d__55>(ref <ProcessATMState>d__);
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000A5F73 File Offset: 0x000A4173
	public void AddATM(ATM_UI newATM)
	{
		this.atmUIs.Add(newATM);
		newATM.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode("atm_terminal_id");
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000A5FA2 File Offset: 0x000A41A2
	public void RemoveATM(ATM_UI atmToRemove)
	{
		this.atmUIs.Remove(atmToRemove);
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x000A5FB4 File Offset: 0x000A41B4
	public void CreatorCodeValidating()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x000A6010 File Offset: 0x000A4210
	public void CreatorCodeValid()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm)
		{
			this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
		}
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000A607C File Offset: 0x000A427C
	public void SwitchToStage(ATM_Manager.ATMStages newStage)
	{
		this.currentATMStage = newStage;
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			if (atm_UI.atmText)
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				switch (newStage)
				{
				case ATM_Manager.ATMStages.Unavailable:
					atm_UI.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_NOT_AVAILABLE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Begin:
					atm_UI.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_STARTUP", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_BEGIN", out text5, "BEGIN");
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Menu:
					if (PlayFabAuthenticator.instance.GetSafety())
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHECK_YOUR_BALANCE", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = "";
						atm_UI.ATM_RightColumnArrowText[1].enabled = false;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.creatorCodeObject.SetActive(false);
					}
					else
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_MAIN_SCREEN", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE", out text3, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = text3;
						atm_UI.ATM_RightColumnArrowText[1].enabled = true;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.creatorCodeObject.SetActive(false);
					}
					break;
				case ATM_Manager.ATMStages.Balance:
					atm_UI.atmText.text = "CURRENT BALANCE:\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CURRENT_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + "\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Choose:
				{
					string defaultResult = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}";
					string defaultResult2 = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}\r\n({discount}% BONUS!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_FIRST", out text2, defaultResult);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text3, defaultResult2);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text4, defaultResult2);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text5, defaultResult2);
					text2 = text2.Replace("{numShinyRocksToBuy}", "1000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "4.99");
					text3 = text3.Replace("{numShinyRocksToBuy}", "2200").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "9.99").Replace("{discount}", "10");
					text4 = text4.Replace("{numShinyRocksToBuy}", "5000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "19.99").Replace("{discount}", "25");
					text5 = text5.Replace("{numShinyRocksToBuy}", "11000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "39.99").Replace("{discount}", "37");
					atm_UI.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHOOSE_PURCHASE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = text3;
					atm_UI.ATM_RightColumnArrowText[1].enabled = true;
					atm_UI.ATM_RightColumnButtonText[2].text = text4;
					atm_UI.ATM_RightColumnArrowText[2].enabled = true;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				}
				case ATM_Manager.ATMStages.Confirm:
					atm_UI.atmText.text = string.Concat(new string[]
					{
						"YOU HAVE CHOSEN TO PURCHASE ",
						this.numShinyRocksToBuy.ToString(),
						" SHINY ROCKS FOR $",
						this.shinyRocksCost.ToString(),
						". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
					});
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CONFIRMATION_STEAM", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CONFIRM", out text2, "CONFIRM");
					text = text.Replace("{numShinyRocksToBuy}", this.numShinyRocksToBuy.ToString());
					text = text.Replace("{currencySymbol}", "$");
					text = text.Replace("{shinyRocksCost}", this.shinyRocksCost.ToString());
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(true);
					break;
				case ATM_Manager.ATMStages.Purchasing:
					atm_UI.atmText.text = "PURCHASING IN STEAM...";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Success:
					atm_UI.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_SUCCESS_NEW_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					if (CreatorCodes.getCurrentCreatorCodeStatus("atm_terminal_id") == CreatorCodes.CreatorCodeStatus.Valid)
					{
						string name = CreatorCodes.supportedMember.name;
						if (!string.IsNullOrEmpty(name))
						{
							TMP_Text atmText = atm_UI.atmText;
							atmText.text = atmText.text + "\n\nTHIS PURCHASE SUPPORTED\n" + name + "!";
							foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
							{
								creatorCodeSmallDisplay.SuccessfulPurchase(name);
							}
						}
					}
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Failure:
					atm_UI.atmText.text = "PURCHASE CANCELLED. NO FUNDS WERE SPENT.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CANCELLED", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.SafeAccount:
					atm_UI.atmText.text = "Out Of Order.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING_DISABLED_OUT_OF_ORDER", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				}
			}
		}
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000A6C54 File Offset: 0x000A4E54
	public void SetATMText(string newText)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.atmText.text = newText;
		}
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000A6CAC File Offset: 0x000A4EAC
	public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
	{
		this.ProcessATMState(currencyPurchaseSize);
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x00002789 File Offset: 0x00000989
	public void LeaveSystemMenu()
	{
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000A6CB5 File Offset: 0x000A4EB5
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusGroups.Length == 0)
		{
			Debug.LogError("You have to set at least one nexusGroup in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000A6CE0 File Offset: 0x000A4EE0
	internal void SetTemporaryCreatorCode(string code)
	{
		if (code == null)
		{
			CreatorCodes.ResetCreatorCode("atm_terminal_id");
			CreatorCodes.AppendKey("atm_terminal_id", this._tempCreatorCodeOveride);
			this._tempCreatorCodeOveride = null;
			return;
		}
		if (this._tempCreatorCodeOveride == null)
		{
			this._tempCreatorCodeOveride = CreatorCodes.getCurrentCreatorCode("atm_terminal_id");
		}
		CreatorCodes.ResetCreatorCode("atm_terminal_id");
		CreatorCodes.AppendKey("atm_terminal_id", code);
	}

	// Token: 0x0400298E RID: 10638
	private const string ATM_STARTUP_KEY = "ATM_STARTUP";

	// Token: 0x0400298F RID: 10639
	private const string ATM_SCREEN_KEY = "ATM_SCREEN";

	// Token: 0x04002990 RID: 10640
	private const string ATM_NOT_AVAILABLE_KEY = "ATM_NOT_AVAILABLE";

	// Token: 0x04002991 RID: 10641
	private const string ATM_BEGIN_KEY = "ATM_BEGIN";

	// Token: 0x04002992 RID: 10642
	private const string ATM_MAIN_SCREEN_KEY = "ATM_MAIN_SCREEN";

	// Token: 0x04002993 RID: 10643
	private const string ATM_CHECK_YOUR_BALANCE_KEY = "ATM_CHECK_YOUR_BALANCE";

	// Token: 0x04002994 RID: 10644
	private const string ATM_PURCHASING_DISABLED_OUT_OF_ORDER_KEY = "ATM_PURCHASING_DISABLED_OUT_OF_ORDER";

	// Token: 0x04002995 RID: 10645
	private const string ATM_CURRENT_BALANCE_KEY = "ATM_CURRENT_BALANCE";

	// Token: 0x04002996 RID: 10646
	private const string ATM_MODDED_CLIENT_KEY = "ATM_MODDED_CLIENT";

	// Token: 0x04002997 RID: 10647
	private const string ATM_CHOOSE_PURCHASE_KEY = "ATM_CHOOSE_PURCHASE";

	// Token: 0x04002998 RID: 10648
	private const string ATM_PURCHASE_CONFIRMATION_KEY = "ATM_PURCHASE_CONFIRMATION";

	// Token: 0x04002999 RID: 10649
	private const string ATM_PURCHASE_CONFIRMATION_STEAM_KEY = "ATM_PURCHASE_CONFIRMATION_STEAM";

	// Token: 0x0400299A RID: 10650
	private const string ATM_PURCHASING_KEY = "ATM_PURCHASING";

	// Token: 0x0400299B RID: 10651
	private const string ATM_SUCCESS_NEW_BALANCE_KEY = "ATM_SUCCESS_NEW_BALANCE";

	// Token: 0x0400299C RID: 10652
	private const string ATM_PURCHASE_CANCELLED_KEY = "ATM_PURCHASE_CANCELLED";

	// Token: 0x0400299D RID: 10653
	private const string ATM_LOCKED_KEY = "ATM_LOCKED";

	// Token: 0x0400299E RID: 10654
	private const string ATM_RETURN_KEY = "ATM_RETURN";

	// Token: 0x0400299F RID: 10655
	private const string ATM_BACK_KEY = "ATM_BACK";

	// Token: 0x040029A0 RID: 10656
	private const string ATM_CONFIRM_KEY = "ATM_CONFIRM";

	// Token: 0x040029A1 RID: 10657
	private const string ATM_IAP_NOT_AVAILABLE_KEY = "ATM_IAP_NOT_AVAILABLE";

	// Token: 0x040029A2 RID: 10658
	private const string ATM_BALANCE_KEY = "ATM_BALANCE";

	// Token: 0x040029A3 RID: 10659
	private const string ATM_PURCHASE_KEY = "ATM_PURCHASE";

	// Token: 0x040029A4 RID: 10660
	private const string ATM_CREATOR_CODE_KEY = "ATM_CREATOR_CODE";

	// Token: 0x040029A5 RID: 10661
	private const string ATM_CREATOR_CODE_VALIDATING_KEY = "ATM_CREATOR_CODE_VALIDATING";

	// Token: 0x040029A6 RID: 10662
	private const string ATM_CREATOR_CODE_VALID_KEY = "ATM_CREATOR_CODE_VALID";

	// Token: 0x040029A7 RID: 10663
	private const string ATM_CREATOR_CODE_INVALID_KEY = "ATM_CREATOR_CODE_INVALID";

	// Token: 0x040029A8 RID: 10664
	private const string ATM_PURCHASE_OPTION_FIRST_KEY = "ATM_PURCHASE_OPTION_FIRST";

	// Token: 0x040029A9 RID: 10665
	private const string ATM_PURCHASE_OPTION_SECOND_KEY = "ATM_PURCHASE_OPTION_SECOND";

	// Token: 0x040029AA RID: 10666
	private const string ATM_PURCHASE_OPTION_THIRD_KEY = "ATM_PURCHASE_OPTION_THIRD";

	// Token: 0x040029AB RID: 10667
	private const string ATM_PURCHASE_OPTION_FOURTH_KEY = "ATM_PURCHASE_OPTION_FOURTH";

	// Token: 0x040029AC RID: 10668
	[OnEnterPlay_SetNull]
	public static volatile ATM_Manager instance;

	// Token: 0x040029AD RID: 10669
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x040029AE RID: 10670
	public List<ATM_UI> atmUIs = new List<ATM_UI>();

	// Token: 0x040029AF RID: 10671
	[HideInInspector]
	public List<CreatorCodeSmallDisplay> smallDisplays;

	// Token: 0x040029B0 RID: 10672
	private ATM_Manager.ATMStages currentATMStage;

	// Token: 0x040029B1 RID: 10673
	public int numShinyRocksToBuy;

	// Token: 0x040029B2 RID: 10674
	public float shinyRocksCost;

	// Token: 0x040029B3 RID: 10675
	public bool alreadyBegan;

	// Token: 0x040029B4 RID: 10676
	[SerializeField]
	private NexusGroupId[] nexusGroups;

	// Token: 0x040029B5 RID: 10677
	private string _tempCreatorCodeOveride;

	// Token: 0x040029B6 RID: 10678
	private const string ATM_TERMINAL_ID = "atm_terminal_id";

	// Token: 0x020004BF RID: 1215
	public enum ATMStages
	{
		// Token: 0x040029B8 RID: 10680
		Unavailable,
		// Token: 0x040029B9 RID: 10681
		Begin,
		// Token: 0x040029BA RID: 10682
		Menu,
		// Token: 0x040029BB RID: 10683
		Balance,
		// Token: 0x040029BC RID: 10684
		Choose,
		// Token: 0x040029BD RID: 10685
		Confirm,
		// Token: 0x040029BE RID: 10686
		Purchasing,
		// Token: 0x040029BF RID: 10687
		Success,
		// Token: 0x040029C0 RID: 10688
		Failure,
		// Token: 0x040029C1 RID: 10689
		SafeAccount
	}
}
