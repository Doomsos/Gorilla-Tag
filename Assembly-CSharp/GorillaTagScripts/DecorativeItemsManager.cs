using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000DDC RID: 3548
	[NetworkBehaviourWeaved(1)]
	public class DecorativeItemsManager : NetworkComponent
	{
		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x06005816 RID: 22550 RVA: 0x001C24AC File Offset: 0x001C06AC
		public static DecorativeItemsManager Instance
		{
			get
			{
				return DecorativeItemsManager._instance;
			}
		}

		// Token: 0x06005817 RID: 22551 RVA: 0x001C24B4 File Offset: 0x001C06B4
		protected override void Awake()
		{
			base.Awake();
			if (DecorativeItemsManager._instance != null && DecorativeItemsManager._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				DecorativeItemsManager._instance = this;
			}
			this.currentIndex = -1;
			this.shouldRunUpdate = true;
			this.zone = base.GetComponent<ZoneBasedObject>();
			foreach (DecorativeItem decorativeItem in this.decorativeItemsContainer.GetComponentsInChildren<DecorativeItem>(false))
			{
				if (decorativeItem)
				{
					this.itemsList.Add(decorativeItem);
					DecorativeItem decorativeItem2 = decorativeItem;
					decorativeItem2.respawnItem = (UnityAction<DecorativeItem>)Delegate.Combine(decorativeItem2.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
				}
			}
			foreach (AttachPoint attachPoint in this.respawnableHooksContainer.GetComponentsInChildren<AttachPoint>(false))
			{
				if (attachPoint)
				{
					this.respawnableHooks.Add(attachPoint);
				}
			}
			this.allHooks.AddRange(this.respawnableHooks);
			foreach (GameObject gameObject in this.nonRespawnableHooksContainer)
			{
				foreach (AttachPoint attachPoint2 in gameObject.GetComponentsInChildren<AttachPoint>(false))
				{
					if (attachPoint2)
					{
						this.allHooks.Add(attachPoint2);
					}
				}
			}
		}

		// Token: 0x06005818 RID: 22552 RVA: 0x001C261C File Offset: 0x001C081C
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (DecorativeItem decorativeItem in this.itemsList)
			{
				decorativeItem.respawnItem = (UnityAction<DecorativeItem>)Delegate.Remove(decorativeItem.respawnItem, new UnityAction<DecorativeItem>(this.OnRequestToRespawn));
			}
			this.itemsList.Clear();
			this.respawnableHooks.Clear();
			if (DecorativeItemsManager._instance == this)
			{
				DecorativeItemsManager._instance = null;
			}
		}

		// Token: 0x06005819 RID: 22553 RVA: 0x001C26B8 File Offset: 0x001C08B8
		private void Update()
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			if (this.wasInZone != this.zone.IsLocalPlayerInZone())
			{
				this.shouldRunUpdate = true;
			}
			if (!this.shouldRunUpdate)
			{
				return;
			}
			if (base.IsMine)
			{
				if (this.wasInZone != this.zone.IsLocalPlayerInZone())
				{
					foreach (AttachPoint attachPoint in this.allHooks)
					{
						attachPoint.SetIsHook(false);
					}
					for (int i = 0; i < this.itemsList.Count; i++)
					{
						this.itemsList[i].itemState = TransferrableObject.ItemStates.State2;
						this.SpawnItem(i);
					}
					this.shouldRunUpdate = false;
				}
				this.wasInZone = this.zone.IsLocalPlayerInZone();
				this.SpawnItem(this.UpdateListPerFrame());
			}
		}

		// Token: 0x0600581A RID: 22554 RVA: 0x001C27A8 File Offset: 0x001C09A8
		private void SpawnItem(int index)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (index < 0 || index >= this.itemsList.Count)
			{
				return;
			}
			if (this.respawnableHooks == null)
			{
				return;
			}
			if (this.itemsList == null)
			{
				return;
			}
			if (this.itemsList.Count > this.respawnableHooks.Count)
			{
				Debug.LogError("Trying to snap more decorative items than allowed! Some items will be left un-hooked!");
				return;
			}
			Transform transform = this.RandomSpawn();
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			DecorativeItem decorativeItem = this.itemsList[index];
			decorativeItem.WorldShareableRequestOwnership();
			decorativeItem.Respawn(position, rotation);
			base.SendRPC("RespawnItemRPC", 1, new object[]
			{
				index,
				position,
				rotation
			});
		}

		// Token: 0x0600581B RID: 22555 RVA: 0x001C286F File Offset: 0x001C0A6F
		[PunRPC]
		private void RespawnItemRPC(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfo info)
		{
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x0600581C RID: 22556 RVA: 0x001C2884 File Offset: 0x001C0A84
		[Rpc]
		private unsafe void RPC_RespawnItem(int index, Vector3 _transformPos, Quaternion _transformRot, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != 4)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.DecorativeItemsManager::RPC_RespawnItem(System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						int num = 8;
						num += 4;
						num += 12;
						num += 16;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.DecorativeItemsManager::RPC_RespawnItem(System.Int32,UnityEngine.Vector3,UnityEngine.Quaternion,Fusion.RpcInfo)", num);
						}
						else
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								*(int*)(ptr2 + num2) = index;
								num2 += 4;
								*(Vector3*)(ptr2 + num2) = _transformPos;
								num2 += 12;
								*(Quaternion*)(ptr2 + num2) = _transformRot;
								num2 += 16;
								ptr.Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 7) != 0)
							{
								info = RpcInfo.FromLocal(base.Runner, 0, 0);
								goto IL_12;
							}
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.RespawnItemShared(index, _transformPos, _transformRot, info);
		}

		// Token: 0x0600581D RID: 22557 RVA: 0x001C2A44 File Offset: 0x001C0C44
		protected void RespawnItemShared(int index, Vector3 _transformPos, Quaternion _transformRot, PhotonMessageInfoWrapped info)
		{
			if (index >= 0 && index <= this.itemsList.Count - 1)
			{
				float num = 10000f;
				if (_transformPos.IsValid(num) && _transformRot.IsValid() && info.Sender == NetworkSystem.Instance.MasterClient)
				{
					GorillaNot.IncrementRPCCall(info, "RespawnItemRPC");
					this.itemsList[index].Respawn(_transformPos, _transformRot);
					return;
				}
			}
		}

		// Token: 0x0600581E RID: 22558 RVA: 0x001C2AB4 File Offset: 0x001C0CB4
		private Transform RandomSpawn()
		{
			this.lastIndex = this.currentIndex;
			bool flag = false;
			bool flag2 = this.zone.IsLocalPlayerInZone();
			int num = Random.Range(0, this.respawnableHooks.Count);
			while (!flag)
			{
				num = Random.Range(0, this.respawnableHooks.Count);
				if (!this.respawnableHooks[num].inForest == flag2)
				{
					flag = true;
				}
			}
			if (!this.respawnableHooks[num].IsHooked())
			{
				this.currentIndex = num;
			}
			else
			{
				this.currentIndex = -1;
			}
			if (this.currentIndex != this.lastIndex && this.currentIndex > -1)
			{
				return this.respawnableHooks[this.currentIndex].attachPoint;
			}
			this.currentIndex = -1;
			return null;
		}

		// Token: 0x0600581F RID: 22559 RVA: 0x001C2B76 File Offset: 0x001C0D76
		private int UpdateListPerFrame()
		{
			this.arrayIndex++;
			if (this.arrayIndex >= this.itemsList.Count || this.arrayIndex < 0)
			{
				this.shouldRunUpdate = false;
				return -1;
			}
			return this.arrayIndex;
		}

		// Token: 0x06005820 RID: 22560 RVA: 0x001C2BB4 File Offset: 0x001C0DB4
		private void OnRequestToRespawn(DecorativeItem item)
		{
			if (base.IsMine)
			{
				if (item == null)
				{
					return;
				}
				int index = this.itemsList.IndexOf(item);
				this.SpawnItem(index);
			}
		}

		// Token: 0x06005821 RID: 22561 RVA: 0x001C2BE8 File Offset: 0x001C0DE8
		public AttachPoint getCurrentAttachPointByPosition(Vector3 _attachPoint)
		{
			foreach (AttachPoint attachPoint in this.allHooks)
			{
				if (attachPoint.attachPoint.position == _attachPoint)
				{
					return attachPoint;
				}
			}
			return null;
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x06005822 RID: 22562 RVA: 0x001C2C50 File Offset: 0x001C0E50
		// (set) Token: 0x06005823 RID: 22563 RVA: 0x001C2C76 File Offset: 0x001C0E76
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return this.Ptr[0];
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing DecorativeItemsManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				this.Ptr[0] = value;
			}
		}

		// Token: 0x06005824 RID: 22564 RVA: 0x001C2C9D File Offset: 0x001C0E9D
		public override void WriteDataFusion()
		{
			this.Data = this.currentIndex;
		}

		// Token: 0x06005825 RID: 22565 RVA: 0x001C2CAB File Offset: 0x001C0EAB
		public override void ReadDataFusion()
		{
			this.currentIndex = this.Data;
		}

		// Token: 0x06005826 RID: 22566 RVA: 0x001C2CB9 File Offset: 0x001C0EB9
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentIndex);
		}

		// Token: 0x06005827 RID: 22567 RVA: 0x001C2CDA File Offset: 0x001C0EDA
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			this.currentIndex = (int)stream.ReceiveNext();
		}

		// Token: 0x06005829 RID: 22569 RVA: 0x001C2D36 File Offset: 0x001C0F36
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600582A RID: 22570 RVA: 0x001C2D4E File Offset: 0x001C0F4E
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0600582B RID: 22571 RVA: 0x001C2D64 File Offset: 0x001C0F64
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RespawnItem@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int index = num2;
			Vector3 vector = *(Vector3*)(ptr + num);
			num += 12;
			Vector3 transformPos = vector;
			Quaternion quaternion = *(Quaternion*)(ptr + num);
			num += 16;
			Quaternion transformRot = quaternion;
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
			behaviour.InvokeRpc = true;
			((DecorativeItemsManager)behaviour).RPC_RespawnItem(index, transformPos, transformRot, info);
		}

		// Token: 0x04006569 RID: 25961
		public GameObject decorativeItemsContainer;

		// Token: 0x0400656A RID: 25962
		public GameObject respawnableHooksContainer;

		// Token: 0x0400656B RID: 25963
		public List<GameObject> nonRespawnableHooksContainer = new List<GameObject>();

		// Token: 0x0400656C RID: 25964
		private readonly List<DecorativeItem> itemsList = new List<DecorativeItem>();

		// Token: 0x0400656D RID: 25965
		private readonly List<AttachPoint> respawnableHooks = new List<AttachPoint>();

		// Token: 0x0400656E RID: 25966
		private readonly List<AttachPoint> allHooks = new List<AttachPoint>();

		// Token: 0x0400656F RID: 25967
		private int lastIndex;

		// Token: 0x04006570 RID: 25968
		private int currentIndex;

		// Token: 0x04006571 RID: 25969
		private int arrayIndex = -1;

		// Token: 0x04006572 RID: 25970
		private bool shouldRunUpdate;

		// Token: 0x04006573 RID: 25971
		private ZoneBasedObject zone;

		// Token: 0x04006574 RID: 25972
		private bool wasInZone;

		// Token: 0x04006575 RID: 25973
		[OnEnterPlay_SetNull]
		private static DecorativeItemsManager _instance;

		// Token: 0x04006576 RID: 25974
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private int _Data;
	}
}
