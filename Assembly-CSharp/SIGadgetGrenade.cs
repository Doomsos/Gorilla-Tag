using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public abstract class SIGadgetGrenade : SIGadget
{
	// Token: 0x0600058C RID: 1420 RVA: 0x000201F0 File Offset: 0x0001E3F0
	protected new virtual void OnEnable()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.activatedLocally = false;
		this.thrownGadget.OnActivated += new Action(this.HandleActivated);
		this.thrownGadget.OnThrown += new Action(this.HandleThrown);
		this.thrownGadget.OnHitSurface += new Action(this.HandleHitSurface);
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00020258 File Offset: 0x0001E458
	protected new virtual void OnDisable()
	{
		this.thrownGadget.OnActivated -= new Action(this.HandleActivated);
		this.thrownGadget.OnThrown -= new Action(this.HandleThrown);
		this.thrownGadget.OnHitSurface -= new Action(this.HandleHitSurface);
	}

	// Token: 0x0600058E RID: 1422
	protected abstract void HandleActivated();

	// Token: 0x0600058F RID: 1423
	protected abstract void HandleThrown();

	// Token: 0x06000590 RID: 1424
	protected abstract void HandleHitSurface();

	// Token: 0x06000591 RID: 1425 RVA: 0x000202B0 File Offset: 0x0001E4B0
	public override void OnEntityInit()
	{
		base.OnEntityInit();
		GameEntityId entityIdFromNetId = this.gameEntity.manager.GetEntityIdFromNetId((int)this.gameEntity.createData);
		this.parentEntity = this.gameEntity.manager.GetGameEntity(entityIdFromNetId);
		SIGadgetHolsterDisk component = this.parentEntity.GetComponent<SIGadgetHolsterDisk>();
		if (component != null)
		{
			component.RegisterGadget(this);
		}
	}

	// Token: 0x040006F1 RID: 1777
	public Action GrenadeFinished;

	// Token: 0x040006F2 RID: 1778
	public Renderer grenadeRenderer;

	// Token: 0x040006F3 RID: 1779
	[SerializeField]
	protected ThrownGadget thrownGadget;

	// Token: 0x040006F4 RID: 1780
	protected Rigidbody rb;

	// Token: 0x040006F5 RID: 1781
	protected GameEntity parentEntity;
}
