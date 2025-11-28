using System;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x040015C5 RID: 5573
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x040015C6 RID: 5574
	public Transform colorPoint;

	// Token: 0x020002BA RID: 698
	public enum ColorMode
	{
		// Token: 0x040015C8 RID: 5576
		None,
		// Token: 0x040015C9 RID: 5577
		Red,
		// Token: 0x040015CA RID: 5578
		Green,
		// Token: 0x040015CB RID: 5579
		Blue,
		// Token: 0x040015CC RID: 5580
		Black,
		// Token: 0x040015CD RID: 5581
		Clear
	}
}
