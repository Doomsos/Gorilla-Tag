using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
[Serializable]
internal class SoundIdRemapping
{
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000D5 RID: 213 RVA: 0x00005905 File Offset: 0x00003B05
	public int SoundIn
	{
		get
		{
			return this.soundIn;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060000D6 RID: 214 RVA: 0x0000590D File Offset: 0x00003B0D
	public int SoundOut
	{
		get
		{
			return this.soundOut;
		}
	}

	// Token: 0x040000ED RID: 237
	[GorillaSoundLookup]
	[SerializeField]
	private int soundIn = 1;

	// Token: 0x040000EE RID: 238
	[GorillaSoundLookup]
	[SerializeField]
	private int soundOut = 2;
}
