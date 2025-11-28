using System;
using UnityEngine;

// Token: 0x0200081F RID: 2079
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x060036A7 RID: 13991 RVA: 0x00127ABB File Offset: 0x00125CBB
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x060036A8 RID: 13992 RVA: 0x00127AD6 File Offset: 0x00125CD6
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x060036A9 RID: 13993 RVA: 0x00127B06 File Offset: 0x00125D06
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor(ShaderProps._BaseColor, color);
		this._applyMaterial.Apply();
	}

	// Token: 0x060036AA RID: 13994 RVA: 0x00127B34 File Offset: 0x00125D34
	private void ApplyWaveParams(float amplitude, float frequency, float scale, float rotation)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetFloat(ShaderProps._WaveAmplitude, amplitude);
		this._applyMaterial.SetFloat(ShaderProps._WaveFrequency, frequency);
		this._applyMaterial.SetFloat(ShaderProps._WaveScale, scale);
		this._applyMaterial.Apply();
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x00127B8D File Offset: 0x00125D8D
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x00127BA8 File Offset: 0x00125DA8
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x00127BBC File Offset: 0x00125DBC
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._animationCurve.Evaluate(this._animProgress / this.animLength);
		float num2 = this._waveCurve.Evaluate(this._animProgress / this.animLength);
		if (num >= 1f)
		{
			this.ApplyColor(this._colorEnd);
			this._animating = false;
			base.enabled = false;
			return;
		}
		Color color = Color.Lerp(this._colorStart, this._colorEnd, num);
		Mathf.Lerp(this.waveNormal.frequency, this.waveAnimating.frequency, num2);
		Mathf.Lerp(this.waveNormal.amplitude, this.waveAnimating.amplitude, num2);
		Mathf.Lerp(this.waveNormal.scale, this.waveAnimating.scale, num2);
		Mathf.Lerp(this.waveNormal.rotation, this.waveAnimating.rotation, num2);
		this.ApplyColor(color);
		this._animProgress += Time.deltaTime;
	}

	// Token: 0x04004607 RID: 17927
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04004608 RID: 17928
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04004609 RID: 17929
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x0400460A RID: 17930
	[SerializeField]
	private bool _animating;

	// Token: 0x0400460B RID: 17931
	[SerializeField]
	private float _animProgress;

	// Token: 0x0400460C RID: 17932
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x0400460D RID: 17933
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x0400460E RID: 17934
	public float animLength = 1f;

	// Token: 0x0400460F RID: 17935
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04004610 RID: 17936
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x02000820 RID: 2080
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04004611 RID: 17937
		public float amplitude;

		// Token: 0x04004612 RID: 17938
		public float frequency;

		// Token: 0x04004613 RID: 17939
		public float scale;

		// Token: 0x04004614 RID: 17940
		public float rotation;
	}
}
