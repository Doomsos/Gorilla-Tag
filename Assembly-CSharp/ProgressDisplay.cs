using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E7 RID: 487
public class ProgressDisplay : MonoBehaviour
{
	// Token: 0x06000D5B RID: 3419 RVA: 0x00047391 File Offset: 0x00045591
	private void Reset()
	{
		this.root = base.gameObject;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x0004739F File Offset: 0x0004559F
	public void SetVisible(bool visible)
	{
		this.root.SetActive(visible);
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000473B0 File Offset: 0x000455B0
	public void SetProgress(int progress, int total)
	{
		if (this.text)
		{
			if (total < this.largestNumberToShow)
			{
				this.text.text = ((progress >= total) ? string.Format("{0}", total) : string.Format("{0}/{1}", progress, total));
				this.SetTextVisible(true);
			}
			else
			{
				this.SetTextVisible(false);
			}
		}
		this.progressImage.fillAmount = (float)progress / (float)total;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x0004742A File Offset: 0x0004562A
	public void SetProgress(float progress)
	{
		this.progressImage.fillAmount = progress;
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00047438 File Offset: 0x00045638
	private void SetTextVisible(bool visible)
	{
		if (this.text.gameObject.activeSelf == visible)
		{
			return;
		}
		this.text.gameObject.SetActive(visible);
	}

	// Token: 0x0400104F RID: 4175
	[SerializeField]
	private GameObject root;

	// Token: 0x04001050 RID: 4176
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04001051 RID: 4177
	[SerializeField]
	private Image progressImage;

	// Token: 0x04001052 RID: 4178
	[SerializeField]
	private int largestNumberToShow = 99;
}
