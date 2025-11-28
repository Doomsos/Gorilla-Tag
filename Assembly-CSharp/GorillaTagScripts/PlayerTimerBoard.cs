using System;
using System.Collections.Generic;
using System.Text;
using KID.Model;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DFB RID: 3579
	public class PlayerTimerBoard : MonoBehaviour
	{
		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x06005956 RID: 22870 RVA: 0x001C930D File Offset: 0x001C750D
		// (set) Token: 0x06005957 RID: 22871 RVA: 0x001C9315 File Offset: 0x001C7515
		public bool IsDirty { get; set; } = true;

		// Token: 0x06005958 RID: 22872 RVA: 0x001C931E File Offset: 0x001C751E
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x06005959 RID: 22873 RVA: 0x001C9326 File Offset: 0x001C7526
		private void OnEnable()
		{
			this.TryInit();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.RedrawPlayerLines));
		}

		// Token: 0x0600595A RID: 22874 RVA: 0x001C933F File Offset: 0x001C753F
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.RegisterTimerBoard(this);
			this.isInitialized = true;
		}

		// Token: 0x0600595B RID: 22875 RVA: 0x001C936A File Offset: 0x001C756A
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.UnregisterTimerBoard(this);
			}
			this.isInitialized = false;
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.RedrawPlayerLines));
		}

		// Token: 0x0600595C RID: 22876 RVA: 0x001C939C File Offset: 0x001C759C
		public void SetSleepState(bool awake)
		{
			this.playerColumn.enabled = awake;
			this.timeColumn.enabled = awake;
			if (this.linesParent != null)
			{
				this.linesParent.SetActive(awake);
			}
		}

		// Token: 0x0600595D RID: 22877 RVA: 0x001C93D0 File Offset: 0x001C75D0
		public void SortLines()
		{
			this.lines.Sort(new Comparison<PlayerTimerBoardLine>(PlayerTimerBoardLine.CompareByTotalTime));
		}

		// Token: 0x0600595E RID: 22878 RVA: 0x001C93EC File Offset: 0x001C75EC
		public void RedrawPlayerLines()
		{
			this.stringBuilder.Clear();
			this.stringBuilderTime.Clear();
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER", out text, "<b><color=yellow>PLAYER</color></b>"))
			{
				Debug.LogError("[LOCALIZATION::MONKE_BLOCKS::TIMER] Failed to get key for Game Mode [MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER]");
			}
			this.stringBuilder.Append("<b><color=yellow>");
			this.stringBuilder.Append(text);
			this.stringBuilder.Append("</color></b>");
			if (!LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES", out text, "<b><color=yellow>LATEST TIME</color></b>"))
			{
				Debug.LogError("[LOCALIZATION::MONKE_BLOCKS::TIMER] Failed to get key for Game Mode [MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES]");
			}
			this.stringBuilderTime.Append("<b><color=yellow>");
			this.stringBuilderTime.Append(text);
			this.stringBuilderTime.Append("</color></b>");
			this.SortLines();
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == 1) && permissionDataByFeature.ManagedBy != 3;
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
							this.stringBuilderTime.Append("\n ");
							this.stringBuilderTime.Append(this.lines[i].playerTimeStr);
						}
					}
				}
				catch
				{
				}
			}
			this.playerColumn.text = this.stringBuilder.ToString();
			this.timeColumn.text = this.stringBuilderTime.ToString();
			this.IsDirty = false;
		}

		// Token: 0x0400667F RID: 26239
		[SerializeField]
		private GameObject linesParent;

		// Token: 0x04006680 RID: 26240
		public List<PlayerTimerBoardLine> lines;

		// Token: 0x04006681 RID: 26241
		public TextMeshPro notInRoomText;

		// Token: 0x04006682 RID: 26242
		public TextMeshPro playerColumn;

		// Token: 0x04006683 RID: 26243
		public TextMeshPro timeColumn;

		// Token: 0x04006684 RID: 26244
		[SerializeField]
		private int startingYValue;

		// Token: 0x04006685 RID: 26245
		[SerializeField]
		private int lineHeight;

		// Token: 0x04006686 RID: 26246
		private StringBuilder stringBuilder = new StringBuilder(220);

		// Token: 0x04006687 RID: 26247
		private StringBuilder stringBuilderTime = new StringBuilder(220);

		// Token: 0x04006688 RID: 26248
		private const string MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER_KEY = "MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER";

		// Token: 0x04006689 RID: 26249
		private const string MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES_KEY = "MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES";

		// Token: 0x0400668A RID: 26250
		private bool isInitialized;
	}
}
