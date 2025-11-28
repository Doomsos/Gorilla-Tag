using System;
using GorillaNetworking;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x02000F58 RID: 3928
	public class RandomDispatcherManager : MonoBehaviour
	{
		// Token: 0x06006275 RID: 25205 RVA: 0x001FB268 File Offset: 0x001F9468
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x06006276 RID: 25206 RVA: 0x001FB2B4 File Offset: 0x001F94B4
		private void OnTimeChanged()
		{
			this.AdjustedServerTime();
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Sync(this.serverTime);
			}
		}

		// Token: 0x06006277 RID: 25207 RVA: 0x001FB2F0 File Offset: 0x001F94F0
		private void AdjustedServerTime()
		{
			DateTime dateTime;
			dateTime..ctor(2020, 1, 1);
			long num = GorillaComputer.instance.GetServerTime().Ticks - dateTime.Ticks;
			this.serverTime = (double)((float)num / 10000000f);
		}

		// Token: 0x06006278 RID: 25208 RVA: 0x001FB338 File Offset: 0x001F9538
		private void Start()
		{
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Init(this.serverTime);
			}
		}

		// Token: 0x06006279 RID: 25209 RVA: 0x001FB394 File Offset: 0x001F9594
		private void Update()
		{
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Tick(this.serverTime);
			}
			this.serverTime += (double)Time.deltaTime;
		}

		// Token: 0x04007104 RID: 28932
		[SerializeField]
		private RandomDispatcher[] randomDispatchers;

		// Token: 0x04007105 RID: 28933
		private static RandomDispatcherManager __instance;

		// Token: 0x04007106 RID: 28934
		private double serverTime;
	}
}
