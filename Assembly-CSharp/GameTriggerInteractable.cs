using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000635 RID: 1589
[RequireComponent(typeof(GameEntity))]
public class GameTriggerInteractable : MonoBehaviour
{
	// Token: 0x0600287F RID: 10367 RVA: 0x000D78E8 File Offset: 0x000D5AE8
	private void OnEnable()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		if (this.interactableWhileGrabbed)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartHolding));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.StopHolding));
		}
		if (this.interactableWhileSnapped)
		{
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.StartHolding));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopHolding));
		}
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000D79BB File Offset: 0x000D5BBB
	public void StartHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.AddIfNew(this);
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000D79C8 File Offset: 0x000D5BC8
	public void StopHolding()
	{
		GameTriggerInteractable.LocalInteractableTriggers.RemoveIfContains(this);
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000D79D8 File Offset: 0x000D5BD8
	public bool PointWithinInteractableArea(Vector3 point)
	{
		return (this.interactableCenter.position - point).magnitude < this.interactableRadius;
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000D7A06 File Offset: 0x000D5C06
	public void BeginTriggerInteraction(int _handIndex)
	{
		this.triggerInteractionActive = true;
		this.handIndex = _handIndex;
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000D7A16 File Offset: 0x000D5C16
	public void EndTriggerInteraction()
	{
		this.triggerInteractionActive = false;
		this.handIndex = -1;
	}

	// Token: 0x040033F2 RID: 13298
	public GameEntity gameEntity;

	// Token: 0x040033F3 RID: 13299
	public Transform interactableCenter;

	// Token: 0x040033F4 RID: 13300
	public float interactableRadius;

	// Token: 0x040033F5 RID: 13301
	public bool interactableWhileGrabbed;

	// Token: 0x040033F6 RID: 13302
	public bool interactableWhileSnapped;

	// Token: 0x040033F7 RID: 13303
	public bool interactablePermanently;

	// Token: 0x040033F8 RID: 13304
	public bool interactableOnOthers;

	// Token: 0x040033F9 RID: 13305
	public bool triggerInteractionActive;

	// Token: 0x040033FA RID: 13306
	public int handIndex = -1;

	// Token: 0x040033FB RID: 13307
	public static List<GameTriggerInteractable> LocalInteractableTriggers = new List<GameTriggerInteractable>();
}
