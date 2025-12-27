using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaExtensions
{
	public static class ListExtensions
	{
		public static TCol ShuffleIntoCollection<TCol, TVal>(this List<TVal> list) where TCol : ICollection<TVal>, new()
		{
			List<TVal> list2 = new List<TVal>(list);
			TCol result = Activator.CreateInstance<TCol>();
			int i = list2.Count;
			while (i > 1)
			{
				i--;
				int num = Random.Range(0, i);
				List<TVal> list3 = list2;
				int num2 = i;
				List<TVal> list4 = list2;
				int num3 = num;
				TVal tval = list2[num];
				TVal tval2 = list2[i];
				list3[num2] = tval;
				list4[num3] = tval2;
			}
			foreach (TVal tval3 in list2)
			{
				result.Add(tval3);
			}
			return result;
		}
	}
}
