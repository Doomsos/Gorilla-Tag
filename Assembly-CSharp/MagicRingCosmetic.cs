using System;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class MagicRingCosmetic : MonoBehaviour
{
	// Token: 0x06000B41 RID: 2881 RVA: 0x0003D1DF File Offset: 0x0003B3DF
	protected void Awake()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.defaultEmissiveColor = this.ringRenderer.sharedMaterial.GetColor(ShaderProps._EmissionColor);
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0003D208 File Offset: 0x0003B408
	protected void LateUpdate()
	{
		float celsius = this.thermalReceiver.celsius;
		if (celsius >= this.fadeInTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedIn)
		{
			this.fadeInSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedIn;
		}
		else if (celsius <= this.fadeOutTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedOut)
		{
			this.fadeOutSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedOut;
		}
		this.emissiveAmount = Mathf.MoveTowards(this.emissiveAmount, (this.fadeState == MagicRingCosmetic.FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / this.fadeTime);
		this.ringRenderer.GetPropertyBlock(this.materialPropertyBlock);
		this.materialPropertyBlock.SetColor(ShaderProps._EmissionColor, new Color(this.defaultEmissiveColor.r, this.defaultEmissiveColor.g, this.defaultEmissiveColor.b, this.emissiveAmount));
		this.ringRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x04000DB1 RID: 3505
	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04000DB2 RID: 3506
	public Renderer ringRenderer;

	// Token: 0x04000DB3 RID: 3507
	public float fadeInTemperatureThreshold = 200f;

	// Token: 0x04000DB4 RID: 3508
	public float fadeOutTemperatureThreshold = 190f;

	// Token: 0x04000DB5 RID: 3509
	public float fadeTime = 1.5f;

	// Token: 0x04000DB6 RID: 3510
	public SoundBankPlayer fadeInSounds;

	// Token: 0x04000DB7 RID: 3511
	public SoundBankPlayer fadeOutSounds;

	// Token: 0x04000DB8 RID: 3512
	private MagicRingCosmetic.FadeState fadeState;

	// Token: 0x04000DB9 RID: 3513
	private Color defaultEmissiveColor;

	// Token: 0x04000DBA RID: 3514
	private float emissiveAmount;

	// Token: 0x04000DBB RID: 3515
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x020001A4 RID: 420
	private enum FadeState
	{
		// Token: 0x04000DBD RID: 3517
		FadedOut,
		// Token: 0x04000DBE RID: 3518
		FadedIn
	}
}
