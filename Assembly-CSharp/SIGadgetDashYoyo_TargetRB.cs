using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class SIGadgetDashYoyo_TargetRB : MonoBehaviour
{
	// Token: 0x06000512 RID: 1298 RVA: 0x00002789 File Offset: 0x00000989
	protected void OnEnable()
	{
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001DC04 File Offset: 0x0001BE04
	protected void OnTriggerEnter(Collider otherCollider)
	{
		if (base.isActiveAndEnabled && this.gadget.gameEntity.IsAuthority() && (this.gadget.gameEntity.heldByActorNumber != -1 || this.gadget.gameEntity.snappedByActorNumber != -1) && (otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) || otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider)) && !ApplicationQuittingState.IsQuitting)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				VRRig componentInParent = otherCollider.GetComponentInParent<VRRig>();
				if (componentInParent == null)
				{
					return;
				}
				NetPlayer creator = componentInParent.creator;
				if (creator == null)
				{
					return;
				}
				if (SuperInfectionManager.GetSIManagerForZone(this.gadget.gameEntity.manager.zone) == null)
				{
					return;
				}
				this.gadget.OnHitPlayer_Authority(superInfectionGame, creator);
				return;
			}
		}
	}

	// Token: 0x04000633 RID: 1587
	[SerializeField]
	private SIGadgetDashYoyo gadget;
}
