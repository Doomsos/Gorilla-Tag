using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020007F1 RID: 2033
public static class GTSignal
{
	// Token: 0x06003567 RID: 13671 RVA: 0x00121F78 File Offset: 0x00120178
	private static void _Emit(GTSignal.EmitMode mode, int signalID, object[] data)
	{
		object[] array = GTSignal._ToEventContent(signalID, PhotonNetwork.Time, data);
		PhotonNetwork.RaiseEvent(186, array, GTSignal.gTargetsToOptions[mode], GTSignal.gSendOptions);
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x00121FB0 File Offset: 0x001201B0
	private static void _Emit(int[] targetActors, int signalID, object[] data)
	{
		if (targetActors.IsNullOrEmpty<int>())
		{
			return;
		}
		GTSignal.gCustomTargetOptions.TargetActors = targetActors;
		object[] array = GTSignal._ToEventContent(signalID, PhotonNetwork.Time, data);
		PhotonNetwork.RaiseEvent(186, array, GTSignal.gCustomTargetOptions, GTSignal.gSendOptions);
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x00121FF4 File Offset: 0x001201F4
	private static object[] _ToEventContent(int signalID, double time, object[] data)
	{
		int num = data.Length;
		int num2 = num + 2;
		object[] array;
		if (!GTSignal.gLengthToContentArray.TryGetValue(num2, ref array))
		{
			array = new object[num2];
			GTSignal.gLengthToContentArray.Add(num2, array);
		}
		array[0] = signalID;
		array[1] = time;
		for (int i = 0; i < num; i++)
		{
			array[i + 2] = data[i];
		}
		return array;
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x00122052 File Offset: 0x00120252
	public static int ComputeID(string s)
	{
		if (!string.IsNullOrWhiteSpace(s))
		{
			return XXHash32.Compute(s.Trim(), 0U);
		}
		return 0;
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x0012206C File Offset: 0x0012026C
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitializeOnLoad()
	{
		GTSignal.gCustomTargetOptions = RaiseEventOptions.Default;
		GTSignal.gSendOptions = SendOptions.SendReliable;
		GTSignal.gSendOptions.Encrypt = true;
		GTSignal.gTargetsToOptions = new Dictionary<GTSignal.EmitMode, RaiseEventOptions>(3);
		RaiseEventOptions @default = RaiseEventOptions.Default;
		@default.Receivers = 1;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.All, @default);
		RaiseEventOptions default2 = RaiseEventOptions.Default;
		default2.Receivers = 0;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.Others, default2);
		RaiseEventOptions default3 = RaiseEventOptions.Default;
		default3.Receivers = 2;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.Host, default3);
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x001220EE File Offset: 0x001202EE
	public static void Emit(string signal, params object[] data)
	{
		GTSignal._Emit(GTSignal.EmitMode.All, GTSignal.ComputeID(signal), data);
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x001220FD File Offset: 0x001202FD
	public static void Emit(GTSignal.EmitMode mode, string signal, params object[] data)
	{
		GTSignal._Emit(mode, GTSignal.ComputeID(signal), data);
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x0012210C File Offset: 0x0012030C
	public static void Emit(int signalID, params object[] data)
	{
		GTSignal._Emit(GTSignal.EmitMode.All, signalID, data);
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x00122116 File Offset: 0x00120316
	public static void Emit(GTSignal.EmitMode mode, int signalID, params object[] data)
	{
		GTSignal._Emit(mode, signalID, data);
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x00122120 File Offset: 0x00120320
	public static void Emit(int target, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[1];
		array[0] = target;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x00122138 File Offset: 0x00120338
	public static void Emit(int target1, int target2, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[2];
		array[0] = target1;
		array[1] = target2;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x00122154 File Offset: 0x00120354
	public static void Emit(int target1, int target2, int target3, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[3];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003573 RID: 13683 RVA: 0x00122175 File Offset: 0x00120375
	public static void Emit(int target1, int target2, int target3, int target4, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[4];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x0012219B File Offset: 0x0012039B
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[5];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003575 RID: 13685 RVA: 0x001221C6 File Offset: 0x001203C6
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[6];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003576 RID: 13686 RVA: 0x001221F6 File Offset: 0x001203F6
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[7];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003577 RID: 13687 RVA: 0x0012222B File Offset: 0x0012042B
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[8];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x00122265 File Offset: 0x00120465
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int target9, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[9];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		array[8] = target9;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x001222A8 File Offset: 0x001204A8
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int target9, int target10, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[10];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		array[8] = target9;
		array[9] = target10;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x001222FC File Offset: 0x001204FC
	// Note: this type is marked as 'beforefieldinit'.
	static GTSignal()
	{
		Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
		dictionary[1] = new object[1];
		dictionary[2] = new object[2];
		dictionary[3] = new object[3];
		dictionary[4] = new object[4];
		dictionary[5] = new object[5];
		dictionary[6] = new object[6];
		dictionary[7] = new object[7];
		dictionary[8] = new object[8];
		dictionary[9] = new object[9];
		dictionary[10] = new object[10];
		dictionary[11] = new object[11];
		dictionary[12] = new object[12];
		dictionary[13] = new object[13];
		dictionary[14] = new object[14];
		dictionary[15] = new object[15];
		dictionary[16] = new object[16];
		GTSignal.gLengthToContentArray = dictionary;
		Dictionary<int, int[]> dictionary2 = new Dictionary<int, int[]>();
		dictionary2[1] = new int[1];
		dictionary2[2] = new int[2];
		dictionary2[3] = new int[3];
		dictionary2[4] = new int[4];
		dictionary2[5] = new int[5];
		dictionary2[6] = new int[6];
		dictionary2[7] = new int[7];
		dictionary2[8] = new int[8];
		dictionary2[9] = new int[9];
		dictionary2[10] = new int[10];
		GTSignal.gLengthToTargetsArray = dictionary2;
	}

	// Token: 0x040044A0 RID: 17568
	public const byte PHOTON_CODE = 186;

	// Token: 0x040044A1 RID: 17569
	private static Dictionary<GTSignal.EmitMode, RaiseEventOptions> gTargetsToOptions;

	// Token: 0x040044A2 RID: 17570
	private static Dictionary<int, object[]> gLengthToContentArray;

	// Token: 0x040044A3 RID: 17571
	private static Dictionary<int, int[]> gLengthToTargetsArray;

	// Token: 0x040044A4 RID: 17572
	private static SendOptions gSendOptions;

	// Token: 0x040044A5 RID: 17573
	private static RaiseEventOptions gCustomTargetOptions;

	// Token: 0x020007F2 RID: 2034
	public enum EmitMode
	{
		// Token: 0x040044A7 RID: 17575
		None = -1,
		// Token: 0x040044A8 RID: 17576
		Others,
		// Token: 0x040044A9 RID: 17577
		Targets,
		// Token: 0x040044AA RID: 17578
		All,
		// Token: 0x040044AB RID: 17579
		Host
	}
}
