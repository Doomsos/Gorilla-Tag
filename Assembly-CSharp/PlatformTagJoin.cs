using System;
using UnityEngine;

// Token: 0x020002E9 RID: 745
[CreateAssetMenu(fileName = "PlatformTagJoin", menuName = "ScriptableObjects/PlatformTagJoin", order = 0)]
public class PlatformTagJoin : ScriptableObject
{
	// Token: 0x06001240 RID: 4672 RVA: 0x0005FF76 File Offset: 0x0005E176
	public override string ToString()
	{
		return this.PlatformTag;
	}

	// Token: 0x040016D8 RID: 5848
	public string PlatformTag = " ";
}
