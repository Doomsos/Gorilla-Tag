using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using TMPro;
using Unity.Profiling;
using UnityEngine;

public class DebugHudStats : MonoBehaviour
{
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

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

	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= new Application.LogCallback(this.LogMessageReceived);
	}

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

	private const int FPS_THRESHOLD = 89;

	private static DebugHudStats _instance;

	[SerializeField]
	public TMP_Text text;

	[SerializeField]
	private TMP_Text fpsWarning;

	[SerializeField]
	private float delayUpdateRate = 0.25f;

	private float updateTimer;

	public float sessionAnytrackingLost;

	public float last30SecondsTrackingLost;

	private float firstAwake;

	private bool leftHandTracked;

	private bool rightHandTracked;

	private StringBuilder builder;

	private Vector3 averagedVelocity;

	private Vector3 groundVelocity;

	private Vector3 centerHeadPos;

	private float distanceMoved;

	private float distanceSwam;

	private List<string> logMessages = new List<string>();

	private bool buttonDown;

	private bool showLog;

	private int lowFps;

	private string zones;

	private GroupJoinZoneAB lastGroupJoinZone;

	[SerializeField]
	private string logFilter;

	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	private int logVerbosity;

	private ProfilerRecorder drawCallsRecorder;

	private ProfilerRecorder trisRecorder;

	private string pLog;

	private enum State
	{
		Inactive,
		Active,
		ShowLog,
		ShowError,
		ShowStats,
		ShowRBs
	}
}
