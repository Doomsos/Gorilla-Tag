using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000BBF RID: 3007
[Serializable]
public class PhotonSignal
{
	// Token: 0x06004A35 RID: 18997 RVA: 0x001857AC File Offset: 0x001839AC
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, info);
		}
	}

	// Token: 0x06004A36 RID: 18998 RVA: 0x001857DC File Offset: 0x001839DC
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, info);
		}
	}

	// Token: 0x06004A37 RID: 18999 RVA: 0x00185808 File Offset: 0x00183A08
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, info);
		}
	}

	// Token: 0x06004A38 RID: 19000 RVA: 0x00185834 File Offset: 0x00183A34
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, info);
		}
	}

	// Token: 0x06004A39 RID: 19001 RVA: 0x0018585C File Offset: 0x00183A5C
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, info);
		}
	}

	// Token: 0x06004A3A RID: 19002 RVA: 0x00185884 File Offset: 0x00183A84
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6, T7>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, arg7, info);
		}
	}

	// Token: 0x06004A3B RID: 19003 RVA: 0x001858A7 File Offset: 0x00183AA7
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5, T6>(OnSignalReceived<T1, T2, T3, T4, T5, T6> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, arg6, info);
		}
	}

	// Token: 0x06004A3C RID: 19004 RVA: 0x001858BD File Offset: 0x00183ABD
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4, T5>(OnSignalReceived<T1, T2, T3, T4, T5> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, arg5, info);
		}
	}

	// Token: 0x06004A3D RID: 19005 RVA: 0x001858D1 File Offset: 0x00183AD1
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3, T4>(OnSignalReceived<T1, T2, T3, T4> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, arg4, info);
		}
	}

	// Token: 0x06004A3E RID: 19006 RVA: 0x001858E3 File Offset: 0x00183AE3
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2, T3>(OnSignalReceived<T1, T2, T3> _event, T1 arg1, T2 arg2, T3 arg3, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, arg3, info);
		}
	}

	// Token: 0x06004A3F RID: 19007 RVA: 0x001858F3 File Offset: 0x00183AF3
	[MethodImpl(256)]
	protected static void _Invoke<T1, T2>(OnSignalReceived<T1, T2> _event, T1 arg1, T2 arg2, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, arg2, info);
		}
	}

	// Token: 0x06004A40 RID: 19008 RVA: 0x00185901 File Offset: 0x00183B01
	[MethodImpl(256)]
	protected static void _Invoke<T1>(OnSignalReceived<T1> _event, T1 arg1, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(arg1, info);
		}
	}

	// Token: 0x06004A41 RID: 19009 RVA: 0x0018590E File Offset: 0x00183B0E
	[MethodImpl(256)]
	protected static void _Invoke(OnSignalReceived _event, PhotonSignalInfo info)
	{
		if (_event != null)
		{
			_event(info);
		}
	}

	// Token: 0x06004A42 RID: 19010 RVA: 0x0018591C File Offset: 0x00183B1C
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A43 RID: 19011 RVA: 0x0018597C File Offset: 0x00183B7C
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A44 RID: 19012 RVA: 0x001859DC File Offset: 0x00183BDC
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A45 RID: 19013 RVA: 0x00185A38 File Offset: 0x00183C38
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8, T9> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A46 RID: 19014 RVA: 0x00185A94 File Offset: 0x00183C94
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7, T8> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A47 RID: 19015 RVA: 0x00185AEC File Offset: 0x00183CEC
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6, T7>(OnSignalReceived<T1, T2, T3, T4, T5, T6, T7> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6, T7>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6, T7>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6, T7> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, arg7, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A48 RID: 19016 RVA: 0x00185B44 File Offset: 0x00183D44
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5, T6>(OnSignalReceived<T1, T2, T3, T4, T5, T6> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5, T6>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5, T6>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5, T6> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, arg6, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A49 RID: 19017 RVA: 0x00185B98 File Offset: 0x00183D98
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4, T5>(OnSignalReceived<T1, T2, T3, T4, T5> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4, T5>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4, T5>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4, T5> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, arg5, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A4A RID: 19018 RVA: 0x00185BEC File Offset: 0x00183DEC
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3, T4>(OnSignalReceived<T1, T2, T3, T4> _event, T1 arg1, T2 arg2, T3 arg3, T4 arg4, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3, T4>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3, T4>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3, T4> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, arg4, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A4B RID: 19019 RVA: 0x00185C3C File Offset: 0x00183E3C
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2, T3>(OnSignalReceived<T1, T2, T3> _event, T1 arg1, T2 arg2, T3 arg3, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2, T3>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2, T3>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2, T3> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, arg3, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A4C RID: 19020 RVA: 0x00185C8C File Offset: 0x00183E8C
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1, T2>(OnSignalReceived<T1, T2> _event, T1 arg1, T2 arg2, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1, T2>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1, T2>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1, T2> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, arg2, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A4D RID: 19021 RVA: 0x00185CD8 File Offset: 0x00183ED8
	[MethodImpl(256)]
	protected static void _SafeInvoke<T1>(OnSignalReceived<T1> _event, T1 arg1, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived<T1>[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived<T1>>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived<T1> onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(arg1, info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x06004A4E RID: 19022 RVA: 0x00185D24 File Offset: 0x00183F24
	[MethodImpl(256)]
	protected static void _SafeInvoke(OnSignalReceived _event, PhotonSignalInfo info)
	{
		ref readonly OnSignalReceived[] ptr = ref PhotonUtils.FetchDelegatesNonAlloc<OnSignalReceived>(_event);
		for (int i = 0; i < ptr.Length; i++)
		{
			try
			{
				OnSignalReceived onSignalReceived = ptr[i];
				if (onSignalReceived != null)
				{
					onSignalReceived(info);
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x170006E3 RID: 1763
	// (get) Token: 0x06004A4F RID: 19023 RVA: 0x00185D70 File Offset: 0x00183F70
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
	}

	// Token: 0x170006E4 RID: 1764
	// (get) Token: 0x06004A50 RID: 19024 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int argCount
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x1400007F RID: 127
	// (add) Token: 0x06004A51 RID: 19025 RVA: 0x00185D78 File Offset: 0x00183F78
	// (remove) Token: 0x06004A52 RID: 19026 RVA: 0x00185DAC File Offset: 0x00183FAC
	public event OnSignalReceived OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06004A53 RID: 19027 RVA: 0x00185DC9 File Offset: 0x00183FC9
	protected PhotonSignal()
	{
		this._refID = PhotonSignal.RefID.Register(this);
	}

	// Token: 0x06004A54 RID: 19028 RVA: 0x00185DEB File Offset: 0x00183FEB
	public PhotonSignal(string signalID) : this()
	{
		signalID = ((signalID != null) ? signalID.Trim() : null);
		if (string.IsNullOrWhiteSpace(signalID))
		{
			throw new ArgumentNullException("signalID");
		}
		this._signalID = XXHash32.Compute(signalID, 0U);
	}

	// Token: 0x06004A55 RID: 19029 RVA: 0x00185E21 File Offset: 0x00184021
	public PhotonSignal(int signalID) : this()
	{
		this._signalID = signalID;
	}

	// Token: 0x06004A56 RID: 19030 RVA: 0x00185E30 File Offset: 0x00184030
	public void Raise()
	{
		this.Raise(this._receivers);
	}

	// Token: 0x06004A57 RID: 19031 RVA: 0x00185E40 File Offset: 0x00184040
	public void Raise(ReceiverGroup receivers)
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
		object[] array = PhotonUtils.FetchScratchArray(2);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo info = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, info);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06004A58 RID: 19032 RVA: 0x00185ECD File Offset: 0x001840CD
	public void Enable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += new Action<EventData>(this._EventHandle);
		this._enabled = true;
	}

	// Token: 0x06004A59 RID: 19033 RVA: 0x00185EEC File Offset: 0x001840EC
	public void Disable()
	{
		this._enabled = false;
		PhotonNetwork.NetworkingClient.EventReceived -= new Action<EventData>(this._EventHandle);
	}

	// Token: 0x06004A5A RID: 19034 RVA: 0x00185F0C File Offset: 0x0018410C
	private void _EventHandle(EventData eventData)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		if (eventData.Code != 177)
		{
			return;
		}
		int sender = eventData.Sender;
		object[] array = eventData.CustomData as object[];
		if (array == null)
		{
			return;
		}
		if (array.Length < 2 + this.argCount)
		{
			return;
		}
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num == 0 || num != this._signalID)
		{
			return;
		}
		obj = array[1];
		if (obj is int)
		{
			int timestamp = (int)obj;
			NetPlayer netPlayer = PhotonUtils.GetNetPlayer(sender);
			PhotonSignalInfo info = new PhotonSignalInfo(netPlayer, timestamp);
			this._Relay(array, info);
			return;
		}
	}

	// Token: 0x06004A5B RID: 19035 RVA: 0x00185FB8 File Offset: 0x001841B8
	protected virtual void _Relay(object[] args, PhotonSignalInfo info)
	{
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke(this._callbacks, info);
			return;
		}
		PhotonSignal._SafeInvoke(this._callbacks, info);
	}

	// Token: 0x06004A5C RID: 19036 RVA: 0x00185FDB File Offset: 0x001841DB
	public virtual void ClearListeners()
	{
		this._callbacks = null;
	}

	// Token: 0x06004A5D RID: 19037 RVA: 0x00185FE4 File Offset: 0x001841E4
	public virtual void Reset()
	{
		this.ClearListeners();
		this.Disable();
	}

	// Token: 0x06004A5E RID: 19038 RVA: 0x00185FF2 File Offset: 0x001841F2
	public virtual void Dispose()
	{
		this._signalID = 0;
		this.Reset();
	}

	// Token: 0x06004A5F RID: 19039 RVA: 0x00186004 File Offset: 0x00184204
	~PhotonSignal()
	{
		this.Dispose();
	}

	// Token: 0x06004A60 RID: 19040 RVA: 0x00186030 File Offset: 0x00184230
	public static implicit operator PhotonSignal(string s)
	{
		return new PhotonSignal(s);
	}

	// Token: 0x06004A61 RID: 19041 RVA: 0x00186038 File Offset: 0x00184238
	public static explicit operator PhotonSignal(int i)
	{
		return new PhotonSignal(i);
	}

	// Token: 0x06004A62 RID: 19042 RVA: 0x00186040 File Offset: 0x00184240
	static PhotonSignal()
	{
		Dictionary<ReceiverGroup, RaiseEventOptions> dictionary = new Dictionary<ReceiverGroup, RaiseEventOptions>();
		dictionary[0] = new RaiseEventOptions
		{
			Receivers = 0
		};
		dictionary[1] = new RaiseEventOptions
		{
			Receivers = 1
		};
		dictionary[2] = new RaiseEventOptions
		{
			Receivers = 2
		};
		PhotonSignal.gGroupToOptions = dictionary;
		PhotonSignal.gSendReliable = SendOptions.SendReliable;
		PhotonSignal.gSendUnreliable = SendOptions.SendUnreliable;
		PhotonSignal.gSendReliable.Encrypt = true;
		PhotonSignal.gSendUnreliable.Encrypt = true;
	}

	// Token: 0x04005ACC RID: 23244
	protected int _signalID;

	// Token: 0x04005ACD RID: 23245
	protected bool _enabled;

	// Token: 0x04005ACE RID: 23246
	[SerializeField]
	protected ReceiverGroup _receivers = 1;

	// Token: 0x04005ACF RID: 23247
	[FormerlySerializedAs("mute")]
	[SerializeField]
	protected bool _mute;

	// Token: 0x04005AD0 RID: 23248
	[SerializeField]
	protected bool _safeInvoke = true;

	// Token: 0x04005AD1 RID: 23249
	[SerializeField]
	protected bool _localOnly;

	// Token: 0x04005AD2 RID: 23250
	[NonSerialized]
	private int _refID;

	// Token: 0x04005AD3 RID: 23251
	private OnSignalReceived _callbacks;

	// Token: 0x04005AD4 RID: 23252
	protected static readonly Dictionary<ReceiverGroup, RaiseEventOptions> gGroupToOptions;

	// Token: 0x04005AD5 RID: 23253
	protected static readonly SendOptions gSendReliable;

	// Token: 0x04005AD6 RID: 23254
	protected static readonly SendOptions gSendUnreliable;

	// Token: 0x04005AD7 RID: 23255
	public const byte EVENT_CODE = 177;

	// Token: 0x04005AD8 RID: 23256
	public const int NULL_SIGNAL = 0;

	// Token: 0x04005AD9 RID: 23257
	protected const int HEADER_SIZE = 2;

	// Token: 0x02000BC0 RID: 3008
	private class RefID
	{
		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06004A63 RID: 19043 RVA: 0x001860BA File Offset: 0x001842BA
		public static int Count
		{
			get
			{
				return PhotonSignal.RefID.gRefCount;
			}
		}

		// Token: 0x06004A64 RID: 19044 RVA: 0x001860C1 File Offset: 0x001842C1
		public RefID()
		{
			this.intValue = StaticHash.ComputeTriple32(PhotonSignal.RefID.gNextID++);
			PhotonSignal.RefID.gRefCount++;
		}

		// Token: 0x06004A65 RID: 19045 RVA: 0x001860F0 File Offset: 0x001842F0
		~RefID()
		{
			PhotonSignal.RefID.gRefCount--;
		}

		// Token: 0x06004A66 RID: 19046 RVA: 0x00186124 File Offset: 0x00184324
		public static int Register(PhotonSignal ps)
		{
			if (ps == null)
			{
				return 0;
			}
			PhotonSignal.RefID refID = new PhotonSignal.RefID();
			PhotonSignal.RefID.gRefTable.Add(ps, refID);
			return refID.intValue;
		}

		// Token: 0x04005ADA RID: 23258
		public int intValue;

		// Token: 0x04005ADB RID: 23259
		private static int gNextID = 1;

		// Token: 0x04005ADC RID: 23260
		private static int gRefCount = 0;

		// Token: 0x04005ADD RID: 23261
		private static readonly ConditionalWeakTable<PhotonSignal, PhotonSignal.RefID> gRefTable = new ConditionalWeakTable<PhotonSignal, PhotonSignal.RefID>();
	}
}
