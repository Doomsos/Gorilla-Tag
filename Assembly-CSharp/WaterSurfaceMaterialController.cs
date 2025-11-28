using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	// Token: 0x060009A0 RID: 2464 RVA: 0x00033CC5 File Offset: 0x00031EC5
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00033CE4 File Offset: 0x00031EE4
	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(ShaderProps._ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		if (this.renderer)
		{
			this.renderer.SetPropertyBlock(this.matPropBlock);
		}
	}

	// Token: 0x04000BBD RID: 3005
	public float ScrollX = 0.6f;

	// Token: 0x04000BBE RID: 3006
	public float ScrollY = 0.6f;

	// Token: 0x04000BBF RID: 3007
	public float Scale = 1f;

	// Token: 0x04000BC0 RID: 3008
	private Renderer renderer;

	// Token: 0x04000BC1 RID: 3009
	private MaterialPropertyBlock matPropBlock;
}
