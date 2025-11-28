using System;
using UnityEngine;

// Token: 0x02000257 RID: 599
[Serializable]
public class GestureNode
{
	// Token: 0x04001332 RID: 4914
	public bool track;

	// Token: 0x04001333 RID: 4915
	public GestureHandState state;

	// Token: 0x04001334 RID: 4916
	public GestureDigitFlexion flexion;

	// Token: 0x04001335 RID: 4917
	public GestureAlignment alignment;

	// Token: 0x04001336 RID: 4918
	[Space]
	public GestureNodeFlags flags;
}
