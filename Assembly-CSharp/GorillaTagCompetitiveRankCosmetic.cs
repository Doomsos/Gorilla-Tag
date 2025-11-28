using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020007BA RID: 1978
public class GorillaTagCompetitiveRankCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06003418 RID: 13336 RVA: 0x001181E2 File Offset: 0x001163E2
	// (set) Token: 0x06003419 RID: 13337 RVA: 0x001181EA File Offset: 0x001163EA
	public bool IsSpawned { get; set; }

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x0600341A RID: 13338 RVA: 0x001181F3 File Offset: 0x001163F3
	// (set) Token: 0x0600341B RID: 13339 RVA: 0x001181FB File Offset: 0x001163FB
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x0600341C RID: 13340 RVA: 0x00118204 File Offset: 0x00116404
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnRankedSubtierChanged += new Action<int, int>(this.OnRankedScoreChanged);
		this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
	}

	// Token: 0x0600341D RID: 13341 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x0600341E RID: 13342 RVA: 0x0011826A File Offset: 0x0011646A
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.UpdateDisplayedCosmetic(-1, -1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x0600341F RID: 13343 RVA: 0x00118291 File Offset: 0x00116491
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnRankedSubtierChanged -= new Action<int, int>(this.OnRankedScoreChanged);
			this.myRig = null;
		}
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x001182C6 File Offset: 0x001164C6
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x001182D8 File Offset: 0x001164D8
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnRankedSubtierChanged += new Action<int, int>(this.OnRankedScoreChanged);
			this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
			return true;
		}
		return false;
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x00118341 File Offset: 0x00116541
	private void OnRankedScoreChanged(int questRank, int pcRank)
	{
		this.UpdateDisplayedCosmetic(questRank, pcRank);
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x0011834C File Offset: 0x0011654C
	private void UpdateDisplayedCosmetic(int questRank, int pcRank)
	{
		if (this.rankCosmetics == null)
		{
			return;
		}
		int num = this.usePCELO ? pcRank : questRank;
		if (num <= 0)
		{
			num = 0;
		}
		for (int i = 0; i < this.rankCosmetics.Length; i++)
		{
			this.rankCosmetics[i].SetActive(i == num);
		}
	}

	// Token: 0x04004267 RID: 16999
	[Tooltip("If enabled, display PC rank. Otherwise, display Quest rank")]
	[SerializeField]
	private bool usePCELO;

	// Token: 0x04004268 RID: 17000
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x04004269 RID: 17001
	[SerializeField]
	private VRRig myRig;

	// Token: 0x0400426A RID: 17002
	[SerializeField]
	private GameObject[] rankCosmetics;
}
