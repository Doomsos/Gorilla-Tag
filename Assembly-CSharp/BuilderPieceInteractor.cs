using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000586 RID: 1414
public class BuilderPieceInteractor : MonoBehaviour
{
	// Token: 0x060023C8 RID: 9160 RVA: 0x000BBF74 File Offset: 0x000BA174
	private void Awake()
	{
		if (BuilderPieceInteractor.instance == null)
		{
			BuilderPieceInteractor.instance = this;
			BuilderPieceInteractor.hasInstance = true;
		}
		else if (BuilderPieceInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		List<GorillaVelocityEstimator> list = new List<GorillaVelocityEstimator>(2);
		list.Add(this.velocityEstimatorLeft);
		list.Add(this.velocityEstimatorRight);
		this.velocityEstimator = list;
		List<BuilderLaserSight> list2 = new List<BuilderLaserSight>(2);
		list2.Add(this.laserSightLeft);
		list2.Add(this.laserSightRight);
		this.laserSight = list2;
		this.handState = new List<BuilderPieceInteractor.HandState>(2);
		this.heldPiece = new List<BuilderPiece>(2);
		this.potentialHeldPiece = new List<BuilderPiece>(2);
		this.potentialGrabbedOffsetDist = new List<float>(2);
		this.heldInitialRot = new List<Quaternion>(2);
		this.heldCurrentRot = new List<Quaternion>(2);
		this.heldInitialPos = new List<Vector3>(2);
		this.heldCurrentPos = new List<Vector3>(2);
		this.heldChainLength = new int[2];
		this.heldChainLength[0] = 0;
		this.heldChainLength[1] = 0;
		this.heldChainCost = new List<int[]>(2);
		for (int i = 0; i < 2; i++)
		{
			this.heldChainCost.Add(new int[3]);
		}
		BuilderPieceInteractor.allPotentialPlacements = new List<BuilderPotentialPlacement>[2];
		this.delayedPotentialPlacement = new List<BuilderPotentialPlacement>(2);
		this.delayedPlacementTime = new List<float>(2);
		this.prevPotentialPlacement = new List<BuilderPotentialPlacement>(2);
		this.glowBumps = new List<List<BuilderBumpGlow>>(2);
		for (int j = 0; j < 2; j++)
		{
			this.handState.Add(BuilderPieceInteractor.HandState.Empty);
			this.heldPiece.Add(null);
			this.potentialHeldPiece.Add(null);
			this.potentialGrabbedOffsetDist.Add(0f);
			this.heldInitialRot.Add(Quaternion.identity);
			this.heldCurrentRot.Add(Quaternion.identity);
			this.heldInitialPos.Add(Vector3.zero);
			this.heldCurrentPos.Add(Vector3.zero);
			this.delayedPotentialPlacement.Add(default(BuilderPotentialPlacement));
			this.delayedPlacementTime.Add(-1f);
			this.prevPotentialPlacement.Add(default(BuilderPotentialPlacement));
			BuilderPieceInteractor.allPotentialPlacements[j] = new List<BuilderPotentialPlacement>(128);
			this.glowBumps.Add(new List<BuilderBumpGlow>(1024));
		}
		this.checkPiecesInSphere = new NativeArray<OverlapSphereCommand>(2, 4, 1);
		this.checkPiecesInSphereResults = new NativeArray<ColliderHit>(2048, 4, 1);
		this.grabSphereCast = new NativeArray<SpherecastCommand>(2, 4, 1);
		this.grabSphereCastResults = new NativeArray<RaycastHit>(128, 4, 1);
		BuilderPieceInteractor.handGridPlaneData = new NativeList<BuilderGridPlaneData>[2];
		BuilderPieceInteractor.handPieceData = new NativeList<BuilderPieceData>[2];
		BuilderPieceInteractor.localAttachableGridPlaneData = new NativeList<BuilderGridPlaneData>[2];
		BuilderPieceInteractor.localAttachablePieceData = new NativeList<BuilderPieceData>[2];
		for (int k = 0; k < 2; k++)
		{
			BuilderPieceInteractor.handGridPlaneData[k] = new NativeList<BuilderGridPlaneData>(512, 4);
			BuilderPieceInteractor.handPieceData[k] = new NativeList<BuilderPieceData>(512, 4);
			BuilderPieceInteractor.localAttachableGridPlaneData[k] = new NativeList<BuilderGridPlaneData>(10240, 4);
			BuilderPieceInteractor.localAttachablePieceData[k] = new NativeList<BuilderPieceData>(2560, 4);
		}
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x000BC2AC File Offset: 0x000BA4AC
	public bool GetIsHolding(XRNode node)
	{
		if (this.heldPiece == null)
		{
			return false;
		}
		if (node == 4)
		{
			return this.heldPiece[0] != null;
		}
		return this.heldPiece[1] != null;
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x00002789 File Offset: 0x00000989
	public void PreInteract()
	{
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000BC2E4 File Offset: 0x000BA4E4
	public void StartFindNearbyPieces()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (!builderTable.isTableMutable)
		{
			return;
		}
		QueryParameters queryParameters = default(QueryParameters);
		queryParameters.layerMask = builderTable.allPiecesMask;
		QueryParameters queryParameters2 = queryParameters;
		this.checkPiecesInSphere[0] = new OverlapSphereCommand(offlineVRRig.leftHand.overrideTarget.position, (this.handState[0] == BuilderPieceInteractor.HandState.Empty) ? 0.0375f : 1f, queryParameters2);
		this.checkPiecesInSphere[1] = new OverlapSphereCommand(offlineVRRig.rightHand.overrideTarget.position, (this.handState[1] == BuilderPieceInteractor.HandState.Empty) ? 0.0375f : 1f, queryParameters2);
		this.checkNearbyPiecesHandle = OverlapSphereCommand.ScheduleBatch(this.checkPiecesInSphere, this.checkPiecesInSphereResults, 1, 1024, default(JobHandle));
		for (int i = 0; i < 64; i++)
		{
			this.grabSphereCastResults[i] = this.emptyRaycastHit;
		}
		this.grabSphereCast[0] = new SpherecastCommand(offlineVRRig.leftHand.overrideTarget.position, 0.0375f, offlineVRRig.leftHand.overrideTarget.rotation * Vector3.right, queryParameters2, 0.15f);
		this.grabSphereCast[1] = new SpherecastCommand(offlineVRRig.rightHand.overrideTarget.position, 0.0375f, offlineVRRig.rightHand.overrideTarget.rotation * -Vector3.right, queryParameters2, 0.15f);
		this.findPiecesToGrab = SpherecastCommand.ScheduleBatch(this.grabSphereCast, this.grabSphereCastResults, 1, 64, default(JobHandle));
		JobHandle.ScheduleBatchedJobs();
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x000BC4B4 File Offset: 0x000BA6B4
	private void CalcLocalGridPlanes()
	{
		this.checkNearbyPiecesHandle.Complete();
		for (int i = 0; i < 2; i++)
		{
			if (this.handState[i] == BuilderPieceInteractor.HandState.Grabbed)
			{
				BuilderPieceInteractor.localAttachableGridPlaneData[i].Clear();
				BuilderPieceInteractor.localAttachablePieceData[i].Clear();
				BuilderPieceInteractor.tempPieceSet.Clear();
				if (this.currentTable.IsInBuilderZone())
				{
					for (int j = 0; j < 1024; j++)
					{
						int num = i * 1024 + j;
						if (this.checkPiecesInSphereResults[num].instanceID == 0)
						{
							break;
						}
						BuilderPiece pieceInHand = this.heldPiece[i];
						BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.checkPiecesInSphereResults[num].collider);
						if (builderPieceFromCollider != null && !BuilderPieceInteractor.tempPieceSet.Contains(builderPieceFromCollider))
						{
							BuilderPieceInteractor.tempPieceSet.Add(builderPieceFromCollider);
							if (this.currentTable.CanPiecesPotentiallySnap(pieceInHand, builderPieceFromCollider))
							{
								int length = BuilderPieceInteractor.localAttachablePieceData[i].Length;
								NativeList<BuilderPieceData>[] array = BuilderPieceInteractor.localAttachablePieceData;
								int num2 = i;
								BuilderPieceData builderPieceData = new BuilderPieceData(builderPieceFromCollider);
								array[num2].Add(ref builderPieceData);
								for (int k = 0; k < builderPieceFromCollider.gridPlanes.Count; k++)
								{
									NativeList<BuilderGridPlaneData>[] array2 = BuilderPieceInteractor.localAttachableGridPlaneData;
									int num3 = i;
									BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[k], length);
									array2[num3].Add(ref builderGridPlaneData);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000BC63C File Offset: 0x000BA83C
	private void OnDestroy()
	{
		if (BuilderPieceInteractor.instance == this)
		{
			BuilderPieceInteractor.hasInstance = false;
			BuilderPieceInteractor.instance = null;
		}
		if (this.checkPiecesInSphere.IsCreated)
		{
			this.checkPiecesInSphere.Dispose();
		}
		if (this.checkPiecesInSphereResults.IsCreated)
		{
			this.checkPiecesInSphereResults.Dispose();
		}
		if (this.grabSphereCast.IsCreated)
		{
			this.grabSphereCast.Dispose();
		}
		if (this.grabSphereCastResults.IsCreated)
		{
			this.grabSphereCastResults.Dispose();
		}
		for (int i = 0; i < 2; i++)
		{
			if (BuilderPieceInteractor.handGridPlaneData[i].IsCreated)
			{
				BuilderPieceInteractor.handGridPlaneData[i].Dispose();
			}
			if (BuilderPieceInteractor.handPieceData[i].IsCreated)
			{
				BuilderPieceInteractor.handPieceData[i].Dispose();
			}
			if (BuilderPieceInteractor.localAttachableGridPlaneData[i].IsCreated)
			{
				BuilderPieceInteractor.localAttachableGridPlaneData[i].Dispose();
			}
			if (BuilderPieceInteractor.localAttachablePieceData[i].IsCreated)
			{
				BuilderPieceInteractor.localAttachablePieceData[i].Dispose();
			}
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000BC760 File Offset: 0x000BA960
	public bool BlockSnowballCreation()
	{
		BuilderTable builderTable;
		return !(GorillaTagger.Instance == null) && BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable) && (builderTable.IsInBuilderZone() && builderTable.isTableMutable && GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99f);
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000BC7C4 File Offset: 0x000BA9C4
	public void OnLateUpdate()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (!builderTable.isTableMutable)
		{
			return;
		}
		this.currentTable = builderTable;
		this.CalcLocalGridPlanes();
		BodyDockPositions myBodyDockPositions = offlineVRRig.myBodyDockPositions;
		this.findPiecesToGrab.Complete();
		this.UpdateHandState(BuilderPieceInteractor.HandType.Left, offlineVRRig.leftHand.overrideTarget, Vector3.right, myBodyDockPositions.leftHandTransform, this.equipmentInteractor.isLeftGrabbing, this.equipmentInteractor.wasLeftGrabPressed, this.equipmentInteractor.leftHandHeldEquipment, this.equipmentInteractor.disableLeftGrab);
		this.UpdateHandState(BuilderPieceInteractor.HandType.Right, offlineVRRig.rightHand.overrideTarget, -Vector3.right, myBodyDockPositions.rightHandTransform, this.equipmentInteractor.isRightGrabbing, this.equipmentInteractor.wasRightGrabPressed, this.equipmentInteractor.rightHandHeldEquipment, this.equipmentInteractor.disableRightGrab);
		this.UpdatePieceDisables();
		if (offlineVRRig != null)
		{
			bool flag = offlineVRRig.scaleFactor < 1f;
			if (flag && !this.isRigSmall)
			{
				if (offlineVRRig.builderArmShelfLeft != null)
				{
					offlineVRRig.builderArmShelfLeft.DropAttachedPieces();
					if (offlineVRRig.builderArmShelfLeft.piece != null)
					{
						foreach (Collider collider in offlineVRRig.builderArmShelfLeft.piece.colliders)
						{
							collider.enabled = false;
						}
					}
				}
				if (!(offlineVRRig.builderArmShelfRight != null))
				{
					goto IL_2C0;
				}
				offlineVRRig.builderArmShelfRight.DropAttachedPieces();
				if (!(offlineVRRig.builderArmShelfRight.piece != null))
				{
					goto IL_2C0;
				}
				using (List<Collider>.Enumerator enumerator = offlineVRRig.builderArmShelfRight.piece.colliders.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_2C0;
				}
			}
			if (!flag && this.isRigSmall)
			{
				if (offlineVRRig.builderArmShelfLeft != null && offlineVRRig.builderArmShelfLeft.piece != null)
				{
					foreach (Collider collider3 in offlineVRRig.builderArmShelfLeft.piece.colliders)
					{
						collider3.enabled = true;
					}
				}
				if (offlineVRRig.builderArmShelfRight != null && offlineVRRig.builderArmShelfRight.piece != null)
				{
					foreach (Collider collider4 in offlineVRRig.builderArmShelfRight.piece.colliders)
					{
						collider4.enabled = true;
					}
				}
			}
			IL_2C0:
			this.isRigSmall = flag;
		}
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000BCACC File Offset: 0x000BACCC
	private void SetHandState(int handIndex, BuilderPieceInteractor.HandState newState)
	{
		if (this.handState[handIndex] == BuilderPieceInteractor.HandState.Empty && this.potentialHeldPiece[handIndex] != null)
		{
			this.potentialHeldPiece[handIndex].PotentialGrab(false);
			this.potentialHeldPiece[handIndex] = null;
		}
		this.handState[handIndex] = newState;
		switch (this.handState[handIndex])
		{
		case BuilderPieceInteractor.HandState.Empty:
			this.heldChainLength[handIndex] = 0;
			for (int i = 0; i < this.heldChainCost[handIndex].Length; i++)
			{
				this.heldChainCost[handIndex][i] = 0;
			}
			break;
		case BuilderPieceInteractor.HandState.Grabbed:
			this.heldChainLength[handIndex] = this.heldPiece[handIndex].GetChildCount() + 1;
			this.heldPiece[handIndex].GetChainCost(this.heldChainCost[handIndex]);
			return;
		case BuilderPieceInteractor.HandState.PotentialGrabbed:
			this.heldChainLength[handIndex] = 0;
			for (int j = 0; j < this.heldChainCost[handIndex].Length; j++)
			{
				this.heldChainCost[handIndex][j] = 0;
			}
			return;
		case BuilderPieceInteractor.HandState.WaitForGrabbed:
			break;
		default:
			return;
		}
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000BCBE8 File Offset: 0x000BADE8
	public void OnCountChangedForRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (this.heldPiece[0] != null && this.heldPiece[0].Equals(piece))
		{
			this.heldChainLength[0] = this.heldPiece[0].GetChainCostAndCount(this.heldChainCost[0]);
			return;
		}
		if (this.heldPiece[1] != null && this.heldPiece[1].Equals(piece))
		{
			this.heldChainLength[1] = this.heldPiece[1].GetChainCostAndCount(this.heldChainCost[1]);
		}
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x000BCC9C File Offset: 0x000BAE9C
	private void UpdateHandState(BuilderPieceInteractor.HandType handType, Transform handTransform, Vector3 palmForwardLocal, Transform handAttachPoint, bool isGrabbing, bool wasGrabPressed, IHoldableObject heldEquipment, bool grabDisabled)
	{
		int num = (int)((handType + 1) % (BuilderPieceInteractor.HandType)2);
		bool flag = GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f;
		bool flag2 = isGrabbing && !wasGrabPressed;
		bool flag3 = this.heldPiece[(int)handType] != null && (!isGrabbing || flag);
		bool flag4 = heldEquipment != null;
		bool flag5 = this.heldPiece[(int)handType] != null;
		bool flag6 = !flag4 && !flag5 && !grabDisabled && !flag && this.currentTable.IsInBuilderZone();
		BuilderPiece builderPiece = null;
		Vector3 position = handTransform.position;
		handTransform.rotation * palmForwardLocal;
		Vector3 zero = Vector3.zero;
		switch (this.handState[(int)handType])
		{
		case BuilderPieceInteractor.HandState.Empty:
			if (!flag)
			{
				float num2 = float.MaxValue;
				for (int i = 0; i < 1024; i++)
				{
					int num3 = (int)(handType * (BuilderPieceInteractor.HandType)1024 + i);
					ColliderHit colliderHit = this.checkPiecesInSphereResults[num3];
					if (colliderHit.instanceID == 0)
					{
						break;
					}
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(colliderHit.collider);
					if (builderPieceFromCollider != null && !builderPieceFromCollider.isBuiltIntoTable)
					{
						float num4 = Vector3.SqrMagnitude(colliderHit.collider.transform.position - handTransform.position);
						if ((builderPiece == null || num4 < num2) && builderPieceFromCollider.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPieceFromCollider.transform.position))
						{
							builderPiece = builderPieceFromCollider;
							num2 = num4;
						}
					}
				}
				if (builderPiece == null)
				{
					for (int j = 0; j < 64; j++)
					{
						int num5 = (int)(handType * (BuilderPieceInteractor.HandType)64 + j);
						RaycastHit raycastHit = this.grabSphereCastResults[num5];
						if (raycastHit.colliderInstanceID == 0)
						{
							break;
						}
						BuilderPiece builderPieceFromCollider2 = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
						if (builderPieceFromCollider2 != null && !builderPieceFromCollider2.isBuiltIntoTable && (builderPiece == null || raycastHit.distance < num2) && builderPieceFromCollider2.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPieceFromCollider2.transform.position))
						{
							builderPiece = builderPieceFromCollider2;
							num2 = raycastHit.distance;
						}
					}
				}
			}
			if (this.potentialHeldPiece[(int)handType] != builderPiece)
			{
				if (this.potentialHeldPiece[(int)handType] != null)
				{
					this.potentialHeldPiece[(int)handType].PotentialGrab(false);
				}
				this.potentialHeldPiece[(int)handType] = builderPiece;
				if (this.potentialHeldPiece[(int)handType] != null)
				{
					this.potentialHeldPiece[(int)handType].PotentialGrab(true);
				}
			}
			if (flag6 && flag2 && builderPiece != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				this.CalcPieceLocalPosAndRot(builderPiece.transform.position, builderPiece.transform.rotation, handAttachPoint, out vector, out quaternion);
				if (BuilderPiece.IsDroppedState(builderPiece.state) || this.heldPiece[num] == builderPiece)
				{
					builderPiece.PlayGrabbedFx();
					this.currentTable.RequestGrabPiece(builderPiece, handType == BuilderPieceInteractor.HandType.Left, vector, quaternion);
					return;
				}
				builderPiece.PlayGrabbedFx();
				this.SetHandState((int)handType, BuilderPieceInteractor.HandState.PotentialGrabbed);
				this.potentialGrabbedOffsetDist[(int)handType] = 0f;
				this.heldPiece[(int)handType] = builderPiece;
				this.heldInitialRot[(int)handType] = quaternion;
				this.heldCurrentRot[(int)handType] = quaternion;
				this.heldInitialPos[(int)handType] = vector;
				this.heldCurrentPos[(int)handType] = vector;
				return;
			}
			break;
		case BuilderPieceInteractor.HandState.Grabbed:
		{
			if (flag3)
			{
				Vector3 vector2 = this.velocityEstimator[(int)handType].linearVelocity;
				if (flag)
				{
					Vector3 vector3 = this.currentTable.roomCenter.position - this.velocityEstimator[(int)handType].handPos;
					vector3.Normalize();
					Vector3 vector4 = Quaternion.Euler(0f, 180f, 0f) * vector3;
					vector2 = BuilderTable.DROP_ZONE_REPEL * vector4;
				}
				else if (this.prevPotentialPlacement[(int)handType].attachPiece == this.heldPiece[(int)handType] && this.prevPotentialPlacement[(int)handType].parentPiece != null)
				{
					Vector3 vector5 = this.prevPotentialPlacement[(int)handType].parentPiece.gridPlanes[this.prevPotentialPlacement[(int)handType].parentAttachIndex].transform.TransformDirection(this.prevPotentialPlacement[(int)handType].attachPlaneNormal);
					vector2 += vector5 * 1f;
				}
				this.currentTable.TryDropPiece(handType == BuilderPieceInteractor.HandType.Left, this.heldPiece[(int)handType], vector2, this.velocityEstimator[(int)handType].angularVelocity);
				return;
			}
			BuilderPiece builderPiece2 = this.heldPiece[(int)handType];
			if (builderPiece2 != null)
			{
				builderPiece2.transform.localRotation = this.heldInitialRot[(int)handType];
				builderPiece2.transform.localPosition = this.heldInitialPos[(int)handType];
				Quaternion quaternion2 = this.heldCurrentRot[(int)handType];
				Vector3 vector6 = this.heldCurrentPos[(int)handType];
				NativeList<BuilderGridPlaneData>[] array = BuilderPieceInteractor.localAttachableGridPlaneData;
				BuilderPieceInteractor.handPieceData[(int)handType].Clear();
				BuilderPieceInteractor.handGridPlaneData[(int)handType].Clear();
				BuilderTableJobs.BuildTestPieceListForJob(builderPiece2, BuilderPieceInteractor.handPieceData[(int)handType], BuilderPieceInteractor.handGridPlaneData[(int)handType]);
				BuilderPieceInteractor.allPotentialPlacements[(int)handType].Clear();
				BuilderPotentialPlacement builderPotentialPlacement;
				bool flag7 = this.currentTable.TryPlacePieceOnTableNoDropJobs(BuilderPieceInteractor.handGridPlaneData[(int)handType], BuilderPieceInteractor.handPieceData[(int)handType], BuilderPieceInteractor.localAttachableGridPlaneData[(int)handType], BuilderPieceInteractor.localAttachablePieceData[(int)handType], out builderPotentialPlacement, BuilderPieceInteractor.allPotentialPlacements[(int)handType]);
				if (flag7)
				{
					BuilderPiece.State state = builderPotentialPlacement.attachPiece.state;
					BuilderPiece.State state2 = (builderPotentialPlacement.parentPiece == null) ? BuilderPiece.State.None : builderPotentialPlacement.parentPiece.state;
					bool flag8 = state == BuilderPiece.State.Grabbed || state == BuilderPiece.State.GrabbedLocal;
					bool flag9 = state2 == BuilderPiece.State.Grabbed || state2 == BuilderPiece.State.GrabbedLocal;
					bool flag10 = flag8 && flag9;
					int num6 = (int)((handType + 1) % (BuilderPieceInteractor.HandType)2);
					BuilderPieceInteractor.HandState handState = this.handState[num6];
					if (flag10 && builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex].male)
					{
						flag7 = false;
					}
					else if (flag10 && handState == BuilderPieceInteractor.HandState.WaitingForSnap)
					{
						flag7 = false;
					}
				}
				if (flag7)
				{
					for (int k = 0; k < BuilderPieceInteractor.allPotentialPlacements[(int)handType].Count; k++)
					{
						BuilderPotentialPlacement builderPotentialPlacement2 = BuilderPieceInteractor.allPotentialPlacements[(int)handType][k];
						BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement2.attachPiece.gridPlanes[builderPotentialPlacement2.attachIndex];
						BuilderAttachGridPlane builderAttachGridPlane2 = (builderPotentialPlacement2.parentPiece == null) ? null : builderPotentialPlacement2.parentPiece.gridPlanes[builderPotentialPlacement2.parentAttachIndex];
						bool flag11 = builderAttachGridPlane.IsConnected(builderPotentialPlacement2.attachBounds);
						if (!flag11)
						{
							flag11 = builderAttachGridPlane2.IsConnected(builderPotentialPlacement2.parentAttachBounds);
						}
						if (flag11)
						{
							flag7 = false;
							break;
						}
					}
				}
				if (flag7)
				{
					Vector3 vector7 = builderPotentialPlacement.localPosition;
					Quaternion quaternion3 = builderPotentialPlacement.localRotation;
					Vector3 vector8 = builderPotentialPlacement.attachPlaneNormal;
					if (builderPotentialPlacement.parentPiece != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
						vector7 = builderAttachGridPlane3.transform.TransformPoint(builderPotentialPlacement.localPosition);
						quaternion3 = builderAttachGridPlane3.transform.rotation * builderPotentialPlacement.localRotation;
						vector8 = builderAttachGridPlane3.transform.TransformDirection(builderPotentialPlacement.attachPlaneNormal);
					}
					Vector3 vector9 = handAttachPoint.transform.InverseTransformPoint(vector7);
					Quaternion quaternion4 = Quaternion.Inverse(handAttachPoint.transform.rotation) * quaternion3;
					float attachDistance = builderPotentialPlacement.attachDistance;
					float num7 = Mathf.InverseLerp(this.currentTable.pushAndEaseParams.snapDelayOffsetDist, this.currentTable.pushAndEaseParams.maxOffsetY, attachDistance);
					num7 = Mathf.Clamp(num7, 0f, 1f);
					bool flag12 = builderPotentialPlacement.attachPiece == builderPiece2;
					bool flag13 = builderPotentialPlacement.attachPiece == builderPiece2;
					if (flag12)
					{
						Quaternion quaternion5 = this.heldInitialRot[(int)handType];
						Quaternion quaternion6 = Quaternion.Slerp(quaternion4, quaternion5, num7);
						quaternion2 = Quaternion.Slerp(quaternion2, quaternion6, 0.1f);
					}
					if (flag13)
					{
						Vector3 vector10 = this.heldInitialPos[(int)handType];
						Vector3 vector11 = handAttachPoint.transform.InverseTransformDirection(vector8);
						Vector3 vector12 = vector9 + vector11 * this.currentTable.pushAndEaseParams.snapDelayOffsetDist - vector10;
						float num8 = Vector3.Dot(vector12, vector11);
						num8 = Mathf.Min(0f, num8);
						Vector3 vector13 = vector11 * num8;
						Vector3 vector14 = vector12 - vector13;
						Vector3 vector15 = vector10 + Vector3.Lerp(vector14, Vector3.zero, num7);
						vector6 = Vector3.Lerp(vector6, vector15, 0.5f);
					}
					this.heldCurrentRot[(int)handType] = quaternion2;
					this.heldCurrentPos[(int)handType] = vector6;
					builderPiece2.transform.localRotation = quaternion2;
					builderPiece2.transform.localPosition = vector6;
					bool flag14 = Vector3.Dot(this.velocityEstimator[(int)handType].linearVelocity, vector8) > 0f;
					float snapAttachDistance = this.currentTable.pushAndEaseParams.snapAttachDistance;
					if (builderPotentialPlacement.attachDistance < snapAttachDistance && !flag14 && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPiece2, builderPotentialPlacement.parentPiece))
					{
						GorillaTagger.Instance.StartVibration(handType == BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength, this.currentTable.pushAndEaseParams.snapDelayTime * 2f);
						if (((builderPotentialPlacement.parentPiece == null) ? BuilderPiece.State.None : builderPotentialPlacement.parentPiece.state) == BuilderPiece.State.GrabbedLocal)
						{
							GorillaTagger.Instance.StartVibration(handType > BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength, this.currentTable.pushAndEaseParams.snapDelayTime * 2f);
						}
						this.delayedPotentialPlacement[(int)handType] = builderPotentialPlacement;
						this.delayedPlacementTime[(int)handType] = 0f;
						this.SetHandState((int)handType, BuilderPieceInteractor.HandState.WaitingForSnap);
					}
					else
					{
						float num9 = this.currentTable.gridSize * 0.5f * (this.currentTable.gridSize * 0.5f);
						if (this.prevPotentialPlacement[(int)handType].attachPiece != builderPotentialPlacement.attachPiece || this.prevPotentialPlacement[(int)handType].parentPiece != builderPotentialPlacement.parentPiece || this.prevPotentialPlacement[(int)handType].attachIndex != builderPotentialPlacement.attachIndex || this.prevPotentialPlacement[(int)handType].parentAttachIndex != builderPotentialPlacement.parentAttachIndex || Vector3.SqrMagnitude(this.prevPotentialPlacement[(int)handType].localPosition - builderPotentialPlacement.localPosition) > num9)
						{
							GorillaTagger.Instance.StartVibration(handType == BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength * 0.15f, this.currentTable.pushAndEaseParams.snapDelayTime);
							try
							{
								this.ClearGlowBumps((int)handType);
								this.AddGlowBumps((int)handType, BuilderPieceInteractor.allPotentialPlacements[(int)handType]);
							}
							catch (Exception ex)
							{
								Debug.LogErrorFormat("Error adding glow bumps {0}", new object[]
								{
									ex.ToString()
								});
							}
						}
					}
					this.UpdateGlowBumps((int)handType, 1f - num7);
					this.prevPotentialPlacement[(int)handType] = builderPotentialPlacement;
					return;
				}
				this.ClearGlowBumps((int)handType);
				Quaternion quaternion7 = this.heldInitialRot[(int)handType];
				quaternion2 = Quaternion.Slerp(quaternion2, quaternion7, 0.1f);
				Vector3 vector16 = this.heldInitialPos[(int)handType];
				vector6 = Vector3.Lerp(vector6, vector16, 0.1f);
				this.heldCurrentRot[(int)handType] = quaternion2;
				this.heldCurrentPos[(int)handType] = vector6;
				builderPiece2.transform.localRotation = quaternion2;
				builderPiece2.transform.localPosition = vector6;
				this.prevPotentialPlacement[(int)handType] = default(BuilderPotentialPlacement);
				return;
			}
			break;
		}
		case BuilderPieceInteractor.HandState.PotentialGrabbed:
		{
			if (flag3)
			{
				BuilderPiece builderPiece3 = this.heldPiece[(int)handType];
				this.ClearUnSnapOffset((int)handType, builderPiece3);
				this.RemovePieceFromHand(builderPiece3, (int)handType);
				this.heldPiece[(int)handType] = null;
				this.SetHandState((int)handType, BuilderPieceInteractor.HandState.Empty);
				return;
			}
			BuilderPiece builderPiece4 = this.heldPiece[(int)handType];
			Vector3 vector17;
			Quaternion quaternion8;
			this.CalcPieceLocalPosAndRot(builderPiece4.transform.position, builderPiece4.transform.rotation, handAttachPoint, out vector17, out quaternion8);
			if (BuilderPiece.IsDroppedState(builderPiece4.state))
			{
				this.currentTable.RequestGrabPiece(builderPiece4, handType == BuilderPieceInteractor.HandType.Left, this.heldInitialPos[(int)handType], this.heldInitialRot[(int)handType]);
				return;
			}
			Vector3 vector18 = this.heldInitialPos[(int)handType] - vector17;
			this.UpdatePullApartOffset((int)handType, builderPiece4, handAttachPoint.TransformVector(vector18));
			float num10 = this.currentTable.pushAndEaseParams.unSnapDelayDist * this.currentTable.pushAndEaseParams.unSnapDelayDist;
			if (vector18.sqrMagnitude > num10)
			{
				GorillaTagger.Instance.StartVibration(handType == BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength * 0.15f, this.currentTable.pushAndEaseParams.unSnapDelayTime * 2f);
				if (((builderPiece4 == null) ? BuilderPiece.State.None : builderPiece4.state) == BuilderPiece.State.GrabbedLocal)
				{
					GorillaTagger.Instance.StartVibration(handType > BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength * 0.15f, this.currentTable.pushAndEaseParams.unSnapDelayTime * 2f);
				}
				this.SetHandState((int)handType, BuilderPieceInteractor.HandState.WaitingForUnSnap);
				this.delayedPlacementTime[(int)handType] = 0f;
				return;
			}
			break;
		}
		case BuilderPieceInteractor.HandState.WaitForGrabbed:
			break;
		case BuilderPieceInteractor.HandState.WaitingForSnap:
		{
			BuilderPiece builderPiece5 = this.heldPiece[(int)handType];
			if (builderPiece5 != null)
			{
				builderPiece5.transform.localRotation = this.heldInitialRot[(int)handType];
				builderPiece5.transform.localPosition = this.heldInitialPos[(int)handType];
				Quaternion quaternion9 = this.heldCurrentRot[(int)handType];
				Vector3 vector19 = this.heldCurrentPos[(int)handType];
				if (this.delayedPlacementTime[(int)handType] >= 0f)
				{
					BuilderPotentialPlacement builderPotentialPlacement3 = this.delayedPotentialPlacement[(int)handType];
					if (this.delayedPlacementTime[(int)handType] > this.currentTable.pushAndEaseParams.snapDelayTime)
					{
						BuilderAttachGridPlane builderAttachGridPlane4 = builderPotentialPlacement3.attachPiece.gridPlanes[builderPotentialPlacement3.attachIndex];
						BuilderAttachGridPlane builderAttachGridPlane5 = (builderPotentialPlacement3.parentPiece == null) ? null : builderPotentialPlacement3.parentPiece.gridPlanes[builderPotentialPlacement3.parentAttachIndex];
						bool flag15 = builderAttachGridPlane4.IsConnected(builderPotentialPlacement3.attachBounds);
						if (!flag15)
						{
							flag15 = builderAttachGridPlane5.IsConnected(builderPotentialPlacement3.parentAttachBounds);
						}
						if (flag15)
						{
							Debug.LogError("Snap Overlapping Why are we doing this!!??");
						}
						if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPiece5, builderPotentialPlacement3.parentPiece))
						{
							this.SetHandState((int)handType, BuilderPieceInteractor.HandState.Grabbed);
						}
						this.currentTable.RequestPlacePiece(builderPiece5, builderPotentialPlacement3.attachPiece, builderPotentialPlacement3.bumpOffsetX, builderPotentialPlacement3.bumpOffsetZ, builderPotentialPlacement3.twist, builderPotentialPlacement3.parentPiece, builderPotentialPlacement3.attachIndex, builderPotentialPlacement3.parentAttachIndex);
						return;
					}
					this.delayedPlacementTime[(int)handType] = this.delayedPlacementTime[(int)handType] + Time.deltaTime;
					Transform parent = builderPiece5.transform.parent;
					Vector3 vector20 = builderPotentialPlacement3.localPosition;
					Quaternion quaternion10 = builderPotentialPlacement3.localRotation;
					Vector3 vector21 = builderPotentialPlacement3.attachPlaneNormal;
					if (builderPotentialPlacement3.parentPiece != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane6 = builderPotentialPlacement3.parentPiece.gridPlanes[builderPotentialPlacement3.parentAttachIndex];
						vector20 = builderAttachGridPlane6.transform.TransformPoint(builderPotentialPlacement3.localPosition);
						quaternion10 = builderAttachGridPlane6.transform.rotation * builderPotentialPlacement3.localRotation;
						vector21 = builderAttachGridPlane6.transform.TransformDirection(builderPotentialPlacement3.attachPlaneNormal);
					}
					Vector3 vector22 = parent.transform.InverseTransformPoint(vector20);
					Quaternion quaternion11 = Quaternion.Inverse(parent.transform.rotation) * quaternion10;
					bool flag16 = builderPotentialPlacement3.attachPiece == builderPiece5;
					bool flag17 = builderPotentialPlacement3.attachPiece == builderPiece5;
					if (flag16)
					{
						quaternion9 = quaternion11;
					}
					if (flag17)
					{
						Vector3 vector23 = parent.transform.InverseTransformDirection(vector21);
						vector19 = vector22 + vector23 * this.currentTable.pushAndEaseParams.snapDelayOffsetDist;
					}
					this.heldCurrentRot[(int)handType] = quaternion9;
					this.heldCurrentPos[(int)handType] = vector19;
					builderPiece5.transform.localRotation = quaternion9;
					builderPiece5.transform.localPosition = vector19;
				}
			}
			break;
		}
		case BuilderPieceInteractor.HandState.WaitingForUnSnap:
		{
			BuilderPiece builderPiece6 = this.heldPiece[(int)handType];
			if (BuilderPiece.IsDroppedState(builderPiece6.state))
			{
				this.currentTable.RequestGrabPiece(builderPiece6, handType == BuilderPieceInteractor.HandType.Left, this.heldInitialPos[(int)handType], this.heldInitialRot[(int)handType]);
				return;
			}
			if (this.delayedPlacementTime[(int)handType] <= this.currentTable.pushAndEaseParams.unSnapDelayTime)
			{
				Vector3 vector24;
				Quaternion quaternion12;
				this.CalcPieceLocalPosAndRot(builderPiece6.transform.position, builderPiece6.transform.rotation, handAttachPoint, out vector24, out quaternion12);
				Vector3 vector25 = this.heldInitialPos[(int)handType] - vector24;
				this.UpdatePullApartOffset((int)handType, builderPiece6, handAttachPoint.TransformVector(vector25));
				this.delayedPlacementTime[(int)handType] = this.delayedPlacementTime[(int)handType] + Time.deltaTime;
				return;
			}
			if (builderPiece6.GetChildCount() > this.maxHoldablePieceStackCount)
			{
				builderPiece6.PlayTooHeavyFx();
				this.ClearUnSnapOffset((int)handType, builderPiece6);
				this.RemovePieceFromHand(builderPiece6, (int)handType);
				return;
			}
			this.currentTable.RequestGrabPiece(builderPiece6, handType == BuilderPieceInteractor.HandType.Left, this.heldInitialPos[(int)handType], this.heldInitialRot[(int)handType]);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x000BDE74 File Offset: 0x000BC074
	private void ClearGlowBumps(int handIndex)
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		BuilderPool builderPool = builderTable.builderPool;
		List<BuilderBumpGlow> list = this.glowBumps[handIndex];
		if (builderPool != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				builderPool.DestroyBumpGlow(list[i]);
			}
		}
		else
		{
			Debug.LogError("BuilderPieceInteractor could not find Builderpool");
		}
		list.Clear();
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000BDEEC File Offset: 0x000BC0EC
	private void AddGlowBumps(int handIndex, List<BuilderPotentialPlacement> allPotentialPlacements)
	{
		this.ClearGlowBumps(handIndex);
		if (allPotentialPlacements == null)
		{
			Debug.LogError("How is allPotentialPlacements null");
			return;
		}
		BuilderPool builderPool = this.currentTable.builderPool;
		if (builderPool == null)
		{
			Debug.LogError("How is the pool null?");
			return;
		}
		float gridSize = this.currentTable.gridSize;
		for (int i = 0; i < allPotentialPlacements.Count; i++)
		{
			BuilderPotentialPlacement builderPotentialPlacement = allPotentialPlacements[i];
			if (builderPotentialPlacement.parentPiece != null && builderPotentialPlacement.attachPiece != null && builderPotentialPlacement.attachPiece.gridPlanes != null && builderPotentialPlacement.parentPiece.gridPlanes != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				if (builderAttachGridPlane != null)
				{
					Vector2Int min = builderPotentialPlacement.parentAttachBounds.min;
					Vector2Int max = builderPotentialPlacement.parentAttachBounds.max;
					for (int j = min.x; j <= max.x; j++)
					{
						for (int k = min.y; k <= max.y; k++)
						{
							Vector3 gridPosition = builderAttachGridPlane.GetGridPosition(j, k, gridSize);
							BuilderBumpGlow builderBumpGlow = builderPool.CreateGlowBump();
							if (builderBumpGlow != null)
							{
								builderBumpGlow.transform.SetPositionAndRotation(gridPosition, builderAttachGridPlane.transform.rotation);
								builderBumpGlow.transform.SetParent(builderAttachGridPlane.transform, true);
								builderBumpGlow.transform.localScale = Vector3.one;
								builderBumpGlow.gameObject.SetActive(true);
								this.glowBumps[handIndex].Add(builderBumpGlow);
							}
						}
					}
				}
				BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex];
				if (builderAttachGridPlane2 != null)
				{
					Vector2Int min2 = builderPotentialPlacement.attachBounds.min;
					Vector2Int max2 = builderPotentialPlacement.attachBounds.max;
					for (int l = min2.x; l <= max2.x; l++)
					{
						for (int m = min2.y; m <= max2.y; m++)
						{
							Vector3 gridPosition2 = builderAttachGridPlane2.GetGridPosition(l, m, gridSize);
							BuilderBumpGlow builderBumpGlow2 = builderPool.CreateGlowBump();
							if (builderBumpGlow2 != null)
							{
								Quaternion quaternion = Quaternion.Euler(180f, 0f, 0f);
								builderBumpGlow2.transform.SetPositionAndRotation(gridPosition2, builderAttachGridPlane2.transform.rotation * quaternion);
								builderBumpGlow2.transform.SetParent(builderAttachGridPlane2.transform, true);
								builderBumpGlow2.transform.localScale = Vector3.one;
								builderBumpGlow2.gameObject.SetActive(true);
								this.glowBumps[handIndex].Add(builderBumpGlow2);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000BE1BC File Offset: 0x000BC3BC
	private void UpdateGlowBumps(int handIndex, float intensity)
	{
		List<BuilderBumpGlow> list = this.glowBumps[handIndex];
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SetIntensity(intensity);
		}
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000BE1F4 File Offset: 0x000BC3F4
	private void UpdatePullApartOffset(int handIndex, BuilderPiece potentialGrabPiece, Vector3 pullApartDiff)
	{
		BuilderPiece parentPiece = potentialGrabPiece.parentPiece;
		BuilderAttachGridPlane builderAttachGridPlane = null;
		if (parentPiece != null)
		{
			builderAttachGridPlane = parentPiece.gridPlanes[potentialGrabPiece.parentAttachIndex];
		}
		Vector3 vector = Vector3.up;
		if (builderAttachGridPlane != null)
		{
			vector = builderAttachGridPlane.transform.TransformDirection(vector);
			if (!builderAttachGridPlane.male)
			{
				vector *= -1f;
			}
		}
		float num = Vector3.Dot(pullApartDiff, vector);
		num = Mathf.Max(num, 0f);
		float num2 = 0.0025f;
		float num3 = num / num2;
		num3 = 1f - 1f / (1f + num3);
		num = num3 * num2;
		Vector3 vector2 = vector * this.potentialGrabbedOffsetDist[handIndex];
		this.potentialGrabbedOffsetDist[handIndex] = num;
		Vector3 vector3 = vector * this.potentialGrabbedOffsetDist[handIndex];
		if (builderAttachGridPlane != null)
		{
			vector2 = builderAttachGridPlane.transform.InverseTransformVector(vector2);
			vector3 = builderAttachGridPlane.transform.InverseTransformVector(vector3);
		}
		Vector3 vector4 = potentialGrabPiece.transform.localPosition - vector2;
		potentialGrabPiece.transform.localPosition = vector4 + vector3;
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000BE316 File Offset: 0x000BC516
	private void ClearUnSnapOffset(int handIndex, BuilderPiece potentialGrabPiece)
	{
		this.UpdatePullApartOffset(handIndex, potentialGrabPiece, Vector3.zero);
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000BE328 File Offset: 0x000BC528
	public void AddPieceToHeld(BuilderPiece piece, bool isLeft, Vector3 localPosition, Quaternion localRotation)
	{
		int num = isLeft ? 0 : 1;
		this.AddPieceToHand(piece, num, localPosition, localRotation);
		int num2 = (num + 1) % 2;
		if (this.heldPiece[num2] == piece)
		{
			this.RemovePieceFromHand(piece, num2);
		}
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000BE36C File Offset: 0x000BC56C
	public void RemovePieceFromHeld(BuilderPiece piece)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.heldPiece[i] == piece)
			{
				this.RemovePieceFromHand(piece, i);
			}
		}
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x000BE3A4 File Offset: 0x000BC5A4
	private void AddPieceToHand(BuilderPiece piece, int handIndex, Vector3 localPosition, Quaternion localRotation)
	{
		this.heldPiece[handIndex] = piece;
		this.delayedPlacementTime[handIndex] = -1f;
		this.SetHandState(handIndex, BuilderPieceInteractor.HandState.Grabbed);
		this.heldInitialRot[handIndex] = localRotation;
		this.heldCurrentRot[handIndex] = localRotation;
		this.heldInitialPos[handIndex] = localPosition;
		this.heldCurrentPos[handIndex] = localPosition;
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x000BE40D File Offset: 0x000BC60D
	private void RemovePieceFromHand(BuilderPiece piece, int handIndex)
	{
		this.heldPiece[handIndex] = null;
		this.delayedPlacementTime[handIndex] = -1f;
		this.SetHandState(handIndex, BuilderPieceInteractor.HandState.Empty);
		this.ClearGlowBumps(handIndex);
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000BE43C File Offset: 0x000BC63C
	public void RemovePiecesFromHands()
	{
		for (int i = 0; i < 2; i++)
		{
			this.heldPiece[i] = null;
			this.delayedPlacementTime[i] = -1f;
			this.SetHandState(i, BuilderPieceInteractor.HandState.Empty);
			this.ClearGlowBumps(i);
		}
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000BE484 File Offset: 0x000BC684
	private void CalcPieceLocalPosAndRot(Vector3 worldPosition, Quaternion worldRotation, Transform attachPoint, out Vector3 localPosition, out Quaternion localRotation)
	{
		Quaternion rotation = attachPoint.transform.rotation;
		Vector3 position = attachPoint.transform.position;
		localRotation = Quaternion.Inverse(rotation) * worldRotation;
		localPosition = Quaternion.Inverse(rotation) * (worldPosition - position);
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000BE4D9 File Offset: 0x000BC6D9
	public void DisableCollisionsWithHands()
	{
		this.DisableCollisionsWithHand(true);
		this.DisableCollisionsWithHand(false);
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000BE4EC File Offset: 0x000BC6EC
	private void DisableCollisionsWithHand(bool leftHand)
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		Transform transform = leftHand ? offlineVRRig.leftHand.overrideTarget : offlineVRRig.rightHand.overrideTarget;
		List<GameObject> list = leftHand ? this.collisionDisabledPiecesLeft : this.collisionDisabledPiecesRight;
		int num = Physics.OverlapSphereNonAlloc(transform.position, 0.15f, BuilderPieceInteractor.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = BuilderPieceInteractor.tempDisableColliders[i].gameObject;
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(BuilderPieceInteractor.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !list.Contains(gameObject))
			{
				gameObject.layer = BuilderTable.heldLayer;
				list.Add(gameObject);
			}
		}
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000BE5C5 File Offset: 0x000BC7C5
	public void UpdatePieceDisables()
	{
		this.UpdatePieceDisablesForHand(true);
		this.UpdatePieceDisablesForHand(false);
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000BE5D8 File Offset: 0x000BC7D8
	public void UpdatePieceDisablesForHand(bool leftHand)
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		Transform transform = leftHand ? offlineVRRig.leftHand.overrideTarget : offlineVRRig.rightHand.overrideTarget;
		List<GameObject> list = leftHand ? this.collisionDisabledPiecesLeft : this.collisionDisabledPiecesRight;
		List<GameObject> list2 = (!leftHand) ? this.collisionDisabledPiecesLeft : this.collisionDisabledPiecesRight;
		Vector3 position = transform.position;
		float num = 0.040000003f;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i];
			if (gameObject == null)
			{
				list.RemoveAt(i);
				i--;
			}
			else if ((gameObject.transform.position - position).sqrMagnitude > num)
			{
				BuilderPiece builderPieceFromTransform = BuilderPiece.GetBuilderPieceFromTransform(gameObject.transform);
				if (builderPieceFromTransform.state == BuilderPiece.State.AttachedAndPlaced && !list2.Contains(gameObject))
				{
					builderPieceFromTransform.SetColliderLayers<Collider>(builderPieceFromTransform.colliders, BuilderTable.placedLayer);
				}
				list.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x04002E9D RID: 11933
	[OnEnterPlay_SetNull]
	public static volatile BuilderPieceInteractor instance;

	// Token: 0x04002E9E RID: 11934
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04002E9F RID: 11935
	public EquipmentInteractor equipmentInteractor;

	// Token: 0x04002EA0 RID: 11936
	private const int NUM_HANDS = 2;

	// Token: 0x04002EA1 RID: 11937
	public GorillaVelocityEstimator velocityEstimatorLeft;

	// Token: 0x04002EA2 RID: 11938
	public GorillaVelocityEstimator velocityEstimatorRight;

	// Token: 0x04002EA3 RID: 11939
	public BuilderLaserSight laserSightLeft;

	// Token: 0x04002EA4 RID: 11940
	public BuilderLaserSight laserSightRight;

	// Token: 0x04002EA5 RID: 11941
	public int maxHoldablePieceStackCount = 50;

	// Token: 0x04002EA6 RID: 11942
	public List<GorillaVelocityEstimator> velocityEstimator;

	// Token: 0x04002EA7 RID: 11943
	public List<BuilderPieceInteractor.HandState> handState;

	// Token: 0x04002EA8 RID: 11944
	public List<BuilderPiece> heldPiece;

	// Token: 0x04002EA9 RID: 11945
	public List<BuilderPiece> potentialHeldPiece;

	// Token: 0x04002EAA RID: 11946
	public List<float> potentialGrabbedOffsetDist;

	// Token: 0x04002EAB RID: 11947
	public List<Quaternion> heldInitialRot;

	// Token: 0x04002EAC RID: 11948
	public List<Quaternion> heldCurrentRot;

	// Token: 0x04002EAD RID: 11949
	public List<Vector3> heldInitialPos;

	// Token: 0x04002EAE RID: 11950
	public List<Vector3> heldCurrentPos;

	// Token: 0x04002EAF RID: 11951
	public List<BuilderPotentialPlacement> delayedPotentialPlacement;

	// Token: 0x04002EB0 RID: 11952
	public List<float> delayedPlacementTime;

	// Token: 0x04002EB1 RID: 11953
	public List<BuilderPotentialPlacement> prevPotentialPlacement;

	// Token: 0x04002EB2 RID: 11954
	public List<BuilderLaserSight> laserSight;

	// Token: 0x04002EB3 RID: 11955
	public int[] heldChainLength;

	// Token: 0x04002EB4 RID: 11956
	public List<int[]> heldChainCost;

	// Token: 0x04002EB5 RID: 11957
	private static List<BuilderPotentialPlacement>[] allPotentialPlacements;

	// Token: 0x04002EB6 RID: 11958
	private static NativeList<BuilderGridPlaneData>[] handGridPlaneData;

	// Token: 0x04002EB7 RID: 11959
	private static NativeList<BuilderPieceData>[] handPieceData;

	// Token: 0x04002EB8 RID: 11960
	private static NativeList<BuilderGridPlaneData>[] localAttachableGridPlaneData;

	// Token: 0x04002EB9 RID: 11961
	private static NativeList<BuilderPieceData>[] localAttachablePieceData;

	// Token: 0x04002EBA RID: 11962
	private JobHandle findNearbyJobHandle;

	// Token: 0x04002EBB RID: 11963
	public List<GameObject> collisionDisabledPiecesLeft = new List<GameObject>();

	// Token: 0x04002EBC RID: 11964
	public List<GameObject> collisionDisabledPiecesRight = new List<GameObject>();

	// Token: 0x04002EBD RID: 11965
	public const int MAX_SPHERE_CHECK_RESULTS = 1024;

	// Token: 0x04002EBE RID: 11966
	public NativeArray<OverlapSphereCommand> checkPiecesInSphere;

	// Token: 0x04002EBF RID: 11967
	public NativeArray<ColliderHit> checkPiecesInSphereResults;

	// Token: 0x04002EC0 RID: 11968
	public JobHandle checkNearbyPiecesHandle;

	// Token: 0x04002EC1 RID: 11969
	public const float GRAB_CAST_RADIUS = 0.0375f;

	// Token: 0x04002EC2 RID: 11970
	public const int MAX_GRAB_CAST_RESULTS = 64;

	// Token: 0x04002EC3 RID: 11971
	public NativeArray<SpherecastCommand> grabSphereCast;

	// Token: 0x04002EC4 RID: 11972
	public NativeArray<RaycastHit> grabSphereCastResults;

	// Token: 0x04002EC5 RID: 11973
	public JobHandle findPiecesToGrab;

	// Token: 0x04002EC6 RID: 11974
	private RaycastHit emptyRaycastHit;

	// Token: 0x04002EC7 RID: 11975
	public BuilderBumpGlow glowBumpPrefab;

	// Token: 0x04002EC8 RID: 11976
	public List<List<BuilderBumpGlow>> glowBumps;

	// Token: 0x04002EC9 RID: 11977
	private const int MAX_GRID_PLANES = 8192;

	// Token: 0x04002ECA RID: 11978
	private bool isRigSmall;

	// Token: 0x04002ECB RID: 11979
	private BuilderTable currentTable;

	// Token: 0x04002ECC RID: 11980
	private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

	// Token: 0x04002ECD RID: 11981
	private static RaycastHit[] tempHitResults = new RaycastHit[64];

	// Token: 0x04002ECE RID: 11982
	private const float PIECE_DISTANCE_DISABLE = 0.15f;

	// Token: 0x04002ECF RID: 11983
	private const float PIECE_DISTANCE_ENABLE = 0.2f;

	// Token: 0x04002ED0 RID: 11984
	private static Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x02000587 RID: 1415
	public enum HandType
	{
		// Token: 0x04002ED2 RID: 11986
		Invalid = -1,
		// Token: 0x04002ED3 RID: 11987
		Left,
		// Token: 0x04002ED4 RID: 11988
		Right
	}

	// Token: 0x02000588 RID: 1416
	public enum HandState
	{
		// Token: 0x04002ED6 RID: 11990
		Invalid = -1,
		// Token: 0x04002ED7 RID: 11991
		Empty,
		// Token: 0x04002ED8 RID: 11992
		Grabbed,
		// Token: 0x04002ED9 RID: 11993
		PotentialGrabbed,
		// Token: 0x04002EDA RID: 11994
		WaitForGrabbed,
		// Token: 0x04002EDB RID: 11995
		WaitingForSnap,
		// Token: 0x04002EDC RID: 11996
		WaitingForUnSnap
	}
}
