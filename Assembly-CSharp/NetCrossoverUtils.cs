using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public static class NetCrossoverUtils
{
	// Token: 0x0600171B RID: 5915 RVA: 0x00080491 File Offset: 0x0007E691
	public static void Prewarm()
	{
		NetCrossoverUtils.FixedBuffer = new byte[2048];
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x000804A4 File Offset: 0x0007E6A4
	public static void WriteNetDataToBuffer<T>(this T data, PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = default(IntPtr);
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x0008053C File Offset: 0x0007E73C
	public static object ReadNetDataFromBuffer<T>(PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsWriting)
		{
			Debug.LogError("Attmpted to read data from a writing stream!");
			return null;
		}
		IntPtr intPtr = default(IntPtr);
		object result;
		try
		{
			Type typeFromHandle = typeof(T);
			int num = (int)stream.ReceiveNext();
			int num2 = Marshal.SizeOf(typeFromHandle);
			if (num != num2)
			{
				Debug.LogError(string.Format("Expected datasize {0} when reading data for type '{1}',", num2, typeFromHandle.Name) + string.Format("but {0} data is available!", num));
				result = null;
			}
			else
			{
				intPtr = Marshal.AllocHGlobal(num2);
				for (int i = 0; i < num2; i++)
				{
					NetCrossoverUtils.FixedBuffer[i] = (byte)stream.ReceiveNext();
				}
				Marshal.Copy(NetCrossoverUtils.FixedBuffer, 0, intPtr, num2);
				result = (T)((object)Marshal.PtrToStructure(intPtr, typeFromHandle));
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x00080624 File Offset: 0x0007E824
	public static void WriteNetDataToBuffer(this object data, PhotonStream stream)
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = default(IntPtr);
		try
		{
			int num = Marshal.SizeOf(data.GetType());
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x000806B8 File Offset: 0x0007E8B8
	public static void SerializeToRPCData<T>(this RPCArgBuffer<T> argBuffer) where T : struct
	{
		IntPtr intPtr = default(IntPtr);
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(argBuffer.Args, intPtr, true);
			Marshal.Copy(intPtr, argBuffer.Data, 0, num);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x00080718 File Offset: 0x0007E918
	public static void PopulateWithRPCData<T>(this RPCArgBuffer<T> argBuffer, byte[] data) where T : struct
	{
		IntPtr intPtr = default(IntPtr);
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(data, 0, intPtr, num);
			argBuffer.Args = Marshal.PtrToStructure<T>(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x00080774 File Offset: 0x0007E974
	public static Dictionary<string, SessionProperty> ToPropDict(this Hashtable hash)
	{
		Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
		foreach (DictionaryEntry dictionaryEntry in hash)
		{
			dictionary.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
		}
		return dictionary;
	}

	// Token: 0x04002113 RID: 8467
	private const int MaxParameterByteLength = 2048;

	// Token: 0x04002114 RID: 8468
	private static byte[] FixedBuffer;
}
