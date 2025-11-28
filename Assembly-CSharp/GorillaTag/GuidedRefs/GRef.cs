using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001036 RID: 4150
	public static class GRef
	{
		// Token: 0x060068D2 RID: 26834 RVA: 0x00222110 File Offset: 0x00220310
		[MethodImpl(256)]
		public static bool ShouldResolveNow(GRef.EResolveModes mode)
		{
			return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
		}

		// Token: 0x060068D3 RID: 26835 RVA: 0x00222121 File Offset: 0x00220321
		public static bool IsAnyResolveModeOn(GRef.EResolveModes mode)
		{
			return mode > GRef.EResolveModes.None;
		}

		// Token: 0x02001037 RID: 4151
		[Flags]
		public enum EResolveModes
		{
			// Token: 0x0400778E RID: 30606
			None = 0,
			// Token: 0x0400778F RID: 30607
			Runtime = 1,
			// Token: 0x04007790 RID: 30608
			SceneProcessing = 2
		}
	}
}
