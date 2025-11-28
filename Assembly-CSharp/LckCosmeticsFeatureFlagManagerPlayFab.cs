using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using UnityEngine.Scripting;

// Token: 0x02000379 RID: 889
[Preserve]
public class LckCosmeticsFeatureFlagManagerPlayFab : ILckCosmeticsFeatureFlagManager
{
	// Token: 0x06001521 RID: 5409 RVA: 0x00077C63 File Offset: 0x00075E63
	[Preserve]
	public LckCosmeticsFeatureFlagManagerPlayFab()
	{
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00077C78 File Offset: 0x00075E78
	public Task<bool> IsEnabledAsync()
	{
		if (this._initializationTask != null)
		{
			return this._initializationTask;
		}
		object @lock = this._lock;
		Task<bool> task2;
		lock (@lock)
		{
			Task<bool> task;
			if ((task = this._initializationTask) == null)
			{
				task2 = (this._initializationTask = this.GetEnabledStateWithRetryAsync());
				task = task2;
			}
			task2 = task;
		}
		return task2;
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x00077CDC File Offset: 0x00075EDC
	private Task<bool> GetEnabledStateWithRetryAsync()
	{
		LckCosmeticsFeatureFlagManagerPlayFab.<GetEnabledStateWithRetryAsync>d__7 <GetEnabledStateWithRetryAsync>d__;
		<GetEnabledStateWithRetryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<GetEnabledStateWithRetryAsync>d__.<>1__state = -1;
		<GetEnabledStateWithRetryAsync>d__.<>t__builder.Start<LckCosmeticsFeatureFlagManagerPlayFab.<GetEnabledStateWithRetryAsync>d__7>(ref <GetEnabledStateWithRetryAsync>d__);
		return <GetEnabledStateWithRetryAsync>d__.<>t__builder.Task;
	}

	// Token: 0x04001FA7 RID: 8103
	private const string TitleDataKey = "EnableLckCosmetics";

	// Token: 0x04001FA8 RID: 8104
	private const int MaxRetries = 2;

	// Token: 0x04001FA9 RID: 8105
	private const int RetryDelayMilliseconds = 5000;

	// Token: 0x04001FAA RID: 8106
	private Task<bool> _initializationTask;

	// Token: 0x04001FAB RID: 8107
	private readonly object _lock = new object();
}
