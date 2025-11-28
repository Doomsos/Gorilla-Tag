using System;

// Token: 0x02000562 RID: 1378
public class BuilderRendererPreRender : MonoBehaviourPostTick
{
	// Token: 0x060022C7 RID: 8903 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000B5D29 File Offset: 0x000B3F29
	public override void PostTick()
	{
		if (this.builderRenderer != null)
		{
			this.builderRenderer.PreRenderIndirect();
		}
	}

	// Token: 0x04002D6E RID: 11630
	public BuilderRenderer builderRenderer;
}
