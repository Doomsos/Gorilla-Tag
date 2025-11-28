using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000905 RID: 2309
public class DayNightCycle : MonoBehaviour
{
	// Token: 0x06003B0E RID: 15118 RVA: 0x00138264 File Offset: 0x00136464
	public void Awake()
	{
		this.fromMap = new Texture2D(this._sunriseMap.width, this._sunriseMap.height);
		this.fromMap = LightmapSettings.lightmaps[0].lightmapColor;
		this.toMap = new Texture2D(this._dayMap.width, this._dayMap.height);
		this.toMap.SetPixels(this._dayMap.GetPixels());
		this.toMap.Apply();
		this.workBlockMix = new Color[this.subTextureSize * this.subTextureSize];
		this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height, this.fromMap.graphicsFormat, 0);
		this.newData = new LightmapData();
		this.textureHeight = this.fromMap.height;
		this.textureWidth = this.fromMap.width;
		this.subTextureArray = new Texture2D[(int)Mathf.Pow((float)(this.textureHeight / this.subTextureSize), 2f)];
		Debug.Log("aaaa " + this.fromMap.format.ToString());
		Debug.Log("aaaa " + this.fromMap.graphicsFormat.ToString());
		this.startJob = false;
		this.startCoroutine = false;
		this.startedCoroutine = false;
		this.finishedCoroutine = false;
	}

	// Token: 0x06003B0F RID: 15119 RVA: 0x001383E8 File Offset: 0x001365E8
	public void Update()
	{
		if (this.startJob)
		{
			this.startJob = false;
			this.startTime = Time.realtimeSinceStartup;
			base.StartCoroutine(this.UpdateWork());
			this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
		}
		if (this.jobStarted && this.jobHandle.IsCompleted)
		{
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.jobHandle.Complete();
			this.jobStarted = false;
			this.newTexture.SetPixels(this.job.mixedPixels.ToArray());
			this.newData.lightmapDir = LightmapSettings.lightmaps[0].lightmapDir;
			LightmapSettings.lightmaps = new LightmapData[]
			{
				this.newData
			};
			this.job.fromPixels.Dispose();
			this.job.toPixels.Dispose();
			this.job.mixedPixels.Dispose();
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
		if (this.startCoroutine)
		{
			this.startCoroutine = false;
			this.startTime = Time.realtimeSinceStartup;
			this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height);
			base.StartCoroutine(this.UpdateWork());
		}
		if (this.startedCoroutine && this.finishedCoroutine)
		{
			this.startedCoroutine = false;
			this.finishedCoroutine = false;
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.newData = LightmapSettings.lightmaps[0];
			this.newData.lightmapColor = this.fromMap;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			lightmaps[0].lightmapColor = this.fromMap;
			LightmapSettings.lightmaps = lightmaps;
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x001385D6 File Offset: 0x001367D6
	public IEnumerator UpdateWork()
	{
		yield return 0;
		this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
		this.startTime = Time.realtimeSinceStartup;
		this.startedCoroutine = true;
		this.currentSubTexture = 0;
		int num;
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.subTextureArray[i] = new Texture2D(this.subTextureSize, this.subTextureSize, this.fromMap.graphicsFormat, 0);
			yield return 0;
			num = i;
		}
		for (int i = 0; i < this.textureWidth / this.subTextureSize; i = num + 1)
		{
			this.currentColumn = i;
			for (int j = 0; j < this.textureHeight / this.subTextureSize; j = num + 1)
			{
				this.currentRow = j;
				this.workBlockFrom = this.fromMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				this.workBlockTo = this.toMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				for (int k = 0; k < this.subTextureSize * this.subTextureSize - 1; k++)
				{
					this.workBlockMix[k] = Color.Lerp(this.workBlockFrom[k], this.workBlockTo[k], this.lerpAmount);
				}
				this.subTextureArray[j * (this.textureWidth / this.subTextureSize) + i].SetPixels(0, 0, this.subTextureSize, this.subTextureSize, this.workBlockMix);
				yield return 0;
				num = j;
			}
			num = i;
		}
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.currentSubTexture = i;
			this.subTextureArray[i].Apply();
			yield return 0;
			Graphics.CopyTexture(this.subTextureArray[i], 0, 0, 0, 0, this.subTextureSize, this.subTextureSize, this.newTexture, 0, 0, i * this.subTextureSize % this.textureHeight, (int)Mathf.Floor((float)(this.subTextureSize * i / this.textureHeight)) * this.subTextureSize);
			yield return 0;
			num = i;
		}
		this.finishedCoroutine = true;
		yield break;
	}

	// Token: 0x04004B47 RID: 19271
	public Texture2D _dayMap;

	// Token: 0x04004B48 RID: 19272
	private Texture2D fromMap;

	// Token: 0x04004B49 RID: 19273
	public Texture2D _sunriseMap;

	// Token: 0x04004B4A RID: 19274
	private Texture2D toMap;

	// Token: 0x04004B4B RID: 19275
	public DayNightCycle.LerpBakedLightingJob job;

	// Token: 0x04004B4C RID: 19276
	public JobHandle jobHandle;

	// Token: 0x04004B4D RID: 19277
	public bool isComplete;

	// Token: 0x04004B4E RID: 19278
	private float startTime;

	// Token: 0x04004B4F RID: 19279
	public float timeTakenStartingJob;

	// Token: 0x04004B50 RID: 19280
	public float timeTakenPostJob;

	// Token: 0x04004B51 RID: 19281
	public float timeTakenDuringJob;

	// Token: 0x04004B52 RID: 19282
	public LightmapData newData;

	// Token: 0x04004B53 RID: 19283
	private Color[] fromPixels;

	// Token: 0x04004B54 RID: 19284
	private Color[] toPixels;

	// Token: 0x04004B55 RID: 19285
	private Color[] mixedPixels;

	// Token: 0x04004B56 RID: 19286
	private LightmapData[] newDatas;

	// Token: 0x04004B57 RID: 19287
	public Texture2D newTexture;

	// Token: 0x04004B58 RID: 19288
	public int textureWidth;

	// Token: 0x04004B59 RID: 19289
	public int textureHeight;

	// Token: 0x04004B5A RID: 19290
	private Color[] workBlockFrom;

	// Token: 0x04004B5B RID: 19291
	private Color[] workBlockTo;

	// Token: 0x04004B5C RID: 19292
	private Color[] workBlockMix;

	// Token: 0x04004B5D RID: 19293
	public int subTextureSize = 1024;

	// Token: 0x04004B5E RID: 19294
	public Texture2D[] subTextureArray;

	// Token: 0x04004B5F RID: 19295
	public bool startCoroutine;

	// Token: 0x04004B60 RID: 19296
	public bool startedCoroutine;

	// Token: 0x04004B61 RID: 19297
	public bool finishedCoroutine;

	// Token: 0x04004B62 RID: 19298
	public bool startJob;

	// Token: 0x04004B63 RID: 19299
	public float switchTimeTaken;

	// Token: 0x04004B64 RID: 19300
	public bool jobStarted;

	// Token: 0x04004B65 RID: 19301
	public float lerpAmount;

	// Token: 0x04004B66 RID: 19302
	public int currentRow;

	// Token: 0x04004B67 RID: 19303
	public int currentColumn;

	// Token: 0x04004B68 RID: 19304
	public int currentSubTexture;

	// Token: 0x04004B69 RID: 19305
	public int currentRowInSubtexture;

	// Token: 0x02000906 RID: 2310
	public struct LerpBakedLightingJob : IJob
	{
		// Token: 0x06003B12 RID: 15122 RVA: 0x001385F8 File Offset: 0x001367F8
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		// Token: 0x04004B6A RID: 19306
		public NativeArray<Color> fromPixels;

		// Token: 0x04004B6B RID: 19307
		public NativeArray<Color> toPixels;

		// Token: 0x04004B6C RID: 19308
		public NativeArray<Color> mixedPixels;

		// Token: 0x04004B6D RID: 19309
		public float lerpValue;
	}
}
