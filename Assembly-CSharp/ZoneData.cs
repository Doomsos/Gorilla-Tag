using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
[Serializable]
public class ZoneData
{
	// Token: 0x04001E73 RID: 7795
	public GTZone zone;

	// Token: 0x04001E74 RID: 7796
	public string sceneName;

	// Token: 0x04001E75 RID: 7797
	public float CameraFarClipPlane = 500f;

	// Token: 0x04001E76 RID: 7798
	public GameObject[] rootGameObjects;

	// Token: 0x04001E77 RID: 7799
	[NonSerialized]
	public bool active;
}
