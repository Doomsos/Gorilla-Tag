using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class PlatformerCollectiblesMain : MonoBehaviour
{
	// Token: 0x0600005F RID: 95 RVA: 0x00003698 File Offset: 0x00001898
	public void Start()
	{
		int num = 0;
		while ((float)num < this.CoinGridCount)
		{
			float num2 = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num / (this.CoinGridCount - 1f);
			int num3 = 0;
			while ((float)num3 < this.CoinGridCount)
			{
				float num4 = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num3 / (this.CoinGridCount - 1f);
				Object.Instantiate<GameObject>(this.Coin).transform.position = new Vector3(num2, 0.2f, num4);
				num3++;
			}
			num++;
		}
	}

	// Token: 0x04000052 RID: 82
	public GameObject Coin;

	// Token: 0x04000053 RID: 83
	public float CoinGridCount = 5f;

	// Token: 0x04000054 RID: 84
	public float CoinGridSize = 7f;
}
