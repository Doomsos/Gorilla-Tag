using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x02000F56 RID: 3926
	[CreateAssetMenu(fileName = "RandomDispatcher", menuName = "NetSynchrony/RandomDispatcher", order = 0)]
	public class RandomDispatcher : ScriptableObject
	{
		// Token: 0x140000AE RID: 174
		// (add) Token: 0x0600626B RID: 25195 RVA: 0x001FB06C File Offset: 0x001F926C
		// (remove) Token: 0x0600626C RID: 25196 RVA: 0x001FB0A4 File Offset: 0x001F92A4
		public event RandomDispatcher.RandomDispatcherEvent Dispatch;

		// Token: 0x0600626D RID: 25197 RVA: 0x001FB0DC File Offset: 0x001F92DC
		public void Init(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			this.dispatchTimes = new List<float>();
			float num = 0f;
			float num2 = this.totalMinutes * 60f;
			Random.InitState(StaticHash.Compute(Application.buildGUID));
			while (num < num2)
			{
				float num3 = Random.Range(this.minWaitTime, this.maxWaitTime);
				num += num3;
				if ((double)num < seconds)
				{
					this.index = this.dispatchTimes.Count;
				}
				this.dispatchTimes.Add(num);
			}
			Random.InitState((int)DateTime.Now.Ticks);
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x001FB180 File Offset: 0x001F9380
		public void Sync(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			for (int i = 0; i < this.dispatchTimes.Count; i++)
			{
				if ((double)this.dispatchTimes[i] < seconds)
				{
					this.index = i;
				}
			}
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x001FB1D4 File Offset: 0x001F93D4
		public void Tick(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			if ((double)this.dispatchTimes[this.index] < seconds)
			{
				this.index = (this.index + 1) % this.dispatchTimes.Count;
				if (this.Dispatch != null)
				{
					this.Dispatch(this);
				}
			}
		}

		// Token: 0x040070FF RID: 28927
		[SerializeField]
		private float minWaitTime = 1f;

		// Token: 0x04007100 RID: 28928
		[SerializeField]
		private float maxWaitTime = 10f;

		// Token: 0x04007101 RID: 28929
		[SerializeField]
		private float totalMinutes = 60f;

		// Token: 0x04007102 RID: 28930
		private List<float> dispatchTimes;

		// Token: 0x04007103 RID: 28931
		private int index = -1;

		// Token: 0x02000F57 RID: 3927
		// (Invoke) Token: 0x06006272 RID: 25202
		public delegate void RandomDispatcherEvent(RandomDispatcher randomDispatcher);
	}
}
