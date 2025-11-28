using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020008FE RID: 2302
public class BetterDayNightManager : MonoBehaviour, IGorillaSliceableSimple, ITimeOfDaySystem
{
	// Token: 0x06003ADB RID: 15067 RVA: 0x001371D0 File Offset: 0x001353D0
	public static void Register(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Add(data);
	}

	// Token: 0x06003ADC RID: 15068 RVA: 0x001371DD File Offset: 0x001353DD
	public static void Unregister(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Remove(data);
	}

	// Token: 0x17000573 RID: 1395
	// (get) Token: 0x06003ADD RID: 15069 RVA: 0x001371EB File Offset: 0x001353EB
	// (set) Token: 0x06003ADE RID: 15070 RVA: 0x001371F3 File Offset: 0x001353F3
	public string currentTimeOfDay { get; private set; }

	// Token: 0x17000574 RID: 1396
	// (get) Token: 0x06003ADF RID: 15071 RVA: 0x001371FC File Offset: 0x001353FC
	public float NormalizedTimeOfDay
	{
		get
		{
			return Mathf.Clamp01((float)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds / this.totalSeconds));
		}
	}

	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x06003AE0 RID: 15072 RVA: 0x00137226 File Offset: 0x00135426
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x06003AE1 RID: 15073 RVA: 0x0013722E File Offset: 0x0013542E
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06003AE2 RID: 15074 RVA: 0x00137238 File Offset: 0x00135438
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (BetterDayNightManager.instance == null)
		{
			BetterDayNightManager.instance = this;
		}
		else if (BetterDayNightManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.currentLerp = 0f;
		this.totalHours = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.totalHours += this.timeOfDayRange[i];
		}
		this.totalSeconds = this.totalHours * 60.0 * 60.0;
		this.currentTimeIndex = 0;
		this.baseSeconds = 0.0;
		this.computerInit = false;
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateWeatherEventTimes();
		this.ChangeMaps(0, 1);
		base.StartCoroutine(this.InitialUpdate());
	}

	// Token: 0x06003AE3 RID: 15075 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003AE4 RID: 15076 RVA: 0x00002789 File Offset: 0x00000989
	protected void OnDestroy()
	{
	}

	// Token: 0x06003AE5 RID: 15077 RVA: 0x0013732C File Offset: 0x0013552C
	private Vector4 MaterialColorCorrection(Vector4 color)
	{
		if (color.x < 0.5f)
		{
			color.x += 3E-08f;
		}
		if (color.y < 0.5f)
		{
			color.y += 3E-08f;
		}
		if (color.z < 0.5f)
		{
			color.z += 3E-08f;
		}
		if (color.w < 0.5f)
		{
			color.w += 3E-08f;
		}
		return color;
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x001373B0 File Offset: 0x001355B0
	public void UpdateTimeOfDay()
	{
		if (Time.time < this.lastTimeChecked + this.currentTimestep)
		{
			return;
		}
		this.lastTimeChecked = Time.time;
		if (this.animatingLightFlash != null)
		{
			return;
		}
		try
		{
			if (!this.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.computerInit = true;
				this.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = (this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds;
				this.currentIndexSeconds = 0.0;
				for (int i = 0; i < this.timeOfDayRange.Length; i++)
				{
					this.currentIndexSeconds += this.timeOfDayRange[i] * 3600.0;
					if (this.currentIndexSeconds > this.currentTime)
					{
						this.currentTimeIndex = i;
						break;
					}
				}
				this.currentWeatherIndex += this.currentTimeIndex;
			}
			else if (!this.computerInit && this.baseSeconds == 0.0)
			{
				this.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier / this.totalSeconds);
				this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
				this.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier % this.totalSeconds;
				this.currentTime = this.baseSeconds % this.totalSeconds;
				this.currentIndexSeconds = 0.0;
				for (int j = 0; j < this.timeOfDayRange.Length; j++)
				{
					this.currentIndexSeconds += this.timeOfDayRange[j] * 3600.0;
					if (this.currentIndexSeconds > this.currentTime)
					{
						this.currentTimeIndex = j;
						break;
					}
				}
				this.currentWeatherIndex += this.currentTimeIndex - 1;
				if (this.currentWeatherIndex < 0)
				{
					this.currentWeatherIndex = this.weatherCycle.Length - 1;
				}
			}
			this.currentTime = ((this.currentSetting == TimeSettings.Normal) ? ((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds) : this.currentTime);
			this.currentIndexSeconds = 0.0;
			for (int k = 0; k < this.timeOfDayRange.Length; k++)
			{
				this.currentIndexSeconds += this.timeOfDayRange[k] * 3600.0;
				if (this.currentIndexSeconds > this.currentTime)
				{
					this.currentTimeIndex = k;
					break;
				}
			}
			if (this.timeIndexOverrideFunc != null)
			{
				this.currentTimeIndex = this.timeIndexOverrideFunc.Invoke(this.currentTimeIndex);
			}
			if (this.currentTimeIndex != this.lastIndex)
			{
				this.currentWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
				this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
			}
			this.currentLerp = (float)(1.0 - (this.currentIndexSeconds - this.currentTime) / (this.timeOfDayRange[this.currentTimeIndex] * 3600.0));
			this.ChangeLerps(this.currentLerp);
			this.lastIndex = this.currentTimeIndex;
			this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		}
		catch (Exception ex)
		{
			string text = "Error in BetterDayNightManager: ";
			Exception ex2 = ex;
			Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null), this);
		}
		this.gameEpochDay = (long)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) / this.totalSeconds + (double)this.initialDayCycles);
		foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
		{
			if (scheduledEvent.lastDayCalled != this.gameEpochDay && scheduledEvent.hour == this.currentTimeIndex)
			{
				scheduledEvent.lastDayCalled = this.gameEpochDay;
				scheduledEvent.action.Invoke();
			}
		}
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x001378A0 File Offset: 0x00135AA0
	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat(this._GlobalDayNightLerpValue, newLerp);
		for (int i = 0; i < this.standardMaterialsUnlit.Length; i++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFrom, this.colorTo, newLerp);
			this.standardMaterialsUnlit[i].color = new Color(this.tempLerp, this.tempLerp, this.tempLerp);
		}
		for (int j = 0; j < this.standardMaterialsUnlitDarker.Length; j++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFromDarker, this.colorToDarker, newLerp);
			Color.RGBToHSV(this.standardMaterialsUnlitDarker[j].color, ref this.h, ref this.s, ref this.v);
			this.standardMaterialsUnlitDarker[j].color = Color.HSVToRGB(this.h, this.s, this.tempLerp);
		}
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x00137980 File Offset: 0x00135B80
	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		this.PopulateAllLightmaps(fromIndex, toIndex);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex1, this.fromSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex2, this.toSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex1, this.fromSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex2, this.toSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex1, this.fromSky3);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex2, this.toSky3);
		this.colorFrom = this.standardUnlitColor[fromIndex];
		this.colorTo = this.standardUnlitColor[toIndex];
		this.colorFromDarker = this.standardUnlitColorWithPremadeColorDarker[fromIndex];
		this.colorToDarker = this.standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x00137B08 File Offset: 0x00135D08
	public void SliceUpdate()
	{
		if (!this.shouldRepopulate)
		{
			using (List<PerSceneRenderData>.Enumerator enumerator = BetterDayNightManager.allScenesRenderData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CheckShouldRepopulate())
					{
						this.shouldRepopulate = true;
					}
				}
			}
		}
		if (this.shouldRepopulate)
		{
			this.PopulateAllLightmaps();
			this.shouldRepopulate = false;
		}
		this.UpdateTimeOfDay();
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x00137B84 File Offset: 0x00135D84
	private IEnumerator InitialUpdate()
	{
		yield return null;
		this.SliceUpdate();
		yield break;
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x00137B93 File Offset: 0x00135D93
	public void RequestRepopulateLightmaps()
	{
		this.shouldRepopulate = true;
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x00137B9C File Offset: 0x00135D9C
	public void PopulateAllLightmaps()
	{
		this.PopulateAllLightmaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x00137BBC File Offset: 0x00135DBC
	public void PopulateAllLightmaps(int fromIndex, int toIndex)
	{
		string fromTimeOfDay;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			fromTimeOfDay = this.dayNightWeatherLightmapNames[fromIndex];
		}
		else
		{
			fromTimeOfDay = this.dayNightLightmapNames[fromIndex];
		}
		string toTimeOfDay;
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			toTimeOfDay = this.dayNightWeatherLightmapNames[toIndex];
		}
		else
		{
			toTimeOfDay = this.dayNightLightmapNames[toIndex];
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		foreach (PerSceneRenderData perSceneRenderData in BetterDayNightManager.allScenesRenderData)
		{
			perSceneRenderData.PopulateLightmaps(fromTimeOfDay, toTimeOfDay, lightmaps);
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x00137C64 File Offset: 0x00135E64
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[this.currentWeatherIndex];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x00137C82 File Offset: 0x00135E82
	public BetterDayNightManager.WeatherType NextWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x00137CAB File Offset: 0x00135EAB
	public BetterDayNightManager.WeatherType LastWeather()
	{
		if (!this.overrideWeather)
		{
			return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
		}
		return this.overrideWeatherType;
	}

	// Token: 0x06003AF1 RID: 15089 RVA: 0x00137CD4 File Offset: 0x00135ED4
	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmapNames.Length];
		this.rainChance = this.rainChance * 2f / (float)this.maxRainDuration;
		for (int i = 1; i < this.weatherCycle.Length; i++)
		{
			this.weatherCycle[i] = (((float)this.randomNumberGenerator.Next(100) < this.rainChance * 100f) ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None);
			if (this.weatherCycle[i] == BetterDayNightManager.WeatherType.Raining)
			{
				this.rainDuration = this.randomNumberGenerator.Next(1, this.maxRainDuration + 1);
				for (int j = 1; j < this.rainDuration; j++)
				{
					if (i + j < this.weatherCycle.Length)
					{
						this.weatherCycle[i + j] = BetterDayNightManager.WeatherType.Raining;
					}
				}
				i += this.rainDuration - 1;
			}
		}
	}

	// Token: 0x06003AF2 RID: 15090 RVA: 0x00137DAC File Offset: 0x00135FAC
	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int num = (int)(DateTime.Now.Ticks % 2147483647L);
		while (BetterDayNightManager.scheduledEvents.ContainsKey(num))
		{
			num++;
		}
		BetterDayNightManager.scheduledEvents.Add(num, new BetterDayNightManager.ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return num;
	}

	// Token: 0x06003AF3 RID: 15091 RVA: 0x00137E09 File Offset: 0x00136009
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x00137E17 File Offset: 0x00136017
	public void SetTimeIndexOverrideFunction(Func<int, int> overrideFunction)
	{
		this.timeIndexOverrideFunc = overrideFunction;
	}

	// Token: 0x06003AF5 RID: 15093 RVA: 0x00137E20 File Offset: 0x00136020
	public void UnsetTimeIndexOverrideFunction()
	{
		this.timeIndexOverrideFunc = null;
	}

	// Token: 0x06003AF6 RID: 15094 RVA: 0x00137E2C File Offset: 0x0013602C
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06003AF7 RID: 15095 RVA: 0x00137E88 File Offset: 0x00136088
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x06003AF8 RID: 15096 RVA: 0x00137EB5 File Offset: 0x001360B5
	private IEnumerator AnimateLightFlashCo(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		int startMap = (this.currentLerp < 0.5f) ? this.currentTimeIndex : ((this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeMaps(startMap, index);
		float endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(index, index);
		this.ChangeLerps(0f);
		endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			yield return null;
		}
		this.ChangeMaps(index, startMap);
		endTimestamp = Time.time + fadeOutDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeLerps(this.currentLerp);
		this.animatingLightFlash = null;
		yield break;
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x00137EDC File Offset: 0x001360DC
	public void SetTimeOfDay(int timeIndex)
	{
		double num = 0.0;
		for (int i = 0; i < timeIndex; i++)
		{
			num += this.timeOfDayRange[i];
		}
		this.currentTime = num * 3600.0;
		this.currentSetting = TimeSettings.Static;
	}

	// Token: 0x06003AFA RID: 15098 RVA: 0x00137F22 File Offset: 0x00136122
	public void FastForward(float seconds)
	{
		this.baseSeconds += (double)seconds;
	}

	// Token: 0x06003AFB RID: 15099 RVA: 0x00137F33 File Offset: 0x00136133
	public void SetFixedWeather(BetterDayNightManager.WeatherType weather)
	{
		this.overrideWeather = true;
		this.overrideWeatherType = weather;
	}

	// Token: 0x06003AFC RID: 15100 RVA: 0x00137F43 File Offset: 0x00136143
	public void ClearFixedWeather()
	{
		this.overrideWeather = false;
	}

	// Token: 0x04004AE4 RID: 19172
	[OnEnterPlay_SetNull]
	public static volatile BetterDayNightManager instance;

	// Token: 0x04004AE5 RID: 19173
	[OnEnterPlay_Clear]
	public static List<PerSceneRenderData> allScenesRenderData = new List<PerSceneRenderData>();

	// Token: 0x04004AE6 RID: 19174
	public Shader standard;

	// Token: 0x04004AE7 RID: 19175
	public Shader standardCutout;

	// Token: 0x04004AE8 RID: 19176
	public Shader gorillaUnlit;

	// Token: 0x04004AE9 RID: 19177
	public Shader gorillaUnlitCutout;

	// Token: 0x04004AEA RID: 19178
	public Material[] standardMaterialsUnlit;

	// Token: 0x04004AEB RID: 19179
	public Material[] standardMaterialsUnlitDarker;

	// Token: 0x04004AEC RID: 19180
	public Material[] dayNightSupportedMaterials;

	// Token: 0x04004AED RID: 19181
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x04004AEE RID: 19182
	public string[] dayNightLightmapNames;

	// Token: 0x04004AEF RID: 19183
	public string[] dayNightWeatherLightmapNames;

	// Token: 0x04004AF0 RID: 19184
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04004AF1 RID: 19185
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04004AF2 RID: 19186
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04004AF3 RID: 19187
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04004AF4 RID: 19188
	public float[] standardUnlitColor;

	// Token: 0x04004AF5 RID: 19189
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x04004AF6 RID: 19190
	public float currentLerp;

	// Token: 0x04004AF7 RID: 19191
	public float currentTimestep;

	// Token: 0x04004AF8 RID: 19192
	public double[] timeOfDayRange;

	// Token: 0x04004AF9 RID: 19193
	public double timeMultiplier;

	// Token: 0x04004AFA RID: 19194
	private float lastTime;

	// Token: 0x04004AFB RID: 19195
	private double currentTime;

	// Token: 0x04004AFC RID: 19196
	private double totalHours;

	// Token: 0x04004AFD RID: 19197
	private double totalSeconds;

	// Token: 0x04004AFE RID: 19198
	private float colorFrom;

	// Token: 0x04004AFF RID: 19199
	private float colorTo;

	// Token: 0x04004B00 RID: 19200
	private float colorFromDarker;

	// Token: 0x04004B01 RID: 19201
	private float colorToDarker;

	// Token: 0x04004B02 RID: 19202
	public int currentTimeIndex;

	// Token: 0x04004B03 RID: 19203
	public int currentWeatherIndex;

	// Token: 0x04004B04 RID: 19204
	private int lastIndex;

	// Token: 0x04004B05 RID: 19205
	private double currentIndexSeconds;

	// Token: 0x04004B06 RID: 19206
	private float tempLerp;

	// Token: 0x04004B07 RID: 19207
	private double baseSeconds;

	// Token: 0x04004B08 RID: 19208
	private bool computerInit;

	// Token: 0x04004B09 RID: 19209
	private float h;

	// Token: 0x04004B0A RID: 19210
	private float s;

	// Token: 0x04004B0B RID: 19211
	private float v;

	// Token: 0x04004B0C RID: 19212
	public int mySeed;

	// Token: 0x04004B0D RID: 19213
	public Random randomNumberGenerator = new Random();

	// Token: 0x04004B0E RID: 19214
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x04004B0F RID: 19215
	public bool overrideWeather;

	// Token: 0x04004B10 RID: 19216
	public BetterDayNightManager.WeatherType overrideWeatherType;

	// Token: 0x04004B12 RID: 19218
	public float rainChance = 0.3f;

	// Token: 0x04004B13 RID: 19219
	public int maxRainDuration = 5;

	// Token: 0x04004B14 RID: 19220
	private int rainDuration;

	// Token: 0x04004B15 RID: 19221
	private float remainingSeconds;

	// Token: 0x04004B16 RID: 19222
	private long initialDayCycles;

	// Token: 0x04004B17 RID: 19223
	private long gameEpochDay;

	// Token: 0x04004B18 RID: 19224
	private int currentWeatherCycle;

	// Token: 0x04004B19 RID: 19225
	private int fromWeatherIndex;

	// Token: 0x04004B1A RID: 19226
	private int toWeatherIndex;

	// Token: 0x04004B1B RID: 19227
	private Texture2D fromSky;

	// Token: 0x04004B1C RID: 19228
	private Texture2D fromSky2;

	// Token: 0x04004B1D RID: 19229
	private Texture2D fromSky3;

	// Token: 0x04004B1E RID: 19230
	private Texture2D toSky;

	// Token: 0x04004B1F RID: 19231
	private Texture2D toSky2;

	// Token: 0x04004B20 RID: 19232
	private Texture2D toSky3;

	// Token: 0x04004B21 RID: 19233
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x04004B22 RID: 19234
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x04004B23 RID: 19235
	private float lastTimeChecked;

	// Token: 0x04004B24 RID: 19236
	private Func<int, int> timeIndexOverrideFunc;

	// Token: 0x04004B25 RID: 19237
	public int overrideIndex = -1;

	// Token: 0x04004B26 RID: 19238
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x04004B27 RID: 19239
	public TimeSettings currentSetting;

	// Token: 0x04004B28 RID: 19240
	private ShaderHashId _Color = "_Color";

	// Token: 0x04004B29 RID: 19241
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x04004B2A RID: 19242
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x04004B2B RID: 19243
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x04004B2C RID: 19244
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x04004B2D RID: 19245
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x04004B2E RID: 19246
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x04004B2F RID: 19247
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x04004B30 RID: 19248
	private bool shouldRepopulate;

	// Token: 0x04004B31 RID: 19249
	private Coroutine animatingLightFlash;

	// Token: 0x020008FF RID: 2303
	public enum WeatherType
	{
		// Token: 0x04004B33 RID: 19251
		None,
		// Token: 0x04004B34 RID: 19252
		Raining,
		// Token: 0x04004B35 RID: 19253
		All
	}

	// Token: 0x02000900 RID: 2304
	private class ScheduledEvent
	{
		// Token: 0x04004B36 RID: 19254
		public long lastDayCalled;

		// Token: 0x04004B37 RID: 19255
		public int hour;

		// Token: 0x04004B38 RID: 19256
		public Action action;
	}
}
