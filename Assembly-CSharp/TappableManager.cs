using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000835 RID: 2101
public class TappableManager : NetworkSceneObject
{
	// Token: 0x06003746 RID: 14150 RVA: 0x00129B30 File Offset: 0x00127D30
	private void Awake()
	{
		if (TappableManager.gManager != null && TappableManager.gManager != this)
		{
			GTDev.LogWarning<string>("Instance of TappableManager already exists. Destroying.", null);
			Object.Destroy(this);
			return;
		}
		if (TappableManager.gManager == null)
		{
			TappableManager.gManager = this;
		}
		if (TappableManager.gRegistry.Count == 0)
		{
			return;
		}
		Tappable[] array = Enumerable.ToArray<Tappable>(TappableManager.gRegistry);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == null))
			{
				this.RegisterInstance(array[i]);
			}
		}
		TappableManager.gRegistry.Clear();
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x00129BC0 File Offset: 0x00127DC0
	private void RegisterInstance(Tappable t)
	{
		if (t == null)
		{
			GTDev.LogError<string>("Tappable is null.", null);
			return;
		}
		t.manager = this;
		if (this.idSet.Add(t.tappableId))
		{
			this.tappables.Add(t);
		}
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x00129BFD File Offset: 0x00127DFD
	private void UnregisterInstance(Tappable t)
	{
		if (t == null)
		{
			return;
		}
		if (!this.idSet.Remove(t.tappableId))
		{
			return;
		}
		this.tappables.Remove(t);
		t.manager = null;
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x00129C31 File Offset: 0x00127E31
	public static void Register(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.RegisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Add(t);
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x00129C58 File Offset: 0x00127E58
	public static void Unregister(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.UnregisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Remove(t);
	}

	// Token: 0x0600374B RID: 14155 RVA: 0x00129C80 File Offset: 0x00127E80
	[Conditional("QATESTING")]
	public void DebugTestTap()
	{
		if (this.tappables.Count > 0)
		{
			int num = Random.Range(0, this.tappables.Count);
			Debug.Log("Send TestTap to tappable index: " + num.ToString() + "/" + this.tappables.Count.ToString());
			this.tappables[num].OnTap(10f);
			return;
		}
		Debug.Log("TappableManager: tappables array is empty.");
	}

	// Token: 0x0600374C RID: 14156 RVA: 0x00129CFC File Offset: 0x00127EFC
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		this.SendOnTapShared(key, tapStrength, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600374D RID: 14157 RVA: 0x00129D0C File Offset: 0x00127F0C
	[Rpc]
	public unsafe static void RPC_SendOnTap(NetworkRunner runner, int key, float tapStrength, RpcInfo info = default(RpcInfo))
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
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void TappableManager::RPC_SendOnTap(Fusion.NetworkRunner,System.Int32,System.Single,Fusion.RpcInfo)"));
						int num2 = 8;
						*(int*)(ptr2 + num2) = key;
						num2 += 4;
						*(float*)(ptr2 + num2) = tapStrength;
						num2 += 4;
						ptr.Offset = num2 * 8;
						ptr.SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, 0, 0);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void TappableManager::RPC_SendOnTap(Fusion.NetworkRunner,System.Int32,System.Single,Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		TappableManager.gManager.SendOnTapShared(key, tapStrength, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x00129E48 File Offset: 0x00128048
	private void SendOnTapShared(int key, float tapStrength, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnTapShared");
		if (key == 0 || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
			if (tappable.tappableId == key)
			{
				tappable.OnTapLocal(tapStrength, Time.time, info);
			}
		}
	}

	// Token: 0x0600374F RID: 14159 RVA: 0x00129EB7 File Offset: 0x001280B7
	[PunRPC]
	public void SendOnGrabRPC(int key, PhotonMessageInfo info)
	{
		this.SendOnGrabShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x00129EC8 File Offset: 0x001280C8
	[Rpc]
	public unsafe static void RPC_SendOnGrab(NetworkRunner runner, int key, RpcInfo info = default(RpcInfo))
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
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void TappableManager::RPC_SendOnGrab(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)"));
						int num2 = 8;
						*(int*)(ptr2 + num2) = key;
						num2 += 4;
						ptr.Offset = num2 * 8;
						ptr.SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, 0, 0);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void TappableManager::RPC_SendOnGrab(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		TappableManager.gManager.SendOnGrabShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x00129FE0 File Offset: 0x001281E0
	private void SendOnGrabShared(int key, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnGrabShared");
		if (key == 0)
		{
			return;
		}
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
			if (tappable.tappableId == key)
			{
				tappable.OnGrabLocal(Time.time, info);
			}
		}
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x0012A034 File Offset: 0x00128234
	[PunRPC]
	public void SendOnReleaseRPC(int key, PhotonMessageInfo info)
	{
		this.SendOnReleaseShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x0012A044 File Offset: 0x00128244
	[Rpc]
	public unsafe static void RPC_SendOnRelease(NetworkRunner runner, int key, RpcInfo info = default(RpcInfo))
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
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void TappableManager::RPC_SendOnRelease(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)"));
						int num2 = 8;
						*(int*)(ptr2 + num2) = key;
						num2 += 4;
						ptr.Offset = num2 * 8;
						ptr.SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, 0, 0);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void TappableManager::RPC_SendOnRelease(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		TappableManager.gManager.SendOnReleaseShared(key, new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0012A15C File Offset: 0x0012835C
	public void SendOnReleaseShared(int key, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnReleaseShared");
		if (key == 0)
		{
			return;
		}
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
			if (tappable.tappableId == key)
			{
				tappable.OnReleaseLocal(Time.time, info);
			}
		}
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x0012A1DC File Offset: 0x001283DC
	[NetworkRpcStaticWeavedInvoker("System.Void TappableManager::RPC_SendOnTap(Fusion.NetworkRunner,System.Int32,System.Single,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendOnTap@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int key = num2;
		float num3 = *(float*)(ptr + num);
		num += 4;
		float tapStrength = num3;
		RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
		NetworkBehaviourUtils.InvokeRpc = true;
		TappableManager.RPC_SendOnTap(runner, key, tapStrength, info);
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x0012A258 File Offset: 0x00128458
	[NetworkRpcStaticWeavedInvoker("System.Void TappableManager::RPC_SendOnGrab(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendOnGrab@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int key = num2;
		RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
		NetworkBehaviourUtils.InvokeRpc = true;
		TappableManager.RPC_SendOnGrab(runner, key, info);
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x0012A2B8 File Offset: 0x001284B8
	[NetworkRpcStaticWeavedInvoker("System.Void TappableManager::RPC_SendOnRelease(Fusion.NetworkRunner,System.Int32,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_SendOnRelease@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int key = num2;
		RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
		NetworkBehaviourUtils.InvokeRpc = true;
		TappableManager.RPC_SendOnRelease(runner, key, info);
	}

	// Token: 0x040046B1 RID: 18097
	private static TappableManager gManager;

	// Token: 0x040046B2 RID: 18098
	[SerializeField]
	private List<Tappable> tappables = new List<Tappable>();

	// Token: 0x040046B3 RID: 18099
	private HashSet<int> idSet = new HashSet<int>();

	// Token: 0x040046B4 RID: 18100
	private static HashSet<Tappable> gRegistry = new HashSet<Tappable>();
}
