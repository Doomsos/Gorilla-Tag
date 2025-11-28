using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200091F RID: 2335
public class GorillaScoreboardSpawner : MonoBehaviour
{
	// Token: 0x06003BAD RID: 15277 RVA: 0x0013B6A7 File Offset: 0x001398A7
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x06003BAE RID: 15278 RVA: 0x0013B6B6 File Offset: 0x001398B6
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06003BAF RID: 15279 RVA: 0x0013B6EE File Offset: 0x001398EE
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x06003BB0 RID: 15280 RVA: 0x0013B6FC File Offset: 0x001398FC
	public void OnJoinedRoom()
	{
		Debug.Log("SCOREBOARD JOIN ROOM");
		if (this.IsCurrentScoreboard())
		{
			this.notInRoomText.SetActive(false);
			this.currentScoreboard = Object.Instantiate<GameObject>(this.scoreboardPrefab, base.transform).GetComponent<GorillaScoreBoard>();
			this.currentScoreboard.transform.rotation = base.transform.rotation;
			if (this.includeMMR)
			{
				this.currentScoreboard.GetComponent<GorillaScoreBoard>().includeMMR = true;
				this.currentScoreboard.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	// Token: 0x06003BB1 RID: 15281 RVA: 0x0013B78C File Offset: 0x0013998C
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GTPlayer.Instance.inOverlay;
	}

	// Token: 0x06003BB2 RID: 15282 RVA: 0x0013B7AC File Offset: 0x001399AC
	private IEnumerator UpdateBoard()
	{
		for (;;)
		{
			try
			{
				if (this.currentScoreboard != null)
				{
					bool flag = this.IsVisible();
					foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
					{
						if (flag != gorillaPlayerScoreboardLine.lastVisible)
						{
							gorillaPlayerScoreboardLine.lastVisible = flag;
						}
					}
					if (this.currentScoreboard.boardText.enabled != flag)
					{
						this.currentScoreboard.boardText.enabled = flag;
					}
					if (this.currentScoreboard.buttonText.enabled != flag)
					{
						this.currentScoreboard.buttonText.enabled = flag;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06003BB3 RID: 15283 RVA: 0x0013B7BB File Offset: 0x001399BB
	public void OnLeftRoom()
	{
		this.Cleanup();
		if (this.notInRoomText)
		{
			this.notInRoomText.SetActive(true);
		}
	}

	// Token: 0x06003BB4 RID: 15284 RVA: 0x0013B7DC File Offset: 0x001399DC
	public void Cleanup()
	{
		if (this.currentScoreboard != null)
		{
			Object.Destroy(this.currentScoreboard.gameObject);
			this.currentScoreboard = null;
		}
	}

	// Token: 0x04004C39 RID: 19513
	public string gameType;

	// Token: 0x04004C3A RID: 19514
	public bool includeMMR;

	// Token: 0x04004C3B RID: 19515
	public GameObject scoreboardPrefab;

	// Token: 0x04004C3C RID: 19516
	public GameObject notInRoomText;

	// Token: 0x04004C3D RID: 19517
	public GameObject controllingParentGameObject;

	// Token: 0x04004C3E RID: 19518
	public bool isActive = true;

	// Token: 0x04004C3F RID: 19519
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x04004C40 RID: 19520
	public bool lastVisible;

	// Token: 0x04004C41 RID: 19521
	public bool forOverlay;
}
