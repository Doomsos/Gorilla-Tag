using System;
using UnityEngine;

// Token: 0x02000309 RID: 777
public static class ShaderPlatformSetter
{
	// Token: 0x060012DD RID: 4829 RVA: 0x0006ABA1 File Offset: 0x00068DA1
	[RuntimeInitializeOnLoadMethod]
	public static void HandleRuntimeInitializeOnLoad()
	{
		Shader.DisableKeyword("PLATFORM_IS_ANDROID");
		Shader.DisableKeyword("QATESTING");
	}
}
