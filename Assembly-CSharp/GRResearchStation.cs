using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020006EF RID: 1775
public class GRResearchStation : MonoBehaviour
{
	// Token: 0x06002D74 RID: 11636 RVA: 0x000F5DFC File Offset: 0x000F3FFC
	public void Init(GRToolProgressionManager tree, GhostReactor ghostReactor)
	{
		this.toolProgressionManager = tree;
		this.toolProgressionManager.OnProgressionUpdated += new Action(this.ResearchTreeUpdated);
		this.reactor = ghostReactor;
		this.totalTools = 0;
		this.selectedToolIndex = 0;
		this._levelString = this.LevelText.text;
		this._costString = this.CostText.text;
		this._researchPointsString = this.ResearchPointsTex.text;
		this._requiredLevelString = this.RequiredLevelText.text;
		this.UpdateUI();
		this.SelectTool(0);
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x000F5E90 File Offset: 0x000F4090
	private void SelectTool(int index)
	{
		if (this.toolProgressionManager == null || this.totalTools == 0)
		{
			return;
		}
		if (index < this.totalTools && index > -1)
		{
			this.selectedToolIndex = index;
			this.selectedToolUpgrades = this.toolProgressionManager.GetToolUpgrades(this.supportedTools[this.selectedToolIndex]);
			this.SelectUpgrade(0);
			this.UpdateUI();
		}
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x000F5EF7 File Offset: 0x000F40F7
	public void ResearchTreeUpdated()
	{
		this.supportedTools = this.toolProgressionManager.GetSupportedTools();
		this.totalTools = this.supportedTools.Count;
		this.SelectTool(this.selectedToolIndex);
		this.UpdateUI();
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x000F5F2D File Offset: 0x000F412D
	public void UpdateUI()
	{
		this.UpdateToolName();
		this.UpdateUpgradeTitles();
		this.UpdateLocked();
		this.UpdateRequiredLevel();
		this.UpdateCost();
		this.UpdateResearchPoints(this.toolProgressionManager.GetNumberOfResearchPoints());
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x000F5F60 File Offset: 0x000F4160
	public void SelectUpgrade(int UpgradeIndex)
	{
		if (this.toolProgressionManager == null)
		{
			return;
		}
		this.selectedUpgradeIndex = UpgradeIndex;
		if (this.selectedToolUpgrades.Count > this.selectedUpgradeIndex)
		{
			this.currentlySelectedToolUpgrade = this.selectedToolUpgrades[this.selectedUpgradeIndex];
			this.currentlySelectedUpgradeMetadata = this.currentlySelectedToolUpgrade.partMetadata;
			this.SetUpgradeTextColors(this.selectedUpgradeIndex);
			this.UpdateDescriptionText(this.currentlySelectedUpgradeMetadata.description);
		}
		this.UpdateUI();
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x000F5FE4 File Offset: 0x000F41E4
	private void SetUpgradeTextColors(int index)
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeButton[i].isOn = false;
			this.UpgradeButton[i].UpdateColor();
		}
		this.UpgradeButton[index].isOn = true;
		this.UpgradeButton[index].UpdateColor();
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x000F603C File Offset: 0x000F423C
	private void UpdateUpgradeTitles()
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.totalTools >= this.selectedToolIndex && this.selectedToolUpgrades.Count > i)
			{
				this.UpgradeTitlesText[i].text = this.selectedToolUpgrades[i].partMetadata.name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x000F60AC File Offset: 0x000F42AC
	public void UpdateLocked()
	{
		if (this.currentlySelectedToolUpgrade.unlocked)
		{
			this.UnlockedText.color = this.unlockedToolColor;
			this.UnlockedText.text = "UNLOCKED";
		}
		else
		{
			this.UnlockedText.color = this.lockedToolColor;
			this.UnlockedText.text = "LOCKED";
		}
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.totalTools >= this.selectedToolIndex && this.selectedToolUpgrades.Count > i)
			{
				bool unlocked = this.selectedToolUpgrades[i].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedToolColor : this.lockedToolColor);
				this.LockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeTitlesText[i].color = Color.black;
				this.LockedImage[i].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x000F61AC File Offset: 0x000F43AC
	public void UpdateRequiredLevel()
	{
		int requiredEmployeeLevel = this.toolProgressionManager.GetRequiredEmployeeLevel(this.currentlySelectedToolUpgrade.requiredEmployeeLevel);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(requiredEmployeeLevel);
		int num = 0;
		GRPlayer grplayer = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			num = GhostReactorProgression.GetTitleLevel(grplayer.CurrentProgression.redeemedPoints);
		}
		string titleNameFromLevel2 = GhostReactorProgression.GetTitleNameFromLevel(num);
		this.RequiredLevelText.text = string.Format(this._requiredLevelString, titleNameFromLevel);
		this.LevelText.text = string.Format(this._levelString, titleNameFromLevel2);
		this.RequiredLevelText.color = ((num >= requiredEmployeeLevel) ? this.unlockedToolColor : this.lockedToolColor);
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x000F6257 File Offset: 0x000F4457
	public void UpdateDescriptionText(string description)
	{
		this.DescriptionText.text = description;
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x000F6268 File Offset: 0x000F4468
	public void UpdateCost()
	{
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > 0 && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex)
		{
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
			this.CostText.text = string.Format(this._costString, researchCost);
			this.CostText.color = ((numberOfResearchPoints >= researchCost) ? this.unlockedToolColor : this.lockedToolColor);
		}
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x000F62FA File Offset: 0x000F44FA
	public void UpdateToolName()
	{
		if (this.supportedTools.Count > 0)
		{
			this.ToolNameText.text = GRUtils.GetToolName(this.supportedTools[this.selectedToolIndex]);
		}
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x000F632B File Offset: 0x000F452B
	public void UpdateResearchPoints(int ResearchPoints)
	{
		this.ResearchPointsTex.text = string.Format(this._researchPointsString, ResearchPoints);
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x000F6349 File Offset: 0x000F4549
	public void MFDButton0Pressed()
	{
		this.SelectUpgrade(0);
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x000F6352 File Offset: 0x000F4552
	public void MFDButton1Pressed()
	{
		this.SelectUpgrade(1);
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x000F635B File Offset: 0x000F455B
	public void MFDButton2Pressed()
	{
		this.SelectUpgrade(2);
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x000F6364 File Offset: 0x000F4564
	public void MFDButton3Pressed()
	{
		this.SelectUpgrade(3);
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x000F636D File Offset: 0x000F456D
	public void MFDButton4Pressed()
	{
		this.SelectUpgrade(4);
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x000F6376 File Offset: 0x000F4576
	public void MFDButton5Pressed()
	{
		this.SelectUpgrade(5);
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x000F637F File Offset: 0x000F457F
	public void NextToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex + 1) % this.totalTools;
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x06002D88 RID: 11656 RVA: 0x000F63A2 File Offset: 0x000F45A2
	public void PreviousToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex - 1).PositiveModulo(this.totalTools);
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x000F63C9 File Offset: 0x000F45C9
	public void UpgradeButtonPressed()
	{
		UnityEvent onSucceeded = this.scanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		GhostReactorProgression.instance.UnlockProgressionTreeNode(this.toolProgressionManager.GetTreeId(), this.currentlySelectedToolUpgrade.id, this.reactor);
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x000F6407 File Offset: 0x000F4607
	public void ResearchCompleted(bool success, string researchID)
	{
		this.UpdateUI();
	}

	// Token: 0x04003B20 RID: 15136
	public Color selectedUpgradeColor = Color.yellow;

	// Token: 0x04003B21 RID: 15137
	public Color unselectedUpgradeColor = Color.black;

	// Token: 0x04003B22 RID: 15138
	public Color lockedToolColor = Color.red;

	// Token: 0x04003B23 RID: 15139
	public Color unlockedToolColor = Color.green;

	// Token: 0x04003B24 RID: 15140
	private int selectedUpgradeIndex;

	// Token: 0x04003B25 RID: 15141
	[SerializeField]
	private IDCardScanner scanner;

	// Token: 0x04003B26 RID: 15142
	[SerializeField]
	private TMP_Text BonusText;

	// Token: 0x04003B27 RID: 15143
	[SerializeField]
	private TMP_Text CostText;

	// Token: 0x04003B28 RID: 15144
	[SerializeField]
	private TMP_Text DescriptionText;

	// Token: 0x04003B29 RID: 15145
	[SerializeField]
	private TMP_Text LevelText;

	// Token: 0x04003B2A RID: 15146
	[SerializeField]
	private TMP_Text ResearchPointsTex;

	// Token: 0x04003B2B RID: 15147
	[SerializeField]
	private TMP_Text RequiredLevelText;

	// Token: 0x04003B2C RID: 15148
	[SerializeField]
	private TMP_Text ToolNameText;

	// Token: 0x04003B2D RID: 15149
	[SerializeField]
	private TMP_Text UnlockedText;

	// Token: 0x04003B2E RID: 15150
	[SerializeField]
	private TMP_Text[] UpgradePointerText;

	// Token: 0x04003B2F RID: 15151
	[SerializeField]
	private TMP_Text[] UpgradeTitlesText;

	// Token: 0x04003B30 RID: 15152
	[SerializeField]
	private Image[] LockedImage;

	// Token: 0x04003B31 RID: 15153
	[SerializeField]
	private GorillaPressableButton[] UpgradeButton;

	// Token: 0x04003B32 RID: 15154
	private string _costString;

	// Token: 0x04003B33 RID: 15155
	private string _levelString;

	// Token: 0x04003B34 RID: 15156
	private string _researchPointsString;

	// Token: 0x04003B35 RID: 15157
	private string _requiredLevelString;

	// Token: 0x04003B36 RID: 15158
	private int selectedToolIndex;

	// Token: 0x04003B37 RID: 15159
	private int totalTools;

	// Token: 0x04003B38 RID: 15160
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04003B39 RID: 15161
	[NonSerialized]
	private List<GRTool.GRToolType> supportedTools = new List<GRTool.GRToolType>();

	// Token: 0x04003B3A RID: 15162
	[NonSerialized]
	private List<GRToolProgressionTree.GRToolProgressionNode> selectedToolUpgrades = new List<GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04003B3B RID: 15163
	[NonSerialized]
	private GRToolProgressionTree.GRToolProgressionNode currentlySelectedToolUpgrade = new GRToolProgressionTree.GRToolProgressionNode();

	// Token: 0x04003B3C RID: 15164
	[NonSerialized]
	private GRToolProgressionManager.ToolProgressionMetaData currentlySelectedUpgradeMetadata = new GRToolProgressionManager.ToolProgressionMetaData();

	// Token: 0x04003B3D RID: 15165
	[NonSerialized]
	private GhostReactor reactor;
}
