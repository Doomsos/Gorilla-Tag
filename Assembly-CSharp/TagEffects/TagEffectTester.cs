using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F7B RID: 3963
	public class TagEffectTester : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06006308 RID: 25352 RVA: 0x001FEA4B File Offset: 0x001FCC4B
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06006309 RID: 25353 RVA: 0x001FEA53 File Offset: 0x001FCC53
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x0600630A RID: 25354 RVA: 0x001FEA5B File Offset: 0x001FCC5B
		public Transform Transform { get; }

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x0600630B RID: 25355 RVA: 0x000743B1 File Offset: 0x000725B1
		public VRRig Rig
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x0600630C RID: 25356 RVA: 0x001FEA63 File Offset: 0x001FCC63
		public bool FingersDown { get; }

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x0600630D RID: 25357 RVA: 0x001FEA6B File Offset: 0x001FCC6B
		public bool FingersUp { get; }

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x0600630E RID: 25358 RVA: 0x001FEA73 File Offset: 0x001FCC73
		public Vector3 Velocity { get; }

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x0600630F RID: 25359 RVA: 0x001FEA7B File Offset: 0x001FCC7B
		// (set) Token: 0x06006310 RID: 25360 RVA: 0x001FEA83 File Offset: 0x001FCC83
		public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06006311 RID: 25361 RVA: 0x001FEA8C File Offset: 0x001FCC8C
		public bool RightHand { get; }

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06006312 RID: 25362 RVA: 0x001FEA94 File Offset: 0x001FCC94
		public float Magnitude { get; }

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06006313 RID: 25363 RVA: 0x001FEA9C File Offset: 0x001FCC9C
		public TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x06006314 RID: 25364 RVA: 0x00002789 File Offset: 0x00000989
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
		}

		// Token: 0x06006315 RID: 25365 RVA: 0x00002076 File Offset: 0x00000276
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return false;
		}

		// Token: 0x040071BE RID: 29118
		[SerializeField]
		private bool isStatic = true;
	}
}
