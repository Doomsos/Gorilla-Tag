using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000130 RID: 304
public class SIResourceCollection : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000827 RID: 2087 RVA: 0x0002C449 File Offset: 0x0002A649
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000828 RID: 2088 RVA: 0x0002C451 File Offset: 0x0002A651
	public bool IsAuthority
	{
		get
		{
			return this.SIManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000829 RID: 2089 RVA: 0x0002C463 File Offset: 0x0002A663
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x0600082A RID: 2090 RVA: 0x0002C470 File Offset: 0x0002A670
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0002C484 File Offset: 0x0002A684
	private void CollectButtonColliders()
	{
		SIResourceCollection.<>c__DisplayClass45_0 CS$<>8__locals1;
		CS$<>8__locals1.buttons = Enumerable.ToList<SITouchscreenButton>(base.GetComponentsInChildren<SITouchscreenButton>(true));
		SIResourceCollection.<CollectButtonColliders>g__RemoveButtonsInside|45_2(Enumerable.ToArray<GameObject>(Enumerable.Select<DestroyIfNotBeta, GameObject>(base.GetComponentsInChildren<DestroyIfNotBeta>(), (DestroyIfNotBeta d) => d.gameObject)), ref CS$<>8__locals1);
		SIResourceCollection.<CollectButtonColliders>g__RemoveButtonsInside|45_2(new GameObject[]
		{
			this.helpScreen
		}, ref CS$<>8__locals1);
		this._nonPopupButtonColliders = Enumerable.ToList<Collider>(Enumerable.Select<SITouchscreenButton, Collider>(CS$<>8__locals1.buttons, (SITouchscreenButton b) => b.GetComponent<Collider>()));
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0002C528 File Offset: 0x0002A728
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0002C57C File Offset: 0x0002A77C
	public void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		if (this.parentTerminal == null)
		{
			this.parentTerminal = base.GetComponentInParent<SICombinedTerminal>();
		}
		this.screenData = new Dictionary<SIResourceCollection.ResourceCollectorTerminalState, GameObject>();
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources, this.currentResourcesScreen);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.HelpScreen, this.helpScreen);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote, this.purchasingRemote);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart, this.purchasingStart);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress, this.purchaseInProgress);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess, this.purchasingSuccess);
		this.screenData.Add(SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure, this.purchasingFailure);
		this.Reset();
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002C654 File Offset: 0x0002A854
	public void Reset()
	{
		this.currentState = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
		this.lastState = this.currentState;
		this.SetScreenVisibility(this.currentState, this.lastState);
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002C67C File Offset: 0x0002A87C
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan);
		}
		stream.SendNext(this.currentHelpButtonPageIndex);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002C6E4 File Offset: 0x0002A8E4
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentHelpButtonPageIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState = (SIResourceCollection.ResourceCollectorTerminalState)stream.ReceiveNext();
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState2 = (SIResourceCollection.ResourceCollectorTerminalState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			resourceCollectorTerminalState = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
			resourceCollectorTerminalState2 = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
		}
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(resourceCollectorTerminalState, resourceCollectorTerminalState2);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002C7C7 File Offset: 0x0002A9C7
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.currentHelpButtonPageIndex);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0002C7F0 File Offset: 0x0002A9F0
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.currentHelpButtonPageIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState = (SIResourceCollection.ResourceCollectorTerminalState)reader.ReadInt32();
		SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState2 = (SIResourceCollection.ResourceCollectorTerminalState)reader.ReadInt32();
		if (!Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			resourceCollectorTerminalState = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
			resourceCollectorTerminalState2 = SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
		}
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState) || !Enum.IsDefined(typeof(SIResourceCollection.ResourceCollectorTerminalState), resourceCollectorTerminalState2))
		{
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan, SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(resourceCollectorTerminalState, resourceCollectorTerminalState2);
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0002C8C4 File Offset: 0x0002AAC4
	public bool PopupActive()
	{
		return this.IsPopupState(this.currentState);
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0002C8D2 File Offset: 0x0002AAD2
	public bool IsPopupState(SIResourceCollection.ResourceCollectorTerminalState state)
	{
		return state == SIResourceCollection.ResourceCollectorTerminalState.HelpScreen || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure || state == SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess;
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0002C8EE File Offset: 0x0002AAEE
	public bool HasHelpButton(SIResourceCollection.ResourceCollectorTerminalState state)
	{
		return state == SIResourceCollection.ResourceCollectorTerminalState.CurrentResources || state == SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan;
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0002C8FA File Offset: 0x0002AAFA
	public void UpdateState(SIResourceCollection.ResourceCollectorTerminalState newState, SIResourceCollection.ResourceCollectorTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0002C914 File Offset: 0x0002AB14
	public void UpdateState(SIResourceCollection.ResourceCollectorTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess:
			break;
		case SIResourceCollection.ResourceCollectorTerminalState.CurrentResources:
			this.currentResourcesResourceCounts.text = this.FormattedPlayerResourceCount(this.ActivePlayer);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote:
			if (this.ActivePlayer != null && this.ActivePlayer == SIPlayer.LocalPlayer)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart);
			}
			this.currentResourceCountsLocal.text = this.FormattedPlayerResourceCountWithMax(this.ActivePlayer);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart:
			if (this.ActivePlayer != null && this.ActivePlayer != SIPlayer.LocalPlayer)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote);
			}
			else
			{
				this.shinyRockInfo.text = "PRICE: 500 SHINY ROCKS\n\nYOU HAVE:\n" + ProgressionManager.Instance.GetShinyRocksTotal().ToString() + " SHINY ROCKS";
			}
			this.currentResourceCountsLocal.text = this.FormattedPlayerResourceCountWithMax(this.ActivePlayer);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure:
			switch (this.failureReason)
			{
			case SIResourceCollection.FailReason.NotEnoughRocks:
				this.failureReasonText.text = "NOT ENOUGH SHINY ROCKS! PLEASE TRY AGAIN LATER, OR PURCHASE MORE SHINY ROCKS!";
				return;
			case SIResourceCollection.FailReason.ResourcesFull:
				this.failureReasonText.text = "YOU ARE ALREADY AT MAX RESOURCES! DONATE YOUR SHINY ROCKS TO A GOOD CAUSE INSTEAD OF US, KNUCKLEHEAD!";
				return;
			case SIResourceCollection.FailReason.Unknown:
				this.failureReasonText.text = "UHHHHH SOMETHING WENT WRONG, I'M NOT SURE WHAT, SORRY TRY AGAIN LATER MAYBE!";
				break;
			default:
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0002CAA0 File Offset: 0x0002ACA0
	public string FormattedPlayerResourceCount(SIPlayer player)
	{
		return string.Concat(new string[]
		{
			this.GetFormattedResource(player, SIResource.ResourceType.TechPoint),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.StrangeWood),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.WeirdGear),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.VibratingSpring),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.BouncySand),
			"\n",
			this.GetFormattedResource(player, SIResource.ResourceType.FloppyMetal)
		});
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0002CB28 File Offset: 0x0002AD28
	public string FormattedPlayerResourceCountWithMax(SIPlayer player)
	{
		return string.Concat(new string[]
		{
			this.GetFormattedResource(player, SIResource.ResourceType.StrangeWood),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.WeirdGear),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.VibratingSpring),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.BouncySand),
			" -> 20\n",
			this.GetFormattedResource(player, SIResource.ResourceType.FloppyMetal),
			" -> 20"
		});
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0002CBA4 File Offset: 0x0002ADA4
	private string GetFormattedResource(SIPlayer player, SIResource.ResourceType resource)
	{
		int resourceMaxCap = SIProgression.Instance.GetResourceMaxCap(resource);
		if (resourceMaxCap == 2147483647)
		{
			return player.CurrentProgression.resourceArray[(int)resource].ToString();
		}
		return string.Format("{0}/{1}", player.CurrentProgression.resourceArray[(int)resource], resourceMaxCap);
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0002CC00 File Offset: 0x0002AE00
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0002CC34 File Offset: 0x0002AE34
	public void SetScreenVisibility(SIResourceCollection.ResourceCollectorTerminalState currentState, SIResourceCollection.ResourceCollectorTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SIResourceCollection.ResourceCollectorTerminalState resourceCollectorTerminalState in this.screenData.Keys)
		{
			bool flag2 = resourceCollectorTerminalState == currentState || (flag && resourceCollectorTerminalState == lastState);
			if (this.screenData[resourceCollectorTerminalState].activeSelf != flag2)
			{
				this.screenData[resourceCollectorTerminalState].SetActive(flag2);
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0002CD2C File Offset: 0x0002AF2C
	public void PlayerHandScanned(int actorNr)
	{
		this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources);
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0002CD38 File Offset: 0x0002AF38
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && (this.ActivePlayer == null || this.ActivePlayer != SIPlayer.LocalPlayer))
		{
			this.parentTerminal.PlayWrongPlayerBuzz(this.uiCenter);
		}
		else
		{
			this.soundBankPlayer.Play();
		}
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && this.ActivePlayer == SIPlayer.LocalPlayer && this.currentState == SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart && buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
		{
			bool flag = ProgressionManager.Instance.GetShinyRocksTotal() >= 500;
			bool flag2 = SIProgression.ResourcesMaxed();
			if (flag && !flag2)
			{
				ProgressionManager.Instance.PurchaseResources(delegate(ProgressionManager.UserInventory userInventoryResponse)
				{
					SIProgression.Instance.SendPurchaseResourcesData();
					ProgressionManager.Instance.RefreshUserInventory();
					this.TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType.Collect, -1, SIPlayer.LocalPlayer.ActorNr);
				}, delegate(string error)
				{
					SIResourceCollection.FailReason data2;
					if (!(error == "Not enough Shiny Rocks to complete this purchase"))
					{
						if (!(error == "already maxed resources"))
						{
							data2 = SIResourceCollection.FailReason.Unknown;
						}
						else
						{
							data2 = SIResourceCollection.FailReason.ResourcesFull;
						}
					}
					else
					{
						data2 = SIResourceCollection.FailReason.NotEnoughRocks;
					}
					this.TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType.OverrideFailure, (int)data2, SIPlayer.LocalPlayer.ActorNr);
				});
			}
			else
			{
				buttonType = SITouchscreenButton.SITouchscreenButtonType.OverrideFailure;
				if (!flag)
				{
					data = 0;
				}
				else if (flag2)
				{
					data = 1;
				}
				else
				{
					data = 2;
				}
			}
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.ResourceCollection);
			return;
		}
		if (this.ActivePlayer == null || actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.soundBankPlayer.Play();
		switch (this.currentState)
		{
		case SIResourceCollection.ResourceCollectorTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.HelpScreen);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.CurrentResources:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Purchase)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.HelpScreen:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.currentHelpButtonPageIndex = 0;
				this.UpdateState(this.lastState);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.currentHelpButtonPageIndex = Mathf.Clamp(this.currentHelpButtonPageIndex + 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.currentHelpButtonPageIndex = Mathf.Clamp(this.currentHelpButtonPageIndex - 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.currentHelpButtonPageIndex);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseRemote:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseStart:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Cancel)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources);
				return;
			}
			this.failureReason = (SIResourceCollection.FailReason)data;
			this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure);
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseInProgress:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.OverrideFailure)
			{
				this.failureReason = (SIResourceCollection.FailReason)data;
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Collect)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess);
			}
			return;
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseSuccess:
		case SIResourceCollection.ResourceCollectorTerminalState.PurchaseFailure:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.UpdateState(SIResourceCollection.ResourceCollectorTerminalState.CurrentResources);
			}
			return;
		default:
			return;
		}
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x00002789 File Offset: 0x00000989
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0002CF6C File Offset: 0x0002B16C
	[CompilerGenerated]
	internal static void <CollectButtonColliders>g__RemoveButtonsInside|45_2(GameObject[] roots, ref SIResourceCollection.<>c__DisplayClass45_0 A_1)
	{
		for (int i = 0; i < roots.Length; i++)
		{
			foreach (SITouchscreenButton sitouchscreenButton in roots[i].GetComponentsInChildren<SITouchscreenButton>(true))
			{
				A_1.buttons.Remove(sitouchscreenButton);
			}
		}
	}

	// Token: 0x040009F1 RID: 2545
	public const int REFILL_PURCHASE_SHINY_ROCK_COST = 500;

	// Token: 0x040009F2 RID: 2546
	private const string lineBreak = "\n";

	// Token: 0x040009F3 RID: 2547
	private const string appendToMax = " -> 20";

	// Token: 0x040009F4 RID: 2548
	public SIResourceCollection.ResourceCollectorTerminalState currentState;

	// Token: 0x040009F5 RID: 2549
	public SIResourceCollection.ResourceCollectorTerminalState lastState;

	// Token: 0x040009F6 RID: 2550
	public int resourceDepositedCount;

	// Token: 0x040009F7 RID: 2551
	private int currentHelpButtonPageIndex;

	// Token: 0x040009F8 RID: 2552
	public GameObject waitingForScanScreen;

	// Token: 0x040009F9 RID: 2553
	public GameObject currentResourcesScreen;

	// Token: 0x040009FA RID: 2554
	public GameObject helpScreen;

	// Token: 0x040009FB RID: 2555
	public SICombinedTerminal parentTerminal;

	// Token: 0x040009FC RID: 2556
	public Sprite[] resourceImageSprites;

	// Token: 0x040009FD RID: 2557
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x040009FE RID: 2558
	public GameObject[] helpPopupScreens;

	// Token: 0x040009FF RID: 2559
	public GameObject purchasingRemote;

	// Token: 0x04000A00 RID: 2560
	public GameObject purchasingStart;

	// Token: 0x04000A01 RID: 2561
	public GameObject purchaseInProgress;

	// Token: 0x04000A02 RID: 2562
	public GameObject purchasingSuccess;

	// Token: 0x04000A03 RID: 2563
	public GameObject purchasingFailure;

	// Token: 0x04000A04 RID: 2564
	public GameObject popupScreen;

	// Token: 0x04000A05 RID: 2565
	public Transform uiCenter;

	// Token: 0x04000A06 RID: 2566
	[Header("Purchasing Pages")]
	public TextMeshProUGUI shinyRockInfo;

	// Token: 0x04000A07 RID: 2567
	public TextMeshProUGUI currentResourceCountsLocal;

	// Token: 0x04000A08 RID: 2568
	public TextMeshProUGUI currentResourceCountsRemote;

	// Token: 0x04000A09 RID: 2569
	public TextMeshProUGUI failureReasonText;

	// Token: 0x04000A0A RID: 2570
	public const string failureFull = "YOU ARE ALREADY AT MAX RESOURCES! DONATE YOUR SHINY ROCKS TO A GOOD CAUSE INSTEAD OF US, KNUCKLEHEAD!";

	// Token: 0x04000A0B RID: 2571
	public const string failureNotEnoughRocks = "NOT ENOUGH SHINY ROCKS! PLEASE TRY AGAIN LATER, OR PURCHASE MORE SHINY ROCKS!";

	// Token: 0x04000A0C RID: 2572
	public const string failureUnknown = "UHHHHH SOMETHING WENT WRONG, I'M NOT SURE WHAT, SORRY TRY AGAIN LATER MAYBE!";

	// Token: 0x04000A0D RID: 2573
	private SIResourceCollection.FailReason failureReason;

	// Token: 0x04000A0E RID: 2574
	public Image background;

	// Token: 0x04000A0F RID: 2575
	public Color active;

	// Token: 0x04000A10 RID: 2576
	public Color notActive;

	// Token: 0x04000A11 RID: 2577
	public TextMeshProUGUI currentResourcesResourceCounts;

	// Token: 0x04000A12 RID: 2578
	private Dictionary<SIResourceCollection.ResourceCollectorTerminalState, GameObject> screenData;

	// Token: 0x04000A13 RID: 2579
	private bool initialized;

	// Token: 0x04000A14 RID: 2580
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000A15 RID: 2581
	[Tooltip("Button colliders to disable while popup screen is shown.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x02000131 RID: 305
	public enum FailReason
	{
		// Token: 0x04000A17 RID: 2583
		NotEnoughRocks,
		// Token: 0x04000A18 RID: 2584
		ResourcesFull,
		// Token: 0x04000A19 RID: 2585
		Unknown
	}

	// Token: 0x02000132 RID: 306
	public enum ResourceCollectorTerminalState
	{
		// Token: 0x04000A1B RID: 2587
		WaitingForScan,
		// Token: 0x04000A1C RID: 2588
		CurrentResources,
		// Token: 0x04000A1D RID: 2589
		HelpScreen,
		// Token: 0x04000A1E RID: 2590
		PurchaseRemote,
		// Token: 0x04000A1F RID: 2591
		PurchaseStart,
		// Token: 0x04000A20 RID: 2592
		PurchaseInProgress,
		// Token: 0x04000A21 RID: 2593
		PurchaseSuccess,
		// Token: 0x04000A22 RID: 2594
		PurchaseFailure
	}
}
