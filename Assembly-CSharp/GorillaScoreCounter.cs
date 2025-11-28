using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005D3 RID: 1491
public class GorillaScoreCounter : MonoBehaviour
{
	// Token: 0x060025AE RID: 9646 RVA: 0x000C9648 File Offset: 0x000C7848
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
		if (this.isRedTeam)
		{
			this.attribute = "redScore";
			return;
		}
		this.attribute = "blueScore";
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000C967C File Offset: 0x000C787C
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties[this.attribute] != null)
		{
			this.text.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties[this.attribute]).ToString();
		}
	}

	// Token: 0x04003148 RID: 12616
	public bool isRedTeam;

	// Token: 0x04003149 RID: 12617
	public Text text;

	// Token: 0x0400314A RID: 12618
	public string attribute;
}
