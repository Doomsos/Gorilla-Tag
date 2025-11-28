using System;
using UnityEngine;

// Token: 0x020009B2 RID: 2482
public class VirtualStumpRevivePlayer : MonoBehaviour
{
	// Token: 0x06003F5E RID: 16222 RVA: 0x00154058 File Offset: 0x00152258
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && (component2.State != GRPlayer.GRPlayerState.Alive || component2.Hp < component2.MaxHp))
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.defaultReviveStation.RevivePlayer(component2);
					}
					if (this.ghostReactorManager.IsAuthority())
					{
						this.ghostReactorManager.RequestPlayerRevive(this.defaultReviveStation, component2);
					}
				}
			}
		}
	}

	// Token: 0x04005096 RID: 20630
	[SerializeField]
	private GhostReactorManager ghostReactorManager;

	// Token: 0x04005097 RID: 20631
	[SerializeField]
	private GRReviveStation defaultReviveStation;
}
