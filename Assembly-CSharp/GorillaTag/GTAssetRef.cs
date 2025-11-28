using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GorillaTag
{
	// Token: 0x02000FCF RID: 4047
	[Serializable]
	public class GTAssetRef<TObject> : AssetReferenceT<TObject> where TObject : Object
	{
		// Token: 0x0600669D RID: 26269 RVA: 0x00216DE0 File Offset: 0x00214FE0
		public GTAssetRef(string guid) : base(guid)
		{
		}
	}
}
