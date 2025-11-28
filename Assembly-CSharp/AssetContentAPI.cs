using System;
using UnityEngine;

// Token: 0x02000CA4 RID: 3236
public class AssetContentAPI : ScriptableObject
{
	// Token: 0x04005DA9 RID: 23977
	public string bundleName;

	// Token: 0x04005DAA RID: 23978
	public LazyLoadReference<TextAsset> bundleFile;

	// Token: 0x04005DAB RID: 23979
	public Object[] assets = new Object[0];
}
