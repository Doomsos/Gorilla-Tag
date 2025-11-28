using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000BC5 RID: 3013
[Serializable]
public class PhotonSignal<T1, T2, T3, T4, T5> : PhotonSignal
{
	// Token: 0x170006EA RID: 1770
	// (get) Token: 0x06004A98 RID: 19096 RVA: 0x001867CB File Offset: 0x001849CB
	public override int argCount
	{
		get
		{
			return 5;
		}
	}

	// Token: 0x14000084 RID: 132
	// (add) Token: 0x06004A99 RID: 19097 RVA: 0x001867CE File Offset: 0x001849CE
	// (remove) Token: 0x06004A9A RID: 19098 RVA: 0x00186802 File Offset: 0x00184A02
	public new event OnSignalReceived<T1, T2, T3, T4, T5> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06004A9B RID: 19099 RVA: 0x001861D7 File Offset: 0x001843D7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06004A9C RID: 19100 RVA: 0x001861E0 File Offset: 0x001843E0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06004A9D RID: 19101 RVA: 0x0018681F File Offset: 0x00184A1F
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06004A9E RID: 19102 RVA: 0x0018682E File Offset: 0x00184A2E
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x06004A9F RID: 19103 RVA: 0x00186844 File Offset: 0x00184A44
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
		array[4] = arg3;
		array[5] = arg4;
		array[6] = arg5;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004AA0 RID: 19104 RVA: 0x00186908 File Offset: 0x00184B08
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		T4 arg4;
		T5 arg5;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3, out arg4, out arg5))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4, T5>(this._callbacks, arg, arg2, arg3, arg4, arg5, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4, T5>(this._callbacks, arg, arg2, arg3, arg4, arg5, info);
	}

	// Token: 0x06004AA1 RID: 19105 RVA: 0x00186956 File Offset: 0x00184B56
	public new static implicit operator PhotonSignal<T1, T2, T3, T4, T5>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(s);
	}

	// Token: 0x06004AA2 RID: 19106 RVA: 0x0018695E File Offset: 0x00184B5E
	public new static explicit operator PhotonSignal<T1, T2, T3, T4, T5>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(i);
	}

	// Token: 0x04005AE6 RID: 23270
	private OnSignalReceived<T1, T2, T3, T4, T5> _callbacks;

	// Token: 0x04005AE7 RID: 23271
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4, T5>).FullName.GetStaticHash();
}
