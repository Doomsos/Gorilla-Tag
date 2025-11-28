using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006E9 RID: 1769
public class GRProgressionScriptableObject : ScriptableObject
{
	// Token: 0x04003AF6 RID: 15094
	[SerializeField]
	[Header("Progression Tiers")]
	public List<GRPlayer.ProgressionLevels> progressionData;
}
