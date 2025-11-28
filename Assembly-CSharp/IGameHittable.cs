using System;

// Token: 0x0200061D RID: 1565
public interface IGameHittable
{
	// Token: 0x060027D2 RID: 10194
	bool IsHitValid(GameHitData hit);

	// Token: 0x060027D3 RID: 10195
	void OnHit(GameHitData hit);
}
