using System;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class HandFXModifier : FXModifier
{
	// Token: 0x060011D6 RID: 4566 RVA: 0x0005E23F File Offset: 0x0005C43F
	private void Awake()
	{
		this.originalScale = base.transform.localScale;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0005E252 File Offset: 0x0005C452
	private void OnDisable()
	{
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0005E265 File Offset: 0x0005C465
	public override void UpdateScale(float scale, Color color)
	{
		scale = Mathf.Clamp(scale, this.minScale, this.maxScale);
		base.transform.localScale = this.originalScale * scale;
	}

	// Token: 0x04001653 RID: 5715
	private Vector3 originalScale;

	// Token: 0x04001654 RID: 5716
	[SerializeField]
	private float minScale;

	// Token: 0x04001655 RID: 5717
	[SerializeField]
	private float maxScale;

	// Token: 0x04001656 RID: 5718
	[SerializeField]
	private ParticleSystem dustBurst;

	// Token: 0x04001657 RID: 5719
	[SerializeField]
	private ParticleSystem dustLinger;
}
