using System;

// Token: 0x02000229 RID: 553
public interface IGameStateReceiver
{
	// Token: 0x06000ECE RID: 3790
	void GameStateReceiverOnStateChanged(long oldState, long newState);
}
