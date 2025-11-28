using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000EAB RID: 3755
	internal static class EditorOnlyScripts
	{
		// Token: 0x06005DC7 RID: 24007 RVA: 0x00002789 File Offset: 0x00000989
		[Conditional("UNITY_EDITOR")]
		public static void Cleanup(GameObject[] rootObjects, bool force = false)
		{
		}
	}
}
