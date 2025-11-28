using System;
using System.Collections;
using System.Threading;
using UnityEngine;

// Token: 0x02000779 RID: 1913
public class GorillaDayNight : MonoBehaviour
{
	// Token: 0x060031CC RID: 12748 RVA: 0x0010DCBC File Offset: 0x0010BEBC
	public void Awake()
	{
		if (GorillaDayNight.instance == null)
		{
			GorillaDayNight.instance = this;
		}
		else if (GorillaDayNight.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.test = false;
		this.working = false;
		this.lerpValue = 0.5f;
		this.workingLightMapDatas = new LightmapData[3];
		this.workingLightMapData = new LightmapData();
		this.workingLightMapData.lightmapColor = this.lightmapDatas[0].lightTextures[0];
		this.workingLightMapData.lightmapDir = this.lightmapDatas[0].dirTextures[0];
	}

	// Token: 0x060031CD RID: 12749 RVA: 0x0010DD60 File Offset: 0x0010BF60
	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x0010DD90 File Offset: 0x0010BF90
	public void DoWork()
	{
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		this.done = true;
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x0010DF9C File Offset: 0x0010C19C
	public void DoLightsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x0010E060 File Offset: 0x0010C260
	public void DoDirsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x0010E123 File Offset: 0x0010C323
	private IEnumerator LightMapSet(int setFirstData, int setSecondData, float setLerp)
	{
		this.working = true;
		this.firstData = setFirstData;
		this.secondData = setSecondData;
		this.lerpValue = setLerp;
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.lightsThread = new Thread(new ThreadStart(this.DoLightsStep));
			this.lightsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapColor.Apply(false);
			this.dirsThread = new Thread(new ThreadStart(this.DoDirsStep));
			this.dirsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		LightmapSettings.lightmaps = this.workingLightMapDatas;
		this.working = false;
		this.done = true;
		yield break;
	}

	// Token: 0x04004052 RID: 16466
	[OnEnterPlay_SetNull]
	public static volatile GorillaDayNight instance;

	// Token: 0x04004053 RID: 16467
	public GorillaLightmapData[] lightmapDatas;

	// Token: 0x04004054 RID: 16468
	private LightmapData[] workingLightMapDatas;

	// Token: 0x04004055 RID: 16469
	private LightmapData workingLightMapData;

	// Token: 0x04004056 RID: 16470
	public float lerpValue;

	// Token: 0x04004057 RID: 16471
	public bool done;

	// Token: 0x04004058 RID: 16472
	public bool finishedStep;

	// Token: 0x04004059 RID: 16473
	private Color[] fromPixels;

	// Token: 0x0400405A RID: 16474
	private Color[] toPixels;

	// Token: 0x0400405B RID: 16475
	private Color[] mixedPixels;

	// Token: 0x0400405C RID: 16476
	public int firstData;

	// Token: 0x0400405D RID: 16477
	public int secondData;

	// Token: 0x0400405E RID: 16478
	public int i;

	// Token: 0x0400405F RID: 16479
	public int j;

	// Token: 0x04004060 RID: 16480
	public int k;

	// Token: 0x04004061 RID: 16481
	public int l;

	// Token: 0x04004062 RID: 16482
	private Thread lightsThread;

	// Token: 0x04004063 RID: 16483
	private Thread dirsThread;

	// Token: 0x04004064 RID: 16484
	public bool test;

	// Token: 0x04004065 RID: 16485
	public bool working;
}
