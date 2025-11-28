using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F6F RID: 3951
	public interface IHandEffectsTrigger
	{
		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x060062D4 RID: 25300
		IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x060062D5 RID: 25301
		Transform Transform { get; }

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x060062D6 RID: 25302
		VRRig Rig { get; }

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x060062D7 RID: 25303
		bool FingersDown { get; }

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x060062D8 RID: 25304
		bool FingersUp { get; }

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x060062D9 RID: 25305
		Vector3 Velocity { get; }

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x060062DA RID: 25306
		// (set) Token: 0x060062DB RID: 25307
		Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x060062DC RID: 25308
		bool RightHand { get; }

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x060062DD RID: 25309
		TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x060062DE RID: 25310
		bool Static { get; }

		// Token: 0x060062DF RID: 25311
		void OnTriggerEntered(IHandEffectsTrigger other);

		// Token: 0x060062E0 RID: 25312
		bool InTriggerZone(IHandEffectsTrigger t);

		// Token: 0x02000F70 RID: 3952
		public enum Mode
		{
			// Token: 0x0400718D RID: 29069
			HighFive,
			// Token: 0x0400718E RID: 29070
			FistBump,
			// Token: 0x0400718F RID: 29071
			Tag3P,
			// Token: 0x04007190 RID: 29072
			Tag1P,
			// Token: 0x04007191 RID: 29073
			HighFive_And_FistBump
		}
	}
}
