using System;
using UnityEngine;

// Token: 0x020005E9 RID: 1513
public class FortuneResults : ScriptableObject
{
	// Token: 0x06002625 RID: 9765 RVA: 0x000CBF28 File Offset: 0x000CA128
	private void OnValidate()
	{
		this.totalChance = 0f;
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			this.totalChance += this.fortuneResults[i].weightedChance;
		}
	}

	// Token: 0x06002626 RID: 9766 RVA: 0x000CBF74 File Offset: 0x000CA174
	public FortuneResults.FortuneResult GetResult()
	{
		float num = Random.Range(0f, this.totalChance);
		int i = 0;
		while (i < this.fortuneResults.Length)
		{
			FortuneResults.FortuneCategory fortuneCategory = this.fortuneResults[i];
			if (num <= fortuneCategory.weightedChance)
			{
				if (fortuneCategory.textResults.Length == 0)
				{
					return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
				}
				int resultIndex = Random.Range(0, fortuneCategory.textResults.Length);
				return new FortuneResults.FortuneResult(fortuneCategory.fortuneType, resultIndex);
			}
			else
			{
				num -= fortuneCategory.weightedChance;
				i++;
			}
		}
		return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x000CBFF8 File Offset: 0x000CA1F8
	public string GetResultText(FortuneResults.FortuneResult result)
	{
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			if (this.fortuneResults[i].fortuneType == result.fortuneType && result.resultIndex >= 0 && result.resultIndex < this.fortuneResults[i].textResults.Length)
			{
				return this.fortuneResults[i].textResults[result.resultIndex];
			}
		}
		return "!! Invalid Fortune !!";
	}

	// Token: 0x040031FC RID: 12796
	[SerializeField]
	private FortuneResults.FortuneCategory[] fortuneResults;

	// Token: 0x040031FD RID: 12797
	[SerializeField]
	private float totalChance;

	// Token: 0x020005EA RID: 1514
	public enum FortuneCategoryType
	{
		// Token: 0x040031FF RID: 12799
		Invalid,
		// Token: 0x04003200 RID: 12800
		Positive,
		// Token: 0x04003201 RID: 12801
		Neutral,
		// Token: 0x04003202 RID: 12802
		Negative,
		// Token: 0x04003203 RID: 12803
		Seasonal
	}

	// Token: 0x020005EB RID: 1515
	[Serializable]
	public struct FortuneCategory
	{
		// Token: 0x04003204 RID: 12804
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04003205 RID: 12805
		public float weightedChance;

		// Token: 0x04003206 RID: 12806
		public string[] textResults;
	}

	// Token: 0x020005EC RID: 1516
	public struct FortuneResult
	{
		// Token: 0x06002629 RID: 9769 RVA: 0x000CC073 File Offset: 0x000CA273
		public FortuneResult(FortuneResults.FortuneCategoryType fortuneType, int resultIndex)
		{
			this.fortuneType = fortuneType;
			this.resultIndex = resultIndex;
		}

		// Token: 0x04003207 RID: 12807
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04003208 RID: 12808
		public int resultIndex;
	}
}
