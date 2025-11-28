using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000BC1 RID: 3009
[Serializable]
public class PhotonSignal<T1> : PhotonSignal
{
	// Token: 0x170006E6 RID: 1766
	// (get) Token: 0x06004A68 RID: 19048 RVA: 0x00027DED File Offset: 0x00025FED
	public override int argCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x14000080 RID: 128
	// (add) Token: 0x06004A69 RID: 19049 RVA: 0x00186166 File Offset: 0x00184366
	// (remove) Token: 0x06004A6A RID: 19050 RVA: 0x0018619A File Offset: 0x0018439A
	public new event OnSignalReceived<T1> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06004A6B RID: 19051 RVA: 0x001861B7 File Offset: 0x001843B7
	public PhotonSignal(string signalID) : base(signalID)
	{
	}

	// Token: 0x06004A6C RID: 19052 RVA: 0x001861C0 File Offset: 0x001843C0
	public PhotonSignal(int signalID) : base(signalID)
	{
	}

	// Token: 0x06004A6D RID: 19053 RVA: 0x001861C9 File Offset: 0x001843C9
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06004A6E RID: 19054 RVA: 0x001861D8 File Offset: 0x001843D8
	public void Raise(T1 arg1)
	{
		this.Raise(this._receivers, arg1);
	}

	// Token: 0x06004A6F RID: 19055 RVA: 0x001861E8 File Offset: 0x001843E8
	public void Raise(ReceiverGroup receivers, T1 arg1)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004A70 RID: 19056 RVA: 0x00186288 File Offset: 0x00184488
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 arg;
		if (!args.TryParseArgs(2, out arg))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1>(this._callbacks, arg, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1>(this._callbacks, arg, info);
	}

	// Token: 0x06004A71 RID: 19057 RVA: 0x001862C4 File Offset: 0x001844C4
	public new static implicit operator PhotonSignal<T1>(string s)
	{
		return new PhotonSignal<T1>(s);
	}

	// Token: 0x06004A72 RID: 19058 RVA: 0x001862CC File Offset: 0x001844CC
	public new static explicit operator PhotonSignal<T1>(int i)
	{
		return new PhotonSignal<T1>(i);
	}

	// Token: 0x04005ADE RID: 23262
	private OnSignalReceived<T1> _callbacks;

	// Token: 0x04005ADF RID: 23263
	private static readonly int kSignature = typeof(PhotonSignal<T1>).FullName.GetStaticHash();
}
