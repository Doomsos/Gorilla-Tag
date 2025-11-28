using System;
using UnityEngine;

// Token: 0x02000393 RID: 915
public class ArcadeMachineButton : GorillaPressableButton
{
	// Token: 0x1400002F RID: 47
	// (add) Token: 0x060015E9 RID: 5609 RVA: 0x0007A8F8 File Offset: 0x00078AF8
	// (remove) Token: 0x060015EA RID: 5610 RVA: 0x0007A930 File Offset: 0x00078B30
	public event ArcadeMachineButton.ArcadeMachineButtonEvent OnStateChange;

	// Token: 0x060015EB RID: 5611 RVA: 0x0007A965 File Offset: 0x00078B65
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.state)
		{
			this.state = true;
			if (this.OnStateChange != null)
			{
				this.OnStateChange(this.ButtonID, this.state);
			}
		}
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x0007A99C File Offset: 0x00078B9C
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled || !this.state)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.state = false;
		if (this.OnStateChange != null)
		{
			this.OnStateChange(this.ButtonID, this.state);
		}
	}

	// Token: 0x0400203B RID: 8251
	private bool state;

	// Token: 0x0400203C RID: 8252
	[SerializeField]
	private int ButtonID;

	// Token: 0x02000394 RID: 916
	// (Invoke) Token: 0x060015EF RID: 5615
	public delegate void ArcadeMachineButtonEvent(int id, bool state);
}
