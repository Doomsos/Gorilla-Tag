using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E22 RID: 3618
	public class VirtualStumpModeSelectButton : ModeSelectButton
	{
		// Token: 0x06005A60 RID: 23136 RVA: 0x001CF36C File Offset: 0x001CD56C
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			if (this.warningScreen.ShouldShowWarning)
			{
				this.warningScreen.Show();
			}
			else
			{
				GorillaComputer.instance.SetGameModeWithoutButton(this.gameMode);
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
			{
				if (GameMode.ActiveGameMode.IsNull())
				{
					GameMode.ChangeGameMode(this.gameMode);
					return;
				}
				if (GameMode.ActiveGameMode.GameType().ToString().ToLower() != this.gameMode.ToLower())
				{
					GameMode.ChangeGameMode(this.gameMode);
				}
			}
		}
	}
}
