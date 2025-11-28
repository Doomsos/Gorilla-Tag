using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000171 RID: 369
public class SkyboxController : MonoBehaviour
{
	// Token: 0x060009F2 RID: 2546 RVA: 0x00035D7C File Offset: 0x00033F7C
	private void Start()
	{
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00035E0A File Offset: 0x0003400A
	private void Update()
	{
		if (!this.lastUpdate.HasElapsed(1f, true))
		{
			return;
		}
		this.UpdateTime();
		this.UpdateSky();
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00035E2C File Offset: 0x0003402C
	private void OnValidate()
	{
		this.UpdateSky();
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00035E34 File Offset: 0x00034034
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00035E6C File Offset: 0x0003406C
	private void UpdateSky()
	{
		if (this.skyMaterials == null || this.skyMaterials.Length == 0)
		{
			return;
		}
		int num = this.skyMaterials.Length;
		float num2 = Mathf.Clamp(this._currentTime, 0f, 1f);
		float num3 = 1f / (float)num;
		int num4 = (int)(num2 / num3);
		float num5 = (num2 - (float)num4 * num3) / num3;
		this._currentSky = this.skyMaterials[num4];
		this._nextSky = this.skyMaterials[(num4 + 1) % num];
		this.skyFront.sharedMaterial = this._currentSky;
		this.skyBack.sharedMaterial = this._nextSky;
		if (this._currentSky.renderQueue != 3000)
		{
			this.SetFrontToTransparent();
		}
		if (this._nextSky.renderQueue == 3000)
		{
			this.SetBackToOpaque();
		}
		this._currentSky.SetFloat(ShaderProps._SkyAlpha, 1f - num5);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00035F48 File Offset: 0x00034148
	private void SetFrontToTransparent()
	{
		bool flag = false;
		bool flag2 = false;
		string text = "Transparent";
		int renderQueue = 3000;
		BlendMode blendMode = 5;
		BlendMode blendMode2 = 10;
		BlendMode blendMode3 = 1;
		BlendMode blendMode4 = 10;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag2 ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, blendMode4);
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00036008 File Offset: 0x00034208
	private void SetFrontToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string text = "Opaque";
		int renderQueue = 2000;
		BlendMode blendMode = 1;
		BlendMode blendMode2 = 0;
		BlendMode blendMode3 = 1;
		BlendMode blendMode4 = 0;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, blendMode4);
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x000360C8 File Offset: 0x000342C8
	private void SetBackToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string text = "Opaque";
		int renderQueue = 2000;
		BlendMode blendMode = 1;
		BlendMode blendMode2 = 0;
		BlendMode blendMode3 = 1;
		BlendMode blendMode4 = 0;
		Material sharedMaterial = this.skyBack.sharedMaterial;
		sharedMaterial.SetFloat(ShaderProps._ZWrite, flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat(ShaderProps._AlphaToMask, flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = renderQueue;
		sharedMaterial.SetFloat(ShaderProps._SrcBlend, blendMode);
		sharedMaterial.SetFloat(ShaderProps._DstBlend, blendMode2);
		sharedMaterial.SetFloat(ShaderProps._SrcBlendAlpha, blendMode3);
		sharedMaterial.SetFloat(ShaderProps._DstBlendAlpha, blendMode4);
	}

	// Token: 0x04000C2D RID: 3117
	public MeshRenderer skyFront;

	// Token: 0x04000C2E RID: 3118
	public MeshRenderer skyBack;

	// Token: 0x04000C2F RID: 3119
	public Material[] skyMaterials = new Material[0];

	// Token: 0x04000C30 RID: 3120
	[Range(0f, 1f)]
	public float lerpValue;

	// Token: 0x04000C31 RID: 3121
	[NonSerialized]
	private Material _currentSky;

	// Token: 0x04000C32 RID: 3122
	[NonSerialized]
	private Material _nextSky;

	// Token: 0x04000C33 RID: 3123
	private TimeSince lastUpdate = TimeSince.Now();

	// Token: 0x04000C34 RID: 3124
	[Space]
	private BetterDayNightManager _dayNightManager;

	// Token: 0x04000C35 RID: 3125
	private double _currentSeconds = -1.0;

	// Token: 0x04000C36 RID: 3126
	private double _totalSecondsInRange = -1.0;

	// Token: 0x04000C37 RID: 3127
	private float _currentTime = -1f;
}
