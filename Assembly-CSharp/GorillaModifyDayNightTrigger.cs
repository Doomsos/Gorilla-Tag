using System;

// Token: 0x0200079E RID: 1950
public class GorillaModifyDayNightTrigger : GorillaTriggerBox
{
	// Token: 0x060032FB RID: 13051 RVA: 0x0011352C File Offset: 0x0011172C
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (this.clearModifiedTime)
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}
		else
		{
			int num = this.timeOfDayIndex % BetterDayNightManager.instance.timeOfDayRange.Length;
			BetterDayNightManager.instance.SetTimeOfDay(this.timeOfDayIndex);
			BetterDayNightManager.instance.SetOverrideIndex(this.timeOfDayIndex);
		}
		if (this.setFixedWeather)
		{
			BetterDayNightManager.instance.SetFixedWeather(this.fixedWeather);
			return;
		}
		BetterDayNightManager.instance.ClearFixedWeather();
	}

	// Token: 0x0400416A RID: 16746
	public bool clearModifiedTime;

	// Token: 0x0400416B RID: 16747
	public int timeOfDayIndex;

	// Token: 0x0400416C RID: 16748
	public bool setFixedWeather;

	// Token: 0x0400416D RID: 16749
	public BetterDayNightManager.WeatherType fixedWeather;
}
