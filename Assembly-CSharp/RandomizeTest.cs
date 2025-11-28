using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008D8 RID: 2264
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06003A0F RID: 14863 RVA: 0x00133324 File Offset: 0x00131524
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			this.testList.Add(i);
		}
		for (int j = 0; j < 10; j++)
		{
			this.testListArray[j] = 0;
		}
		for (int k = 0; k < this.testList.Count; k++)
		{
			this.testListArray[k] = this.testList[k];
		}
		this.RandomizeList(ref this.testList);
		for (int l = 0; l < 10; l++)
		{
			this.testListArray[l] = 0;
		}
		for (int m = 0; m < this.testList.Count; m++)
		{
			this.testListArray[m] = this.testList[m];
		}
	}

	// Token: 0x06003A10 RID: 14864 RVA: 0x001333DC File Offset: 0x001315DC
	public void RandomizeList(ref List<int> listToRandomize)
	{
		this.randomIterator = 0;
		while (this.randomIterator < listToRandomize.Count)
		{
			this.tempRandIndex = Random.Range(this.randomIterator, listToRandomize.Count);
			this.tempRandValue = listToRandomize[this.randomIterator];
			listToRandomize[this.randomIterator] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandValue;
			this.randomIterator++;
		}
	}

	// Token: 0x04004950 RID: 18768
	public List<int> testList = new List<int>();

	// Token: 0x04004951 RID: 18769
	public int[] testListArray = new int[10];

	// Token: 0x04004952 RID: 18770
	public int randomIterator;

	// Token: 0x04004953 RID: 18771
	public int tempRandIndex;

	// Token: 0x04004954 RID: 18772
	public int tempRandValue;
}
