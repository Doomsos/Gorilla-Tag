using System;
using UnityEngine;

// Token: 0x02000322 RID: 802
public class ThermalSourceVolume : MonoBehaviour
{
	// Token: 0x0600137C RID: 4988 RVA: 0x000707CA File Offset: 0x0006E9CA
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x000707D2 File Offset: 0x0006E9D2
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x04001CF6 RID: 7414
	[Tooltip("Temperature in celsius. Default is 20 which is room temperature.")]
	public float celsius = 20f;

	// Token: 0x04001CF7 RID: 7415
	public float innerRadius = 0.1f;

	// Token: 0x04001CF8 RID: 7416
	public float outerRadius = 1f;
}
