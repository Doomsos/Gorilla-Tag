using System;
using UnityEngine;

// Token: 0x020005B1 RID: 1457
public class Campfire : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060024B7 RID: 9399 RVA: 0x000C5E04 File Offset: 0x000C4004
	private void Start()
	{
		this.lastAngleBottom = 0f;
		this.lastAngleMiddle = 0f;
		this.lastAngleTop = 0f;
		this.perlinBottom = (float)Random.Range(0, 100);
		this.perlinMiddle = (float)Random.Range(200, 300);
		this.perlinTop = (float)Random.Range(400, 500);
		this.startingRotationBottom = this.baseFire.localEulerAngles.x;
		this.startingRotationMiddle = this.middleFire.localEulerAngles.x;
		this.startingRotationTop = this.topFire.localEulerAngles.x;
		this.tempVec = new Vector3(0f, 0f, 0f);
		this.mergedBottom = false;
		this.mergedMiddle = false;
		this.mergedTop = false;
		this.wasActive = false;
		this.lastTime = Time.time;
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x000C5EF0 File Offset: 0x000C40F0
	public void SliceUpdate()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		if ((this.isActive[BetterDayNightManager.instance.currentTimeIndex] && BetterDayNightManager.instance.CurrentWeather() != BetterDayNightManager.WeatherType.Raining) || this.overrideDayNight == 1)
		{
			if (!this.wasActive)
			{
				this.wasActive = true;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, ref this.h, ref this.s, ref this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 1f);
			}
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
		}
		else
		{
			if (this.wasActive)
			{
				this.wasActive = false;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, ref this.h, ref this.s, ref this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 0.25f);
			}
			this.ReturnToOff(ref this.baseFire, this.startingRotationBottom, ref this.mergedBottom);
			this.ReturnToOff(ref this.middleFire, this.startingRotationMiddle, ref this.mergedMiddle);
			this.ReturnToOff(ref this.topFire, this.startingRotationTop, ref this.mergedTop);
		}
		this.lastTime = Time.time;
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x000C60F4 File Offset: 0x000C42F4
	private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
	{
		perlinValue += perlinStep;
		lastAngle += (Time.time - this.lastTime) * Mathf.PerlinNoise(perlinValue, 0f);
		this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
		if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > flameTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (isMerged)
		{
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
		{
			isMerged = true;
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
		flameTransform.localEulerAngles = this.tempVec;
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x000C623C File Offset: 0x000C443C
	private void ReturnToOff(ref Transform startTransform, float targetAngle, ref bool isMerged)
	{
		this.tempVec.x = targetAngle;
		if (Mathf.Abs(this.tempVec.x - startTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > startTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (!isMerged)
		{
			if (Mathf.Abs(startTransform.localEulerAngles.x - targetAngle) < 1f)
			{
				isMerged = true;
				return;
			}
			this.tempVec.x = (this.tempVec.x - startTransform.localEulerAngles.x) * this.slerp + startTransform.localEulerAngles.x;
			startTransform.localEulerAngles = this.tempVec;
		}
	}

	// Token: 0x04003042 RID: 12354
	public Transform baseFire;

	// Token: 0x04003043 RID: 12355
	public Transform middleFire;

	// Token: 0x04003044 RID: 12356
	public Transform topFire;

	// Token: 0x04003045 RID: 12357
	public float baseMultiplier;

	// Token: 0x04003046 RID: 12358
	public float middleMultiplier;

	// Token: 0x04003047 RID: 12359
	public float topMultiplier;

	// Token: 0x04003048 RID: 12360
	public float bottomRange;

	// Token: 0x04003049 RID: 12361
	public float middleRange;

	// Token: 0x0400304A RID: 12362
	public float topRange;

	// Token: 0x0400304B RID: 12363
	private float lastAngleBottom;

	// Token: 0x0400304C RID: 12364
	private float lastAngleMiddle;

	// Token: 0x0400304D RID: 12365
	private float lastAngleTop;

	// Token: 0x0400304E RID: 12366
	public float perlinStepBottom;

	// Token: 0x0400304F RID: 12367
	public float perlinStepMiddle;

	// Token: 0x04003050 RID: 12368
	public float perlinStepTop;

	// Token: 0x04003051 RID: 12369
	private float perlinBottom;

	// Token: 0x04003052 RID: 12370
	private float perlinMiddle;

	// Token: 0x04003053 RID: 12371
	private float perlinTop;

	// Token: 0x04003054 RID: 12372
	public float startingRotationBottom;

	// Token: 0x04003055 RID: 12373
	public float startingRotationMiddle;

	// Token: 0x04003056 RID: 12374
	public float startingRotationTop;

	// Token: 0x04003057 RID: 12375
	public float slerp = 0.01f;

	// Token: 0x04003058 RID: 12376
	private bool mergedBottom;

	// Token: 0x04003059 RID: 12377
	private bool mergedMiddle;

	// Token: 0x0400305A RID: 12378
	private bool mergedTop;

	// Token: 0x0400305B RID: 12379
	public string lastTimeOfDay;

	// Token: 0x0400305C RID: 12380
	public Material mat;

	// Token: 0x0400305D RID: 12381
	private float h;

	// Token: 0x0400305E RID: 12382
	private float s;

	// Token: 0x0400305F RID: 12383
	private float v;

	// Token: 0x04003060 RID: 12384
	public int overrideDayNight;

	// Token: 0x04003061 RID: 12385
	private Vector3 tempVec;

	// Token: 0x04003062 RID: 12386
	public bool[] isActive;

	// Token: 0x04003063 RID: 12387
	public bool wasActive;

	// Token: 0x04003064 RID: 12388
	private float lastTime;
}
