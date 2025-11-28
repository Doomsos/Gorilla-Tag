using System;
using UnityEngine;

// Token: 0x02000C2A RID: 3114
public class BetterBaker : MonoBehaviour
{
	// Token: 0x04005C58 RID: 23640
	public string bakeryLightmapDirectory;

	// Token: 0x04005C59 RID: 23641
	public string dayNightLightmapsDirectory;

	// Token: 0x04005C5A RID: 23642
	public GameObject[] allLights;

	// Token: 0x02000C2B RID: 3115
	public struct LightMapMap
	{
		// Token: 0x04005C5B RID: 23643
		public string timeOfDayName;

		// Token: 0x04005C5C RID: 23644
		public GameObject lightObject;
	}
}
