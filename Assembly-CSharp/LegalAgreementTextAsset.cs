using System;
using UnityEngine;

// Token: 0x02000ADA RID: 2778
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x0400573C RID: 22332
	public string title;

	// Token: 0x0400573D RID: 22333
	public string playFabKey;

	// Token: 0x0400573E RID: 22334
	public string latestVersionKey;

	// Token: 0x0400573F RID: 22335
	[TextArea(3, 5)]
	public string errorMessage;

	// Token: 0x04005740 RID: 22336
	public bool optional;

	// Token: 0x04005741 RID: 22337
	public LegalAgreementTextAsset.PostAcceptAction optInAction;

	// Token: 0x04005742 RID: 22338
	public string confirmString;

	// Token: 0x02000ADB RID: 2779
	public enum PostAcceptAction
	{
		// Token: 0x04005744 RID: 22340
		NONE
	}
}
