using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000566 RID: 1382
[CreateAssetMenu(fileName = "BuilderTableSerializationConfig", menuName = "Gorilla Tag/Builder/Serialization", order = 0)]
public class BuilderTableSerializationConfig : ScriptableObject
{
	// Token: 0x04002D7C RID: 11644
	public string tableConfigurationKey;

	// Token: 0x04002D7D RID: 11645
	public string titleDataKey;

	// Token: 0x04002D7E RID: 11646
	public string startingMapConfigKey;

	// Token: 0x04002D7F RID: 11647
	public List<string> scanSlotMothershipKeys;

	// Token: 0x04002D80 RID: 11648
	public string scanSlotDevKey;

	// Token: 0x04002D81 RID: 11649
	public string publishedScanMothershipKey;

	// Token: 0x04002D82 RID: 11650
	public string timeAppend;

	// Token: 0x04002D83 RID: 11651
	public string playfabScanKey;

	// Token: 0x04002D84 RID: 11652
	public string sharedBlocksApiBaseURL;

	// Token: 0x04002D85 RID: 11653
	public string recentVotesPrefsKey;

	// Token: 0x04002D86 RID: 11654
	public string localMapsPrefsKey;
}
