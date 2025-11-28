using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x0200095A RID: 2394
public class CustomMapsSearchBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06003D3D RID: 15677 RVA: 0x00145398 File Offset: 0x00143598
	public CustomMapsSearchBehaviour(CustomMapsAIBehaviourController AIcontroller, AIAgent agentSettings)
	{
		this.sightOffset = agentSettings.sightOffset;
		this.sightDist = agentSettings.sightDist;
		this.sightDistSq = this.sightDist * this.sightDist;
		this.sightFOV = agentSettings.sightFOV;
		this.sightMinDot = Mathf.Cos(this.sightFOV / 2f * 0.017453292f);
		this.controller = AIcontroller;
	}

	// Token: 0x06003D3E RID: 15678 RVA: 0x00145406 File Offset: 0x00143606
	public override bool CanExecute()
	{
		return !this.controller.IsNull();
	}

	// Token: 0x06003D3F RID: 15679 RVA: 0x00145418 File Offset: 0x00143618
	public override bool CanContinueExecuting()
	{
		return this.CanExecute() && this.controller.TargetPlayer == null;
	}

	// Token: 0x06003D40 RID: 15680 RVA: 0x00145438 File Offset: 0x00143638
	public override void Execute()
	{
		if (Time.time < this.lastSearchTime + 0.1f)
		{
			return;
		}
		this.lastSearchTime = Time.time;
		Vector3 sourcePos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		this.controller.SetTarget(this.controller.FindBestTarget(sourcePos, this.sightDist, this.sightDistSq, this.sightMinDot));
	}

	// Token: 0x06003D41 RID: 15681 RVA: 0x00002789 File Offset: 0x00000989
	public override void NetExecute()
	{
	}

	// Token: 0x06003D42 RID: 15682 RVA: 0x00002789 File Offset: 0x00000989
	public override void ResetBehavior()
	{
	}

	// Token: 0x06003D43 RID: 15683 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnTriggerEnter(Collider otherCollider)
	{
	}

	// Token: 0x04004E03 RID: 19971
	private const float SEARCH_COOLDOWN = 0.1f;

	// Token: 0x04004E04 RID: 19972
	private CustomMapsAIBehaviourController controller;

	// Token: 0x04004E05 RID: 19973
	private float sightDist;

	// Token: 0x04004E06 RID: 19974
	private float sightDistSq;

	// Token: 0x04004E07 RID: 19975
	private Vector3 sightOffset;

	// Token: 0x04004E08 RID: 19976
	private float sightFOV;

	// Token: 0x04004E09 RID: 19977
	private float sightMinDot;

	// Token: 0x04004E0A RID: 19978
	private float lastSearchTime;
}
