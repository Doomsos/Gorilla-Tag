using System;
using TMPro;
using UnityEngine;

// Token: 0x02000501 RID: 1281
public class GorillaDebugUI : MonoBehaviour
{
	// Token: 0x04002B6D RID: 11117
	private readonly float Delay = 0.5f;

	// Token: 0x04002B6E RID: 11118
	public GameObject parentCanvas;

	// Token: 0x04002B6F RID: 11119
	public GameObject rayInteractorLeft;

	// Token: 0x04002B70 RID: 11120
	public GameObject rayInteractorRight;

	// Token: 0x04002B71 RID: 11121
	[SerializeField]
	private TMP_Dropdown playfabIdDropdown;

	// Token: 0x04002B72 RID: 11122
	[SerializeField]
	private TMP_Dropdown roomIdDropdown;

	// Token: 0x04002B73 RID: 11123
	[SerializeField]
	private TMP_Dropdown locationDropdown;

	// Token: 0x04002B74 RID: 11124
	[SerializeField]
	private TMP_Dropdown playerNameDropdown;

	// Token: 0x04002B75 RID: 11125
	[SerializeField]
	private TMP_Dropdown gameModeDropdown;

	// Token: 0x04002B76 RID: 11126
	[SerializeField]
	private TMP_Dropdown timeOfDayDropdown;

	// Token: 0x04002B77 RID: 11127
	[SerializeField]
	private TMP_Text networkStateTextBox;

	// Token: 0x04002B78 RID: 11128
	[SerializeField]
	private TMP_Text gameModeTextBox;

	// Token: 0x04002B79 RID: 11129
	[SerializeField]
	private TMP_Text currentRoomTextBox;

	// Token: 0x04002B7A RID: 11130
	[SerializeField]
	private TMP_Text playerCountTextBox;

	// Token: 0x04002B7B RID: 11131
	[SerializeField]
	private TMP_Text roomVisibilityTextBox;

	// Token: 0x04002B7C RID: 11132
	[SerializeField]
	private TMP_Text timeMultiplierTextBox;

	// Token: 0x04002B7D RID: 11133
	[SerializeField]
	private TMP_Text versionTextBox;
}
