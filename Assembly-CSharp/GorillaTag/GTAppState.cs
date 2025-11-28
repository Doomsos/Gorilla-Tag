using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FD8 RID: 4056
	public static class GTAppState
	{
		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x060066C7 RID: 26311 RVA: 0x002173D8 File Offset: 0x002155D8
		// (set) Token: 0x060066C8 RID: 26312 RVA: 0x002173DF File Offset: 0x002155DF
		public static bool isQuitting { get; private set; }

		// Token: 0x060066C9 RID: 26313 RVA: 0x002173E8 File Offset: 0x002155E8
		[RuntimeInitializeOnLoadMethod(4)]
		private static void HandleOnSubsystemRegistration()
		{
			GTAppState.isQuitting = false;
			Application.quitting += delegate()
			{
				GTAppState.isQuitting = true;
			};
			Debug.Log(string.Concat(new string[]
			{
				"GTAppState:\n- SystemInfo.operatingSystem=",
				SystemInfo.operatingSystem,
				"\n- SystemInfo.maxTextureArraySlices=",
				SystemInfo.maxTextureArraySlices.ToString(),
				"\n"
			}));
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x00002789 File Offset: 0x00000989
		[RuntimeInitializeOnLoadMethod(0)]
		private static void HandleOnAfterSceneLoad()
		{
		}
	}
}
