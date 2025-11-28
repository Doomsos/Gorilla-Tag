using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000712 RID: 1810
public class GRTool : MonoBehaviour, IGameEntitySerialize, IGameEntityComponent, IGameEntityDebugComponent
{
	// Token: 0x1400004E RID: 78
	// (add) Token: 0x06002E68 RID: 11880 RVA: 0x000FC220 File Offset: 0x000FA420
	// (remove) Token: 0x06002E69 RID: 11881 RVA: 0x000FC258 File Offset: 0x000FA458
	public event GRTool.EnergyChangeEvent OnEnergyChange;

	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06002E6A RID: 11882 RVA: 0x000FC290 File Offset: 0x000FA490
	// (remove) Token: 0x06002E6B RID: 11883 RVA: 0x000FC2C8 File Offset: 0x000FA4C8
	public event GRTool.ToolUpgradedEvent onToolUpgraded;

	// Token: 0x06002E6C RID: 11884 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x000FC2FD File Offset: 0x000FA4FD
	private void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.RefreshMeters();
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x000FC320 File Offset: 0x000FA520
	public void OnEntityInit()
	{
		this.energy = this.GetEnergyStart();
		GhostReactor.ToolEntityCreateData toolEntityCreateData = GhostReactor.ToolEntityCreateData.Unpack(this.gameEntity.createData);
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null)
		{
			GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = ghostReactorManager.GetToolUpgradeStationFullForIndex(toolEntityCreateData.stationIndex);
			if (toolUpgradeStationFullForIndex != null)
			{
				toolUpgradeStationFullForIndex.InitLinkedEntity(this.gameEntity);
			}
		}
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002E70 RID: 11888 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x000FC381 File Offset: 0x000FA581
	public int GetEnergyMax()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyMax);
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x000FC38F File Offset: 0x000FA58F
	public int GetEnergyUseCost()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyUseCost);
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x000FC39D File Offset: 0x000FA59D
	public int GetEnergyStart()
	{
		if (!this.attributes.HasValueForAttribute(GRAttributeType.EnergyStart))
		{
			return 0;
		}
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyStart);
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x000FC3BB File Offset: 0x000FA5BB
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x000FC3E4 File Offset: 0x000FA5E4
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x000FC40D File Offset: 0x000FA60D
	public void RefillEnergy(int count, GameEntityId chargingEntityId)
	{
		this.SetEnergyInternal(this.energy + count, chargingEntityId);
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x000FC41E File Offset: 0x000FA61E
	public void RefillEnergy()
	{
		this.SetEnergyInternal(this.GetEnergyMax(), GameEntityId.Invalid);
	}

	// Token: 0x06002E78 RID: 11896 RVA: 0x000FC431 File Offset: 0x000FA631
	public void UseEnergy()
	{
		this.SetEnergyInternal(this.energy - this.GetEnergyUseCost(), GameEntityId.Invalid);
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x000FC44B File Offset: 0x000FA64B
	public bool HasEnoughEnergy()
	{
		return this.energy >= this.GetEnergyUseCost();
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000FC45E File Offset: 0x000FA65E
	public void SetEnergy(int newEnergy)
	{
		this.SetEnergyInternal(newEnergy, GameEntityId.Invalid);
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000FC46C File Offset: 0x000FA66C
	public bool IsEnergyFull()
	{
		return this.energy >= this.GetEnergyMax();
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x000FC480 File Offset: 0x000FA680
	private void SetEnergyInternal(int value, GameEntityId chargingEntityId)
	{
		int num = this.energy;
		this.energy = Mathf.Clamp(value, 0, this.GetEnergyMax());
		int energyChange = this.energy - num;
		GRTool.EnergyChangeEvent onEnergyChange = this.OnEnergyChange;
		if (onEnergyChange != null)
		{
			onEnergyChange(this, energyChange, chargingEntityId);
		}
		this.RefreshMeters();
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x000FC4CC File Offset: 0x000FA6CC
	public void RefreshMeters()
	{
		for (int i = 0; i < this.energyMeters.Count; i++)
		{
			this.energyMeters[i].Refresh();
		}
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x000FC500 File Offset: 0x000FA700
	public bool HasUpgradeInstalled(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i].installedItem != null && this.upgradeSlots[i].installedItem.UpgradeType == upgradeID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000FC554 File Offset: 0x000FA754
	public GRTool.Upgrade FindMatchingUpgrade(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].UpgradeType == upgradeID)
			{
				return this.upgrades[i];
			}
		}
		return null;
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000FC59C File Offset: 0x000FA79C
	public float GetPointDistanceToUpgrade(Vector3 point, GRTool.Upgrade upgrade)
	{
		if (upgrade.VisibleItem.Count < 1)
		{
			return -1f;
		}
		if (this.upgradeListsAreValidFor != upgrade)
		{
			this.reservedMeshFilterSearchList.Clear();
			upgrade.VisibleItem[0].GetComponentsInChildren<MeshFilter>(this.reservedMeshFilterSearchList);
			this.reservedMeshFilterSearchListSkinned.Clear();
			upgrade.VisibleItem[0].GetComponentsInChildren<SkinnedMeshRenderer>(false, this.reservedMeshFilterSearchListSkinned);
			this.upgradeListsAreValidFor = upgrade;
		}
		float num = float.MaxValue;
		foreach (MeshFilter meshFilter in this.reservedMeshFilterSearchList)
		{
			Vector3 vector = meshFilter.transform.InverseTransformPoint(point);
			Bounds bounds = meshFilter.sharedMesh.bounds;
			Vector3 vector2;
			vector2..ctor(Mathf.Clamp(vector.x, bounds.min.x, bounds.max.x), Mathf.Clamp(vector.y, bounds.min.y, bounds.max.y), Mathf.Clamp(vector.z, bounds.min.z, bounds.max.z));
			Vector3 vector3 = vector - vector2;
			float sqrMagnitude = meshFilter.transform.TransformVector(vector3).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
			}
		}
		if (this.reservedMeshFilterSearchListSkinned != null)
		{
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.reservedMeshFilterSearchListSkinned)
			{
				Vector3 vector4 = skinnedMeshRenderer.transform.InverseTransformPoint(point);
				Bounds localBounds = skinnedMeshRenderer.localBounds;
				Vector3 vector5;
				vector5..ctor(Mathf.Clamp(vector4.x, localBounds.min.x, localBounds.max.x), Mathf.Clamp(vector4.y, localBounds.min.y, localBounds.max.y), Mathf.Clamp(vector4.z, localBounds.min.z, localBounds.max.z));
				Vector3 vector6 = vector4 - vector5;
				float sqrMagnitude2 = skinnedMeshRenderer.transform.TransformVector(vector6).sqrMagnitude;
				if (sqrMagnitude2 < num)
				{
					num = sqrMagnitude2;
				}
			}
		}
		if (num == 3.4028235E+38f)
		{
			return Vector3.Distance(point, upgrade.VisibleItem[0].transform.position);
		}
		return Mathf.Sqrt(num);
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x000FC838 File Offset: 0x000FAA38
	public Transform GetUpgradeAttachTransform(GRTool.Upgrade upgrade)
	{
		if (upgrade.VisibleItem.Count < 1)
		{
			return null;
		}
		return upgrade.VisibleItem[0].transform;
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000FC85C File Offset: 0x000FAA5C
	public void UpgradeTool(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].UpgradeType == upgradeID)
			{
				this.ClearUpgradeSlot(this.upgrades[i].Slot);
				for (int j = 0; j < this.upgrades[i].VisibleItem.Count; j++)
				{
					this.upgrades[i].VisibleItem[j].SetActive(true);
				}
				for (int k = 0; k < this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems.Count; k++)
				{
					this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems[k].SetActive(false);
				}
				foreach (GRBonusEntry entry in this.upgrades[i].bonusEffects)
				{
					this.attributes.AddBonus(entry);
				}
				this.upgradeSlots[this.upgrades[i].Slot].installedItem = this.upgrades[i];
				if (this.UpgradeFXNode != null && this.upgrades[i].VisibleItem.Count > 0)
				{
					this.UpgradeFXNode.transform.position = this.upgrades[i].VisibleItem[0].transform.position;
					this.UpgradeFXNode.transform.rotation = this.upgrades[i].VisibleItem[0].transform.rotation;
					ParticleSystem componentInChildren = this.UpgradeFXNode.GetComponentInChildren<ParticleSystem>();
					AudioSource componentInChildren2 = this.UpgradeFXNode.GetComponentInChildren<AudioSource>();
					if (componentInChildren != null)
					{
						componentInChildren.Play();
					}
					if (componentInChildren2 != null)
					{
						componentInChildren2.Play();
					}
				}
			}
		}
		GRTool.ToolUpgradedEvent toolUpgradedEvent = this.onToolUpgraded;
		if (toolUpgradedEvent == null)
		{
			return;
		}
		toolUpgradedEvent(this);
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000FCAB0 File Offset: 0x000FACB0
	public void ClearUpgradeSlot(int slot)
	{
		if (this.upgradeSlots[slot].installedItem != null)
		{
			for (int i = 0; i < this.upgradeSlots[slot].installedItem.VisibleItem.Count; i++)
			{
				this.upgradeSlots[slot].installedItem.VisibleItem[i].SetActive(false);
			}
			foreach (GRBonusEntry entry in this.upgradeSlots[slot].installedItem.bonusEffects)
			{
				this.attributes.RemoveBonus(entry);
			}
			for (int j = 0; j < this.upgradeSlots[slot].DefaultVisibleItems.Count; j++)
			{
				this.upgradeSlots[slot].DefaultVisibleItems[j].SetActive(true);
			}
		}
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000FCBB4 File Offset: 0x000FADB4
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		writer.Write(this.upgradeSlots.Count);
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i] != null)
			{
				if (this.upgradeSlots[i].installedItem != null)
				{
					writer.Write(this.upgradeSlots[i].installedItem.UpgradeType.ToString());
				}
				else
				{
					writer.Write("");
				}
			}
			else
			{
				writer.Write("");
			}
		}
		writer.Write(this.energy);
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000FCC58 File Offset: 0x000FAE58
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GRToolProgressionManager.ToolParts upgradeID = GRToolProgressionManager.ToolParts.None;
			if (Enum.TryParse<GRToolProgressionManager.ToolParts>(reader.ReadString(), ref upgradeID))
			{
				this.UpgradeTool(upgradeID);
			}
		}
		int num2 = reader.ReadInt32();
		this.SetEnergy(num2);
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000FCCA0 File Offset: 0x000FAEA0
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000FCCFB File Offset: 0x000FAEFB
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Tool Energy: <color=\"yellow\">{0}<color=\"white\"> ", this.energy));
	}

	// Token: 0x04003C8A RID: 15498
	public GRAttributes attributes;

	// Token: 0x04003C8B RID: 15499
	public List<GRTool.Upgrade> upgrades;

	// Token: 0x04003C8C RID: 15500
	public List<GRTool.UpgradeSlot> upgradeSlots = new List<GRTool.UpgradeSlot>();

	// Token: 0x04003C8D RID: 15501
	public List<GRMeterEnergy> energyMeters;

	// Token: 0x04003C8E RID: 15502
	public GameEntity gameEntity;

	// Token: 0x04003C8F RID: 15503
	public GRTool.GRToolType toolType;

	// Token: 0x04003C90 RID: 15504
	[ReadOnly]
	public int energy;

	// Token: 0x04003C92 RID: 15506
	public GameObject UpgradeFXNode;

	// Token: 0x04003C94 RID: 15508
	private List<MeshFilter> reservedMeshFilterSearchList = new List<MeshFilter>(32);

	// Token: 0x04003C95 RID: 15509
	private List<SkinnedMeshRenderer> reservedMeshFilterSearchListSkinned = new List<SkinnedMeshRenderer>(32);

	// Token: 0x04003C96 RID: 15510
	private GRTool.Upgrade upgradeListsAreValidFor;

	// Token: 0x02000713 RID: 1811
	public enum GRToolType
	{
		// Token: 0x04003C98 RID: 15512
		None,
		// Token: 0x04003C99 RID: 15513
		Club,
		// Token: 0x04003C9A RID: 15514
		Collector,
		// Token: 0x04003C9B RID: 15515
		Flash,
		// Token: 0x04003C9C RID: 15516
		Lantern,
		// Token: 0x04003C9D RID: 15517
		Revive,
		// Token: 0x04003C9E RID: 15518
		ShieldGun,
		// Token: 0x04003C9F RID: 15519
		DirectionalShield,
		// Token: 0x04003CA0 RID: 15520
		DockWrist,
		// Token: 0x04003CA1 RID: 15521
		EnergyEfficiency,
		// Token: 0x04003CA2 RID: 15522
		DropPod,
		// Token: 0x04003CA3 RID: 15523
		HockeyStick,
		// Token: 0x04003CA4 RID: 15524
		StatusWatch,
		// Token: 0x04003CA5 RID: 15525
		RattyBackpack
	}

	// Token: 0x02000714 RID: 1812
	[Serializable]
	public class Upgrade
	{
		// Token: 0x04003CA6 RID: 15526
		public GRToolProgressionManager.ToolParts UpgradeType;

		// Token: 0x04003CA7 RID: 15527
		public int Slot;

		// Token: 0x04003CA8 RID: 15528
		public List<GameObject> VisibleItem;

		// Token: 0x04003CA9 RID: 15529
		public List<GRBonusEntry> bonusEffects;
	}

	// Token: 0x02000715 RID: 1813
	[Serializable]
	public class UpgradeSlot
	{
		// Token: 0x04003CAA RID: 15530
		public List<GameObject> DefaultVisibleItems;

		// Token: 0x04003CAB RID: 15531
		[NonSerialized]
		public GRTool.Upgrade installedItem;
	}

	// Token: 0x02000716 RID: 1814
	// (Invoke) Token: 0x06002E8C RID: 11916
	public delegate void EnergyChangeEvent(GRTool tool, int energyChange, GameEntityId chargingEntityId);

	// Token: 0x02000717 RID: 1815
	// (Invoke) Token: 0x06002E90 RID: 11920
	public delegate void ToolUpgradedEvent(GRTool tool);
}
