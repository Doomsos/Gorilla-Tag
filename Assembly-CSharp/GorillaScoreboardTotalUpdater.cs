using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTag;
using UnityEngine;

// Token: 0x02000921 RID: 2337
public class GorillaScoreboardTotalUpdater : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003BBC RID: 15292 RVA: 0x0013B948 File Offset: 0x00139B48
	public void UpdateLineState(GorillaPlayerScoreboardLine line)
	{
		if (line.playerActorNumber == -1)
		{
			return;
		}
		if (this.reportDict.ContainsKey(line.playerActorNumber))
		{
			this.reportDict[line.playerActorNumber] = new GorillaScoreboardTotalUpdater.PlayerReports(this.reportDict[line.playerActorNumber], line);
			return;
		}
		this.reportDict.Add(line.playerActorNumber, new GorillaScoreboardTotalUpdater.PlayerReports(line));
	}

	// Token: 0x06003BBD RID: 15293 RVA: 0x0013B9B2 File Offset: 0x00139BB2
	protected void Awake()
	{
		if (GorillaScoreboardTotalUpdater.hasInstance && GorillaScoreboardTotalUpdater.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaScoreboardTotalUpdater.SetInstance(this);
	}

	// Token: 0x06003BBE RID: 15294 RVA: 0x0013B9D8 File Offset: 0x00139BD8
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.JoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06003BBF RID: 15295 RVA: 0x0013BA51 File Offset: 0x00139C51
	public static void CreateManager()
	{
		GorillaScoreboardTotalUpdater.SetInstance(new GameObject("GorillaScoreboardTotalUpdater").AddComponent<GorillaScoreboardTotalUpdater>());
	}

	// Token: 0x06003BC0 RID: 15296 RVA: 0x0013BA67 File Offset: 0x00139C67
	private static void SetInstance(GorillaScoreboardTotalUpdater manager)
	{
		GorillaScoreboardTotalUpdater.instance = manager;
		GorillaScoreboardTotalUpdater.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06003BC1 RID: 15297 RVA: 0x0013BA82 File Offset: 0x00139C82
	public static void RegisterSL(GorillaPlayerScoreboardLine sL)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (!GorillaScoreboardTotalUpdater.allScoreboardLines.Contains(sL))
		{
			GorillaScoreboardTotalUpdater.allScoreboardLines.Add(sL);
		}
	}

	// Token: 0x06003BC2 RID: 15298 RVA: 0x0013BAA8 File Offset: 0x00139CA8
	public static void UnregisterSL(GorillaPlayerScoreboardLine sL)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (GorillaScoreboardTotalUpdater.allScoreboardLines.Contains(sL))
		{
			GorillaScoreboardTotalUpdater.allScoreboardLines.Remove(sL);
		}
	}

	// Token: 0x06003BC3 RID: 15299 RVA: 0x0013BACF File Offset: 0x00139CCF
	public static void RegisterScoreboard(GorillaScoreBoard sB)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (!GorillaScoreboardTotalUpdater.allScoreboards.Contains(sB))
		{
			GorillaScoreboardTotalUpdater.allScoreboards.Add(sB);
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(sB);
		}
	}

	// Token: 0x06003BC4 RID: 15300 RVA: 0x0013BB00 File Offset: 0x00139D00
	public static void UnregisterScoreboard(GorillaScoreBoard sB)
	{
		if (!GorillaScoreboardTotalUpdater.hasInstance)
		{
			GorillaScoreboardTotalUpdater.CreateManager();
		}
		if (GorillaScoreboardTotalUpdater.allScoreboards.Contains(sB))
		{
			GorillaScoreboardTotalUpdater.allScoreboards.Remove(sB);
		}
	}

	// Token: 0x06003BC5 RID: 15301 RVA: 0x0013BB28 File Offset: 0x00139D28
	public void UpdateActiveScoreboards()
	{
		for (int i = 0; i < GorillaScoreboardTotalUpdater.allScoreboards.Count; i++)
		{
			this.UpdateScoreboard(GorillaScoreboardTotalUpdater.allScoreboards[i]);
		}
	}

	// Token: 0x06003BC6 RID: 15302 RVA: 0x0013BB5B File Offset: 0x00139D5B
	public void SetOfflineFailureText(string failureText)
	{
		this.offlineTextErrorString = failureText;
		this.UpdateActiveScoreboards();
	}

	// Token: 0x06003BC7 RID: 15303 RVA: 0x0013BB6A File Offset: 0x00139D6A
	public void ClearOfflineFailureText()
	{
		this.offlineTextErrorString = null;
		this.UpdateActiveScoreboards();
	}

	// Token: 0x06003BC8 RID: 15304 RVA: 0x0013BB7C File Offset: 0x00139D7C
	public void UpdateScoreboard(GorillaScoreBoard sB)
	{
		sB.SetSleepState(this.joinedRoom);
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (!this.joinedRoom)
		{
			if (sB.notInRoomText != null)
			{
				sB.notInRoomText.gameObject.SetActive(true);
				sB.notInRoomText.text = ((this.offlineTextErrorString != null) ? this.offlineTextErrorString : GorillaComputer.instance.offlineTextInitialString);
			}
			for (int i = 0; i < sB.lines.Count; i++)
			{
				sB.lines[i].ResetData();
			}
			return;
		}
		if (sB.notInRoomText != null)
		{
			sB.notInRoomText.gameObject.SetActive(false);
		}
		for (int j = 0; j < sB.lines.Count; j++)
		{
			GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine = sB.lines[j];
			if (j < this.playersInRoom.Count)
			{
				gorillaPlayerScoreboardLine.gameObject.SetActive(true);
				gorillaPlayerScoreboardLine.SetLineData(this.playersInRoom[j]);
			}
			else
			{
				gorillaPlayerScoreboardLine.ResetData();
				gorillaPlayerScoreboardLine.gameObject.SetActive(false);
			}
		}
		sB.RedrawPlayerLines();
	}

	// Token: 0x06003BC9 RID: 15305 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003BCA RID: 15306 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003BCB RID: 15307 RVA: 0x0013BCA4 File Offset: 0x00139EA4
	public void SliceUpdate()
	{
		if (GorillaScoreboardTotalUpdater.allScoreboardLines.Count == 0)
		{
			return;
		}
		for (int i = 0; i < GorillaScoreboardTotalUpdater.linesPerFrame; i++)
		{
			if (GorillaScoreboardTotalUpdater.lineIndex >= GorillaScoreboardTotalUpdater.allScoreboardLines.Count)
			{
				GorillaScoreboardTotalUpdater.lineIndex = 0;
			}
			GorillaScoreboardTotalUpdater.allScoreboardLines[GorillaScoreboardTotalUpdater.lineIndex].UpdateLine();
			GorillaScoreboardTotalUpdater.lineIndex++;
		}
		for (int j = 0; j < GorillaScoreboardTotalUpdater.allScoreboards.Count; j++)
		{
			if (GorillaScoreboardTotalUpdater.allScoreboards[j].IsDirty)
			{
				this.UpdateScoreboard(GorillaScoreboardTotalUpdater.allScoreboards[j]);
			}
		}
	}

	// Token: 0x06003BCC RID: 15308 RVA: 0x0013BD3D File Offset: 0x00139F3D
	private void OnPlayerEnteredRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Null netplayer");
		}
		if (!this.playersInRoom.Contains(netPlayer))
		{
			this.playersInRoom.Add(netPlayer);
		}
		this.UpdateActiveScoreboards();
	}

	// Token: 0x06003BCD RID: 15309 RVA: 0x0013BD6C File Offset: 0x00139F6C
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Null netplayer");
		}
		this.playersInRoom.Remove(netPlayer);
		this.UpdateActiveScoreboards();
		ReportMuteTimer reportMuteTimer;
		if (GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.TryGetValue(netPlayer.ActorNumber, ref reportMuteTimer))
		{
			GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.Remove(netPlayer.ActorNumber);
			GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Return(reportMuteTimer);
		}
	}

	// Token: 0x06003BCE RID: 15310 RVA: 0x0013BDCC File Offset: 0x00139FCC
	internal void JoinedRoom()
	{
		this.joinedRoom = true;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			this.playersInRoom.Add(netPlayer);
		}
		this.playersInRoom.Sort((NetPlayer x, NetPlayer y) => x.ActorNumber.CompareTo(y.ActorNumber));
		foreach (GorillaScoreBoard sB in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			this.UpdateScoreboard(sB);
		}
	}

	// Token: 0x06003BCF RID: 15311 RVA: 0x0013BE7C File Offset: 0x0013A07C
	private void OnLeftRoom()
	{
		this.joinedRoom = false;
		this.playersInRoom.Clear();
		this.reportDict.Clear();
		foreach (GorillaScoreBoard sB in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			this.UpdateScoreboard(sB);
		}
		foreach (KeyValuePair<int, ReportMuteTimer> keyValuePair in GorillaScoreboardTotalUpdater.m_reportMuteTimerDict)
		{
			GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Return(keyValuePair.Value);
		}
		GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.Clear();
	}

	// Token: 0x06003BD0 RID: 15312 RVA: 0x0013BF40 File Offset: 0x0013A140
	public static void ReportMute(NetPlayer player, int muted)
	{
		ReportMuteTimer reportMuteTimer;
		if (GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.TryGetValue(player.ActorNumber, ref reportMuteTimer))
		{
			reportMuteTimer.Muted = muted;
			if (!reportMuteTimer.Running)
			{
				reportMuteTimer.Start();
			}
			return;
		}
		reportMuteTimer = GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Take();
		reportMuteTimer.SetReportData(player.UserId, player.NickName, muted);
		reportMuteTimer.coolDown = 5f;
		reportMuteTimer.Start();
		GorillaScoreboardTotalUpdater.m_reportMuteTimerDict[player.ActorNumber] = reportMuteTimer;
	}

	// Token: 0x04004C45 RID: 19525
	public static GorillaScoreboardTotalUpdater instance;

	// Token: 0x04004C46 RID: 19526
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04004C47 RID: 19527
	public static List<GorillaPlayerScoreboardLine> allScoreboardLines = new List<GorillaPlayerScoreboardLine>();

	// Token: 0x04004C48 RID: 19528
	public static int lineIndex = 0;

	// Token: 0x04004C49 RID: 19529
	private static int linesPerFrame = 2;

	// Token: 0x04004C4A RID: 19530
	public static List<GorillaScoreBoard> allScoreboards = new List<GorillaScoreBoard>();

	// Token: 0x04004C4B RID: 19531
	public static int boardIndex = 0;

	// Token: 0x04004C4C RID: 19532
	private List<NetPlayer> playersInRoom = new List<NetPlayer>();

	// Token: 0x04004C4D RID: 19533
	private bool joinedRoom;

	// Token: 0x04004C4E RID: 19534
	private bool wasGameManagerNull;

	// Token: 0x04004C4F RID: 19535
	public bool forOverlay;

	// Token: 0x04004C50 RID: 19536
	public string offlineTextErrorString;

	// Token: 0x04004C51 RID: 19537
	public Dictionary<int, GorillaScoreboardTotalUpdater.PlayerReports> reportDict = new Dictionary<int, GorillaScoreboardTotalUpdater.PlayerReports>();

	// Token: 0x04004C52 RID: 19538
	private static readonly Dictionary<int, ReportMuteTimer> m_reportMuteTimerDict = new Dictionary<int, ReportMuteTimer>(10);

	// Token: 0x04004C53 RID: 19539
	private static readonly ObjectPool<ReportMuteTimer> m_reportMuteTimerPool = new ObjectPool<ReportMuteTimer>(10);

	// Token: 0x02000922 RID: 2338
	public struct PlayerReports
	{
		// Token: 0x06003BD3 RID: 15315 RVA: 0x0013C02C File Offset: 0x0013A22C
		public PlayerReports(GorillaScoreboardTotalUpdater.PlayerReports reportToUpdate, GorillaPlayerScoreboardLine lineToUpdate)
		{
			this.cheating = (reportToUpdate.cheating || lineToUpdate.reportedCheating);
			this.toxicity = (reportToUpdate.toxicity || lineToUpdate.reportedToxicity);
			this.hateSpeech = (reportToUpdate.hateSpeech || lineToUpdate.reportedHateSpeech);
			this.pressedReport = lineToUpdate.reportInProgress;
		}

		// Token: 0x06003BD4 RID: 15316 RVA: 0x0013C08A File Offset: 0x0013A28A
		public PlayerReports(GorillaPlayerScoreboardLine lineToUpdate)
		{
			this.cheating = lineToUpdate.reportedCheating;
			this.toxicity = lineToUpdate.reportedToxicity;
			this.hateSpeech = lineToUpdate.reportedHateSpeech;
			this.pressedReport = lineToUpdate.reportInProgress;
		}

		// Token: 0x04004C54 RID: 19540
		public bool cheating;

		// Token: 0x04004C55 RID: 19541
		public bool toxicity;

		// Token: 0x04004C56 RID: 19542
		public bool hateSpeech;

		// Token: 0x04004C57 RID: 19543
		public bool pressedReport;
	}
}
