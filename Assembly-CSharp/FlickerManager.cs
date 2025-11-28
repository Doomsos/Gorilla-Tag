using System;
using System.Linq;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200050A RID: 1290
public sealed class FlickerManager : MonoBehaviour
{
	// Token: 0x060020F5 RID: 8437 RVA: 0x000AE740 File Offset: 0x000AC940
	private void Awake()
	{
		if (this.FlickerDurations.Length % 2 != 0)
		{
			Debug.LogWarning("FlickerManager should have an even number of steps; removing last entry.");
			this.FlickerDurations = Enumerable.ToArray<float>(Enumerable.Take<float>(this.FlickerDurations, this.FlickerDurations.Length - 1));
		}
		if (this.FlickerDurations.Length == 0)
		{
			Debug.LogWarning("No flicker durations set for FlickerManager, disabling.");
			Object.Destroy(this);
			return;
		}
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000AE7A0 File Offset: 0x000AC9A0
	private void Update()
	{
		float serverTime = FlickerManager.GetServerTime();
		if (serverTime < this._nextFlickerTime)
		{
			return;
		}
		BetterDayNightManager.instance.AnimateLightFlash(this.LightmapIndex, this.FlickerFadeInDuration, this.FlickerDurations[this._flickerIndex], this.FlickerFadeOutDuration);
		this._nextFlickerTime = serverTime + this.FlickerDurations[this._flickerIndex + 1];
		this._flickerIndex = (this._flickerIndex + 2) % this.FlickerDurations.Length;
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000AE818 File Offset: 0x000ACA18
	private static float GetServerTime()
	{
		return (float)(GorillaComputer.instance.GetServerTime() - GorillaComputer.instance.startupTime).TotalSeconds;
	}

	// Token: 0x04002B9F RID: 11167
	public float[] FlickerDurations;

	// Token: 0x04002BA0 RID: 11168
	public float FlickerFadeInDuration;

	// Token: 0x04002BA1 RID: 11169
	public float FlickerFadeOutDuration;

	// Token: 0x04002BA2 RID: 11170
	public int LightmapIndex;

	// Token: 0x04002BA3 RID: 11171
	private int _flickerIndex;

	// Token: 0x04002BA4 RID: 11172
	private float _nextFlickerTime = float.MinValue;
}
