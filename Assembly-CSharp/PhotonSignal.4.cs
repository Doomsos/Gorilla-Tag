using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000BC3 RID: 3011
[Serializable]
public class PhotonSignal<T1, T2, T3> : PhotonSignal
{
	// Token: 0x170006E8 RID: 1768
	// (get) Token: 0x06004A80 RID: 19072 RVA: 0x00114802 File Offset: 0x00112A02
	public override int argCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x14000082 RID: 130
	// (add) Token: 0x06004A81 RID: 19073 RVA: 0x00186473 File Offset: 0x00184673
	// (remove) Token: 0x06004A82 RID: 19074 RVA: 0x001864A7 File Offset: 0x001846A7
	public new event OnSignalReceived<T1, T2, T3> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06004A83 RID: 19075 RVA: 0x001861B7 File Offset: 0x001843B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06004A84 RID: 19076 RVA: 0x001861C0 File Offset: 0x001843C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06004A85 RID: 19077 RVA: 0x001864C4 File Offset: 0x001846C4
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06004A86 RID: 19078 RVA: 0x001864D3 File Offset: 0x001846D3
	public void Raise(T1 arg1, T2 arg2, T3 arg3)
	{
		this.Raise(this._receivers, arg1, arg2, arg3);
	}

	// Token: 0x06004A87 RID: 19079 RVA: 0x001864E4 File Offset: 0x001846E4
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004A88 RID: 19080 RVA: 0x00186594 File Offset: 0x00184794
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3>(this._callbacks, arg, arg2, arg3, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3>(this._callbacks, arg, arg2, arg3, info);
	}

	// Token: 0x06004A89 RID: 19081 RVA: 0x001865D8 File Offset: 0x001847D8
	public new static implicit operator PhotonSignal<T1, T2, T3>(string s)
	{
		return new PhotonSignal<T1, T2, T3>(s);
	}

	// Token: 0x06004A8A RID: 19082 RVA: 0x001865E0 File Offset: 0x001847E0
	public new static explicit operator PhotonSignal<T1, T2, T3>(int i)
	{
		return new PhotonSignal<T1, T2, T3>(i);
	}

	// Token: 0x04005AE2 RID: 23266
	private OnSignalReceived<T1, T2, T3> _callbacks;

	// Token: 0x04005AE3 RID: 23267
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3>).FullName.GetStaticHash();
}
