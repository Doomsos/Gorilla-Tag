using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000583 RID: 1411
public class BuilderPiece : MonoBehaviour
{
	// Token: 0x06002377 RID: 9079 RVA: 0x000B9A9C File Offset: 0x000B7C9C
	private void Awake()
	{
		if (this.fXInfo == null)
		{
			Debug.LogErrorFormat("BuilderPiece {0} is missing Effect Info", new object[]
			{
				base.gameObject.name
			});
		}
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.placedOnlyColliders = new List<Collider>(4);
		List<Collider> list = new List<Collider>(4);
		foreach (GameObject gameObject in this.onlyWhenPlaced)
		{
			list.Clear();
			gameObject.GetComponentsInChildren<Collider>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isTrigger)
				{
					BuilderPieceCollider builderPieceCollider = list[i].GetComponent<BuilderPieceCollider>();
					if (builderPieceCollider == null)
					{
						builderPieceCollider = list[i].AddComponent<BuilderPieceCollider>();
					}
					builderPieceCollider.piece = this;
					this.placedOnlyColliders.Add(list[i]);
				}
			}
		}
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		for (int j = this.colliders.Count - 1; j >= 0; j--)
		{
			if (this.colliders[j].isTrigger)
			{
				this.colliders.RemoveAt(j);
			}
			else
			{
				BuilderPieceCollider builderPieceCollider2 = this.colliders[j].GetComponent<BuilderPieceCollider>();
				if (builderPieceCollider2 == null)
				{
					builderPieceCollider2 = this.colliders[j].AddComponent<BuilderPieceCollider>();
				}
				builderPieceCollider2.piece = this;
			}
		}
		this.gridPlanes = new List<BuilderAttachGridPlane>(8);
		base.GetComponentsInChildren<BuilderAttachGridPlane>(this.gridPlanes);
		this.pieceComponents = new List<IBuilderPieceComponent>(1);
		base.GetComponentsInChildren<IBuilderPieceComponent>(true, this.pieceComponents);
		this.pieceComponentsActive = false;
		this.functionalPieceComponent = base.GetComponentInChildren<IBuilderPieceFunctional>(true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		this.renderingIndirect = new List<MeshRenderer>(4);
		this.renderingDirect = new List<MeshRenderer>(4);
		this.FindActiveRenderers();
		this.paintingCount = 0;
		this.potentialGrabCount = 0;
		this.potentialGrabChildCount = 0;
		this.isPrivatePlot = (this.plotComponent != null);
		this.privatePlotIndex = -1;
		this.ClearCollisionHistory();
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000B9D58 File Offset: 0x000B7F58
	public void SetTable(BuilderTable table)
	{
		this.tableOwner = table;
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000B9D61 File Offset: 0x000B7F61
	public BuilderTable GetTable()
	{
		return this.tableOwner;
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000B9D6C File Offset: 0x000B7F6C
	public void OnReturnToPool()
	{
		this.tableOwner.builderRenderer.RemovePiece(this);
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceDestroy();
		}
		this.functionalPieceState = 0;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.overrideSavedPiece = false;
		this.savedMaterialType = -1;
		this.savedPieceType = -1;
		this.shelfOwner = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.activatedTimeStamp = 0;
		this.forcedFrozen = false;
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		base.transform.localScale = Vector3.one;
		if (this.isArmShelf)
		{
			if (this.armShelf != null)
			{
				this.armShelf.piece = null;
			}
			this.armShelf = null;
		}
		for (int j = 0; j < this.gridPlanes.Count; j++)
		{
			this.gridPlanes[j].OnReturnToPool(this.tableOwner.builderPool);
		}
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000B9EEE File Offset: 0x000B80EE
	public void OnCreatedByPool()
	{
		this.materialSwapTargets = new List<MeshRenderer>(4);
		base.GetComponentsInChildren<MeshRenderer>(this.areMeshesToggledOnPlace, this.materialSwapTargets);
		this.surfaceOverrides = new List<GorillaSurfaceOverride>(4);
		base.GetComponentsInChildren<GorillaSurfaceOverride>(this.areMeshesToggledOnPlace, this.surfaceOverrides);
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x000B9F2C File Offset: 0x000B812C
	public void SetupPiece(float gridSize)
	{
		for (int i = 0; i < this.gridPlanes.Count; i++)
		{
			this.gridPlanes[i].Setup(this, i, gridSize);
		}
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000B9F64 File Offset: 0x000B8164
	public void SetMaterial(int inMaterialType, bool force = false)
	{
		if (this.materialOptions == null || this.materialSwapTargets == null || this.materialSwapTargets.Count < 1)
		{
			return;
		}
		if (this.materialType == inMaterialType && !force)
		{
			return;
		}
		this.materialType = inMaterialType;
		Material material = null;
		int num = -1;
		if (inMaterialType == -1)
		{
			this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
		}
		else
		{
			this.materialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material == null)
			{
				this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
			}
		}
		if (material == null)
		{
			Debug.LogErrorFormat("Piece {0} has no material matching Type {1}", new object[]
			{
				this.GetPieceId(),
				inMaterialType
			});
			return;
		}
		foreach (MeshRenderer meshRenderer in this.materialSwapTargets)
		{
			if (!(meshRenderer == null) && meshRenderer.enabled)
			{
				meshRenderer.material = material;
			}
		}
		if (this.surfaceOverrides != null && num != -1)
		{
			foreach (GorillaSurfaceOverride gorillaSurfaceOverride in this.surfaceOverrides)
			{
				gorillaSurfaceOverride.overrideIndex = num;
			}
		}
		if (this.renderingIndirect.Count > 0)
		{
			this.tableOwner.builderRenderer.ChangePieceIndirectMaterial(this, this.materialSwapTargets, material);
		}
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000BA0F8 File Offset: 0x000B82F8
	public int GetPieceId()
	{
		return this.pieceId;
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000BA100 File Offset: 0x000B8300
	public int GetParentPieceId()
	{
		if (!(this.parentPiece == null))
		{
			return this.parentPiece.pieceId;
		}
		return -1;
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000BA11D File Offset: 0x000B831D
	public int GetAttachIndex()
	{
		return this.attachIndex;
	}

	// Token: 0x06002381 RID: 9089 RVA: 0x000BA125 File Offset: 0x000B8325
	public int GetParentAttachIndex()
	{
		return this.parentAttachIndex;
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x000BA130 File Offset: 0x000B8330
	private void SetPieceActive(List<IBuilderPieceComponent> components, bool active)
	{
		if (components == null || active == this.pieceComponentsActive)
		{
			return;
		}
		this.pieceComponentsActive = active;
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				if (active)
				{
					components[i].OnPieceActivate();
				}
				else
				{
					components[i].OnPieceDeactivate();
				}
			}
		}
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000BA188 File Offset: 0x000B8388
	private void SetBehavioursEnabled<T>(List<T> components, bool enabled) where T : Behaviour
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000BA1D0 File Offset: 0x000B83D0
	private void SetCollidersEnabled<T>(List<T> components, bool enabled) where T : Collider
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000BA218 File Offset: 0x000B8418
	public void SetColliderLayers<T>(List<T> components, int layer) where T : Collider
	{
		this.currentColliderLayer = layer;
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].gameObject.layer = layer;
			}
		}
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000BA26C File Offset: 0x000B846C
	private void SetActive(List<GameObject> gameObjects, bool active)
	{
		if (gameObjects == null)
		{
			return;
		}
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] != null)
			{
				gameObjects[i].SetActive(active);
			}
		}
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000BA2AA File Offset: 0x000B84AA
	public void SetFunctionalPieceState(byte fState, NetPlayer instigator, int timeStamp)
	{
		if (this.functionalPieceComponent == null || !this.functionalPieceComponent.IsStateValid(fState))
		{
			fState = 0;
		}
		this.functionalPieceState = fState;
		IBuilderPieceFunctional builderPieceFunctional = this.functionalPieceComponent;
		if (builderPieceFunctional == null)
		{
			return;
		}
		builderPieceFunctional.OnStateChanged(fState, instigator, timeStamp);
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000BA2DF File Offset: 0x000B84DF
	public void SetScale(float scale)
	{
		if (this.scaleRoot != null)
		{
			this.scaleRoot.localScale = Vector3.one * scale;
		}
		this.pieceScale = scale;
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000BA30C File Offset: 0x000B850C
	public float GetScale()
	{
		return this.pieceScale;
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000BA314 File Offset: 0x000B8514
	public void PaintingTint(bool enable)
	{
		if (enable)
		{
			this.paintingCount++;
			if (this.paintingCount == 1)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.paintingCount--;
			if (this.paintingCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x0600238B RID: 9099 RVA: 0x000BA354 File Offset: 0x000B8554
	public void PotentialGrab(bool enable)
	{
		if (enable)
		{
			this.potentialGrabCount++;
			if (this.potentialGrabCount == 1 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.potentialGrabCount--;
			if (this.potentialGrabCount == 0 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000BA3B0 File Offset: 0x000B85B0
	public static void PotentialGrabChildren(BuilderPiece piece, bool enable)
	{
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			if (enable)
			{
				builderPiece.potentialGrabChildCount++;
				if (builderPiece.potentialGrabChildCount == 1 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			else
			{
				builderPiece.potentialGrabChildCount--;
				if (builderPiece.potentialGrabChildCount == 0 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			BuilderPiece.PotentialGrabChildren(builderPiece, enable);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x000BA42C File Offset: 0x000B862C
	private void RefreshTint()
	{
		if (this.potentialGrabCount > 0 || this.potentialGrabChildCount > 0)
		{
			this.SetTint(this.tableOwner.potentialGrabTint);
			return;
		}
		if (this.paintingCount > 0)
		{
			this.SetTint(this.tableOwner.paintingTint);
			return;
		}
		switch (this.state)
		{
		case BuilderPiece.State.AttachedToDropped:
		case BuilderPiece.State.Dropped:
			this.SetTint(this.tableOwner.droppedTint);
			return;
		case BuilderPiece.State.Grabbed:
		case BuilderPiece.State.GrabbedLocal:
		case BuilderPiece.State.AttachedToArm:
			this.SetTint(this.tableOwner.grabbedTint);
			return;
		case BuilderPiece.State.OnShelf:
		case BuilderPiece.State.OnConveyor:
			this.SetTint(this.tableOwner.shelfTint);
			return;
		}
		this.SetTint(this.tableOwner.defaultTint);
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000BA4F0 File Offset: 0x000B86F0
	private void SetTint(float tint)
	{
		if (tint == this.tint)
		{
			return;
		}
		this.tint = tint;
		this.tableOwner.builderRenderer.SetPieceTint(this, tint);
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000BA518 File Offset: 0x000B8718
	public void SetParentPiece(int newAttachIndex, BuilderPiece newParentPiece, int newParentAttachIndex)
	{
		if (this.parentHeld != null)
		{
			Debug.LogErrorFormat(newParentPiece.gameObject, "Cannot attach to piece {0} while already held", new object[]
			{
				(newParentPiece == null) ? null : newParentPiece.gameObject.name
			});
			return;
		}
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = newAttachIndex;
		this.parentPiece = newParentPiece;
		this.parentAttachIndex = newParentAttachIndex;
		this.AddPieceToParent(this);
		Transform transform = null;
		if (newParentPiece != null)
		{
			if (newParentAttachIndex >= 0)
			{
				transform = newParentPiece.gridPlanes[newParentAttachIndex].transform;
			}
			else
			{
				transform = newParentPiece.transform;
			}
		}
		base.transform.SetParent(transform, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000BA5D0 File Offset: 0x000B87D0
	public void ClearParentPiece(bool ignoreSnaps = false)
	{
		if (this.parentPiece == null)
		{
			if (!ignoreSnaps)
			{
				BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this, this.tableOwner.builderPool);
			}
			return;
		}
		BuilderPiece builderPiece = this.parentPiece;
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = -1;
		this.parentPiece = null;
		this.parentAttachIndex = -1;
		base.transform.SetParent(null, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
		if (!ignoreSnaps)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this.GetRootPiece(), this.tableOwner.builderPool);
		}
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000BA660 File Offset: 0x000B8860
	public static void RemoveOverlapsWithDifferentPieceRoot(BuilderPiece piece, BuilderPiece root, BuilderPool pool)
	{
		for (int i = 0; i < piece.gridPlanes.Count; i++)
		{
			piece.gridPlanes[i].RemoveSnapsWithDifferentRoot(root, pool);
		}
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(builderPiece, root, pool);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000BA6B8 File Offset: 0x000B88B8
	private void AddPieceToParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		this.nextSiblingPiece = builderPiece.firstChildPiece;
		builderPiece.firstChildPiece = piece;
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(1 + piece.GetChildCount());
		}
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000BA724 File Offset: 0x000B8924
	private static void RemovePieceFromParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		BuilderPiece builderPiece2 = builderPiece.firstChildPiece;
		if (builderPiece2 == null)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have any children", new object[]
			{
				builderPiece.name,
				piece.name
			});
		}
		bool flag = false;
		if (builderPiece2 == piece)
		{
			builderPiece.firstChildPiece = builderPiece2.nextSiblingPiece;
			flag = true;
		}
		else
		{
			while (builderPiece2 != null)
			{
				if (builderPiece2.nextSiblingPiece == piece)
				{
					builderPiece2.nextSiblingPiece = piece.nextSiblingPiece;
					piece.nextSiblingPiece = null;
					flag = true;
					break;
				}
				builderPiece2 = builderPiece2.nextSiblingPiece;
			}
		}
		if (!flag)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have the piece a child", new object[]
			{
				builderPiece.name,
				piece.name
			});
			return;
		}
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(-1 * (1 + piece.GetChildCount()));
		}
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000BA828 File Offset: 0x000B8A28
	public void SetParentHeld(Transform parentHeld, int heldByPlayerActorNumber, bool heldInLeftHand)
	{
		if (this.parentPiece != null)
		{
			Debug.LogErrorFormat(this.parentPiece.gameObject, "Cannot hold while already attached to piece {0}", new object[]
			{
				this.parentPiece.gameObject.name
			});
			return;
		}
		this.heldByPlayerActorNumber = heldByPlayerActorNumber;
		this.parentHeld = parentHeld;
		this.heldInLeftHand = heldInLeftHand;
		base.transform.SetParent(parentHeld);
		this.tableOwner.UpdatePieceData(this);
		if (heldByPlayerActorNumber != -1)
		{
			this.OnGrabbedAsRoot();
			return;
		}
		this.OnReleasedAsRoot();
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000BA8B0 File Offset: 0x000B8AB0
	public void ClearParentHeld()
	{
		if (this.parentHeld == null)
		{
			return;
		}
		if (this.isArmShelf && this.armShelf != null)
		{
			this.armShelf.piece = null;
			this.armShelf = null;
		}
		this.heldByPlayerActorNumber = -1;
		this.parentHeld = null;
		this.heldInLeftHand = false;
		base.transform.SetParent(this.parentHeld);
		this.tableOwner.UpdatePieceData(this);
		this.OnReleasedAsRoot();
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000BA92D File Offset: 0x000B8B2D
	public bool IsHeldLocal()
	{
		return this.heldByPlayerActorNumber != -1 && this.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000BA94C File Offset: 0x000B8B4C
	public bool IsHeldBy(int actorNumber)
	{
		return actorNumber != -1 && this.heldByPlayerActorNumber == actorNumber;
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000BA95D File Offset: 0x000B8B5D
	public bool IsHeldInLeftHand()
	{
		return this.heldInLeftHand;
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000BA965 File Offset: 0x000B8B65
	public static bool IsDroppedState(BuilderPiece.State state)
	{
		return state == BuilderPiece.State.Dropped || state == BuilderPiece.State.AttachedToDropped || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.OnConveyor;
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000BA97C File Offset: 0x000B8B7C
	public void SetActivateTimeStamp(int timeStamp)
	{
		this.activatedTimeStamp = timeStamp;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetActivateTimeStamp(timeStamp);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000BA9B0 File Offset: 0x000B8BB0
	public void SetState(BuilderPiece.State newState, bool force = false)
	{
		if (newState == this.state && !force)
		{
			if (newState == BuilderPiece.State.Grabbed)
			{
				int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
				if (this.currentColliderLayer != expectedGrabCollisionLayer)
				{
					this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
					this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
				}
			}
			return;
		}
		if (newState == BuilderPiece.State.Dropped && this.state != BuilderPiece.State.Dropped)
		{
			this.tableOwner.AddPieceToDropList(this);
		}
		else if (this.state == BuilderPiece.State.Dropped && newState != BuilderPiece.State.Dropped)
		{
			this.tableOwner.RemovePieceFromDropList(this);
		}
		BuilderPiece.State state = this.state;
		this.state = newState;
		if (this.pieceDataIndex >= 0)
		{
			this.tableOwner.UpdatePieceData(this);
		}
		switch (this.state)
		{
		case BuilderPiece.State.None:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, false);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.None, force);
			this.tableOwner.builderRenderer.RemovePiece(this);
			this.isStatic = true;
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedAndPlaced:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, true);
			this.SetActive(this.onlyWhenPlaced, true);
			this.SetActive(this.onlyWhenNotPlaced, false);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.placedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedAndPlaced, force);
			this.SetStatic(false, force || this.areMeshesToggledOnPlace);
			this.SetPieceActive(this.pieceComponents, true);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToDropped:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Grabbed:
		{
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			int expectedGrabCollisionLayer2 = this.GetExpectedGrabCollisionLayer();
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer2);
			this.SetChildrenState(BuilderPiece.State.Grabbed, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		}
		case BuilderPiece.State.Dropped:
			this.ClearCollisionHistory();
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(false, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.OnShelf:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnShelf, force);
			this.SetStatic(true, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Displayed:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetChildrenState(BuilderPiece.State.Displayed, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.GrabbedLocal:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.GrabbedLocal, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		case BuilderPiece.State.OnConveyor:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnConveyor, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToArm:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.AttachedToArm, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000BAF44 File Offset: 0x000B9144
	public void OnGrabbedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.heldByPlayerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.listeningToHandLinks)
		{
			HandLink.OnHandLinkChanged = (Action)Delegate.Combine(HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = true;
		}
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000BAFA0 File Offset: 0x000B91A0
	public void OnReleasedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.listeningToHandLinks)
		{
			HandLink.OnHandLinkChanged = (Action)Delegate.Remove(HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = false;
		}
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000BAFDC File Offset: 0x000B91DC
	public void SetKinematic(bool kinematic, bool destroyImmediate = true)
	{
		if (kinematic && this.rigidBody != null)
		{
			if (destroyImmediate)
			{
				Object.DestroyImmediate(this.rigidBody);
				this.rigidBody = null;
			}
			else
			{
				Object.Destroy(this.rigidBody);
				this.rigidBody = null;
			}
		}
		else if (!kinematic && this.rigidBody == null)
		{
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			if (this.rigidBody != null)
			{
				Debug.LogErrorFormat("We should never already have a rigid body here {0} {1}", new object[]
				{
					this.pieceId,
					this.pieceType
				});
			}
			if (this.rigidBody == null)
			{
				this.rigidBody = base.gameObject.AddComponent<Rigidbody>();
			}
			if (this.rigidBody != null)
			{
				this.rigidBody.isKinematic = kinematic;
			}
		}
		if (this.rigidBody != null)
		{
			this.rigidBody.mass = 1f;
		}
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000BB0E4 File Offset: 0x000B92E4
	public void ClearCollisionHistory()
	{
		if (this.collisionEnterHistory == null)
		{
			this.collisionEnterHistory = new float[this.collisionEnterLimit];
		}
		for (int i = 0; i < this.collisionEnterLimit; i++)
		{
			this.collisionEnterHistory[i] = float.MinValue;
		}
		this.collidersEntered.Clear();
		this.oldCollisionTimeIndex = 0;
		this.forcedFrozen = false;
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x000BB144 File Offset: 0x000B9344
	private void OnCollisionEnter(Collision other)
	{
		if (this.state != BuilderPiece.State.Dropped || this.forcedFrozen)
		{
			return;
		}
		BuilderPieceCollider component = other.collider.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if ((piece.state == BuilderPiece.State.AttachedAndPlaced || piece.forcedFrozen) && !this.collidersEntered.Add(other.collider.GetInstanceID()))
			{
				if (this.collisionEnterHistory[this.oldCollisionTimeIndex] > Time.time)
				{
					this.tableOwner.FreezeDroppedPiece(this);
					return;
				}
				this.collisionEnterHistory[this.oldCollisionTimeIndex] = Time.time + this.collisionEnterCooldown;
				int num = this.oldCollisionTimeIndex + 1;
				this.oldCollisionTimeIndex = num;
				this.oldCollisionTimeIndex = num % this.collisionEnterLimit;
			}
		}
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x000BB204 File Offset: 0x000B9404
	public int GetExpectedGrabCollisionLayer()
	{
		if (this.heldByPlayerActorNumber != -1)
		{
			if (!GorillaTagger.Instance.offlineVRRig.IsInHandHoldChainWithOtherPlayer(this.heldByPlayerActorNumber))
			{
				return BuilderTable.heldLayer;
			}
			return BuilderTable.heldLayerLocal;
		}
		else
		{
			if (this.parentPiece != null)
			{
				return this.parentPiece.currentColliderLayer;
			}
			return BuilderTable.heldLayer;
		}
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000BB25C File Offset: 0x000B945C
	public void UpdateGrabbedPieceCollisionLayer()
	{
		int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
		if (this.currentColliderLayer != expectedGrabCollisionLayer)
		{
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
			this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
		}
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000BB290 File Offset: 0x000B9490
	private void SetChildrenCollisionLayer(int layer)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetColliderLayers<Collider>(builderPiece.colliders, layer);
			builderPiece.SetChildrenCollisionLayer(layer);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000BB2CC File Offset: 0x000B94CC
	public void SetStatic(bool isStatic, bool force = false)
	{
		isStatic = true;
		if (this.isStatic == isStatic && !force)
		{
			return;
		}
		this.SetDirectRenderersVisible(true);
		this.tableOwner.builderRenderer.RemovePiece(this);
		this.isStatic = isStatic;
		if (this.areMeshesToggledOnPlace)
		{
			this.FindActiveRenderers();
		}
		this.tableOwner.builderRenderer.AddPiece(this);
		this.SetDirectRenderersVisible(this.tableOwner.IsInBuilderZone());
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000BB338 File Offset: 0x000B9538
	private void FindActiveRenderers()
	{
		if (this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = true;
			}
		}
		this.renderingDirect.Clear();
		BuilderPiece.tempRenderers.Clear();
		base.GetComponentsInChildren<MeshRenderer>(false, BuilderPiece.tempRenderers);
		foreach (MeshRenderer meshRenderer2 in BuilderPiece.tempRenderers)
		{
			if (meshRenderer2.enabled)
			{
				this.renderingDirect.Add(meshRenderer2);
			}
		}
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000BB408 File Offset: 0x000B9608
	public void SetDirectRenderersVisible(bool visible)
	{
		if (this.renderingDirect != null && this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = visible;
			}
		}
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000BB470 File Offset: 0x000B9670
	private void SetChildrenState(BuilderPiece.State newState, bool force)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetState(newState, force);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000BB4A0 File Offset: 0x000B96A0
	public void OnCreate()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceCreate(this.pieceType, this.pieceId);
		}
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000BB4E0 File Offset: 0x000B96E0
	public void OnPlacementDeserialized()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPiecePlacementDeserialized();
		}
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000BB514 File Offset: 0x000B9714
	public void PlayPlacementFx()
	{
		this.PlayFX(this.fXInfo.placeVFX);
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000BB527 File Offset: 0x000B9727
	public void PlayDisconnectFx()
	{
		this.PlayFX(this.fXInfo.disconnectVFX);
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000BB53A File Offset: 0x000B973A
	public void PlayGrabbedFx()
	{
		this.PlayFX(this.fXInfo.grabbedVFX);
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000BB54D File Offset: 0x000B974D
	public void PlayTooHeavyFx()
	{
		this.PlayFX(this.fXInfo.tooHeavyVFX);
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000BB560 File Offset: 0x000B9760
	public void PlayLocationLockFx()
	{
		this.PlayFX(this.fXInfo.locationLockVFX);
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000BB573 File Offset: 0x000B9773
	public void PlayRecycleFx()
	{
		this.PlayFX(this.fXInfo.recycleVFX);
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000BB586 File Offset: 0x000B9786
	private void PlayFX(GameObject fx)
	{
		ObjectPools.instance.Instantiate(fx, base.transform.position, true);
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000BB5A0 File Offset: 0x000B97A0
	public static BuilderPiece GetBuilderPieceFromCollider(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		BuilderPieceCollider component = collider.GetComponent<BuilderPieceCollider>();
		if (!(component == null))
		{
			return component.piece;
		}
		return null;
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000BB5D0 File Offset: 0x000B97D0
	public static BuilderPiece GetBuilderPieceFromTransform(Transform transform)
	{
		while (transform != null)
		{
			BuilderPiece component = transform.GetComponent<BuilderPiece>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000BB604 File Offset: 0x000B9804
	public static void MakePieceRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (piece.parentPiece == null || piece.parentPiece.isBuiltIntoTable)
		{
			return;
		}
		BuilderPiece.MakePieceRoot(piece.parentPiece);
		int newAttachIndex = piece.parentAttachIndex;
		int newParentAttachIndex = piece.attachIndex;
		BuilderPiece builderPiece = piece.parentPiece;
		bool ignoreSnaps = true;
		piece.ClearParentPiece(ignoreSnaps);
		builderPiece.SetParentPiece(newAttachIndex, piece, newParentAttachIndex);
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000BB668 File Offset: 0x000B9868
	public BuilderPiece GetRootPiece()
	{
		BuilderPiece builderPiece = this;
		while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
		{
			builderPiece = builderPiece.parentPiece;
		}
		return builderPiece;
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000BB69C File Offset: 0x000B989C
	public bool IsPrivatePlot()
	{
		return this.isPrivatePlot;
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000BB6A4 File Offset: 0x000B98A4
	public bool TryGetPlotComponent(out BuilderPiecePrivatePlot plot)
	{
		plot = this.plotComponent;
		return this.isPrivatePlot;
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000BB6BC File Offset: 0x000B98BC
	public static bool CanPlayerAttachPieceToPiece(int playerActorNumber, BuilderPiece attachingPiece, BuilderPiece attachToPiece)
	{
		if (attachToPiece.state != BuilderPiece.State.AttachedAndPlaced && !attachToPiece.IsPrivatePlot() && attachToPiece.state != BuilderPiece.State.AttachedToArm)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = attachToPiece.GetAttachedBuiltInPiece();
		if (attachedBuiltInPiece == null || (!attachedBuiltInPiece.isPrivatePlot && !attachedBuiltInPiece.isArmShelf))
		{
			return true;
		}
		if (attachedBuiltInPiece.isArmShelf)
		{
			return attachedBuiltInPiece.heldByPlayerActorNumber == playerActorNumber && attachedBuiltInPiece.armShelf != null && attachedBuiltInPiece.armShelf.CanAttachToArmPiece();
		}
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || (builderPiecePrivatePlot.CanPlayerAttachToPlot(playerActorNumber) && builderPiecePrivatePlot.IsChainUnderCapacity(attachingPiece));
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000BB754 File Offset: 0x000B9954
	public bool CanPlayerGrabPiece(int actorNumber, Vector3 worldPosition)
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced && !this.isPrivatePlot)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = this.GetAttachedBuiltInPiece();
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return attachedBuiltInPiece == null || !attachedBuiltInPiece.isPrivatePlot || !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || builderPiecePrivatePlot.CanPlayerGrabFromPlot(actorNumber, worldPosition) || this.tableOwner.IsLocationWithinSharedBuildArea(worldPosition);
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000BB7B4 File Offset: 0x000B99B4
	public bool IsPieceMoving()
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return false;
		}
		if (this.attachPlayerToPiece)
		{
			return true;
		}
		if (this.attachIndex < 0 || this.attachIndex >= this.gridPlanes.Count)
		{
			return false;
		}
		if (this.gridPlanes[this.attachIndex].IsAttachedToMovingGrid())
		{
			return true;
		}
		using (List<BuilderAttachGridPlane>.Enumerator enumerator = this.gridPlanes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isMoving)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000BB85C File Offset: 0x000B9A5C
	public BuilderPiece GetAttachedBuiltInPiece()
	{
		if (this.isBuiltIntoTable)
		{
			return this;
		}
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return null;
		}
		BuilderPiece rootPiece = this.GetRootPiece();
		if (rootPiece.parentPiece != null)
		{
			rootPiece = rootPiece.parentPiece;
		}
		if (rootPiece.isBuiltIntoTable)
		{
			return rootPiece;
		}
		return null;
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000BB8A4 File Offset: 0x000B9AA4
	public int GetChainCostAndCount(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		return 1 + this.GetChildCountAndCost(costArray);
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000BB938 File Offset: 0x000B9B38
	public int GetChildCountAndCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			num += builderPiece.GetChildCountAndCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
		return num;
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000BB9DC File Offset: 0x000B9BDC
	public int GetChildCount()
	{
		int num = 0;
		foreach (BuilderAttachGridPlane builderAttachGridPlane in this.gridPlanes)
		{
			num += builderAttachGridPlane.GetChildCount();
		}
		return num;
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000BBA34 File Offset: 0x000B9C34
	public void GetChainCost(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		this.AddChildCost(costArray);
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000BBAC8 File Offset: 0x000B9CC8
	public void AddChildCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			builderPiece.AddChildCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000BBB68 File Offset: 0x000B9D68
	public void BumpTwistToPositionRotation(byte twist, sbyte xOffset, sbyte zOffset, int potentialAttachIndex, BuilderAttachGridPlane potentialParentGridPlane, out Vector3 localPosition, out Quaternion localRotation, out Vector3 worldPosition, out Quaternion worldRotation)
	{
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		Transform center = potentialParentGridPlane.center;
		Vector3 position = center.position;
		Quaternion rotation = center.rotation;
		float num = flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset;
		float num2 = flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset;
		float num3 = num - potentialParentGridPlane.widthOffset;
		float num4 = num2 - potentialParentGridPlane.lengthOffset;
		Quaternion quaternion = Quaternion.Euler(0f, (float)twist * 90f, 0f);
		Quaternion quaternion2 = rotation * quaternion;
		float num5 = (float)xOffset * gridSize + num3;
		float num6 = (float)zOffset * gridSize + num4;
		Vector3 vector;
		vector..ctor(num5, 0f, num6);
		Vector3 vector2 = position + rotation * vector;
		Transform center2 = builderAttachGridPlane.center;
		Quaternion quaternion3 = quaternion2 * Quaternion.Inverse(center2.localRotation);
		Vector3 vector3 = base.transform.InverseTransformPoint(center2.position);
		Vector3 vector4 = vector2 - quaternion3 * vector3;
		localPosition = potentialParentGridPlane.transform.InverseTransformPoint(vector4);
		localRotation = quaternion * Quaternion.Inverse(center2.localRotation);
		worldPosition = vector4;
		worldRotation = quaternion3;
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000BBCBC File Offset: 0x000B9EBC
	public Quaternion TwistToLocalRotation(byte twist, int potentialAttachIndex)
	{
		float num = 90f * (float)twist;
		Quaternion quaternion = Quaternion.Euler(0f, num, 0f);
		if (potentialAttachIndex < 0 || potentialAttachIndex >= this.gridPlanes.Count)
		{
			return quaternion;
		}
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		Transform transform = (builderAttachGridPlane.center != null) ? builderAttachGridPlane.center : builderAttachGridPlane.transform;
		return quaternion * Quaternion.Inverse(transform.localRotation);
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000BBD34 File Offset: 0x000B9F34
	public int GetPiecePlacement()
	{
		byte pieceTwist = this.GetPieceTwist();
		sbyte xOffset;
		sbyte zOffset;
		this.GetPieceBumpOffset(pieceTwist, out xOffset, out zOffset);
		return BuilderTable.PackPiecePlacement(pieceTwist, xOffset, zOffset);
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000BBD5C File Offset: 0x000B9F5C
	public byte GetPieceTwist()
	{
		if (this.attachIndex == -1)
		{
			return 0;
		}
		Quaternion localRotation = base.transform.localRotation;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		Quaternion quaternion = localRotation * builderAttachGridPlane.transform.localRotation;
		float num = 0.866f;
		Vector3 vector = quaternion * Vector3.forward;
		float num2 = Vector3.Dot(vector, Vector3.forward);
		float num3 = Vector3.Dot(vector, Vector3.right);
		bool flag = Mathf.Abs(num2) > num;
		bool flag2 = Mathf.Abs(num3) > num;
		if (!flag && !flag2)
		{
			return 0;
		}
		uint num4;
		if (flag)
		{
			num4 = ((num2 > 0f) ? 0U : 2U);
		}
		else
		{
			num4 = ((num3 > 0f) ? 1U : 3U);
		}
		return (byte)num4;
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000BBE10 File Offset: 0x000BA010
	public void GetPieceBumpOffset(byte twist, out sbyte xOffset, out sbyte zOffset)
	{
		if (this.attachIndex == -1 || this.parentPiece == null)
		{
			xOffset = 0;
			zOffset = 0;
			return;
		}
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		BuilderAttachGridPlane builderAttachGridPlane2 = this.parentPiece.gridPlanes[this.parentAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		float num = flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset;
		float num2 = flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset;
		float num3 = num - builderAttachGridPlane2.widthOffset;
		float num4 = num2 - builderAttachGridPlane2.lengthOffset;
		Vector3 position = builderAttachGridPlane.center.position;
		Vector3 position2 = builderAttachGridPlane2.center.position;
		Vector3 vector = Quaternion.Inverse(builderAttachGridPlane2.center.rotation) * (position - position2);
		xOffset = (sbyte)Mathf.RoundToInt((vector.x - num3) / gridSize);
		zOffset = (sbyte)Mathf.RoundToInt((vector.z - num4) / gridSize);
	}

	// Token: 0x04002E47 RID: 11847
	public const int INVALID = -1;

	// Token: 0x04002E48 RID: 11848
	public const float LIGHT_MASS = 1f;

	// Token: 0x04002E49 RID: 11849
	public const float HEAVY_MASS = 10000f;

	// Token: 0x04002E4A RID: 11850
	[Tooltip("Name for debug text")]
	public string displayName;

	// Token: 0x04002E4B RID: 11851
	[Tooltip("(Optional) scriptable object containing material swaps")]
	public BuilderMaterialOptions materialOptions;

	// Token: 0x04002E4C RID: 11852
	[Tooltip("Builder Resources used by this object\nbuilderRscBasic for simple meshes\nbuilderRscDecorative for detailed meshes\nbuilderRscFunctional for extra scripts or effects")]
	public BuilderResources cost;

	// Token: 0x04002E4D RID: 11853
	[Tooltip("Spawn Offset")]
	public Vector3 desiredShelfOffset = Vector3.zero;

	// Token: 0x04002E4E RID: 11854
	[Tooltip("Spawn Offset")]
	public Vector3 desiredShelfRotationOffset = Vector3.zero;

	// Token: 0x04002E4F RID: 11855
	[FormerlySerializedAs("vFXInfo")]
	[Tooltip("sounds for block actions. everything uses BuilderPieceEffectInfo_Default")]
	[SerializeField]
	private BuilderPieceEffectInfo fXInfo;

	// Token: 0x04002E50 RID: 11856
	private List<MeshRenderer> materialSwapTargets;

	// Token: 0x04002E51 RID: 11857
	private List<GorillaSurfaceOverride> surfaceOverrides;

	// Token: 0x04002E52 RID: 11858
	[Tooltip("parent object of everything scaled with the piece")]
	public Transform scaleRoot;

	// Token: 0x04002E53 RID: 11859
	[Tooltip("Is the block part of the room / immovable (used for the base terrain)")]
	public bool isBuiltIntoTable;

	// Token: 0x04002E54 RID: 11860
	public bool isArmShelf;

	// Token: 0x04002E55 RID: 11861
	[HideInInspector]
	public BuilderArmShelf armShelf;

	// Token: 0x04002E56 RID: 11862
	[Tooltip("Used to prevent log warnings from materials incompatible with the builder renderer\nAnything that needs text/transparency/or particles uses the normal rendering pipeline")]
	public bool suppressMaterialWarnings;

	// Token: 0x04002E57 RID: 11863
	[Tooltip("Only used by private plots")]
	private bool isPrivatePlot;

	// Token: 0x04002E58 RID: 11864
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x04002E59 RID: 11865
	[Tooltip("Only used by private plots")]
	public BuilderPiecePrivatePlot plotComponent;

	// Token: 0x04002E5A RID: 11866
	[Tooltip("Add piece movement to player movement when touched")]
	public bool attachPlayerToPiece;

	// Token: 0x04002E5B RID: 11867
	public int pieceType;

	// Token: 0x04002E5C RID: 11868
	public int pieceId;

	// Token: 0x04002E5D RID: 11869
	public int pieceDataIndex;

	// Token: 0x04002E5E RID: 11870
	public int materialType = -1;

	// Token: 0x04002E5F RID: 11871
	public int heldByPlayerActorNumber;

	// Token: 0x04002E60 RID: 11872
	public bool heldInLeftHand;

	// Token: 0x04002E61 RID: 11873
	public Transform parentHeld;

	// Token: 0x04002E62 RID: 11874
	[HideInInspector]
	public BuilderPiece parentPiece;

	// Token: 0x04002E63 RID: 11875
	[HideInInspector]
	public BuilderPiece firstChildPiece;

	// Token: 0x04002E64 RID: 11876
	[HideInInspector]
	public BuilderPiece nextSiblingPiece;

	// Token: 0x04002E65 RID: 11877
	[HideInInspector]
	public int attachIndex;

	// Token: 0x04002E66 RID: 11878
	[HideInInspector]
	public int parentAttachIndex;

	// Token: 0x04002E67 RID: 11879
	public int shelfOwner = -1;

	// Token: 0x04002E68 RID: 11880
	[HideInInspector]
	public List<BuilderAttachGridPlane> gridPlanes;

	// Token: 0x04002E69 RID: 11881
	[HideInInspector]
	public List<Collider> colliders;

	// Token: 0x04002E6A RID: 11882
	public List<Collider> placedOnlyColliders;

	// Token: 0x04002E6B RID: 11883
	private int currentColliderLayer = BuilderTable.droppedLayer;

	// Token: 0x04002E6C RID: 11884
	[Tooltip("Components enabled when the block is snapped to the build table")]
	public List<Behaviour> onlyWhenPlacedBehaviours;

	// Token: 0x04002E6D RID: 11885
	[Tooltip("Game objects enabled when the block is snapped to the build table\nAny concave collision should be here")]
	public List<GameObject> onlyWhenPlaced;

	// Token: 0x04002E6E RID: 11886
	[Tooltip("Game objects enabled when the block is not snapped to the build table\n Convex collision should be here if there is concave collision when placed")]
	public List<GameObject> onlyWhenNotPlaced;

	// Token: 0x04002E6F RID: 11887
	public List<IBuilderPieceComponent> pieceComponents;

	// Token: 0x04002E70 RID: 11888
	public IBuilderPieceFunctional functionalPieceComponent;

	// Token: 0x04002E71 RID: 11889
	public byte functionalPieceState;

	// Token: 0x04002E72 RID: 11890
	public List<IBuilderPieceFunctional> pieceFunctionComponents;

	// Token: 0x04002E73 RID: 11891
	private bool pieceComponentsActive;

	// Token: 0x04002E74 RID: 11892
	[Tooltip("Check if any renderers are in the onlyWhenPlaced or onlyWhenNotPlaced lists")]
	public bool areMeshesToggledOnPlace;

	// Token: 0x04002E75 RID: 11893
	[NonSerialized]
	public Rigidbody rigidBody;

	// Token: 0x04002E76 RID: 11894
	[NonSerialized]
	public int activatedTimeStamp;

	// Token: 0x04002E77 RID: 11895
	[HideInInspector]
	public int preventSnapUntilMoved;

	// Token: 0x04002E78 RID: 11896
	[HideInInspector]
	public Vector3 preventSnapUntilMovedFromPos;

	// Token: 0x04002E79 RID: 11897
	[HideInInspector]
	public BuilderPiece requestedParentPiece;

	// Token: 0x04002E7A RID: 11898
	private BuilderTable tableOwner;

	// Token: 0x04002E7B RID: 11899
	public PieceFallbackInfo fallbackInfo;

	// Token: 0x04002E7C RID: 11900
	[NonSerialized]
	public bool overrideSavedPiece;

	// Token: 0x04002E7D RID: 11901
	[NonSerialized]
	public int savedPieceType = -1;

	// Token: 0x04002E7E RID: 11902
	[NonSerialized]
	public int savedMaterialType = -1;

	// Token: 0x04002E7F RID: 11903
	private float pieceScale;

	// Token: 0x04002E80 RID: 11904
	private float[] collisionEnterHistory;

	// Token: 0x04002E81 RID: 11905
	private int collisionEnterLimit = 10;

	// Token: 0x04002E82 RID: 11906
	private float collisionEnterCooldown = 2f;

	// Token: 0x04002E83 RID: 11907
	private int oldCollisionTimeIndex;

	// Token: 0x04002E84 RID: 11908
	[HideInInspector]
	public BuilderPiece.State state;

	// Token: 0x04002E85 RID: 11909
	[HideInInspector]
	public bool isStatic;

	// Token: 0x04002E86 RID: 11910
	[NonSerialized]
	private bool listeningToHandLinks;

	// Token: 0x04002E87 RID: 11911
	[HideInInspector]
	public List<MeshRenderer> renderingDirect;

	// Token: 0x04002E88 RID: 11912
	[HideInInspector]
	public List<MeshRenderer> renderingIndirect;

	// Token: 0x04002E89 RID: 11913
	[HideInInspector]
	public List<int> renderingIndirectTransformIndex;

	// Token: 0x04002E8A RID: 11914
	[HideInInspector]
	public float tint;

	// Token: 0x04002E8B RID: 11915
	private int paintingCount;

	// Token: 0x04002E8C RID: 11916
	private int potentialGrabCount;

	// Token: 0x04002E8D RID: 11917
	private int potentialGrabChildCount;

	// Token: 0x04002E8E RID: 11918
	internal bool forcedFrozen;

	// Token: 0x04002E8F RID: 11919
	private HashSet<int> collidersEntered = new HashSet<int>(128);

	// Token: 0x04002E90 RID: 11920
	private static List<MeshRenderer> tempRenderers = new List<MeshRenderer>(48);

	// Token: 0x02000584 RID: 1412
	public enum State
	{
		// Token: 0x04002E92 RID: 11922
		None = -1,
		// Token: 0x04002E93 RID: 11923
		AttachedAndPlaced,
		// Token: 0x04002E94 RID: 11924
		AttachedToDropped,
		// Token: 0x04002E95 RID: 11925
		Grabbed,
		// Token: 0x04002E96 RID: 11926
		Dropped,
		// Token: 0x04002E97 RID: 11927
		OnShelf,
		// Token: 0x04002E98 RID: 11928
		Displayed,
		// Token: 0x04002E99 RID: 11929
		GrabbedLocal,
		// Token: 0x04002E9A RID: 11930
		OnConveyor,
		// Token: 0x04002E9B RID: 11931
		AttachedToArm
	}
}
