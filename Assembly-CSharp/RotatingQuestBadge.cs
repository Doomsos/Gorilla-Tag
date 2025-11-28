using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class RotatingQuestBadge : MonoBehaviour, ISpawnable
{
	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000DAB RID: 3499 RVA: 0x000487DB File Offset: 0x000469DB
	// (set) Token: 0x06000DAC RID: 3500 RVA: 0x000487E3 File Offset: 0x000469E3
	public bool IsSpawned { get; set; }

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000DAD RID: 3501 RVA: 0x000487EC File Offset: 0x000469EC
	// (set) Token: 0x06000DAE RID: 3502 RVA: 0x000487F4 File Offset: 0x000469F4
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000DAF RID: 3503 RVA: 0x00048800 File Offset: 0x00046A00
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnQuestScoreChanged += new Action<int>(this.OnProgressScoreChanged);
		this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00048859 File Offset: 0x00046A59
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.SetBadgeLevel(-1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x0004887F File Offset: 0x00046A7F
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnQuestScoreChanged -= new Action<int>(this.OnProgressScoreChanged);
			this.myRig = null;
		}
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x000488B4 File Offset: 0x00046AB4
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x000488C4 File Offset: 0x00046AC4
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnQuestScoreChanged += new Action<int>(this.OnProgressScoreChanged);
			this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
			return true;
		}
		return false;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00048920 File Offset: 0x00046B20
	private void OnProgressScoreChanged(int score)
	{
		score = Mathf.Clamp(score, 0, 99999);
		this.displayField.text = score.ToString();
		this.UpdateBadge(score);
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0004894C File Offset: 0x00046B4C
	private void UpdateBadge(int score)
	{
		int num = -1;
		int badgeLevel = -1;
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			if (this.badgeLevels[i].requiredPoints <= score && this.badgeLevels[i].requiredPoints > num)
			{
				num = this.badgeLevels[i].requiredPoints;
				badgeLevel = i;
			}
		}
		this.SetBadgeLevel(badgeLevel);
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x000489B4 File Offset: 0x00046BB4
	private void SetBadgeLevel(int level)
	{
		level = Mathf.Clamp(level, 0, this.badgeLevels.Length - 1);
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			this.badgeLevels[i].badge.SetActive(i == level);
		}
	}

	// Token: 0x040010A3 RID: 4259
	[SerializeField]
	private TextMeshPro displayField;

	// Token: 0x040010A4 RID: 4260
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x040010A5 RID: 4261
	[SerializeField]
	private VRRig myRig;

	// Token: 0x040010A6 RID: 4262
	[SerializeField]
	private RotatingQuestBadge.BadgeLevel[] badgeLevels;

	// Token: 0x020001F6 RID: 502
	[Serializable]
	public struct BadgeLevel
	{
		// Token: 0x040010A9 RID: 4265
		public GameObject badge;

		// Token: 0x040010AA RID: 4266
		public int requiredPoints;
	}
}
