using System;

// Token: 0x0200027D RID: 637
public interface IProximityEffectReceiver
{
	// Token: 0x06001059 RID: 4185
	void OnProximityCalculated(float distance, float alignment, float parallel);
}
