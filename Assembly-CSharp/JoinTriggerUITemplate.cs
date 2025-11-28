using System;
using UnityEngine;

// Token: 0x020002DA RID: 730
[CreateAssetMenu(fileName = "JoinTriggerUITemplate", menuName = "ScriptableObjects/JoinTriggerUITemplate")]
public class JoinTriggerUITemplate : ScriptableObject
{
	// Token: 0x0400166C RID: 5740
	public Material Milestone_Error;

	// Token: 0x0400166D RID: 5741
	public Material Milestone_AlreadyInRoom;

	// Token: 0x0400166E RID: 5742
	public Material Milestone_InPrivateRoom;

	// Token: 0x0400166F RID: 5743
	public Material Milestone_NotConnectedSoloJoin;

	// Token: 0x04001670 RID: 5744
	public Material Milestone_LeaveRoomAndSoloJoin;

	// Token: 0x04001671 RID: 5745
	public Material Milestone_LeaveRoomAndGroupJoin;

	// Token: 0x04001672 RID: 5746
	public Material Milestone_AbandonPartyAndSoloJoin;

	// Token: 0x04001673 RID: 5747
	public Material Milestone_ChangingGameModeSoloJoin;

	// Token: 0x04001674 RID: 5748
	public Material ScreenBG_Error;

	// Token: 0x04001675 RID: 5749
	public Material ScreenBG_AlreadyInRoom;

	// Token: 0x04001676 RID: 5750
	public Material ScreenBG_InPrivateRoom;

	// Token: 0x04001677 RID: 5751
	public Material ScreenBG_NotConnectedSoloJoin;

	// Token: 0x04001678 RID: 5752
	public Material ScreenBG_LeaveRoomAndSoloJoin;

	// Token: 0x04001679 RID: 5753
	public Material ScreenBG_LeaveRoomAndGroupJoin;

	// Token: 0x0400167A RID: 5754
	public Material ScreenBG_AbandonPartyAndSoloJoin;

	// Token: 0x0400167B RID: 5755
	public Material ScreenBG_ChangingGameModeSoloJoin;

	// Token: 0x0400167C RID: 5756
	public string ScreenText_Error;

	// Token: 0x0400167D RID: 5757
	public bool showFullErrorMessages;

	// Token: 0x0400167E RID: 5758
	public JoinTriggerUITemplate.FormattedString ScreenText_AlreadyInRoom;

	// Token: 0x0400167F RID: 5759
	public JoinTriggerUITemplate.FormattedString ScreenText_InPrivateRoom;

	// Token: 0x04001680 RID: 5760
	public JoinTriggerUITemplate.FormattedString ScreenText_NotConnectedSoloJoin;

	// Token: 0x04001681 RID: 5761
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndSoloJoin;

	// Token: 0x04001682 RID: 5762
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndGroupJoin;

	// Token: 0x04001683 RID: 5763
	public JoinTriggerUITemplate.FormattedString ScreenText_AbandonPartyAndSoloJoin;

	// Token: 0x04001684 RID: 5764
	public JoinTriggerUITemplate.FormattedString ScreenText_ChangingGameModeSoloJoin;

	// Token: 0x020002DB RID: 731
	[Serializable]
	public struct FormattedString
	{
		// Token: 0x060011EB RID: 4587 RVA: 0x0005E6E0 File Offset: 0x0005C8E0
		public string GetText(string oldZone, string newZone, string oldGameType, string newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(new string[]
			{
				oldZone,
				newZone,
				oldGameType,
				newGameType
			});
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0005E71D File Offset: 0x0005C91D
		public string GetText(Func<string> oldZone, Func<string> newZone, Func<string> oldGameType, Func<string> newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldZone, newZone, oldGameType, newGameType);
		}

		// Token: 0x04001685 RID: 5765
		[TextArea]
		[SerializeField]
		private string formatText;

		// Token: 0x04001686 RID: 5766
		[NonSerialized]
		private StringFormatter formatter;
	}
}
