using System;

// Token: 0x0200057D RID: 1405
public interface IBuilderPieceFunctional
{
	// Token: 0x0600236A RID: 9066
	void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x0600236B RID: 9067
	void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x0600236C RID: 9068
	bool IsStateValid(byte state);

	// Token: 0x0600236D RID: 9069
	void FunctionalPieceUpdate();

	// Token: 0x0600236E RID: 9070 RVA: 0x000029BC File Offset: 0x00000BBC
	void FunctionalPieceFixedUpdate()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x000B9993 File Offset: 0x000B7B93
	float GetInteractionDistace()
	{
		return 2.5f;
	}
}
