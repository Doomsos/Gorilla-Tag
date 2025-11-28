using System;
using System.Collections.Generic;
using GorillaTag.Rendering.Shaders;
using UnityEngine;

// Token: 0x02000C2C RID: 3116
public class BetterBakerBakeMe : FlagForBaking
{
	// Token: 0x04005C5D RID: 23645
	public GameObject[] stuffIncludingParentsToBake;

	// Token: 0x04005C5E RID: 23646
	public GameObject getMatStuffFromHere;

	// Token: 0x04005C5F RID: 23647
	public List<ShaderConfigData.ShaderConfig> allConfigs = new List<ShaderConfigData.ShaderConfig>();
}
