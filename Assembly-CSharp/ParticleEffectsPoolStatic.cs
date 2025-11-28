using System;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class ParticleEffectsPoolStatic<T> : ParticleEffectsPool where T : ParticleEffectsPool
{
	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06001045 RID: 4165 RVA: 0x000554F3 File Offset: 0x000536F3
	public static T Instance
	{
		get
		{
			return ParticleEffectsPoolStatic<T>.gInstance;
		}
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000554FA File Offset: 0x000536FA
	protected override void OnPoolAwake()
	{
		if (ParticleEffectsPoolStatic<T>.gInstance && ParticleEffectsPoolStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
			return;
		}
		ParticleEffectsPoolStatic<T>.gInstance = (this as T);
	}

	// Token: 0x0400143B RID: 5179
	protected static T gInstance;
}
