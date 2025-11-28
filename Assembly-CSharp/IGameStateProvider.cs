using System;

// Token: 0x02000228 RID: 552
public interface IGameStateProvider
{
	// Token: 0x06000ECC RID: 3788
	void GameStateReceiverRegister(IGameStateReceiver receiver);

	// Token: 0x06000ECD RID: 3789
	void GameStateReceiverUnregister(IGameStateReceiver receiver);
}
