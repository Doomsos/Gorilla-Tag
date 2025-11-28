using System;
using UnityEngine;

// Token: 0x020006C9 RID: 1737
public class GRGameEntityInteractionPoint : MonoBehaviour
{
	// Token: 0x06002C9C RID: 11420 RVA: 0x000F1B13 File Offset: 0x000EFD13
	public void Start()
	{
		base.transform.parent = this.targetParent;
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000F1B28 File Offset: 0x000EFD28
	public void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000F1B84 File Offset: 0x000EFD84
	public void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000F1BDF File Offset: 0x000EFDDF
	public void OnGrabbed()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.TickWhileHeld));
		Action onGrabStart = this.OnGrabStart;
		if (onGrabStart == null)
		{
			return;
		}
		onGrabStart.Invoke();
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x000F1C18 File Offset: 0x000EFE18
	public void OnReleased()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.TickWhileHeld));
		this.gameEntity.transform.parent = this.targetParent;
		this.gameEntity.transform.localRotation = Quaternion.identity;
		this.gameEntity.transform.localPosition = Vector3.zero;
		this.OnGrabEnd.Invoke();
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x000F1C98 File Offset: 0x000EFE98
	public void TickWhileHeld()
	{
		if (this.targetParent != null)
		{
			Vector3 position = this.targetParent.transform.position;
			Vector3 position2 = base.transform.position;
			if (Vector3.Magnitude(position - position2) > this.autoReleaseDistance)
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
				if (gamePlayer != null)
				{
					gamePlayer.ClearGrabbedIfHeld(this.gameEntity.id);
				}
				if (gamePlayer != null && GamePlayerLocal.instance.gamePlayer == gamePlayer)
				{
					GamePlayerLocal.instance.ClearGrabbedIfHeld(this.gameEntity.id);
				}
				this.OnReleased();
				return;
			}
		}
		Action onGrabContinue = this.OnGrabContinue;
		if (onGrabContinue == null)
		{
			return;
		}
		onGrabContinue.Invoke();
	}

	// Token: 0x040039ED RID: 14829
	public GameEntity gameEntity;

	// Token: 0x040039EE RID: 14830
	public float autoReleaseDistance = 0.1f;

	// Token: 0x040039EF RID: 14831
	public Action OnGrabStart;

	// Token: 0x040039F0 RID: 14832
	public Action OnGrabContinue;

	// Token: 0x040039F1 RID: 14833
	public Action OnGrabEnd;

	// Token: 0x040039F2 RID: 14834
	public Transform targetParent;
}
