using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020005B2 RID: 1458
public class CheatUpdate : MonoBehaviour
{
	// Token: 0x060024BE RID: 9406 RVA: 0x000C6315 File Offset: 0x000C4515
	private void Start()
	{
		base.StartCoroutine(this.UpdateNumberOfPlayers());
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000C6324 File Offset: 0x000C4524
	public IEnumerator UpdateNumberOfPlayers()
	{
		for (;;)
		{
			base.StartCoroutine(this.UpdatePlayerCount());
			yield return new WaitForSeconds(10f);
		}
		yield break;
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000C6333 File Offset: 0x000C4533
	private IEnumerator UpdatePlayerCount()
	{
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("player_count", PhotonNetwork.CountOfPlayers - 1);
		wwwform.AddField("game_version", "live");
		wwwform.AddField("game_name", Application.productName);
		Debug.Log(PhotonNetwork.CountOfPlayers - 1);
		using (UnityWebRequest www = UnityWebRequest.Post("http://ntsfranz.crabdance.com/update_monke_count", wwwform))
		{
			yield return www.SendWebRequest();
			if (www.result == 2 || www.result == 3)
			{
				Debug.Log(www.error);
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}
}
