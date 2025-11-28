using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BoingKit;
using CjLib;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DBD RID: 3517
	public class BuilderTable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x0600568F RID: 22159 RVA: 0x001B3CC5 File Offset: 0x001B1EC5
		// (set) Token: 0x06005690 RID: 22160 RVA: 0x001B3CCD File Offset: 0x001B1ECD
		public bool TickRunning { get; set; }

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06005691 RID: 22161 RVA: 0x001B3CD6 File Offset: 0x001B1ED6
		[HideInInspector]
		public float gridSize
		{
			get
			{
				return this.pieceScale / 2f;
			}
		}

		// Token: 0x06005692 RID: 22162 RVA: 0x001B3CE4 File Offset: 0x001B1EE4
		private void ExecuteAction(BuilderAction action)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(action.pieceId);
			BuilderPiece piece2 = this.GetPiece(action.parentPieceId);
			int playerActorNumber = action.playerActorNumber;
			bool flag = PhotonNetwork.LocalPlayer.ActorNumber == action.playerActorNumber;
			switch (action.type)
			{
			case BuilderActionType.AttachToPlayer:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				RigContainer rigContainer;
				if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerActorNumber), out rigContainer))
				{
					string.Format("Execute Builder Action {0} {1} {2} {3} {4}", new object[]
					{
						action.localCommandId,
						action.type,
						action.pieceId,
						action.playerActorNumber,
						action.isLeftHand
					});
					return;
				}
				BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
				Transform parentHeld = action.isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				piece.SetParentHeld(parentHeld, playerActorNumber, action.isLeftHand);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				BuilderPiece.State newState = flag ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed;
				piece.SetState(newState, false);
				if (!flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				if (flag)
				{
					BuilderPieceInteractor.instance.AddPieceToHeld(piece, action.isLeftHand, action.localPosition, action.localRotation);
					return;
				}
				break;
			}
			case BuilderActionType.DetachFromPlayer:
				if (flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				return;
			case BuilderActionType.AttachToPiece:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				Quaternion identity = Quaternion.identity;
				Vector3 zero = Vector3.zero;
				Vector3 position = piece.transform.position;
				Quaternion rotation = piece.transform.rotation;
				if (piece2 != null)
				{
					piece.BumpTwistToPositionRotation(action.twist, action.bumpOffsetx, action.bumpOffsetz, action.attachIndex, piece2.gridPlanes[action.parentAttachIndex], out zero, out identity, out position, out rotation);
				}
				piece.transform.SetPositionAndRotation(position, rotation);
				BuilderPiece.State stateWhenPlaced;
				if (piece2 == null)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedToArm;
				}
				else if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.state == BuilderPiece.State.Grabbed)
				{
					stateWhenPlaced = BuilderPiece.State.Grabbed;
				}
				else if (piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					stateWhenPlaced = BuilderPiece.State.GrabbedLocal;
				}
				else
				{
					stateWhenPlaced = BuilderPiece.State.AttachedToDropped;
				}
				BuilderPiece rootPiece = piece2.GetRootPiece();
				this.gridPlaneData.Clear();
				this.checkGridPlaneData.Clear();
				this.allPotentialPlacements.Clear();
				BuilderTable.tempPieceSet.Clear();
				QueryParameters queryParameters = default(QueryParameters);
				queryParameters.layerMask = this.allPiecesMask;
				QueryParameters queryParameters2 = queryParameters;
				OverlapSphereCommand overlapSphereCommand;
				overlapSphereCommand..ctor(position, 1f, queryParameters2);
				this.nearbyPiecesCommands[0] = overlapSphereCommand;
				OverlapSphereCommand.ScheduleBatch(this.nearbyPiecesCommands, this.nearbyPiecesResults, 1, 1024, default(JobHandle)).Complete();
				int num = 0;
				while (num < 1024 && this.nearbyPiecesResults[num].instanceID != 0)
				{
					BuilderPiece pieceInHand = piece;
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.nearbyPiecesResults[num].collider);
					if (builderPieceFromCollider != null && !BuilderTable.tempPieceSet.Contains(builderPieceFromCollider))
					{
						BuilderTable.tempPieceSet.Add(builderPieceFromCollider);
						if (this.CanPiecesPotentiallyOverlap(pieceInHand, rootPiece, stateWhenPlaced, builderPieceFromCollider))
						{
							for (int i = 0; i < builderPieceFromCollider.gridPlanes.Count; i++)
							{
								BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[i], -1);
								this.checkGridPlaneData.Add(ref builderGridPlaneData);
							}
						}
					}
					num++;
				}
				BuilderTableJobs.BuildTestPieceListForJob(piece, this.gridPlaneData);
				BuilderPotentialPlacement potentialPlacement = new BuilderPotentialPlacement
				{
					localPosition = zero,
					localRotation = identity,
					attachIndex = action.attachIndex,
					parentAttachIndex = action.parentAttachIndex,
					attachPiece = piece,
					parentPiece = piece2
				};
				this.CalcAllPotentialPlacements(this.gridPlaneData, this.checkGridPlaneData, potentialPlacement, this.allPotentialPlacements);
				piece.SetParentPiece(action.attachIndex, piece2, action.parentAttachIndex);
				for (int j = 0; j < this.allPotentialPlacements.Count; j++)
				{
					BuilderPotentialPlacement builderPotentialPlacement = this.allPotentialPlacements[j];
					BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex];
					BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
					BuilderAttachGridPlane movingParentGrid = builderAttachGridPlane.GetMovingParentGrid();
					bool flag2 = movingParentGrid != null;
					BuilderAttachGridPlane movingParentGrid2 = builderAttachGridPlane2.GetMovingParentGrid();
					bool flag3 = movingParentGrid2 != null;
					if (flag2 == flag3 && (!flag2 || !(movingParentGrid != movingParentGrid2)))
					{
						SnapOverlap newOverlap = this.builderPool.CreateSnapOverlap(builderAttachGridPlane2, builderPotentialPlacement.attachBounds);
						builderAttachGridPlane.AddSnapOverlap(newOverlap);
						SnapOverlap newOverlap2 = this.builderPool.CreateSnapOverlap(builderAttachGridPlane, builderPotentialPlacement.parentAttachBounds);
						builderAttachGridPlane2.AddSnapOverlap(newOverlap2);
					}
				}
				piece.transform.SetLocalPositionAndRotation(zero, identity);
				if (piece2 != null && piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					BuilderPiece rootPiece2 = piece2.GetRootPiece();
					BuilderPieceInteractor.instance.OnCountChangedForRoot(rootPiece2);
				}
				if (piece2 == null)
				{
					piece.SetActivateTimeStamp(action.timeStamp);
					piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
					this.SetIsDirty(true);
					if (flag)
					{
						BuilderPieceInteractor.instance.DisableCollisionsWithHands();
						return;
					}
				}
				else
				{
					if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
					{
						piece.SetState(BuilderPiece.State.AttachedToArm, false);
						return;
					}
					if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
					{
						piece.SetActivateTimeStamp(action.timeStamp);
						piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
						if (piece2 != null)
						{
							BuilderPiece attachedBuiltInPiece = piece2.GetAttachedBuiltInPiece();
							BuilderPiecePrivatePlot builderPiecePrivatePlot;
							if (attachedBuiltInPiece != null && attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
							{
								builderPiecePrivatePlot.OnPieceAttachedToPlot(piece);
							}
						}
						this.SetIsDirty(true);
						if (flag)
						{
							BuilderPieceInteractor.instance.DisableCollisionsWithHands();
							return;
						}
					}
					else
					{
						if (piece2.state == BuilderPiece.State.Grabbed)
						{
							piece.SetState(BuilderPiece.State.Grabbed, false);
							return;
						}
						if (piece2.state == BuilderPiece.State.GrabbedLocal)
						{
							piece.SetState(BuilderPiece.State.GrabbedLocal, false);
							return;
						}
						piece.SetState(BuilderPiece.State.AttachedToDropped, false);
						return;
					}
				}
				break;
			}
			case BuilderActionType.DetachFromPiece:
			{
				BuilderPiece piece3 = piece;
				bool flag4 = piece.state == BuilderPiece.State.GrabbedLocal;
				if (flag4)
				{
					piece3 = piece.GetRootPiece();
				}
				if (piece.state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.SetIsDirty(true);
					BuilderPiece attachedBuiltInPiece2 = piece.GetAttachedBuiltInPiece();
					BuilderPiecePrivatePlot builderPiecePrivatePlot2;
					if (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.TryGetPlotComponent(out builderPiecePrivatePlot2))
					{
						builderPiecePrivatePlot2.OnPieceDetachedFromPlot(piece);
					}
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				if (flag4)
				{
					BuilderPieceInteractor.instance.OnCountChangedForRoot(piece3);
					return;
				}
				break;
			}
			case BuilderActionType.MakePieceRoot:
				BuilderPiece.MakePieceRoot(piece);
				return;
			case BuilderActionType.DropPiece:
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				piece.SetState(BuilderPiece.State.Dropped, false);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				if (piece.rigidBody != null)
				{
					piece.rigidBody.position = action.localPosition;
					piece.rigidBody.rotation = action.localRotation;
					piece.rigidBody.linearVelocity = action.velocity;
					piece.rigidBody.angularVelocity = action.angVelocity;
					return;
				}
				break;
			case BuilderActionType.AttachToShelf:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				int attachIndex = action.attachIndex;
				bool isLeftHand = action.isLeftHand;
				int parentAttachIndex = action.parentAttachIndex;
				float x = action.velocity.x;
				piece.transform.localScale = Vector3.one;
				piece.SetState(isLeftHand ? BuilderPiece.State.OnConveyor : BuilderPiece.State.OnShelf, false);
				if (isLeftHand)
				{
					if (attachIndex >= 0 && attachIndex < this.conveyors.Count)
					{
						BuilderConveyor builderConveyor = this.conveyors[attachIndex];
						float num2 = x / builderConveyor.GetFrameMovement();
						if (PhotonNetwork.ServerTimestamp >= parentAttachIndex)
						{
							uint num3 = (uint)(PhotonNetwork.ServerTimestamp - parentAttachIndex);
							num2 += num3 / 1000f;
						}
						piece.shelfOwner = attachIndex;
						builderConveyor.OnShelfPieceCreated(piece, num2);
						return;
					}
				}
				else
				{
					if (attachIndex >= 0 && attachIndex < this.dispenserShelves.Count)
					{
						BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[attachIndex];
						piece.shelfOwner = attachIndex;
						builderDispenserShelf.OnShelfPieceCreated(piece, false);
						return;
					}
					piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06005693 RID: 22163 RVA: 0x001B4598 File Offset: 0x001B2798
		public static bool AreStatesCompatibleForOverlap(BuilderPiece.State stateA, BuilderPiece.State stateB, BuilderPiece rootA, BuilderPiece rootB)
		{
			switch (stateA)
			{
			case BuilderPiece.State.None:
				return false;
			case BuilderPiece.State.AttachedAndPlaced:
				return stateB == BuilderPiece.State.AttachedAndPlaced;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Dropped:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.OnConveyor:
				return (stateB == BuilderPiece.State.AttachedToDropped || stateB == BuilderPiece.State.Dropped || stateB == BuilderPiece.State.OnShelf || stateB == BuilderPiece.State.OnConveyor) && rootA.Equals(rootB);
			case BuilderPiece.State.Grabbed:
				return stateB == BuilderPiece.State.Grabbed && rootA.Equals(rootB);
			case BuilderPiece.State.Displayed:
				return false;
			case BuilderPiece.State.GrabbedLocal:
				return stateB == BuilderPiece.State.GrabbedLocal && rootA.heldInLeftHand == rootB.heldInLeftHand;
			case BuilderPiece.State.AttachedToArm:
			{
				if (stateB != BuilderPiece.State.AttachedToArm)
				{
					return false;
				}
				object obj = (rootA.parentPiece != null) ? rootA.parentPiece : rootA;
				BuilderPiece builderPiece = (rootB.parentPiece != null) ? rootB.parentPiece : rootB;
				return obj.Equals(builderPiece);
			}
			default:
				return false;
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06005694 RID: 22164 RVA: 0x001B465D File Offset: 0x001B285D
		// (set) Token: 0x06005695 RID: 22165 RVA: 0x001B4665 File Offset: 0x001B2865
		public int CurrentSaveSlot
		{
			get
			{
				return this.currentSaveSlot;
			}
			set
			{
				if (this.saveInProgress)
				{
					return;
				}
				if (!BuilderScanKiosk.IsSaveSlotValid(value))
				{
					this.currentSaveSlot = -1;
				}
				if (this.currentSaveSlot != value)
				{
					this.SetIsDirty(true);
				}
				this.currentSaveSlot = value;
			}
		}

		// Token: 0x06005696 RID: 22166 RVA: 0x001B4698 File Offset: 0x001B2898
		private void Awake()
		{
			if (BuilderTable.zoneToInstance == null)
			{
				BuilderTable.zoneToInstance = new Dictionary<GTZone, BuilderTable>(2);
			}
			if (!BuilderTable.zoneToInstance.TryAdd(this.tableZone, this))
			{
				Object.Destroy(this);
			}
			this.acceptableSqrDistFromCenter = Mathf.Pow(217f * this.pieceScale, 2f);
			if (this.buttonSnapRotation != null)
			{
				this.buttonSnapRotation.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreeRotation));
				this.buttonSnapRotation.SetPressed(this.useSnapRotation);
			}
			if (this.buttonSnapPosition != null)
			{
				this.buttonSnapPosition.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreePosition));
				this.buttonSnapPosition.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
			}
			if (this.buttonSaveLayout != null)
			{
				this.buttonSaveLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonSaveLayout));
			}
			if (this.buttonClearLayout != null)
			{
				this.buttonClearLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonClearLayout));
			}
			this.isSetup = false;
			this.nextPieceId = 10000;
			BuilderTable.placedLayer = LayerMask.NameToLayer("Gorilla Object");
			BuilderTable.heldLayerLocal = LayerMask.NameToLayer("Prop");
			BuilderTable.heldLayer = LayerMask.NameToLayer("BuilderProp");
			BuilderTable.droppedLayer = LayerMask.NameToLayer("BuilderProp");
			this.currSnapParams = this.pushAndEaseParams;
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
			this.inRoom = false;
			this.inBuilderZone = false;
			this.builderNetworking.SetTable(this);
			this.plotOwners = new Dictionary<int, int>(10);
			this.doesLocalPlayerOwnPlot = false;
			this.queuedBuildCommands = new List<BuilderTable.BuilderCommand>(1028);
			if (this.isTableMutable)
			{
				this.playerToArmShelfLeft = new Dictionary<int, int>(10);
				this.playerToArmShelfRight = new Dictionary<int, int>(10);
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.rollBackActions = new List<BuilderAction>(1028);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.droppedPieces = new List<BuilderPiece>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.droppedPieceData = new List<BuilderTable.DroppedPieceData>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.SetupMonkeBlocksRoom();
				this.gridPlaneData = new NativeList<BuilderGridPlaneData>(1024, 4);
				this.checkGridPlaneData = new NativeList<BuilderGridPlaneData>(1024, 4);
				this.nearbyPiecesResults = new NativeArray<ColliderHit>(1024, 4, 1);
				this.nearbyPiecesCommands = new NativeArray<OverlapSphereCommand>(1, 4, 1);
				this.allPotentialPlacements = new List<BuilderPotentialPlacement>(1024);
			}
			else
			{
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(128);
				this.rollBackActions = new List<BuilderAction>(128);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(128);
			}
			this.SetupResources();
			if (!this.isTableMutable && this.linkedTerminal != null)
			{
				this.linkedTerminal.Init(this);
			}
		}

		// Token: 0x06005697 RID: 22167 RVA: 0x0001877F File Offset: 0x0001697F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005698 RID: 22168 RVA: 0x00018787 File Offset: 0x00016987
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06005699 RID: 22169 RVA: 0x001B497B File Offset: 0x001B2B7B
		public static bool TryGetBuilderTableForZone(GTZone zone, out BuilderTable table)
		{
			if (BuilderTable.zoneToInstance == null)
			{
				table = null;
				return false;
			}
			return BuilderTable.zoneToInstance.TryGetValue(zone, ref table);
		}

		// Token: 0x0600569A RID: 22170 RVA: 0x001B4998 File Offset: 0x001B2B98
		private void SetupMonkeBlocksRoom()
		{
			if (this.shelves == null)
			{
				this.shelves = new List<BuilderShelf>(64);
			}
			if (this.shelvesRoot != null)
			{
				this.shelvesRoot.GetComponentsInChildren<BuilderShelf>(this.shelves);
			}
			this.conveyors = new List<BuilderConveyor>(32);
			this.dispenserShelves = new List<BuilderDispenserShelf>(32);
			if (this.allShelvesRoot != null)
			{
				for (int i = 0; i < this.allShelvesRoot.Count; i++)
				{
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderConveyor>(BuilderTable.tempConveyors);
					this.conveyors.AddRange(BuilderTable.tempConveyors);
					BuilderTable.tempConveyors.Clear();
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderDispenserShelf>(BuilderTable.tempDispensers);
					this.dispenserShelves.AddRange(BuilderTable.tempDispensers);
					BuilderTable.tempDispensers.Clear();
				}
			}
			this.recyclers = new List<BuilderRecycler>(5);
			if (this.recyclerRoot != null)
			{
				for (int j = 0; j < this.recyclerRoot.Count; j++)
				{
					this.recyclerRoot[j].GetComponentsInChildren<BuilderRecycler>(BuilderTable.tempRecyclers);
					this.recyclers.AddRange(BuilderTable.tempRecyclers);
					BuilderTable.tempRecyclers.Clear();
				}
			}
			for (int k = 0; k < this.recyclers.Count; k++)
			{
				this.recyclers[k].recyclerID = k;
				this.recyclers[k].table = this;
			}
			this.dropZones = new List<BuilderDropZone>(6);
			this.dropZoneRoot.GetComponentsInChildren<BuilderDropZone>(this.dropZones);
			for (int l = 0; l < this.dropZones.Count; l++)
			{
				this.dropZones[l].dropZoneID = l;
				this.dropZones[l].table = this;
			}
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this;
			}
		}

		// Token: 0x0600569B RID: 22171 RVA: 0x001B4BA4 File Offset: 0x001B2DA4
		private void SetupResources()
		{
			this.maxResources = new int[3];
			if (this.totalResources != null && this.totalResources.quantities != null)
			{
				for (int i = 0; i < this.totalResources.quantities.Count; i++)
				{
					if (this.totalResources.quantities[i].type >= BuilderResourceType.Basic && this.totalResources.quantities[i].type < BuilderResourceType.Count)
					{
						this.maxResources[(int)this.totalResources.quantities[i].type] += this.totalResources.quantities[i].count;
					}
				}
			}
			this.usedResources = new int[3];
			this.reservedResources = new int[3];
			if (this.totalReservedResources != null && this.totalReservedResources.quantities != null)
			{
				for (int j = 0; j < this.totalReservedResources.quantities.Count; j++)
				{
					if (this.totalReservedResources.quantities[j].type >= BuilderResourceType.Basic && this.totalReservedResources.quantities[j].type < BuilderResourceType.Count)
					{
						this.reservedResources[(int)this.totalReservedResources.quantities[j].type] += this.totalReservedResources.quantities[j].count;
					}
				}
			}
			this.plotMaxResources = new int[3];
			if (this.resourcesPerPrivatePlot != null && this.resourcesPerPrivatePlot.quantities != null)
			{
				for (int k = 0; k < this.resourcesPerPrivatePlot.quantities.Count; k++)
				{
					if (this.resourcesPerPrivatePlot.quantities[k].type >= BuilderResourceType.Basic && this.resourcesPerPrivatePlot.quantities[k].type < BuilderResourceType.Count)
					{
						this.plotMaxResources[(int)this.resourcesPerPrivatePlot.quantities[k].type] += this.resourcesPerPrivatePlot.quantities[k].count;
					}
				}
			}
			this.OnAvailableResourcesChange();
		}

		// Token: 0x0600569C RID: 22172 RVA: 0x001B4DEC File Offset: 0x001B2FEC
		private void Start()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom != this.inRoom)
			{
				this.SetInRoom(NetworkSystem.Instance.InRoom);
			}
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			this.HandleOnZoneChanged();
			this.RequestTableConfiguration();
			this.FetchSharedBlocksStartingMapConfig();
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdate));
		}

		// Token: 0x0600569D RID: 22173 RVA: 0x001B4E7B File Offset: 0x001B307B
		private void OnApplicationQuit()
		{
			this.ClearTable();
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x0600569E RID: 22174 RVA: 0x001B4E8C File Offset: 0x001B308C
		private void OnDestroy()
		{
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdate));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (this.isTableMutable)
			{
				if (this.gridPlaneData.IsCreated)
				{
					this.gridPlaneData.Dispose();
				}
				if (this.checkGridPlaneData.IsCreated)
				{
					this.checkGridPlaneData.Dispose();
				}
				if (this.nearbyPiecesResults.IsCreated)
				{
					this.nearbyPiecesResults.Dispose();
				}
				if (this.nearbyPiecesCommands.IsCreated)
				{
					this.nearbyPiecesCommands.Dispose();
				}
			}
			this.DestroyData();
		}

		// Token: 0x0600569F RID: 22175 RVA: 0x001B4F48 File Offset: 0x001B3148
		private void HandleOnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.tableZone);
			this.SetInBuilderZone(flag);
		}

		// Token: 0x060056A0 RID: 22176 RVA: 0x001B4F70 File Offset: 0x001B3170
		public void InitIfNeeded()
		{
			if (!this.isSetup)
			{
				if (BuilderSetManager.instance == null)
				{
					return;
				}
				BuilderSetManager.instance.InitPieceDictionary();
				this.builderRenderer.BuildRenderer(BuilderSetManager.pieceList);
				this.baseGridPlanes.Clear();
				this.basePieces = new List<BuilderPiece>(1024);
				for (int i = 0; i < this.builtInPieceRoots.Count; i++)
				{
					this.builtInPieceRoots[i].SetActive(true);
					this.builtInPieceRoots[i].GetComponentsInChildren<BuilderPiece>(false, BuilderTable.tempPieces);
					this.basePieces.AddRange(BuilderTable.tempPieces);
				}
				this.allPrivatePlots = new List<BuilderPiecePrivatePlot>(20);
				this.CreateData();
				for (int j = 0; j < this.basePieces.Count; j++)
				{
					BuilderPiece builderPiece = this.basePieces[j];
					builderPiece.SetTable(this);
					builderPiece.pieceId = 5 + j;
					builderPiece.SetScale(this.pieceScale);
					builderPiece.SetupPiece(this.gridSize);
					builderPiece.OnCreate();
					builderPiece.SetState(BuilderPiece.State.OnShelf, true);
					this.baseGridPlanes.AddRange(builderPiece.gridPlanes);
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (builderPiece.IsPrivatePlot() && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						this.allPrivatePlots.Add(builderPiecePrivatePlot);
					}
					this.AddPieceData(builderPiece);
				}
				this.builderPool = BuilderPool.instance;
				this.builderPool.Setup();
				base.StartCoroutine(this.builderPool.BuildFromPieceSets());
				if (this.isTableMutable)
				{
					for (int k = 0; k < this.conveyors.Count; k++)
					{
						this.conveyors[k].table = this;
						this.conveyors[k].shelfID = k;
						this.conveyors[k].Setup();
					}
					for (int l = 0; l < this.dispenserShelves.Count; l++)
					{
						this.dispenserShelves[l].table = this;
						this.dispenserShelves[l].shelfID = l;
						this.dispenserShelves[l].Setup();
					}
					this.conveyorManager.Setup(this);
					this.repelledPieceRoots = new HashSet<int>[this.repelHistoryLength];
					for (int m = 0; m < this.repelHistoryLength; m++)
					{
						this.repelledPieceRoots[m] = new HashSet<int>(10);
					}
					this.sharedBuildAreas = this.sharedBuildArea.GetComponents<BoxCollider>();
					BoxCollider[] array = this.sharedBuildAreas;
					for (int n = 0; n < array.Length; n++)
					{
						array[n].enabled = false;
					}
					this.sharedBuildArea.SetActive(false);
				}
				BoxCollider[] components = this.noBlocksArea.GetComponents<BoxCollider>();
				this.noBlocksAreas = new List<BuilderTable.BoxCheckParams>(components.Length);
				foreach (BoxCollider boxCollider in components)
				{
					boxCollider.enabled = true;
					BuilderTable.BoxCheckParams boxCheckParams = new BuilderTable.BoxCheckParams
					{
						center = boxCollider.transform.TransformPoint(boxCollider.center),
						halfExtents = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size) / 2f,
						rotation = boxCollider.transform.rotation
					};
					this.noBlocksAreas.Add(boxCheckParams);
					boxCollider.enabled = false;
				}
				this.noBlocksArea.SetActive(false);
				this.isSetup = true;
			}
		}

		// Token: 0x060056A1 RID: 22177 RVA: 0x001B52F2 File Offset: 0x001B34F2
		private void SetIsDirty(bool dirty)
		{
			if (this.isDirty != dirty)
			{
				UnityEvent<bool> onSaveDirtyChanged = this.OnSaveDirtyChanged;
				if (onSaveDirtyChanged != null)
				{
					onSaveDirtyChanged.Invoke(dirty);
				}
			}
			this.isDirty = dirty;
		}

		// Token: 0x060056A2 RID: 22178 RVA: 0x001B5318 File Offset: 0x001B3518
		private void FixedUpdate()
		{
			if (this.tableState != BuilderTable.TableState.Ready && this.tableState != BuilderTable.TableState.WaitForMasterResync)
			{
				return;
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegisterFixed)
			{
				if (builderPieceFunctional != null)
				{
					this.fixedUpdateFunctionalComponents.Add(builderPieceFunctional);
				}
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.funcComponentsToUnregisterFixed)
			{
				this.fixedUpdateFunctionalComponents.Remove(builderPieceFunctional2);
			}
			this.funcComponentsToRegisterFixed.Clear();
			this.funcComponentsToUnregisterFixed.Clear();
			foreach (IBuilderPieceFunctional builderPieceFunctional3 in this.fixedUpdateFunctionalComponents)
			{
				builderPieceFunctional3.FunctionalPieceFixedUpdate();
			}
		}

		// Token: 0x060056A3 RID: 22179 RVA: 0x001B5424 File Offset: 0x001B3624
		public void Tick()
		{
			this.RunUpdate();
		}

		// Token: 0x060056A4 RID: 22180 RVA: 0x001B542C File Offset: 0x001B362C
		private void RunUpdate()
		{
			this.InitIfNeeded();
			this.UpdateTableState();
			if (this.isTableMutable)
			{
				this.UpdateDroppedPieces(Time.deltaTime);
				this.repelHistoryIndex = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				int num = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				this.repelledPieceRoots[num].Clear();
			}
		}

		// Token: 0x060056A5 RID: 22181 RVA: 0x001B548A File Offset: 0x001B368A
		public void AddQueuedCommand(BuilderTable.BuilderCommand cmd)
		{
			this.queuedBuildCommands.Add(cmd);
		}

		// Token: 0x060056A6 RID: 22182 RVA: 0x001B5498 File Offset: 0x001B3698
		public void ClearQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				this.queuedBuildCommands.Clear();
			}
			this.RemoveRollBackActions();
			if (this.rollBackBufferedCommands != null)
			{
				this.rollBackBufferedCommands.Clear();
			}
			this.RemoveRollForwardCommands();
		}

		// Token: 0x060056A7 RID: 22183 RVA: 0x001B54CC File Offset: 0x001B36CC
		public int GetNumQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				return this.queuedBuildCommands.Count;
			}
			return 0;
		}

		// Token: 0x060056A8 RID: 22184 RVA: 0x001B54E3 File Offset: 0x001B36E3
		public void AddRollbackAction(BuilderAction action)
		{
			this.rollBackActions.Add(action);
		}

		// Token: 0x060056A9 RID: 22185 RVA: 0x001B54F1 File Offset: 0x001B36F1
		public void RemoveRollBackActions()
		{
			this.rollBackActions.Clear();
		}

		// Token: 0x060056AA RID: 22186 RVA: 0x001B5500 File Offset: 0x001B3700
		public void RemoveRollBackActions(int localCommandId)
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollBackActions[i].localCommandId == localCommandId)
				{
					this.rollBackActions.RemoveAt(i);
				}
			}
		}

		// Token: 0x060056AB RID: 22187 RVA: 0x001B554C File Offset: 0x001B374C
		public bool HasRollBackActionsForCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollBackActions.Count; i++)
			{
				if (this.rollBackActions[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060056AC RID: 22188 RVA: 0x001B5586 File Offset: 0x001B3786
		public void AddRollForwardCommand(BuilderTable.BuilderCommand command)
		{
			this.rollForwardCommands.Add(command);
		}

		// Token: 0x060056AD RID: 22189 RVA: 0x001B5594 File Offset: 0x001B3794
		public void RemoveRollForwardCommands()
		{
			this.rollForwardCommands.Clear();
		}

		// Token: 0x060056AE RID: 22190 RVA: 0x001B55A4 File Offset: 0x001B37A4
		public void RemoveRollForwardCommands(int localCommandId)
		{
			for (int i = this.rollForwardCommands.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					this.rollForwardCommands.RemoveAt(i);
				}
			}
		}

		// Token: 0x060056AF RID: 22191 RVA: 0x001B55F0 File Offset: 0x001B37F0
		public bool HasRollForwardCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				if (this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x001B562C File Offset: 0x001B382C
		public bool ShouldRollbackBufferCommand(BuilderTable.BuilderCommand cmd)
		{
			return cmd.type != BuilderTable.BuilderCommandType.Create && cmd.type != BuilderTable.BuilderCommandType.CreateArmShelf && this.rollBackActions.Count > 0 && (cmd.player == null || !cmd.player.IsLocal || !this.HasRollForwardCommand(cmd.localCommandId));
		}

		// Token: 0x060056B1 RID: 22193 RVA: 0x001B5683 File Offset: 0x001B3883
		public void AddRollbackBufferedCommand(BuilderTable.BuilderCommand bufferedCmd)
		{
			this.rollBackBufferedCommands.Add(bufferedCmd);
		}

		// Token: 0x060056B2 RID: 22194 RVA: 0x001B5694 File Offset: 0x001B3894
		private void ExecuteRollBackActions()
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				this.ExecuteAction(this.rollBackActions[i]);
			}
			this.rollBackActions.Clear();
		}

		// Token: 0x060056B3 RID: 22195 RVA: 0x001B56D8 File Offset: 0x001B38D8
		private void ExecuteRollbackBufferedCommands()
		{
			for (int i = 0; i < this.rollBackBufferedCommands.Count; i++)
			{
				BuilderTable.BuilderCommand cmd = this.rollBackBufferedCommands[i];
				cmd.isQueued = false;
				cmd.canRollback = false;
				this.ExecuteBuildCommand(cmd);
			}
			this.rollBackBufferedCommands.Clear();
		}

		// Token: 0x060056B4 RID: 22196 RVA: 0x001B572C File Offset: 0x001B392C
		private void ExecuteRollForwardCommands()
		{
			BuilderTable.tempRollForwardCommands.Clear();
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.tempRollForwardCommands.Add(this.rollForwardCommands[i]);
			}
			this.rollForwardCommands.Clear();
			for (int j = 0; j < BuilderTable.tempRollForwardCommands.Count; j++)
			{
				BuilderTable.BuilderCommand cmd = BuilderTable.tempRollForwardCommands[j];
				cmd.isQueued = true;
				cmd.canRollback = true;
				this.ExecuteBuildCommand(cmd);
			}
			BuilderTable.tempRollForwardCommands.Clear();
		}

		// Token: 0x060056B5 RID: 22197 RVA: 0x001B57BC File Offset: 0x001B39BC
		private void UpdateRollForwardCommandData()
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollForwardCommands[i];
				if (builderCommand.type == BuilderTable.BuilderCommandType.Drop)
				{
					BuilderPiece piece = this.GetPiece(builderCommand.pieceId);
					if (piece != null && piece.rigidBody != null)
					{
						builderCommand.localPosition = piece.rigidBody.position;
						builderCommand.localRotation = piece.rigidBody.rotation;
						builderCommand.velocity = piece.rigidBody.linearVelocity;
						builderCommand.angVelocity = piece.rigidBody.angularVelocity;
						this.rollForwardCommands[i] = builderCommand;
					}
				}
			}
		}

		// Token: 0x060056B6 RID: 22198 RVA: 0x001B5874 File Offset: 0x001B3A74
		public bool TryRollbackAndReExecute(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				if (this.rollBackBufferedCommands.Count > 0)
				{
					this.UpdateRollForwardCommandData();
					this.ExecuteRollBackActions();
					this.ExecuteRollbackBufferedCommands();
					this.ExecuteRollForwardCommands();
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				else
				{
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060056B7 RID: 22199 RVA: 0x001B58D3 File Offset: 0x001B3AD3
		public void RollbackFailedCommand(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				this.UpdateRollForwardCommandData();
				this.ExecuteRollBackActions();
				this.ExecuteRollbackBufferedCommands();
				this.RemoveRollForwardCommands(-1);
				this.ExecuteRollForwardCommands();
			}
		}

		// Token: 0x060056B8 RID: 22200 RVA: 0x001B58FD File Offset: 0x001B3AFD
		public BuilderTable.TableState GetTableState()
		{
			return this.tableState;
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x001B5908 File Offset: 0x001B3B08
		public void SetTableState(BuilderTable.TableState newState)
		{
			this.InitIfNeeded();
			if (newState == this.tableState)
			{
				return;
			}
			BuilderTable.TableState tableState = this.tableState;
			this.tableState = newState;
			switch (this.tableState)
			{
			case BuilderTable.TableState.WaitingForInitalBuild:
				if (!this.isTableMutable && !NetworkSystem.Instance.IsMasterClient)
				{
					this.sharedBlocksMap = null;
					UnityEvent onMapCleared = this.OnMapCleared;
					if (onMapCleared == null)
					{
						return;
					}
					onMapCleared.Invoke();
					return;
				}
				break;
			case BuilderTable.TableState.ReceivingInitialBuild:
			case BuilderTable.TableState.ReceivingMasterResync:
			case BuilderTable.TableState.InitialBuild:
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.WaitForInitialBuildMaster:
				this.nextPieceId = 10000;
				if (this.isTableMutable)
				{
					this.BuildInitialTableForPlayer();
					return;
				}
				this.BuildSelectedSharedMap();
				return;
			case BuilderTable.TableState.WaitForMasterResync:
				this.ClearQueuedCommands();
				this.ResetConveyors();
				return;
			case BuilderTable.TableState.Ready:
				this.OnAvailableResourcesChange();
				if (!this.isTableMutable)
				{
					string text = (this.sharedBlocksMap == null) ? "" : this.sharedBlocksMap.MapID;
					UnityEvent<string> onMapLoaded = this.OnMapLoaded;
					if (onMapLoaded != null)
					{
						onMapLoaded.Invoke(text);
					}
					this.SetPendingMap(null);
					return;
				}
				break;
			case BuilderTable.TableState.BadData:
				this.ClearTable();
				this.ClearQueuedCommands();
				break;
			case BuilderTable.TableState.WaitingForSharedMapLoad:
				this.ClearTable();
				this.ClearQueuedCommands();
				this.builderNetworking.ResetSerializedTableForAllPlayers();
				return;
			default:
				return;
			}
		}

		// Token: 0x060056BA RID: 22202 RVA: 0x001B5A34 File Offset: 0x001B3C34
		public void SetPendingMap(string mapID)
		{
			this.pendingMapID = mapID;
		}

		// Token: 0x060056BB RID: 22203 RVA: 0x001B5A3D File Offset: 0x001B3C3D
		public string GetPendingMap()
		{
			return this.pendingMapID;
		}

		// Token: 0x060056BC RID: 22204 RVA: 0x001B5A45 File Offset: 0x001B3C45
		public string GetCurrentMapID()
		{
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap = this.sharedBlocksMap;
			if (sharedBlocksMap == null)
			{
				return null;
			}
			return sharedBlocksMap.MapID;
		}

		// Token: 0x060056BD RID: 22205 RVA: 0x001B5A58 File Offset: 0x001B3C58
		public void LoadSharedMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (map.MapID.IsNullOrEmpty())
				{
					GTDev.LogWarning<string>("Invalid map to load", null);
					UnityEvent<string> onMapLoadFailed = this.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("Invalid Map ID");
					return;
				}
				else
				{
					if (this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.BadData)
					{
						this.builderNetworking.RequestLoadSharedBlocksMap(map.MapID);
						return;
					}
					UnityEvent<string> onMapLoadFailed2 = this.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("WAIT FOR LOAD IN PROGRESS");
					return;
				}
			}
			else
			{
				UnityEvent<string> onMapLoadFailed3 = this.OnMapLoadFailed;
				if (onMapLoadFailed3 == null)
				{
					return;
				}
				onMapLoadFailed3.Invoke("Not In Room");
				return;
			}
		}

		// Token: 0x060056BE RID: 22206 RVA: 0x001B5AF0 File Offset: 0x001B3CF0
		public void SetInRoom(bool inRoom)
		{
			this.inRoom = inRoom;
			bool flag = inRoom && this.inBuilderZone;
			if (!inRoom)
			{
				this.pendingMapID = null;
				this.sharedBlocksMap = null;
				UnityEvent onMapCleared = this.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
			}
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient && this.isTableMutable)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient() && this.isTableMutable)
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x060056BF RID: 22207 RVA: 0x001B5BC4 File Offset: 0x001B3DC4
		public static bool IsLocalPlayerInBuilderZone()
		{
			GorillaTagger instance = GorillaTagger.Instance;
			ZoneEntityBSP zoneEntityBSP;
			if (instance == null)
			{
				zoneEntityBSP = null;
			}
			else
			{
				VRRig offlineVRRig = instance.offlineVRRig;
				zoneEntityBSP = ((offlineVRRig != null) ? offlineVRRig.zoneEntity : null);
			}
			ZoneEntityBSP zoneEntityBSP2 = zoneEntityBSP;
			BuilderTable builderTable;
			return !(zoneEntityBSP2 == null) && BuilderTable.TryGetBuilderTableForZone(zoneEntityBSP2.currentZone, out builderTable) && builderTable.IsInBuilderZone();
		}

		// Token: 0x060056C0 RID: 22208 RVA: 0x001B5C11 File Offset: 0x001B3E11
		public bool IsInBuilderZone()
		{
			return this.inBuilderZone;
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x001B5C1C File Offset: 0x001B3E1C
		public void SetInBuilderZone(bool inBuilderZone)
		{
			this.inBuilderZone = inBuilderZone;
			this.ShowPieces(inBuilderZone);
			bool flag = this.inRoom && inBuilderZone;
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient())
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x060056C2 RID: 22210 RVA: 0x001B5CC0 File Offset: 0x001B3EC0
		private void ShowPieces(bool show)
		{
			if (this.builderRenderer != null)
			{
				this.builderRenderer.Show(show);
			}
			if (this.pieces == null || this.basePieces == null)
			{
				return;
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				this.pieces[i].SetDirectRenderersVisible(show);
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				this.basePieces[j].SetDirectRenderersVisible(show);
			}
		}

		// Token: 0x060056C3 RID: 22211 RVA: 0x001B5D48 File Offset: 0x001B3F48
		private void UpdateTableState()
		{
			switch (this.tableState)
			{
			case BuilderTable.TableState.InitialBuild:
			{
				BuilderTableNetworking.PlayerTableInitState localTableInit = this.builderNetworking.GetLocalTableInit();
				try
				{
					this.ClearTable();
					this.ClearQueuedCommands();
					byte[] array = GZipStream.UncompressBuffer(localTableInit.serializedTableState);
					localTableInit.totalSerializedBytes = array.Length;
					Array.Copy(array, 0, localTableInit.serializedTableState, 0, localTableInit.totalSerializedBytes);
					this.DeserializeTableState(localTableInit.serializedTableState, localTableInit.numSerializedBytes);
					if (this.tableState == BuilderTable.TableState.BadData)
					{
						return;
					}
					this.SetTableState(BuilderTable.TableState.ExecuteQueuedCommands);
					this.SetIsDirty(true);
					return;
				}
				catch (Exception)
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				break;
			}
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.Ready:
			{
				JobHandle jobHandle = default(JobHandle);
				if (this.isTableMutable)
				{
					this.conveyorManager.UpdateManager();
					jobHandle = this.conveyorManager.ConstructJobHandle();
					JobHandle.ScheduleBatchedJobs();
					foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
					{
						builderDispenserShelf.UpdateShelf();
					}
					foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
					{
						builderPiecePrivatePlot.UpdatePlot();
					}
					foreach (BuilderRecycler builderRecycler in this.recyclers)
					{
						builderRecycler.UpdateRecycler();
					}
					for (int i = this.shelfSliceUpdateIndex; i < this.dispenserShelves.Count; i += BuilderTable.SHELF_SLICE_BUCKETS)
					{
						this.dispenserShelves[i].UpdateShelfSliced();
					}
					this.shelfSliceUpdateIndex = (this.shelfSliceUpdateIndex + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegister)
				{
					if (builderPieceFunctional != null)
					{
						this.activeFunctionalComponents.Add(builderPieceFunctional);
					}
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.funcComponentsToUnregister)
				{
					this.activeFunctionalComponents.Remove(builderPieceFunctional2);
				}
				this.funcComponentsToRegister.Clear();
				this.funcComponentsToUnregister.Clear();
				foreach (IBuilderPieceFunctional builderPieceFunctional3 in this.activeFunctionalComponents)
				{
					if (builderPieceFunctional3 != null)
					{
						builderPieceFunctional3.FunctionalPieceUpdate();
					}
				}
				if (this.isTableMutable)
				{
					foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
					{
						builderResourceMeter.UpdateMeterFill();
					}
					this.CleanUpDroppedPiece();
					jobHandle.Complete();
					return;
				}
				return;
			}
			default:
				return;
			}
			for (int j = 0; j < this.queuedBuildCommands.Count; j++)
			{
				BuilderTable.BuilderCommand cmd = this.queuedBuildCommands[j];
				cmd.isQueued = true;
				this.ExecuteBuildCommand(cmd);
			}
			this.queuedBuildCommands.Clear();
			this.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x060056C4 RID: 22212 RVA: 0x001B60D4 File Offset: 0x001B42D4
		private void RouteNewCommand(BuilderTable.BuilderCommand cmd, bool force)
		{
			bool flag = this.ShouldExecuteCommand();
			if (force)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (flag && this.ShouldRollbackBufferCommand(cmd))
			{
				this.AddRollbackBufferedCommand(cmd);
				return;
			}
			if (flag)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (this.ShouldQueueCommand())
			{
				this.AddQueuedCommand(cmd);
				return;
			}
			this.ShouldDiscardCommand();
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x001B612C File Offset: 0x001B432C
		private void ExecuteBuildCommand(BuilderTable.BuilderCommand cmd)
		{
			if (!this.isTableMutable && cmd.type != BuilderTable.BuilderCommandType.FunctionalStateChange)
			{
				return;
			}
			switch (cmd.type)
			{
			case BuilderTable.BuilderCommandType.Create:
				this.ExecutePieceCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.Place:
				this.ExecutePiecePlacedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Grab:
				this.ExecutePieceGrabbedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Drop:
				this.ExecutePieceDroppedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Remove:
				break;
			case BuilderTable.BuilderCommandType.Paint:
				this.ExecutePiecePainted(cmd);
				return;
			case BuilderTable.BuilderCommandType.Recycle:
				this.ExecutePieceRecycled(cmd);
				return;
			case BuilderTable.BuilderCommandType.ClaimPlot:
				this.ExecuteClaimPlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.FreePlot:
				this.ExecuteFreePlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.CreateArmShelf:
				this.ExecuteArmShelfCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.PlayerLeftRoom:
				this.ExecutePlayerLeftRoom(cmd);
				return;
			case BuilderTable.BuilderCommandType.FunctionalStateChange:
				this.ExecuteSetFunctionalPieceState(cmd);
				return;
			case BuilderTable.BuilderCommandType.SetSelection:
				this.ExecuteSetSelection(cmd);
				return;
			case BuilderTable.BuilderCommandType.Repel:
				this.ExecutePieceRepelled(cmd);
				break;
			default:
				return;
			}
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x001B61F9 File Offset: 0x001B43F9
		public void ClearTable()
		{
			this.ClearTableInternal();
		}

		// Token: 0x060056C7 RID: 22215 RVA: 0x001B6204 File Offset: 0x001B4404
		private void ClearTableInternal()
		{
			BuilderTable.tempDeletePieces.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				BuilderTable.tempDeletePieces.Add(this.pieces[i]);
			}
			if (this.isTableMutable)
			{
				this.droppedPieces.Clear();
				this.droppedPieceData.Clear();
			}
			for (int j = 0; j < BuilderTable.tempDeletePieces.Count; j++)
			{
				BuilderTable.tempDeletePieces[j].ClearParentPiece(false);
				BuilderTable.tempDeletePieces[j].ClearParentHeld();
				BuilderTable.tempDeletePieces[j].SetState(BuilderPiece.State.None, false);
				this.RemovePiece(BuilderTable.tempDeletePieces[j]);
			}
			for (int k = 0; k < BuilderTable.tempDeletePieces.Count; k++)
			{
				this.builderPool.DestroyPiece(BuilderTable.tempDeletePieces[k]);
			}
			BuilderTable.tempDeletePieces.Clear();
			this.pieces.Clear();
			this.pieceIDToIndexCache.Clear();
			this.nextPieceId = 10000;
			if (this.isTableMutable)
			{
				this.conveyorManager.OnClearTable();
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					builderDispenserShelf.OnClearTable();
				}
				for (int l = 0; l < this.repelHistoryLength; l++)
				{
					this.repelledPieceRoots[l].Clear();
				}
			}
			this.funcComponentsToRegister.Clear();
			this.funcComponentsToUnregister.Clear();
			this.activeFunctionalComponents.Clear();
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				foreach (BuilderAttachGridPlane builderAttachGridPlane in builderPiece.gridPlanes)
				{
					builderAttachGridPlane.OnReturnToPool(this.builderPool);
				}
			}
			if (this.isTableMutable)
			{
				this.ClearBuiltInPlots();
				this.playerToArmShelfLeft.Clear();
				this.playerToArmShelfRight.Clear();
				if (BuilderPieceInteractor.instance != null)
				{
					BuilderPieceInteractor.instance.RemovePiecesFromHands();
				}
			}
		}

		// Token: 0x060056C8 RID: 22216 RVA: 0x001B6470 File Offset: 0x001B4670
		private void ClearBuiltInPlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.ClearPlot();
			}
			this.plotOwners.Clear();
			this.SetLocalPlayerOwnsPlot(false);
		}

		// Token: 0x060056C9 RID: 22217 RVA: 0x001B64D4 File Offset: 0x001B46D4
		private void OnDeserializeUpdatePlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.RecountPlotCost();
			}
		}

		// Token: 0x060056CA RID: 22218 RVA: 0x001B6524 File Offset: 0x001B4724
		public void BuildPiecesOnShelves()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (this.shelves == null)
			{
				return;
			}
			for (int i = 0; i < this.shelves.Count; i++)
			{
				if (this.shelves[i] != null)
				{
					this.shelves[i].Init();
				}
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int j = 0; j < this.shelves.Count; j++)
				{
					if (this.shelves[j].HasOpenSlot())
					{
						this.shelves[j].BuildNextPiece(this);
						if (this.shelves[j].HasOpenSlot())
						{
							flag = true;
						}
					}
				}
			}
		}

		// Token: 0x060056CB RID: 22219 RVA: 0x001B65D7 File Offset: 0x001B47D7
		private void OnFinishedInitialTableBuild()
		{
			this.BuildPiecesOnShelves();
			this.SetTableState(BuilderTable.TableState.Ready);
			this.CreateArmShelvesForPlayersInBuilder();
		}

		// Token: 0x060056CC RID: 22220 RVA: 0x001B65EC File Offset: 0x001B47EC
		public int CreatePieceId()
		{
			int result = this.nextPieceId;
			if (this.nextPieceId == 2147483647)
			{
				this.nextPieceId = 20000;
			}
			this.nextPieceId++;
			return result;
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x001B661C File Offset: 0x001B481C
		public void ResetConveyors()
		{
			if (this.isTableMutable)
			{
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					builderConveyor.ResetConveyorState();
				}
			}
		}

		// Token: 0x060056CE RID: 22222 RVA: 0x001B6674 File Offset: 0x001B4874
		public void RequestCreateConveyorPiece(int newPieceType, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			BuilderConveyor builderConveyor = this.conveyors[shelfID];
			if (builderConveyor == null)
			{
				return;
			}
			Transform spawnTransform = builderConveyor.GetSpawnTransform();
			this.builderNetworking.CreateShelfPiece(newPieceType, spawnTransform.position, spawnTransform.rotation, materialType, BuilderPiece.State.OnConveyor, shelfID);
		}

		// Token: 0x060056CF RID: 22223 RVA: 0x001B66CD File Offset: 0x001B48CD
		public void RequestCreateDispenserShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			this.builderNetworking.CreateShelfPiece(pieceType, position, rotation, materialType, BuilderPiece.State.OnShelf, shelfID);
		}

		// Token: 0x060056D0 RID: 22224 RVA: 0x001B6710 File Offset: 0x001B4910
		public void CreateConveyorPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID, int sendTimestamp)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			if (this.conveyors[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnConveyor,
				parentPieceId = shelfID,
				parentAttachIndex = sendTimestamp,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x060056D1 RID: 22225 RVA: 0x001B67B8 File Offset: 0x001B49B8
		public void CreateDispenserShelfPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnShelf,
				parentPieceId = shelfID,
				isLeft = true,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x060056D2 RID: 22226 RVA: 0x001B685E File Offset: 0x001B4A5E
		public void RequestShelfSelection(int shelfId, int groupID, bool isConveyor)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestShelfSelection(shelfId, groupID, isConveyor);
		}

		// Token: 0x060056D3 RID: 22227 RVA: 0x001B6878 File Offset: 0x001B4A78
		public void VerifySetSelections()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.conveyors)
			{
				builderConveyor.VerifySetSelection();
			}
			foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
			{
				builderDispenserShelf.VerifySetSelection();
			}
		}

		// Token: 0x060056D4 RID: 22228 RVA: 0x001B6910 File Offset: 0x001B4B10
		public bool ValidateShelfSelectionParams(int shelfId, int displayGroupID, bool isConveyor, Player player)
		{
			bool flag = shelfId >= 0 && ((isConveyor && shelfId < this.conveyors.Count) || (!isConveyor && shelfId < this.dispenserShelves.Count)) && BuilderSetManager.instance.DoesPlayerOwnDisplayGroup(player, displayGroupID);
			if (PhotonNetwork.IsMasterClient)
			{
				if (isConveyor)
				{
					BuilderConveyor builderConveyor = this.conveyors[shelfId];
					bool flag2 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderConveyor.transform.position, false, true, 4f);
					flag = (flag && flag2);
				}
				else
				{
					BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[shelfId];
					bool flag3 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderDispenserShelf.transform.position, false, true, 4f);
					flag = (flag && flag3);
				}
			}
			return flag;
		}

		// Token: 0x060056D5 RID: 22229 RVA: 0x001B69CC File Offset: 0x001B4BCC
		private void SetConveyorSelection(int conveyorId, int setId)
		{
			BuilderConveyor builderConveyor = this.conveyors[conveyorId];
			if (builderConveyor == null)
			{
				return;
			}
			builderConveyor.SetSelection(setId);
		}

		// Token: 0x060056D6 RID: 22230 RVA: 0x001B69F8 File Offset: 0x001B4BF8
		private void SetDispenserSelection(int conveyorId, int setId)
		{
			BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[conveyorId];
			if (builderDispenserShelf == null)
			{
				return;
			}
			builderDispenserShelf.SetSelection(setId);
		}

		// Token: 0x060056D7 RID: 22231 RVA: 0x001B6A24 File Offset: 0x001B4C24
		public void ChangeSetSelection(int shelfID, int setID, bool isConveyor)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.SetSelection,
				parentPieceId = shelfID,
				pieceType = setID,
				isLeft = isConveyor,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x060056D8 RID: 22232 RVA: 0x001B6A78 File Offset: 0x001B4C78
		public void ExecuteSetSelection(BuilderTable.BuilderCommand cmd)
		{
			bool isLeft = cmd.isLeft;
			int parentPieceId = cmd.parentPieceId;
			int pieceType = cmd.pieceType;
			if (isLeft)
			{
				this.SetConveyorSelection(parentPieceId, pieceType);
				return;
			}
			this.SetDispenserSelection(parentPieceId, pieceType);
		}

		// Token: 0x060056D9 RID: 22233 RVA: 0x001B6AAC File Offset: 0x001B4CAC
		public bool ValidateFunctionalPieceState(int pieceID, byte state, NetPlayer player)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			return !(piece == null) && piece.functionalPieceComponent != null && (!NetworkSystem.Instance.IsMasterClient || player.IsMasterClient || this.IsPlayerHandNearAction(player, piece.transform.position, true, false, piece.functionalPieceComponent.GetInteractionDistace())) && piece.functionalPieceComponent.IsStateValid(state);
		}

		// Token: 0x060056DA RID: 22234 RVA: 0x001B6B1C File Offset: 0x001B4D1C
		public void OnFunctionalStateRequest(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			if (piece.functionalPieceComponent == null)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			piece.functionalPieceComponent.OnStateRequest(state, player, timeStamp);
		}

		// Token: 0x060056DB RID: 22235 RVA: 0x001B6B58 File Offset: 0x001B4D58
		public void SetFunctionalPieceState(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FunctionalStateChange,
				pieceId = pieceID,
				twist = state,
				player = player,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x060056DC RID: 22236 RVA: 0x001B6BA4 File Offset: 0x001B4DA4
		public void ExecuteSetFunctionalPieceState(BuilderTable.BuilderCommand cmd)
		{
			BuilderPiece piece = this.GetPiece(cmd.pieceId);
			if (piece == null)
			{
				return;
			}
			piece.SetFunctionalPieceState(cmd.twist, cmd.player, cmd.serverTimeStamp);
		}

		// Token: 0x060056DD RID: 22237 RVA: 0x001B6BE0 File Offset: 0x001B4DE0
		public void RegisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegister.Add(component);
			}
		}

		// Token: 0x060056DE RID: 22238 RVA: 0x001B6BF1 File Offset: 0x001B4DF1
		public void UnregisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToUnregister.Add(component);
			}
		}

		// Token: 0x060056DF RID: 22239 RVA: 0x001B6C02 File Offset: 0x001B4E02
		public void RegisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Add(component);
			}
		}

		// Token: 0x060056E0 RID: 22240 RVA: 0x001B6C13 File Offset: 0x001B4E13
		public void UnregisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Remove(component);
			}
		}

		// Token: 0x060056E1 RID: 22241 RVA: 0x00002789 File Offset: 0x00000989
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x060056E2 RID: 22242 RVA: 0x001B6C28 File Offset: 0x001B4E28
		public void CreatePiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = state,
				player = NetPlayer.Get(player)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x060056E3 RID: 22243 RVA: 0x001B6C90 File Offset: 0x001B4E90
		public void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, playFX, recyclerID);
		}

		// Token: 0x060056E4 RID: 22244 RVA: 0x001B6CBC File Offset: 0x001B4EBC
		public void RecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Recycle,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				player = NetPlayer.Get(player),
				isLeft = playFX,
				parentPieceId = recyclerID
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x060056E5 RID: 22245 RVA: 0x001B6D1B File Offset: 0x001B4F1B
		private bool ShouldExecuteCommand()
		{
			return this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster;
		}

		// Token: 0x060056E6 RID: 22246 RVA: 0x001B6D31 File Offset: 0x001B4F31
		private bool ShouldQueueCommand()
		{
			return this.tableState == BuilderTable.TableState.ReceivingInitialBuild || this.tableState == BuilderTable.TableState.ReceivingMasterResync || this.tableState == BuilderTable.TableState.InitialBuild || this.tableState == BuilderTable.TableState.ExecuteQueuedCommands;
		}

		// Token: 0x060056E7 RID: 22247 RVA: 0x001B6D59 File Offset: 0x001B4F59
		private bool ShouldDiscardCommand()
		{
			return this.tableState == BuilderTable.TableState.WaitingForInitalBuild || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster || this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x060056E8 RID: 22248 RVA: 0x001B6D78 File Offset: 0x001B4F78
		public bool DoesChainContainPiece(BuilderPiece targetPiece, BuilderPiece firstInChain, BuilderPiece nextInChain)
		{
			return !(targetPiece == null) && !(firstInChain == null) && (targetPiece.Equals(firstInChain) || (!(nextInChain == null) && (targetPiece.Equals(nextInChain) || (!(firstInChain == nextInChain) && this.DoesChainContainPiece(targetPiece, firstInChain, nextInChain.parentPiece)))));
		}

		// Token: 0x060056E9 RID: 22249 RVA: 0x001B6DD4 File Offset: 0x001B4FD4
		public bool DoesChainContainChain(BuilderPiece chainARoot, BuilderPiece chainBAttachPiece)
		{
			if (chainARoot == null || chainBAttachPiece == null)
			{
				return false;
			}
			if (this.DoesChainContainPiece(chainARoot, chainBAttachPiece, chainBAttachPiece.parentPiece))
			{
				return true;
			}
			BuilderPiece builderPiece = chainARoot.firstChildPiece;
			while (builderPiece != null)
			{
				if (this.DoesChainContainChain(builderPiece, chainBAttachPiece))
				{
					return true;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
			return false;
		}

		// Token: 0x060056EA RID: 22250 RVA: 0x001B6E30 File Offset: 0x001B5030
		private bool IsPlayerHandNearAction(NetPlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 2.5f)
		{
			bool flag = true;
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				if (isLeftHand || checkBothHands)
				{
					flag = ((worldPosition - rigContainer.Rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius);
				}
				if (!isLeftHand || checkBothHands)
				{
					float sqrMagnitude = (worldPosition - rigContainer.Rig.rightHandTransform.position).sqrMagnitude;
					flag = (flag && sqrMagnitude < acceptableRadius * acceptableRadius);
				}
			}
			return flag;
		}

		// Token: 0x060056EB RID: 22251 RVA: 0x001B6EC4 File Offset: 0x001B50C4
		public bool ValidatePlacePieceParams(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return false;
			}
			if (piece.heldByPlayerActorNumber != placedByPlayer.ActorNumber)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece2.isBuiltIntoTable)
			{
				return false;
			}
			if (twist > 3)
			{
				return false;
			}
			BuilderPiece piece3 = this.GetPiece(parentPieceId);
			if (!(piece3 != null))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(placedByPlayer.ActorNumber, piece2, piece3))
			{
				return false;
			}
			if (this.DoesChainContainChain(piece2, piece3))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			if (piece3 != null && (parentAttachIndex < 0 || parentAttachIndex >= piece3.gridPlanes.Count))
			{
				return false;
			}
			if (piece3 != null)
			{
				bool flag = (long)(twist % 2) == 1L;
				BuilderAttachGridPlane builderAttachGridPlane = piece2.gridPlanes[attachIndex];
				int num = flag ? builderAttachGridPlane.length : builderAttachGridPlane.width;
				int num2 = flag ? builderAttachGridPlane.width : builderAttachGridPlane.length;
				BuilderAttachGridPlane builderAttachGridPlane2 = piece3.gridPlanes[parentAttachIndex];
				int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
				int num4 = num3 - (builderAttachGridPlane2.width - 1);
				if ((int)bumpOffsetX < num4 - num || (int)bumpOffsetX > num3 + num)
				{
					return false;
				}
				int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
				int num6 = num5 - (builderAttachGridPlane2.length - 1);
				if ((int)bumpOffsetZ < num6 - num2 || (int)bumpOffsetZ > num5 + num2)
				{
					return false;
				}
			}
			if (placedByPlayer == null)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient && piece3 != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				Vector3 vector2;
				Quaternion quaternion2;
				piece2.BumpTwistToPositionRotation(twist, bumpOffsetX, bumpOffsetZ, attachIndex, piece3.gridPlanes[parentAttachIndex], out vector, out quaternion, out vector2, out quaternion2);
				Vector3 vector3 = piece2.transform.InverseTransformPoint(piece.transform.position);
				Vector3 worldPosition = vector2 + quaternion2 * vector3;
				if (!this.IsPlayerHandNearAction(placedByPlayer, worldPosition, piece.heldInLeftHand, false, 2.5f))
				{
					return false;
				}
				if (!this.ValidatePieceWorldTransform(vector2, quaternion2))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060056EC RID: 22252 RVA: 0x001B70D8 File Offset: 0x001B52D8
		public bool ValidatePlacePieceState(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			return !(piece2 == null) && !(this.GetPiece(parentPieceId) == null) && placedByPlayer != null && !piece2.GetRootPiece() != piece;
		}

		// Token: 0x060056ED RID: 22253 RVA: 0x001B713C File Offset: 0x001B533C
		public void ExecutePieceCreated(BuilderTable.BuilderCommand cmd)
		{
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateCreatePieceParams(cmd.pieceType, cmd.pieceId, cmd.state, cmd.materialType))
			{
				return;
			}
			BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, cmd.localPosition, cmd.localRotation, cmd.state, cmd.materialType, 0, this);
			if (!(builderPiece != null) || cmd.state != BuilderPiece.State.OnConveyor)
			{
				if (builderPiece != null && cmd.isLeft && cmd.state == BuilderPiece.State.OnShelf)
				{
					if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.dispenserShelves.Count)
					{
						return;
					}
					builderPiece.shelfOwner = cmd.parentPieceId;
					this.dispenserShelves[builderPiece.shelfOwner].OnShelfPieceCreated(builderPiece, true);
				}
				return;
			}
			if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.conveyors.Count)
			{
				return;
			}
			builderPiece.shelfOwner = cmd.parentPieceId;
			BuilderConveyor builderConveyor = this.conveyors[builderPiece.shelfOwner];
			int parentAttachIndex = cmd.parentAttachIndex;
			float timeOffset = 0f;
			if (PhotonNetwork.ServerTimestamp > parentAttachIndex)
			{
				timeOffset = (PhotonNetwork.ServerTimestamp - parentAttachIndex) / 1000f;
			}
			builderConveyor.OnShelfPieceCreated(builderPiece, timeOffset);
		}

		// Token: 0x060056EE RID: 22254 RVA: 0x001B7283 File Offset: 0x001B5483
		public void ExecutePieceRecycled(BuilderTable.BuilderCommand cmd)
		{
			this.RecyclePieceInternal(cmd.pieceId, false, cmd.isLeft, cmd.parentPieceId);
		}

		// Token: 0x060056EF RID: 22255 RVA: 0x001B729E File Offset: 0x001B549E
		private bool ValidateCreatePieceParams(int newPieceType, int newPieceId, BuilderPiece.State state, int materialType)
		{
			return !(this.GetPiecePrefab(newPieceType) == null) && !(this.GetPiece(newPieceId) != null);
		}

		// Token: 0x060056F0 RID: 22256 RVA: 0x001B72C4 File Offset: 0x001B54C4
		private bool ValidateDeserializedRootPieceState(int pieceId, BuilderPiece.State state, int shelfOwner, int heldByActor, Vector3 localPosition, Quaternion localRotation)
		{
			switch (state)
			{
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. held piece in immutable table {0}", pieceId), null);
				}
				else if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			case BuilderPiece.State.Dropped:
				if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. dropped piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				if (shelfOwner == -1 && !this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				break;
			case BuilderPiece.State.OnConveyor:
				if (shelfOwner == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. OnConveyor piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.AttachedToArm:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. AttachedToArm piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		// Token: 0x060056F1 RID: 22257 RVA: 0x001B73DC File Offset: 0x001B55DC
		private bool ValidateDeserializedChildPieceState(int pieceId, BuilderPiece.State state)
		{
			switch (state)
			{
			case BuilderPiece.State.AttachedAndPlaced:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				return true;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
			case BuilderPiece.State.AttachedToArm:
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. Invalid state {0} of child piece {1} in Immutable table", state, pieceId), null);
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060056F2 RID: 22258 RVA: 0x001B7440 File Offset: 0x001B5640
		public bool ValidatePieceWorldTransform(Vector3 position, Quaternion rotation)
		{
			float num = 10000f;
			return position.IsValid(num) && rotation.IsValid() && (this.roomCenter.position - position).sqrMagnitude <= this.acceptableSqrDistFromCenter;
		}

		// Token: 0x060056F3 RID: 22259 RVA: 0x001B7490 File Offset: 0x001B5690
		private BuilderPiece CreatePieceInternal(int newPieceType, int newPieceId, Vector3 position, Quaternion rotation, BuilderPiece.State state, int materialType, int activateTimeStamp, BuilderTable table)
		{
			if (this.GetPiecePrefab(newPieceType) == null)
			{
				return null;
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				this.nextPieceId = newPieceId + 1;
			}
			BuilderPiece builderPiece = this.builderPool.CreatePiece(newPieceType, false);
			builderPiece.SetScale(table.pieceScale);
			builderPiece.transform.SetPositionAndRotation(position, rotation);
			builderPiece.pieceType = newPieceType;
			builderPiece.pieceId = newPieceId;
			builderPiece.SetTable(table);
			builderPiece.gameObject.SetActive(true);
			builderPiece.SetupPiece(this.gridSize);
			builderPiece.OnCreate();
			builderPiece.activatedTimeStamp = ((state == BuilderPiece.State.AttachedAndPlaced) ? activateTimeStamp : 0);
			builderPiece.SetMaterial(materialType, true);
			builderPiece.SetState(state, true);
			this.AddPiece(builderPiece);
			return builderPiece;
		}

		// Token: 0x060056F4 RID: 22260 RVA: 0x001B7544 File Offset: 0x001B5744
		private void RecyclePieceInternal(int pieceId, bool ignoreHaptics, bool playFX, int recyclerId)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (playFX)
			{
				try
				{
					piece.PlayRecycleFx();
				}
				catch (Exception)
				{
				}
			}
			if (!ignoreHaptics)
			{
				BuilderPiece rootPiece = piece.GetRootPiece();
				if (rootPiece != null && rootPiece.IsHeldLocal())
				{
					GorillaTagger.Instance.StartVibration(piece.IsHeldInLeftHand(), GorillaTagger.Instance.tapHapticStrength, this.pushAndEaseParams.snapDelayTime * 2f);
				}
			}
			BuilderPiece builderPiece = piece.firstChildPiece;
			while (builderPiece != null)
			{
				int pieceId2 = builderPiece.pieceId;
				builderPiece = builderPiece.nextSiblingPiece;
				this.RecyclePieceInternal(pieceId2, true, playFX, recyclerId);
			}
			if (this.isTableMutable && recyclerId >= 0 && recyclerId < this.recyclers.Count)
			{
				this.recyclers[recyclerId].OnRecycleRequestedAtRecycler(piece);
			}
			if (piece.state == BuilderPiece.State.OnConveyor && piece.shelfOwner >= 0 && piece.shelfOwner < this.conveyors.Count)
			{
				this.conveyors[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			else if ((piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed) && piece.shelfOwner >= 0 && piece.shelfOwner < this.dispenserShelves.Count)
			{
				this.dispenserShelves[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			if (piece.isArmShelf && this.isTableMutable)
			{
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				int num;
				if (piece.heldInLeftHand && this.playerToArmShelfLeft.TryGetValue(piece.heldByPlayerActorNumber, ref num) && num == piece.pieceId)
				{
					this.playerToArmShelfLeft.Remove(piece.heldByPlayerActorNumber);
				}
				int num2;
				if (!piece.heldInLeftHand && this.playerToArmShelfRight.TryGetValue(piece.heldByPlayerActorNumber, ref num2) && num2 == piece.pieceId)
				{
					this.playerToArmShelfRight.Remove(piece.heldByPlayerActorNumber);
				}
			}
			else if (PhotonNetwork.LocalPlayer.ActorNumber == piece.heldByPlayerActorNumber)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			int pieceId3 = piece.pieceId;
			piece.ClearParentPiece(false);
			piece.ClearParentHeld();
			piece.SetState(BuilderPiece.State.None, false);
			this.RemovePiece(piece);
			this.builderPool.DestroyPiece(piece);
		}

		// Token: 0x060056F5 RID: 22261 RVA: 0x001B77A0 File Offset: 0x001B59A0
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			return BuilderSetManager.instance.GetPiecePrefab(pieceType);
		}

		// Token: 0x060056F6 RID: 22262 RVA: 0x001B77B0 File Offset: 0x001B59B0
		private bool ValidateAttachPieceParams(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int piecePlacement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece2 == null)
			{
				return false;
			}
			if ((piecePlacement & 262143) != piecePlacement)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (this.DoesChainContainChain(piece, piece2))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece.gridPlanes.Count)
			{
				return false;
			}
			if (parentAttachIndex < 0 || parentAttachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(piecePlacement, out b, out b2, out b3);
			bool flag = (long)(b % 2) == 1L;
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[attachIndex];
			int num = flag ? builderAttachGridPlane.length : builderAttachGridPlane.width;
			int num2 = flag ? builderAttachGridPlane.width : builderAttachGridPlane.length;
			BuilderAttachGridPlane builderAttachGridPlane2 = piece2.gridPlanes[parentAttachIndex];
			int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
			int num4 = num3 - (builderAttachGridPlane2.width - 1);
			if ((int)b2 < num4 - num || (int)b2 > num3 + num)
			{
				return false;
			}
			int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
			int num6 = num5 - (builderAttachGridPlane2.length - 1);
			return (int)b3 >= num6 - num2 && (int)b3 <= num5 + num2;
		}

		// Token: 0x060056F7 RID: 22263 RVA: 0x001B78FC File Offset: 0x001B5AFC
		private void AttachPieceInternal(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int placement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece == null)
			{
				return;
			}
			byte b;
			sbyte xOffset;
			sbyte zOffset;
			BuilderTable.UnpackPiecePlacement(placement, out b, out xOffset, out zOffset);
			Vector3 zero = Vector3.zero;
			Quaternion quaternion;
			if (piece2 != null && parentAttachIndex >= 0 && parentAttachIndex < piece2.gridPlanes.Count)
			{
				Vector3 vector;
				Quaternion quaternion2;
				piece.BumpTwistToPositionRotation(b, xOffset, zOffset, attachIndex, piece2.gridPlanes[parentAttachIndex], out zero, out quaternion, out vector, out quaternion2);
			}
			else
			{
				quaternion = Quaternion.Euler(0f, (float)b * 90f, 0f);
			}
			piece.SetParentPiece(attachIndex, piece2, parentAttachIndex);
			piece.transform.SetLocalPositionAndRotation(zero, quaternion);
		}

		// Token: 0x060056F8 RID: 22264 RVA: 0x001B79A8 File Offset: 0x001B5BA8
		private void AttachPieceToActorInternal(int pieceId, int actorNumber, bool isLeftHand)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				return;
			}
			VRRig rig = rigContainer.Rig;
			BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
			Transform parentHeld = isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
			if (piece.isArmShelf)
			{
				if (!this.isTableMutable)
				{
					return;
				}
				parentHeld = (isLeftHand ? rig.builderArmShelfLeft.pieceAnchor : rig.builderArmShelfRight.pieceAnchor);
				if (isLeftHand)
				{
					rig.builderArmShelfLeft.piece = piece;
					piece.armShelf = rig.builderArmShelfLeft;
					int num;
					if (this.playerToArmShelfLeft.TryGetValue(actorNumber, ref num) && num != pieceId)
					{
						BuilderPiece piece2 = this.GetPiece(num);
						if (piece2 != null && piece2.isArmShelf)
						{
							piece2.ClearParentHeld();
							this.playerToArmShelfLeft.Remove(actorNumber);
						}
					}
					this.playerToArmShelfLeft.TryAdd(actorNumber, pieceId);
				}
				else
				{
					rig.builderArmShelfRight.piece = piece;
					piece.armShelf = rig.builderArmShelfRight;
					int num2;
					if (this.playerToArmShelfRight.TryGetValue(actorNumber, ref num2) && num2 != pieceId)
					{
						BuilderPiece piece3 = this.GetPiece(num2);
						if (piece3 != null && piece3.isArmShelf)
						{
							piece3.ClearParentHeld();
							this.playerToArmShelfRight.Remove(actorNumber);
						}
					}
					this.playerToArmShelfRight.TryAdd(actorNumber, pieceId);
				}
			}
			Vector3 localPosition = piece.transform.localPosition;
			Quaternion localRotation = piece.transform.localRotation;
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.SetParentHeld(parentHeld, actorNumber, isLeftHand);
			piece.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			BuilderPiece.State newState = player.IsLocal ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed;
			if (piece.isArmShelf)
			{
				newState = BuilderPiece.State.AttachedToArm;
				piece.transform.localScale = Vector3.one;
			}
			piece.SetState(newState, false);
			if (!player.IsLocal)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			if (player.IsLocal && !piece.isArmShelf)
			{
				BuilderPieceInteractor.instance.AddPieceToHeld(piece, isLeftHand, localPosition, localRotation);
			}
		}

		// Token: 0x060056F9 RID: 22265 RVA: 0x001B7BC0 File Offset: 0x001B5DC0
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
		}

		// Token: 0x060056FA RID: 22266 RVA: 0x001B7BF0 File Offset: 0x001B5DF0
		public void PlacePiece(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			this.PiecePlacedInternal(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer, timeStamp, force);
		}

		// Token: 0x060056FB RID: 22267 RVA: 0x001B7C18 File Offset: 0x001B5E18
		public void PiecePlacedInternal(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			if (!force && placedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Place,
				pieceId = pieceId,
				bumpOffsetX = bumpOffsetX,
				bumpOffsetZ = bumpOffsetZ,
				twist = twist,
				attachPieceId = attachPieceId,
				parentPieceId = parentPieceId,
				attachIndex = attachIndex,
				parentAttachIndex = parentAttachIndex,
				player = placedByPlayer,
				canRollback = force,
				localCommandId = localCommandId,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x060056FC RID: 22268 RVA: 0x001B7CD0 File Offset: 0x001B5ED0
		public void ExecutePiecePlacedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int attachPieceId = cmd.attachPieceId;
			int parentPieceId = cmd.parentPieceId;
			int parentAttachIndex = cmd.parentAttachIndex;
			int attachIndex = cmd.attachIndex;
			NetPlayer player = cmd.player;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			byte twist = cmd.twist;
			sbyte bumpOffsetX = cmd.bumpOffsetX;
			sbyte bumpOffsetZ = cmd.bumpOffsetZ;
			if ((player == null || !player.IsLocal) && !this.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return;
			}
			BuilderAction action = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction action2 = BuilderActions.CreateMakeRoot(localCommandId, attachPieceId);
			BuilderAction action3 = BuilderActions.CreateAttachToPiece(localCommandId, attachPieceId, cmd.parentPieceId, cmd.attachIndex, cmd.parentAttachIndex, bumpOffsetX, bumpOffsetZ, twist, actorNumber, cmd.serverTimeStamp);
			if (cmd.canRollback)
			{
				BuilderAction action4 = BuilderActions.CreateDetachFromPiece(localCommandId, attachPieceId, actorNumber);
				BuilderAction action5 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderAction action6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(action6);
				this.AddRollbackAction(action5);
				this.AddRollbackAction(action4);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
			this.ExecuteAction(action3);
			if (!cmd.isQueued)
			{
				piece2.PlayPlacementFx();
			}
		}

		// Token: 0x060056FD RID: 22269 RVA: 0x001B7E34 File Offset: 0x001B6034
		public bool ValidateGrabPieceParams(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
		{
			float num = 10000f;
			if (!localPosition.IsValid(num) || !localRotation.IsValid())
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (grabbedByPlayer == null)
			{
				return false;
			}
			if (!piece.CanPlayerGrabPiece(grabbedByPlayer.ActorNumber, piece.transform.position))
			{
				return false;
			}
			if (localPosition.sqrMagnitude > 6400f)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				Vector3 position = piece.transform.position;
				if (!this.IsPlayerHandNearAction(grabbedByPlayer, position, isLeftHand, false, 2.5f))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060056FE RID: 22270 RVA: 0x001B7ED4 File Offset: 0x001B60D4
		public bool ValidateGrabPieceState(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, Player grabbedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			return !(piece == null) && piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.None;
		}

		// Token: 0x060056FF RID: 22271 RVA: 0x001B7F0C File Offset: 0x001B610C
		public bool IsLocationWithinSharedBuildArea(Vector3 worldPosition)
		{
			Vector3 vector = this.sharedBuildArea.transform.InverseTransformPoint(worldPosition);
			foreach (BoxCollider boxCollider in this.sharedBuildAreas)
			{
				Vector3 vector2 = boxCollider.center + boxCollider.size / 2f;
				Vector3 vector3 = boxCollider.center - boxCollider.size / 2f;
				if (vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005700 RID: 22272 RVA: 0x001B7FEC File Offset: 0x001B61EC
		private bool NoBlocksCheck()
		{
			foreach (BuilderTable.BoxCheckParams boxCheckParams in this.noBlocksAreas)
			{
				DebugUtil.DrawBox(boxCheckParams.center, boxCheckParams.rotation, boxCheckParams.halfExtents * 2f, Color.magenta, true, DebugUtil.Style.Wireframe);
				int num = 0;
				num |= 1 << BuilderTable.placedLayer;
				int num2 = Physics.OverlapBoxNonAlloc(boxCheckParams.center, boxCheckParams.halfExtents, this.noBlocksCheckResults, boxCheckParams.rotation, num);
				for (int i = 0; i < num2; i++)
				{
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.noBlocksCheckResults[i]);
					if (builderPieceFromCollider != null && builderPieceFromCollider.GetTable() == this && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable)
					{
						GTDev.LogError<string>(string.Format("Builder Table found piece {0} {1} in NO BLOCK AREA {2}", builderPieceFromCollider.pieceId, builderPieceFromCollider.displayName, builderPieceFromCollider.transform.position), null);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06005701 RID: 22273 RVA: 0x001B811C File Offset: 0x001B631C
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
		}

		// Token: 0x06005702 RID: 22274 RVA: 0x001B8138 File Offset: 0x001B6338
		public void GrabPiece(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			this.PieceGrabbedInternal(localCommandId, pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer, force);
		}

		// Token: 0x06005703 RID: 22275 RVA: 0x001B814C File Offset: 0x001B634C
		public void PieceGrabbedInternal(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			if (!force && grabbedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Grab,
				pieceId = pieceId,
				attachPieceId = -1,
				isLeft = isLeftHand,
				localPosition = localPosition,
				localRotation = localRotation,
				player = grabbedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005704 RID: 22276 RVA: 0x001B81E0 File Offset: 0x001B63E0
		public void ExecutePieceGrabbedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			bool isLeft = cmd.isLeft;
			NetPlayer player = cmd.player;
			Vector3 localPosition = cmd.localPosition;
			Quaternion localRotation = cmd.localRotation;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((player == null || !player.Equals(NetworkSystem.Instance.LocalPlayer)) && !this.ValidateGrabPieceParams(pieceId, isLeft, localPosition, localRotation, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			bool flag = PhotonNetwork.CurrentRoom.GetPlayer(piece.heldByPlayerActorNumber, false) != null;
			bool flag2 = BuilderPiece.IsDroppedState(piece.state);
			bool flag3 = piece.state == BuilderPiece.State.OnConveyor || piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed;
			BuilderAction action = BuilderActions.CreateAttachToPlayer(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, actorNumber, cmd.isLeft);
			BuilderAction action2 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			if (flag)
			{
				BuilderAction action3 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				if (cmd.canRollback)
				{
					BuilderAction action4 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
					this.AddRollbackAction(action4);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action3);
				this.ExecuteAction(action);
				return;
			}
			if (flag3)
			{
				BuilderAction action5;
				if (piece.state == BuilderPiece.State.OnConveyor)
				{
					int serverTimestamp = PhotonNetwork.ServerTimestamp;
					float splineProgressForPiece = this.conveyorManager.GetSplineProgressForPiece(piece);
					action5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, true, serverTimestamp, splineProgressForPiece);
				}
				else
				{
					if (piece.state == BuilderPiece.State.Displayed)
					{
						int actorNumber2 = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					}
					action5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, false, 0, 0f);
				}
				BuilderAction action6 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece = piece.GetRootPiece();
				BuilderAction action7 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action5);
					this.AddRollbackAction(action7);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action6);
				this.ExecuteAction(action);
				return;
			}
			if (flag2)
			{
				BuilderAction action8 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece2 = piece.GetRootPiece();
				BuilderAction action9 = BuilderActions.CreateDropPieceRollback(localCommandId, rootPiece2, actorNumber);
				BuilderAction action10 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece2.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action9);
					this.AddRollbackAction(action10);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action8);
				this.ExecuteAction(action);
				return;
			}
			if (piece.parentPiece != null)
			{
				BuilderAction action11 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction action12 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action12);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action11);
				this.ExecuteAction(action);
			}
		}

		// Token: 0x06005705 RID: 22277 RVA: 0x001B84B4 File Offset: 0x001B66B4
		public bool ValidateDropPieceParams(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer)
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3))
					{
						BuilderPiece piece = this.GetPiece(pieceId);
						if (piece == null)
						{
							return false;
						}
						if (piece.isBuiltIntoTable)
						{
							return false;
						}
						if (droppedByPlayer == null)
						{
							return false;
						}
						if (velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
						{
							return false;
						}
						if (angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
						{
							return false;
						}
						if ((this.roomCenter.position - position).sqrMagnitude > this.acceptableSqrDistFromCenter)
						{
							return false;
						}
						if (piece.state == BuilderPiece.State.AttachedToArm)
						{
							if (piece.parentPiece == null)
							{
								return false;
							}
							if (piece.parentPiece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
							{
								return false;
							}
						}
						else if (piece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
						{
							return false;
						}
						return !PhotonNetwork.IsMasterClient || this.IsPlayerHandNearAction(droppedByPlayer, position, piece.heldInLeftHand, false, 2.5f);
					}
				}
			}
			return false;
		}

		// Token: 0x06005706 RID: 22278 RVA: 0x001B85D4 File Offset: 0x001B67D4
		public bool ValidateDropPieceState(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			bool flag = piece.state == BuilderPiece.State.AttachedToArm;
			return (flag || piece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber) && (!flag || piece.parentPiece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber);
		}

		// Token: 0x06005707 RID: 22279 RVA: 0x001B862A File Offset: 0x001B682A
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
		}

		// Token: 0x06005708 RID: 22280 RVA: 0x001B8648 File Offset: 0x001B6848
		public void DropPiece(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			this.PieceDroppedInternal(localCommandId, pieceId, position, rotation, velocity, angVelocity, droppedByPlayer, force);
		}

		// Token: 0x06005709 RID: 22281 RVA: 0x001B8668 File Offset: 0x001B6868
		public void PieceDroppedInternal(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			if (!force && droppedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Drop,
				pieceId = pieceId,
				parentPieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				velocity = velocity,
				angVelocity = angVelocity,
				player = droppedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x0600570A RID: 22282 RVA: 0x001B8704 File Offset: 0x001B6904
		public void ExecutePieceDroppedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateDropPieceParams(pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, cmd.player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm)
			{
				BuilderPiece parentPiece = piece.parentPiece;
				BuilderAction action = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction action2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
				if (cmd.canRollback)
				{
					BuilderAction action3 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
					this.AddRollbackAction(action3);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action);
				this.ExecuteAction(action2);
				return;
			}
			BuilderAction action4 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction action5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
			if (cmd.canRollback)
			{
				BuilderAction action6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(action6);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(action4);
			this.ExecuteAction(action5);
		}

		// Token: 0x0600570B RID: 22283 RVA: 0x001B8848 File Offset: 0x001B6A48
		public void ExecutePieceRepelled(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			int attachPieceId = cmd.attachPieceId;
			BuilderPiece piece = this.GetPiece(pieceId);
			Vector3 velocity = cmd.velocity;
			if (piece == null)
			{
				return;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return;
			}
			if (piece.state != BuilderPiece.State.Grabbed && piece.state != BuilderPiece.State.GrabbedLocal && piece.state != BuilderPiece.State.Dropped && piece.state != BuilderPiece.State.AttachedToDropped && piece.state != BuilderPiece.State.AttachedToArm)
			{
				return;
			}
			if (attachPieceId >= 0 && attachPieceId < this.dropZones.Count)
			{
				BuilderDropZone builderDropZone = this.dropZones[attachPieceId];
				builderDropZone.PlayEffect();
				if (builderDropZone.overrideDirection)
				{
					velocity = builderDropZone.GetRepelDirectionWorld() * BuilderTable.DROP_ZONE_REPEL;
				}
			}
			if (piece.heldByPlayerActorNumber >= 0)
			{
				BuilderAction action = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction action2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
				this.ExecuteAction(action);
				this.ExecuteAction(action2);
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm && piece.parentPiece != null)
			{
				BuilderAction action3 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction action4 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
				this.ExecuteAction(action3);
				this.ExecuteAction(action4);
				return;
			}
			BuilderAction action5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
			this.ExecuteAction(action5);
		}

		// Token: 0x0600570C RID: 22284 RVA: 0x001B89E8 File Offset: 0x001B6BE8
		private void CleanUpDroppedPiece()
		{
			if (!PhotonNetwork.IsMasterClient || this.droppedPieces.Count <= BuilderTable.DROPPED_PIECE_LIMIT)
			{
				return;
			}
			BuilderPiece builderPiece = this.FindFirstSleepingPiece();
			if (builderPiece != null && builderPiece.state == BuilderPiece.State.Dropped)
			{
				this.RequestRecyclePiece(builderPiece, false, -1);
				return;
			}
			Debug.LogErrorFormat("Piece {0} in Dropped List is {1}", new object[]
			{
				builderPiece.pieceId,
				builderPiece.state
			});
		}

		// Token: 0x0600570D RID: 22285 RVA: 0x001B8A60 File Offset: 0x001B6C60
		public void FreezeDroppedPiece(BuilderPiece piece)
		{
			int num = this.droppedPieces.IndexOf(piece);
			if (num >= 0)
			{
				BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[num];
				droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Frozen;
				droppedPieceData.speedThreshCrossedTime = 0f;
				this.droppedPieceData[num] = droppedPieceData;
				if (piece.rigidBody != null)
				{
					piece.SetKinematic(true, false);
				}
				piece.forcedFrozen = true;
			}
		}

		// Token: 0x0600570E RID: 22286 RVA: 0x001B8ACC File Offset: 0x001B6CCC
		public void AddPieceToDropList(BuilderPiece piece)
		{
			this.droppedPieces.Add(piece);
			this.droppedPieceData.Add(new BuilderTable.DroppedPieceData
			{
				speedThreshCrossedTime = 0f,
				droppedState = BuilderTable.DroppedPieceState.Light,
				filteredSpeed = 0f
			});
		}

		// Token: 0x0600570F RID: 22287 RVA: 0x001B8B1C File Offset: 0x001B6D1C
		private BuilderPiece FindFirstSleepingPiece()
		{
			if (this.droppedPieces.Count < 1)
			{
				return null;
			}
			BuilderPiece builderPiece = this.droppedPieces[0];
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieces[i].rigidBody != null && this.droppedPieces[i].rigidBody.IsSleeping())
				{
					BuilderPiece result = this.droppedPieces[i];
					this.droppedPieces.RemoveAt(i);
					this.droppedPieceData.RemoveAt(i);
					return result;
				}
			}
			BuilderPiece result2 = this.droppedPieces[0];
			this.droppedPieces.RemoveAt(0);
			this.droppedPieceData.RemoveAt(0);
			return result2;
		}

		// Token: 0x06005710 RID: 22288 RVA: 0x001B8BD6 File Offset: 0x001B6DD6
		public void RemovePieceFromDropList(BuilderPiece piece)
		{
			if (piece.state == BuilderPiece.State.Dropped)
			{
				this.droppedPieces.Remove(piece);
			}
		}

		// Token: 0x06005711 RID: 22289 RVA: 0x001B8BF0 File Offset: 0x001B6DF0
		private void UpdateDroppedPieces(float dt)
		{
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieceData[i].droppedState == BuilderTable.DroppedPieceState.Frozen && this.droppedPieces[i].state == BuilderPiece.State.Dropped)
				{
					BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[i];
					droppedPieceData.speedThreshCrossedTime += dt;
					if (droppedPieceData.speedThreshCrossedTime > 60f)
					{
						this.droppedPieces[i].forcedFrozen = false;
						this.droppedPieces[i].ClearCollisionHistory();
						this.droppedPieces[i].SetKinematic(false, true);
						droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Light;
						droppedPieceData.speedThreshCrossedTime = 0f;
					}
					this.droppedPieceData[i] = droppedPieceData;
				}
				else
				{
					Rigidbody rigidBody = this.droppedPieces[i].rigidBody;
					if (rigidBody != null)
					{
						BuilderTable.DroppedPieceData droppedPieceData2 = this.droppedPieceData[i];
						float magnitude = rigidBody.linearVelocity.magnitude;
						droppedPieceData2.filteredSpeed = droppedPieceData2.filteredSpeed * 0.95f + magnitude * 0.05f;
						switch (droppedPieceData2.droppedState)
						{
						case BuilderTable.DroppedPieceState.Light:
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed < 0.05f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0f)
							{
								rigidBody.mass = 10000f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Heavy;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						case BuilderTable.DroppedPieceState.Heavy:
							droppedPieceData2.speedThreshCrossedTime += dt;
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed > 0.075f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0.5f)
							{
								rigidBody.mass = 1f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Light;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						}
						this.droppedPieceData[i] = droppedPieceData2;
					}
				}
			}
		}

		// Token: 0x06005712 RID: 22290 RVA: 0x001B8E09 File Offset: 0x001B7009
		private void SetLocalPlayerOwnsPlot(bool ownsPlot)
		{
			this.doesLocalPlayerOwnPlot = ownsPlot;
			UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
			if (onLocalPlayerClaimedPlot == null)
			{
				return;
			}
			onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
		}

		// Token: 0x06005713 RID: 22291 RVA: 0x001B8E28 File Offset: 0x001B7028
		public void PlotClaimed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.ClaimPlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005714 RID: 22292 RVA: 0x001B8E64 File Offset: 0x001B7064
		public void ExecuteClaimPlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (pieceId == -1)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (this.plotOwners.TryAdd(player.ActorNumber, pieceId) && piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				builderPiecePrivatePlot.ClaimPlotForPlayerNumber(player.ActorNumber);
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(true);
				}
			}
		}

		// Token: 0x06005715 RID: 22293 RVA: 0x001B8EE8 File Offset: 0x001B70E8
		public void PlayerLeftRoom(int playerActorNumber)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.PlayerLeftRoom,
				pieceId = playerActorNumber,
				player = null
			};
			bool force = this.tableState == BuilderTable.TableState.WaitForMasterResync;
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005716 RID: 22294 RVA: 0x001B8F2C File Offset: 0x001B712C
		public void ExecutePlayerLeftRoom(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			int num = (player != null) ? player.ActorNumber : cmd.pieceId;
			this.FreePlotInternal(-1, num);
			int pieceId;
			if (this.playerToArmShelfLeft.TryGetValue(num, ref pieceId))
			{
				this.RecyclePieceInternal(pieceId, true, false, -1);
			}
			this.playerToArmShelfLeft.Remove(num);
			int pieceId2;
			if (this.playerToArmShelfRight.TryGetValue(num, ref pieceId2))
			{
				this.RecyclePieceInternal(pieceId2, true, false, -1);
			}
			this.playerToArmShelfRight.Remove(num);
		}

		// Token: 0x06005717 RID: 22295 RVA: 0x001B8FA8 File Offset: 0x001B71A8
		public void PlotFreed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FreePlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005718 RID: 22296 RVA: 0x001B8FE4 File Offset: 0x001B71E4
		public void ExecuteFreePlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			this.FreePlotInternal(pieceId, player.ActorNumber);
		}

		// Token: 0x06005719 RID: 22297 RVA: 0x001B9010 File Offset: 0x001B7210
		private void FreePlotInternal(int plotPieceId, int requestingPlayer)
		{
			if (plotPieceId == -1 && !this.plotOwners.TryGetValue(requestingPlayer, ref plotPieceId))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(plotPieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				int ownerActorNumber = builderPiecePrivatePlot.GetOwnerActorNumber();
				this.plotOwners.Remove(ownerActorNumber);
				builderPiecePrivatePlot.FreePlot();
				if (ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(false);
				}
			}
		}

		// Token: 0x0600571A RID: 22298 RVA: 0x001B9084 File Offset: 0x001B7284
		public bool DoesPlayerOwnPlot(int actorNum)
		{
			return this.plotOwners.ContainsKey(actorNum);
		}

		// Token: 0x0600571B RID: 22299 RVA: 0x001B9092 File Offset: 0x001B7292
		public void RequestPaintPiece(int pieceId, int materialType)
		{
			this.builderNetworking.RequestPaintPiece(pieceId, materialType);
		}

		// Token: 0x0600571C RID: 22300 RVA: 0x001B90A1 File Offset: 0x001B72A1
		public void PaintPiece(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			this.PaintPieceInternal(pieceId, materialType, paintingPlayer, force);
		}

		// Token: 0x0600571D RID: 22301 RVA: 0x001B90B0 File Offset: 0x001B72B0
		private void PaintPieceInternal(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			if (!force && paintingPlayer == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Paint,
				pieceId = pieceId,
				materialType = materialType,
				player = NetPlayer.Get(paintingPlayer)
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x0600571E RID: 22302 RVA: 0x001B9104 File Offset: 0x001B7304
		public void ExecutePiecePainted(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int materialType = cmd.materialType;
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece != null && !piece.isBuiltIntoTable)
			{
				piece.SetMaterial(materialType, false);
			}
		}

		// Token: 0x0600571F RID: 22303 RVA: 0x001B9140 File Offset: 0x001B7340
		public void CreateArmShelvesForPlayersInBuilder()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				foreach (Player player in this.builderNetworking.armShelfRequests)
				{
					if (player != null)
					{
						this.builderNetworking.RequestCreateArmShelfForPlayer(player);
					}
				}
				this.builderNetworking.armShelfRequests.Clear();
			}
		}

		// Token: 0x06005720 RID: 22304 RVA: 0x001B91C8 File Offset: 0x001B73C8
		public void RemoveArmShelfForPlayer(Player player)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				this.builderNetworking.armShelfRequests.Remove(player);
				return;
			}
			int pieceId;
			if (this.playerToArmShelfLeft.TryGetValue(player.ActorNumber, ref pieceId))
			{
				BuilderPiece piece = this.GetPiece(pieceId);
				this.playerToArmShelfLeft.Remove(player.ActorNumber);
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(pieceId, piece.transform.position, piece.transform.rotation, false, -1);
				}
				else
				{
					this.DropPieceForPlayerLeavingInternal(piece, player.ActorNumber);
				}
			}
			int pieceId2;
			if (this.playerToArmShelfRight.TryGetValue(player.ActorNumber, ref pieceId2))
			{
				BuilderPiece piece2 = this.GetPiece(pieceId2);
				this.playerToArmShelfRight.Remove(player.ActorNumber);
				if (piece2.armShelf != null)
				{
					piece2.armShelf.piece = null;
					piece2.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(pieceId2, piece2.transform.position, piece2.transform.rotation, false, -1);
					return;
				}
				this.DropPieceForPlayerLeavingInternal(piece2, player.ActorNumber);
			}
		}

		// Token: 0x06005721 RID: 22305 RVA: 0x001B9314 File Offset: 0x001B7514
		public void DropAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.DropPieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06005722 RID: 22306 RVA: 0x001B9374 File Offset: 0x001B7574
		public void RecycleAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.RecyclePieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06005723 RID: 22307 RVA: 0x001B93D4 File Offset: 0x001B75D4
		private void DropPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction action = BuilderActions.CreateDetachFromPlayer(-1, piece.pieceId, playerActorNumber);
			BuilderAction action2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
		}

		// Token: 0x06005724 RID: 22308 RVA: 0x001B942B File Offset: 0x001B762B
		private void RecyclePieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, false, -1);
		}

		// Token: 0x06005725 RID: 22309 RVA: 0x001B9458 File Offset: 0x001B7658
		private void DetachPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction action = BuilderActions.CreateDetachFromPiece(-1, piece.pieceId, playerActorNumber);
			BuilderAction action2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
		}

		// Token: 0x06005726 RID: 22310 RVA: 0x001B94B0 File Offset: 0x001B76B0
		public void CreateArmShelf(int pieceIdLeft, int pieceIdRight, int pieceType, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdLeft,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = true
			};
			this.RouteNewCommand(cmd, false);
			BuilderTable.BuilderCommand cmd2 = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdRight,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = false
			};
			this.RouteNewCommand(cmd2, false);
		}

		// Token: 0x06005727 RID: 22311 RVA: 0x001B9540 File Offset: 0x001B7740
		public void ExecuteArmShelfCreated(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			bool isLeft = cmd.isLeft;
			if (this.GetPiece(cmd.pieceId) != null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				BuilderArmShelf builderArmShelf = isLeft ? rigContainer.Rig.builderArmShelfLeft : rigContainer.Rig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					if (builderArmShelf.piece != null)
					{
						if (builderArmShelf.piece.isArmShelf && builderArmShelf.piece.isActiveAndEnabled)
						{
							builderArmShelf.piece.armShelf = null;
							this.RecyclePiece(builderArmShelf.piece.pieceId, builderArmShelf.piece.transform.position, builderArmShelf.piece.transform.rotation, false, -1, PhotonNetwork.LocalPlayer);
						}
						else
						{
							builderArmShelf.piece = null;
						}
						BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece;
						builderPiece.armShelf = builderArmShelf;
						builderPiece.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.AddOrUpdate(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.AddOrUpdate(player.ActorNumber, cmd.pieceId);
						return;
					}
					else
					{
						BuilderPiece builderPiece2 = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece2;
						builderPiece2.armShelf = builderArmShelf;
						builderPiece2.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece2.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.TryAdd(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.TryAdd(player.ActorNumber, cmd.pieceId);
					}
				}
			}
		}

		// Token: 0x06005728 RID: 22312 RVA: 0x001B978C File Offset: 0x001B798C
		public void ClearLocalArmShelf()
		{
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig != null)
			{
				BuilderArmShelf builderArmShelf = offlineVRRig.builderArmShelfLeft;
				if (builderArmShelf != null)
				{
					BuilderPiece piece = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece != null)
					{
						piece.transform.SetParent(null);
					}
				}
				builderArmShelf = offlineVRRig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					BuilderPiece piece2 = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece2 != null)
					{
						piece2.transform.SetParent(null);
					}
				}
			}
		}

		// Token: 0x06005729 RID: 22313 RVA: 0x001B9814 File Offset: 0x001B7A14
		public void PieceEnteredDropZone(int pieceId, Vector3 worldPos, Quaternion worldRot, int dropZoneId)
		{
			Vector3 velocity = (this.roomCenter.position - worldPos).normalized * BuilderTable.DROP_ZONE_REPEL;
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Repel,
				pieceId = pieceId,
				parentPieceId = pieceId,
				attachPieceId = dropZoneId,
				localPosition = worldPos,
				localRotation = worldRot,
				velocity = velocity,
				angVelocity = Vector3.zero,
				player = NetworkSystem.Instance.MasterClient,
				canRollback = false
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x0600572A RID: 22314 RVA: 0x001B98B8 File Offset: 0x001B7AB8
		public bool ValidateRepelPiece(BuilderPiece piece)
		{
			if (!this.isSetup)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return false;
			}
			if (piece.state == BuilderPiece.State.Grabbed || piece.state == BuilderPiece.State.GrabbedLocal || piece.state == BuilderPiece.State.Dropped || piece.state == BuilderPiece.State.AttachedToDropped || piece.state == BuilderPiece.State.AttachedToArm)
			{
				bool flag = false;
				for (int i = 0; i < this.repelHistoryLength; i++)
				{
					flag = (flag || this.repelledPieceRoots[i].Contains(piece.pieceId));
					if (flag)
					{
						return false;
					}
				}
				this.repelledPieceRoots[this.repelHistoryIndex].Add(piece.pieceId);
				return true;
			}
			return false;
		}

		// Token: 0x0600572B RID: 22315 RVA: 0x001B995C File Offset: 0x001B7B5C
		public void RepelPieceTowardTable(int pieceID)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			Vector3 position = piece.transform.position;
			Quaternion rotation = piece.transform.rotation;
			if (position.y < this.tableCenter.position.y)
			{
				position.y = this.tableCenter.position.y;
			}
			Vector3 linearVelocity = (this.tableCenter.position - position).normalized * BuilderTable.DROP_ZONE_REPEL;
			if (piece.IsHeldLocal())
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.transform.localScale = Vector3.one;
			piece.SetState(BuilderPiece.State.Dropped, false);
			piece.transform.SetLocalPositionAndRotation(position, rotation);
			if (piece.rigidBody != null)
			{
				piece.rigidBody.position = position;
				piece.rigidBody.rotation = rotation;
				piece.rigidBody.linearVelocity = linearVelocity;
				piece.rigidBody.AddForce(Vector3.up * (BuilderTable.DROP_ZONE_REPEL / 2f) * piece.rigidBody.mass, 1);
				piece.rigidBody.angularVelocity = Vector3.zero;
			}
		}

		// Token: 0x0600572C RID: 22316 RVA: 0x001B9AA4 File Offset: 0x001B7CA4
		public BuilderPiece GetPiece(int pieceId)
		{
			int num;
			if (this.pieceIDToIndexCache.TryGetValue(pieceId, ref num))
			{
				if (num >= 0 && num < this.pieces.Count)
				{
					return this.pieces[num];
				}
				this.pieceIDToIndexCache.Remove(pieceId);
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].pieceId == pieceId)
				{
					this.pieceIDToIndexCache.Add(pieceId, i);
					return this.pieces[i];
				}
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				if (this.basePieces[j].pieceId == pieceId)
				{
					return this.basePieces[j];
				}
			}
			return null;
		}

		// Token: 0x0600572D RID: 22317 RVA: 0x001B9B69 File Offset: 0x001B7D69
		public void AddPiece(BuilderPiece piece)
		{
			this.pieces.Add(piece);
			this.UseResources(piece);
			this.AddPieceData(piece);
		}

		// Token: 0x0600572E RID: 22318 RVA: 0x001B9B86 File Offset: 0x001B7D86
		public void RemovePiece(BuilderPiece piece)
		{
			this.pieces.Remove(piece);
			this.AddResources(piece);
			this.RemovePieceData(piece);
			this.pieceIDToIndexCache.Clear();
		}

		// Token: 0x0600572F RID: 22319 RVA: 0x00002789 File Offset: 0x00000989
		private void CreateData()
		{
		}

		// Token: 0x06005730 RID: 22320 RVA: 0x00002789 File Offset: 0x00000989
		private void DestroyData()
		{
		}

		// Token: 0x06005731 RID: 22321 RVA: 0x000F2F39 File Offset: 0x000F1139
		private int AddPieceData(BuilderPiece piece)
		{
			return -1;
		}

		// Token: 0x06005732 RID: 22322 RVA: 0x00002789 File Offset: 0x00000989
		public void UpdatePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06005733 RID: 22323 RVA: 0x00002789 File Offset: 0x00000989
		private void RemovePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06005734 RID: 22324 RVA: 0x000F2F39 File Offset: 0x000F1139
		private int AddGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
			return -1;
		}

		// Token: 0x06005735 RID: 22325 RVA: 0x00002789 File Offset: 0x00000989
		private void RemoveGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
		}

		// Token: 0x06005736 RID: 22326 RVA: 0x000F2F39 File Offset: 0x000F1139
		private int AddPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			return -1;
		}

		// Token: 0x06005737 RID: 22327 RVA: 0x00002789 File Offset: 0x00000989
		private void RemovePrivatePlotData(BuilderPiecePrivatePlot plot)
		{
		}

		// Token: 0x06005738 RID: 22328 RVA: 0x001B9BAE File Offset: 0x001B7DAE
		public void OnButtonFreeRotation(BuilderOptionButton button, bool isLeftHand)
		{
			this.useSnapRotation = !this.useSnapRotation;
			button.SetPressed(this.useSnapRotation);
		}

		// Token: 0x06005739 RID: 22329 RVA: 0x001B9BCB File Offset: 0x001B7DCB
		public void OnButtonFreePosition(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.usePlacementStyle == BuilderPlacementStyle.Float)
			{
				this.usePlacementStyle = BuilderPlacementStyle.SnapDown;
			}
			else if (this.usePlacementStyle == BuilderPlacementStyle.SnapDown)
			{
				this.usePlacementStyle = BuilderPlacementStyle.Float;
			}
			button.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
		}

		// Token: 0x0600573A RID: 22330 RVA: 0x00002789 File Offset: 0x00000989
		public void OnButtonSaveLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x0600573B RID: 22331 RVA: 0x00002789 File Offset: 0x00000989
		public void OnButtonClearLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x0600573C RID: 22332 RVA: 0x001B9C00 File Offset: 0x001B7E00
		public bool TryPlaceGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, List<BuilderAttachGridPlane> checkGridPlanes, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			Vector3 position = gridPlane.transform.position;
			Quaternion rotation = gridPlane.transform.rotation;
			if (this.gridSize <= 0f)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < checkGridPlanes.Count; i++)
			{
				BuilderAttachGridPlane checkGridPlane = checkGridPlanes[i];
				this.TryPlaceGridPlaneOnGridPlane(piece, gridPlane, position, rotation, checkGridPlane, ref potentialPlacement, ref result);
			}
			return result;
		}

		// Token: 0x0600573D RID: 22333 RVA: 0x001B9C74 File Offset: 0x001B7E74
		public bool TryPlaceGridPlaneOnGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, Vector3 gridPlanePos, Quaternion gridPlaneRot, BuilderAttachGridPlane checkGridPlane, ref BuilderPotentialPlacement potentialPlacement, ref bool success)
		{
			if (checkGridPlane.male == gridPlane.male)
			{
				return false;
			}
			if (checkGridPlane.piece == gridPlane.piece)
			{
				return false;
			}
			Transform center = checkGridPlane.center;
			Vector3 position = center.position;
			float sqrMagnitude = (position - gridPlanePos).sqrMagnitude;
			float num = checkGridPlane.boundingRadius + gridPlane.boundingRadius;
			if (sqrMagnitude > num * num)
			{
				return false;
			}
			Quaternion rotation = center.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Quaternion quaternion2 = quaternion * gridPlaneRot;
			if (Vector3.Dot(Vector3.up, quaternion2 * Vector3.up) < this.currSnapParams.maxUpDotProduct)
			{
				return false;
			}
			Vector3 vector = quaternion * (gridPlanePos - position);
			float y = vector.y;
			float num2 = -Mathf.Abs(y);
			if (success && num2 < potentialPlacement.score)
			{
				return false;
			}
			if (Mathf.Abs(y) > 1f)
			{
				return false;
			}
			if ((gridPlane.male && y > this.currSnapParams.minOffsetY) || (!gridPlane.male && y < -this.currSnapParams.minOffsetY))
			{
				return false;
			}
			if (Mathf.Abs(y) > this.currSnapParams.maxOffsetY)
			{
				return false;
			}
			Quaternion quaternion3;
			Quaternion quaternion4;
			BoingKit.QuaternionUtil.DecomposeSwingTwist(quaternion2, Vector3.up, out quaternion3, out quaternion4);
			float maxTwistDotProduct = this.currSnapParams.maxTwistDotProduct;
			Vector3 vector2 = quaternion4 * Vector3.forward;
			float num3 = Vector3.Dot(vector2, Vector3.forward);
			float num4 = Vector3.Dot(vector2, Vector3.right);
			bool flag = Mathf.Abs(num3) > maxTwistDotProduct;
			bool flag2 = Mathf.Abs(num4) > maxTwistDotProduct;
			if (!flag && !flag2)
			{
				return false;
			}
			float num5;
			uint num6;
			if (flag)
			{
				num5 = ((num3 > 0f) ? 0f : 180f);
				num6 = ((num3 > 0f) ? 0U : 2U);
			}
			else
			{
				num5 = ((num4 > 0f) ? 90f : 270f);
				num6 = ((num4 > 0f) ? 1U : 3U);
			}
			int num7 = flag2 ? gridPlane.width : gridPlane.length;
			int num8 = flag2 ? gridPlane.length : gridPlane.width;
			float num9 = (num8 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num10 = (num7 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num11 = (checkGridPlane.width % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num12 = (checkGridPlane.length % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num13 = num9 - num11;
			float num14 = num10 - num12;
			int num15 = Mathf.RoundToInt((vector.x - num13) / this.gridSize);
			int num16 = Mathf.RoundToInt((vector.z - num14) / this.gridSize);
			int num17 = num15 + Mathf.FloorToInt((float)num8 / 2f);
			int num18 = num16 + Mathf.FloorToInt((float)num7 / 2f);
			int num19 = num17 - (num8 - 1);
			int num20 = num18 - (num7 - 1);
			int num21 = Mathf.FloorToInt((float)checkGridPlane.width / 2f);
			int num22 = Mathf.FloorToInt((float)checkGridPlane.length / 2f);
			int num23 = num21 - (checkGridPlane.width - 1);
			int num24 = num22 - (checkGridPlane.length - 1);
			if (num19 > num21 || num17 < num23 || num20 > num22 || num18 < num24)
			{
				return false;
			}
			BuilderPiece rootPiece = checkGridPlane.piece.GetRootPiece();
			if (BuilderTable.ShareSameRoot(gridPlane.piece, rootPiece))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, gridPlane.piece, rootPiece))
			{
				return false;
			}
			BuilderPiece piece2 = checkGridPlane.piece;
			if (piece2 != null)
			{
				if (piece2.preventSnapUntilMoved > 0)
				{
					return false;
				}
				if (piece2.requestedParentPiece != null && BuilderTable.ShareSameRoot(piece, piece2.requestedParentPiece))
				{
					return false;
				}
			}
			Quaternion quaternion5 = Quaternion.Euler(0f, num5, 0f);
			Quaternion quaternion6 = rotation * quaternion5;
			float num25 = (float)num15 * this.gridSize + num13;
			float num26 = (float)num16 * this.gridSize + num14;
			Vector3 vector3;
			vector3..ctor(num25, 0f, num26);
			Vector3 vector4 = position + rotation * vector3;
			Transform center2 = gridPlane.center;
			Quaternion quaternion7 = quaternion6 * Quaternion.Inverse(center2.localRotation);
			Vector3 vector5 = piece.transform.InverseTransformPoint(center2.position);
			Vector3 localPosition = vector4 - quaternion7 * vector5;
			potentialPlacement.localPosition = localPosition;
			potentialPlacement.localRotation = quaternion7;
			potentialPlacement.score = num2;
			success = true;
			potentialPlacement.parentPiece = piece2;
			potentialPlacement.parentAttachIndex = checkGridPlane.attachIndex;
			potentialPlacement.attachDistance = Mathf.Abs(y);
			potentialPlacement.attachPlaneNormal = Vector3.up;
			if (!checkGridPlane.male)
			{
				potentialPlacement.attachPlaneNormal *= -1f;
			}
			if (potentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				potentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(potentialPlacement.localPosition);
				potentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * potentialPlacement.localRotation;
			}
			potentialPlacement.parentAttachBounds.min.x = Mathf.Max(num23, num19);
			potentialPlacement.parentAttachBounds.min.y = Mathf.Max(num24, num20);
			potentialPlacement.parentAttachBounds.max.x = Mathf.Min(num21, num17);
			potentialPlacement.parentAttachBounds.max.y = Mathf.Min(num22, num18);
			Vector2Int v = Vector2Int.zero;
			Vector2Int v2 = Vector2Int.zero;
			v.x = potentialPlacement.parentAttachBounds.min.x - num15;
			v2.x = potentialPlacement.parentAttachBounds.max.x - num15;
			v.y = potentialPlacement.parentAttachBounds.min.y - num16;
			v2.y = potentialPlacement.parentAttachBounds.max.y - num16;
			potentialPlacement.twist = (byte)num6;
			potentialPlacement.bumpOffsetX = (sbyte)num15;
			potentialPlacement.bumpOffsetZ = (sbyte)num16;
			int offsetX = (num8 % 2 == 0) ? 1 : 0;
			int offsetY = (num7 % 2 == 0) ? 1 : 0;
			if (flag && num3 < 0f)
			{
				v = this.Rotate180(v, offsetX, offsetY);
				v2 = this.Rotate180(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 < 0f)
			{
				v = this.Rotate270(v, offsetX, offsetY);
				v2 = this.Rotate270(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 > 0f)
			{
				v = this.Rotate90(v, offsetX, offsetY);
				v2 = this.Rotate90(v2, offsetX, offsetY);
			}
			potentialPlacement.attachBounds.min.x = Mathf.Min(v.x, v2.x);
			potentialPlacement.attachBounds.min.y = Mathf.Min(v.y, v2.y);
			potentialPlacement.attachBounds.max.x = Mathf.Max(v.x, v2.x);
			potentialPlacement.attachBounds.max.y = Mathf.Max(v.y, v2.y);
			return true;
		}

		// Token: 0x0600573E RID: 22334 RVA: 0x001BA3EE File Offset: 0x001B85EE
		private Vector2Int Rotate90(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y * -1 + offsetY, v.x);
		}

		// Token: 0x0600573F RID: 22335 RVA: 0x001BA407 File Offset: 0x001B8607
		private Vector2Int Rotate270(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y, v.x * -1 + offsetX);
		}

		// Token: 0x06005740 RID: 22336 RVA: 0x001BA420 File Offset: 0x001B8620
		private Vector2Int Rotate180(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.x * -1 + offsetX, v.y * -1 + offsetY);
		}

		// Token: 0x06005741 RID: 22337 RVA: 0x001BA43D File Offset: 0x001B863D
		public bool ShareSameRoot(BuilderAttachGridPlane plane, BuilderAttachGridPlane otherPlane)
		{
			return !(plane == null) && !(otherPlane == null) && !(otherPlane.piece == null) && BuilderTable.ShareSameRoot(plane.piece, otherPlane.piece);
		}

		// Token: 0x06005742 RID: 22338 RVA: 0x001BA474 File Offset: 0x001B8674
		public static bool ShareSameRoot(BuilderPiece piece, BuilderPiece otherPiece)
		{
			if (otherPiece == null || piece == null)
			{
				return false;
			}
			if (piece == otherPiece)
			{
				return true;
			}
			BuilderPiece builderPiece = piece;
			int num = 2048;
			while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
			{
				builderPiece = builderPiece.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			num = 2048;
			BuilderPiece builderPiece2 = otherPiece;
			while (builderPiece2.parentPiece != null && !builderPiece2.parentPiece.isBuiltIntoTable)
			{
				builderPiece2 = builderPiece2.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			return builderPiece == builderPiece2;
		}

		// Token: 0x06005743 RID: 22339 RVA: 0x001BA514 File Offset: 0x001B8714
		public bool TryPlacePieceOnTableNoDrop(bool leftHand, BuilderPiece testPiece, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			if (testPiece == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			return this.TryPlacePieceGridPlanesOnTableInternal(testPiece, this.maxPlacementChildDepth, checkGridPlanesMale, checkGridPlanesFemale, out potentialPlacement);
		}

		// Token: 0x06005744 RID: 22340 RVA: 0x001BA564 File Offset: 0x001B8764
		public bool TryPlacePieceOnTableNoDropJobs(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderPieceData> pieceData, NativeList<BuilderGridPlaneData> checkGridPlaneData, NativeList<BuilderPieceData> checkPieceData, out BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(3);
			IJobParallelForExtensions.Schedule<BuilderFindPotentialSnaps>(new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = Vector3.zero,
				worldToLocalRot = Quaternion.identity,
				localToWorldPos = Vector3.zero,
				localToWorldRot = Quaternion.identity,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}, gridPlaneData.Length, 32, default(JobHandle)).Complete();
			BuilderPotentialPlacementData builderPotentialPlacementData = default(BuilderPotentialPlacementData);
			bool flag = false;
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData2 = nativeQueue.Dequeue();
				if (!flag || builderPotentialPlacementData2.score > builderPotentialPlacementData.score)
				{
					builderPotentialPlacementData = builderPotentialPlacementData2;
					flag = true;
				}
			}
			if (flag)
			{
				potentialPlacement = builderPotentialPlacementData.ToPotentialPlacement(this);
			}
			if (flag)
			{
				nativeQueue.Clear();
				this.currSnapParams = this.overlapParams;
				Vector3 worldToLocalPos = -potentialPlacement.attachPiece.transform.position;
				Quaternion worldToLocalRot = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				Quaternion localToWorldRot = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
				Vector3 localToWorldPos = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
				IJobParallelForExtensions.Schedule<BuilderFindPotentialSnaps>(new BuilderFindPotentialSnaps
				{
					gridSize = this.gridSize,
					currSnapParams = this.currSnapParams,
					gridPlanes = gridPlaneData,
					checkGridPlanes = checkGridPlaneData,
					worldToLocalPos = worldToLocalPos,
					worldToLocalRot = worldToLocalRot,
					localToWorldPos = localToWorldPos,
					localToWorldRot = localToWorldRot,
					potentialPlacements = nativeQueue.AsParallelWriter()
				}, gridPlaneData.Length, 32, default(JobHandle)).Complete();
				while (!nativeQueue.IsEmpty())
				{
					BuilderPotentialPlacementData builderPotentialPlacementData3 = nativeQueue.Dequeue();
					if (builderPotentialPlacementData3.attachDistance < this.currSnapParams.maxBlockSnapDist)
					{
						allPlacements.Add(builderPotentialPlacementData3.ToPotentialPlacement(this));
					}
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06005745 RID: 22341 RVA: 0x001BA7D0 File Offset: 0x001B89D0
		public bool CalcAllPotentialPlacements(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderGridPlaneData> checkGridPlaneData, BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			if (this == null)
			{
				return false;
			}
			bool result = false;
			this.currSnapParams = this.overlapParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(3);
			nativeQueue.Clear();
			Vector3 worldToLocalPos = -potentialPlacement.attachPiece.transform.position;
			Quaternion worldToLocalRot = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
			BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
			Quaternion localToWorldRot = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
			Vector3 localToWorldPos = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
			IJobParallelForExtensions.Schedule<BuilderFindPotentialSnaps>(new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = worldToLocalPos,
				worldToLocalRot = worldToLocalRot,
				localToWorldPos = localToWorldPos,
				localToWorldRot = localToWorldRot,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}, gridPlaneData.Length, 32, default(JobHandle)).Complete();
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData = nativeQueue.Dequeue();
				if (builderPotentialPlacementData.attachDistance < this.currSnapParams.maxBlockSnapDist)
				{
					allPlacements.Add(builderPotentialPlacementData.ToPotentialPlacement(this));
				}
			}
			nativeQueue.Dispose();
			return result;
		}

		// Token: 0x06005746 RID: 22342 RVA: 0x001BA93C File Offset: 0x001B8B3C
		public bool CanPiecesPotentiallySnap(BuilderPiece pieceInHand, BuilderPiece piece)
		{
			BuilderPiece rootPiece = piece.GetRootPiece();
			return !(rootPiece == pieceInHand) && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece) && (!(piece.requestedParentPiece != null) || !BuilderTable.ShareSameRoot(pieceInHand, piece.requestedParentPiece)) && piece.preventSnapUntilMoved <= 0;
		}

		// Token: 0x06005747 RID: 22343 RVA: 0x001BA9A0 File Offset: 0x001B8BA0
		public bool CanPiecesPotentiallyOverlap(BuilderPiece pieceInHand, BuilderPiece rootWhenPlaced, BuilderPiece.State stateWhenPlaced, BuilderPiece otherPiece)
		{
			BuilderPiece rootPiece = otherPiece.GetRootPiece();
			if (rootPiece == pieceInHand)
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece))
			{
				return false;
			}
			if (otherPiece.requestedParentPiece != null && BuilderTable.ShareSameRoot(pieceInHand, otherPiece.requestedParentPiece))
			{
				return false;
			}
			if (otherPiece.preventSnapUntilMoved > 0)
			{
				return false;
			}
			BuilderPiece.State stateB = otherPiece.state;
			if (otherPiece.isBuiltIntoTable && !otherPiece.isArmShelf)
			{
				stateB = BuilderPiece.State.AttachedAndPlaced;
			}
			return BuilderTable.AreStatesCompatibleForOverlap(stateWhenPlaced, stateB, rootWhenPlaced, rootPiece);
		}

		// Token: 0x06005748 RID: 22344 RVA: 0x001BAA29 File Offset: 0x001B8C29
		public void TryDropPiece(bool leftHand, BuilderPiece testPiece, Vector3 velocity, Vector3 angVelocity)
		{
			if (this == null)
			{
				return;
			}
			if (testPiece == null)
			{
				return;
			}
			this.RequestDropPiece(testPiece, testPiece.transform.position, testPiece.transform.rotation, velocity, angVelocity);
		}

		// Token: 0x06005749 RID: 22345 RVA: 0x001BAA60 File Offset: 0x001B8C60
		public bool TryPlacePieceGridPlanesOnTableInternal(BuilderPiece testPiece, int recurse, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			bool result = false;
			bool flag = false;
			if (testPiece != null && testPiece.gridPlanes != null && testPiece.gridPlanes.Count > 0 && testPiece.gridPlanes != null)
			{
				for (int i = 0; i < testPiece.gridPlanes.Count; i++)
				{
					List<BuilderAttachGridPlane> checkGridPlanes = testPiece.gridPlanes[i].male ? checkGridPlanesFemale : checkGridPlanesMale;
					BuilderPotentialPlacement builderPotentialPlacement;
					if (this.TryPlaceGridPlane(testPiece, testPiece.gridPlanes[i], checkGridPlanes, out builderPotentialPlacement))
					{
						if (builderPotentialPlacement.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag = true;
						}
						if (builderPotentialPlacement.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement;
							potentialPlacement.attachIndex = i;
							potentialPlacement.attachPiece = testPiece;
							result = true;
						}
					}
				}
			}
			if (recurse > 0)
			{
				BuilderPiece builderPiece = testPiece.firstChildPiece;
				while (builderPiece != null)
				{
					BuilderPotentialPlacement builderPotentialPlacement2;
					if (this.TryPlacePieceGridPlanesOnTableInternal(builderPiece, recurse - 1, checkGridPlanesMale, checkGridPlanesFemale, out builderPotentialPlacement2))
					{
						if (builderPotentialPlacement2.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag = true;
						}
						if (builderPotentialPlacement2.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement2;
							result = true;
						}
					}
					builderPiece = builderPiece.nextSiblingPiece;
				}
			}
			if (testPiece.preventSnapUntilMoved > 0 && !flag)
			{
				testPiece.preventSnapUntilMoved--;
				this.UpdatePieceData(testPiece);
			}
			return result;
		}

		// Token: 0x0600574A RID: 22346 RVA: 0x001BABE4 File Offset: 0x001B8DE4
		public void TryPlaceRandomlyOnTable(BuilderPiece piece)
		{
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[Random.Range(0, piece.gridPlanes.Count)];
			List<BuilderAttachGridPlane> list = this.baseGridPlanes;
			int num = Random.Range(0, list.Count);
			int i = 0;
			while (i < list.Count)
			{
				int num2 = (i + num) % list.Count;
				BuilderAttachGridPlane builderAttachGridPlane2 = list[num2];
				if (builderAttachGridPlane2.male != builderAttachGridPlane.male && !(builderAttachGridPlane2.piece == builderAttachGridPlane.piece) && !this.ShareSameRoot(builderAttachGridPlane, builderAttachGridPlane2))
				{
					Vector3 zero = Vector3.zero;
					Quaternion identity = Quaternion.identity;
					BuilderPiece piece2 = builderAttachGridPlane2.piece;
					int attachIndex = builderAttachGridPlane2.attachIndex;
					Transform center = builderAttachGridPlane.center;
					Quaternion quaternion = builderAttachGridPlane2.transform.rotation * Quaternion.Inverse(center.localRotation);
					Vector3 vector = piece.transform.InverseTransformPoint(center.position);
					Vector3 vector2 = builderAttachGridPlane2.transform.position - quaternion * vector;
					if (piece2 != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = piece2.gridPlanes[attachIndex];
						Vector3 lossyScale = builderAttachGridPlane3.transform.lossyScale;
						Vector3 vector3;
						vector3..ctor(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * Vector3.Scale(vector2 - builderAttachGridPlane3.transform.position, vector3);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * quaternion;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x0600574B RID: 22347 RVA: 0x001BADA0 File Offset: 0x001B8FA0
		public void UseResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.UseResource(cost.quantities[i]);
			}
		}

		// Token: 0x0600574C RID: 22348 RVA: 0x001BADE6 File Offset: 0x001B8FE6
		private void UseResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] += quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x0600574D RID: 22349 RVA: 0x001BAE28 File Offset: 0x001B9028
		public void AddResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.AddResource(cost.quantities[i]);
			}
		}

		// Token: 0x0600574E RID: 22350 RVA: 0x001BAE6E File Offset: 0x001B906E
		private void AddResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] -= quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x0600574F RID: 22351 RVA: 0x001BAEB0 File Offset: 0x001B90B0
		public bool HasEnoughUnreservedResources(BuilderResources resources)
		{
			if (resources == null)
			{
				return false;
			}
			for (int i = 0; i < resources.quantities.Count; i++)
			{
				if (!this.HasEnoughUnreservedResource(resources.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005750 RID: 22352 RVA: 0x001BAEF8 File Offset: 0x001B90F8
		public bool HasEnoughUnreservedResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + this.reservedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06005751 RID: 22353 RVA: 0x001BAF50 File Offset: 0x001B9150
		public bool HasEnoughResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return false;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				if (!this.HasEnoughResource(cost.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005752 RID: 22354 RVA: 0x001BAF9C File Offset: 0x001B919C
		public bool HasEnoughResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06005753 RID: 22355 RVA: 0x001BAFD8 File Offset: 0x001B91D8
		public int GetAvailableResources(BuilderResourceType type)
		{
			if (type < BuilderResourceType.Basic || type >= BuilderResourceType.Count)
			{
				return 0;
			}
			return this.maxResources[(int)type] - this.usedResources[(int)type];
		}

		// Token: 0x06005754 RID: 22356 RVA: 0x001BAFF8 File Offset: 0x001B91F8
		private void OnAvailableResourcesChange()
		{
			if (this.isSetup && this.isTableMutable)
			{
				for (int i = 0; i < this.conveyors.Count; i++)
				{
					this.conveyors[i].OnAvailableResourcesChange();
				}
				foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
				{
					builderResourceMeter.OnAvailableResourcesChange();
				}
			}
		}

		// Token: 0x06005755 RID: 22357 RVA: 0x001BB080 File Offset: 0x001B9280
		public int GetPrivateResourceLimitForType(int type)
		{
			if (this.plotMaxResources == null)
			{
				return 0;
			}
			return this.plotMaxResources[type];
		}

		// Token: 0x06005756 RID: 22358 RVA: 0x001BB094 File Offset: 0x001B9294
		private void WriteVector3(BinaryWriter writer, Vector3 data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
		}

		// Token: 0x06005757 RID: 22359 RVA: 0x001BB0BA File Offset: 0x001B92BA
		private void WriteQuaternion(BinaryWriter writer, Quaternion data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
			writer.Write(data.w);
		}

		// Token: 0x06005758 RID: 22360 RVA: 0x001BB0EC File Offset: 0x001B92EC
		private Vector3 ReadVector3(BinaryReader reader)
		{
			Vector3 result;
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			return result;
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x001BB124 File Offset: 0x001B9324
		private Quaternion ReadQuaternion(BinaryReader reader)
		{
			Quaternion result;
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			result.w = reader.ReadSingle();
			return result;
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x001BB168 File Offset: 0x001B9368
		public static int PackPiecePlacement(byte twist, sbyte xOffset, sbyte zOffset)
		{
			int num = (int)(twist & 3);
			int num2 = (int)xOffset + 128;
			int num3 = (int)zOffset + 128;
			return num2 + (num3 << 8) + (num << 16);
		}

		// Token: 0x0600575B RID: 22363 RVA: 0x001BB194 File Offset: 0x001B9394
		public static void UnpackPiecePlacement(int packed, out byte twist, out sbyte xOffset, out sbyte zOffset)
		{
			int num = packed & 255;
			int num2 = packed >> 8 & 255;
			int num3 = packed >> 16 & 3;
			twist = (byte)num3;
			xOffset = (sbyte)(num - 128);
			zOffset = (sbyte)(num2 - 128);
		}

		// Token: 0x0600575C RID: 22364 RVA: 0x001BB1D4 File Offset: 0x001B93D4
		private long PackSnapInfo(int attachGridIndex, int otherAttachGridIndex, Vector2Int min, Vector2Int max)
		{
			long num = (long)Mathf.Clamp(attachGridIndex, 0, 31);
			long num2 = (long)Mathf.Clamp(otherAttachGridIndex, 0, 31);
			long num3 = (long)Mathf.Clamp(min.x + 1024, 0, 2047);
			long num4 = (long)Mathf.Clamp(min.y + 1024, 0, 2047);
			long num5 = (long)Mathf.Clamp(max.x + 1024, 0, 2047);
			long num6 = (long)Mathf.Clamp(max.y + 1024, 0, 2047);
			return num + (num2 << 5) + (num3 << 10) + (num4 << 21) + (num5 << 32) + (num6 << 43);
		}

		// Token: 0x0600575D RID: 22365 RVA: 0x001BB278 File Offset: 0x001B9478
		private void UnpackSnapInfo(long packed, out int attachGridIndex, out int otherAttachGridIndex, out Vector2Int min, out Vector2Int max)
		{
			long num = packed & 31L;
			attachGridIndex = (int)num;
			num = (packed >> 5 & 31L);
			otherAttachGridIndex = (int)num;
			int num2 = (int)(packed >> 10 & 2047L) - 1024;
			int num3 = (int)(packed >> 21 & 2047L) - 1024;
			min = new Vector2Int(num2, num3);
			int num4 = (int)(packed >> 32 & 2047L) - 1024;
			int num5 = (int)(packed >> 43 & 2047L) - 1024;
			max = new Vector2Int(num4, num5);
		}

		// Token: 0x0600575E RID: 22366 RVA: 0x001BB305 File Offset: 0x001B9505
		private void OnTitleDataUpdate(string key)
		{
			if (key.Equals(this.SharedMapConfigTitleDataKey))
			{
				this.FetchSharedBlocksStartingMapConfig();
			}
		}

		// Token: 0x0600575F RID: 22367 RVA: 0x001BB31B File Offset: 0x001B951B
		private void FetchSharedBlocksStartingMapConfig()
		{
			if (!this.isTableMutable)
			{
				PlayFabTitleDataCache.Instance.GetTitleData(this.SharedMapConfigTitleDataKey, new Action<string>(this.OnGetStartingMapConfigSuccess), new Action<PlayFabError>(this.OnGetStartingMapConfigFail), false);
			}
		}

		// Token: 0x06005760 RID: 22368 RVA: 0x001BB350 File Offset: 0x001B9550
		private void OnGetStartingMapConfigSuccess(string result)
		{
			this.ResetStartingMapConfig();
			if (result.IsNullOrEmpty())
			{
				return;
			}
			try
			{
				SharedBlocksManager.StartingMapConfig startingMapConfig = JsonUtility.FromJson<SharedBlocksManager.StartingMapConfig>(result);
				if (startingMapConfig.useMapID)
				{
					if (SharedBlocksManager.IsMapIDValid(startingMapConfig.mapID))
					{
						this.startingMapConfig.useMapID = true;
						this.startingMapConfig.mapID = startingMapConfig.mapID;
					}
					else
					{
						GTDev.LogError<string>(string.Format("BuilderTable {0} OnGetStartingMapConfigSuccess Title Data Default Map Config has Invalid Map ID", this.tableZone), null);
					}
				}
				else
				{
					this.startingMapConfig.pageNumber = Mathf.Max(startingMapConfig.pageNumber, 0);
					this.startingMapConfig.pageSize = Mathf.Max(startingMapConfig.pageSize, 1);
					if (!startingMapConfig.sortMethod.IsNullOrEmpty() && (startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.Top.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.NewlyCreated.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.RecentlyUpdated.ToString())))
					{
						this.startingMapConfig.sortMethod = startingMapConfig.sortMethod;
					}
					else
					{
						GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Unknown sort method " + startingMapConfig.sortMethod, null);
					}
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Exception Deserializing " + ex.Message, null);
			}
		}

		// Token: 0x06005761 RID: 22369 RVA: 0x001BB4E8 File Offset: 0x001B96E8
		private void OnGetStartingMapConfigFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigFail " + error.Error.ToString(), null);
			this.ResetStartingMapConfig();
		}

		// Token: 0x06005762 RID: 22370 RVA: 0x001BB528 File Offset: 0x001B9728
		private void ResetStartingMapConfig()
		{
			this.startingMapConfig = new SharedBlocksManager.StartingMapConfig
			{
				pageNumber = 0,
				pageSize = 10,
				sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
				useMapID = false,
				mapID = null
			};
		}

		// Token: 0x06005763 RID: 22371 RVA: 0x001BB57B File Offset: 0x001B977B
		private void RequestTableConfiguration()
		{
			SharedBlocksManager.instance.OnGetTableConfiguration += new Action<string>(this.OnGetTableConfiguration);
			SharedBlocksManager.instance.RequestTableConfiguration();
		}

		// Token: 0x06005764 RID: 22372 RVA: 0x001BB59D File Offset: 0x001B979D
		private void OnGetTableConfiguration(string configString)
		{
			SharedBlocksManager.instance.OnGetTableConfiguration -= new Action<string>(this.OnGetTableConfiguration);
			if (!configString.IsNullOrEmpty())
			{
				this.ParseTableConfiguration(configString);
			}
		}

		// Token: 0x06005765 RID: 22373 RVA: 0x001BB5C4 File Offset: 0x001B97C4
		private void ParseTableConfiguration(string dataRecord)
		{
			if (string.IsNullOrEmpty(dataRecord))
			{
				return;
			}
			BuilderTableConfiguration builderTableConfiguration = JsonUtility.FromJson<BuilderTableConfiguration>(dataRecord);
			if (builderTableConfiguration != null)
			{
				if (builderTableConfiguration.TableResourceLimits != null)
				{
					for (int i = 0; i < builderTableConfiguration.TableResourceLimits.Length; i++)
					{
						int num = builderTableConfiguration.TableResourceLimits[i];
						if (num >= 0)
						{
							this.maxResources[i] = num;
						}
					}
				}
				if (builderTableConfiguration.PlotResourceLimits != null)
				{
					for (int j = 0; j < builderTableConfiguration.PlotResourceLimits.Length; j++)
					{
						int num2 = builderTableConfiguration.PlotResourceLimits[j];
						if (num2 >= 0)
						{
							this.plotMaxResources[j] = num2;
						}
					}
				}
				int droppedPieceLimit = builderTableConfiguration.DroppedPieceLimit;
				if (droppedPieceLimit >= 0)
				{
					BuilderTable.DROPPED_PIECE_LIMIT = droppedPieceLimit;
				}
				if (builderTableConfiguration.updateCountdownDate != null && !string.IsNullOrEmpty(builderTableConfiguration.updateCountdownDate))
				{
					try
					{
						DateTime.Parse(builderTableConfiguration.updateCountdownDate, CultureInfo.InvariantCulture);
						BuilderTable.nextUpdateOverride = builderTableConfiguration.updateCountdownDate;
						goto IL_DC;
					}
					catch
					{
						BuilderTable.nextUpdateOverride = string.Empty;
						goto IL_DC;
					}
				}
				BuilderTable.nextUpdateOverride = string.Empty;
				IL_DC:
				this.OnAvailableResourcesChange();
				UnityEvent onTableConfigurationUpdated = this.OnTableConfigurationUpdated;
				if (onTableConfigurationUpdated == null)
				{
					return;
				}
				onTableConfigurationUpdated.Invoke();
			}
		}

		// Token: 0x06005766 RID: 22374 RVA: 0x001BB6D4 File Offset: 0x001B98D4
		private void DumpTableConfig()
		{
			BuilderTableConfiguration builderTableConfiguration = new BuilderTableConfiguration();
			Array.Clear(builderTableConfiguration.TableResourceLimits, 0, builderTableConfiguration.TableResourceLimits.Length);
			Array.Clear(builderTableConfiguration.PlotResourceLimits, 0, builderTableConfiguration.PlotResourceLimits.Length);
			foreach (BuilderResourceQuantity builderResourceQuantity in this.totalResources.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < (BuilderResourceType)builderTableConfiguration.TableResourceLimits.Length)
				{
					builderTableConfiguration.TableResourceLimits[(int)builderResourceQuantity.type] = builderResourceQuantity.count;
				}
			}
			foreach (BuilderResourceQuantity builderResourceQuantity2 in this.resourcesPerPrivatePlot.quantities)
			{
				if (builderResourceQuantity2.type >= BuilderResourceType.Basic && builderResourceQuantity2.type < (BuilderResourceType)builderTableConfiguration.PlotResourceLimits.Length)
				{
					builderTableConfiguration.PlotResourceLimits[(int)builderResourceQuantity2.type] = builderResourceQuantity2.count;
				}
			}
			builderTableConfiguration.DroppedPieceLimit = BuilderTable.DROPPED_PIECE_LIMIT;
			builderTableConfiguration.updateCountdownDate = "1/10/2025 16:00:00";
			string text = JsonUtility.ToJson(builderTableConfiguration);
			Debug.Log("Configuration Dump \n" + text);
		}

		// Token: 0x06005767 RID: 22375 RVA: 0x001BB820 File Offset: 0x001B9A20
		private string GetSaveDataTimeKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2") + "Time";
		}

		// Token: 0x06005768 RID: 22376 RVA: 0x001BB83D File Offset: 0x001B9A3D
		private string GetSaveDataKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2");
		}

		// Token: 0x06005769 RID: 22377 RVA: 0x001BB855 File Offset: 0x001B9A55
		public void FindAndLoadSharedBlocksMap(string mapID)
		{
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundSharedBlocksMap));
		}

		// Token: 0x0600576A RID: 22378 RVA: 0x001BB86E File Offset: 0x001B9A6E
		public string GetSharedBlocksMapID()
		{
			if (this.sharedBlocksMap != null)
			{
				return this.sharedBlocksMap.MapID;
			}
			return string.Empty;
		}

		// Token: 0x0600576B RID: 22379 RVA: 0x001BB88C File Offset: 0x001B9A8C
		private void FoundSharedBlocksMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (map == null || map.MapData.IsNullOrEmpty())
			{
				this.builderNetworking.LoadSharedBlocksFailedMaster((map == null) ? string.Empty : map.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			this.sharedBlocksMap = map;
			this.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
		}

		// Token: 0x0600576C RID: 22380 RVA: 0x001BB908 File Offset: 0x001B9B08
		private void BuildInitialTableForPlayer()
		{
			if (NetworkSystem.Instance.IsNull() || !NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate || NetworkSystem.Instance.GetLocalPlayer() == null || !NetworkSystem.Instance.IsMasterClient)
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			SharedBlocksManager.instance.OnFetchPrivateScanComplete += new Action<int, bool>(this.OnFetchPrivateScanComplete);
			SharedBlocksManager.instance.RequestFetchPrivateScan(this.currentSaveSlot);
		}

		// Token: 0x0600576D RID: 22381 RVA: 0x001BB994 File Offset: 0x001B9B94
		private void OnFetchPrivateScanComplete(int slot, bool success)
		{
			SharedBlocksManager.instance.OnFetchPrivateScanComplete -= new Action<int, bool>(this.OnFetchPrivateScanComplete);
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			string tableJson;
			if (!success || !SharedBlocksManager.instance.TryGetPrivateScanResponse(slot, out tableJson))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!this.BuildTableFromJson(tableJson, false))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			this.SetIsDirty(false);
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x0600576E RID: 22382 RVA: 0x001BB9F8 File Offset: 0x001B9BF8
		private void BuildSelectedSharedMap()
		{
			if (!NetworkSystem.Instance.IsNull() && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				if (this.sharedBlocksMap != null && !this.sharedBlocksMap.MapData.IsNullOrEmpty())
				{
					this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
					return;
				}
				if (SharedBlocksManager.IsMapIDValid(this.pendingMapID))
				{
					SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = this.pendingMapID
					};
					this.LoadSharedMap(map);
					return;
				}
				this.FindStartingMap();
			}
		}

		// Token: 0x0600576F RID: 22383 RVA: 0x001BBA84 File Offset: 0x001B9C84
		private void FindStartingMap()
		{
			if (this.hasStartingMap && Time.timeAsDouble < this.startingMapCacheTime + 60.0)
			{
				this.FoundDefaultSharedBlocksMap(true, this.startingMap);
				return;
			}
			if (this.getStartingMapInProgress)
			{
				return;
			}
			this.hasStartingMap = false;
			this.getStartingMapInProgress = true;
			if (this.startingMapConfig.useMapID && SharedBlocksManager.IsMapIDValid(this.startingMapConfig.mapID))
			{
				this.startingMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = this.startingMapConfig.mapID
				};
				SharedBlocksManager.instance.RequestMapDataFromID(this.startingMapConfig.mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
				return;
			}
			if (this.hasCachedTopMaps && Time.timeAsDouble <= this.lastGetTopMapsTime + 60.0)
			{
				this.ChooseMapFromList();
				return;
			}
			SharedBlocksManager.instance.OnGetPopularMapsComplete += new Action<bool>(this.FoundStartingMapList);
			if (!SharedBlocksManager.instance.RequestGetTopMaps(this.startingMapConfig.pageNumber, this.startingMapConfig.pageSize, this.startingMapConfig.sortMethod.ToString()))
			{
				this.FoundStartingMapList(false);
			}
		}

		// Token: 0x06005770 RID: 22384 RVA: 0x001BBBA8 File Offset: 0x001B9DA8
		private void FoundStartingMapList(bool success)
		{
			SharedBlocksManager.instance.OnGetPopularMapsComplete -= new Action<bool>(this.FoundStartingMapList);
			if (success && SharedBlocksManager.instance.LatestPopularMaps.Count > 0)
			{
				this.startingMapList.Clear();
				this.startingMapList.AddRange(SharedBlocksManager.instance.LatestPopularMaps);
				this.hasCachedTopMaps = (this.startingMapList.Count > 0);
				this.lastGetTopMapsTime = (double)Time.time;
				this.ChooseMapFromList();
				return;
			}
			this.FoundDefaultSharedBlocksMap(false, null);
		}

		// Token: 0x06005771 RID: 22385 RVA: 0x001BBC30 File Offset: 0x001B9E30
		private void ChooseMapFromList()
		{
			int num = Random.Range(0, this.startingMapList.Count);
			this.startingMap = this.startingMapList[num];
			if (this.startingMap == null || !SharedBlocksManager.IsMapIDValid(this.startingMap.MapID))
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			SharedBlocksManager.instance.RequestMapDataFromID(this.startingMap.MapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
		}

		// Token: 0x06005772 RID: 22386 RVA: 0x001BBCA8 File Offset: 0x001B9EA8
		private void FoundTopMapData(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null || !SharedBlocksManager.IsMapIDValid(map.MapID) || map.MapID != this.startingMap.MapID)
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			this.hasStartingMap = true;
			this.startingMapCacheTime = Time.timeAsDouble;
			this.startingMap.MapData = map.MapData;
			this.FoundDefaultSharedBlocksMap(true, this.startingMap);
		}

		// Token: 0x06005773 RID: 22387 RVA: 0x001BBD18 File Offset: 0x001B9F18
		private void FoundDefaultSharedBlocksMap(bool success, SharedBlocksManager.SharedBlocksMap map)
		{
			this.getStartingMapInProgress = false;
			if (success && !map.MapData.IsNullOrEmpty())
			{
				this.startingMapCacheTime = Time.timeAsDouble;
				this.startingMap = map;
				this.hasStartingMap = true;
				this.sharedBlocksMap = map;
				this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
				return;
			}
			this.TryBuildingFromTitleData();
		}

		// Token: 0x06005774 RID: 22388 RVA: 0x001BBD74 File Offset: 0x001B9F74
		private void TryBuildingSharedBlocksMap(string mapData)
		{
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!this.BuildTableFromJson(mapData, true))
			{
				GTDev.LogWarning<string>("Unable to build shared blocks map", null);
				this.builderNetworking.LoadSharedBlocksFailedMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			base.StartCoroutine(this.CheckForNoBlocks());
		}

		// Token: 0x06005775 RID: 22389 RVA: 0x001BBDE9 File Offset: 0x001B9FE9
		private IEnumerator CheckForNoBlocks()
		{
			yield return null;
			if (!this.NoBlocksCheck())
			{
				GTDev.LogError<string>("Failed No Blocks Check", null);
				this.builderNetworking.SharedBlocksOutOfBoundsMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				yield break;
			}
			this.OnFinishedInitialTableBuild();
			yield break;
		}

		// Token: 0x06005776 RID: 22390 RVA: 0x001BBDF8 File Offset: 0x001B9FF8
		private void TryBuildingFromTitleData()
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete += new Action<string>(this.OnGetTitleDataBuildComplete);
			SharedBlocksManager.instance.FetchTitleDataBuild();
		}

		// Token: 0x06005777 RID: 22391 RVA: 0x001BBE1C File Offset: 0x001BA01C
		private void OnGetTitleDataBuildComplete(string titleDataBuild)
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete -= new Action<string>(this.OnGetTitleDataBuildComplete);
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!titleDataBuild.IsNullOrEmpty())
			{
				if (!this.BuildTableFromJson(titleDataBuild, true))
				{
					this.tableData = new BuilderTableData();
				}
			}
			else
			{
				this.tableData = new BuilderTableData();
			}
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x06005778 RID: 22392 RVA: 0x001BBE7C File Offset: 0x001BA07C
		public void SaveTableForPlayer(string busyStr, string blocksErrStr)
		{
			if (SharedBlocksManager.instance.IsWaitingOnRequest())
			{
				this.SetIsDirty(true);
				UnityEvent<string> onSaveFailure = this.OnSaveFailure;
				if (onSaveFailure == null)
				{
					return;
				}
				onSaveFailure.Invoke(busyStr);
				return;
			}
			else
			{
				this.saveInProgress = true;
				if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
				{
					this.saveInProgress = false;
					return;
				}
				if (!this.isDirty)
				{
					this.saveInProgress = false;
					UnityEvent onSaveTimeUpdated = this.OnSaveTimeUpdated;
					if (onSaveTimeUpdated == null)
					{
						return;
					}
					onSaveTimeUpdated.Invoke();
					return;
				}
				else
				{
					if (this.NoBlocksCheck())
					{
						if (this.tableData == null)
						{
							this.tableData = new BuilderTableData();
						}
						this.SetIsDirty(false);
						this.tableData.numEdits++;
						string text = this.WriteTableToJson();
						text = Convert.ToBase64String(GZipStream.CompressString(text));
						SharedBlocksManager.instance.OnSavePrivateScanSuccess += new Action<int>(this.OnSaveScanSuccess);
						SharedBlocksManager.instance.OnSavePrivateScanFailed += new Action<int, string>(this.OnSaveScanFailure);
						SharedBlocksManager.instance.RequestSavePrivateScan(this.currentSaveSlot, text);
						return;
					}
					this.saveInProgress = false;
					this.SetIsDirty(true);
					UnityEvent<string> onSaveFailure2 = this.OnSaveFailure;
					if (onSaveFailure2 == null)
					{
						return;
					}
					onSaveFailure2.Invoke(blocksErrStr);
					return;
				}
			}
		}

		// Token: 0x06005779 RID: 22393 RVA: 0x001BBF90 File Offset: 0x001BA190
		private void OnSaveScanSuccess(int scan)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= new Action<int>(this.OnSaveScanSuccess);
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= new Action<int, string>(this.OnSaveScanFailure);
			this.saveInProgress = false;
			UnityEvent onSaveSuccess = this.OnSaveSuccess;
			if (onSaveSuccess == null)
			{
				return;
			}
			onSaveSuccess.Invoke();
		}

		// Token: 0x0600577A RID: 22394 RVA: 0x001BBFE0 File Offset: 0x001BA1E0
		private void OnSaveScanFailure(int scan, string message)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= new Action<int>(this.OnSaveScanSuccess);
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= new Action<int, string>(this.OnSaveScanFailure);
			this.saveInProgress = false;
			this.SetIsDirty(true);
			UnityEvent<string> onSaveFailure = this.OnSaveFailure;
			if (onSaveFailure == null)
			{
				return;
			}
			onSaveFailure.Invoke(message);
		}

		// Token: 0x0600577B RID: 22395 RVA: 0x001BC038 File Offset: 0x001BA238
		private string WriteTableToJson()
		{
			this.tableData.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.tableData.pieceType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedPieceType : this.pieces[i].pieceType);
					this.tableData.pieceId.Add(this.pieces[i].pieceId);
					this.tableData.parentId.Add((this.pieces[i].parentPiece == null) ? -1 : this.pieces[i].parentPiece.pieceId);
					this.tableData.attachIndex.Add(this.pieces[i].attachIndex);
					this.tableData.parentAttachIndex.Add((this.pieces[i].parentPiece == null) ? -1 : this.pieces[i].parentAttachIndex);
					this.tableData.placement.Add(this.pieces[i].GetPiecePlacement());
					this.tableData.materialType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedMaterialType : this.pieces[i].materialType);
					BuilderMovingSnapPiece component = this.pieces[i].GetComponent<BuilderMovingSnapPiece>();
					int num = (component == null) ? 0 : component.GetTimeOffset();
					this.tableData.timeOffset.Add(num);
					for (int j = 0; j < this.pieces[i].gridPlanes.Count; j++)
					{
						if (!(this.pieces[i].gridPlanes[j] == null))
						{
							for (SnapOverlap snapOverlap = this.pieces[i].gridPlanes[j].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								if (snapOverlap.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(this.pieces[i].pieceId, snapOverlap.otherPlane.piece.pieceId, j, snapOverlap.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
										long num2 = this.PackSnapInfo(j, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
										this.tableData.overlapingPieces.Add(this.pieces[i].pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(num2);
									}
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				if (!(builderPiece == null))
				{
					for (int k = 0; k < builderPiece.gridPlanes.Count; k++)
					{
						if (!(builderPiece.gridPlanes[k] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece.gridPlanes[k].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								if (snapOverlap2.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap2.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey2 = BuilderTable.BuildOverlapKey(builderPiece.pieceId, snapOverlap2.otherPlane.piece.pieceId, k, snapOverlap2.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey2))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey2);
										long num3 = this.PackSnapInfo(k, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
										this.tableData.overlapingPieces.Add(builderPiece.pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(num3);
									}
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			this.tableData.numPieces = this.tableData.pieceType.Count;
			return JsonUtility.ToJson(this.tableData);
		}

		// Token: 0x0600577C RID: 22396 RVA: 0x001BC58C File Offset: 0x001BA78C
		private static BuilderTable.SnapOverlapKey BuildOverlapKey(int pieceId, int otherPieceId, int attachGridIndex, int otherAttachGridIndex)
		{
			BuilderTable.SnapOverlapKey result = default(BuilderTable.SnapOverlapKey);
			result.piece = (long)pieceId;
			result.piece <<= 32;
			result.piece |= (long)attachGridIndex;
			result.otherPiece = (long)otherPieceId;
			result.otherPiece <<= 32;
			result.otherPiece |= (long)otherAttachGridIndex;
			return result;
		}

		// Token: 0x0600577D RID: 22397 RVA: 0x001BC5E8 File Offset: 0x001BA7E8
		private bool BuildTableFromJson(string tableJson, bool fromTitleData)
		{
			if (string.IsNullOrEmpty(tableJson))
			{
				return false;
			}
			this.tableData = null;
			try
			{
				this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
			}
			catch
			{
			}
			try
			{
				if (this.tableData == null)
				{
					tableJson = GZipStream.UncompressString(Convert.FromBase64String(tableJson));
					this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
			if (this.tableData == null)
			{
				return false;
			}
			if (this.tableData.version < 4)
			{
				return false;
			}
			int num = (this.tableData.pieceType == null) ? 0 : this.tableData.pieceType.Count;
			if (num == 0)
			{
				this.OnDeserializeUpdatePlots();
				return true;
			}
			if (this.tableData.pieceId == null || this.tableData.pieceId.Count != num || this.tableData.placement == null || this.tableData.placement.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch", null);
				return false;
			}
			if (num >= this.maxResources[0])
			{
				GTDev.LogError<string>(string.Format("BuildTableFromJson Failed sanity piece count check {0}", num), null);
				return false;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>(num);
			bool flag = this.tableData.timeOffset != null && this.tableData.timeOffset.Count > 0;
			if (flag && this.tableData.timeOffset.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch (Time Offsets)", null);
				return false;
			}
			int i = 0;
			while (i < this.tableData.pieceType.Count)
			{
				int num2 = this.CreatePieceId();
				if (!dictionary.TryAdd(this.tableData.pieceId[i], num2))
				{
					GTDev.LogError<string>("BuildTableFromJson Piece id duplicate in save", null);
					this.ClearTable();
					return false;
				}
				int num3 = (this.tableData.materialType != null && this.tableData.materialType.Count > i) ? this.tableData.materialType[i] : -1;
				int newPieceType = this.tableData.pieceType[i];
				int num4 = num3;
				bool flag2 = true;
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.tableData.pieceType[i]);
				if (piecePrefab == null)
				{
					this.ClearTable();
					return false;
				}
				if (fromTitleData)
				{
					goto IL_2B2;
				}
				if (num4 == -1 && piecePrefab.materialOptions != null)
				{
					int num5;
					Material material;
					int num6;
					piecePrefab.materialOptions.GetDefaultMaterial(out num5, out material, out num6);
					num4 = num5;
				}
				flag2 = BuilderSetManager.instance.IsPieceOwnedLocally(this.tableData.pieceType[i], num4);
				if (!fromTitleData && !flag2)
				{
					if (!piecePrefab.fallbackInfo.materialSwapThisPrefab)
					{
						if (piecePrefab.fallbackInfo.prefab == null)
						{
							goto IL_3E0;
						}
						newPieceType = piecePrefab.fallbackInfo.prefab.name.GetStaticHash();
					}
					num4 = -1;
				}
				goto IL_2B2;
				IL_3E0:
				i++;
				continue;
				IL_2B2:
				if (piecePrefab.cost != null && piecePrefab.cost.quantities != null)
				{
					for (int j = 0; j < piecePrefab.cost.quantities.Count; j++)
					{
						BuilderResourceQuantity builderResourceQuantity = piecePrefab.cost.quantities[j];
						if (!this.HasEnoughResource(builderResourceQuantity))
						{
							if (builderResourceQuantity.type == BuilderResourceType.Basic)
							{
								this.ClearTable();
								GTDev.LogError<string>("BuildTableFromJson saved table uses too many basic resource", null);
								return false;
							}
							GTDev.LogWarning<string>("BuildTableFromJson saved table uses too many functional or decorative resource", null);
						}
					}
				}
				int num7 = flag ? this.tableData.timeOffset[i] : 0;
				BuilderPiece builderPiece = this.CreatePieceInternal(newPieceType, num2, Vector3.zero, Quaternion.identity, BuilderPiece.State.AttachedAndPlaced, num4, NetworkSystem.Instance.ServerTimestamp - num7, this);
				if (builderPiece == null)
				{
					this.ClearTable();
					GTDev.LogError<string>(string.Format("Piece Type {0} is not defined", this.tableData.pieceType[i]), null);
					return false;
				}
				if (!fromTitleData && !flag2)
				{
					builderPiece.overrideSavedPiece = true;
					builderPiece.savedPieceType = this.tableData.pieceType[i];
					builderPiece.savedMaterialType = num3;
				}
				goto IL_3E0;
			}
			for (int k = 0; k < this.tableData.pieceType.Count; k++)
			{
				int parentAttachIndex = (this.tableData.parentAttachIndex == null || this.tableData.parentAttachIndex.Count <= k) ? -1 : this.tableData.parentAttachIndex[k];
				int attachIndex = (this.tableData.attachIndex == null || this.tableData.attachIndex.Count <= k) ? -1 : this.tableData.attachIndex[k];
				int valueOrDefault = CollectionExtensions.GetValueOrDefault<int, int>(dictionary, this.tableData.pieceId[k], -1);
				int parentId = -1;
				int num8;
				if (dictionary.TryGetValue(this.tableData.parentId[k], ref num8))
				{
					parentId = num8;
				}
				else if (this.tableData.parentId[k] < 10000 && this.tableData.parentId[k] >= 5)
				{
					parentId = this.tableData.parentId[k];
				}
				this.AttachPieceInternal(valueOrDefault, attachIndex, parentId, parentAttachIndex, this.tableData.placement[k]);
			}
			foreach (BuilderPiece builderPiece2 in this.pieces)
			{
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece2.OnPlacementDeserialized();
				}
			}
			this.OnDeserializeUpdatePlots();
			BuilderTable.tempDuplicateOverlaps.Clear();
			if (this.tableData.overlapingPieces != null)
			{
				int num9 = 0;
				while (num9 < this.tableData.overlapingPieces.Count && num9 < this.tableData.overlappedPieces.Count && num9 < this.tableData.overlapInfo.Count)
				{
					int num10 = -1;
					int num11;
					if (dictionary.TryGetValue(this.tableData.overlapingPieces[num9], ref num11))
					{
						num10 = num11;
					}
					else if (this.tableData.overlapingPieces[num9] < 10000 && this.tableData.overlapingPieces[num9] >= 5)
					{
						num10 = this.tableData.overlapingPieces[num9];
					}
					int num12 = -1;
					int num13;
					if (dictionary.TryGetValue(this.tableData.overlappedPieces[num9], ref num13))
					{
						num12 = num13;
					}
					else if (this.tableData.overlappedPieces[num9] < 10000 && this.tableData.overlappedPieces[num9] >= 5)
					{
						num12 = this.tableData.overlappedPieces[num9];
					}
					if (num10 != -1 && num12 != -1)
					{
						long packed = this.tableData.overlapInfo[num9];
						BuilderPiece piece = this.GetPiece(num10);
						if (!(piece == null))
						{
							BuilderPiece piece2 = this.GetPiece(num12);
							if (!(piece2 == null))
							{
								int num14;
								int num15;
								Vector2Int min;
								Vector2Int max;
								this.UnpackSnapInfo(packed, out num14, out num15, out min, out max);
								if (num14 >= 0 && num14 < piece.gridPlanes.Count && num15 >= 0 && num15 < piece2.gridPlanes.Count)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(num10, num12, num14, num15);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
										piece.gridPlanes[num14].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num15], new SnapBounds(min, max)));
									}
								}
							}
						}
					}
					num9++;
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			return true;
		}

		// Token: 0x0600577E RID: 22398 RVA: 0x001BCDD0 File Offset: 0x001BAFD0
		public int SerializeTableState(byte[] bytes, int maxBytes)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.conveyors == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.conveyors.Count);
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					int selectedDisplayGroupID = builderConveyor.GetSelectedDisplayGroupID();
					binaryWriter.Write(selectedDisplayGroupID);
				}
			}
			if (this.dispenserShelves == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.dispenserShelves.Count);
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					int selectedDisplayGroupID2 = builderDispenserShelf.GetSelectedDisplayGroupID();
					binaryWriter.Write(selectedDisplayGroupID2);
				}
			}
			BuilderTable.childPieces.Clear();
			BuilderTable.rootPieces.Clear();
			ListExtensions.EnsureCapacity<BuilderPiece>(BuilderTable.childPieces, this.pieces.Count);
			ListExtensions.EnsureCapacity<BuilderPiece>(BuilderTable.rootPieces, this.pieces.Count);
			foreach (BuilderPiece builderPiece in this.pieces)
			{
				if (builderPiece.parentPiece == null)
				{
					BuilderTable.rootPieces.Add(builderPiece);
				}
				else
				{
					BuilderTable.childPieces.Add(builderPiece);
				}
			}
			binaryWriter.Write(BuilderTable.rootPieces.Count);
			for (int i = 0; i < BuilderTable.rootPieces.Count; i++)
			{
				BuilderPiece builderPiece2 = BuilderTable.rootPieces[i];
				binaryWriter.Write(builderPiece2.pieceType);
				binaryWriter.Write(builderPiece2.pieceId);
				binaryWriter.Write((byte)builderPiece2.state);
				if (builderPiece2.state == BuilderPiece.State.OnConveyor || builderPiece2.state == BuilderPiece.State.OnShelf || builderPiece2.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece2.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece2.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece2.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece2.materialType);
				long num = BitPackUtils.PackWorldPosForNetwork(builderPiece2.transform.localPosition);
				int num2 = BitPackUtils.PackQuaternionForNetwork(builderPiece2.transform.localRotation);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece2.functionalPieceState);
					binaryWriter.Write(builderPiece2.activatedTimeStamp);
				}
				if (builderPiece2.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece2));
				}
			}
			binaryWriter.Write(BuilderTable.childPieces.Count);
			for (int j = 0; j < BuilderTable.childPieces.Count; j++)
			{
				BuilderPiece builderPiece3 = BuilderTable.childPieces[j];
				binaryWriter.Write(builderPiece3.pieceType);
				binaryWriter.Write(builderPiece3.pieceId);
				int num3 = (builderPiece3.parentPiece == null) ? -1 : builderPiece3.parentPiece.pieceId;
				binaryWriter.Write(num3);
				binaryWriter.Write(builderPiece3.attachIndex);
				binaryWriter.Write(builderPiece3.parentAttachIndex);
				binaryWriter.Write((byte)builderPiece3.state);
				if (builderPiece3.state == BuilderPiece.State.OnConveyor || builderPiece3.state == BuilderPiece.State.OnShelf || builderPiece3.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece3.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece3.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece3.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece3.materialType);
				int piecePlacement = builderPiece3.GetPiecePlacement();
				binaryWriter.Write(piecePlacement);
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece3.functionalPieceState);
					binaryWriter.Write(builderPiece3.activatedTimeStamp);
				}
				if (builderPiece3.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece3));
				}
			}
			if (this.isTableMutable)
			{
				binaryWriter.Write(this.plotOwners.Count);
				using (Dictionary<int, int>.Enumerator enumerator4 = this.plotOwners.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<int, int> keyValuePair = enumerator4.Current;
						binaryWriter.Write(keyValuePair.Key);
						binaryWriter.Write(keyValuePair.Value);
					}
					goto IL_4F9;
				}
			}
			if (this.sharedBlocksMap == null || this.sharedBlocksMap.MapID == null || !SharedBlocksManager.IsMapIDValid(this.sharedBlocksMap.MapID))
			{
				for (int k = 0; k < BuilderTable.mapIDBuffer.Length; k++)
				{
					BuilderTable.mapIDBuffer[k] = 'a';
				}
			}
			else
			{
				for (int l = 0; l < BuilderTable.mapIDBuffer.Length; l++)
				{
					BuilderTable.mapIDBuffer[l] = this.sharedBlocksMap.MapID.get_Chars(l);
				}
			}
			binaryWriter.Write(BuilderTable.mapIDBuffer);
			IL_4F9:
			long position = memoryStream.Position;
			BuilderTable.overlapPieces.Clear();
			BuilderTable.overlapOtherPieces.Clear();
			BuilderTable.overlapPacked.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			foreach (BuilderPiece builderPiece4 in this.pieces)
			{
				if (!(builderPiece4 == null))
				{
					for (int m = 0; m < builderPiece4.gridPlanes.Count; m++)
					{
						if (!(builderPiece4.gridPlanes[m] == null))
						{
							for (SnapOverlap snapOverlap = builderPiece4.gridPlanes[m].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(builderPiece4.pieceId, snapOverlap.otherPlane.piece.pieceId, m, snapOverlap.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
								{
									BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
									long num4 = this.PackSnapInfo(m, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece4.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(num4);
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece5 in this.basePieces)
			{
				if (!(builderPiece5 == null))
				{
					for (int n = 0; n < builderPiece5.gridPlanes.Count; n++)
					{
						if (!(builderPiece5.gridPlanes[n] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece5.gridPlanes[n].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								BuilderTable.SnapOverlapKey snapOverlapKey2 = BuilderTable.BuildOverlapKey(builderPiece5.pieceId, snapOverlap2.otherPlane.piece.pieceId, n, snapOverlap2.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey2))
								{
									BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey2);
									long num5 = this.PackSnapInfo(n, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece5.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(num5);
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			binaryWriter.Write(BuilderTable.overlapPieces.Count);
			for (int num6 = 0; num6 < BuilderTable.overlapPieces.Count; num6++)
			{
				binaryWriter.Write(BuilderTable.overlapPieces[num6]);
				binaryWriter.Write(BuilderTable.overlapOtherPieces[num6]);
				binaryWriter.Write(BuilderTable.overlapPacked[num6]);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x0600577F RID: 22399 RVA: 0x001BD6B8 File Offset: 0x001BB8B8
		public void DeserializeTableState(byte[] bytes, int numBytes)
		{
			if (numBytes <= 0)
			{
				return;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentPeiceIds.Clear();
			BuilderTable.tempAttachIndexes.Clear();
			BuilderTable.tempParentAttachIndexes.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			BuilderTable.tempPiecePlacement.Clear();
			int num = binaryReader.ReadInt32();
			bool flag = this.conveyors != null;
			for (int i = 0; i < num; i++)
			{
				int selection = binaryReader.ReadInt32();
				if (flag && i < this.conveyors.Count)
				{
					this.conveyors[i].SetSelection(selection);
				}
			}
			int num2 = binaryReader.ReadInt32();
			bool flag2 = this.dispenserShelves != null;
			for (int j = 0; j < num2; j++)
			{
				int selection2 = binaryReader.ReadInt32();
				if (flag2 && j < this.dispenserShelves.Count)
				{
					this.dispenserShelves[j].SetSelection(selection2);
				}
			}
			int num3 = binaryReader.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				int newPieceType = binaryReader.ReadInt32();
				int num4 = binaryReader.ReadInt32();
				BuilderPiece.State state = (BuilderPiece.State)binaryReader.ReadByte();
				int num5 = binaryReader.ReadInt32();
				bool flag3 = binaryReader.ReadByte() > 0;
				int materialType = binaryReader.ReadInt32();
				long data = binaryReader.ReadInt64();
				int data2 = binaryReader.ReadInt32();
				Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
				Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data2);
				byte fState = (state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0;
				int activateTimeStamp = (state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0;
				int num6 = (state == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0;
				float num7 = 10000f;
				if (!vector.IsValid(num7) || !quaternion.IsValid() || !this.ValidateCreatePieceParams(newPieceType, num4, state, materialType))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num8 = -1;
				if (state == BuilderPiece.State.OnConveyor || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
				{
					num8 = num5;
					num5 = -1;
				}
				if (this.ValidateDeserializedRootPieceState(num4, state, num8, num5, vector, quaternion))
				{
					BuilderPiece builderPiece = this.CreatePieceInternal(newPieceType, num4, vector, quaternion, state, materialType, activateTimeStamp, this);
					BuilderTable.tempPeiceIds.Add(num4);
					BuilderTable.tempParentActorNumbers.Add(num5);
					BuilderTable.tempInLeftHand.Add(flag3);
					builderPiece.SetFunctionalPieceState(fState, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					if (num8 >= 0 && this.isTableMutable)
					{
						builderPiece.shelfOwner = num8;
						if (state == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor = this.conveyors[num8];
							float timeOffset = 0f;
							if (PhotonNetwork.ServerTimestamp > num6)
							{
								timeOffset = (PhotonNetwork.ServerTimestamp - num6) / 1000f;
							}
							builderConveyor.OnShelfPieceCreated(builderPiece, timeOffset);
						}
						else if (state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num8].OnShelfPieceCreated(builderPiece, false);
						}
					}
				}
			}
			for (int l = 0; l < BuilderTable.tempPeiceIds.Count; l++)
			{
				if (BuilderTable.tempParentActorNumbers[l] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[l], BuilderTable.tempParentActorNumbers[l], BuilderTable.tempInLeftHand[l]);
				}
			}
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			int num9 = binaryReader.ReadInt32();
			for (int m = 0; m < num9; m++)
			{
				int newPieceType2 = binaryReader.ReadInt32();
				int num10 = binaryReader.ReadInt32();
				int num11 = binaryReader.ReadInt32();
				int num12 = binaryReader.ReadInt32();
				int num13 = binaryReader.ReadInt32();
				BuilderPiece.State state2 = (BuilderPiece.State)binaryReader.ReadByte();
				int num14 = binaryReader.ReadInt32();
				bool flag4 = binaryReader.ReadByte() > 0;
				int materialType2 = binaryReader.ReadInt32();
				int num15 = binaryReader.ReadInt32();
				byte fState2 = (state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0;
				int activateTimeStamp2 = (state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0;
				int num16 = (state2 == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0;
				if (!this.ValidateCreatePieceParams(newPieceType2, num10, state2, materialType2))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num17 = -1;
				if (state2 == BuilderPiece.State.OnConveyor || state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
				{
					num17 = num14;
					num14 = -1;
				}
				if (this.ValidateDeserializedChildPieceState(num10, state2))
				{
					BuilderPiece builderPiece2 = this.CreatePieceInternal(newPieceType2, num10, this.roomCenter.position, Quaternion.identity, state2, materialType2, activateTimeStamp2, this);
					builderPiece2.SetFunctionalPieceState(fState2, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					BuilderTable.tempPeiceIds.Add(num10);
					BuilderTable.tempParentPeiceIds.Add(num11);
					BuilderTable.tempAttachIndexes.Add(num12);
					BuilderTable.tempParentAttachIndexes.Add(num13);
					BuilderTable.tempParentActorNumbers.Add(num14);
					BuilderTable.tempInLeftHand.Add(flag4);
					BuilderTable.tempPiecePlacement.Add(num15);
					if (num17 >= 0 && this.isTableMutable)
					{
						builderPiece2.shelfOwner = num17;
						if (state2 == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor2 = this.conveyors[num17];
							float timeOffset2 = 0f;
							if (PhotonNetwork.ServerTimestamp > num16)
							{
								timeOffset2 = (PhotonNetwork.ServerTimestamp - num16) / 1000f;
							}
							builderConveyor2.OnShelfPieceCreated(builderPiece2, timeOffset2);
						}
						else if (state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num17].OnShelfPieceCreated(builderPiece2, false);
						}
					}
				}
			}
			for (int n = 0; n < BuilderTable.tempPeiceIds.Count; n++)
			{
				if (!this.ValidateAttachPieceParams(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]))
				{
					this.RecyclePieceInternal(BuilderTable.tempPeiceIds[n], true, false, -1);
				}
				else
				{
					this.AttachPieceInternal(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]);
				}
			}
			for (int num18 = 0; num18 < BuilderTable.tempPeiceIds.Count; num18++)
			{
				if (BuilderTable.tempParentActorNumbers[num18] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[num18], BuilderTable.tempParentActorNumbers[num18], BuilderTable.tempInLeftHand[num18]);
				}
			}
			foreach (BuilderPiece builderPiece3 in this.pieces)
			{
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece3.OnPlacementDeserialized();
				}
			}
			if (this.isTableMutable)
			{
				this.plotOwners.Clear();
				this.doesLocalPlayerOwnPlot = false;
				int num19 = binaryReader.ReadInt32();
				for (int num20 = 0; num20 < num19; num20++)
				{
					int num21 = binaryReader.ReadInt32();
					int num22 = binaryReader.ReadInt32();
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (this.plotOwners.TryAdd(num21, num22) && this.GetPiece(num22).TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						builderPiecePrivatePlot.ClaimPlotForPlayerNumber(num21);
						if (num21 == PhotonNetwork.LocalPlayer.ActorNumber)
						{
							this.doesLocalPlayerOwnPlot = true;
						}
					}
				}
				UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
				if (onLocalPlayerClaimedPlot != null)
				{
					onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
				}
				this.OnDeserializeUpdatePlots();
			}
			else
			{
				BuilderTable.mapIDBuffer = binaryReader.ReadChars(BuilderTable.mapIDBuffer.Length);
				string mapID = new string(BuilderTable.mapIDBuffer);
				if (SharedBlocksManager.IsMapIDValid(mapID))
				{
					this.sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = mapID
					};
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			int num23 = binaryReader.ReadInt32();
			for (int num24 = 0; num24 < num23; num24++)
			{
				int pieceId = binaryReader.ReadInt32();
				int num25 = binaryReader.ReadInt32();
				long packed = binaryReader.ReadInt64();
				BuilderPiece piece = this.GetPiece(pieceId);
				if (!(piece == null))
				{
					BuilderPiece piece2 = this.GetPiece(num25);
					if (!(piece2 == null))
					{
						int num26;
						int num27;
						Vector2Int min;
						Vector2Int max;
						this.UnpackSnapInfo(packed, out num26, out num27, out min, out max);
						if (num26 >= 0 && num26 < piece.gridPlanes.Count && num27 >= 0 && num27 < piece2.gridPlanes.Count)
						{
							BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(pieceId, num25, num26, num27);
							if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
							{
								BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
								piece.gridPlanes[num26].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num27], new SnapBounds(min, max)));
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
		}

		// Token: 0x040063CD RID: 25549
		public const GTZone BUILDER_ZONE = GTZone.monkeBlocks;

		// Token: 0x040063CE RID: 25550
		private const int INITIAL_BUILTIN_PIECE_ID = 5;

		// Token: 0x040063CF RID: 25551
		private const int INITIAL_CREATED_PIECE_ID = 10000;

		// Token: 0x040063D0 RID: 25552
		public static float MAX_DROP_VELOCITY = 20f;

		// Token: 0x040063D1 RID: 25553
		public static float MAX_DROP_ANG_VELOCITY = 50f;

		// Token: 0x040063D2 RID: 25554
		private const float MAX_DISTANCE_FROM_CENTER = 217f;

		// Token: 0x040063D3 RID: 25555
		private const float MAX_LOCAL_MAGNITUDE = 80f;

		// Token: 0x040063D4 RID: 25556
		public const float MAX_DISTANCE_FROM_HAND = 2.5f;

		// Token: 0x040063D5 RID: 25557
		public static float DROP_ZONE_REPEL = 2.25f;

		// Token: 0x040063D6 RID: 25558
		public static int placedLayer;

		// Token: 0x040063D7 RID: 25559
		public static int heldLayer;

		// Token: 0x040063D8 RID: 25560
		public static int heldLayerLocal;

		// Token: 0x040063D9 RID: 25561
		public static int droppedLayer;

		// Token: 0x040063DA RID: 25562
		private float acceptableSqrDistFromCenter = 47089f;

		// Token: 0x040063DB RID: 25563
		public float pieceScale = 0.04f;

		// Token: 0x040063DC RID: 25564
		public GTZone tableZone = GTZone.monkeBlocks;

		// Token: 0x040063DD RID: 25565
		[SerializeField]
		private string SharedMapConfigTitleDataKey = "SharedBlocksStartingMapConfig";

		// Token: 0x040063DE RID: 25566
		public BuilderTableNetworking builderNetworking;

		// Token: 0x040063DF RID: 25567
		public BuilderRenderer builderRenderer;

		// Token: 0x040063E0 RID: 25568
		[HideInInspector]
		public BuilderPool builderPool;

		// Token: 0x040063E1 RID: 25569
		public Transform tableCenter;

		// Token: 0x040063E2 RID: 25570
		public Transform roomCenter;

		// Token: 0x040063E3 RID: 25571
		public Transform worldCenter;

		// Token: 0x040063E4 RID: 25572
		public GameObject noBlocksArea;

		// Token: 0x040063E5 RID: 25573
		public List<GameObject> builtInPieceRoots;

		// Token: 0x040063E6 RID: 25574
		[Tooltip("Optional terminal to control loaded blocks")]
		public SharedBlocksTerminal linkedTerminal;

		// Token: 0x040063E7 RID: 25575
		[Tooltip("Can Blocks Be Placed and Grabbed")]
		public bool isTableMutable;

		// Token: 0x040063E8 RID: 25576
		public GameObject shelvesRoot;

		// Token: 0x040063E9 RID: 25577
		public GameObject dropZoneRoot;

		// Token: 0x040063EA RID: 25578
		public List<GameObject> recyclerRoot;

		// Token: 0x040063EB RID: 25579
		public List<GameObject> allShelvesRoot;

		// Token: 0x040063EC RID: 25580
		[NonSerialized]
		public List<BuilderConveyor> conveyors = new List<BuilderConveyor>();

		// Token: 0x040063ED RID: 25581
		[NonSerialized]
		public List<BuilderDispenserShelf> dispenserShelves = new List<BuilderDispenserShelf>();

		// Token: 0x040063EE RID: 25582
		public BuilderConveyorManager conveyorManager;

		// Token: 0x040063EF RID: 25583
		public List<BuilderResourceMeter> resourceMeters;

		// Token: 0x040063F0 RID: 25584
		public GameObject sharedBuildArea;

		// Token: 0x040063F1 RID: 25585
		private BoxCollider[] sharedBuildAreas;

		// Token: 0x040063F2 RID: 25586
		public BuilderPiece armShelfPieceType;

		// Token: 0x040063F3 RID: 25587
		[NonSerialized]
		public List<BuilderRecycler> recyclers;

		// Token: 0x040063F4 RID: 25588
		[NonSerialized]
		public List<BuilderDropZone> dropZones;

		// Token: 0x040063F5 RID: 25589
		private int shelfSliceUpdateIndex;

		// Token: 0x040063F6 RID: 25590
		public static int SHELF_SLICE_BUCKETS = 6;

		// Token: 0x040063F7 RID: 25591
		public float defaultTint = 1f;

		// Token: 0x040063F8 RID: 25592
		public float droppedTint = 0.75f;

		// Token: 0x040063F9 RID: 25593
		public float grabbedTint = 0.75f;

		// Token: 0x040063FA RID: 25594
		public float shelfTint = 1f;

		// Token: 0x040063FB RID: 25595
		public float potentialGrabTint = 0.75f;

		// Token: 0x040063FC RID: 25596
		public float paintingTint = 0.6f;

		// Token: 0x040063FE RID: 25598
		private List<BuilderTable.BoxCheckParams> noBlocksAreas;

		// Token: 0x040063FF RID: 25599
		private Collider[] noBlocksCheckResults = new Collider[64];

		// Token: 0x04006400 RID: 25600
		public LayerMask allPiecesMask;

		// Token: 0x04006401 RID: 25601
		public bool useSnapRotation;

		// Token: 0x04006402 RID: 25602
		public BuilderPlacementStyle usePlacementStyle;

		// Token: 0x04006403 RID: 25603
		public BuilderOptionButton buttonSnapRotation;

		// Token: 0x04006404 RID: 25604
		public BuilderOptionButton buttonSnapPosition;

		// Token: 0x04006405 RID: 25605
		public BuilderOptionButton buttonSaveLayout;

		// Token: 0x04006406 RID: 25606
		public BuilderOptionButton buttonClearLayout;

		// Token: 0x04006407 RID: 25607
		[HideInInspector]
		public List<BuilderAttachGridPlane> baseGridPlanes;

		// Token: 0x04006408 RID: 25608
		private List<BuilderPiece> basePieces;

		// Token: 0x04006409 RID: 25609
		[HideInInspector]
		public List<BuilderPiecePrivatePlot> allPrivatePlots;

		// Token: 0x0400640A RID: 25610
		private int nextPieceId;

		// Token: 0x0400640B RID: 25611
		[HideInInspector]
		public List<BuilderTable.BuildPieceSpawn> buildPieceSpawns;

		// Token: 0x0400640C RID: 25612
		[HideInInspector]
		public List<BuilderShelf> shelves;

		// Token: 0x0400640D RID: 25613
		[NonSerialized]
		public List<BuilderPiece> pieces = new List<BuilderPiece>(1024);

		// Token: 0x0400640E RID: 25614
		private Dictionary<int, int> pieceIDToIndexCache = new Dictionary<int, int>(1024);

		// Token: 0x0400640F RID: 25615
		[HideInInspector]
		public Dictionary<int, int> plotOwners;

		// Token: 0x04006410 RID: 25616
		private bool doesLocalPlayerOwnPlot;

		// Token: 0x04006411 RID: 25617
		public Dictionary<int, int> playerToArmShelfLeft;

		// Token: 0x04006412 RID: 25618
		public Dictionary<int, int> playerToArmShelfRight;

		// Token: 0x04006413 RID: 25619
		private HashSet<int> builderPiecesVisited = new HashSet<int>(128);

		// Token: 0x04006414 RID: 25620
		public BuilderResources totalResources;

		// Token: 0x04006415 RID: 25621
		[Tooltip("Resources reserved for conveyors and dispensers")]
		public BuilderResources totalReservedResources;

		// Token: 0x04006416 RID: 25622
		public BuilderResources resourcesPerPrivatePlot;

		// Token: 0x04006417 RID: 25623
		[NonSerialized]
		public int[] maxResources;

		// Token: 0x04006418 RID: 25624
		private int[] plotMaxResources;

		// Token: 0x04006419 RID: 25625
		[NonSerialized]
		public int[] usedResources;

		// Token: 0x0400641A RID: 25626
		[NonSerialized]
		public int[] reservedResources;

		// Token: 0x0400641B RID: 25627
		private List<int> playersInBuilder;

		// Token: 0x0400641C RID: 25628
		private List<IBuilderPieceFunctional> activeFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x0400641D RID: 25629
		private List<IBuilderPieceFunctional> funcComponentsToRegister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x0400641E RID: 25630
		private List<IBuilderPieceFunctional> funcComponentsToUnregister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x0400641F RID: 25631
		private List<IBuilderPieceFunctional> fixedUpdateFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04006420 RID: 25632
		private List<IBuilderPieceFunctional> funcComponentsToRegisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006421 RID: 25633
		private List<IBuilderPieceFunctional> funcComponentsToUnregisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006422 RID: 25634
		private const int MAX_SPHERE_CHECK_RESULTS = 1024;

		// Token: 0x04006423 RID: 25635
		private NativeList<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x04006424 RID: 25636
		private NativeList<BuilderGridPlaneData> checkGridPlaneData;

		// Token: 0x04006425 RID: 25637
		private NativeArray<ColliderHit> nearbyPiecesResults;

		// Token: 0x04006426 RID: 25638
		private NativeArray<OverlapSphereCommand> nearbyPiecesCommands;

		// Token: 0x04006427 RID: 25639
		private List<BuilderPotentialPlacement> allPotentialPlacements;

		// Token: 0x04006428 RID: 25640
		private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

		// Token: 0x04006429 RID: 25641
		private BuilderTable.TableState tableState;

		// Token: 0x0400642A RID: 25642
		private bool inRoom;

		// Token: 0x0400642B RID: 25643
		private bool inBuilderZone;

		// Token: 0x0400642C RID: 25644
		private static int DROPPED_PIECE_LIMIT = 100;

		// Token: 0x0400642D RID: 25645
		public static string nextUpdateOverride = string.Empty;

		// Token: 0x0400642E RID: 25646
		private List<BuilderPiece> droppedPieces;

		// Token: 0x0400642F RID: 25647
		private List<BuilderTable.DroppedPieceData> droppedPieceData;

		// Token: 0x04006430 RID: 25648
		private HashSet<int>[] repelledPieceRoots;

		// Token: 0x04006431 RID: 25649
		private int repelHistoryLength = 3;

		// Token: 0x04006432 RID: 25650
		private int repelHistoryIndex;

		// Token: 0x04006433 RID: 25651
		private bool hasRequestedConfig;

		// Token: 0x04006434 RID: 25652
		private bool isDirty;

		// Token: 0x04006435 RID: 25653
		private bool saveInProgress;

		// Token: 0x04006436 RID: 25654
		private int currentSaveSlot = -1;

		// Token: 0x04006437 RID: 25655
		[HideInInspector]
		public UnityEvent OnSaveTimeUpdated;

		// Token: 0x04006438 RID: 25656
		[HideInInspector]
		public UnityEvent<bool> OnSaveDirtyChanged;

		// Token: 0x04006439 RID: 25657
		[HideInInspector]
		public UnityEvent OnSaveSuccess;

		// Token: 0x0400643A RID: 25658
		[HideInInspector]
		public UnityEvent<string> OnSaveFailure;

		// Token: 0x0400643B RID: 25659
		[HideInInspector]
		public UnityEvent OnTableConfigurationUpdated;

		// Token: 0x0400643C RID: 25660
		[HideInInspector]
		public UnityEvent<bool> OnLocalPlayerClaimedPlot;

		// Token: 0x0400643D RID: 25661
		[HideInInspector]
		public UnityEvent OnMapCleared;

		// Token: 0x0400643E RID: 25662
		[HideInInspector]
		public UnityEvent<string> OnMapLoaded;

		// Token: 0x0400643F RID: 25663
		[HideInInspector]
		public UnityEvent<string> OnMapLoadFailed;

		// Token: 0x04006440 RID: 25664
		private List<BuilderTable.BuilderCommand> queuedBuildCommands;

		// Token: 0x04006441 RID: 25665
		private List<BuilderAction> rollBackActions;

		// Token: 0x04006442 RID: 25666
		private List<BuilderTable.BuilderCommand> rollBackBufferedCommands;

		// Token: 0x04006443 RID: 25667
		private List<BuilderTable.BuilderCommand> rollForwardCommands;

		// Token: 0x04006444 RID: 25668
		[OnEnterPlay_Clear]
		private static Dictionary<GTZone, BuilderTable> zoneToInstance;

		// Token: 0x04006445 RID: 25669
		private bool isSetup;

		// Token: 0x04006446 RID: 25670
		public BuilderTable.SnapParams pushAndEaseParams;

		// Token: 0x04006447 RID: 25671
		public BuilderTable.SnapParams overlapParams;

		// Token: 0x04006448 RID: 25672
		private BuilderTable.SnapParams currSnapParams;

		// Token: 0x04006449 RID: 25673
		public int maxPlacementChildDepth = 5;

		// Token: 0x0400644A RID: 25674
		private static List<BuilderPiece> tempPieces = new List<BuilderPiece>(256);

		// Token: 0x0400644B RID: 25675
		private static List<BuilderConveyor> tempConveyors = new List<BuilderConveyor>(256);

		// Token: 0x0400644C RID: 25676
		private static List<BuilderDispenserShelf> tempDispensers = new List<BuilderDispenserShelf>(256);

		// Token: 0x0400644D RID: 25677
		private static List<BuilderRecycler> tempRecyclers = new List<BuilderRecycler>(5);

		// Token: 0x0400644E RID: 25678
		private static List<BuilderTable.BuilderCommand> tempRollForwardCommands = new List<BuilderTable.BuilderCommand>(128);

		// Token: 0x0400644F RID: 25679
		private static List<BuilderPiece> tempDeletePieces = new List<BuilderPiece>(1024);

		// Token: 0x04006450 RID: 25680
		public const int MAX_PIECE_DATA = 2560;

		// Token: 0x04006451 RID: 25681
		public const int MAX_GRID_PLANE_DATA = 10240;

		// Token: 0x04006452 RID: 25682
		public const int MAX_PRIVATE_PLOT_DATA = 64;

		// Token: 0x04006453 RID: 25683
		public const int MAX_PLAYER_DATA = 64;

		// Token: 0x04006454 RID: 25684
		private BuilderTableData tableData;

		// Token: 0x04006455 RID: 25685
		private int fetchConfigurationAttempts;

		// Token: 0x04006456 RID: 25686
		private int maxRetries = 3;

		// Token: 0x04006457 RID: 25687
		private SharedBlocksManager.SharedBlocksMap sharedBlocksMap;

		// Token: 0x04006458 RID: 25688
		private string pendingMapID;

		// Token: 0x04006459 RID: 25689
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x0400645A RID: 25690
		private List<SharedBlocksManager.SharedBlocksMap> startingMapList = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x0400645B RID: 25691
		private SharedBlocksManager.SharedBlocksMap startingMap;

		// Token: 0x0400645C RID: 25692
		private bool hasStartingMap;

		// Token: 0x0400645D RID: 25693
		private double startingMapCacheTime = double.MinValue;

		// Token: 0x0400645E RID: 25694
		private bool getStartingMapInProgress;

		// Token: 0x0400645F RID: 25695
		private bool hasCachedTopMaps;

		// Token: 0x04006460 RID: 25696
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x04006461 RID: 25697
		private static string personalBuildKey = "MyBuild";

		// Token: 0x04006462 RID: 25698
		private static HashSet<BuilderTable.SnapOverlapKey> tempDuplicateOverlaps = new HashSet<BuilderTable.SnapOverlapKey>(16384);

		// Token: 0x04006463 RID: 25699
		private static List<BuilderPiece> childPieces = new List<BuilderPiece>(4096);

		// Token: 0x04006464 RID: 25700
		private static List<BuilderPiece> rootPieces = new List<BuilderPiece>(4096);

		// Token: 0x04006465 RID: 25701
		private static List<int> overlapPieces = new List<int>(4096);

		// Token: 0x04006466 RID: 25702
		private static List<int> overlapOtherPieces = new List<int>(4096);

		// Token: 0x04006467 RID: 25703
		private static List<long> overlapPacked = new List<long>(4096);

		// Token: 0x04006468 RID: 25704
		private static char[] mapIDBuffer = new char[8];

		// Token: 0x04006469 RID: 25705
		private static Dictionary<long, int> snapOverlapSanity = new Dictionary<long, int>(16384);

		// Token: 0x0400646A RID: 25706
		private static List<int> tempPeiceIds = new List<int>(4096);

		// Token: 0x0400646B RID: 25707
		private static List<int> tempParentPeiceIds = new List<int>(4096);

		// Token: 0x0400646C RID: 25708
		private static List<int> tempAttachIndexes = new List<int>(4096);

		// Token: 0x0400646D RID: 25709
		private static List<int> tempParentAttachIndexes = new List<int>(4096);

		// Token: 0x0400646E RID: 25710
		private static List<int> tempParentActorNumbers = new List<int>(4096);

		// Token: 0x0400646F RID: 25711
		private static List<bool> tempInLeftHand = new List<bool>(4096);

		// Token: 0x04006470 RID: 25712
		private static List<int> tempPiecePlacement = new List<int>(4096);

		// Token: 0x02000DBE RID: 3518
		private struct BoxCheckParams
		{
			// Token: 0x04006471 RID: 25713
			public Vector3 center;

			// Token: 0x04006472 RID: 25714
			public Vector3 halfExtents;

			// Token: 0x04006473 RID: 25715
			public Quaternion rotation;
		}

		// Token: 0x02000DBF RID: 3519
		[Serializable]
		public class BuildPieceSpawn
		{
			// Token: 0x04006474 RID: 25716
			public GameObject buildPiecePrefab;

			// Token: 0x04006475 RID: 25717
			public int count = 1;
		}

		// Token: 0x02000DC0 RID: 3520
		public enum BuilderCommandType
		{
			// Token: 0x04006477 RID: 25719
			Create,
			// Token: 0x04006478 RID: 25720
			Place,
			// Token: 0x04006479 RID: 25721
			Grab,
			// Token: 0x0400647A RID: 25722
			Drop,
			// Token: 0x0400647B RID: 25723
			Remove,
			// Token: 0x0400647C RID: 25724
			Paint,
			// Token: 0x0400647D RID: 25725
			Recycle,
			// Token: 0x0400647E RID: 25726
			ClaimPlot,
			// Token: 0x0400647F RID: 25727
			FreePlot,
			// Token: 0x04006480 RID: 25728
			CreateArmShelf,
			// Token: 0x04006481 RID: 25729
			PlayerLeftRoom,
			// Token: 0x04006482 RID: 25730
			FunctionalStateChange,
			// Token: 0x04006483 RID: 25731
			SetSelection,
			// Token: 0x04006484 RID: 25732
			Repel
		}

		// Token: 0x02000DC1 RID: 3521
		public enum TableState
		{
			// Token: 0x04006486 RID: 25734
			WaitingForZoneAndRoom,
			// Token: 0x04006487 RID: 25735
			WaitingForInitalBuild,
			// Token: 0x04006488 RID: 25736
			ReceivingInitialBuild,
			// Token: 0x04006489 RID: 25737
			WaitForInitialBuildMaster,
			// Token: 0x0400648A RID: 25738
			WaitForMasterResync,
			// Token: 0x0400648B RID: 25739
			ReceivingMasterResync,
			// Token: 0x0400648C RID: 25740
			InitialBuild,
			// Token: 0x0400648D RID: 25741
			ExecuteQueuedCommands,
			// Token: 0x0400648E RID: 25742
			Ready,
			// Token: 0x0400648F RID: 25743
			BadData,
			// Token: 0x04006490 RID: 25744
			WaitingForSharedMapLoad
		}

		// Token: 0x02000DC2 RID: 3522
		public enum DroppedPieceState
		{
			// Token: 0x04006492 RID: 25746
			None = -1,
			// Token: 0x04006493 RID: 25747
			Light,
			// Token: 0x04006494 RID: 25748
			Heavy,
			// Token: 0x04006495 RID: 25749
			Frozen
		}

		// Token: 0x02000DC3 RID: 3523
		private struct DroppedPieceData
		{
			// Token: 0x04006496 RID: 25750
			public BuilderTable.DroppedPieceState droppedState;

			// Token: 0x04006497 RID: 25751
			public float speedThreshCrossedTime;

			// Token: 0x04006498 RID: 25752
			public float filteredSpeed;
		}

		// Token: 0x02000DC4 RID: 3524
		public struct BuilderCommand
		{
			// Token: 0x04006499 RID: 25753
			public BuilderTable.BuilderCommandType type;

			// Token: 0x0400649A RID: 25754
			public int pieceType;

			// Token: 0x0400649B RID: 25755
			public int pieceId;

			// Token: 0x0400649C RID: 25756
			public int attachPieceId;

			// Token: 0x0400649D RID: 25757
			public int parentPieceId;

			// Token: 0x0400649E RID: 25758
			public int parentAttachIndex;

			// Token: 0x0400649F RID: 25759
			public int attachIndex;

			// Token: 0x040064A0 RID: 25760
			public Vector3 localPosition;

			// Token: 0x040064A1 RID: 25761
			public Quaternion localRotation;

			// Token: 0x040064A2 RID: 25762
			public byte twist;

			// Token: 0x040064A3 RID: 25763
			public sbyte bumpOffsetX;

			// Token: 0x040064A4 RID: 25764
			public sbyte bumpOffsetZ;

			// Token: 0x040064A5 RID: 25765
			public Vector3 velocity;

			// Token: 0x040064A6 RID: 25766
			public Vector3 angVelocity;

			// Token: 0x040064A7 RID: 25767
			public bool isLeft;

			// Token: 0x040064A8 RID: 25768
			public int materialType;

			// Token: 0x040064A9 RID: 25769
			public NetPlayer player;

			// Token: 0x040064AA RID: 25770
			public BuilderPiece.State state;

			// Token: 0x040064AB RID: 25771
			public bool isQueued;

			// Token: 0x040064AC RID: 25772
			public bool canRollback;

			// Token: 0x040064AD RID: 25773
			public int localCommandId;

			// Token: 0x040064AE RID: 25774
			public int serverTimeStamp;
		}

		// Token: 0x02000DC5 RID: 3525
		[Serializable]
		public struct SnapParams
		{
			// Token: 0x040064AF RID: 25775
			public float minOffsetY;

			// Token: 0x040064B0 RID: 25776
			public float maxOffsetY;

			// Token: 0x040064B1 RID: 25777
			public float maxUpDotProduct;

			// Token: 0x040064B2 RID: 25778
			public float maxTwistDotProduct;

			// Token: 0x040064B3 RID: 25779
			public float snapAttachDistance;

			// Token: 0x040064B4 RID: 25780
			public float snapDelayTime;

			// Token: 0x040064B5 RID: 25781
			public float snapDelayOffsetDist;

			// Token: 0x040064B6 RID: 25782
			public float unSnapDelayTime;

			// Token: 0x040064B7 RID: 25783
			public float unSnapDelayDist;

			// Token: 0x040064B8 RID: 25784
			public float maxBlockSnapDist;
		}

		// Token: 0x02000DC6 RID: 3526
		private struct SnapOverlapKey
		{
			// Token: 0x06005783 RID: 22403 RVA: 0x001BE279 File Offset: 0x001BC479
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int>(this.piece.GetHashCode(), this.otherPiece.GetHashCode());
			}

			// Token: 0x06005784 RID: 22404 RVA: 0x001BE296 File Offset: 0x001BC496
			public bool Equals(BuilderTable.SnapOverlapKey other)
			{
				return this.piece == other.piece && this.otherPiece == other.otherPiece;
			}

			// Token: 0x06005785 RID: 22405 RVA: 0x001BE2B6 File Offset: 0x001BC4B6
			public override bool Equals(object o)
			{
				return o is BuilderTable.SnapOverlapKey && this.Equals((BuilderTable.SnapOverlapKey)o);
			}

			// Token: 0x040064B9 RID: 25785
			public long piece;

			// Token: 0x040064BA RID: 25786
			public long otherPiece;
		}
	}
}
