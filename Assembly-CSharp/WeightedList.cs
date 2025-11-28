using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class WeightedList<T>
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000367 RID: 871 RVA: 0x000142AA File Offset: 0x000124AA
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000368 RID: 872 RVA: 0x000142B7 File Offset: 0x000124B7
	public List<T> Items
	{
		get
		{
			return this.items;
		}
	}

	// Token: 0x06000369 RID: 873 RVA: 0x000142C0 File Offset: 0x000124C0
	public void Add(T item, float weight)
	{
		if (weight <= 0f)
		{
			throw new ArgumentException("Weight must be greater than zero.");
		}
		this.totalWeight += weight;
		this.items.Add(item);
		this.weights.Add(weight);
		this.cumulativeWeights.Add(this.totalWeight);
	}

	// Token: 0x1700003D RID: 61
	[TupleElementNames(new string[]
	{
		"Item",
		"Weight"
	})]
	public ValueTuple<T, float> this[int index]
	{
		[return: TupleElementNames(new string[]
		{
			"Item",
			"Weight"
		})]
		get
		{
			if (index < 0 || index >= this.items.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return new ValueTuple<T, float>(this.items[index], this.weights[index]);
		}
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0001434E File Offset: 0x0001254E
	public T GetRandomItem()
	{
		return this.items[this.GetRandomIndex()];
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00014364 File Offset: 0x00012564
	public int GetRandomIndex()
	{
		if (this.items.Count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}
		float num = Random.value * this.totalWeight;
		int num2 = this.cumulativeWeights.BinarySearch(num);
		if (num2 < 0)
		{
			num2 = ~num2;
		}
		return num2;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x000143AC File Offset: 0x000125AC
	public bool Remove(T item)
	{
		int num = this.items.IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		this.RemoveAt(num);
		return true;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x000143D4 File Offset: 0x000125D4
	public void RemoveAt(int index)
	{
		if (index < 0 || index >= this.items.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		this.totalWeight -= this.weights[index];
		this.items.RemoveAt(index);
		this.weights.RemoveAt(index);
		this.RecalculateCumulativeWeights();
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00014438 File Offset: 0x00012638
	private void RecalculateCumulativeWeights()
	{
		this.cumulativeWeights.Clear();
		float num = 0f;
		foreach (float num2 in this.weights)
		{
			num += num2;
			this.cumulativeWeights.Add(num);
		}
		this.totalWeight = num;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000144AC File Offset: 0x000126AC
	public void Clear()
	{
		this.items.Clear();
		this.weights.Clear();
		this.cumulativeWeights.Clear();
		this.totalWeight = 0f;
	}

	// Token: 0x040003FC RID: 1020
	private List<T> items = new List<T>();

	// Token: 0x040003FD RID: 1021
	private List<float> weights = new List<float>();

	// Token: 0x040003FE RID: 1022
	private List<float> cumulativeWeights = new List<float>();

	// Token: 0x040003FF RID: 1023
	private float totalWeight;
}
