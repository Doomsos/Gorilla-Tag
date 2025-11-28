using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DFC RID: 3580
	public class PlayerTimerBoardLine : MonoBehaviour
	{
		// Token: 0x06005960 RID: 22880 RVA: 0x001C9687 File Offset: 0x001C7887
		public void ResetData()
		{
			this.linePlayer = null;
			this.currentNickname = string.Empty;
			this.playerTimeStr = string.Empty;
			this.playerTimeSeconds = 0f;
		}

		// Token: 0x06005961 RID: 22881 RVA: 0x001C96B4 File Offset: 0x001C78B4
		public void SetLineData(NetPlayer netPlayer)
		{
			if (!netPlayer.InRoom || netPlayer == this.linePlayer)
			{
				return;
			}
			this.linePlayer = netPlayer;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.rigContainer = rigContainer;
				this.playerVRRig = rigContainer.Rig;
			}
			this.InitializeLine();
		}

		// Token: 0x06005962 RID: 22882 RVA: 0x001C9702 File Offset: 0x001C7902
		public void InitializeLine()
		{
			this.currentNickname = string.Empty;
			this.UpdatePlayerText();
			this.UpdateTimeText();
		}

		// Token: 0x06005963 RID: 22883 RVA: 0x001C971C File Offset: 0x001C791C
		public void UpdateLine()
		{
			if (this.linePlayer != null)
			{
				if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
				{
					this.UpdatePlayerText();
					this.parentBoard.IsDirty = true;
				}
				string text = this.playerTimeStr;
				this.UpdateTimeText();
				if (!this.playerTimeStr.Equals(text))
				{
					this.parentBoard.IsDirty = true;
				}
			}
		}

		// Token: 0x06005964 RID: 22884 RVA: 0x001C9784 File Offset: 0x001C7984
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
			}
			catch (Exception)
			{
				this.playerNameVisible = this.linePlayer.DefaultName;
				GorillaNot.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}

		// Token: 0x06005965 RID: 22885 RVA: 0x001C98B8 File Offset: 0x001C7AB8
		private void UpdateTimeText()
		{
			if (this.linePlayer == null || !(PlayerTimerManager.instance != null))
			{
				this.playerTimeStr = "--:--:--";
				return;
			}
			this.playerTimeSeconds = PlayerTimerManager.instance.GetLastDurationForPlayer(this.linePlayer.ActorNumber);
			if (this.playerTimeSeconds > 0f)
			{
				this.playerTimeStr = TimeSpan.FromSeconds((double)this.playerTimeSeconds).ToString("mm\\:ss\\:ff");
				return;
			}
			this.playerTimeStr = "--:--:--";
		}

		// Token: 0x06005966 RID: 22886 RVA: 0x001C993C File Offset: 0x001C7B3C
		public string NormalizeName(bool doIt, string text)
		{
			if (doIt)
			{
				if (GorillaComputer.instance.CheckAutoBanListForName(text))
				{
					text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
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

		// Token: 0x06005967 RID: 22887 RVA: 0x001C99E0 File Offset: 0x001C7BE0
		public static int CompareByTotalTime(PlayerTimerBoardLine lineA, PlayerTimerBoardLine lineB)
		{
			if (lineA.playerTimeSeconds > 0f && lineB.playerTimeSeconds > 0f)
			{
				return lineA.playerTimeSeconds.CompareTo(lineB.playerTimeSeconds);
			}
			if (lineA.playerTimeSeconds <= 0f)
			{
				return 1;
			}
			if (lineB.playerTimeSeconds <= 0f)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x0400668C RID: 26252
		public string playerNameVisible;

		// Token: 0x0400668D RID: 26253
		public string playerTimeStr;

		// Token: 0x0400668E RID: 26254
		private float playerTimeSeconds;

		// Token: 0x0400668F RID: 26255
		public NetPlayer linePlayer;

		// Token: 0x04006690 RID: 26256
		public VRRig playerVRRig;

		// Token: 0x04006691 RID: 26257
		public PlayerTimerBoard parentBoard;

		// Token: 0x04006692 RID: 26258
		internal RigContainer rigContainer;

		// Token: 0x04006693 RID: 26259
		private string currentNickname;
	}
}
