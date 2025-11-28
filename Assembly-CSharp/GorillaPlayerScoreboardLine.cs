using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200091B RID: 2331
public class GorillaPlayerScoreboardLine : MonoBehaviour
{
	// Token: 0x06003B82 RID: 15234 RVA: 0x0013A362 File Offset: 0x00138562
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
	}

	// Token: 0x06003B83 RID: 15235 RVA: 0x0013A380 File Offset: 0x00138580
	public void InitializeLine()
	{
		this.currentNickname = string.Empty;
		this.UpdatePlayerText();
		if (this.linePlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.muteButton.gameObject.SetActive(false);
			this.reportButton.gameObject.SetActive(false);
			this.hateSpeechButton.SetActive(false);
			this.toxicityButton.SetActive(false);
			this.cheatingButton.SetActive(false);
			this.cancelButton.SetActive(false);
			return;
		}
		this.muteButton.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null && GorillaScoreboardTotalUpdater.instance.reportDict.ContainsKey(this.playerActorNumber))
		{
			GorillaScoreboardTotalUpdater.PlayerReports playerReports = GorillaScoreboardTotalUpdater.instance.reportDict[this.playerActorNumber];
			this.reportedCheating = playerReports.cheating;
			this.reportedHateSpeech = playerReports.hateSpeech;
			this.reportedToxicity = playerReports.toxicity;
			this.reportInProgress = playerReports.pressedReport;
		}
		else
		{
			this.reportedCheating = false;
			this.reportedHateSpeech = false;
			this.reportedToxicity = false;
			this.reportInProgress = false;
		}
		this.reportButton.isOn = (this.reportedCheating || this.reportedHateSpeech || this.reportedToxicity);
		this.reportButton.UpdateColor();
		this.SwapToReportState(this.reportInProgress);
		this.muteButton.gameObject.SetActive(true);
		this.isMuteManual = PlayerPrefs.HasKey(this.linePlayer.UserId);
		this.mute = PlayerPrefs.GetInt(this.linePlayer.UserId, 0);
		this.muteButton.isOn = (this.mute != 0);
		this.muteButton.isAutoOn = false;
		this.muteButton.UpdateColor();
		if (this.rigContainer != null)
		{
			this.rigContainer.hasManualMute = this.isMuteManual;
			this.rigContainer.Muted = (this.mute != 0);
		}
	}

	// Token: 0x06003B84 RID: 15236 RVA: 0x0013A57C File Offset: 0x0013877C
	public void SetLineData(NetPlayer netPlayer)
	{
		if (!netPlayer.InRoom || netPlayer == this.linePlayer)
		{
			return;
		}
		if (this.playerActorNumber != netPlayer.ActorNumber)
		{
			this.initTime = Time.time;
		}
		this.playerActorNumber = netPlayer.ActorNumber;
		this.linePlayer = netPlayer;
		this.playerNameValue = (netPlayer.NickName ?? "");
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			this.rigContainer = rigContainer;
			this.playerVRRig = rigContainer.Rig;
		}
		this.InitializeLine();
	}

	// Token: 0x06003B85 RID: 15237 RVA: 0x0013A604 File Offset: 0x00138804
	public void UpdateLine()
	{
		if (this.linePlayer != null)
		{
			if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
			{
				this.UpdatePlayerText();
				this.parentScoreboard.IsDirty = true;
				if (this.playerVRRig.creator.IsMasterClient && GorillaComputer.instance.IsPlayerInVirtualStump())
				{
					CustomMapModeSelector.RefreshHostName();
				}
			}
			if (this.rigContainer != null)
			{
				if (Time.time > this.initTime + this.emptyRigCooldown)
				{
					if (this.playerVRRig.netView != null)
					{
						this.emptyRigCount = 0;
					}
					else
					{
						this.emptyRigCount++;
						if (this.emptyRigCount > 30)
						{
							GorillaNot.instance.SendReport("empty rig", this.linePlayer.UserId, this.linePlayer.NickName);
						}
					}
				}
				Material material;
				if (this.playerVRRig.setMatIndex == 0)
				{
					material = this.playerVRRig.scoreboardMaterial;
				}
				else
				{
					material = this.playerVRRig.materialsToChangeTo[this.playerVRRig.setMatIndex];
				}
				if (this.playerSwatch.material != material)
				{
					this.playerSwatch.material = material;
				}
				if (this.playerSwatch.color != this.playerVRRig.materialsToChangeTo[0].color)
				{
					this.playerSwatch.color = this.playerVRRig.materialsToChangeTo[0].color;
				}
				if (this.myRecorder == null)
				{
					this.myRecorder = NetworkSystem.Instance.LocalRecorder;
				}
				if (this.playerVRRig != null)
				{
					if (this.playerVRRig.remoteUseReplacementVoice || this.playerVRRig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE")
					{
						if (this.playerVRRig.SpeakingLoudness > this.playerVRRig.replacementVoiceLoudnessThreshold && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
						{
							this.speakerIcon.enabled = true;
						}
						else
						{
							this.speakerIcon.enabled = false;
						}
					}
					else if ((this.rigContainer.Voice != null && this.rigContainer.Voice.IsSpeaking) || (this.playerVRRig.rigSerializer != null && this.playerVRRig.rigSerializer.IsLocallyOwned && this.myRecorder != null && this.myRecorder.IsCurrentlyTransmitting))
					{
						this.speakerIcon.enabled = true;
					}
					else
					{
						this.speakerIcon.enabled = false;
					}
				}
				else
				{
					this.speakerIcon.enabled = false;
				}
				if (!this.isMuteManual)
				{
					bool isPlayerAutoMuted = this.rigContainer.GetIsPlayerAutoMuted();
					if (this.muteButton.isAutoOn != isPlayerAutoMuted)
					{
						this.muteButton.isAutoOn = isPlayerAutoMuted;
						this.muteButton.UpdateColor();
					}
				}
			}
		}
	}

	// Token: 0x06003B86 RID: 15238 RVA: 0x0013A8FC File Offset: 0x00138AFC
	private void UpdatePlayerText()
	{
		try
		{
			if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				this.currentNickname = this.linePlayer.NickName;
			}
			else if (this.rigContainer.Initialized)
			{
				this.playerNameVisible = this.playerVRRig.playerNameVisible;
			}
			else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			this.currentNickname = this.linePlayer.NickName;
			this.playerName.text = (flag ? this.playerNameVisible : this.linePlayer.DefaultName);
		}
		catch (Exception)
		{
			this.playerNameVisible = this.linePlayer.DefaultName;
			GorillaNot.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
		}
	}

	// Token: 0x06003B87 RID: 15239 RVA: 0x0013AA74 File Offset: 0x00138C74
	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		if (buttonType != GorillaPlayerLineButton.ButtonType.Mute)
		{
			if (buttonType == GorillaPlayerLineButton.ButtonType.Report)
			{
				this.SetReportState(true, buttonType);
				return;
			}
			this.SetReportState(false, buttonType);
		}
		else if (this.linePlayer != null && this.playerVRRig != null)
		{
			this.isMuteManual = true;
			this.muteButton.isAutoOn = false;
			this.mute = (isOn ? 1 : 0);
			PlayerPrefs.SetInt(this.linePlayer.UserId, this.mute);
			if (this.rigContainer != null)
			{
				this.rigContainer.hasManualMute = this.isMuteManual;
				this.rigContainer.Muted = (this.mute != 0);
			}
			PlayerPrefs.Save();
			this.muteButton.UpdateColor();
			GorillaScoreboardTotalUpdater.ReportMute(this.linePlayer, this.mute);
			return;
		}
	}

	// Token: 0x06003B88 RID: 15240 RVA: 0x0013AB4C File Offset: 0x00138D4C
	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		this.canPressNextReportButton = (buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report);
		this.reportInProgress = reportState;
		if (reportState)
		{
			this.SwapToReportState(true);
		}
		else
		{
			this.SwapToReportState(false);
			if (this.linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				if ((!this.reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!this.reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!this.reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
				{
					GorillaPlayerScoreboardLine.ReportPlayer(this.linePlayer.UserId, buttonType, this.playerNameVisible);
					this.doneReporting = true;
				}
				this.reportedCheating = (this.reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating);
				this.reportedToxicity = (this.reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity);
				this.reportedHateSpeech = (this.reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech);
				this.reportButton.isOn = true;
				this.reportButton.UpdateColor();
			}
		}
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateLineState(this);
		}
		this.parentScoreboard.RedrawPlayerLines();
	}

	// Token: 0x06003B89 RID: 15241 RVA: 0x0013AC58 File Offset: 0x00138E58
	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags flags = new WebFlags(3);
		NetEventOptions options = new NetEventOptions
		{
			Flags = flags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte code = 50;
		object[] data = new object[]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(code, data, options, true);
	}

	// Token: 0x06003B8A RID: 15242 RVA: 0x0013ACF0 File Offset: 0x00138EF0
	public static void MutePlayer(string PlayerID, string OtherPlayerNickName, int muting)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags flags = new WebFlags(3);
		NetEventOptions options = new NetEventOptions
		{
			Flags = flags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte code = 51;
		object[] data = new object[]
		{
			PlayerID,
			muting,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(code, data, options, true);
	}

	// Token: 0x06003B8B RID: 15243 RVA: 0x0013AD88 File Offset: 0x00138F88
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 11);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
				GorillaNot.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}
		return text;
	}

	// Token: 0x06003B8C RID: 15244 RVA: 0x0013AE3F File Offset: 0x0013903F
	public void ResetData()
	{
		this.emptyRigCount = 0;
		this.playerActorNumber = -1;
		this.linePlayer = null;
		this.playerNameValue = string.Empty;
		this.currentNickname = string.Empty;
	}

	// Token: 0x06003B8D RID: 15245 RVA: 0x0013AE6C File Offset: 0x0013906C
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterSL(this);
	}

	// Token: 0x06003B8E RID: 15246 RVA: 0x0013AE74 File Offset: 0x00139074
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterSL(this);
	}

	// Token: 0x06003B8F RID: 15247 RVA: 0x0013AE7C File Offset: 0x0013907C
	private void SwapToReportState(bool reportInProgress)
	{
		this.reportButton.gameObject.SetActive(!reportInProgress);
		this.hateSpeechButton.SetActive(reportInProgress);
		this.toxicityButton.SetActive(reportInProgress);
		this.cheatingButton.SetActive(reportInProgress);
		this.cancelButton.SetActive(reportInProgress);
	}

	// Token: 0x04004BF6 RID: 19446
	private static int[] targetActors = new int[]
	{
		-1
	};

	// Token: 0x04004BF7 RID: 19447
	public Text playerName;

	// Token: 0x04004BF8 RID: 19448
	public Text playerLevel;

	// Token: 0x04004BF9 RID: 19449
	public Text playerMMR;

	// Token: 0x04004BFA RID: 19450
	public Image playerSwatch;

	// Token: 0x04004BFB RID: 19451
	public Texture infectedTexture;

	// Token: 0x04004BFC RID: 19452
	public NetPlayer linePlayer;

	// Token: 0x04004BFD RID: 19453
	public VRRig playerVRRig;

	// Token: 0x04004BFE RID: 19454
	public string playerLevelValue;

	// Token: 0x04004BFF RID: 19455
	public string playerMMRValue;

	// Token: 0x04004C00 RID: 19456
	public string playerNameValue;

	// Token: 0x04004C01 RID: 19457
	public string playerNameVisible;

	// Token: 0x04004C02 RID: 19458
	public int playerActorNumber;

	// Token: 0x04004C03 RID: 19459
	public GorillaPlayerLineButton muteButton;

	// Token: 0x04004C04 RID: 19460
	public GorillaPlayerLineButton reportButton;

	// Token: 0x04004C05 RID: 19461
	public GameObject hateSpeechButton;

	// Token: 0x04004C06 RID: 19462
	public GameObject toxicityButton;

	// Token: 0x04004C07 RID: 19463
	public GameObject cheatingButton;

	// Token: 0x04004C08 RID: 19464
	public GameObject cancelButton;

	// Token: 0x04004C09 RID: 19465
	public SpriteRenderer speakerIcon;

	// Token: 0x04004C0A RID: 19466
	public bool canPressNextReportButton = true;

	// Token: 0x04004C0B RID: 19467
	public Text[] texts;

	// Token: 0x04004C0C RID: 19468
	public SpriteRenderer[] sprites;

	// Token: 0x04004C0D RID: 19469
	public MeshRenderer[] meshes;

	// Token: 0x04004C0E RID: 19470
	public Image[] images;

	// Token: 0x04004C0F RID: 19471
	private Recorder myRecorder;

	// Token: 0x04004C10 RID: 19472
	private bool isMuteManual;

	// Token: 0x04004C11 RID: 19473
	private int mute;

	// Token: 0x04004C12 RID: 19474
	private int emptyRigCount;

	// Token: 0x04004C13 RID: 19475
	public GameObject myRig;

	// Token: 0x04004C14 RID: 19476
	public bool reportedCheating;

	// Token: 0x04004C15 RID: 19477
	public bool reportedToxicity;

	// Token: 0x04004C16 RID: 19478
	public bool reportedHateSpeech;

	// Token: 0x04004C17 RID: 19479
	public bool reportInProgress;

	// Token: 0x04004C18 RID: 19480
	private string currentNickname;

	// Token: 0x04004C19 RID: 19481
	public bool doneReporting;

	// Token: 0x04004C1A RID: 19482
	public bool lastVisible = true;

	// Token: 0x04004C1B RID: 19483
	public GorillaScoreBoard parentScoreboard;

	// Token: 0x04004C1C RID: 19484
	public float initTime;

	// Token: 0x04004C1D RID: 19485
	public float emptyRigCooldown = 10f;

	// Token: 0x04004C1E RID: 19486
	internal RigContainer rigContainer;
}
