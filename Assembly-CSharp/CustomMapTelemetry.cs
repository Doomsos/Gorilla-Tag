using System;
using System.Collections;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000954 RID: 2388
public class CustomMapTelemetry : MonoBehaviour
{
	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x06003D12 RID: 15634 RVA: 0x001445B3 File Offset: 0x001427B3
	public static bool IsActive
	{
		get
		{
			return CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted;
		}
	}

	// Token: 0x06003D13 RID: 15635 RVA: 0x001445C3 File Offset: 0x001427C3
	private void Awake()
	{
		if (CustomMapTelemetry.instance == null)
		{
			CustomMapTelemetry.instance = this;
			return;
		}
		if (CustomMapTelemetry.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003D14 RID: 15636 RVA: 0x001445F7 File Offset: 0x001427F7
	private static void OnPlayerJoinedRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount++;
		CustomMapTelemetry.maxPlayersInMap = Math.Max(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.maxPlayersInMap);
	}

	// Token: 0x06003D15 RID: 15637 RVA: 0x00144619 File Offset: 0x00142819
	private static void OnPlayerLeftRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount--;
		CustomMapTelemetry.minPlayersInMap = Math.Min(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.minPlayersInMap);
	}

	// Token: 0x06003D16 RID: 15638 RVA: 0x0014463C File Offset: 0x0014283C
	public static void StartMapTracking()
	{
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.mapEnterTime = Time.unscaledTime;
		float value = Random.value;
		if (value <= 0.01f)
		{
			CustomMapTelemetry.StartMetricsCapture();
		}
		else if (value >= 0.99f)
		{
			CustomMapTelemetry.StartPerfCapture();
		}
		if (!CustomMapTelemetry.metricsCaptureStarted)
		{
			bool flag = CustomMapTelemetry.perfCaptureStarted;
		}
	}

	// Token: 0x06003D17 RID: 15639 RVA: 0x00144691 File Offset: 0x00142891
	public static void EndMapTracking()
	{
		CustomMapTelemetry.EndMetricsCapture();
		CustomMapTelemetry.EndPerfCapture();
		CustomMapTelemetry.mapName = "NULL";
		CustomMapTelemetry.mapCreatorUsername = "NULL";
		CustomMapTelemetry.mapEnterTime = -1f;
		CustomMapTelemetry.mapModId = 0L;
	}

