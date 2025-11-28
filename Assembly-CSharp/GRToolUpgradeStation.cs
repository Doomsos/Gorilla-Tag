using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200073B RID: 1851
public class GRToolUpgradeStation : MonoBehaviour
{
	// Token: 0x06002FC8 RID: 12232 RVA: 0x001053EF File Offset: 0x001035EF
	public void Init(GRToolProgressionManager tree, GhostReactor reactor)
	{
		this._reactor = reactor;
		this.defaultCostText = this.CostText.text;
		this.toolProgressionManager = tree;
		this.toolProgressionManager.OnProgressionUpdated += new Action(this.ResearchTreeUpdated);
		this.ResetScreen();
	}

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x06002FC9 RID: 12233 RVA: 0x0010542D File Offset: 0x0010362D
	public bool canInsertTool
	{
		get
		{
			return this.currentState == GRToolUpgradeStation.UpgradeStationState.Idle && !this.bIsToolInserted;
		}
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x00105442 File Offset: 0x00103642
	public void ResearchTreeUpdated()
	{
		this.UpdateUI();
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x0010544A File Offset: 0x0010364A
	public void Update()
	{
		if (this.currentState == GRToolUpgradeStation.UpgradeStationState.Upgrading)
		{
			this.UpgradingUpdate(PhotonNetwork.Time);
		}
	}

	// Token: 0x06002FCC RID: 12236 RVA: 0x00105460 File Offset: 0x00103660
	public void ToolInserted(GRTool tool)
	{
		if (!this.canInsertTool)
		{
			return;
		}
		this.bIsToolInserted = true;
		this.insertedTool = tool;
		this.insertedToolType = this.insertedTool.toolType;
		this.selectedToolUpgrades = this.toolProgressionManager.GetToolUpgrades(this.insertedToolType);
		this.ResetScreen();
		this.UpdateUI();
		this.SelectUpgrade(0);
		this.LocalPlacedToolInUpgradeStation(tool.gameEntity.id);
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x001054D0 File Offset: 0x001036D0
	public void UpdateUI()
	{
		this.UpdateUpgradeTexts();
		this.UpdateSelectedUpgrade();
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x001054E0 File Offset: 0x001036E0
	public void UpdateUpgradeTexts()
	{
		this.ToolNameText.text = GRUtils.GetToolName(this.insertedToolType);
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.selectedToolUpgrades.Count > i)
			{
				this.UpgradeTitlesText[i].text = this.selectedToolUpgrades[i].partMetadata.name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x00002789 File Offset: 0x00000989
	public void UnlockAllUpgrades()
	{
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x00105558 File Offset: 0x00103758
	public void UpdateSelectedUpgrade()
	{
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex && this.selectedToolUpgrades[this.selectedUpgradeIndex] != null)
		{
			if (this.selectedToolUpgrades[this.selectedUpgradeIndex].unlocked)
			{
				this.DescriptionText.text = this.selectedToolUpgrades[this.selectedUpgradeIndex].partMetadata.description;
				int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
				this.CostText.text = string.Format(this.defaultCostText, researchCost.ToString());
				GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
				this.CostText.color = ((researchCost > grplayer.ShiftCredits) ? this.lockedColor : this.unlockedColor);
				return;
			}
			this.CostText.text = "NEEDS RESEARCH";
			this.CostText.color = this.lockedColor;
		}
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x00105660 File Offset: 0x00103860
	public void ResetScreen()
	{
		this.DescriptionText.text = "PLEASE INSERT A TOOL";
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeTitlesText[i].text = "----";
			this.UpgradeTitlesText[i].color = this.lockedColor;
			this.MFD_ButtonTexts[i].color = this.unSelectedColor;
		}
		this.ToolNameText.text = "----";
		this.CostText.text = "-";
		this.ToolNameText.color = this.unSelectedColor;
		this.DescriptionText.color = this.unSelectedColor;
		this.CostText.color = this.unSelectedColor;
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x0010571C File Offset: 0x0010391C
	public void SelectUpgrade(int index)
	{
		if (index >= this.selectedToolUpgrades.Count)
		{
			return;
		}
		this.selectedUpgradeIndex = index;
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (i < this.selectedToolUpgrades.Count)
			{
				bool unlocked = this.selectedToolUpgrades[i].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedColor : this.lockedColor);
				this.UpgradeLockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeLockedImage[i].gameObject.SetActive(true);
				this.UpgradeTitlesText[i].color = this.lockedColor;
			}
			this.UpgradeButtons[i].isOn = false;
			this.UpgradeButtons[i].UpdateColor();
		}
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex && this.selectedToolUpgrades[this.selectedUpgradeIndex] != null)
		{
			this.UpgradeButtons[this.selectedUpgradeIndex].isOn = true;
			this.UpgradeButtons[this.selectedUpgradeIndex].UpdateColor();
			this.DescriptionText.color = this.UpgradeTitlesText[this.selectedUpgradeIndex].color;
			this.CostText.color = this.UpgradeTitlesText[this.selectedUpgradeIndex].color;
		}
		this.UpdateUI();
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x00105885 File Offset: 0x00103A85
	public void UpgradeTool()
	{
		this._reactor.grManager.ToolUpgradeStationRequestUpgrade(this.selectedToolUpgrades[this.selectedUpgradeIndex].type, this.insertedToolEntity.GetNetId());
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x001058B8 File Offset: 0x00103AB8
	public void LocalPlacedToolInUpgradeStation(GameEntityId entityId)
	{
		GameEntity gameEntity = this._reactor.grManager.gameEntityManager.GetGameEntity(entityId);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.ItemInserted;
		if (gameEntity.heldByActorNumber >= 0)
		{
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
			int handIndex = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
				GamePlayerLocal.instance.ClearGrabbed(handIndex);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased.Invoke();
			}
			this.PositionInsertedTool(gameEntity);
			this.SelectUpgrade(0);
		}
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x00105964 File Offset: 0x00103B64
	public void PositionInsertedTool(GameEntity entity)
	{
		this.insertedToolEntity = entity;
		entity.transform.SetParent(this.startingLocation);
		entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		entity.pickupable = false;
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x001059F4 File Offset: 0x00103BF4
	public void PayForUpgrade(int Player)
	{
		if (Player == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			bool flag = researchCost <= grplayer.ShiftCredits;
			bool unlocked = this.selectedToolUpgrades[this.selectedUpgradeIndex].unlocked;
			if (flag && unlocked)
			{
				UnityEvent onSucceeded = this.IDCardScanner.onSucceeded;
				if (onSucceeded != null)
				{
					onSucceeded.Invoke();
				}
				this.StartUpgrade(PhotonNetwork.Time);
			}
		}
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x00105A78 File Offset: 0x00103C78
	public void StartUpgrade(double startTime)
	{
		if (this.currentState != GRToolUpgradeStation.UpgradeStationState.ItemInserted)
		{
			return;
		}
		this.upgradeStartTime = startTime;
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Upgrading;
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x00105ACD File Offset: 0x00103CCD
	public void UpgradingUpdate(double currentTime)
	{
		if (currentTime >= this.upgradeStartTime + this.upgradeAnimationLength)
		{
			this.CompleteUpgrade();
		}
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x00105AE5 File Offset: 0x00103CE5
	public void CompleteUpgrade()
	{
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		this.ResetScreen();
		this.MoveToolToFinished();
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x00105AFC File Offset: 0x00103CFC
	public void MoveItemToUpgradeSlot()
	{
		this.insertedToolEntity.transform.SetParent(this.upgradingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.upgradingLocation.position;
			component.rotation = this.upgradingLocation.rotation;
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = false;
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x00105B9C File Offset: 0x00103D9C
	public void MoveToolToFinished()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Complete;
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.UpgradeTool();
		this.EjectToolFromEnd();
		this.ResetScreen();
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x00105C64 File Offset: 0x00103E64
	public void EjectToolFromStart()
	{
		this.insertedToolEntity.transform.SetParent(this.startingLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.startingLocation.position;
			component.rotation = this.startingLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolType = GRTool.GRToolType.None;
		this.bIsToolInserted = false;
		this.ResetScreen();
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00105D50 File Offset: 0x00103F50
	public void EjectToolFromEnd()
	{
		this.insertedToolEntity.transform.SetParent(this.depositedLocation);
		this.insertedToolEntity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		this.insertedToolEntity.transform.SetParent(null, true);
		Rigidbody component = this.insertedToolEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = this.depositedLocation.position;
			component.rotation = this.depositedLocation.rotation;
			component.linearVelocity = this.ejectionTransform.forward * this.ejectionVelocity;
			component.angularVelocity = Vector3.zero;
		}
		this.insertedToolEntity.pickupable = true;
		this.insertedToolEntity = null;
		this.insertedTool = null;
		this.insertedToolType = GRTool.GRToolType.None;
		this.bIsToolInserted = false;
		this.currentState = GRToolUpgradeStation.UpgradeStationState.Idle;
	}

	// Token: 0x04003EAC RID: 16044
	private GRTool insertedTool;

	// Token: 0x04003EAD RID: 16045
	private GRTool.GRToolType insertedToolType;

	// Token: 0x04003EAE RID: 16046
	private GameEntity insertedToolEntity;

	// Token: 0x04003EAF RID: 16047
	[NonSerialized]
	private GhostReactor _reactor;

	// Token: 0x04003EB0 RID: 16048
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04003EB1 RID: 16049
	[NonSerialized]
	private List<GRToolProgressionTree.GRToolProgressionNode> selectedToolUpgrades = new List<GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04003EB2 RID: 16050
	[NonSerialized]
	public bool bIsToolInserted;

	// Token: 0x04003EB3 RID: 16051
	public Transform startingLocation;

	// Token: 0x04003EB4 RID: 16052
	public Transform upgradingLocation;

	// Token: 0x04003EB5 RID: 16053
	public Transform depositedLocation;

	// Token: 0x04003EB6 RID: 16054
	public Transform ejectionTransform;

	// Token: 0x04003EB7 RID: 16055
	public float ejectionVelocity;

	// Token: 0x04003EB8 RID: 16056
	public Color selectedColor;

	// Token: 0x04003EB9 RID: 16057
	public Color unSelectedColor;

	// Token: 0x04003EBA RID: 16058
	public Color lockedColor;

	// Token: 0x04003EBB RID: 16059
	public Color unlockedColor;

	// Token: 0x04003EBC RID: 16060
	public TMP_Text[] UpgradeTitlesText;

	// Token: 0x04003EBD RID: 16061
	public TMP_Text[] MFD_ButtonTexts;

	// Token: 0x04003EBE RID: 16062
	public GorillaPressableButton[] UpgradeButtons;

	// Token: 0x04003EBF RID: 16063
	public Image[] UpgradeLockedImage;

	// Token: 0x04003EC0 RID: 16064
	public TMP_Text ToolNameText;

	// Token: 0x04003EC1 RID: 16065
	public TMP_Text DescriptionText;

	// Token: 0x04003EC2 RID: 16066
	public TMP_Text CostText;

	// Token: 0x04003EC3 RID: 16067
	private string defaultCostText;

	// Token: 0x04003EC4 RID: 16068
	public IDCardScanner IDCardScanner;

	// Token: 0x04003EC5 RID: 16069
	private int selectedUpgradeIndex;

	// Token: 0x04003EC6 RID: 16070
	private double upgradeStartTime;

	// Token: 0x04003EC7 RID: 16071
	public double upgradeAnimationLength;

	// Token: 0x04003EC8 RID: 16072
	public Vector3 rotationAnimation;

	// Token: 0x04003EC9 RID: 16073
	private GRToolUpgradeStation.UpgradeStationState currentState;

	// Token: 0x04003ECA RID: 16074
	public GameEntity attachedItem;

	// Token: 0x0200073C RID: 1852
	private enum UpgradeStationState
	{
		// Token: 0x04003ECC RID: 16076
		Idle,
		// Token: 0x04003ECD RID: 16077
		ItemInserted,
		// Token: 0x04003ECE RID: 16078
		Upgrading,
		// Token: 0x04003ECF RID: 16079
		Complete
	}
}
