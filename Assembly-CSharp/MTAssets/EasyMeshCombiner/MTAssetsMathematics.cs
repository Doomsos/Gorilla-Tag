using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000F5C RID: 3932
	[AddComponentMenu("")]
	public class MTAssetsMathematics : MonoBehaviour
	{
		// Token: 0x06006283 RID: 25219 RVA: 0x001FB53C File Offset: 0x001F973C
		public static List<T> RandomizeThisList<T>(List<T> list)
		{
			int count = list.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int num2 = Random.Range(i, count);
				T t = list[i];
				list[i] = list[num2];
				list[num2] = t;
			}
			return list;
		}

		// Token: 0x06006284 RID: 25220 RVA: 0x001FB589 File Offset: 0x001F9789
		public static Vector3 GetHalfPositionBetweenTwoPoints(Vector3 pointA, Vector3 pointB)
		{
			return Vector3.Lerp(pointA, pointB, 0.5f);
		}
	}
}
