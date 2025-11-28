using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class PartyGameModeWarning : MonoBehaviour
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x0600120A RID: 4618 RVA: 0x0005F12C File Offset: 0x0005D32C
	public bool ShouldShowWarning
	{
		get
		{
			return FriendshipGroupDetection.Instance.IsInParty && FriendshipGroupDetection.Instance.AnyPartyMembersOutsideFriendCollider();
		}
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x0005F148 File Offset: 0x0005D348
	private void Awake()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0005F173 File Offset: 0x0005D373
	public void Show()
	{
		this.visibleUntilTimestamp = Time.time + this.visibleDuration;
		if (this.hideCoroutine == null)
		{
			this.hideCoroutine = base.StartCoroutine(this.HideCo());
		}
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0005F1A1 File Offset: 0x0005D3A1
	private IEnumerator HideCo()
	{
		GameObject[] array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		float lastVisible;
		do
		{
			lastVisible = this.visibleUntilTimestamp;
			yield return new WaitForSeconds(this.visibleUntilTimestamp - Time.time);
		}
		while (lastVisible != this.visibleUntilTimestamp);
		array = this.showParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		array = this.hideParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		this.hideCoroutine = null;
		yield break;
	}

	// Token: 0x040016AE RID: 5806
	[SerializeField]
	private GameObject[] showParts;

	// Token: 0x040016AF RID: 5807
	[SerializeField]
	private GameObject[] hideParts;

	// Token: 0x040016B0 RID: 5808
	[SerializeField]
	private float visibleDuration;

	// Token: 0x040016B1 RID: 5809
	private float visibleUntilTimestamp;

	// Token: 0x040016B2 RID: 5810
	private Coroutine hideCoroutine;
}
