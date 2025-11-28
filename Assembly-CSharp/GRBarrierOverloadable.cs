using System;
using UnityEngine;

// Token: 0x02000687 RID: 1671
public class GRBarrierOverloadable : MonoBehaviour
{
	// Token: 0x06002ABC RID: 10940 RVA: 0x000E6095 File Offset: 0x000E4295
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x000E60C8 File Offset: 0x000E42C8
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		if (this.state == GRBarrierOverloadable.State.Active && tool.energy >= tool.GetEnergyMax())
		{
			this.SetState(GRBarrierOverloadable.State.Destroyed);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000E6117 File Offset: 0x000E4317
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRBarrierOverloadable.State)nextState);
		}
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000E6130 File Offset: 0x000E4330
	public void SetState(GRBarrierOverloadable.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRBarrierOverloadable.State state = this.state;
			if (state == GRBarrierOverloadable.State.Active)
			{
				this.meshRenderer.enabled = true;
				this.collider.enabled = true;
				return;
			}
			if (state != GRBarrierOverloadable.State.Destroyed)
			{
				return;
			}
			this.audioSource.Play();
			this.meshRenderer.enabled = false;
			this.collider.enabled = false;
		}
	}

	// Token: 0x04003723 RID: 14115
	public GRTool tool;

	// Token: 0x04003724 RID: 14116
	public GameEntity gameEntity;

	// Token: 0x04003725 RID: 14117
	public AudioSource audioSource;

	// Token: 0x04003726 RID: 14118
	public MeshRenderer meshRenderer;

	// Token: 0x04003727 RID: 14119
	public Collider collider;

	// Token: 0x04003728 RID: 14120
	private GRBarrierOverloadable.State state;

	// Token: 0x02000688 RID: 1672
	public enum State
	{
		// Token: 0x0400372A RID: 14122
		Active,
		// Token: 0x0400372B RID: 14123
		Destroyed
	}
}
