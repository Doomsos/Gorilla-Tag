using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020006CD RID: 1741
public class GRHazardousMaterial : MonoBehaviour
{
	// Token: 0x06002CB2 RID: 11442 RVA: 0x000F268B File Offset: 0x000F088B
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x000F2694 File Offset: 0x000F0894
	public void OnLocalPlayerOverlap()
	{
		GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
		if (component != null && component.State == GRPlayer.GRPlayerState.Alive)
		{
			this.reactor.grManager.RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Ghost);
		}
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x000F26CF File Offset: 0x000F08CF
	private void OnTriggerEnter(Collider collider)
	{
		if (collider == GTPlayer.Instance.headCollider || collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x000F26FB File Offset: 0x000F08FB
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider == GTPlayer.Instance.headCollider || collision.collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x04003A0E RID: 14862
	private GhostReactor reactor;
}
