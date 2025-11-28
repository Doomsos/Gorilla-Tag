using System;

// Token: 0x02000AC6 RID: 2758
internal struct PlayerAgeGateWarningStatus
{
	// Token: 0x040056C4 RID: 22212
	public string header;

	// Token: 0x040056C5 RID: 22213
	public string body;

	// Token: 0x040056C6 RID: 22214
	public string leftButtonText;

	// Token: 0x040056C7 RID: 22215
	public string rightButtonText;

	// Token: 0x040056C8 RID: 22216
	public WarningButtonResult leftButtonResult;

	// Token: 0x040056C9 RID: 22217
	public WarningButtonResult rightButtonResult;

	// Token: 0x040056CA RID: 22218
	public WarningButtonResult noWarningResult;

	// Token: 0x040056CB RID: 22219
	public EImageVisibility showImage;

	// Token: 0x040056CC RID: 22220
	public Action onLeftButtonPressedAction;

	// Token: 0x040056CD RID: 22221
	public Action onRightButtonPressedAction;
}
