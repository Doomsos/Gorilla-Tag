using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
[Serializable]
public struct KeyValueStringPair
{
	// Token: 0x06000417 RID: 1047 RVA: 0x000181ED File Offset: 0x000163ED
	public KeyValueStringPair(string key, string value)
	{
		this.Key = key;
		this.Value = value;
	}

	// Token: 0x04000488 RID: 1160
	public string Key;

	// Token: 0x04000489 RID: 1161
	[Multiline]
	public string Value;
}
