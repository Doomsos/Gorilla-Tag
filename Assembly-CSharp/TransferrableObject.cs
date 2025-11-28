using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x020004A0 RID: 1184
public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable, ISpawnable, IBuildValidation
{
	// Token: 0x06001E59 RID: 7769 RVA: 0x000A03FC File Offset: 0x0009E5FC
	public void FixTransformOverride()
	{
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x00002789 File Offset: 0x00000989
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001E5B RID: 7771 RVA: 0x000A040A File Offset: 0x0009E60A
	// (set) Token: 0x06001E5C RID: 7772 RVA: 0x000A0412 File Offset: 0x0009E612
	public VRRig myRig
	{
		get
		{
			return this._myRig;
		}
		private set
		{
			this._myRig = value;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001E5D RID: 7773 RVA: 0x000A041B File Offset: 0x0009E61B
	// (set) Token: 0x06001E5E RID: 7774 RVA: 0x000A0423 File Offset: 0x0009E623
	public bool isMyRigValid { get; private set; }

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001E5F RID: 7775 RVA: 0x000A042C File Offset: 0x0009E62C
	// (set) Token: 0x06001E60 RID: 7776 RVA: 0x000A0434 File Offset: 0x0009E634
	public VRRig myOnlineRig
	{
		get
		{
			return this._myOnlineRig;
		}
		private set
		{
			this._myOnlineRig = value;
			this.isMyOnlineRigValid = true;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001E61 RID: 7777 RVA: 0x000A0444 File Offset: 0x0009E644
	// (set) Token: 0x06001E62 RID: 7778 RVA: 0x000A044C File Offset: 0x0009E64C
	public bool isMyOnlineRigValid { get; private set; }

	// Token: 0x06001E63 RID: 7779 RVA: 0x000A0458 File Offset: 0x0009E658
	public void SetTargetRig(VRRig rig)
	{
		if (rig == null)
		{
			this.targetRigSet = false;
			if (this.isSceneObject)
			{
				this.targetRig = rig;
				this.targetDockPositions = null;
				this.anchorOverrides = null;
				return;
			}
			if (this.myRig)
			{
				this.SetTargetRig(this.myRig);
			}
			if (this.myOnlineRig)
			{
				this.SetTargetRig(this.myOnlineRig);
			}
			return;
		}
		else
		{
			this.targetRigSet = true;
			this.targetRig = rig;
			BodyDockPositions component = rig.GetComponent<BodyDockPositions>();
			VRRigAnchorOverrides component2 = rig.GetComponent<VRRigAnchorOverrides>();
			if (!component)
			{
				Debug.LogError("There is no dock attached to this rig", this);
				return;
			}
			if (!component2)
			{
				Debug.LogError("There is no overrides attached to this rig", this);
				return;
			}
			this.anchorOverrides = component2;
			this.targetDockPositions = component;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			return;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001E64 RID: 7780 RVA: 0x000A0528 File Offset: 0x0009E728
	public bool IsLocalOwnedWorldShareable
	{
		get
		{
			return this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine;
		}
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x000A054C File Offset: 0x0009E74C
	public void WorldShareableRequestOwnership()
	{
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001E66 RID: 7782 RVA: 0x000A05A8 File Offset: 0x0009E7A8
	// (set) Token: 0x06001E67 RID: 7783 RVA: 0x000A05B0 File Offset: 0x0009E7B0
	public bool isRigidbodySet { get; private set; }

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001E68 RID: 7784 RVA: 0x000A05B9 File Offset: 0x0009E7B9
	// (set) Token: 0x06001E69 RID: 7785 RVA: 0x000A05C1 File Offset: 0x0009E7C1
	public bool shouldUseGravity { get; private set; }

	// Token: 0x06001E6A RID: 7786 RVA: 0x000A05CA File Offset: 0x0009E7CA
	protected virtual void Awake()
	{
		if (this.isSceneObject)
		{
			this.IsSpawned = true;
			this.OnSpawn(null);
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001E6B RID: 7787 RVA: 0x000A05E2 File Offset: 0x0009E7E2
	// (set) Token: 0x06001E6C RID: 7788 RVA: 0x000A05EA File Offset: 0x0009E7EA
	public bool IsSpawned { get; set; }

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06001E6D RID: 7789 RVA: 0x000A05F3 File Offset: 0x0009E7F3
	// (set) Token: 0x06001E6E RID: 7790 RVA: 0x000A05FB File Offset: 0x0009E7FB
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001E6F RID: 7791 RVA: 0x000A0604 File Offset: 0x0009E804
	public virtual void OnSpawn(VRRig rig)
	{
		try
		{
			if (!this.isSceneObject)
			{
				if (!rig)
				{
					Debug.LogError("Disabling TransferrableObject because could not find VRRig! \"" + base.transform.GetPath() + "\"", this);
					base.enabled = false;
					this.isMyRigValid = false;
					this.isMyOnlineRigValid = false;
					return;
				}
				this.myRig = (rig.isOfflineVRRig ? rig : null);
				this.myOnlineRig = (rig.isOfflineVRRig ? null : rig);
			}
			else
			{
				this.myRig = null;
				this.myOnlineRig = null;
			}
			this.isMyRigValid = true;
			this.isMyOnlineRigValid = true;
			this.targetDockPositions = base.GetComponentInParent<BodyDockPositions>();
			this.anchor = base.transform.parent;
			if (this.rigidbodyInstance == null)
			{
				this.rigidbodyInstance = base.GetComponent<Rigidbody>();
			}
			if (this.rigidbodyInstance != null)
			{
				this.isRigidbodySet = true;
				this.shouldUseGravity = this.rigidbodyInstance.useGravity;
			}
			this.audioSrc = base.GetComponent<AudioSource>();
			this.latched = false;
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.anchor == null)
			{
				this.InitialDockObject = base.transform.parent;
			}
			else
			{
				this.InitialDockObject = this.anchor.parent;
			}
			this.isGrabAnchorSet = (this.grabAnchor != null);
			if (this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = true;
						spawnable.CosmeticSelectedSide = this.CosmeticSelectedSide;
						spawnable.OnSpawn(this.myRig);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x000A0800 File Offset: 0x0009EA00
	public virtual void OnDespawn()
	{
		try
		{
			if (!this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = false;
						spawnable.OnDespawn();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x000A0888 File Offset: 0x0009EA88
	private void SetInitMatrix()
	{
		this.initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		if (this.handPoseLeft != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseLeftReferenceRotation * Quaternion.Inverse(this.handPoseLeft.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseLeftReferencePoint) - this.handPoseLeft.transform.position;
			this.leftHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.leftHandMatrix = this.initMatrix;
		}
		if (this.handPoseRight != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseRightReferenceRotation * Quaternion.Inverse(this.handPoseRight.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseRightReferencePoint) - this.handPoseRight.transform.position;
			this.rightHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.rightHandMatrix = this.initMatrix;
		}
		base.transform.localPosition = this.initMatrix.Position();
		base.transform.localRotation = this.initMatrix.Rotation();
		this.positionInitialized = true;
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void Start()
	{
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x000A0A00 File Offset: 0x0009EC00
	internal virtual void OnEnable()
	{
		try
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			if (!this.isSceneObject && !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				Debug.LogError("`TransferrableObject.OnEnable()` was called before allPartsInstantiated was true. Path: " + base.transform.GetPathQ(), this);
				if (!this._isListeningFor_OnPostInstantiateAllPrefabs2)
				{
					this._isListeningFor_OnPostInstantiateAllPrefabs2 = true;
					CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2 = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2, new Action(this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject));
				}
			}
			else
			{
				this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
		{
			this.previousItemState = (TransferrableObject.ItemStates)0;
			this.itemState = (TransferrableObject.ItemStates)0;
		}
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x000A0B14 File Offset: 0x0009ED14
	public virtual void OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!base.enabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this._isListeningFor_OnPostInstantiateAllPrefabs2 = false;
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2 = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2, new Action(this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject));
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		try
		{
			TransferrableObjectManager.Register(this);
			this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.isSceneObject)
			{
				if (!this.worldShareableInstance)
				{
					Debug.LogError("Missing Sharable Instance on Scene enabled object: " + base.gameObject.name);
				}
				else
				{
					this.worldShareableInstance.SyncToSceneObject(this);
					this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().AddCallbackTarget(this);
				}
			}
			else
			{
				if (!this.isSceneObject && !this.myRig && !this.myOnlineRig && !this.ownerRig)
				{
					this.ownerRig = base.GetComponentInParent<VRRig>(true);
					if (this.ownerRig.isOfflineVRRig)
					{
						this.myRig = this.ownerRig;
					}
					else
					{
						this.myOnlineRig = this.ownerRig;
					}
				}
				if (!this.myRig && this.myOnlineRig)
				{
					this.ownerRig = this.myOnlineRig;
					this.SetTargetRig(this.myOnlineRig);
				}
				if (this.myRig == null && this.myOnlineRig == null)
				{
					if (!this.isSceneObject)
					{
						base.gameObject.SetActive(false);
					}
				}
				else
				{
					this.objectIndex = this.targetDockPositions.ReturnTransferrableItemIndex(this.myIndex);
					if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
					{
						this.storedZone = BodyDockPositions.DropPositions.LeftArm;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
					{
						this.storedZone = BodyDockPositions.DropPositions.RightArm;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
					{
						this.storedZone = BodyDockPositions.DropPositions.LeftBack;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
					{
						this.storedZone = BodyDockPositions.DropPositions.RightBack;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnChest)
					{
						this.storedZone = BodyDockPositions.DropPositions.Chest;
					}
					if (this.IsLocalObject())
					{
						this.ownerRig = GorillaTagger.Instance.offlineVRRig;
						this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
					}
					if (this.objectIndex == -1)
					{
						base.gameObject.SetActive(false);
					}
					else
					{
						if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
						{
							Transform transform = this.GetAnchor(this.currentState);
							transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
						}
						this.initState = this.currentState;
						this.enabledOnFrame = Time.frameCount;
						this.startInterpolation = true;
						if (NetworkSystem.Instance.InRoom)
						{
							if (this.canDrop || this.shareable)
							{
								this.SpawnTransferableObjectViews();
								if (this.myRig)
								{
									if (this.myRig != null && this.worldShareableInstance != null)
									{
										this.OnWorldShareableItemSpawn();
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x000A0E84 File Offset: 0x0009F084
	internal virtual void OnDisable()
	{
		TransferrableObjectManager.Unregister(this);
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		this._isListeningFor_OnPostInstantiateAllPrefabs2 = false;
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2 = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs2, new Action(this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject));
		this.enabledOnFrame = -1;
		base.transform.localScale = Vector3.one;
		try
		{
			if (!this.isSceneObject && this.IsLocalObject() && this.worldShareableInstance && !this.IsMyItem())
			{
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
				{
				});
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.Invalidate();
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
				if (this.targetDockPositions)
				{
					this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
				}
				if (!this.isSceneObject)
				{
					this.worldShareableInstance = null;
				}
			}
			this.PlayDestroyedOrDisabledEffect();
			if (this.isSceneObject)
			{
				this.IsSpawned = false;
				this.OnDespawn();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x000A1024 File Offset: 0x0009F224
	protected new virtual void OnDestroy()
	{
		TransferrableObjectManager.Unregister(this);
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x000A102C File Offset: 0x0009F22C
	public void CleanupDisable()
	{
		this.currentState = TransferrableObject.PositionState.None;
		this.enabledOnFrame = -1;
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			if (this.anchor != base.transform)
			{
				base.transform.parent = this.anchor;
			}
		}
		else
		{
			base.transform.parent = this.anchor;
		}
		this.interpState = TransferrableObject.InterpolateState.None;
		Transform transform = base.transform;
		Matrix4x4 defaultTransformationMatrix = this.GetDefaultTransformationMatrix();
		transform.SetLocalMatrixRelativeToParentWithXParity(defaultTransformationMatrix);
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x000A10B7 File Offset: 0x0009F2B7
	public virtual void PreDisable()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
		{
			this.previousItemState = (TransferrableObject.ItemStates)0;
			this.itemState = (TransferrableObject.ItemStates)0;
		}
		this.currentState = TransferrableObject.PositionState.None;
		this.interpState = TransferrableObject.InterpolateState.None;
		this.ResetToDefaultState();
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000A10EC File Offset: 0x0009F2EC
	public virtual Matrix4x4 GetDefaultTransformationMatrix()
	{
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState == TransferrableObject.PositionState.InLeftHand)
		{
			return this.leftHandMatrix;
		}
		if (positionState != TransferrableObject.PositionState.InRightHand)
		{
			return this.initMatrix;
		}
		return this.rightHandMatrix;
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000A111E File Offset: 0x0009F31E
	public virtual bool ShouldBeKinematic()
	{
		if (this.detatchOnGrab)
		{
			return this.currentState != TransferrableObject.PositionState.Dropped && this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand;
		}
		return this.currentState != TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000A115C File Offset: 0x0009F35C
	private void SpawnShareableObject()
	{
		if (this.isSceneObject)
		{
			if (this.worldShareableInstance == null)
			{
				return;
			}
			this.worldShareableInstance.GetComponent<WorldShareableItem>().SetupSceneObjectOnNetwork(NetworkSystem.Instance.MasterClient);
			return;
		}
		else
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.SpawnTransferableObjectViews();
			if (!this.myRig)
			{
				return;
			}
			if (!this.canDrop && !this.shareable)
			{
				return;
			}
			if (this.myRig != null && this.worldShareableInstance != null)
			{
				this.OnWorldShareableItemSpawn();
			}
			return;
		}
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000A11F0 File Offset: 0x0009F3F0
	public void SpawnTransferableObjectViews()
	{
		NetPlayer owner = NetworkSystem.Instance.LocalPlayer;
		if (!this.ownerRig.isOfflineVRRig)
		{
			owner = this.ownerRig.creator;
		}
		if (this.worldShareableInstance == null)
		{
			this.worldShareableInstance = this.targetDockPositions.AllocateSharableInstance(this.storedZone, owner);
		}
		GorillaTagger.OnPlayerSpawned(delegate
		{
			this.worldShareableInstance.SetupSharableObject(this.myIndex, owner, this.transform);
		});
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000A1274 File Offset: 0x0009F474
	public virtual void OnJoinedRoom()
	{
		if (this.isSceneObject)
		{
			this.worldShareableInstance == null;
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		this.SpawnTransferableObjectViews();
		if (!this.myRig)
		{
			return;
		}
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000A12EC File Offset: 0x0009F4EC
	public virtual void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.isSceneObject)
		{
			return;
		}
		if (!this.shareable && !this.allowWorldSharableInstance && !this.canDrop)
		{
			return;
		}
		if (base.gameObject.activeSelf && this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			else
			{
				this.worldShareableInstance.ResetViews();
				ObjectPools.instance.Destroy(this.worldShareableInstance.gameObject);
			}
			this.worldShareableInstance = null;
		}
		if (!this.IsLocalObject())
		{
			this.OnItemDestroyedOrDisabled();
			base.gameObject.Disable();
			return;
		}
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x000A13BA File Offset: 0x0009F5BA
	public bool IsLocalObject()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x000A13D1 File Offset: 0x0009F5D1
	public void SetWorldShareableItem(WorldShareableItem item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x000A13E0 File Offset: 0x0009F5E0
	protected virtual void OnItemDestroyedOrDisabled()
	{
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			Debug.LogError("Setting WSI to null in OnItemDestroyedOrDisabled", this);
			this.worldShareableInstance = null;
		}
		this.PlayDestroyedOrDisabledEffect();
		this.enabledOnFrame = -1;
		this.currentState = TransferrableObject.PositionState.None;
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000A145A File Offset: 0x0009F65A
	public virtual void TriggeredLateUpdate()
	{
		if (this.IsLocalObject() && this.canDrop)
		{
			this.LocalMyObjectValidation();
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000A148E File Offset: 0x0009F68E
	protected Transform DefaultAnchor()
	{
		if (this._isDefaultAnchorSet)
		{
			return this._defaultAnchor;
		}
		this._isDefaultAnchorSet = true;
		this._defaultAnchor = ((this.anchor == null) ? base.transform : this.anchor);
		return this._defaultAnchor;
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000A14CE File Offset: 0x0009F6CE
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x000A14FC File Offset: 0x0009F6FC
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x000A152C File Offset: 0x0009F72C
	private Transform GetTargetStorageZone(BodyDockPositions.DropPositions state)
	{
		switch (state)
		{
		case BodyDockPositions.DropPositions.None:
			return null;
		case BodyDockPositions.DropPositions.LeftArm:
			return this.targetDockPositions.leftArmTransform;
		case BodyDockPositions.DropPositions.RightArm:
			return this.targetDockPositions.rightArmTransform;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.Chest:
			return this.targetDockPositions.chestTransform;
		case BodyDockPositions.DropPositions.LeftBack:
			return this.targetDockPositions.leftBackTransform;
		default:
			if (state == BodyDockPositions.DropPositions.RightBack)
			{
				return this.targetDockPositions.rightBackTransform;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x000A15AD File Offset: 0x0009F7AD
	public static Transform GetTargetDock(TransferrableObject.PositionState state, VRRig rig)
	{
		return TransferrableObject.GetTargetDock(state, rig.myBodyDockPositions, rig.GetComponent<VRRigAnchorOverrides>());
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x000A15C4 File Offset: 0x0009F7C4
	public static Transform GetTargetDock(TransferrableObject.PositionState state, BodyDockPositions dockPositions, VRRigAnchorOverrides anchorOverrides)
	{
		if (state <= TransferrableObject.PositionState.InRightHand)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftArmTransform);
			case TransferrableObject.PositionState.OnRightArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.rightArmTransform);
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftHandTransform);
			default:
				if (state == TransferrableObject.PositionState.InRightHand)
				{
					return anchorOverrides.AnchorOverride(state, dockPositions.rightHandTransform);
				}
				break;
			}
		}
		else
		{
			if (state == TransferrableObject.PositionState.OnChest)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.chestTransform);
			}
			if (state == TransferrableObject.PositionState.OnLeftShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.leftBackTransform);
			}
			if (state == TransferrableObject.PositionState.OnRightShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.rightBackTransform);
			}
		}
		return null;
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x000A1668 File Offset: 0x0009F868
	private void UpdateFollowXform()
	{
		if (!this.targetRigSet)
		{
			return;
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		try
		{
			transform2 = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
		}
		catch
		{
			Debug.LogError("anchorOverrides or targetDock has been destroyed", this);
			this.SetTargetRig(null);
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped && this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		if (this.detatchOnGrab && (this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand))
		{
			base.transform.parent = null;
		}
		if (this.interpState == TransferrableObject.InterpolateState.None)
		{
			try
			{
				if (transform == null)
				{
					return;
				}
				this.startInterpolation |= (transform2 != transform.parent);
			}
			catch
			{
			}
			if (!this.startInterpolation && !this.isGrabAnchorSet && base.transform.parent != transform && transform != base.transform)
			{
				this.startInterpolation = true;
			}
			if (this.startInterpolation)
			{
				Vector3 position = base.transform.position;
				Quaternion rotation = base.transform.rotation;
				if (base.transform.parent != transform && transform != base.transform)
				{
					base.transform.parent = transform;
				}
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				if (this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					if (this.flipOnXForLeftHand)
					{
						transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (this.flipOnYForLeftHand)
					{
						transform.localScale = new Vector3(1f, -1f, 1f);
					}
					else
					{
						transform.localScale = Vector3.one;
					}
				}
				else
				{
					transform.localScale = Vector3.one;
				}
				if (Time.frameCount == this.enabledOnFrame || Time.frameCount == this.enabledOnFrame + 1)
				{
					Matrix4x4 matrix4x = this.GetDefaultTransformationMatrix();
					if ((this.currentState != TransferrableObject.PositionState.InLeftHand || !(this.handPoseLeft != null)) && this.currentState == TransferrableObject.PositionState.InRightHand)
					{
						this.handPoseRight != null;
					}
					Matrix4x4 matrix4x2;
					if (this.transferrableItemSlotTransformOverride && this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x2))
					{
						matrix4x = matrix4x2;
					}
					Matrix4x4 matrix = transform.localToWorldMatrix * matrix4x;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
					base.transform.localScale = matrix.lossyScale;
				}
				else
				{
					this.interpState = TransferrableObject.InterpolateState.Interpolating;
					if (this.IsMyItem() && this.useGrabType == TransferrableObject.GrabType.Free)
					{
						bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
						if (!flag)
						{
							GameObject rightHand = EquipmentInteractor.instance.rightHand;
						}
						else
						{
							GameObject leftHand = EquipmentInteractor.instance.leftHand;
						}
						Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig);
						this.SetupMatrixForFreeGrab(position, rotation, targetDock, flag);
					}
					this.interpDt = this.interpTime;
					this.interpStartRot = rotation;
					this.interpStartPos = position;
					base.transform.position = position;
					base.transform.rotation = rotation;
				}
				this.startInterpolation = false;
			}
		}
		if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
		{
			Matrix4x4 matrix4x3 = this.GetDefaultTransformationMatrix();
			if (this.transferrableItemSlotTransformOverride != null)
			{
				if (this.transferrableItemSlotTransformOverrideCachedMatrix == null)
				{
					Matrix4x4 matrix4x4;
					this.transferrableItemSlotTransformOverrideApplicable = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x4);
					this.transferrableItemSlotTransformOverrideCachedMatrix = new Matrix4x4?(matrix4x4);
				}
				if (this.transferrableItemSlotTransformOverrideApplicable)
				{
					matrix4x3 = this.transferrableItemSlotTransformOverrideCachedMatrix.Value;
				}
			}
			float num = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			Mathf.SmoothStep(0f, 1f, num);
			Matrix4x4 matrix2 = transform.localToWorldMatrix * matrix4x3;
			Transform transform3 = base.transform;
			Vector3 vector = matrix2.Position();
			transform3.position = this.interpStartPos.LerpToUnclamped(vector, num);
			base.transform.rotation = Quaternion.Slerp(this.interpStartRot, matrix2.Rotation(), num);
			base.transform.localScale = matrix4x3.lossyScale;
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = TransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
				matrix2 = transform.localToWorldMatrix * matrix4x3;
				base.transform.SetLocalToWorldMatrixNoScale(matrix2);
				base.transform.localScale = matrix4x3.lossyScale;
			}
		}
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x000A1BA8 File Offset: 0x0009FDA8
	public virtual void DropItem()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		this.currentState = TransferrableObject.PositionState.Dropped;
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.transferableObjectState = this.currentState;
		}
		if (this.canDrop)
		{
			base.transform.parent = null;
			if (this.anchor)
			{
				this.anchor.parent = this.InitialDockObject;
			}
			if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.isKinematic = true;
			}
		}
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x000A1CD0 File Offset: 0x0009FED0
	protected virtual void OnStateChanged()
	{
		if (this.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
		{
			int num = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
			if (!this.InHand() && num != 0)
			{
				TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
				if (syncOptions == TransferrableObject.SyncOptions.Bool)
				{
					this.ResetStateBools();
					return;
				}
				if (syncOptions != TransferrableObject.SyncOptions.Int)
				{
					return;
				}
				this.SetItemStateInt(0);
			}
		}
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000A1D28 File Offset: 0x0009FF28
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		if (this.isSceneObject)
		{
			this.disableItem = false;
		}
		else
		{
			for (int i = 0; i < this.ownerRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.ownerRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					this.disableItem = false;
					break;
				}
			}
			if (this.disableItem)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		if (this.previousState != this.currentState)
		{
			this.previousState = this.currentState;
			if (!this.Attached())
			{
				base.transform.parent = null;
				if (!this.ShouldBeKinematic() && this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = false;
				}
			}
			if (this.currentState == TransferrableObject.PositionState.None)
			{
				this.ResetToHome();
			}
			this.transferrableItemSlotTransformOverrideCachedMatrix = default(Matrix4x4?);
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			this.OnStateChanged();
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			if (!this.canDrop || this.allowReparenting)
			{
				goto IL_15A;
			}
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			try
			{
				if (this.anchor != null && this.anchor.parent != this.InitialDockObject)
				{
					this.anchor.parent = this.InitialDockObject;
				}
				goto IL_15A;
			}
			catch
			{
				goto IL_15A;
			}
		}
		if (this.currentState != TransferrableObject.PositionState.None)
		{
			this.UpdateFollowXform();
		}
		IL_15A:
		if (this.InHand() && !this.wasHeldShared)
		{
			UnityEvent onHeldShared = this.OnHeldShared;
			if (onHeldShared != null)
			{
				onHeldShared.Invoke();
			}
			this.wasHeldShared = true;
		}
		else if (!this.InHand() && !this.Dropped() && this.wasHeldShared)
		{
			UnityEvent onDockedShared = this.OnDockedShared;
			if (onDockedShared != null)
			{
				onDockedShared.Invoke();
			}
			this.wasHeldShared = false;
		}
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.rigidbodyInstance.isKinematic != this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
			if (this.worldShareableInstance)
			{
				if (this.currentState == TransferrableObject.PositionState.Dropped)
				{
					this.worldShareableInstance.EnableRemoteSync = true;
					return;
				}
				this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic();
			}
		}
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000A1F64 File Offset: 0x000A0164
	public virtual void ResetToHome()
	{
		if (this.isSceneObject)
		{
			this.currentState = TransferrableObject.PositionState.None;
		}
		this.ResetXf();
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x000A1FB0 File Offset: 0x000A01B0
	protected void ResetXf()
	{
		if (!this.positionInitialized)
		{
			this.initOffset = base.transform.localPosition;
			this.initRotation = base.transform.localRotation;
		}
		if (this.canDrop || this.allowWorldSharableInstance)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			if (this.ClearLocalPositionOnReset)
			{
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				base.transform.localScale = Vector3.one;
			}
			if (this.InitialDockObject)
			{
				this.anchor.localPosition = Vector3.zero;
				this.anchor.localRotation = Quaternion.identity;
				this.anchor.localScale = Vector3.one;
			}
			if (this.grabAnchor)
			{
				if (this.grabAnchor.parent != base.transform)
				{
					this.grabAnchor.parent = base.transform;
				}
				this.grabAnchor.localPosition = Vector3.zero;
				this.grabAnchor.localRotation = Quaternion.identity;
				this.grabAnchor.localScale = Vector3.one;
			}
			if (this.transferrableItemSlotTransformOverride)
			{
				Transform transformFromPositionState = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState);
				if (transformFromPositionState)
				{
					base.transform.position = transformFromPositionState.position;
					base.transform.rotation = transformFromPositionState.rotation;
					return;
				}
				if (this.anchorOverrides != null)
				{
					Transform transform2 = this.GetAnchor(this.currentState);
					Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
					Matrix4x4 matrix4x = this.GetDefaultTransformationMatrix();
					Matrix4x4 matrix4x2;
					if (this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, targetDock, out matrix4x2))
					{
						matrix4x = matrix4x2;
					}
					Matrix4x4 matrix = transform2.localToWorldMatrix * matrix4x;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
					base.transform.localScale = matrix.lossyScale;
					return;
				}
			}
			else
			{
				base.transform.SetLocalMatrixRelativeToParent(this.GetDefaultTransformationMatrix());
			}
		}
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x000A21F0 File Offset: 0x000A03F0
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		this.ResetXf();
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x000A2248 File Offset: 0x000A0448
	private void HandleLocalInput()
	{
		Behaviour[] array2;
		if (this.Dropped())
		{
			foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (gameObject2.activeSelf)
				{
					gameObject2.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			return;
		}
		if (!this.InHand())
		{
			foreach (GameObject gameObject3 in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject3.activeSelf)
				{
					gameObject3.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject4 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (!gameObject4.activeSelf)
				{
					gameObject4.SetActive(true);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
			return;
		}
		foreach (GameObject gameObject5 in this.gameObjectsActiveOnlyWhileHeld)
		{
			if (!gameObject5.activeSelf)
			{
				gameObject5.SetActive(true);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileHeld;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
		foreach (GameObject gameObject6 in this.gameObjectsActiveOnlyWhileDocked)
		{
			if (gameObject6.activeSelf)
			{
				gameObject6.SetActive(false);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileDocked;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		XRNode node = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? 4 : 5;
		this.indexTrigger = ControllerInputPoller.TriggerFloat(node);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void LocalMyObjectValidation()
	{
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x000A24D0 File Offset: 0x000A06D0
	protected virtual void LocalPersistanceValidation()
	{
		if (this.maxDistanceFromOriginBeforeRespawn != 0f && Vector3.Distance(base.transform.position, this.originPoint.position) > this.maxDistanceFromOriginBeforeRespawn)
		{
			if (this.audioSrc != null && this.resetPositionAudioClip != null)
			{
				this.audioSrc.GTPlayOneShot(this.resetPositionAudioClip, 1f);
			}
			if (this.currentState != TransferrableObject.PositionState.Dropped)
			{
				this.DropItem();
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			base.transform.position = this.originPoint.position;
			if (!this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.linearVelocity = Vector3.zero;
			}
		}
		if (this.rigidbodyInstance && this.rigidbodyInstance.linearVelocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			this.ResetToHome();
		}
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x000A25D0 File Offset: 0x000A07D0
	public void ObjectBeingTaken()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000A2670 File Offset: 0x000A0870
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		this.LocalPersistanceValidation();
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.isSceneObject && this.IsLocalObject())
			{
				this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
				this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
				this.myRig.SetTransferrableDockPosition(this.objectIndex, this.storedZone);
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.transferableObjectState = this.currentState;
				this.worldShareableInstance.transferableObjectItemState = this.itemState;
			}
		}
		this.HandleLocalInput();
		if (this.InHand() && !this.wasHeldLocal)
		{
			UnityEvent onHeldLocal = this.OnHeldLocal;
			if (onHeldLocal != null)
			{
				onHeldLocal.Invoke();
			}
			this.wasHeldLocal = true;
			return;
		}
		if (!this.InHand() && !this.Dropped() && this.wasHeldLocal)
		{
			UnityEvent onDockedLocal = this.OnDockedLocal;
			if (onDockedLocal != null)
			{
				onDockedLocal.Invoke();
			}
			this.wasHeldLocal = false;
		}
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000A2788 File Offset: 0x000A0988
	protected void LateUpdateReplicatedSceneObject()
	{
		if (this.myOnlineRig != null)
		{
			this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		}
		if (this.worldShareableInstance != null)
		{
			this.currentState = this.worldShareableInstance.transferableObjectState;
			this.itemState = this.worldShareableInstance.transferableObjectItemState;
			this.worldShareableInstance.EnableRemoteSync = (!this.ShouldBeKinematic() || this.currentState == TransferrableObject.PositionState.Dropped);
		}
		if (this.isRigidbodySet && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x000A282C File Offset: 0x000A0A2C
	protected virtual void LateUpdateReplicated()
	{
		if (this.isSceneObject || this.shareable)
		{
			this.LateUpdateReplicatedSceneObject();
			return;
		}
		if (this.myOnlineRig == null)
		{
			return;
		}
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (!this.ValidateState(this.currentState))
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.Disable();
			}
			this.currentState = this.previousState;
		}
		if (this.isRigidbodySet)
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
		}
		bool flag = true;
		this.previousItemState = this.itemState;
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		int num = this.myOnlineRig.ActiveTransferrableObjectIndexLength();
		for (int i = 0; i < num; i++)
		{
			if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				flag = false;
				foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
				{
					bool flag2 = this.InHand();
					if (gameObject.activeSelf != flag2)
					{
						gameObject.SetActive(flag2);
					}
				}
				Behaviour[] array2 = this.behavioursEnabledOnlyWhileHeld;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = this.InHand();
				}
				foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
				{
					bool flag3 = this.InHand();
					if (gameObject2.activeSelf == flag3)
					{
						gameObject2.SetActive(!flag3);
					}
				}
				array2 = this.behavioursEnabledOnlyWhileDocked;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = !this.InHand();
				}
			}
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.previousItemState != this.itemState)
		{
			int num2 = (int)(this.previousItemState & (TransferrableObject.ItemStates)(-65));
			int num3 = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
			if (num2 != num3)
			{
				this.OnNetworkItemStateChanged(num3);
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x000A2A40 File Offset: 0x000A0C40
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		if (!this.IsLocalObject() && this.worldShareableInstance && !this.isSceneObject)
		{
			if (this.IsMyItem())
			{
				return;
			}
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		this.ResetXf();
		TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
		if (syncOptions == TransferrableObject.SyncOptions.Bool)
		{
			this.ResetStateBools();
			return;
		}
		if (syncOptions != TransferrableObject.SyncOptions.Int)
		{
			return;
		}
		this.SetItemStateInt(0);
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x000A2AE4 File Offset: 0x000A0CE4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.worldShareableInstance == null) && !this.worldShareableInstance.guard.isTrulyMine)
		{
			if (!this.IsGrabbable())
			{
				return;
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
		if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState != TransferrableObject.PositionState.OnLeftArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabLeft = false;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.startInterpolation = true;
			}
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InLeftHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InLeftHand, EquipmentInteractor.instance.leftHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, true);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabRight = false;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.startInterpolation = true;
			}
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InRightHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InRightHand, EquipmentInteractor.instance.rightHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, false);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		if (this.rigidbodyInstance && !this.rigidbodyInstance.isKinematic && this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		PlayerGameEvents.GrabbedObject(this.interactEventName);
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000A2D3C File Offset: 0x000A0F3C
	private void SetupMatrixForFreeGrab(Vector3 worldPosition, Quaternion worldRotation, Transform attachPoint, bool leftHand)
	{
		Quaternion rotation = attachPoint.transform.rotation;
		Vector3 position = attachPoint.transform.position;
		Quaternion localRotation = Quaternion.Inverse(rotation) * worldRotation;
		Vector3 localPosition = Quaternion.Inverse(rotation) * (worldPosition - position);
		this.OnHandMatrixUpdate(localPosition, localRotation, leftHand);
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x000A2D8F File Offset: 0x000A0F8F
	protected void SetupHandMatrix(Vector3 leftHandPos, Quaternion leftHandRot, Vector3 rightHandPos, Quaternion rightHandRot)
	{
		this.leftHandMatrix = Matrix4x4.TRS(leftHandPos, leftHandRot, Vector3.one);
		this.rightHandMatrix = Matrix4x4.TRS(rightHandPos, rightHandRot, Vector3.one);
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
	{
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x000A2DB8 File Offset: 0x000A0FB8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (!this.IsHeld())
		{
			return false;
		}
		if (releasingHand == EquipmentInteractor.instance.leftHand)
		{
			this.canAutoGrabLeft = true;
		}
		else
		{
			this.canAutoGrabRight = true;
		}
		if (zoneReleased != null)
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
			bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
			if (flag || flag2)
			{
				return false;
			}
			if (this.targetDockPositions.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDockPositions && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
			{
				this.storedZone = zoneReleased.dropPosition;
			}
		}
		bool flag3 = false;
		this.interpState = TransferrableObject.InterpolateState.None;
		if (this.isSceneObject || this.canDrop || this.allowWorldSharableInstance)
		{
			if (!this.rigidbodyInstance)
			{
				return false;
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.EnableRemoteSync = true;
			}
			if (!flag3)
			{
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			if (this.rigidbodyInstance.isKinematic && !this.ShouldBeKinematic())
			{
				this.rigidbodyInstance.isKinematic = false;
			}
			GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
			if (component != null && this.rigidbodyInstance != null)
			{
				this.rigidbodyInstance.linearVelocity = component.linearVelocity;
				this.rigidbodyInstance.angularVelocity = component.angularVelocity;
			}
		}
		else
		{
			bool flag4 = this.allowWorldSharableInstance;
		}
		this.DropItemCleanup();
		EquipmentInteractor.instance.ForceDropEquipment(this);
		PlayerGameEvents.DroppedObject(this.interactEventName);
		return true;
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000A2F6C File Offset: 0x000A116C
	public override void DropItemCleanup()
	{
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000A2FDC File Offset: 0x000A11DC
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsGrabbable())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x000A3040 File Offset: 0x000A1240
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		if (this.myRig.netView != null)
		{
			this.myRig.netView.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				soundIndex,
				flag,
				0.1f
			});
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x000A30C1 File Offset: 0x000A12C1
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x000A30D3 File Offset: 0x000A12D3
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x000A30DC File Offset: 0x000A12DC
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x000A30E5 File Offset: 0x000A12E5
	public virtual bool IsMyItem()
	{
		return GorillaTagger.Instance == null || (this.targetRig != null && this.targetRig == GorillaTagger.Instance.offlineVRRig);
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x000A310F File Offset: 0x000A130F
	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance != null && (EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x000A313C File Offset: 0x000A133C
	public virtual bool IsGrabbable()
	{
		return this.IsMyItem() || ((this.isSceneObject || this.shareable) && (this.isSceneObject || this.shareable) && (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None));
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x000A319B File Offset: 0x000A139B
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x000A31B1 File Offset: 0x000A13B1
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000A31C0 File Offset: 0x000A13C0
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x000A31CB File Offset: 0x000A13CB
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000A31D6 File Offset: 0x000A13D6
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000A31E2 File Offset: 0x000A13E2
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000A31FA File Offset: 0x000A13FA
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000A3228 File Offset: 0x000A1428
	public bool ValidateState(TransferrableObject.PositionState state)
	{
		if (state <= TransferrableObject.PositionState.OnChest)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnRightArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				return false;
			case TransferrableObject.PositionState.InLeftHand:
				break;
			default:
				if (state != TransferrableObject.PositionState.InRightHand)
				{
					if (state != TransferrableObject.PositionState.OnChest)
					{
						return false;
					}
					if ((this.dockPositions & BodyDockPositions.DropPositions.Chest) != BodyDockPositions.DropPositions.None)
					{
						return true;
					}
					return false;
				}
				break;
			}
			return true;
		}
		if (state != TransferrableObject.PositionState.OnLeftShoulder)
		{
			if (state != TransferrableObject.PositionState.OnRightShoulder)
			{
				if (state == TransferrableObject.PositionState.Dropped)
				{
					return this.canDrop || this.shareable;
				}
			}
			else if ((this.dockPositions & BodyDockPositions.DropPositions.RightBack) != BodyDockPositions.DropPositions.None)
			{
				return true;
			}
		}
		else if ((this.dockPositions & BodyDockPositions.DropPositions.LeftBack) != BodyDockPositions.DropPositions.None)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000A32C8 File Offset: 0x000A14C8
	private void OnNetworkItemStateChanged(int stateBits)
	{
		TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
		if (syncOptions != TransferrableObject.SyncOptions.Bool)
		{
			if (syncOptions != TransferrableObject.SyncOptions.Int)
			{
				return;
			}
			UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
			if (onItemStateIntChanged == null)
			{
				return;
			}
			onItemStateIntChanged.Invoke(stateBits);
		}
		else
		{
			int num = (int)(this.previousItemState & TransferrableObject.ItemStates.State0);
			int num2 = (int)(this.itemState & TransferrableObject.ItemStates.State0);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolFalse = this.OnItemStateBoolFalse;
				if (onItemStateBoolFalse != null)
				{
					onItemStateBoolFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolTrue = this.OnItemStateBoolTrue;
				if (onItemStateBoolTrue != null)
				{
					onItemStateBoolTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State1);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State1);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
				if (onItemStateBoolBFalse != null)
				{
					onItemStateBoolBFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
				if (onItemStateBoolBTrue != null)
				{
					onItemStateBoolBTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State2);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State2);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
				if (onItemStateBoolCFalse != null)
				{
					onItemStateBoolCFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
				if (onItemStateBoolCTrue != null)
				{
					onItemStateBoolCTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State3);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State3);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
				if (onItemStateBoolDFalse == null)
				{
					return;
				}
				onItemStateBoolDFalse.Invoke();
				return;
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
				if (onItemStateBoolDTrue == null)
				{
					return;
				}
				onItemStateBoolDTrue.Invoke();
				return;
			}
		}
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000A33FB File Offset: 0x000A15FB
	public void ToggleNetworkedItemStateBool()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(1);
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000A340E File Offset: 0x000A160E
	public void ToggleNetworkedItemStateBoolB()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(2);
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000A3421 File Offset: 0x000A1621
	public void ToggleNetworkedItemStateBoolC()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(4);
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000A3434 File Offset: 0x000A1634
	public void ToggleNetworkedItemStateBoolD()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(8);
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000A3448 File Offset: 0x000A1648
	protected void ResetStateBools()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		if (!this.IsLocalObject())
		{
			return;
		}
		int bitmask = 15;
		this.SetStateBit(false, bitmask);
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000A3473 File Offset: 0x000A1673
	public void SetItemStateBool(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 1);
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000A3487 File Offset: 0x000A1687
	public void SetItemStateBoolB(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 2);
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x000A349B File Offset: 0x000A169B
	public void SetItemStateBoolC(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 4);
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000A34AF File Offset: 0x000A16AF
	public void SetItemStateBoolD(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 8);
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000A34C4 File Offset: 0x000A16C4
	private void SetStateBit(bool value, int bitmask)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		int num = (int)this.itemState;
		if (value)
		{
			num |= bitmask;
		}
		else
		{
			num &= ~bitmask;
		}
		TransferrableObject.ItemStates itemStates = (TransferrableObject.ItemStates)num;
		if (this.itemState != itemStates)
		{
			this.previousItemState = this.itemState;
			this.itemState = itemStates;
			this.OnNetworkItemStateChanged(num);
		}
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000A3514 File Offset: 0x000A1714
	private void ToggleStateBit(int bitmask)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		bool flag = (this.itemState & (TransferrableObject.ItemStates)bitmask) != (TransferrableObject.ItemStates)0;
		int num = (int)this.itemState;
		if (!flag)
		{
			num |= bitmask;
		}
		else
		{
			num &= ~bitmask;
		}
		this.previousItemState = this.itemState;
		this.itemState = (TransferrableObject.ItemStates)num;
		this.OnNetworkItemStateChanged(num);
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000A3560 File Offset: 0x000A1760
	public void SetItemStateInt(int newState)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Int)
		{
			return;
		}
		newState = Mathf.Clamp(newState, 0, 63);
		int num = newState & -65;
		int num2 = (int)(this.itemState & TransferrableObject.ItemStates.Part0Held);
		TransferrableObject.ItemStates itemStates = (TransferrableObject.ItemStates)(num | num2);
		if (this.itemState != itemStates)
		{
			this.previousItemState = this.itemState;
			this.itemState = itemStates;
			this.OnNetworkItemStateChanged(num);
		}
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000A35C4 File Offset: 0x000A17C4
	public virtual void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer != null && toPlayer.Equals(fromPlayer))
		{
			return;
		}
		if (object.Equals(fromPlayer, NetworkSystem.Instance.LocalPlayer) && this.IsHeld())
		{
			this.DropItem();
		}
		if (toPlayer == null)
		{
			this.SetTargetRig(null);
			return;
		}
		this.rigidbodyInstance.useGravity = (this.shouldUseGravity && object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer));
		if (!this.shareable && !this.isSceneObject)
		{
			return;
		}
		if (object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer))
		{
			if (GorillaTagger.Instance == null)
			{
				Debug.LogError("OnOwnershipTransferred has been initiated too quickly, The local player is not ready");
				return;
			}
			this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
			return;
		}
		else
		{
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!vrrig)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
				return;
			}
			this.SetTargetRig(vrrig);
			return;
		}
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000A369C File Offset: 0x000A189C
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return false;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.isSceneObject)
		{
			return false;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x000A3754 File Offset: 0x000A1954
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x000A37F8 File Offset: 0x000A19F8
	public void OnMyOwnerLeft()
	{
		if (this.currentState == TransferrableObject.PositionState.None || this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		this.DropItem();
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			this.anchor.localPosition = Vector3.zero;
			this.anchor.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000A385F File Offset: 0x000A1A5F
	public void OnMyCreatorLeft()
	{
		this.OnItemDestroyedOrDisabled();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000A3874 File Offset: 0x000A1A74
	public bool BuildValidationCheck()
	{
		int num = 0;
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.Chest))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftBack))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightBack))
		{
			num++;
		}
		if (num > 1)
		{
			Debug.LogError("transferrableitem is starting with multiple storedzones: " + base.transform.parent.name, base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x0400288D RID: 10381
	private VRRig _myRig;

	// Token: 0x0400288F RID: 10383
	private VRRig _myOnlineRig;

	// Token: 0x04002891 RID: 10385
	public bool latched;

	// Token: 0x04002892 RID: 10386
	private float indexTrigger;

	// Token: 0x04002893 RID: 10387
	public bool testActivate;

	// Token: 0x04002894 RID: 10388
	public bool testDeactivate;

	// Token: 0x04002895 RID: 10389
	[Tooltip("When the grip/trigger input is greater than this value the transferrable object is activated")]
	public float myThreshold = 0.8f;

	// Token: 0x04002896 RID: 10390
	[Tooltip("When the grip/trigger input is less than (myThreshold - hysterisis) the transferrable object is deactivated")]
	public float hysterisis = 0.05f;

	// Token: 0x04002897 RID: 10391
	[Tooltip("Set the x scale to -1 when held in left hand")]
	public bool flipOnXForLeftHand;

	// Token: 0x04002898 RID: 10392
	[Tooltip("Set the y scale to -1 when held in left hand")]
	public bool flipOnYForLeftHand;

	// Token: 0x04002899 RID: 10393
	[Tooltip("Set the x scale to -1 when docked on left arm")]
	public bool flipOnXForLeftArm;

	// Token: 0x0400289A RID: 10394
	[Tooltip("disable grabbing the item from out of your other hand")]
	public bool disableStealing;

	// Token: 0x0400289B RID: 10395
	[Tooltip("Allow other players to pick up this item")]
	public bool allowPlayerStealing;

	// Token: 0x0400289C RID: 10396
	private TransferrableObject.PositionState initState;

	// Token: 0x0400289D RID: 10397
	public TransferrableObject.ItemStates itemState;

	// Token: 0x0400289E RID: 10398
	protected TransferrableObject.ItemStates previousItemState;

	// Token: 0x0400289F RID: 10399
	protected const int HELD_BIT_MASK = 64;

	// Token: 0x040028A0 RID: 10400
	private const int BOOL_A_BITMASK = 1;

	// Token: 0x040028A1 RID: 10401
	private const int BOOL_B_BITMASK = 2;

	// Token: 0x040028A2 RID: 10402
	private const int BOOL_C_BITMASK = 4;

	// Token: 0x040028A3 RID: 10403
	private const int BOOL_D_BITMASK = 8;

	// Token: 0x040028A4 RID: 10404
	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x040028A5 RID: 10405
	protected TransferrableObject.PositionState previousState;

	// Token: 0x040028A6 RID: 10406
	[DevInspectorYellow]
	[DevInspectorShow]
	public TransferrableObject.PositionState currentState;

	// Token: 0x040028A7 RID: 10407
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x040028A8 RID: 10408
	[DevInspectorCyan]
	[DevInspectorShow]
	public AdvancedItemState advancedGrabState;

	// Token: 0x040028A9 RID: 10409
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig targetRig;

	// Token: 0x040028AA RID: 10410
	[HideInInspector]
	public bool targetRigSet;

	// Token: 0x040028AB RID: 10411
	public TransferrableObject.GrabType useGrabType;

	// Token: 0x040028AC RID: 10412
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig ownerRig;

	// Token: 0x040028AD RID: 10413
	[DebugReadout]
	[NonSerialized]
	public BodyDockPositions targetDockPositions;

	// Token: 0x040028AE RID: 10414
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x040028AF RID: 10415
	public bool canAutoGrabLeft;

	// Token: 0x040028B0 RID: 10416
	public bool canAutoGrabRight;

	// Token: 0x040028B1 RID: 10417
	[DevInspectorShow]
	public int objectIndex;

	// Token: 0x040028B2 RID: 10418
	[NonSerialized]
	public Transform anchor;

	// Token: 0x040028B3 RID: 10419
	[Tooltip("In Functional prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x040028B4 RID: 10420
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x040028B5 RID: 10421
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Left mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseLeft;

	// Token: 0x040028B6 RID: 10422
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Right mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseRight;

	// Token: 0x040028B7 RID: 10423
	[HideInInspector]
	public bool isGrabAnchorSet;

	// Token: 0x040028B8 RID: 10424
	private static Vector3 handPoseRightReferencePoint = new Vector3(-0.0141f, 0.0065f, -0.278f);

	// Token: 0x040028B9 RID: 10425
	private static Quaternion handPoseRightReferenceRotation = Quaternion.Euler(-2.058f, -17.2f, 65.05f);

	// Token: 0x040028BA RID: 10426
	private static Vector3 handPoseLeftReferencePoint = new Vector3(0.0136f, 0.0045f, -0.2809f);

	// Token: 0x040028BB RID: 10427
	private static Quaternion handPoseLeftReferenceRotation = Quaternion.Euler(-0.58f, 21.356f, -63.965f);

	// Token: 0x040028BC RID: 10428
	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	// Token: 0x040028BD RID: 10429
	public int myIndex;

	// Token: 0x040028BE RID: 10430
	[Tooltip("(Optional) objects to enable when held in hand and disable when not in hand")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x040028BF RID: 10431
	[Tooltip("(Optional) objects to disable when held in hand and enable when not in hand")]
	public GameObject[] gameObjectsActiveOnlyWhileDocked;

	// Token: 0x040028C0 RID: 10432
	[Tooltip("(Optional) components to enable when held in hand and disable when not in hand")]
	public Behaviour[] behavioursEnabledOnlyWhileHeld;

	// Token: 0x040028C1 RID: 10433
	[Tooltip("(Optional) components to disable when held in hand and enable when not in hand")]
	public Behaviour[] behavioursEnabledOnlyWhileDocked;

	// Token: 0x040028C2 RID: 10434
	[SerializeField]
	protected internal WorldShareableItem worldShareableInstance;

	// Token: 0x040028C3 RID: 10435
	private float interpTime = 0.2f;

	// Token: 0x040028C4 RID: 10436
	private float interpDt;

	// Token: 0x040028C5 RID: 10437
	private Vector3 interpStartPos;

	// Token: 0x040028C6 RID: 10438
	private Quaternion interpStartRot;

	// Token: 0x040028C7 RID: 10439
	protected int enabledOnFrame = -1;

	// Token: 0x040028C8 RID: 10440
	protected Vector3 initOffset;

	// Token: 0x040028C9 RID: 10441
	protected Quaternion initRotation;

	// Token: 0x040028CA RID: 10442
	private Matrix4x4 initMatrix = Matrix4x4.identity;

	// Token: 0x040028CB RID: 10443
	private Matrix4x4 leftHandMatrix = Matrix4x4.identity;

	// Token: 0x040028CC RID: 10444
	private Matrix4x4 rightHandMatrix = Matrix4x4.identity;

	// Token: 0x040028CD RID: 10445
	private bool positionInitialized;

	// Token: 0x040028CE RID: 10446
	public bool isSceneObject;

	// Token: 0x040028CF RID: 10447
	public Rigidbody rigidbodyInstance;

	// Token: 0x040028D2 RID: 10450
	public bool canDrop;

	// Token: 0x040028D3 RID: 10451
	[Tooltip("completely drop the item instead of auto-returning to a stored zone")]
	public bool allowReparenting;

	// Token: 0x040028D4 RID: 10452
	[Tooltip("(Scene object) has a worldSharableInstance")]
	public bool shareable;

	// Token: 0x040028D5 RID: 10453
	[Tooltip("(Balloon) Unparent this object from the rig when grabbed")]
	public bool detatchOnGrab;

	// Token: 0x040028D6 RID: 10454
	[Tooltip("(Balloon) is this cosmetic droppable in the world")]
	public bool allowWorldSharableInstance;

	// Token: 0x040028D7 RID: 10455
	[ItemCanBeNull]
	public Transform originPoint;

	// Token: 0x040028D8 RID: 10456
	[ItemCanBeNull]
	public float maxDistanceFromOriginBeforeRespawn;

	// Token: 0x040028D9 RID: 10457
	public AudioClip resetPositionAudioClip;

	// Token: 0x040028DA RID: 10458
	public float maxDistanceFromTargetPlayerBeforeRespawn;

	// Token: 0x040028DB RID: 10459
	private bool wasHover;

	// Token: 0x040028DC RID: 10460
	private bool isHover;

	// Token: 0x040028DD RID: 10461
	private bool disableItem;

	// Token: 0x040028DE RID: 10462
	protected bool loaded;

	// Token: 0x040028DF RID: 10463
	public bool ClearLocalPositionOnReset;

	// Token: 0x040028E0 RID: 10464
	[SerializeField]
	protected TransferrableObject.SyncOptions networkedStateEvents;

	// Token: 0x040028E1 RID: 10465
	[SerializeField]
	protected bool resetOnDocked = true;

	// Token: 0x040028E2 RID: 10466
	[SerializeField]
	protected string boolADebugName;

	// Token: 0x040028E3 RID: 10467
	[SerializeField]
	protected UnityEvent OnItemStateBoolTrue;

	// Token: 0x040028E4 RID: 10468
	[SerializeField]
	protected UnityEvent OnItemStateBoolFalse;

	// Token: 0x040028E5 RID: 10469
	[SerializeField]
	protected string boolBDebugName;

	// Token: 0x040028E6 RID: 10470
	[SerializeField]
	protected UnityEvent OnItemStateBoolBTrue;

	// Token: 0x040028E7 RID: 10471
	[SerializeField]
	protected UnityEvent OnItemStateBoolBFalse;

	// Token: 0x040028E8 RID: 10472
	[SerializeField]
	protected string boolCDebugName;

	// Token: 0x040028E9 RID: 10473
	[SerializeField]
	protected UnityEvent OnItemStateBoolCTrue;

	// Token: 0x040028EA RID: 10474
	[SerializeField]
	protected UnityEvent OnItemStateBoolCFalse;

	// Token: 0x040028EB RID: 10475
	[SerializeField]
	protected string boolDDebugName;

	// Token: 0x040028EC RID: 10476
	[SerializeField]
	protected UnityEvent OnItemStateBoolDTrue;

	// Token: 0x040028ED RID: 10477
	[SerializeField]
	protected UnityEvent OnItemStateBoolDFalse;

	// Token: 0x040028EE RID: 10478
	[SerializeField]
	protected UnityEvent<int> OnItemStateIntChanged;

	// Token: 0x040028EF RID: 10479
	[FormerlySerializedAs("OnUndocked")]
	[SerializeField]
	private UnityEvent OnHeldLocal;

	// Token: 0x040028F0 RID: 10480
	[SerializeField]
	private UnityEvent OnHeldShared;

	// Token: 0x040028F1 RID: 10481
	[FormerlySerializedAs("OnDocked")]
	[SerializeField]
	private UnityEvent OnDockedLocal;

	// Token: 0x040028F2 RID: 10482
	[FormerlySerializedAs("OnDockedLocal")]
	[SerializeField]
	private UnityEvent OnDockedShared;

	// Token: 0x040028F3 RID: 10483
	private bool wasHeldLocal;

	// Token: 0x040028F4 RID: 10484
	private bool wasHeldShared;

	// Token: 0x040028F5 RID: 10485
	[Tooltip("(Optional) name broadcast by PlayerGameEvents")]
	public string interactEventName;

	// Token: 0x040028F6 RID: 10486
	public const int kPositionStateCount = 8;

	// Token: 0x040028F7 RID: 10487
	[DevInspectorShow]
	public TransferrableObject.InterpolateState interpState;

	// Token: 0x040028F8 RID: 10488
	public bool startInterpolation;

	// Token: 0x040028F9 RID: 10489
	public Transform InitialDockObject;

	// Token: 0x040028FA RID: 10490
	private AudioSource audioSrc;

	// Token: 0x040028FB RID: 10491
	private bool _isListeningFor_OnPostInstantiateAllPrefabs2;

	// Token: 0x040028FE RID: 10494
	protected Transform _defaultAnchor;

	// Token: 0x040028FF RID: 10495
	protected bool _isDefaultAnchorSet;

	// Token: 0x04002900 RID: 10496
	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	// Token: 0x04002901 RID: 10497
	private bool transferrableItemSlotTransformOverrideApplicable;

	// Token: 0x020004A1 RID: 1185
	public enum SyncOptions
	{
		// Token: 0x04002903 RID: 10499
		None,
		// Token: 0x04002904 RID: 10500
		Bool,
		// Token: 0x04002905 RID: 10501
		Int
	}

	// Token: 0x020004A2 RID: 1186
	public enum ItemStates
	{
		// Token: 0x04002907 RID: 10503
		State0 = 1,
		// Token: 0x04002908 RID: 10504
		State1,
		// Token: 0x04002909 RID: 10505
		State2 = 4,
		// Token: 0x0400290A RID: 10506
		State3 = 8,
		// Token: 0x0400290B RID: 10507
		State4 = 16,
		// Token: 0x0400290C RID: 10508
		State5 = 32,
		// Token: 0x0400290D RID: 10509
		Part0Held = 64,
		// Token: 0x0400290E RID: 10510
		Part1Held = 128
	}

	// Token: 0x020004A3 RID: 1187
	public enum GrabType
	{
		// Token: 0x04002910 RID: 10512
		Default,
		// Token: 0x04002911 RID: 10513
		Free
	}

	// Token: 0x020004A4 RID: 1188
	[Flags]
	public enum PositionState
	{
		// Token: 0x04002913 RID: 10515
		OnLeftArm = 1,
		// Token: 0x04002914 RID: 10516
		OnRightArm = 2,
		// Token: 0x04002915 RID: 10517
		InLeftHand = 4,
		// Token: 0x04002916 RID: 10518
		InRightHand = 8,
		// Token: 0x04002917 RID: 10519
		OnChest = 16,
		// Token: 0x04002918 RID: 10520
		OnLeftShoulder = 32,
		// Token: 0x04002919 RID: 10521
		OnRightShoulder = 64,
		// Token: 0x0400291A RID: 10522
		Dropped = 128,
		// Token: 0x0400291B RID: 10523
		None = 0
	}

	// Token: 0x020004A5 RID: 1189
	public enum InterpolateState
	{
		// Token: 0x0400291D RID: 10525
		None,
		// Token: 0x0400291E RID: 10526
		Interpolating
	}
}
