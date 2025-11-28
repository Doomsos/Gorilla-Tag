using System;

// Token: 0x02000664 RID: 1636
[Serializable]
public class AbilityHaptic
{
	// Token: 0x060029E2 RID: 10722 RVA: 0x000E2668 File Offset: 0x000E0868
	public void PlayIfHeldLocal(GameEntity gameEntity)
	{
		if (gameEntity == null || !gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), this.strength, this.duration);
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x000E26CC File Offset: 0x000E08CC
	public void PlayIfSnappedLocal(GameEntity gameEntity)
	{
		if (gameEntity == null || !gameEntity.IsSnappedByLocalPlayer())
		{
			return;
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component == null)
		{
			return;
		}
		if (component.IsSnappedToLeftArm())
		{
			GorillaTagger.Instance.StartVibration(true, this.strength, this.duration);
		}
		if (component.IsSnappedToRightArm())
		{
			GorillaTagger.Instance.StartVibration(false, this.strength, this.duration);
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), this.strength, this.duration);
	}

	// Token: 0x04003617 RID: 13847
	public float strength = 0.2f;

	// Token: 0x04003618 RID: 13848
	public float duration = 0.1f;
}
