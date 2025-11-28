using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000707 RID: 1799
[Serializable]
public class GRShuttleUI
{
	// Token: 0x06002E17 RID: 11799 RVA: 0x000FAA3D File Offset: 0x000F8C3D
	public void Setup(GhostReactor reactor, NetPlayer player)
	{
		this.reactor = reactor;
		this.player = player;
		this.RefreshUI();
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x000FAA54 File Offset: 0x000F8C54
	public void RefreshUI()
	{
		if (this.playerName != null)
		{
			this.playerName.text = ((this.player == null) ? null : this.player.SanitizedNickName);
		}
		if (this.playerTitle != null)
		{
			GRPlayer grplayer = (this.player == null) ? null : GRPlayer.Get(this.player.ActorNumber);
			if (grplayer != null)
			{
				this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			}
			else
			{
				this.playerTitle.text = null;
			}
		}
		if (this.shuttle != null)
		{
			int targetFloor = this.shuttle.GetTargetFloor();
			if (this.destFloorText != null)
			{
				if (targetFloor == -1)
				{
					this.destFloorText.text = "HQ";
				}
				else
				{
					this.destFloorText.text = (targetFloor + 1).ToString();
				}
			}
			bool flag = targetFloor <= this.shuttle.GetMaxDropFloor();
			this.validScreen.SetActive(flag);
			this.invalidScreen.SetActive(!flag);
			if (flag)
			{
				this.infoText.text = "READY!\n\nDROP TO LEVEL";
				return;
			}
			this.infoText.text = "UNSAFE!\n\nUPGRADE DROP CHASSIS";
		}
	}

	// Token: 0x04003C3E RID: 15422
	public TMP_Text playerName;

	// Token: 0x04003C3F RID: 15423
	public TMP_Text playerTitle;

	// Token: 0x04003C40 RID: 15424
	public TMP_Text destFloorText;

	// Token: 0x04003C41 RID: 15425
	public TMP_Text infoText;

	// Token: 0x04003C42 RID: 15426
	public GameObject validScreen;

	// Token: 0x04003C43 RID: 15427
	public GameObject invalidScreen;

	// Token: 0x04003C44 RID: 15428
	public GRShuttle shuttle;

	// Token: 0x04003C45 RID: 15429
	private NetPlayer player;

	// Token: 0x04003C46 RID: 15430
	private GhostReactor reactor;
}
