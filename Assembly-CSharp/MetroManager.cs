using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class MetroManager : MonoBehaviour
{
	// Token: 0x06000A01 RID: 2561 RVA: 0x000363A0 File Offset: 0x000345A0
	private void Update()
	{
		for (int i = 0; i < this._blimps.Length; i++)
		{
			this._blimps[i].Tick();
		}
		for (int j = 0; j < this._spotlights.Length; j++)
		{
			this._spotlights[j].Tick();
		}
	}

	// Token: 0x04000C46 RID: 3142
	[SerializeField]
	private MetroBlimp[] _blimps = new MetroBlimp[0];

	// Token: 0x04000C47 RID: 3143
	[SerializeField]
	private MetroSpotlight[] _spotlights = new MetroSpotlight[0];

	// Token: 0x04000C48 RID: 3144
	[Space]
	[SerializeField]
	private Transform _blimpsRotationAnchor;
}
