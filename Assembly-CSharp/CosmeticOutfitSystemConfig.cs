using System;
using UnityEngine;

// Token: 0x020004C8 RID: 1224
[CreateAssetMenu(fileName = "CosmeticOutfitSystemConfig", menuName = "Gorilla Tag/Cosmetics/OutfitSystem", order = 0)]
public class CosmeticOutfitSystemConfig : ScriptableObject
{
	// Token: 0x040029EE RID: 10734
	public int maxOutfits;

	// Token: 0x040029EF RID: 10735
	public string mothershipKey;

	// Token: 0x040029F0 RID: 10736
	public char outfitSeparator;

	// Token: 0x040029F1 RID: 10737
	public char itemSeparator;

	// Token: 0x040029F2 RID: 10738
	public string selectedOutfitPref;
}
