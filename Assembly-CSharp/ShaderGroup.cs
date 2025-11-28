using System;
using UnityEngine;

// Token: 0x02000CE8 RID: 3304
[Serializable]
public struct ShaderGroup
{
	// Token: 0x0600505B RID: 20571 RVA: 0x0019D7CC File Offset: 0x0019B9CC
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04005F93 RID: 24467
	public Material material;

	// Token: 0x04005F94 RID: 24468
	public Shader originalShader;

	// Token: 0x04005F95 RID: 24469
	public Shader gameplayShader;

	// Token: 0x04005F96 RID: 24470
	public Shader bakingShader;
}
