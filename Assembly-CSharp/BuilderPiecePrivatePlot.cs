using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000589 RID: 1417
public class BuilderPiecePrivatePlot : MonoBehaviour
{
	// Token: 0x060023E4 RID: 9188 RVA: 0x000BE72E File Offset: 0x000BC92E
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000BE738 File Offset: 0x000BC938
	private void Init()
	{
		if (this.initDone)
		{
			return;
		}
		this.materialProps = new MaterialPropertyBlock();
		this.usedResources = new int[3];
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.tempResourceCount = new int[3];
		this.piece = base.GetComponent<BuilderPiece>();
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
		this.piecesToCount = new Queue<BuilderPiece>(1024);
		this.initDone = true;
		this.privatePlotIndex = -1;
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000BE7C0 File Offset: 0x000BC9C0
	private void Start()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			BuilderTable table = this.piece.GetTable();
			this.doesLocalPlayerOwnAPlot = table.DoesPlayerOwnPlot(PhotonNetwork.LocalPlayer.ActorNumber);
			table.OnLocalPlayerClaimedPlot.AddListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
			this.UpdateVisuals();
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this.piece.GetTable();
			}
		}
		this.buildArea.gameObject.SetActive(true);
		this.buildArea.enabled = true;
		this.buildAreaBounds = this.buildArea.bounds;
		this.buildArea.gameObject.SetActive(false);
		this.buildArea.enabled = false;
		this.zoneRenderers.Clear();
		this.zoneRenderers.Add(this.tmpLabel.GetComponent<Renderer>());
		foreach (BuilderResourceMeter builderResourceMeter2 in this.resourceMeters)
		{
			this.zoneRenderers.AddRange(builderResourceMeter2.GetComponentsInChildren<Renderer>());
		}
		this.zoneRenderers.AddRange(this.borderMeshes);
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000BE97C File Offset: 0x000BCB7C
	private void OnDestroy()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			this.piece.GetTable().OnLocalPlayerClaimedPlot.RemoveListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000BEA00 File Offset: 0x000BCC00
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(this.piece.GetTable().tableZone);
		this.inBuilderZone = flag;
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000BEA2F File Offset: 0x000BCC2F
	private void OnLocalPlayerClaimedPlot(bool claim)
	{
		this.doesLocalPlayerOwnAPlot = claim;
		this.UpdateVisuals();
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000BEA40 File Offset: 0x000BCC40
	public void UpdatePlot()
	{
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainLength == null || BuilderPieceInteractor.instance.heldChainLength.Length < 2)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.initDone)
		{
			this.Init();
		}
		if ((this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.doesLocalPlayerOwnAPlot))
		{
			BuilderPiece parentPiece = BuilderPieceInteractor.instance.prevPotentialPlacement[0].parentPiece;
			BuilderPiece parentPiece2 = BuilderPieceInteractor.instance.prevPotentialPlacement[1].parentPiece;
			bool flag = false;
			if (parentPiece == null && this.leftPotentialParent != null)
			{
				this.isLeftOverPlot = false;
				this.leftPotentialParent = null;
				flag = true;
			}
			else if ((this.leftPotentialParent == null && parentPiece != null) || (parentPiece != null && !parentPiece.Equals(this.leftPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece = parentPiece.GetAttachedBuiltInPiece();
				this.isLeftOverPlot = (attachedBuiltInPiece != null && attachedBuiltInPiece.Equals(this.piece));
				this.leftPotentialParent = parentPiece;
				flag = true;
			}
			if (parentPiece2 == null && this.rightPotentialParent != null)
			{
				this.isRightOverPlot = false;
				this.rightPotentialParent = null;
				flag = true;
			}
			else if ((this.rightPotentialParent == null && parentPiece2 != null) || (parentPiece2 != null && !parentPiece2.Equals(this.rightPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece2 = parentPiece2.GetAttachedBuiltInPiece();
				this.isRightOverPlot = (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.Equals(this.piece));
				this.rightPotentialParent = parentPiece2;
				flag = true;
			}
			if (flag)
			{
				this.UpdateVisuals();
			}
		}
		else if (this.isRightOverPlot || this.isLeftOverPlot)
		{
			this.isRightOverPlot = false;
			this.isLeftOverPlot = false;
			this.UpdateVisuals();
		}
		foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
		{
			builderResourceMeter.UpdateMeterFill();
		}
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000BEC78 File Offset: 0x000BCE78
	public void RecountPlotCost()
	{
		this.Init();
		this.piece.GetChainCost(this.usedResources);
		this.UpdateVisuals();
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000BEC97 File Offset: 0x000BCE97
	public void OnPieceAttachedToPlot(BuilderPiece attachPiece)
	{
		this.AddChainResourcesToCount(attachPiece, true);
		this.UpdateVisuals();
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000BECA7 File Offset: 0x000BCEA7
	public void OnPieceDetachedFromPlot(BuilderPiece detachPiece)
	{
		this.AddChainResourcesToCount(detachPiece, false);
		this.UpdateVisuals();
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000BECB7 File Offset: 0x000BCEB7
	public void ChangeAttachedPieceCount(int delta)
	{
		this.attachedPieceCount += delta;
		this.UpdateVisuals();
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000BECD0 File Offset: 0x000BCED0
	public void AddChainResourcesToCount(BuilderPiece chain, bool attach)
	{
		if (chain == null)
		{
			return;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = 0;
		}
		this.piecesToCount.Enqueue(chain);
		this.AddPieceCostToArray(chain, this.tempResourceCount);
		bool flag = false;
		while (this.piecesToCount.Count > 0 && !flag)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					Debug.LogWarning("Builder plot placing pieces over limits");
					flag = true;
					break;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		for (int j = 0; j < this.usedResources.Length; j++)
		{
			if (attach)
			{
				this.usedResources[j] += this.tempResourceCount[j];
			}
			else
			{
				this.usedResources[j] -= this.tempResourceCount[j];
			}
		}
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000BEDD1 File Offset: 0x000BCFD1
	public void ClaimPlotForPlayerNumber(int player)
	{
		this.owningPlayerActorNumber = player;
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Occupied);
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000BEDE1 File Offset: 0x000BCFE1
	public int GetOwnerActorNumber()
	{
		return this.owningPlayerActorNumber;
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000BEDEC File Offset: 0x000BCFEC
	public void ClearPlot()
	{
		this.Init();
		this.attachedPieceCount = 0;
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000BEE29 File Offset: 0x000BD029
	public void FreePlot()
	{
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000BEE32 File Offset: 0x000BD032
	public bool IsPlotClaimed()
	{
		return this.plotState > BuilderPiecePrivatePlot.PlotState.Vacant;
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000BEE40 File Offset: 0x000BD040
	public bool IsChainUnderCapacity(BuilderPiece chain)
	{
		if (chain == null)
		{
			return true;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = this.usedResources[i];
		}
		this.piecesToCount.Enqueue(chain);
		if (!this.AddPieceCostToArray(chain, this.tempResourceCount))
		{
			return false;
		}
		while (this.piecesToCount.Count > 0)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					return false;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		return true;
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000BEEF4 File Offset: 0x000BD0F4
	public bool AddPieceCostToArray(BuilderPiece addedPiece, int[] array)
	{
		if (addedPiece == null)
		{
			return true;
		}
		if (addedPiece.cost != null)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in addedPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					array[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
					if (array[(int)builderResourceQuantity.type] > this.piece.GetTable().GetPrivateResourceLimitForType((int)builderResourceQuantity.type))
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x000BEFB0 File Offset: 0x000BD1B0
	public bool CanPlayerAttachToPlot(int actorNumber)
	{
		return (this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == actorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.piece.GetTable().DoesPlayerOwnPlot(actorNumber));
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000BEFE4 File Offset: 0x000BD1E4
	public bool CanPlayerGrabFromPlot(int actorNumber, Vector3 worldPosition)
	{
		if (this.owningPlayerActorNumber == actorNumber || this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			return true;
		}
		int pieceId;
		if (this.piece.GetTable().plotOwners.TryGetValue(actorNumber, ref pieceId))
		{
			BuilderPiece builderPiece = this.piece.GetTable().GetPiece(pieceId);
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (builderPiece != null && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				return builderPiecePrivatePlot.IsLocationWithinPlotExtents(worldPosition);
			}
		}
		return false;
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000BF04C File Offset: 0x000BD24C
	private void SetPlotState(BuilderPiecePrivatePlot.PlotState newState)
	{
		this.plotState = newState;
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState == BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				if (this.tmpLabel != null && NetworkSystem.Instance != null)
				{
					string text = string.Empty;
					NetPlayer player = NetworkSystem.Instance.GetPlayer(this.owningPlayerActorNumber);
					RigContainer rigContainer;
					if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						text = rigContainer.Rig.playerNameVisible;
					}
					if (string.IsNullOrEmpty(text) && !this.tmpLabel.text.Equals("OCCUPIED"))
					{
						this.tmpLabel.text = "OCCUPIED";
					}
					else if (!this.tmpLabel.text.Equals(text))
					{
						this.tmpLabel.text = text;
					}
				}
				else if (this.tmpLabel != null && !this.tmpLabel.text.Equals("OCCUPIED"))
				{
					this.tmpLabel.text = "OCCUPIED";
				}
			}
		}
		else
		{
			this.owningPlayerActorNumber = -1;
			if (this.tmpLabel != null && !this.tmpLabel.text.Equals(string.Empty))
			{
				this.tmpLabel.text = string.Empty;
			}
		}
		this.UpdateVisuals();
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000BF1A0 File Offset: 0x000BD3A0
	public bool IsLocationWithinPlotExtents(Vector3 worldPosition)
	{
		if (!this.buildAreaBounds.Contains(worldPosition))
		{
			return false;
		}
		Vector3 vector = this.buildArea.transform.InverseTransformPoint(worldPosition);
		Vector3 vector2 = this.buildArea.center + this.buildArea.size / 2f;
		Vector3 vector3 = this.buildArea.center - this.buildArea.size / 2f;
		return vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z;
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000BF274 File Offset: 0x000BD474
	public void OnAvailableResourceChange()
	{
		this.UpdateVisuals();
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000BF27C File Offset: 0x000BD47C
	private void UpdateVisuals()
	{
		if (this.usedResources == null || this.piece.GetTable() == null)
		{
			return;
		}
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState != BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				return;
			}
			if (this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			int num = 0;
			while (num < this.resourceMeters.Count && num < 3)
			{
				int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(num);
				if (privateResourceLimitForType != 0)
				{
					this.resourceMeters[num].SetNormalizedFillTarget((float)(privateResourceLimitForType - this.usedResources[num]) / (float)privateResourceLimitForType);
				}
				num++;
			}
		}
		else
		{
			if (!this.doesLocalPlayerOwnAPlot)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			for (int i = 0; i < this.resourceMeters.Count; i++)
			{
				if (i >= 3)
				{
					return;
				}
				int privateResourceLimitForType2 = this.piece.GetTable().GetPrivateResourceLimitForType(i);
				if (privateResourceLimitForType2 != 0)
				{
					this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType2 - this.usedResources[i]) / (float)privateResourceLimitForType2);
				}
			}
			return;
		}
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000BF398 File Offset: 0x000BD598
	private void UpdateVisualsForOwner()
	{
		bool flag = true;
		if (this.usedResources == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainCost == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = this.usedResources[i];
			if (this.isLeftOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[0][i];
			}
			if (this.isRightOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[1][i];
			}
			int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(i);
			if (num < privateResourceLimitForType)
			{
				flag = false;
			}
			if (privateResourceLimitForType != 0 && this.resourceMeters.Count > i)
			{
				this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType - num) / (float)privateResourceLimitForType);
			}
		}
		if (flag)
		{
			this.SetBorderColor(this.placementDisallowedColor);
			return;
		}
		this.SetBorderColor(this.placementAllowedColor);
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000BF484 File Offset: 0x000BD684
	private void SetBorderColor(Color color)
	{
		this.borderMeshes[0].GetPropertyBlock(this.materialProps);
		this.materialProps.SetColor(ShaderProps._BaseColor, color);
		foreach (MeshRenderer meshRenderer in this.borderMeshes)
		{
			meshRenderer.SetPropertyBlock(this.materialProps);
		}
	}

	// Token: 0x04002EDD RID: 11997
	[SerializeField]
	private Color placementAllowedColor;

	// Token: 0x04002EDE RID: 11998
	[SerializeField]
	private Color placementDisallowedColor;

	// Token: 0x04002EDF RID: 11999
	[SerializeField]
	private Color overCapacityColor;

	// Token: 0x04002EE0 RID: 12000
	public List<MeshRenderer> borderMeshes;

	// Token: 0x04002EE1 RID: 12001
	public BoxCollider buildArea;

	// Token: 0x04002EE2 RID: 12002
	[SerializeField]
	private TMP_Text tmpLabel;

	// Token: 0x04002EE3 RID: 12003
	[SerializeField]
	private List<BuilderResourceMeter> resourceMeters;

	// Token: 0x04002EE4 RID: 12004
	[NonSerialized]
	public int[] usedResources;

	// Token: 0x04002EE5 RID: 12005
	[NonSerialized]
	public int[] tempResourceCount;

	// Token: 0x04002EE6 RID: 12006
	[SerializeField]
	private GameObject plotClaimedFX;

	// Token: 0x04002EE7 RID: 12007
	private BuilderPiece leftPotentialParent;

	// Token: 0x04002EE8 RID: 12008
	private BuilderPiece rightPotentialParent;

	// Token: 0x04002EE9 RID: 12009
	private bool isLeftOverPlot;

	// Token: 0x04002EEA RID: 12010
	private bool isRightOverPlot;

	// Token: 0x04002EEB RID: 12011
	private Bounds buildAreaBounds;

	// Token: 0x04002EEC RID: 12012
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x04002EED RID: 12013
	private int owningPlayerActorNumber;

	// Token: 0x04002EEE RID: 12014
	private int attachedPieceCount;

	// Token: 0x04002EEF RID: 12015
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x04002EF0 RID: 12016
	[HideInInspector]
	public BuilderPiecePrivatePlot.PlotState plotState;

	// Token: 0x04002EF1 RID: 12017
	private bool doesLocalPlayerOwnAPlot;

	// Token: 0x04002EF2 RID: 12018
	private Queue<BuilderPiece> piecesToCount;

	// Token: 0x04002EF3 RID: 12019
	private bool initDone;

	// Token: 0x04002EF4 RID: 12020
	private MaterialPropertyBlock materialProps;

	// Token: 0x04002EF5 RID: 12021
	private List<Renderer> zoneRenderers = new List<Renderer>(12);

	// Token: 0x04002EF6 RID: 12022
	private bool inBuilderZone;

	// Token: 0x0200058A RID: 1418
	public enum PlotState
	{
		// Token: 0x04002EF8 RID: 12024
		Vacant,
		// Token: 0x04002EF9 RID: 12025
		Occupied
	}
}
