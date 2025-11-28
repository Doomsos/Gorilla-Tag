using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000181 RID: 385
public class IDCardScanner : MonoBehaviour
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000A43 RID: 2627 RVA: 0x000374D4 File Offset: 0x000356D4
	// (remove) Token: 0x06000A44 RID: 2628 RVA: 0x0003750C File Offset: 0x0003570C
	public event IDCardScanner.CardSwipeEvent OnPlayerCardSwipe;

	// Token: 0x06000A45 RID: 2629 RVA: 0x00037544 File Offset: 0x00035744
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ScannableIDCard>() != null)
		{
			UnityEvent unityEvent = this.onCardSwiped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			GameEntity component = other.GetComponent<GameEntity>();
			if (component == null && other.attachedRigidbody != null)
			{
				component = other.attachedRigidbody.GetComponent<GameEntity>();
			}
			if (component != null && component.heldByActorNumber != -1)
			{
				bool flag = !this.requireSpecificPlayer || (this.restrictToPlayer != null && this.restrictToPlayer.ActorNumber == component.heldByActorNumber && component.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
				bool flag2 = !this.requireAuthority || component.manager.IsAuthority();
				if (flag && flag2)
				{
					UnityEvent<int> unityEvent2 = this.onCardSwipedByPlayer;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(component.heldByActorNumber);
					}
					IDCardScanner.CardSwipeEvent onPlayerCardSwipe = this.OnPlayerCardSwipe;
					if (onPlayerCardSwipe == null)
					{
						return;
					}
					onPlayerCardSwipe(component.heldByActorNumber);
				}
			}
		}
	}

	// Token: 0x04000C82 RID: 3202
	public UnityEvent onCardSwiped;

	// Token: 0x04000C83 RID: 3203
	public UnityEvent<int> onCardSwipedByPlayer;

	// Token: 0x04000C84 RID: 3204
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onSucceeded;

	// Token: 0x04000C85 RID: 3205
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onFailed;

	// Token: 0x04000C86 RID: 3206
	public bool requireSpecificPlayer;

	// Token: 0x04000C87 RID: 3207
	public bool requireAuthority;

	// Token: 0x04000C88 RID: 3208
	[NonSerialized]
	public NetPlayer restrictToPlayer;

	// Token: 0x02000182 RID: 386
	// (Invoke) Token: 0x06000A48 RID: 2632
	public delegate void CardSwipeEvent(int actorNumber);
}
