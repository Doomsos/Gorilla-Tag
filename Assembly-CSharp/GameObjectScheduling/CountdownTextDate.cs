using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02001155 RID: 4437
	[CreateAssetMenu(fileName = "New CountdownText Date", menuName = "Game Object Scheduling/CountdownText Date", order = 1)]
	public class CountdownTextDate : ScriptableObject
	{
		// Token: 0x04008056 RID: 32854
		public string CountdownTo = "1/1/0001 00:00:00";

		// Token: 0x04008057 RID: 32855
		public string FormatString = "{0} {1}";

		// Token: 0x04008058 RID: 32856
		public string DefaultString = "";

		// Token: 0x04008059 RID: 32857
		public int DaysThreshold = 365;
	}
}
