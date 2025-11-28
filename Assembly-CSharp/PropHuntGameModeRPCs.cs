using System;

// Token: 0x0200020B RID: 523
internal class PropHuntGameModeRPCs : RPCNetworkBase
{
	// Token: 0x06000E68 RID: 3688 RVA: 0x0004C2B9 File Offset: 0x0004A4B9
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.propHuntManager = (GorillaPropHuntGameManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x04001167 RID: 4455
	private GameModeSerializer serializer;

	// Token: 0x04001168 RID: 4456
	private GorillaPropHuntGameManager propHuntManager;
}
