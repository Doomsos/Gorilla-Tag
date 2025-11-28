using System;
using UnityEngine;

// Token: 0x02000C29 RID: 3113
public abstract class BasePageHandler : MonoBehaviour
{
	// Token: 0x17000729 RID: 1833
	// (get) Token: 0x06004C80 RID: 19584 RVA: 0x0018D7C0 File Offset: 0x0018B9C0
	// (set) Token: 0x06004C81 RID: 19585 RVA: 0x0018D7C8 File Offset: 0x0018B9C8
	private protected int selectedIndex { protected get; private set; }

	// Token: 0x1700072A RID: 1834
	// (get) Token: 0x06004C82 RID: 19586 RVA: 0x0018D7D1 File Offset: 0x0018B9D1
	// (set) Token: 0x06004C83 RID: 19587 RVA: 0x0018D7D9 File Offset: 0x0018B9D9
	private protected int currentPage { protected get; private set; }

	// Token: 0x1700072B RID: 1835
	// (get) Token: 0x06004C84 RID: 19588 RVA: 0x0018D7E2 File Offset: 0x0018B9E2
	// (set) Token: 0x06004C85 RID: 19589 RVA: 0x0018D7EA File Offset: 0x0018B9EA
	private protected int pages { protected get; private set; }

	// Token: 0x1700072C RID: 1836
	// (get) Token: 0x06004C86 RID: 19590 RVA: 0x0018D7F3 File Offset: 0x0018B9F3
	// (set) Token: 0x06004C87 RID: 19591 RVA: 0x0018D7FB File Offset: 0x0018B9FB
	private protected int maxEntires { protected get; private set; }

	// Token: 0x1700072D RID: 1837
	// (get) Token: 0x06004C88 RID: 19592
	protected abstract int pageSize { get; }

	// Token: 0x1700072E RID: 1838
	// (get) Token: 0x06004C89 RID: 19593
	protected abstract int entriesCount { get; }

	// Token: 0x06004C8A RID: 19594 RVA: 0x0018D804 File Offset: 0x0018BA04
	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	// Token: 0x06004C8B RID: 19595 RVA: 0x0018D86C File Offset: 0x0018BA6C
	public void SelectEntryOnPage(int entryIndex)
	{
		int num = entryIndex + this.pageSize * this.currentPage;
		if (num > this.entriesCount)
		{
			return;
		}
		this.selectedIndex = num;
		this.PageEntrySelected(entryIndex, this.selectedIndex);
	}

	// Token: 0x06004C8C RID: 19596 RVA: 0x0018D8A8 File Offset: 0x0018BAA8
	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int pageEntry = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(pageEntry, index);
		this.SetPage(this.currentPage);
	}

	// Token: 0x06004C8D RID: 19597 RVA: 0x0018D8F4 File Offset: 0x0018BAF4
	public void ChangePage(bool left)
	{
		int num = left ? -1 : 1;
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	// Token: 0x06004C8E RID: 19598 RVA: 0x0018D924 File Offset: 0x0018BB24
	public void SetPage(int page)
	{
		if (page > this.pages)
		{
			return;
		}
		this.currentPage = page;
		int num = this.pageSize * page;
		this.ShowPage(this.currentPage, num, Mathf.Min(num + this.pageSize, this.entriesCount));
	}

	// Token: 0x06004C8F RID: 19599
	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	// Token: 0x06004C90 RID: 19600
	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);
}
