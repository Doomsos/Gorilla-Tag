using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA3 RID: 4003
	public class RopeSwingManager : NetworkSceneObject
	{
		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06006488 RID: 25736 RVA: 0x0020CA83 File Offset: 0x0020AC83
		// (set) Token: 0x06006489 RID: 25737 RVA: 0x0020CA8A File Offset: 0x0020AC8A
		public static RopeSwingManager instance { get; private set; }

		// Token: 0x0600648A RID: 25738 RVA: 0x0020CA94 File Offset: 0x0020AC94
		private void Awake()
		{
			if (RopeSwingManager.instance != null && RopeSwingManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of RopeSwingManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (RopeSwingManager.instance == null)
			{
				RopeSwingManager.instance = this;
			}
		}

		// Token: 0x0600648B RID: 25739 RVA: 0x0020CAE0 File Offset: 0x0020ACE0
		private void RegisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Add(t.ropeId, t);
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x0020CAF4 File Offset: 0x0020ACF4
		private void UnregisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Remove(t.ropeId);
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x0020CB08 File Offset: 0x0020AD08
		public static void Register(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.RegisterInstance(t);
		}

		// Token: 0x0600648E RID: 25742 RVA: 0x0020CB15 File Offset: 0x0020AD15
		public static void Unregister(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.UnregisterInstance(t);
		}

		// Token: 0x0600648F RID: 25743 RVA: 0x0020CB24 File Offset: 0x0020AD24
		public void SendSetVelocity_RPC(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.photonView.RPC("SetVelocity", 0, new object[]
				{
					ropeId,
					boneIndex,
					velocity,
					wholeRope
				});
				return;
			}
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, default(PhotonMessageInfoWrapped));
		}

		// Token: 0x06006490 RID: 25744 RVA: 0x0020CB8E File Offset: 0x0020AD8E
		public bool TryGetRope(int ropeId, out GorillaRopeSwing result)
		{
			return this.ropes.TryGetValue(ropeId, ref result);
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x0020CBA0 File Offset: 0x0020ADA0
		[PunRPC]
		public void SetVelocity(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
		{
			PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
			this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, info2);
			Utils.Log("Receiving RPC for ropes");
		}

		// Token: 0x06006492 RID: 25746 RVA: 0x0020CBCC File Offset: 0x0020ADCC
		[Rpc]
		public unsafe static void RPC_SetVelocity(NetworkRunner runner, int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != 4)
				{
					int num = 8;
					num += 4;
					num += 4;
					num += 12;
					num += 4;
					if (SimulationMessage.CanAllocateUserPayload(num))
					{
						if (runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)"));
							int num2 = 8;
							*(int*)(ptr2 + num2) = ropeId;
							num2 += 4;
							*(int*)(ptr2 + num2) = boneIndex;
							num2 += 4;
							*(Vector3*)(ptr2 + num2) = velocity;
							num2 += 12;
							ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), wholeRope);
							num2 += 4;
							ptr.Offset = num2 * 8;
							ptr.SetStatic();
							runner.SendRpc(ptr);
						}
						info = RpcInfo.FromLocal(runner, 0, 0);
						goto IL_10;
					}
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)", num);
				}
				return;
			}
			IL_10:
			PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
			RopeSwingManager.instance.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, info2);
		}

		// Token: 0x06006493 RID: 25747 RVA: 0x0020CD64 File Offset: 0x0020AF64
		private void SetVelocityShared(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (info.Sender != null)
			{
				GorillaNot.IncrementRPCCall(info, "SetVelocityShared");
			}
			GorillaRopeSwing gorillaRopeSwing;
			if (this.TryGetRope(ropeId, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				gorillaRopeSwing.SetVelocity(boneIndex, velocity, wholeRope, info);
			}
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x0020CDBC File Offset: 0x0020AFBC
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_SetVelocity@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int ropeId = num2;
			int num3 = *(int*)(ptr + num);
			num += 4;
			int boneIndex = num3;
			Vector3 vector = *(Vector3*)(ptr + num);
			num += 12;
			Vector3 velocity = vector;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
			num += 4;
			bool wholeRope = flag;
			RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
			NetworkBehaviourUtils.InvokeRpc = true;
			RopeSwingManager.RPC_SetVelocity(runner, ropeId, boneIndex, velocity, wholeRope, info);
		}

		// Token: 0x0400743D RID: 29757
		private Dictionary<int, GorillaRopeSwing> ropes = new Dictionary<int, GorillaRopeSwing>();
	}
}
