using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaTag;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000143 RID: 323
[DefaultExecutionOrder(100)]
public class SITechTreeStation : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x0600089A RID: 2202 RVA: 0x0002DE68 File Offset: 0x0002C068
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x0600089B RID: 2203 RVA: 0x0002DE70 File Offset: 0x0002C070
	public SITechTreeNode CurrentNode
	{
		get
		{
			return this.techTreeSO.GetTreeNode(this.parentTerminal.ActivePage, this.currentNodeId);
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x0600089C RID: 2204 RVA: 0x0002DE8E File Offset: 0x0002C08E
	public SITechTreePage CurrentPage
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x0600089D RID: 2205 RVA: 0x0002DEB0 File Offset: 0x0002C0B0
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x0600089E RID: 2206 RVA: 0x0002DEBD File Offset: 0x0002C0BD
	public string ActivePlayerName
	{
		get
		{
			return this.ActivePlayer.gamePlayer.rig.OwningNetPlayer.SanitizedNickName;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x0600089F RID: 2207 RVA: 0x0002DED9 File Offset: 0x0002C0D9
	public bool IsAuthority
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x060008A0 RID: 2208 RVA: 0x0002DEF5 File Offset: 0x0002C0F5
	public GameEntityManager GameEntityManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x060008A1 RID: 2209 RVA: 0x0002DF0C File Offset: 0x0002C10C
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002DF20 File Offset: 0x0002C120
	private void CollectButtonColliders()
	{
		SITechTreeStation.<>c__DisplayClass77_0 CS$<>8__locals1;
		CS$<>8__locals1.buttons = Enumerable.ToList<SITouchscreenButton>(base.GetComponentsInChildren<SITouchscreenButton>(true));
		SITechTreeStation.<CollectButtonColliders>g__RemoveButtonsInside|77_2(Enumerable.ToArray<GameObject>(Enumerable.Select<DestroyIfNotBeta, GameObject>(base.GetComponentsInChildren<DestroyIfNotBeta>(), (DestroyIfNotBeta d) => d.gameObject)), ref CS$<>8__locals1);
		SITechTreeStation.<CollectButtonColliders>g__RemoveButtonsInside|77_2(new GameObject[]
		{
			this.techTreeHelpScreen,
			this.nodePopupScreen
		}, ref CS$<>8__locals1);
		this._nonPopupButtonColliders = Enumerable.ToList<Collider>(Enumerable.Select<SITouchscreenButton, Collider>(CS$<>8__locals1.buttons, (SITouchscreenButton b) => b.GetComponent<Collider>()));
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0002DFCC File Offset: 0x0002C1CC
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002E020 File Offset: 0x0002C220
	private void OnEnable()
	{
		SIProgression instance = SIProgression.Instance;
		instance.OnTreeReady = (Action)Delegate.Combine(instance.OnTreeReady, new Action(this.OnProgressionUpdate));
		SIProgression instance2 = SIProgression.Instance;
		instance2.OnInventoryReady = (Action)Delegate.Combine(instance2.OnInventoryReady, new Action(this.OnProgressionUpdate));
		SIProgression instance3 = SIProgression.Instance;
		instance3.OnNodeUnlocked = (Action<SIUpgradeType>)Delegate.Combine(instance3.OnNodeUnlocked, new Action<SIUpgradeType>(this.OnProgressionUpdateNode));
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0002E0A0 File Offset: 0x0002C2A0
	private void OnDisable()
	{
		SIProgression instance = SIProgression.Instance;
		instance.OnTreeReady = (Action)Delegate.Remove(instance.OnTreeReady, new Action(this.OnProgressionUpdate));
		SIProgression instance2 = SIProgression.Instance;
		instance2.OnInventoryReady = (Action)Delegate.Remove(instance2.OnInventoryReady, new Action(this.OnProgressionUpdate));
		SIProgression instance3 = SIProgression.Instance;
		instance3.OnNodeUnlocked = (Action<SIUpgradeType>)Delegate.Remove(instance3.OnNodeUnlocked, new Action<SIUpgradeType>(this.OnProgressionUpdateNode));
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002E120 File Offset: 0x0002C320
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
		this.screenData = new Dictionary<SITechTreeStation.TechTreeStationTerminalState, GameObject>();
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList, this.pagesListScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.TechTreePage, this.pageScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup, this.nodePopupScreen);
		this.screenData.Add(SITechTreeStation.TechTreeStationTerminalState.HelpScreen, this.techTreeHelpScreen);
		this.techTreeSO.EnsureInitialized();
		this.pageButtons = new List<SIGadgetListEntry>();
		this.techTreePages = new List<SITechTreeUIPage>();
		this.spriteByType.Add(SIResource.ResourceType.TechPoint, this.techPointSprite);
		this.spriteByType.Add(SIResource.ResourceType.StrangeWood, this.strangeWoodSprite);
		this.spriteByType.Add(SIResource.ResourceType.WeirdGear, this.weirdGearSprite);
		this.spriteByType.Add(SIResource.ResourceType.VibratingSpring, this.vibratingSpringSprite);
		this.spriteByType.Add(SIResource.ResourceType.BouncySand, this.bouncySandSprite);
		this.spriteByType.Add(SIResource.ResourceType.FloppyMetal, this.floppyMetalSprite);
		this.techTreeIconById.Add(SITechTreePageId.Thruster, this.thrustersIcon);
		this.techTreeIconById.Add(SITechTreePageId.Stilt, this.longArmsIcon);
		this.techTreeIconById.Add(SITechTreePageId.Grenades, this.floppyMetalSprite);
		this.techTreeIconById.Add(SITechTreePageId.Dash, this.dashYoYoIcon);
		this.techTreeIconById.Add(SITechTreePageId.Platform, this.platformsIcon);
		this.techTreeIconById.Add(SITechTreePageId.TapTeleport, this.floppyMetalSprite);
		this.techTreeIconById.Add(SITechTreePageId.Tentacle, this.floppyMetalSprite);
		this.techTreeIconById.Add(SITechTreePageId.AirGrab, this.floppyMetalSprite);
		this.techTreeIconById.Add(SITechTreePageId.SlipMitt, this.floppyMetalSprite);
		for (int i = 0; i < this.techTreeSO.TreePages.Count; i++)
		{
			SITechTreePage sitechTreePage = this.techTreeSO.TreePages[i];
			if (sitechTreePage.IsValid)
			{
				SIGadgetListEntry sigadgetListEntry = Object.Instantiate<SIGadgetListEntry>(this.pageListEntryPrefab, this.pageListParent);
				StaticLodManager.TryAddLateInstantiatedMembers(sigadgetListEntry.gameObject);
				sigadgetListEntry.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText, SITouchscreenButton.SITouchscreenButtonType.PageSelect, i, -0.07f);
				this.pageButtons.Add(sigadgetListEntry);
				SITechTreeUIPage sitechTreeUIPage = Object.Instantiate<SITechTreeUIPage>(this.pagePrefab, this.pageParent);
				StaticLodManager.TryAddLateInstantiatedMembers(sitechTreeUIPage.gameObject);
				sitechTreeUIPage.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText);
				this.techTreePages.Add(sitechTreeUIPage);
			}
		}
		this.Reset();
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002E3C8 File Offset: 0x0002C5C8
	public void Reset()
	{
		this.currentState = SITechTreeStation.TechTreeStationTerminalState.WaitingForScan;
		this.nodePopupState = SITechTreeStation.NodePopupState.Description;
		this.SetScreenVisibility(this.currentState, this.currentState);
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002E3EC File Offset: 0x0002C5EC
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
		}
		stream.SendNext(this.currentNodeId);
		stream.SendNext(this.helpScreenIndex);
		stream.SendNext((int)this.nodePopupState);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0002E478 File Offset: 0x0002C678
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentNodeId = (int)stream.ReceiveNext();
		if (this.CurrentNode == null)
		{
			this.currentNodeId = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		this.helpScreenIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this.nodePopupState = (SITechTreeStation.NodePopupState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(SITechTreeStation.NodePopupState), this.nodePopupState))
		{
			this.nodePopupState = SITechTreeStation.NodePopupState.Description;
		}
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState = (SITechTreeStation.TechTreeStationTerminalState)stream.ReceiveNext();
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState2 = (SITechTreeStation.TechTreeStationTerminalState)stream.ReceiveNext();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState) || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState2))
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(techTreeStationTerminalState, techTreeStationTerminalState2);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0002E58B File Offset: 0x0002C78B
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.currentNodeId);
		writer.Write(this.helpScreenIndex);
		writer.Write((int)this.nodePopupState);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0002E5CC File Offset: 0x0002C7CC
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.currentNodeId = reader.ReadInt32();
		if (!Enum.IsDefined(typeof(SIUpgradeType), this.CurrentNode.upgradeType))
		{
			GTDev.LogError<string>("issue with currentnodeid wee woo wee woo", null);
			this.currentNodeId = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		this.helpScreenIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		this.nodePopupState = (SITechTreeStation.NodePopupState)reader.ReadInt32();
		if (!Enum.IsDefined(typeof(SITechTreeStation.NodePopupState), this.nodePopupState))
		{
			this.nodePopupState = SITechTreeStation.NodePopupState.Description;
		}
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState = (SITechTreeStation.TechTreeStationTerminalState)reader.ReadInt32();
		SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState2 = (SITechTreeStation.TechTreeStationTerminalState)reader.ReadInt32();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState) || !Enum.IsDefined(typeof(SITechTreeStation.TechTreeStationTerminalState), techTreeStationTerminalState2))
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan, SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(techTreeStationTerminalState, techTreeStationTerminalState2);
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002E6EA File Offset: 0x0002C8EA
	public void UpdateState(SITechTreeStation.TechTreeStationTerminalState newState, SITechTreeStation.TechTreeStationTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002E704 File Offset: 0x0002C904
	public void UpdateState(SITechTreeStation.TechTreeStationTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SITechTreeStation.TechTreeStationTerminalState.WaitingForScan:
			break;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList:
			this.playerNameText.text = this.ActivePlayerName;
			this.screenDescriptionText.text = "TECH TREE PAGES";
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePage:
		{
			this.playerNameText.text = this.ActivePlayerName;
			this.UpdateNodeData(this.ActivePlayer);
			TMP_Text tmp_Text = this.screenDescriptionText;
			SITechTreePage treePage = this.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
			tmp_Text.text = ((treePage != null) ? treePage.nickName : null);
			foreach (SIGadgetListEntry sigadgetListEntry in this.pageButtons)
			{
				sigadgetListEntry.selectionIndicator.SetActive(sigadgetListEntry.Id == this.parentTerminal.ActivePage);
			}
			foreach (SITechTreeUIPage sitechTreeUIPage in this.techTreePages)
			{
				sitechTreeUIPage.gameObject.SetActive(sitechTreeUIPage.id == (SITechTreePageId)this.parentTerminal.ActivePage);
			}
			Sprite sprite;
			this.techTreeIconById.TryGetValue((SITechTreePageId)this.parentTerminal.ActivePage, ref sprite);
			this.techTreeIcon.sprite = sprite;
			return;
		}
		case SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup:
			switch (this.nodePopupState)
			{
			case SITechTreeStation.NodePopupState.Description:
				this.nodeNameText.text = this.CurrentNode.nickName;
				this.nodeDescriptionText.text = this.CurrentNode.description;
				if (this.ActivePlayer.NodeResearched(this.CurrentNode.upgradeType))
				{
					this.nodeResearched.SetActive(true);
					this.nodeLocked.SetActive(false);
					this.nodeAvailable.SetActive(false);
					this.nodeResearchButton.SetActive(false);
					this.canAffordNode.SetActive(false);
					this.cantAffordNode.SetActive(false);
				}
				else if (this.ActivePlayer.NodeParentsUnlocked(this.CurrentNode.upgradeType))
				{
					this.nodeResearched.SetActive(false);
					this.nodeLocked.SetActive(false);
					this.nodeAvailable.SetActive(true);
					this.nodeResearchButton.SetActive(true);
					bool flag = this.ActivePlayer.PlayerCanAffordNode(this.CurrentNode);
					this.canAffordNode.SetActive(flag);
					this.cantAffordNode.SetActive(!flag);
				}
				else
				{
					this.nodeResearched.SetActive(false);
					this.nodeAvailable.SetActive(false);
					this.nodeLocked.SetActive(true);
					this.nodeResearchButton.SetActive(false);
					this.canAffordNode.SetActive(false);
					this.cantAffordNode.SetActive(false);
				}
				this.nodeResourceTypeText.text = this.FormattedCurrentResourceTypesForNode(this.CurrentNode);
				this.nodeResourceCostText.text = this.FormattedResearchCost(this.CurrentNode);
				this.playerCurrentResourceAmountsText.text = this.FormattedCurrentResourceAmountsForNode(this.CurrentNode);
				break;
			case SITechTreeStation.NodePopupState.NotEnoughResources:
				this.nodeNameResearchMessageText.text = "NOT ENOUGH RESOURCES TO UNLOCK NODE! GATHER MORE AND TRY AGAIN!";
				break;
			case SITechTreeStation.NodePopupState.Success:
				this.nodeNameResearchMessageText.text = "SUCCESSFULLY UNLOCKED TECH NODE!";
				break;
			case SITechTreeStation.NodePopupState.Loading:
				if (this.ActivePlayer.NodeResearched(this.CurrentNode.upgradeType))
				{
					this.nodePopupState = SITechTreeStation.NodePopupState.Success;
					this.nodeNameResearchMessageText.text = "SUCCESSFULLY UNLOCKED TECH NODE!";
				}
				else
				{
					this.nodeNameResearchMessageText.text = "ATTEMPTING TO UNLOCK NODE\n\nLOADING . . .";
				}
				break;
			}
			this.UpdateNodePopupPage();
			return;
		case SITechTreeStation.TechTreeStationTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.helpScreenIndex);
			break;
		default:
			return;
		}
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0002EAEC File Offset: 0x0002CCEC
	public void SetScreenVisibility(SITechTreeStation.TechTreeStationTerminalState currentState, SITechTreeStation.TechTreeStationTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SITechTreeStation.TechTreeStationTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SITechTreeStation.TechTreeStationTerminalState techTreeStationTerminalState in this.screenData.Keys)
		{
			if (techTreeStationTerminalState == SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList)
			{
				this.screenData[techTreeStationTerminalState].SetActive(currentState > SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
			}
			else
			{
				bool flag2 = techTreeStationTerminalState == currentState || (flag && techTreeStationTerminalState == lastState);
				if (this.screenData[techTreeStationTerminalState].activeSelf != flag2)
				{
					this.screenData[techTreeStationTerminalState].SetActive(flag2);
				}
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		bool flag3 = currentState > SITechTreeStation.TechTreeStationTerminalState.WaitingForScan;
		this.screenDescriptionText.gameObject.SetActive(flag3);
		this.playerNameText.gameObject.SetActive(flag3);
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00027614 File Offset: 0x00025814
	public bool IsPopupState(SITechTreeStation.TechTreeStationTerminalState state)
	{
		return state == SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup || state == SITechTreeStation.TechTreeStationTerminalState.HelpScreen;
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0002EC2C File Offset: 0x0002CE2C
	public void PlayerHandScanned(int actorNr)
	{
		if (!this.IsAuthority)
		{
			this.parentTerminal.PlayerHandScanned(actorNr);
			return;
		}
		this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0002EC4A File Offset: 0x0002CE4A
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
		if (!isPopupButton)
		{
			this._nonPopupButtonColliders.Add(button.GetComponent<Collider>());
		}
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0002EC60 File Offset: 0x0002CE60
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
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && this.ActivePlayer == SIPlayer.LocalPlayer && this.currentState == SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup && this.nodePopupState == SITechTreeStation.NodePopupState.Description && buttonType == SITouchscreenButton.SITouchscreenButtonType.Research && !SIPlayer.LocalPlayer.NodeResearched(this.CurrentNode.upgradeType) && SIPlayer.LocalPlayer.NodeParentsUnlocked(this.CurrentNode.upgradeType))
		{
			SIProgression.Instance.TryUnlock(this.CurrentNode.upgradeType);
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.TechTree);
			return;
		}
		if (this.ActivePlayer == null || actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.soundBankPlayer.Play();
		if (buttonType == SITouchscreenButton.SITouchscreenButtonType.PageSelect)
		{
			this.parentTerminal.SetActivePage(data);
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
			return;
		}
		switch (this.currentState)
		{
		case SITechTreeStation.TechTreeStationTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.HelpScreen);
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.parentTerminal.SetActivePage(data);
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreePage:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.currentNodeId = data;
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePagesList);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.HelpScreen);
				return;
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup:
			if (this.nodePopupState == SITechTreeStation.NodePopupState.Description)
			{
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
				{
					this.UpdateState(this.lastState);
				}
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Research)
				{
					if (this.ActivePlayer.PlayerCanAffordNode(this.CurrentNode))
					{
						this.nodePopupState = SITechTreeStation.NodePopupState.Loading;
					}
					else
					{
						this.nodePopupState = SITechTreeStation.NodePopupState.NotEnoughResources;
					}
					this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup);
					return;
				}
			}
			else if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.nodePopupState = SITechTreeStation.NodePopupState.Description;
				this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreeNodePopup);
			}
			return;
		case SITechTreeStation.TechTreeStationTerminalState.HelpScreen:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.helpScreenIndex = 0;
				this.UpdateState(this.lastState);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.helpScreenIndex = Mathf.Clamp(this.helpScreenIndex + 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.helpScreenIndex);
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.helpScreenIndex = Mathf.Clamp(this.helpScreenIndex - 1, 0, this.helpPopupScreens.Length - 1);
				this.UpdateHelpButtonPage(this.helpScreenIndex);
			}
			return;
		default:
			return;
		}
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0002EED8 File Offset: 0x0002D0D8
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002EF0C File Offset: 0x0002D10C
	public void UpdateNodePopupPage()
	{
		int num = (this.nodePopupState == SITechTreeStation.NodePopupState.Description) ? 0 : 1;
		if (this.nodePopupScreens[0].activeSelf != (num == 0))
		{
			this.nodePopupScreens[0].SetActive(num == 0);
		}
		if (this.nodePopupScreens[1].activeSelf != (num == 1))
		{
			this.nodePopupScreens[1].SetActive(num == 1);
		}
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002EF70 File Offset: 0x0002D170
	public void UpdateNodeData(SIPlayer player)
	{
		if (player == null)
		{
			for (int i = 0; i < this.techTreePages.Count; i++)
			{
				this.techTreePages[i].PopulateDefaultNodeData();
			}
			return;
		}
		for (int j = 0; j < this.techTreePages.Count; j++)
		{
			this.techTreePages[j].PopulatePlayerNodeData(player);
		}
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0002EFD8 File Offset: 0x0002D1D8
	public string FormattedResearchCost(SITechTreeNode node)
	{
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			string text = "";
			text = text + sinode.costs[SIResource.ResourceType.TechPoint].ToString() + "\n";
			foreach (KeyValuePair<SIResource.ResourceType, int> keyValuePair in sinode.costs)
			{
				if (keyValuePair.Key != SIResource.ResourceType.TechPoint)
				{
					text += keyValuePair.Value.ToString();
					return text;
				}
			}
			return text;
		}
		return string.Join<int>("\n", Enumerable.Select<SIResource.ResourceCost, int>(node.nodeCost, (SIResource.ResourceCost c) => c.amount));
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0002F0BC File Offset: 0x0002D2BC
	public string FormattedCurrentResourceAmountsForNode(SITechTreeNode node)
	{
		string text = "";
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			text = text + this.ActivePlayer.CurrentProgression.resourceArray[0].ToString() + "\n";
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					if (keyValuePair.Key != SIResource.ResourceType.TechPoint)
					{
						text = text + this.ActivePlayer.CurrentProgression.resourceArray[(int)keyValuePair.Key].ToString() + "\n";
					}
				}
				return text;
			}
		}
		for (int i = 0; i < node.nodeCost.Length; i++)
		{
			text = text + this.ActivePlayer.CurrentProgression.resourceArray[(int)node.nodeCost[i].type].ToString() + "\n";
		}
		return text;
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0002F1D4 File Offset: 0x0002D3D4
	public string FormattedCurrentResourceTypesForNode(SITechTreeNode node)
	{
		string text = "";
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			text = text + SIResource.ResourceType.TechPoint.ToString().ToUpperInvariant() + "\n";
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					if (keyValuePair.Key != SIResource.ResourceType.TechPoint)
					{
						text = text + keyValuePair.Key.ToString().ToUpperInvariant() + "\n";
						this.resourceCost.sprite = this.spriteByType[keyValuePair.Key];
					}
				}
				return text;
			}
		}
		for (int i = 0; i < node.nodeCost.Length; i++)
		{
			text = text + node.nodeCost[i].type.ToString().ToUpperInvariant() + "\n";
		}
		return text;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0002F2F0 File Offset: 0x0002D4F0
	private void OnProgressionUpdate()
	{
		this.UpdateNodeData(this.ActivePlayer);
		this.UpdateState(this.currentState);
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0002F30A File Offset: 0x0002D50A
	private void OnProgressionUpdateNode(SIUpgradeType type)
	{
		this.OnProgressionUpdate();
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0002F314 File Offset: 0x0002D514
	public void SetActivePage()
	{
		if (this.CurrentNode == null)
		{
			this.currentNodeId = this.CurrentPage.AllNodes[0].Value.upgradeType.GetNodeId();
		}
		if (this.ActivePlayer != null)
		{
			this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.TechTreePage);
			return;
		}
		this.UpdateState(SITechTreeStation.TechTreeStationTerminalState.WaitingForScan);
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0002F36C File Offset: 0x0002D56C
	public bool IsValidPage(int pageId)
	{
		if (pageId < 0)
		{
			return false;
		}
		using (List<SITechTreeUIPage>.Enumerator enumerator = this.techTreePages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.id == (SITechTreePageId)pageId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0002F3EC File Offset: 0x0002D5EC
	[CompilerGenerated]
	internal static void <CollectButtonColliders>g__RemoveButtonsInside|77_2(GameObject[] roots, ref SITechTreeStation.<>c__DisplayClass77_0 A_1)
	{
		for (int i = 0; i < roots.Length; i++)
		{
			foreach (SITouchscreenButton sitouchscreenButton in roots[i].GetComponentsInChildren<SITouchscreenButton>(true))
			{
				A_1.buttons.Remove(sitouchscreenButton);
			}
		}
	}

	// Token: 0x04000A5F RID: 2655
	private Dictionary<SITechTreeStation.TechTreeStationTerminalState, GameObject> screenData;

	// Token: 0x04000A60 RID: 2656
	public SITechTreeStation.TechTreeStationTerminalState currentState;

	// Token: 0x04000A61 RID: 2657
	public SITechTreeStation.TechTreeStationTerminalState lastState;

	// Token: 0x04000A62 RID: 2658
	public SICombinedTerminal parentTerminal;

	// Token: 0x04000A63 RID: 2659
	public Sprite techPointSprite;

	// Token: 0x04000A64 RID: 2660
	public Sprite strangeWoodSprite;

	// Token: 0x04000A65 RID: 2661
	public Sprite weirdGearSprite;

	// Token: 0x04000A66 RID: 2662
	public Sprite vibratingSpringSprite;

	// Token: 0x04000A67 RID: 2663
	public Sprite bouncySandSprite;

	// Token: 0x04000A68 RID: 2664
	public Sprite floppyMetalSprite;

	// Token: 0x04000A69 RID: 2665
	public Sprite thrustersIcon;

	// Token: 0x04000A6A RID: 2666
	public Sprite longArmsIcon;

	// Token: 0x04000A6B RID: 2667
	public Sprite dashYoYoIcon;

	// Token: 0x04000A6C RID: 2668
	public Sprite platformsIcon;

	// Token: 0x04000A6D RID: 2669
	public int currentNodeId;

	// Token: 0x04000A6E RID: 2670
	public SITechTreeSO techTreeSO;

	// Token: 0x04000A6F RID: 2671
	public GameObject waitingForScanScreen;

	// Token: 0x04000A70 RID: 2672
	public GameObject pagesListScreen;

	// Token: 0x04000A71 RID: 2673
	public GameObject pageScreen;

	// Token: 0x04000A72 RID: 2674
	public GameObject nodePopupScreen;

	// Token: 0x04000A73 RID: 2675
	public GameObject techTreeHelpScreen;

	// Token: 0x04000A74 RID: 2676
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000A75 RID: 2677
	public Color active;

	// Token: 0x04000A76 RID: 2678
	public Color notActive;

	// Token: 0x04000A77 RID: 2679
	[Header("Main Screen Shared")]
	public TextMeshProUGUI screenDescriptionText;

	// Token: 0x04000A78 RID: 2680
	public TextMeshProUGUI playerNameText;

	// Token: 0x04000A79 RID: 2681
	public Image background;

	// Token: 0x04000A7A RID: 2682
	public Transform uiCenter;

	// Token: 0x04000A7B RID: 2683
	[Header("Popup Shared")]
	public GameObject popupScreen;

	// Token: 0x04000A7C RID: 2684
	[Header("Pages List")]
	[SerializeField]
	private Transform pageListParent;

	// Token: 0x04000A7D RID: 2685
	[SerializeField]
	private SIGadgetListEntry pageListEntryPrefab;

	// Token: 0x04000A7E RID: 2686
	private List<SIGadgetListEntry> pageButtons;

	// Token: 0x04000A7F RID: 2687
	[Header("Tree Page")]
	[SerializeField]
	private Transform pageParent;

	// Token: 0x04000A80 RID: 2688
	[SerializeField]
	private SITechTreeUIPage pagePrefab;

	// Token: 0x04000A81 RID: 2689
	private List<SITechTreeUIPage> techTreePages;

	// Token: 0x04000A82 RID: 2690
	[SerializeField]
	private SpriteRenderer techTreeIcon;

	// Token: 0x04000A83 RID: 2691
	[Header("Node Popup")]
	public GameObject[] nodePopupScreens;

	// Token: 0x04000A84 RID: 2692
	[Header("Research Node Description")]
	public TextMeshProUGUI nodeNameText;

	// Token: 0x04000A85 RID: 2693
	public TextMeshProUGUI nodeDescriptionText;

	// Token: 0x04000A86 RID: 2694
	public TextMeshProUGUI nodeResourceTypeText;

	// Token: 0x04000A87 RID: 2695
	public TextMeshProUGUI nodeResourceCostText;

	// Token: 0x04000A88 RID: 2696
	public TextMeshProUGUI playerCurrentResourceAmountsText;

	// Token: 0x04000A89 RID: 2697
	public GameObject nodeAvailable;

	// Token: 0x04000A8A RID: 2698
	public GameObject nodeLocked;

	// Token: 0x04000A8B RID: 2699
	public GameObject nodeResearched;

	// Token: 0x04000A8C RID: 2700
	public GameObject canAffordNode;

	// Token: 0x04000A8D RID: 2701
	public GameObject cantAffordNode;

	// Token: 0x04000A8E RID: 2702
	public GameObject nodeResearchButton;

	// Token: 0x04000A8F RID: 2703
	public SpriteRenderer techPointCost;

	// Token: 0x04000A90 RID: 2704
	public SpriteRenderer resourceCost;

	// Token: 0x04000A91 RID: 2705
	[Header("Research Attempt")]
	public TextMeshProUGUI nodeNameResearchMessageText;

	// Token: 0x04000A92 RID: 2706
	public SITechTreeStation.NodePopupState nodePopupState;

	// Token: 0x04000A93 RID: 2707
	[Header("Help")]
	public int helpScreenIndex;

	// Token: 0x04000A94 RID: 2708
	public GameObject[] helpPopupScreens;

	// Token: 0x04000A95 RID: 2709
	[Header("Audio")]
	[SerializeField]
	private SoundBankPlayer soundBankPlayer;

	// Token: 0x04000A96 RID: 2710
	[Header("Main Screen Colliders")]
	[Tooltip("Button colliders to disable while popup screen is shown.  Gets updated live to include page and gadget node buttons.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x04000A97 RID: 2711
	private Dictionary<SIResource.ResourceType, Sprite> spriteByType = new Dictionary<SIResource.ResourceType, Sprite>();

	// Token: 0x04000A98 RID: 2712
	private Dictionary<SITechTreePageId, Sprite> techTreeIconById = new Dictionary<SITechTreePageId, Sprite>();

	// Token: 0x04000A99 RID: 2713
	private bool initialized;

	// Token: 0x02000144 RID: 324
	public enum NodePopupState
	{
		// Token: 0x04000A9B RID: 2715
		Description,
		// Token: 0x04000A9C RID: 2716
		NotEnoughResources,
		// Token: 0x04000A9D RID: 2717
		Success,
		// Token: 0x04000A9E RID: 2718
		PurchaseInitiation,
		// Token: 0x04000A9F RID: 2719
		Loading
	}

	// Token: 0x02000145 RID: 325
	public enum TechTreeStationTerminalState
	{
		// Token: 0x04000AA1 RID: 2721
		WaitingForScan,
		// Token: 0x04000AA2 RID: 2722
		TechTreePagesList,
		// Token: 0x04000AA3 RID: 2723
		TechTreePage,
		// Token: 0x04000AA4 RID: 2724
		TechTreeNodePopup,
		// Token: 0x04000AA5 RID: 2725
		HelpScreen
	}
}
