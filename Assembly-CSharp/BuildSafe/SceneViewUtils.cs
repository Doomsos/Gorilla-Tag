using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000EB6 RID: 3766
	public static class SceneViewUtils
	{
		// Token: 0x06005DF4 RID: 24052 RVA: 0x001E1A55 File Offset: 0x001DFC55
		private static bool RaycastWorldSafe(Vector2 screenPos, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x04006BD8 RID: 27608
		public static readonly SceneViewUtils.FuncRaycastWorld RaycastWorld = new SceneViewUtils.FuncRaycastWorld(SceneViewUtils.RaycastWorldSafe);

		// Token: 0x02000EB7 RID: 3767
		// (Invoke) Token: 0x06005DF7 RID: 24055
		public delegate bool FuncRaycastWorld(Vector2 screenPos, out RaycastHit hit);

		// Token: 0x02000EB8 RID: 3768
		// (Invoke) Token: 0x06005DFB RID: 24059
		public delegate GameObject FuncPickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
	}
}
