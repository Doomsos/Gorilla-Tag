using System;
using UnityEngine;

// Token: 0x020004FF RID: 1279
public class DestroyIfNotBeta : MonoBehaviour
{
	// Token: 0x060020D6 RID: 8406 RVA: 0x000AE04D File Offset: 0x000AC24D
	private void Awake()
	{
		bool shouldKeepIfBeta = this.m_shouldKeepIfBeta;
		bool shouldKeepIfCreatorBuild = this.m_shouldKeepIfCreatorBuild;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04002B6B RID: 11115
	public bool m_shouldKeepIfBeta = true;

	// Token: 0x04002B6C RID: 11116
	public bool m_shouldKeepIfCreatorBuild;
}
