using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using TMPro;
using UnityEngine;

// Token: 0x020007B0 RID: 1968
public class GorillaScoreBoard : MonoBehaviour
{
	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x060033A8 RID: 13224 RVA: 0x00116438 File Offset: 0x00114638
	// (set) Token: 0x060033A9 RID: 13225 RVA: 0x0011644F File Offset: 0x0011464F
	public bool IsDirty
	{
		get
		{
			return this._isDirty || string.IsNullOrEmpty(this.initialGameMode);
		}
		set
		{
			this._isDirty = value;
		}
	}

	// Token: 0x060033AA RID: 13226 RVA: 0x00116458 File Offset: 0x00114658
	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		if (this.linesParent != null)
		{
			this.linesParent.SetActive(awake);
		}
	}

	// Token: 0x060033AB RID: 13227 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDestroy()
	{
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x0011648C File Offset: 0x0011468C
	public string GetBeginningString()
	{
		return "ROOM ID: " + (NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME: " : (NetworkSystem.Instance.RoomName + "   GAME: ")) + this.RoomType() + "\n  PLAYER     COLOR  MUTE   REPORT";
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x001164CC File Offset: 0x001146CC
	public string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		for (int i = 0; i < count; i++)
		{
			this.tempGmName = this.gmNames[i];
			if (this.initialGameMode.Contains(this.tempGmName))
			{
				this.gmName = this.tempGmName;
				break;
			}
		}
		return this.gmName;
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x0011654C File Offset: 0x0011474C
	public void RedrawPlayerLines()
	{
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		for (int i = 0; i < this.lines.Count; i++)
		{
			try
			{
				if (this.lines[i].gameObject.activeInHierarchy)
				{
					this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
					if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
					{
						this.stringBuilder.Append("\n ");
						this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
						if (this.lines[i].linePlayer != NetworkSystem.Instance.LocalPlayer)
						{
							if (this.lines[i].reportButton.isActiveAndEnabled)
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT\n");
							}
							else
							{
								this.buttonStringBuilder.Append("MUTE                HATE SPEECH    TOXICITY     CHEATING       CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("\n");
						}
					}
				}
			}
			catch
			{
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
		this._isDirty = false;
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x00116738 File Offset: 0x00114938
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text.Substring(0, 10);
			}
			text = text.ToUpper();
		}
		return text;
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x00116797 File Offset: 0x00114997
	private void Start()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x0011679F File Offset: 0x0011499F
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this._isDirty = true;
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x001167AE File Offset: 0x001149AE
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
	}

	// Token: 0x04004212 RID: 16914
	public GameObject scoreBoardLinePrefab;

	// Token: 0x04004213 RID: 16915
	public int startingYValue;

	// Token: 0x04004214 RID: 16916
	public int lineHeight;

	// Token: 0x04004215 RID: 16917
	public bool includeMMR;

	// Token: 0x04004216 RID: 16918
	public bool isActive;

	// Token: 0x04004217 RID: 16919
	public GameObject linesParent;

	// Token: 0x04004218 RID: 16920
	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	// Token: 0x04004219 RID: 16921
	public TextMeshPro boardText;

	// Token: 0x0400421A RID: 16922
	public TextMeshPro buttonText;

	// Token: 0x0400421B RID: 16923
	public bool needsUpdate;

	// Token: 0x0400421C RID: 16924
	public TextMeshPro notInRoomText;

	// Token: 0x0400421D RID: 16925
	public string initialGameMode;

	// Token: 0x0400421E RID: 16926
	private string tempGmName;

	// Token: 0x0400421F RID: 16927
	private string gmName;

	// Token: 0x04004220 RID: 16928
	private const string error = "ERROR";

	// Token: 0x04004221 RID: 16929
	private List<string> gmNames;

	// Token: 0x04004222 RID: 16930
	private bool _isDirty = true;

	// Token: 0x04004223 RID: 16931
	private StringBuilder stringBuilder = new StringBuilder(220);

	// Token: 0x04004224 RID: 16932
	private StringBuilder buttonStringBuilder = new StringBuilder(720);
}
