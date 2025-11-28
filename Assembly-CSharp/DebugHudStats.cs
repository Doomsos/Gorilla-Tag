using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using TMPro;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000C3F RID: 3135
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x17000733 RID: 1843
	// (get) Token: 0x06004CF5 RID: 19701 RVA: 0x0018F576 File Offset: 0x0018D776
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	// Token: 0x06004CF6 RID: 19702 RVA: 0x0018F57D File Offset: 0x0018D77D
	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004CF7 RID: 19703 RVA: 0x0018F5B8 File Offset: 0x0018D7B8
	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
			if (this.drawCallsRecorder.Valid)
			{
				this.drawCallsRecorder.Dispose();
			}
			if (this.trisRecorder.Valid)
			{
				this.trisRecorder.Dispose();
			}
		}
	}

	// Token: 0x06004CF8 RID: 19704 RVA: 0x0018F608 File Offset: 0x0018D808
	private void LateUpdate()
	{
		base.transform.LookAt(Camera.main.transform.position, Vector3.up);
		bool flag = ControllerInputPoller.SecondaryButtonPress(4);
		if (this.buttonDown && !flag)
		{
			Application.logMessageReceived -= new Application.LogCallback(this.LogMessageReceived);
			PlayerGameEvents.OnPlayerMoved -= new Action<float, float>(this.OnPlayerMoved);
			PlayerGameEvents.OnPlayerSwam -= new Action<float, float>(this.OnPlayerSwam);
			switch (this.currentState)
			{
			case DebugHudStats.State.Inactive:
				this.currentState = DebugHudStats.State.Active;
				break;
			case DebugHudStats.State.Active:
				this.currentState = DebugHudStats.State.ShowLog;
				break;
			case DebugHudStats.State.ShowLog:
				this.currentState = DebugHudStats.State.ShowError;
				break;
			case DebugHudStats.State.ShowError:
				this.currentState = DebugHudStats.State.ShowStats;
				break;
			case DebugHudStats.State.ShowStats:
				this.currentState = DebugHudStats.State.ShowRBs;
				break;
			case DebugHudStats.State.ShowRBs:
				this.currentState = DebugHudStats.State.Inactive;
				break;
			}
			Application.logMessageReceived -= new Application.LogCallback(this.LogMessageReceived);
			PlayerGameEvents.OnPlayerMoved -= new Action<float, float>(this.OnPlayerMoved);
			PlayerGameEvents.OnPlayerSwam -= new Action<float, float>(this.OnPlayerSwam);
			DebugHudStats.State state = this.currentState;
			if (state - DebugHudStats.State.ShowLog > 1)
			{
				if (state == DebugHudStats.State.ShowStats)
				{
					this.distanceMoved = (this.distanceSwam = 0f);
					PlayerGameEvents.OnPlayerMoved += new Action<float, float>(this.OnPlayerMoved);
					PlayerGameEvents.OnPlayerSwam += new Action<float, float>(this.OnPlayerSwam);
				}
			}
			else
			{
				Application.logMessageReceived += new Application.LogCallback(this.LogMessageReceived);
			}
			this.logMessages.Clear();
			this.logVerbosity = ((this.currentState == DebugHudStats.State.ShowError) ? 1 : 0);
			this.text.gameObject.SetActive(this.currentState > DebugHudStats.State.Inactive);
			if (RigidbodyHighlighter.Instance != null)
			{
				RigidbodyHighlighter.Instance.Active = (this.currentState == DebugHudStats.State.ShowRBs);
			}
		}
		this.buttonDown = flag;
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < 89)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5 && this.currentState == DebugHudStats.State.Inactive);
		if (this.currentState != DebugHudStats.State.Inactive)
		{
			this.builder.Clear();
			this.builder.Append("<color=\"" + this.colorFromState(this.currentState) + "\">");
			this.builder.Append("gt: ");
			this.builder.Append(GorillaComputer.instance.version);
			this.builder.Append(":");
			this.builder.Append(GorillaComputer.instance.buildCode);
			this.builder.Append("</color>");
			num = Mathf.Min(num, 90);
			this.builder.Append((num < 89) ? " - <color=\"red\">" : " - <color=\"white\">");
			this.builder.Append(num);
			this.builder.AppendLine(" fps</color>");
			this.builder.AppendLine(string.Format("draw calls: {0} tris: {1}", this.drawCallsRecorder.LastValue, this.trisRecorder.LastValue));
			if (GorillaComputer.instance != null)
			{
				this.builder.AppendLine(GorillaComputer.instance.GetServerTime().ToString());
			}
			else
			{
				this.builder.AppendLine("Server Time Unavailable");
			}
			this.zones = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString().ToUpperInvariant();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.builder.Append("H");
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					this.builder.Append("Pri ");
				}
				else
				{
					this.builder.Append("Pub ");
				}
			}
			else
			{
				this.builder.Append("DC ");
			}
			this.builder.Append("z: <color=\"green\">");
			this.builder.Append(this.zones);
			this.builder.AppendLine("</color>");
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaGameManager instance = GorillaGameManager.instance;
				if (instance != null)
				{
					GorillaTagCompetitiveManager gorillaTagCompetitiveManager = instance as GorillaTagCompetitiveManager;
					if (gorillaTagCompetitiveManager != null)
					{
						this.builder.Append("Ranked Mode ELO: ");
						this.builder.Append(gorillaTagCompetitiveManager.GetScoring().Progression.GetEloScore().ToString());
						this.builder.Append("  Tier: ");
						this.builder.AppendLine(gorillaTagCompetitiveManager.GetScoring().Progression.GetRankedProgressionTierName());
						RankedMultiplayerScore.PlayerScoreInRound inGameScoreForSelf = gorillaTagCompetitiveManager.GetScoring().GetInGameScoreForSelf();
						this.builder.Append("Tags: ");
						this.builder.Append(inGameScoreForSelf.NumTags.ToString());
						this.builder.Append("  Defense: ");
						this.builder.Append(Mathf.RoundToInt(inGameScoreForSelf.PointsOnDefense).ToString());
						this.builder.Append("  Score: ");
						this.builder.AppendLine(Mathf.RoundToInt(gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(inGameScoreForSelf.NumTags, inGameScoreForSelf.PointsOnDefense)).ToString());
						if (gorillaTagCompetitiveManager.ShowDebugPing)
						{
							this.builder.AppendLine("Server MatchID Ping!");
						}
					}
				}
			}
			if (this.currentState == DebugHudStats.State.ShowStats)
			{
				this.builder.AppendLine();
				Vector3 vector = GTPlayer.Instance.AveragedVelocity;
				Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
				float magnitude = vector.magnitude;
				this.groundVelocity = vector;
				this.groundVelocity.y = 0f;
				this.builder.AppendLine(string.Format("v: {0:F1} m/s\t\todo: {1:F2}m\tswam: {2:F2}m", magnitude, this.distanceMoved, this.distanceSwam));
				this.builder.AppendLine(string.Format("ground: {0:F1} m/s\thead: {1:F2}", this.groundVelocity.magnitude, headCenterPosition));
			}
			else if (this.currentState == DebugHudStats.State.ShowLog || this.currentState == DebugHudStats.State.ShowError)
			{
				this.builder.AppendLine();
				for (int i = this.logMessages.Count - 1; i >= 0; i--)
				{
					if (this.logFilter.Length == 0 || this.logMessages[i].Contains(this.logFilter))
					{
						this.builder.AppendLine(this.logMessages[i]);
					}
				}
			}
			this.text.text = this.builder.ToString();
		}
		this.updateTimer = 0f;
	}

	// Token: 0x06004CF9 RID: 19705 RVA: 0x0018FD31 File Offset: 0x0018DF31
	private string colorFromState(DebugHudStats.State s)
	{
		switch (s)
		{
		case DebugHudStats.State.ShowLog:
			return "yellow";
		case DebugHudStats.State.ShowError:
			return "orange";
		case DebugHudStats.State.ShowStats:
			return "green";
		case DebugHudStats.State.ShowRBs:
			return "red";
		default:
			return "white";
		}
	}

	// Token: 0x06004CFA RID: 19706 RVA: 0x0018FD6A File Offset: 0x0018DF6A
	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	// Token: 0x06004CFB RID: 19707 RVA: 0x0018FD82 File Offset: 0x0018DF82
	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	// Token: 0x06004CFC RID: 19708 RVA: 0x0018FD9A File Offset: 0x0018DF9A
	private void OnDisable()
	{
		Application.logMessageReceived -= new Application.LogCallback(this.LogMessageReceived);
	}

	// Token: 0x06004CFD RID: 19709 RVA: 0x0018FDB0 File Offset: 0x0018DFB0
	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (this.logVerbosity == 1 && type != 4 && type != 1 && type != null)
		{
			return;
		}
		string text = string.Format("{0:F2}> {1}{2}</color>", Time.realtimeSinceStartup, this.getColorStringFromLogType(type), condition);
		if (this.pLog != condition)
		{
			this.logMessages.Add(text);
		}
		else
		{
			this.logMessages[this.logMessages.Count - 1] = text;
		}
		this.pLog = condition;
		if (this.logMessages.Count > 10)
		{
			this.logMessages.RemoveAt(0);
		}
	}

	// Token: 0x06004CFE RID: 19710 RVA: 0x0018FE46 File Offset: 0x0018E046
	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case 0:
		case 1:
		case 4:
			return "<color=\"red\">";
		case 2:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	// Token: 0x06004CFF RID: 19711 RVA: 0x0018FE78 File Offset: 0x0018E078
	private void OnZoneChanged(ZoneData[] zoneData)
	{
		this.zones = string.Empty;
		for (int i = 0; i < zoneData.Length; i++)
		{
			if (zoneData[i].active)
			{
				this.zones = this.zones + zoneData[i].zone.ToString().ToUpper() + "; ";
			}
		}
	}

	// Token: 0x04005C8C RID: 23692
	private const int FPS_THRESHOLD = 89;

	// Token: 0x04005C8D RID: 23693
	private static DebugHudStats _instance;

	// Token: 0x04005C8E RID: 23694
	[SerializeField]
	public TMP_Text text;

	// Token: 0x04005C8F RID: 23695
	[SerializeField]
	private TMP_Text fpsWarning;

	// Token: 0x04005C90 RID: 23696
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x04005C91 RID: 23697
	private float updateTimer;

	// Token: 0x04005C92 RID: 23698
	public float sessionAnytrackingLost;

	// Token: 0x04005C93 RID: 23699
	public float last30SecondsTrackingLost;

	// Token: 0x04005C94 RID: 23700
	private float firstAwake;

	// Token: 0x04005C95 RID: 23701
	private bool leftHandTracked;

	// Token: 0x04005C96 RID: 23702
	private bool rightHandTracked;

	// Token: 0x04005C97 RID: 23703
	private StringBuilder builder;

	// Token: 0x04005C98 RID: 23704
	private Vector3 averagedVelocity;

	// Token: 0x04005C99 RID: 23705
	private Vector3 groundVelocity;

	// Token: 0x04005C9A RID: 23706
	private Vector3 centerHeadPos;

	// Token: 0x04005C9B RID: 23707
	private float distanceMoved;

	// Token: 0x04005C9C RID: 23708
	private float distanceSwam;

	// Token: 0x04005C9D RID: 23709
	private List<string> logMessages = new List<string>();

	// Token: 0x04005C9E RID: 23710
	private bool buttonDown;

	// Token: 0x04005C9F RID: 23711
	private bool showLog;

	// Token: 0x04005CA0 RID: 23712
	private int lowFps;

	// Token: 0x04005CA1 RID: 23713
	private string zones;

	// Token: 0x04005CA2 RID: 23714
	private GroupJoinZoneAB lastGroupJoinZone;

	// Token: 0x04005CA3 RID: 23715
	[SerializeField]
	private string logFilter;

	// Token: 0x04005CA4 RID: 23716
	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	// Token: 0x04005CA5 RID: 23717
	private int logVerbosity;

	// Token: 0x04005CA6 RID: 23718
	private ProfilerRecorder drawCallsRecorder;

	// Token: 0x04005CA7 RID: 23719
	private ProfilerRecorder trisRecorder;

	// Token: 0x04005CA8 RID: 23720
	private string pLog;

	// Token: 0x02000C40 RID: 3136
	private enum State
	{
		// Token: 0x04005CAA RID: 23722
		Inactive,
		// Token: 0x04005CAB RID: 23723
		Active,
		// Token: 0x04005CAC RID: 23724
		ShowLog,
		// Token: 0x04005CAD RID: 23725
		ShowError,
		// Token: 0x04005CAE RID: 23726
		ShowStats,
		// Token: 0x04005CAF RID: 23727
		ShowRBs
	}
}
