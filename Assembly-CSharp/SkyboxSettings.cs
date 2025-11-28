using System;
using UnityEngine;

// Token: 0x02000317 RID: 791
[ExecuteInEditMode]
public class SkyboxSettings : MonoBehaviour
{
	// Token: 0x06001347 RID: 4935 RVA: 0x0006FA82 File Offset: 0x0006DC82
	private void OnEnable()
	{
		if (this._skyMaterial)
		{
			RenderSettings.skybox = this._skyMaterial;
		}
	}

	// Token: 0x04001CCB RID: 7371
	[SerializeField]
	private Material _skyMaterial;
}
