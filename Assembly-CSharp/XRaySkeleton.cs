using System;
using UnityEngine;

// Token: 0x020004FA RID: 1274
public class XRaySkeleton : SyncToPlayerColor
{
	// Token: 0x060020C8 RID: 8392 RVA: 0x000ADB7C File Offset: 0x000ABD7C
	protected override void Awake()
	{
		base.Awake();
		this.target = this.renderer.material;
		Material[] materialsToChangeTo = this.rig.materialsToChangeTo;
		this.tagMaterials = new Material[materialsToChangeTo.Length];
		this.tagMaterials[0] = new Material(this.target);
		for (int i = 1; i < materialsToChangeTo.Length; i++)
		{
			Material material = new Material(materialsToChangeTo[i]);
			this.tagMaterials[i] = material;
		}
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000ADBED File Offset: 0x000ABDED
	public void SetMaterialIndex(int index)
	{
		this.renderer.sharedMaterial = this.tagMaterials[index];
		this._lastMatIndex = index;
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000ADC09 File Offset: 0x000ABE09
	private void Setup()
	{
		this.colorPropertiesToSync = new ShaderHashId[]
		{
			XRaySkeleton._BaseColor,
			XRaySkeleton._EmissionColor
		};
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000ADC30 File Offset: 0x000ABE30
	public override void UpdateColor(Color color)
	{
		if (this._lastMatIndex != 0)
		{
			return;
		}
		Material material = this.tagMaterials[0];
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(color, ref num, ref num2, ref num3);
		Color color2 = Color.HSVToRGB(num, num2, Mathf.Clamp(num3, this.baseValueMinMax.x, this.baseValueMinMax.y));
		material.SetColor(XRaySkeleton._BaseColor, color2);
		float num4;
		float num5;
		float num6;
		Color.RGBToHSV(color, ref num4, ref num5, ref num6);
		Color color3 = Color.HSVToRGB(num4, 0.82f, 0.9f, true);
		color3..ctor(color3.r * 1.4f, color3.g * 1.4f, color3.b * 1.4f);
		material.SetColor(XRaySkeleton._EmissionColor, ColorUtils.ComposeHDR(new Color32(36, 191, 136, byte.MaxValue), 2f));
		this.renderer.sharedMaterial = material;
	}

	// Token: 0x04002B5B RID: 11099
	public SkinnedMeshRenderer renderer;

	// Token: 0x04002B5C RID: 11100
	public Vector2 baseValueMinMax = new Vector2(0.69f, 1f);

	// Token: 0x04002B5D RID: 11101
	public Material[] tagMaterials = new Material[0];

	// Token: 0x04002B5E RID: 11102
	private int _lastMatIndex;

	// Token: 0x04002B5F RID: 11103
	private static readonly ShaderHashId _BaseColor = "_BaseColor";

	// Token: 0x04002B60 RID: 11104
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";
}
