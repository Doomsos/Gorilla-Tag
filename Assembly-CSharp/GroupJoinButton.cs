using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x020007F0 RID: 2032
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06003564 RID: 13668 RVA: 0x00121F02 File Offset: 0x00120102
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06003565 RID: 13669 RVA: 0x00121F2A File Offset: 0x0012012A
	public void Update()
	{
		this.inPrivate = (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible);
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x0400449D RID: 17565
	public int gameModeIndex;

	// Token: 0x0400449E RID: 17566
	public GorillaFriendCollider friendCollider;

	// Token: 0x0400449F RID: 17567
	public bool inPrivate;
}
