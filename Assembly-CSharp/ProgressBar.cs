using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085D RID: 2141
public class ProgressBar : MonoBehaviour
{
	// Token: 0x06003862 RID: 14434 RVA: 0x0012D840 File Offset: 0x0012BA40
	public void UpdateProgress(float newFill)
	{
		bool flag = newFill > 1f;
		this._fillAmount = Mathf.Clamp(newFill, 0f, 1f);
		this.fillImage.fillAmount = this._fillAmount;
		if (this.useColors)
		{
			if (flag)
			{
				this.fillImage.color = this.overCapacity;
				return;
			}
			if (Mathf.Approximately(this._fillAmount, 1f))
			{
				this.fillImage.color = this.atCapacity;
				return;
			}
			this.fillImage.color = this.underCapacity;
		}
	}

	// Token: 0x0400476B RID: 18283
	[SerializeField]
	private Image fillImage;

	// Token: 0x0400476C RID: 18284
	[SerializeField]
	private bool useColors;

	// Token: 0x0400476D RID: 18285
	[SerializeField]
	private Color underCapacity = Color.green;

	// Token: 0x0400476E RID: 18286
	[SerializeField]
	private Color overCapacity = Color.red;

	// Token: 0x0400476F RID: 18287
	[SerializeField]
	private Color atCapacity = Color.yellow;

	// Token: 0x04004770 RID: 18288
	private float _fillAmount;
}
