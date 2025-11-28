using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F78 RID: 3960
	[Serializable]
	public class ModeTagEffect
	{
		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x060062FE RID: 25342 RVA: 0x001FE9C6 File Offset: 0x001FCBC6
		public HashSet<GameModeType> Modes
		{
			get
			{
				if (this.modesHash == null)
				{
					this.modesHash = new HashSet<GameModeType>(this.modes);
				}
				return this.modesHash;
			}
		}

		// Token: 0x040071B7 RID: 29111
		[SerializeField]
		private GameModeType[] modes;

		// Token: 0x040071B8 RID: 29112
		private HashSet<GameModeType> modesHash;

		// Token: 0x040071B9 RID: 29113
		public TagEffectPack tagEffect;

		// Token: 0x040071BA RID: 29114
		public bool blockTagOverride;

		// Token: 0x040071BB RID: 29115
		public bool blockFistBumpOverride;

		// Token: 0x040071BC RID: 29116
		public bool blockHiveFiveOverride;
	}
}
