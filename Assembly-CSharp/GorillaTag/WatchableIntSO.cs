using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FEE RID: 4078
	[CreateAssetMenu(fileName = "WatchableIntSO", menuName = "ScriptableObjects/WatchableIntSO")]
	public class WatchableIntSO : WatchableGenericSO<int>
	{
		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06006716 RID: 26390 RVA: 0x002187DE File Offset: 0x002169DE
		private int currentValue
		{
			get
			{
				return base.Value;
			}
		}
	}
}
