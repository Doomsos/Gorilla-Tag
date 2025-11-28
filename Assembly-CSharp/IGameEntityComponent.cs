using System;

// Token: 0x02000603 RID: 1539
public interface IGameEntityComponent
{
	// Token: 0x060026CE RID: 9934
	void OnEntityInit();

	// Token: 0x060026CF RID: 9935
	void OnEntityDestroy();

	// Token: 0x060026D0 RID: 9936
	void OnEntityStateChange(long prevState, long newState);
}
