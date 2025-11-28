using System;
using UnityEngine;

// Token: 0x02000833 RID: 2099
public class TapInnerGlow : MonoBehaviour
{
	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x06003735 RID: 14133 RVA: 0x00129898 File Offset: 0x00127A98
	private Material targetMaterial
	{
		get
		{
			if (this._instance.AsNull<Material>() == null)
			{
				return this._instance = this._renderer.material;
			}
			return this._instance;
		}
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x001298D4 File Offset: 0x00127AD4
	public void Tap()
	{
		if (!this._renderer)
		{
			return;
		}
		Material targetMaterial = this.targetMaterial;
		float value = this.tapLength;
		float time = GTShaderGlobals.Time;
		UberShader.InnerGlowSinePeriod.SetValue<float>(targetMaterial, value);
		UberShader.InnerGlowSinePhaseShift.SetValue<float>(targetMaterial, time);
	}

	// Token: 0x040046A8 RID: 18088
	public Renderer _renderer;

	// Token: 0x040046A9 RID: 18089
	public float tapLength = 1f;

	// Token: 0x040046AA RID: 18090
	[Space]
	private Material _instance;
}
