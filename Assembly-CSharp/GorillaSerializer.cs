using System;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000757 RID: 1879
[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	// Token: 0x0600308E RID: 12430 RVA: 0x00109C54 File Offset: 0x00107E54
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, info))
		{
			return;
		}
		if (stream.IsReading)
		{
			this.serializeTarget.OnSerializeRead(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeWrite(stream, info);
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x00109CA0 File Offset: 0x00107EA0
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.photonView == null)
		{
			return;
		}
		this.successfullInstantiate = this.OnInstantiateSetup(info, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			if (this.targetType != null && this.targetObject.IsNotNull())
			{
				IGorillaSerializeable gorillaSerializeable = this.targetObject.GetComponent(this.targetType) as IGorillaSerializeable;
				if (gorillaSerializeable != null)
				{
					this.serializeTarget = gorillaSerializeable;
				}
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccessfullInstantiate(info);
			return;
		}
		if (PhotonNetwork.InRoom && this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(this.photonView);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.photonView.ObservedComponents.Remove(this);
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x00109D76 File Offset: 0x00107F76
	protected virtual bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IGorillaSerializeable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x00109D8D File Offset: 0x00107F8D
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x00109DA5 File Offset: 0x00107FA5
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T result = base.gameObject.AddComponent<T>();
		this.photonView.RefreshRpcMonoBehaviourCache();
		return result;
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x00109DC0 File Offset: 0x00107FC0
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget rpcTarget = targetOthers ? 1 : 2;
		this.photonView.RPC(rpcName, rpcTarget, data);
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x00109DE3 File Offset: 0x00107FE3
	public void SendRPC(string rpcName, Player targetPlayer, params object[] data)
	{
		this.photonView.RPC(rpcName, targetPlayer, data);
	}

	// Token: 0x04003F9E RID: 16286
	protected bool successfullInstantiate;

	// Token: 0x04003F9F RID: 16287
	protected IGorillaSerializeable serializeTarget;

	// Token: 0x04003FA0 RID: 16288
	private Type targetType;

	// Token: 0x04003FA1 RID: 16289
	protected GameObject targetObject;

	// Token: 0x04003FA2 RID: 16290
	[SerializeField]
	protected PhotonView photonView;
}
