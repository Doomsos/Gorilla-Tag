using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001008 RID: 4104
	[CreateAssetMenu(fileName = "GorillaButtonColorSettings", menuName = "ScriptableObjects/GorillaButtonColorSettings", order = 0)]
	public class ButtonColorSettings : ScriptableObject
	{
		// Token: 0x040076D9 RID: 30425
		public Color UnpressedColor;

		// Token: 0x040076DA RID: 30426
		public Color PressedColor;

		// Token: 0x040076DB RID: 30427
		[Tooltip("Optional\nThe time the change will be in effect")]
		public float PressedTime;
	}
}
