using System;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class RuntimeMaterialCombinerTargetMono : MonoBehaviour
{
	// Token: 0x060012D0 RID: 4816 RVA: 0x00063B2A File Offset: 0x00061D2A
	protected void Awake()
	{
		throw new NotImplementedException("// TODO: get the material combiner manager to fingerprint and combine these materials.");
	}

	// Token: 0x0400180A RID: 6154
	[HideInInspector]
	public GTSerializableDict<string, string>[] m_matSlot_to_texProp_to_texGuid;
}
