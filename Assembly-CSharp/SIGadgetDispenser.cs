using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000116 RID: 278
public class SIGadgetDispenser : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000711 RID: 1809 RVA: 0x00026CA5 File Offset: 0x00024EA5
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000712 RID: 1810 RVA: 0x00026CAD File Offset: 0x00024EAD
	public SIPlayer ActivePlayer
	{
		get
		{
			return this.parentTerminal.activePlayer;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000713 RID: 1811 RVA: 0x00026CBA File Offset: 0x00024EBA
	public string ActivePlayerName
	{
		get
		{
			return this.ActivePlayer.gamePlayer.rig.OwningNetPlayer.SanitizedNickName;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000714 RID: 1812 RVA: 0x00026CD6 File Offset: 0x00024ED6
	public bool IsAuthority
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000715 RID: 1813 RVA: 0x00026CF2 File Offset: 0x00024EF2
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000716 RID: 1814 RVA: 0x00026D04 File Offset: 0x00024F04
	public GameEntityManager GameEntityManager
	{
		get
		{
			return this.parentTerminal.superInfection.siManager.gameEntityManager;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000717 RID: 1815 RVA: 0x00026D1B File Offset: 0x00024F1B
	public SITechTreeNode CurrentNode
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreeNode(this.parentTerminal.ActivePage, this._currentNode);
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000718 RID: 1816 RVA: 0x00026D43 File Offset: 0x00024F43
	public SITechTreePage CurrentPage
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO.GetTreePage((SITechTreePageId)this.parentTerminal.ActivePage);
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000719 RID: 1817 RVA: 0x00026D65 File Offset: 0x00024F65
	public SITechTreeSO TechTreeSO
	{
		get
		{
			return this.parentTerminal.superInfection.techTreeSO;
		}
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00026D78 File Offset: 0x00024F78
	private void CollectButtonColliders()
	{
		SIGadgetDispenser.<>c__DisplayClass52_0 CS$<>8__locals1;
		CS$<>8__locals1.buttons = Enumerable.ToList<SITouchscreenButton>(base.GetComponentsInChildren<SITouchscreenButton>(true));
		SIGadgetDispenser.<CollectButtonColliders>g__RemoveButtonsInside|52_2(Enumerable.ToArray<GameObject>(Enumerable.Select<DestroyIfNotBeta, GameObject>(base.GetComponentsInChildren<DestroyIfNotBeta>(), (DestroyIfNotBeta d) => d.gameObject)), ref CS$<>8__locals1);
		SIGadgetDispenser.<CollectButtonColliders>g__RemoveButtonsInside|52_2(new GameObject[]
		{
			this.gadgetDispensedScreen,
			this.gadgetsHelpScreen
		}, ref CS$<>8__locals1);
		this._nonPopupButtonColliders = Enumerable.ToList<Collider>(Enumerable.Select<SITouchscreenButton, Collider>(CS$<>8__locals1.buttons, (SITouchscreenButton b) => b.GetComponent<Collider>()));
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00026E24 File Offset: 0x00025024
	private void SetNonPopupButtonsEnabled(bool enable)
	{
		foreach (Collider collider in this._nonPopupButtonColliders)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x00026E78 File Offset: 0x00025078
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
		this.screenData = new Dictionary<SIGadgetDispenser.GadgetDispenserTerminalState, GameObject>();
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, this.waitingForScanScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList, this.gadgetListScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation, this.gadgetInformationScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed, this.gadgetDispensedScreen);
		this.screenData.Add(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen, this.gadgetsHelpScreen);
		this.parentTerminal.superInfection.techTreeSO.EnsureInitialized();
		int num = 0;
		this.gadgetPages = new List<SIGadgetListEntry>();
		for (int i = 0; i < this.parentTerminal.superInfection.techTreeSO.TreePages.Count; i++)
		{
			SITechTreePage sitechTreePage = this.parentTerminal.superInfection.techTreeSO.TreePages[i];
			SIGadgetListEntry sigadgetListEntry = Object.Instantiate<SIGadgetListEntry>(this.pageListEntryPrefab, this.pageListParent);
			sigadgetListEntry.Configure(this, sitechTreePage, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText, SITouchscreenButton.SITouchscreenButtonType.Select, i, -0.07f);
			this.gadgetPages.Add(sigadgetListEntry);
			num = Math.Max(num, sitechTreePage.DispensableGadgets.Count);
		}
		this.gadgetEntries = new List<SIDispenserGadgetListEntry>();
		for (int j = 0; j < num; j++)
		{
			SIDispenserGadgetListEntry sidispenserGadgetListEntry = Object.Instantiate<SIDispenserGadgetListEntry>(this.gadgetListEntryPrefab, this.gadgetListParent);
			sidispenserGadgetListEntry.transform.localPosition += new Vector3(0f, (float)j * -0.07f, 0f);
			sidispenserGadgetListEntry.SetStation(this, this.parentTerminal.zeroZeroImage, this.parentTerminal.onePointTwoText);
			this.gadgetEntries.Add(sidispenserGadgetListEntry);
		}
		this.Reset();
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0002705D File Offset: 0x0002525D
	public void Reset()
	{
		this.currentState = SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan;
		this.SetScreenVisibility(this.currentState, this.currentState);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00027078 File Offset: 0x00025278
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy)
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
		}
		stream.SendNext(this.helpScreenIndex);
		stream.SendNext(this._currentNode);
		stream.SendNext((int)this.currentState);
		stream.SendNext((int)this.lastState);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x000270F4 File Offset: 0x000252F4
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.helpScreenIndex = Mathf.Clamp((int)stream.ReceiveNext(), 0, this.helpPopupScreens.Length - 1);
		this._currentNode = (int)stream.ReceiveNext();
		if (this.CurrentNode == null && this.CurrentPage != null && this.CurrentPage.AllNodes.Count > 0 && this.CurrentPage.AllNodes[0].Value != null)
		{
			this._currentNode = (int)this.CurrentPage.AllNodes[0].Value.upgradeType;
		}
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState = (SIGadgetDispenser.GadgetDispenserTerminalState)stream.ReceiveNext();
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState2 = (SIGadgetDispenser.GadgetDispenserTerminalState)stream.ReceiveNext();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState) || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState2))
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(gadgetDispenserTerminalState, gadgetDispenserTerminalState2);
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00027206 File Offset: 0x00025406
	public void ZoneDataSerializeWrite(BinaryWriter writer)
	{
		writer.Write(this.helpScreenIndex);
		writer.Write(this._currentNode);
		writer.Write((int)this.currentState);
		writer.Write((int)this.lastState);
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00027238 File Offset: 0x00025438
	public void ZoneDataSerializeRead(BinaryReader reader)
	{
		this.helpScreenIndex = Mathf.Clamp(reader.ReadInt32(), 0, this.helpPopupScreens.Length - 1);
		int num = reader.ReadInt32();
		if (this.CurrentPage != null && this.CurrentPage.AllNodes != null)
		{
			this._currentNode = Mathf.Clamp(num, 0, this.CurrentPage.AllNodes.Count - 1);
		}
		else
		{
			this._currentNode = 0;
		}
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState = (SIGadgetDispenser.GadgetDispenserTerminalState)reader.ReadInt32();
		SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState2 = (SIGadgetDispenser.GadgetDispenserTerminalState)reader.ReadInt32();
		if (this.ActivePlayer == null || !this.ActivePlayer.gameObject.activeInHierarchy || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState) || !Enum.IsDefined(typeof(SIGadgetDispenser.GadgetDispenserTerminalState), gadgetDispenserTerminalState2))
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan, SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
			return;
		}
		this.UpdateState(gadgetDispenserTerminalState, gadgetDispenserTerminalState2);
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00027312 File Offset: 0x00025512
	public void UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState newState, SIGadgetDispenser.GadgetDispenserTerminalState newLastState)
	{
		if (!this.IsPopupState(newLastState))
		{
			this.currentState = newLastState;
		}
		this.UpdateState(newState);
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0002732C File Offset: 0x0002552C
	public void UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState newState)
	{
		if (!this.IsPopupState(this.currentState))
		{
			this.lastState = this.currentState;
		}
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, this.lastState);
		switch (this.currentState)
		{
		case SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan:
			break;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList:
			this.screenDescription.text = "UNLOCKED " + this.CurrentPage.nickName + " GADGETS";
			this.UpdateGadgetListVisibility();
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation:
			this.screenDescription.text = this.CurrentNode.nickName;
			this.gadgetDescriptionText.text = this.CurrentNode.description;
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed:
			this.gadgetDispensedText.text = this.ActivePlayerName + " HAS DISPENSED A " + this.CurrentNode.nickName + "!";
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen:
			this.UpdateHelpButtonPage(this.helpScreenIndex);
			break;
		default:
			return;
		}
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00027420 File Offset: 0x00025620
	public void SetScreenVisibility(SIGadgetDispenser.GadgetDispenserTerminalState currentState, SIGadgetDispenser.GadgetDispenserTerminalState lastState)
	{
		bool flag = this.IsPopupState(currentState);
		this.background.color = ((currentState == SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan) ? Color.white : ((this.ActivePlayer != null && this.ActivePlayer.gamePlayer.IsLocal()) ? this.active : this.notActive));
		foreach (SIGadgetDispenser.GadgetDispenserTerminalState gadgetDispenserTerminalState in this.screenData.Keys)
		{
			bool flag2 = gadgetDispenserTerminalState == currentState || (flag && gadgetDispenserTerminalState == lastState);
			if (this.screenData[gadgetDispenserTerminalState].activeSelf != flag2)
			{
				this.screenData[gadgetDispenserTerminalState].SetActive(flag2);
			}
		}
		if (this.popupScreen.activeSelf != flag)
		{
			this.popupScreen.SetActive(flag);
		}
		this.screenDescription.gameObject.SetActive(currentState > SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
		this.SetNonPopupButtonsEnabled(!flag);
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0002752C File Offset: 0x0002572C
	public void UpdateGadgetListVisibility()
	{
		foreach (SIDispenserGadgetListEntry sidispenserGadgetListEntry in this.gadgetEntries)
		{
			sidispenserGadgetListEntry.gameObject.SetActive(false);
		}
		int num = 0;
		foreach (SITechTreeNode sitechTreeNode in this.CurrentPage.DispensableGadgets)
		{
			if (this.ActivePlayer.CurrentProgression.IsUnlocked(sitechTreeNode.upgradeType))
			{
				SIDispenserGadgetListEntry sidispenserGadgetListEntry2 = this.gadgetEntries[num++];
				sidispenserGadgetListEntry2.SetTechTreeNode(sitechTreeNode);
				sidispenserGadgetListEntry2.gameObject.SetActive(true);
			}
		}
		this.noDispensableGadgetsMessage.SetActive(num == 0);
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00027614 File Offset: 0x00025814
	public bool IsPopupState(SIGadgetDispenser.GadgetDispenserTerminalState state)
	{
		return state == SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed || state == SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00027620 File Offset: 0x00025820
	public void PlayerHandScanned(int actorNr)
	{
		this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00027629 File Offset: 0x00025829
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
		if (!isPopupButton)
		{
			this._nonPopupButtonColliders.Add(button.GetComponent<Collider>());
		}
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00027640 File Offset: 0x00025840
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		if (actorNr == SIPlayer.LocalPlayer.ActorNr && (this.ActivePlayer == null || this.ActivePlayer != SIPlayer.LocalPlayer))
		{
			this.parentTerminal.PlayWrongPlayerBuzz(this.uiCenter);
		}
		else
		{
			this.touchSoundBankPlayer.Play();
		}
		if (!this.IsAuthority)
		{
			this.parentTerminal.TouchscreenButtonPressed(buttonType, data, actorNr, SICombinedTerminal.TerminalSubFunction.GadgetDispenser);
			return;
		}
		if (actorNr != this.ActivePlayer.ActorNr)
		{
			return;
		}
		this.touchSoundBankPlayer.Play();
		switch (this.currentState)
		{
		case SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				SITechTreeNode treeNode = this.TechTreeSO.GetTreeNode((int)this.CurrentPage.pageId, data);
				if (treeNode != null && treeNode.IsDispensableGadget)
				{
					this._currentNode = data;
					this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation);
				}
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Dispense)
			{
				SITechTreeNode treeNode2 = this.TechTreeSO.GetTreeNode((int)this.CurrentPage.pageId, data);
				if (treeNode2 != null && treeNode2.IsDispensableGadget)
				{
					this._currentNode = data;
					this.DispenseGadgetForPlayer(this.ActivePlayer);
					this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed);
				}
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetInformation:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Dispense)
			{
				this.DispenseGadgetForPlayer(this.ActivePlayer);
				this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.GadgetDispensed:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Exit)
			{
				this.UpdateState(this.lastState);
			}
			return;
		case SIGadgetDispenser.GadgetDispenserTerminalState.HelpScreen:
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

	// Token: 0x0600072A RID: 1834 RVA: 0x00027830 File Offset: 0x00025A30
	public void UpdateHelpButtonPage(int helpButtonPageIndex)
	{
		for (int i = 0; i < this.helpPopupScreens.Length; i++)
		{
			this.helpPopupScreens[i].SetActive(i == helpButtonPageIndex);
		}
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00027864 File Offset: 0x00025A64
	public void DispenseGadgetForPlayer(SIPlayer player)
	{
		int num = 0;
		int staticHash = this.CurrentNode.unlockedGadgetPrefab.name.GetStaticHash();
		for (int i = player.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			GameEntity gameEntityFromNetId = this.GameEntityManager.GetGameEntityFromNetId(player.activePlayerGadgets[i]);
			if (gameEntityFromNetId == null)
			{
				player.activePlayerGadgets.RemoveAt(i);
			}
			else
			{
				num++;
				if (num >= player.totalGadgetLimit)
				{
					this.GameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
					break;
				}
			}
		}
		SIUpgradeSet upgrades = player.GetUpgrades(this.CurrentPage.pageId);
		int num2 = 0;
		foreach (GraphNode<SITechTreeNode> graphNode in this.CurrentPage.AllNodes)
		{
			num2 |= 1 << graphNode.Value.upgradeType.GetNodeId();
		}
		upgrades.SetBits(upgrades.GetBits() & num2);
		foreach (SITechTreeNode sitechTreeNode in this.CurrentPage.DispensableGadgets)
		{
			if (sitechTreeNode != this.CurrentNode)
			{
				upgrades.Remove(sitechTreeNode.upgradeType);
			}
		}
		this.GameEntityManager.RequestCreateItem(staticHash, this.gadgetDispensePosition.position, this.gadgetDispensePosition.rotation, upgrades.GetCreateData(player));
		this.dispenseSoundBankPlayer.Play();
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00027A0C File Offset: 0x00025C0C
	public void SetActivePage()
	{
		if (this.CurrentNode == null)
		{
			this._currentNode = this.CurrentPage.AllNodes[0].Value.upgradeType.GetNodeId();
		}
		if (this.ActivePlayer != null)
		{
			this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.GadgetList);
			return;
		}
		this.UpdateState(SIGadgetDispenser.GadgetDispenserTerminalState.WaitingForScan);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00027A64 File Offset: 0x00025C64
	public bool IsValidPage(int pageId)
	{
		if (pageId < 0)
		{
			return false;
		}
		using (List<SIGadgetListEntry>.Enumerator enumerator = this.gadgetPages.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Id == pageId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00027AC4 File Offset: 0x00025CC4
	[CompilerGenerated]
	internal static void <CollectButtonColliders>g__RemoveButtonsInside|52_2(GameObject[] roots, ref SIGadgetDispenser.<>c__DisplayClass52_0 A_1)
	{
		for (int i = 0; i < roots.Length; i++)
		{
			foreach (SITouchscreenButton sitouchscreenButton in roots[i].GetComponentsInChildren<SITouchscreenButton>(true))
			{
				A_1.buttons.Remove(sitouchscreenButton);
			}
		}
	}

	// Token: 0x040008E9 RID: 2281
	public SIGadgetDispenser.GadgetDispenserTerminalState currentState;

	// Token: 0x040008EA RID: 2282
	public SIGadgetDispenser.GadgetDispenserTerminalState lastState;

	// Token: 0x040008EB RID: 2283
	public Transform gadgetDispensePosition;

	// Token: 0x040008EC RID: 2284
	public int _currentNode;

	// Token: 0x040008ED RID: 2285
	public SICombinedTerminal parentTerminal;

	// Token: 0x040008EE RID: 2286
	public GameObject waitingForScanScreen;

	// Token: 0x040008EF RID: 2287
	public GameObject gadgetListScreen;

	// Token: 0x040008F0 RID: 2288
	public GameObject gadgetInformationScreen;

	// Token: 0x040008F1 RID: 2289
	public GameObject gadgetDispensedScreen;

	// Token: 0x040008F2 RID: 2290
	public GameObject gadgetsHelpScreen;

	// Token: 0x040008F3 RID: 2291
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x040008F4 RID: 2292
	[Header("Main Screen Shared")]
	public TextMeshProUGUI screenDescription;

	// Token: 0x040008F5 RID: 2293
	public Image background;

	// Token: 0x040008F6 RID: 2294
	public Color active;

	// Token: 0x040008F7 RID: 2295
	public Color notActive;

	// Token: 0x040008F8 RID: 2296
	public Transform uiCenter;

	// Token: 0x040008F9 RID: 2297
	[Header("Popup Shared")]
	public GameObject popupScreen;

	// Token: 0x040008FA RID: 2298
	[Header("Gadgets Type")]
	[SerializeField]
	private RectTransform pageListParent;

	// Token: 0x040008FB RID: 2299
	[SerializeField]
	private SIGadgetListEntry pageListEntryPrefab;

	// Token: 0x040008FC RID: 2300
	private List<SIGadgetListEntry> gadgetPages;

	// Token: 0x040008FD RID: 2301
	[FormerlySerializedAs("noDispensableGadgetsNotif")]
	[Header("Gadgets List")]
	[SerializeField]
	private GameObject noDispensableGadgetsMessage;

	// Token: 0x040008FE RID: 2302
	[SerializeField]
	private RectTransform gadgetListParent;

	// Token: 0x040008FF RID: 2303
	[SerializeField]
	private SIDispenserGadgetListEntry gadgetListEntryPrefab;

	// Token: 0x04000900 RID: 2304
	private List<SIDispenserGadgetListEntry> gadgetEntries;

	// Token: 0x04000901 RID: 2305
	[Header("Gadgets Description")]
	public TextMeshProUGUI gadgetDescriptionText;

	// Token: 0x04000902 RID: 2306
	[Header("Gadget Dispensed")]
	public TextMeshProUGUI gadgetDispensedText;

	// Token: 0x04000903 RID: 2307
	[Header("Help")]
	public int helpScreenIndex;

	// Token: 0x04000904 RID: 2308
	public GameObject[] helpPopupScreens;

	// Token: 0x04000905 RID: 2309
	[Header("Audio")]
	[SerializeField]
	private SoundBankPlayer touchSoundBankPlayer;

	// Token: 0x04000906 RID: 2310
	[SerializeField]
	private SoundBankPlayer dispenseSoundBankPlayer;

	// Token: 0x04000907 RID: 2311
	[Header("Main Screen Colliders")]
	[Tooltip("Button colliders to disable while popup screen is shown.  Gets updated live to include page and gadget buttons.")]
	[SerializeField]
	private List<Collider> _nonPopupButtonColliders;

	// Token: 0x04000908 RID: 2312
	private Dictionary<SIGadgetDispenser.GadgetDispenserTerminalState, GameObject> screenData;

	// Token: 0x04000909 RID: 2313
	private bool initialized;

	// Token: 0x02000117 RID: 279
	public enum GadgetDispenserTerminalState
	{
		// Token: 0x0400090B RID: 2315
		WaitingForScan,
		// Token: 0x0400090C RID: 2316
		GadgetList,
		// Token: 0x0400090D RID: 2317
		GadgetInformation,
		// Token: 0x0400090E RID: 2318
		GadgetDispensed,
		// Token: 0x0400090F RID: 2319
		HelpScreen
	}
}
