using System;
using UnityEngine;

// Token: 0x02000C2F RID: 3119
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x04005C64 RID: 23652
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x02000C30 RID: 3120
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x04005C65 RID: 23653
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x04005C66 RID: 23654
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
