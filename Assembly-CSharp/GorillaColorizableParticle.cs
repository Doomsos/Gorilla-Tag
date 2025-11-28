using System;
using UnityEngine;

// Token: 0x02000776 RID: 1910
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x060031C7 RID: 12743 RVA: 0x0010DC18 File Offset: 0x0010BE18
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2;
		color2..ctor(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x0400404A RID: 16458
	public ParticleSystem particleSystem;

	// Token: 0x0400404B RID: 16459
	public float gradientColorPower = 2f;

	// Token: 0x0400404C RID: 16460
	public bool useLinearColor = true;
}
