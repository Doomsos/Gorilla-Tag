using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005D2 RID: 1490
public class GorillaPlayerCounter : MonoBehaviour
{
	// Token: 0x060025AB RID: 9643 RVA: 0x000C9580 File Offset: 0x000C7780
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x000C9594 File Offset: 0x000C7794
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			int num = 0;
			foreach (KeyValuePair<int, Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
			{
				if ((bool)keyValuePair.Value.CustomProperties["isRedTeam"] == this.isRedTeam)
				{
					num++;
				}
			}
			this.text.text = num.ToString();
		}
	}

	// Token: 0x04003145 RID: 12613
	public bool isRedTeam;

	// Token: 0x04003146 RID: 12614
	public Text text;

	// Token: 0x04003147 RID: 12615
	public string attribute;
}
