using System;
using UnityEngine;

// Token: 0x0200056D RID: 1389
public class BuilderBumpGlow : MonoBehaviour
{
	// Token: 0x060022EC RID: 8940 RVA: 0x000B6770 File Offset: 0x000B4970
	public void Awake()
	{
		this.blendIn = 1f;
		this.intensity = 0f;
		this.UpdateRender();
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000B678E File Offset: 0x000B498E
	public void SetIntensity(float intensity)
	{
		this.intensity = intensity;
		this.UpdateRender();
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000B679D File Offset: 0x000B499D
	public void SetBlendIn(float blendIn)
	{
		this.blendIn = blendIn;
		this.UpdateRender();
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x00002789 File Offset: 0x00000989
	private void UpdateRender()
	{
	}

	// Token: 0x04002DAF RID: 11695
	public MeshRenderer glowRenderer;

	// Token: 0x04002DB0 RID: 11696
	private float blendIn;

	// Token: 0x04002DB1 RID: 11697
	private float intensity;
}
