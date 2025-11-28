using System;
using TMPro;
using UnityEngine;

// Token: 0x020006D6 RID: 1750
public class GRNameDisplayPlate : MonoBehaviour
{
	// Token: 0x06002CD3 RID: 11475 RVA: 0x000F2E90 File Offset: 0x000F1090
	public void RefreshPlayerName(VRRig vrRig)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (vrRig != null && grplayer != null)
		{
			if (!this.namePlateLabel.text.Equals(vrRig.playerNameVisible))
			{
				this.namePlateLabel.text = vrRig.playerNameVisible;
				return;
			}
		}
		else
		{
			this.namePlateLabel.text = "";
		}
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000F2EF0 File Offset: 0x000F10F0
	public void Clear()
	{
		this.namePlateLabel.text = "";
	}

	// Token: 0x04003A3C RID: 14908
	public TMP_Text namePlateLabel;
}
