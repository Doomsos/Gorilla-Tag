using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000436 RID: 1078
[NetworkBehaviourWeaved(0)]
public class WorldShareableItem : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001A69 RID: 6761 RVA: 0x0008C778 File Offset: 0x0008A978
	// (set) Token: 0x06001A6A RID: 6762 RVA: 0x0008C780 File Offset: 0x0008A980
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001A6B RID: 6763 RVA: 0x0008C789 File Offset: 0x0008A989
	// (set) Token: 0x06001A6C RID: 6764 RVA: 0x0008C791 File Offset: 0x0008A991
	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001A6D RID: 6765 RVA: 0x0008C79A File Offset: 0x0008A99A
	// (set) Token: 0x06001A6E RID: 6766 RVA: 0x0008C7A2 File Offset: 0x0008A9A2
	public TransferrableObject.PositionState transferableObjectStateNetworked { get; set; }

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06001A6F RID: 6767 RVA: 0x0008C7AB File Offset: 0x0008A9AB
	// (set) Token: 0x06001A70 RID: 6768 RVA: 0x0008C7B3 File Offset: 0x0008A9B3
	public TransferrableObject.ItemStates transferableObjectItemStateNetworked { get; set; }

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06001A71 RID: 6769 RVA: 0x0008C7BC File Offset: 0x0008A9BC
	// (set) Token: 0x06001A72 RID: 6770 RVA: 0x0008C7C4 File Offset: 0x0008A9C4
	[DevInspectorShow]
	public WorldTargetItem target
	{
		get
		{
			return this._target;
		}
		set
		{
			this._target = value;
		}
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x0008C7CD File Offset: 0x0008A9CD
	protected override void Awake()
	{
		base.Awake();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x0008C7FD File Offset: 0x0008A9FD
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		if (GTAppState.isQuitting)
		{
			return;
		}
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
		WorldShareableItemManager.Register(this);
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x0008C838 File Offset: 0x0008AA38
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		if (this.target == null || !this.target.transferrableObject.isSceneObject)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
		this.guard.RemoveCallbackTarget(this);
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x0008C8B8 File Offset: 0x0008AAB8
	public void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x0008C8C8 File Offset: 0x0008AAC8
	public void SetupSharableViewIDs(NetPlayer player, int slotID)
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		this.guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			this.guard.SetCreator(player);
		}
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x0008C954 File Offset: 0x0008AB54
	public void ResetViews()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x0008C984 File Offset: 0x0008AB84
	public void SetupSharableObject(int itemIDx, NetPlayer owner, Transform targetXform)
	{
		if (this.target != null)
		{
			Debug.LogError("ERROR!!!  WorldShareableItem.SetupSharableObject: target is expected to be null before this call. In scene path = \"" + base.transform.GetPathQ() + "\"", this);
			return;
		}
		this.target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (this.target.targetObject != targetXform)
		{
			Debug.LogError(string.Format("The target object found a transform that does not match the target transform, this should never happen. owner: {0} itemIDx: {1} targetXformPath: {2}, target.targetObject: {3}", new object[]
			{
				owner,
				itemIDx,
				targetXform.GetPath(),
				this.target.targetObject.GetPath()
			}));
		}
		TransferrableObject component = this.target.targetObject.GetComponent<TransferrableObject>();
		this.validShareable = (component.canDrop || component.shareable || component.allowWorldSharableInstance);
		if (!this.validShareable)
		{
			Debug.LogError(string.Format("tried to setup an invalid shareable {0} {1} {2}", owner, itemIDx, targetXform.GetPath()));
			base.gameObject.SetActive(false);
			this.Invalidate();
			return;
		}
		this.guard.AddCallbackTarget(component);
		this.guard.giveCreatorAbsoluteAuthority = true;
		component.SetWorldShareableItem(this);
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x0008CA9E File Offset: 0x0008AC9E
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x0008CAA8 File Offset: 0x0008ACA8
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(newOwner);
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(previousOwner);
			this.onOwnerChangeCb(player, player2);
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001A7C RID: 6780 RVA: 0x0008CAE2 File Offset: 0x0008ACE2
	// (set) Token: 0x06001A7D RID: 6781 RVA: 0x0008CAEA File Offset: 0x0008ACEA
	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x0008CAF4 File Offset: 0x0008ACF4
	public void TriggeredUpdate()
	{
		if (!this.IsTargetValid())
		{
			return;
		}
		if (this.guard.isTrulyMine)
		{
			Vector3 vector;
			Quaternion quaternion;
			this.target.targetObject.GetPositionAndRotation(ref vector, ref quaternion);
			base.transform.SetPositionAndRotation(vector, quaternion);
			return;
		}
		if (!base.IsMine && this.EnableRemoteSync)
		{
			Vector3 vector2;
			Quaternion quaternion2;
			base.transform.GetPositionAndRotation(ref vector2, ref quaternion2);
			this.target.targetObject.SetPositionAndRotation(vector2, quaternion2);
		}
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x0008CB6A File Offset: 0x0008AD6A
	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x0008CB8C File Offset: 0x0008AD8C
	public void SetupSceneObjectOnNetwork(NetPlayer owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x0008CB9C File Offset: 0x0008AD9C
	public bool IsTargetValid()
	{
		return this.target != null;
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x0008CBA7 File Offset: 0x0008ADA7
	public void Invalidate()
	{
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x0008CBC0 File Offset: 0x0008ADC0
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null)
		{
			return;
		}
		WorldShareableItem.CachedData cachedData;
		if (this.cachedDatas.TryGetValue(toPlayer, ref cachedData))
		{
			this.transferableObjectState = cachedData.cachedTransferableObjectState;
			this.transferableObjectItemState = cachedData.cachedTransferableObjectItemState;
			this.cachedDatas.Remove(toPlayer);
		}
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x0008CC06 File Offset: 0x0008AE06
	public override void WriteDataFusion()
	{
		this.transferableObjectItemStateNetworked = this.transferableObjectItemState;
		this.transferableObjectStateNetworked = this.transferableObjectState;
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x0008CC20 File Offset: 0x0008AE20
	public override void ReadDataFusion()
	{
		this.transferableObjectItemState = this.transferableObjectItemStateNetworked;
		this.transferableObjectState = this.transferableObjectStateNetworked;
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x0008CC3A File Offset: 0x0008AE3A
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.transferableObjectState);
		stream.SendNext(this.transferableObjectItemState);
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x0008CC60 File Offset: 0x0008AE60
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player != this.guard.actualOwner)
		{
			Debug.Log("Blocking info from non owner");
			this.cachedDatas.AddOrUpdate(player, new WorldShareableItem.CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
			return;
		}
		this.transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x0008CCF2 File Offset: 0x0008AEF2
	[PunRPC]
	internal void RPCWorldShareable(PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		if (this.rpcCallBack == null)
		{
			return;
		}
		this.rpcCallBack.Invoke();
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x00027DED File Offset: 0x00025FED
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return true;
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x00027DED File Offset: 0x00025FED
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return true;
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x0008CD24 File Offset: 0x0008AF24
	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040023FF RID: 9215
	private bool validShareable = true;

	// Token: 0x04002400 RID: 9216
	public RequestableOwnershipGuard guard;

	// Token: 0x04002401 RID: 9217
	private TransformViewTeleportSerializer teleportSerializer;

	// Token: 0x04002402 RID: 9218
	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem _target;

	// Token: 0x04002403 RID: 9219
	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x04002404 RID: 9220
	public Action rpcCallBack;

	// Token: 0x04002405 RID: 9221
	private bool enableRemoteSync = true;

	// Token: 0x04002406 RID: 9222
	public Dictionary<NetPlayer, WorldShareableItem.CachedData> cachedDatas = new Dictionary<NetPlayer, WorldShareableItem.CachedData>();

	// Token: 0x02000437 RID: 1079
	// (Invoke) Token: 0x06001A92 RID: 6802
	public delegate void Delegate();

	// Token: 0x02000438 RID: 1080
	// (Invoke) Token: 0x06001A96 RID: 6806
	public delegate void OnOwnerChangeDelegate(NetPlayer newOwner, NetPlayer prevOwner);

	// Token: 0x02000439 RID: 1081
	public struct CachedData
	{
		// Token: 0x04002407 RID: 9223
		public TransferrableObject.PositionState cachedTransferableObjectState;

		// Token: 0x04002408 RID: 9224
		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