	// Token: 0x06003D18 RID: 15640 RVA: 0x001446C4 File Offset: 0x001428C4
	private static void StartMetricsCapture()
	{
		if (CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = true;
		NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(CustomMapTelemetry.OnPlayerJoinedRoom);
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(CustomMapTelemetry.OnPlayerJoinedRoom);
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(CustomMapTelemetry.OnPlayerLeftRoom);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(CustomMapTelemetry.OnPlayerLeftRoom);
		CustomMapTelemetry.runningPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		CustomMapTelemetry.minPlayersInMap = CustomMapTelemetry.runningPlayerCount;
		CustomMapTelemetry.maxPlayersInMap = CustomMapTelemetry.runningPlayerCount;
	}

	// Token: 0x06003D19 RID: 15641 RVA: 0x00144788 File Offset: 0x00142988
	private static void EndMetricsCapture()
	{
		if (!CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = false;
		NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(CustomMapTelemetry.OnPlayerJoinedRoom);
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(CustomMapTelemetry.OnPlayerLeftRoom);
		CustomMapTelemetry.inPrivateRoom = (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate);
		int num = Mathf.RoundToInt(Time.unscaledTime - CustomMapTelemetry.mapEnterTime);
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndMetricsCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapTracking(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.mapCreatorUsername, CustomMapTelemetry.minPlayersInMap, CustomMapTelemetry.maxPlayersInMap, num, CustomMapTelemetry.inPrivateRoom);
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x00144864 File Offset: 0x00142A64
	private static void StartPerfCapture()
	{
		if (CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = true;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndPerfCapture();
		}
		CustomMapTelemetry.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1, 24);
		CustomMapTelemetry.LowestFPS = int.MaxValue;
		CustomMapTelemetry.HighestFPS = int.MinValue;
		CustomMapTelemetry.totalFPS = 0;
		CustomMapTelemetry.totalDrawCalls = 0;
		CustomMapTelemetry.totalPlayerCount = 0;
		CustomMapTelemetry.frameCounter = 0;
		CustomMapTelemetry.instance.perfCaptureCoroutine = CustomMapTelemetry.instance.StartCoroutine(CustomMapTelemetry.instance.CaptureMapPerformance());
	}

	// Token: 0x06003D1B RID: 15643 RVA: 0x001448FC File Offset: 0x00142AFC
	private static void EndPerfCapture()
	{
		if (!CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = false;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.instance.StopAllCoroutines();
			CustomMapTelemetry.instance.perfCaptureCoroutine = null;
		}
		CustomMapTelemetry.drawCallsRecorder.Dispose();
		if (CustomMapTelemetry.frameCounter == 0)
		{
			return;
		}
		int num = Mathf.RoundToInt(Time.unscaledTime - CustomMapTelemetry.mapEnterTime);
		CustomMapTelemetry.AverageFPS = CustomMapTelemetry.totalFPS / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AverageDrawCalls = CustomMapTelemetry.totalDrawCalls / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AveragePlayerCount = CustomMapTelemetry.totalPlayerCount / CustomMapTelemetry.frameCounter;
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndPerfCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapPerformance(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.LowestFPS, CustomMapTelemetry.LowestFPSDrawCalls, CustomMapTelemetry.LowestFPSPlayerCount, CustomMapTelemetry.AverageFPS, CustomMapTelemetry.AverageDrawCalls, CustomMapTelemetry.AveragePlayerCount, CustomMapTelemetry.HighestFPS, CustomMapTelemetry.HighestFPSDrawCalls, CustomMapTelemetry.HighestFPSPlayerCount, num);
	}

	// Token: 0x06003D1C RID: 15644 RVA: 0x001449F7 File Offset: 0x00142BF7
	private IEnumerator CaptureMapPerformance()
	{
		for (;;)
		{
			int num = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
			int num2 = Mathf.RoundToInt((float)CustomMapTelemetry.drawCallsRecorder.LastValue);
			int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
			CustomMapTelemetry.totalFPS += num;
			CustomMapTelemetry.totalDrawCalls += num2;
			CustomMapTelemetry.totalPlayerCount += roomPlayerCount;
			if (num > CustomMapTelemetry.HighestFPS)
			{
				CustomMapTelemetry.HighestFPS = num;
				CustomMapTelemetry.HighestFPSDrawCalls = num2;
				CustomMapTelemetry.HighestFPSPlayerCount = roomPlayerCount;
			}
			if (num < CustomMapTelemetry.LowestFPS)
			{
				CustomMapTelemetry.LowestFPS = num;
				CustomMapTelemetry.LowestFPSDrawCalls = num2;
				CustomMapTelemetry.LowestFPSPlayerCount = roomPlayerCount;
			}
			CustomMapTelemetry.frameCounter++;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x001449FF File Offset: 0x00142BFF
	private void OnDestroy()
	{
		if (this.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x04004DC9 RID: 19913
	[OnEnterPlay_SetNull]
	private static volatile CustomMapTelemetry instance;

	// Token: 0x04004DCA RID: 19914
	private static string mapName;

	// Token: 0x04004DCB RID: 19915
	private static long mapModId;

	// Token: 0x04004DCC RID: 19916
	private static string mapCreatorUsername;

	// Token: 0x04004DCD RID: 19917
	private static bool metricsCaptureStarted;

	// Token: 0x04004DCE RID: 19918
	private static float mapEnterTime;

	// Token: 0x04004DCF RID: 19919
	private static int runningPlayerCount;

	// Token: 0x04004DD0 RID: 19920
	private static int minPlayersInMap;

	// Token: 0x04004DD1 RID: 19921
	private static int maxPlayersInMap;

	// Token: 0x04004DD2 RID: 19922
	private static bool inPrivateRoom;

	// Token: 0x04004DD3 RID: 19923
	private const int minimumPlaytimeForTracking = 30;

	// Token: 0x04004DD4 RID: 19924
	private static int LowestFPS = int.MaxValue;

	// Token: 0x04004DD5 RID: 19925
	private static int LowestFPSDrawCalls;

	// Token: 0x04004DD6 RID: 19926
	private static int LowestFPSPlayerCount;

	// Token: 0x04004DD7 RID: 19927
	private static int AverageFPS;

	// Token: 0x04004DD8 RID: 19928
	private static int AverageDrawCalls;

	// Token: 0x04004DD9 RID: 19929
	private static int AveragePlayerCount;

	// Token: 0x04004DDA RID: 19930
	private static int HighestFPS = int.MinValue;

	// Token: 0x04004DDB RID: 19931
	private static int HighestFPSDrawCalls;

	// Token: 0x04004DDC RID: 19932
	private static int HighestFPSPlayerCount;

	// Token: 0x04004DDD RID: 19933
	private static int totalFPS;

	// Token: 0x04004DDE RID: 19934
	private static int totalDrawCalls;

	// Token: 0x04004DDF RID: 19935
	private static int totalPlayerCount;

	// Token: 0x04004DE0 RID: 19936
	private static int frameCounter;

	// Token: 0x04004DE1 RID: 19937
	private Coroutine perfCaptureCoroutine;

	// Token: 0x04004DE2 RID: 19938
	private static ProfilerRecorder drawCallsRecorder;

	// Token: 0x04004DE3 RID: 19939
	private static bool perfCaptureStarted;
}
