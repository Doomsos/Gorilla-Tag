using System;
using Fusion;
using Photon.Pun;

// Token: 0x0200000E RID: 14
[NetworkBehaviourWeaved(0)]
public class ArcadeMachineJoystickNetworkState : NetworkComponent
{
	// Token: 0x0600003A RID: 58 RVA: 0x000029AE File Offset: 0x00000BAE
	private new void Awake()
	{
		this.joystick = base.GetComponent<ArcadeMachineJoystick>();
	}

	// Token: 0x0600003B RID: 59 RVA: 0x000029BC File Offset: 0x00000BBC
	public override void ReadDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000029BC File Offset: 0x00000BBC
	public override void WriteDataFusion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002789 File Offset: 0x00000989
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000040 RID: 64 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000041 RID: 65 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04000026 RID: 38
	private ArcadeMachineJoystick joystick;
}
