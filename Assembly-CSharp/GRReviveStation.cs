using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020006F1 RID: 1777
public class GRReviveStation : MonoBehaviour
{
	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x06002D8F RID: 11663 RVA: 0x000F6500 File Offset: 0x000F4700
	// (set) Token: 0x06002D90 RID: 11664 RVA: 0x000F6508 File Offset: 0x000F4708
	public int Index { get; set; }

	// Token: 0x06002D91 RID: 11665 RVA: 0x000F6511 File Offset: 0x000F4711
	public void Init(GhostReactor reactor, int index)
	{
		this.reactor = reactor;
		this.Index = index;
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x000F6521 File Offset: 0x000F4721
	public void SetReviveCooldownSeconds(double seconds)
	{
		this.reviveCooldownSeconds = seconds;
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x000F652A File Offset: 0x000F472A
	public double GetReviveCooldownSeconds()
	{
		return this.reviveCooldownSeconds;
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x000F6534 File Offset: 0x000F4734
	public double CalculateRemainingReviveCooldownSeconds(int ActorNumber)
	{
		if (this.reviveCooldownSeconds == 0.0)
		{
			return 0.0;
		}
		if (this.cooldownStartTime.ContainsKey(ActorNumber))
		{
			return this.reviveCooldownSeconds - (GorillaComputer.instance.GetServerTime() - this.cooldownStartTime[ActorNumber]).TotalSeconds;
		}
		return 0.0;
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x000F65A0 File Offset: 0x000F47A0
	public void RevivePlayer(GRPlayer player)
	{
		if (player != null)
		{
			int actorNumber = player.gamePlayer.rig.OwningNetPlayer.ActorNumber;
			this.cooldownStartTime[actorNumber] = GorillaComputer.instance.GetServerTime();
			if (player.State != GRPlayer.GRPlayerState.Alive || player.Hp < player.MaxHp)
			{
				player.OnPlayerRevive(this.reactor.grManager);
				if (this.audioSource != null)
				{
					this.audioSource.Play();
				}
				if (this.particleEffects != null)
				{
					for (int i = 0; i < this.particleEffects.Length; i++)
					{
						this.particleEffects[i].Play();
					}
				}
			}
		}
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x000F6650 File Offset: 0x000F4850
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
						this.RevivePlayer(component2);
					}
					if (this.reactor.grManager.IsAuthority() && this.CalculateRemainingReviveCooldownSeconds(component2.gamePlayer.rig.OwningNetPlayer.ActorNumber) <= 0.0)
					{
						this.reactor.grManager.RequestPlayerRevive(this, component2);
					}
				}
			}
		}
	}

	// Token: 0x04003B40 RID: 15168
	public AudioSource audioSource;

	// Token: 0x04003B41 RID: 15169
	public ParticleSystem[] particleEffects;

	// Token: 0x04003B42 RID: 15170
	[SerializeField]
	private double reviveCooldownSeconds;

	// Token: 0x04003B43 RID: 15171
	private Dictionary<int, DateTime> cooldownStartTime = new Dictionary<int, DateTime>();

	// Token: 0x04003B45 RID: 15173
	private GhostReactor reactor;
}
