using System;
using UnityEngine;

// Token: 0x020000A2 RID: 162
[CreateAssetMenu(fileName = "New KeyValuePairSet", menuName = "Data/KeyValuePairSet", order = 0)]
public class KeyValuePairSet : ScriptableObject
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000415 RID: 1045 RVA: 0x000181E5 File Offset: 0x000163E5
	public KeyValueStringPair[] Entries
	{
		get
		{
			return this.m_entries;
		}
	}

	// Token: 0x04000487 RID: 1159
	[SerializeField]
	private KeyValueStringPair[] m_entries;
}
