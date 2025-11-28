using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000BC4 RID: 3012
[Serializable]
public class PhotonSignal<T1, T2, T3, T4> : PhotonSignal
{
	// Token: 0x170006E9 RID: 1769
	// (get) Token: 0x06004A8C RID: 19084 RVA: 0x00186623 File Offset: 0x00184823
	public override int argCount
	{
		get
		{
			return 4;
		}
	}

	// Token: 0x14000083 RID: 131
	// (add) Token: 0x06004A8D RID: 19085 RVA: 0x00186626 File Offset: 0x00184826
	// (remove) Token: 0x06004A8E RID: 19086 RVA: 0x0018665A File Offset: 0x0018485A
	public new event OnSignalReceived<T1, T2, T3, T4> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06004A8F RID: 19087 RVA: 0x001861D7 File Offset: 0x001843D7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06004A90 RID: 19088 RVA: 0x001861E0 File Offset: 0x001843E0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06004A91 RID: 19089 RVA: 0x00186677 File Offset: 0x00184877
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06004A92 RID: 19090 RVA: 0x00186686 File Offset: 0x00184886
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4);
	}

	// Token: 0x06004A93 RID: 19091 RVA: 0x0018669C File Offset: 0x0018489C
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004A94 RID: 19092 RVA: 0x00186758 File Offset: 0x00184958
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		T2 arg2;
		T3 arg3;
		T4 arg4;
		if (!args.TryParseArgs(2, out arg, out arg2, out arg3, out arg4))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4>(this._callbacks, arg, arg2, arg3, arg4, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4>(this._callbacks, arg, arg2, arg3, arg4, info);
	}

	// Token: 0x06004A95 RID: 19093 RVA: 0x001867A0 File Offset: 0x001849A0
	public new static implicit operator PhotonSignal<T1, T2, T3, T4>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4>(s);
	}

	// Token: 0x06004A96 RID: 19094 RVA: 0x001867A8 File Offset: 0x001849A8
	public new static explicit operator PhotonSignal<T1, T2, T3, T4>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4>(i);
	}

	// Token: 0x04005AE4 RID: 23268
	private OnSignalReceived<T1, T2, T3, T4> _callbacks;

	// Token: 0x04005AE5 RID: 23269
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4>).FullName.GetStaticHash();
}
