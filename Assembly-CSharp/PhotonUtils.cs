using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x02000BC6 RID: 3014
public static class PhotonUtils
{
	// Token: 0x06004AA4 RID: 19108 RVA: 0x00186984 File Offset: 0x00184B84
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11, out T12 arg12)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
		arg11 = (T11)((object)args[startIndex + 10]);
		arg12 = (T12)((object)args[startIndex + 11]);
	}

	// Token: 0x06004AA5 RID: 19109 RVA: 0x00186A5C File Offset: 0x00184C5C
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
		arg11 = (T11)((object)args[startIndex + 10]);
	}

	// Token: 0x06004AA6 RID: 19110 RVA: 0x00186B24 File Offset: 0x00184D24
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
		arg10 = (T10)((object)args[startIndex + 9]);
	}

	// Token: 0x06004AA7 RID: 19111 RVA: 0x00186BD8 File Offset: 0x00184DD8
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
		arg9 = (T9)((object)args[startIndex + 8]);
	}

	// Token: 0x06004AA8 RID: 19112 RVA: 0x00186C7C File Offset: 0x00184E7C
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
		arg8 = (T8)((object)args[startIndex + 7]);
	}

	// Token: 0x06004AA9 RID: 19113 RVA: 0x00186D10 File Offset: 0x00184F10
	public static void ParseArgs<T1, T2, T3, T4, T5, T6, T7>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
		arg7 = (T7)((object)args[startIndex + 6]);
	}

	// Token: 0x06004AAA RID: 19114 RVA: 0x00186D90 File Offset: 0x00184F90
	public static void ParseArgs<T1, T2, T3, T4, T5, T6>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
		arg6 = (T6)((object)args[startIndex + 5]);
	}

	// Token: 0x06004AAB RID: 19115 RVA: 0x00186E00 File Offset: 0x00185000
	public static void ParseArgs<T1, T2, T3, T4, T5>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
		arg5 = (T5)((object)args[startIndex + 4]);
	}

	// Token: 0x06004AAC RID: 19116 RVA: 0x00186E60 File Offset: 0x00185060
	public static void ParseArgs<T1, T2, T3, T4>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
		arg4 = (T4)((object)args[startIndex + 3]);
	}

	// Token: 0x06004AAD RID: 19117 RVA: 0x00186EAD File Offset: 0x001850AD
	public static void ParseArgs<T1, T2, T3>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
		arg3 = (T3)((object)args[startIndex + 2]);
	}

	// Token: 0x06004AAE RID: 19118 RVA: 0x00186EDE File Offset: 0x001850DE
	public static void ParseArgs<T1, T2>(this object[] args, int startIndex, out T1 arg1, out T2 arg2)
	{
		arg1 = (T1)((object)args[startIndex]);
		arg2 = (T2)((object)args[startIndex + 1]);
	}

	// Token: 0x06004AAF RID: 19119 RVA: 0x00186EFE File Offset: 0x001850FE
	public static void ParseArgs<T1>(this object[] args, int startIndex, out T1 arg1)
	{
		arg1 = (T1)((object)args[startIndex]);
	}

	// Token: 0x06004AB0 RID: 19120 RVA: 0x00186F10 File Offset: 0x00185110
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11, out T12 arg12)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		arg11 = default(T11);
		arg12 = default(T12);
		if (args == null || args.Length < startIndex + 12)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10, out arg11, out arg12);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB1 RID: 19121 RVA: 0x00186FC4 File Offset: 0x001851C4
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10, out T11 arg11)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		arg11 = default(T11);
		if (args == null || args.Length < startIndex + 11)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10, out arg11);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB2 RID: 19122 RVA: 0x0018706C File Offset: 0x0018526C
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9, out T10 arg10)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		arg10 = default(T10);
		if (args == null || args.Length < startIndex + 10)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9, out arg10);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB3 RID: 19123 RVA: 0x0018710C File Offset: 0x0018530C
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8, out T9 arg9)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		arg9 = default(T9);
		if (args == null || args.Length < startIndex + 9)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8, out arg9);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB4 RID: 19124 RVA: 0x001871A0 File Offset: 0x001853A0
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7, out T8 arg8)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		arg8 = default(T8);
		if (args == null || args.Length < startIndex + 8)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7, out arg8);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB5 RID: 19125 RVA: 0x00187228 File Offset: 0x00185428
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6, out T7 arg7)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		arg7 = default(T7);
		if (args == null || args.Length < startIndex + 7)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6, out arg7);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB6 RID: 19126 RVA: 0x001872A8 File Offset: 0x001854A8
	public static bool TryParseArgs<T1, T2, T3, T4, T5, T6>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		arg6 = default(T6);
		if (args == null || args.Length < startIndex + 6)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5, out arg6);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB7 RID: 19127 RVA: 0x0018731C File Offset: 0x0018551C
	public static bool TryParseArgs<T1, T2, T3, T4, T5>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		arg5 = default(T5);
		if (args == null || args.Length < startIndex + 5)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4, out arg5);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB8 RID: 19128 RVA: 0x00187388 File Offset: 0x00185588
	public static bool TryParseArgs<T1, T2, T3, T4>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		arg4 = default(T4);
		if (args == null || args.Length < startIndex + 4)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3, out arg4);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004AB9 RID: 19129 RVA: 0x001873E8 File Offset: 0x001855E8
	public static bool TryParseArgs<T1, T2, T3>(this object[] args, int startIndex, out T1 arg1, out T2 arg2, out T3 arg3)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		arg3 = default(T3);
		if (args == null || args.Length < startIndex + 3)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2, out arg3);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004ABA RID: 19130 RVA: 0x00187440 File Offset: 0x00185640
	public static bool TryParseArgs<T1, T2>(this object[] args, int startIndex, out T1 arg1, out T2 arg2)
	{
		arg1 = default(T1);
		arg2 = default(T2);
		if (args == null || args.Length < startIndex + 2)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1, out arg2);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004ABB RID: 19131 RVA: 0x0018748C File Offset: 0x0018568C
	public static bool TryParseArgs<T1>(this object[] args, int startIndex, out T1 arg1)
	{
		arg1 = default(T1);
		if (args == null || args.Length < startIndex + 1)
		{
			return false;
		}
		try
		{
			args.ParseArgs(startIndex, out arg1);
		}
		catch
		{
			return false;
		}
		return true;
	}

	// Token: 0x06004ABC RID: 19132 RVA: 0x001874D0 File Offset: 0x001856D0
	public static ref readonly T[] FetchDelegatesNonAlloc<T>(T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return PhotonUtils.EmptyArray<T>.Ref();
		}
		return @delegate.GetInvocationListUnsafe<T>();
	}

	// Token: 0x06004ABD RID: 19133 RVA: 0x001874EC File Offset: 0x001856EC
	public static object[] FetchScratchArray(int size)
	{
		if (size < 0)
		{
			throw new Exception("Size cannot be less than 0.");
		}
		object[] array;
		if (!PhotonUtils.gLengthToArgsArray.TryGetValue(size, ref array))
		{
			array = new object[size];
			PhotonUtils.gLengthToArgsArray.Add(size, array);
		}
		return array;
	}

	// Token: 0x06004ABE RID: 19134 RVA: 0x0018752C File Offset: 0x0018572C
	public static NetPlayer GetNetPlayer(int actorNumber)
	{
		NetworkSystem networkSystem;
		if (!PhotonUtils.TryGetNetSystem(out networkSystem))
		{
			return null;
		}
		return networkSystem.GetPlayer(actorNumber);
	}

	// Token: 0x170006EB RID: 1771
	// (get) Token: 0x06004ABF RID: 19135 RVA: 0x0018754C File Offset: 0x0018574C
	public static int LocalActorNumber
	{
		get
		{
			NetPlayer localNetPlayer = PhotonUtils.LocalNetPlayer;
			if (localNetPlayer == null)
			{
				return -1;
			}
			return localNetPlayer.ActorNumber;
		}
	}

	// Token: 0x170006EC RID: 1772
	// (get) Token: 0x06004AC0 RID: 19136 RVA: 0x0018756C File Offset: 0x0018576C
	public static NetPlayer LocalNetPlayer
	{
		get
		{
			if (PhotonUtils.gLocalNetPlayer != null)
			{
				return PhotonUtils.gLocalNetPlayer;
			}
			NetworkSystem networkSystem;
			if (PhotonUtils.TryGetNetSystem(out networkSystem))
			{
				PhotonUtils.gLocalNetPlayer = networkSystem.GetLocalPlayer();
			}
			return PhotonUtils.gLocalNetPlayer;
		}
	}

	// Token: 0x06004AC1 RID: 19137 RVA: 0x0018759F File Offset: 0x0018579F
	private static bool TryGetNetSystem(out NetworkSystem ns)
	{
		if (!PhotonUtils.gNetSystem)
		{
			PhotonUtils.gNetSystem = NetworkSystem.Instance;
		}
		if (!PhotonUtils.gNetSystem)
		{
			ns = null;
			return false;
		}
		ns = PhotonUtils.gNetSystem;
		return true;
	}

	// Token: 0x06004AC2 RID: 19138 RVA: 0x001875D0 File Offset: 0x001857D0
	static PhotonUtils()
	{
		for (int i = 0; i <= 16; i++)
		{
			PhotonUtils.gLengthToArgsArray.Add(i, new object[i]);
		}
	}

	// Token: 0x04005AE8 RID: 23272
	private static NetworkSystem gNetSystem;

	// Token: 0x04005AE9 RID: 23273
	private static NetPlayer gLocalNetPlayer;

	// Token: 0x04005AEA RID: 23274
	private static readonly Dictionary<int, object[]> gLengthToArgsArray = new Dictionary<int, object[]>(16);

	// Token: 0x04005AEB RID: 23275
	private const int ARG_ARRAYS = 16;

	// Token: 0x02000BC7 RID: 3015
	private static class EmptyArray<T>
	{
		// Token: 0x06004AC3 RID: 19139 RVA: 0x00187607 File Offset: 0x00185807
		public static ref readonly T[] Ref()
		{
			return ref PhotonUtils.EmptyArray<T>.gEmpty;
		}

		// Token: 0x04005AEC RID: 23276
		private static readonly T[] gEmpty = Array.Empty<T>();
	}

	// Token: 0x02000BC8 RID: 3016
	public static class CustomTypes
	{
		// Token: 0x06004AC5 RID: 19141 RVA: 0x0018761A File Offset: 0x0018581A
		[RuntimeInitializeOnLoadMethod(1)]
		private static void InitOnLoad()
		{
			PhotonPeer.RegisterType(typeof(Color32), 67, new SerializeMethod(PhotonUtils.CustomTypes.SerializeColor32), new DeserializeMethod(PhotonUtils.CustomTypes.DeserializeColor32));
		}

		// Token: 0x06004AC6 RID: 19142 RVA: 0x00187646 File Offset: 0x00185846
		public static byte[] SerializeColor32(object value)
		{
			return PhotonUtils.CustomTypes.CastToBytes<Color32>((Color32)value);
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x00187653 File Offset: 0x00185853
		public static object DeserializeColor32(byte[] data)
		{
			return PhotonUtils.CustomTypes.CastToStruct<Color32>(data);
		}

		// Token: 0x06004AC8 RID: 19144 RVA: 0x00187660 File Offset: 0x00185860
		private static T CastToStruct<T>(byte[] bytes) where T : struct
		{
			GCHandle gchandle = GCHandle.Alloc(bytes, 3);
			T result = Marshal.PtrToStructure<T>(gchandle.AddrOfPinnedObject());
			gchandle.Free();
			return result;
		}

		// Token: 0x06004AC9 RID: 19145 RVA: 0x00187688 File Offset: 0x00185888
		private static byte[] CastToBytes<T>(T data) where T : struct
		{
			byte[] array = new byte[Marshal.SizeOf<T>()];
			GCHandle gchandle = GCHandle.Alloc(array, 3);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			Marshal.StructureToPtr<T>(data, intPtr, true);
			gchandle.Free();
			return array;
		}

		// Token: 0x04005AED RID: 23277
		private const short LEN_C32 = 4;
	}
}
