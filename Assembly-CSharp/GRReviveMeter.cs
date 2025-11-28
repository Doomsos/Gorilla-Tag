using System;
using UnityEngine;

// Token: 0x020006F0 RID: 1776
public class GRReviveMeter : MonoBehaviourTick
{
	// Token: 0x06002D8C RID: 11660 RVA: 0x00002789 File Offset: 0x00000989
	public void Awake()
	{
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x000F6480 File Offset: 0x000F4680
	public override void Tick()
	{
		float num = 0f;
		if (this.reviveStation != null && VRRig.LocalRig.OwningNetPlayer != null && this.reviveStation.GetReviveCooldownSeconds() > 0.0)
		{
			num = (float)this.reviveStation.CalculateRemainingReviveCooldownSeconds(VRRig.LocalRig.OwningNetPlayer.ActorNumber) / (float)this.reviveStation.GetReviveCooldownSeconds();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		num = 1f - num;
		this.meter.localScale = new Vector3(1f, num, 1f);
	}

	// Token: 0x04003B3E RID: 15166
	[SerializeField]
	private GRReviveStation reviveStation;

	// Token: 0x04003B3F RID: 15167
	[SerializeField]
	private Transform meter;
}
