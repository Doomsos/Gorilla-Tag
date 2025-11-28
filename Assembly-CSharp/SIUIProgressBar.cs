using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000153 RID: 339
public class SIUIProgressBar : MonoBehaviour
{
	// Token: 0x060008F1 RID: 2289 RVA: 0x00030674 File Offset: 0x0002E874
	public void UpdateFillPercent(float percentFull)
	{
		float num = this.backgroundImage.rectTransform.sizeDelta.x * (1f - 2f * this.borderPercent / 100f);
		float num2 = num * Mathf.Min(1f, percentFull);
		float num3 = -(num - num2) / 2f * this.progressImage.rectTransform.localScale.x;
		this.progressImage.rectTransform.sizeDelta = new Vector2(num2, this.progressImage.rectTransform.sizeDelta.y);
		this.progressImage.rectTransform.localPosition = new Vector3(num3, this.progressImage.rectTransform.localPosition.y, this.progressImage.rectTransform.localPosition.z);
	}

	// Token: 0x04000B0A RID: 2826
	public Image backgroundImage;

	// Token: 0x04000B0B RID: 2827
	public Image progressImage;

	// Token: 0x04000B0C RID: 2828
	public float borderPercent;

	// Token: 0x04000B0D RID: 2829
	public TextMeshProUGUI progressText;
}
