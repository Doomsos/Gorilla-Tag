using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x0200115D RID: 4445
	[CreateAssetMenu(fileName = "New Options", menuName = "Game Object Scheduling/Options", order = 0)]
	public class SchedulingOptions : ScriptableObject
	{
		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x0600701C RID: 28700 RVA: 0x0024763D File Offset: 0x0024583D
		public DateTime DtDebugServerTime
		{
			get
			{
				return this.dtDebugServerTime.AddSeconds((double)(Time.time * this.timescale));
			}
		}

		// Token: 0x04008070 RID: 32880
		[SerializeField]
		private string debugServerTime;

		// Token: 0x04008071 RID: 32881
		[SerializeField]
		private DateTime dtDebugServerTime;

		// Token: 0x04008072 RID: 32882
		[SerializeField]
		[Range(-60f, 3660f)]
		private float timescale = 1f;
	}
}
