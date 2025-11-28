using System;

// Token: 0x020001F8 RID: 504
public interface GorillaQuestManager
{
	// Token: 0x06000DBF RID: 3519
	void LoadQuestsFromJson(string jsonString);

	// Token: 0x06000DC0 RID: 3520
	void LoadQuestProgress();

	// Token: 0x06000DC1 RID: 3521
	void SaveQuestProgress();

	// Token: 0x06000DC2 RID: 3522
	void SetupAllQuestEventListeners();

	// Token: 0x06000DC3 RID: 3523
	void ClearAllQuestEventListeners();

	// Token: 0x06000DC4 RID: 3524
	void HandleQuestProgressChanged(bool initialLoad);

	// Token: 0x06000DC5 RID: 3525
	void HandleQuestCompleted(int questID);
}
