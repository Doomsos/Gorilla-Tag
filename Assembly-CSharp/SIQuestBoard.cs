using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class SIQuestBoard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060007EB RID: 2027 RVA: 0x0002B4BC File Offset: 0x000296BC
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			stream.SendNext(this.questDisplays[i].activePlayerActorNumber);
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0002B4FC File Offset: 0x000296FC
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			this.questDisplays[i].activePlayerActorNumber = (int)stream.ReceiveNext();
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0002B53B File Offset: 0x0002973B
	public void GrantBonusPointProgress()
	{
		if (!this.bounds.Contains(GTPlayer.Instance.HeadCenterPosition))
		{
			return;
		}
		SIPlayer.LocalPlayer.GetBonusProgress(this.superInfection.siManager);
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0002B56C File Offset: 0x0002976C
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.superInfection.siManager.gameEntityManager.IsAuthority())
		{
			this.AuthorityUpdateScreenAssignments();
		}
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = utcNow.Date + SIProgression.Instance.CROSSOVER_TIME_OF_DAY;
		if (dateTime < utcNow)
		{
			dateTime = dateTime.AddDays(1.0);
		}
		TimeSpan timeSpan = dateTime - utcNow;
		if (this.lastHours != timeSpan.Hours || this.lastMinutes != timeSpan.Minutes || this.lastSeconds != timeSpan.Seconds)
		{
			this.timeToNewQuests.text = "NEW QUESTS IN: " + timeSpan.ToString("hh\\:mm\\:ss");
		}
		this.lastHours = timeSpan.Hours;
		this.lastMinutes = timeSpan.Minutes;
		this.lastSeconds = timeSpan.Seconds;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0002B64C File Offset: 0x0002984C
	private void AuthorityUpdateScreenAssignments()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			list.Add(allNetPlayers[i].ActorNumber);
		}
		for (int j = 0; j < this.questDisplays.Count; j++)
		{
			int activePlayerActorNumber = this.questDisplays[j].activePlayerActorNumber;
			if (activePlayerActorNumber != -1)
			{
				if (!list.Contains(activePlayerActorNumber))
				{
					this.questDisplays[j].activePlayerActorNumber = -1;
				}
				else if (!list2.Contains(activePlayerActorNumber))
				{
					list2.Add(activePlayerActorNumber);
				}
			}
		}
		for (int k = 0; k < allNetPlayers.Length; k++)
		{
			int actorNumber = allNetPlayers[k].ActorNumber;
			if (!list2.Contains(actorNumber))
			{
				for (int l = 0; l < this.questDisplays.Count; l++)
				{
					if (this.questDisplays[l].activePlayerActorNumber == -1)
					{
						this.questDisplays[l].activePlayerActorNumber = actorNumber;
						break;
					}
				}
			}
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0002B75C File Offset: 0x0002995C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.bonusPointArea.gameObject.activeSelf)
		{
			this.bounds = this.bonusPointArea.bounds;
			this.bonusPointArea.gameObject.SetActive(false);
		}
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00002789 File Offset: 0x00000989
	public void ForceCompleteQuest(int index)
	{
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x00002789 File Offset: 0x00000989
	public void CheatAddPoints(int points)
	{
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00002789 File Offset: 0x00000989
	public void CheatAddBonusPoints(int points)
	{
	}

	// Token: 0x040009CC RID: 2508
	public SuperInfection superInfection;

	// Token: 0x040009CD RID: 2509
	public List<SIUIPlayerQuestDisplay> questDisplays;

	// Token: 0x040009CE RID: 2510
	public BoxCollider bonusPointArea;

	// Token: 0x040009CF RID: 2511
	public Bounds bounds;

	// Token: 0x040009D0 RID: 2512
	public ParticleSystem celebrateParticle;

	// Token: 0x040009D1 RID: 2513
	public TextMeshProUGUI timeToNewQuests;

	// Token: 0x040009D2 RID: 2514
	private int lastHours;

	// Token: 0x040009D3 RID: 2515
	private int lastMinutes;

	// Token: 0x040009D4 RID: 2516
	private int lastSeconds;
}
