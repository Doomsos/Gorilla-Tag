using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000BC2 RID: 3010
[Serializable]
public class PhotonSignal<T1, T2> : PhotonSignal
{
	// Token: 0x170006E7 RID: 1767
	// (get) Token: 0x06004A74 RID: 19060 RVA: 0x000126CB File Offset: 0x000108CB
	public override int argCount
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x06004A75 RID: 19061 RVA: 0x001862EF File Offset: 0x001844EF
	// (remove) Token: 0x06004A76 RID: 19062 RVA: 0x00186323 File Offset: 0x00184523
	public new event OnSignalReceived<T1, T2> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06004A77 RID: 19063 RVA: 0x001861B7 File Offset: 0x001843B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06004A78 RID: 19064 RVA: 0x001861C0 File Offset: 0x001843C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06004A79 RID: 19065 RVA: 0x00186340 File Offset: 0x00184540
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06004A7A RID: 19066 RVA: 0x0018634F File Offset: 0x0018454F
	public void Raise(T1 arg1, T2 arg2)
	{
		this.Raise(this._receivers, arg1, arg2);
	}

	// Token: 0x06004A7B RID: 19067 RVA: 0x00186360 File Offset: 0x00184560
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		RaiseEventOptions raiseEventOptions = PhotonSignal.gGroupToOptions[receivers];
		object[] array = PhotonUtils.FetchScratchArray(2 + this.argCount);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		array[2] = arg1;
		array[3] = arg2;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004A7C RID: 19068 RVA: 0x00186408 File Offset: 0x00184608
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		if (!args.TryParseArgs(2, out arg, out arg2))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2>(this._callbacks, arg, arg2, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2>(this._callbacks, arg, arg2, info);
	}

	// Token: 0x06004A7D RID: 19069 RVA: 0x00186448 File Offset: 0x00184648
	public new static implicit operator PhotonSignal<T1, T2>(string s)
	{
		return new PhotonSignal<T1, T2>(s);
	}

	// Token: 0x06004A7E RID: 19070 RVA: 0x00186450 File Offset: 0x00184650
	public new static explicit operator PhotonSignal<T1, T2>(int i)
	{
		return new PhotonSignal<T1, T2>(i);
	}

	// Token: 0x04005AE0 RID: 23264
	private OnSignalReceived<T1, T2> _callbacks;

	// Token: 0x04005AE1 RID: 23265
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2>).FullName.GetStaticHash();
}
