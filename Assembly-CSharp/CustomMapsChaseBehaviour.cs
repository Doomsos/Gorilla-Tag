using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000959 RID: 2393
public class CustomMapsChaseBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06003D34 RID: 15668 RVA: 0x0014516C File Offset: 0x0014336C
	public CustomMapsChaseBehaviour(CustomMapsAIBehaviourController AIController, AIAgent agentSettings)
	{
		this.sightOffset = agentSettings.sightOffset;
		this.rememberLoseSightPos = agentSettings.rememberLoseSightPosition;
		this.loseSightDist = agentSettings.loseSightDist;
		this.loseSightDistSq = this.loseSightDist * this.loseSightDist;
		this.stopDistSq = agentSettings.stopDist * agentSettings.stopDist;
		this.controller = AIController;
	}

	// Token: 0x06003D35 RID: 15669 RVA: 0x001451D0 File Offset: 0x001433D0
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull();
	}

	// Token: 0x06003D36 RID: 15670 RVA: 0x001451F8 File Offset: 0x001433F8
	public override bool CanContinueExecuting()
	{
		if (!this.CanExecute())
		{
			return false;
		}
		bool flag;
		if (this.IsTargetInChaseRange(out flag))
		{
			return !flag;
		}
		if (!this.controller.IsTargetable(this.controller.TargetPlayer))
		{
			this.controller.StopMoving();
		}
		this.controller.ClearTarget();
		return false;
	}

	// Token: 0x06003D37 RID: 15671 RVA: 0x00145250 File Offset: 0x00143450
	public override void Execute()
	{
		bool flag;
		if (!this.IsTargetInChaseRange(out flag))
		{
			this.controller.ClearTarget();
			this.isChasing = false;
			if (!this.rememberLoseSightPos)
			{
				this.controller.StopMoving();
			}
			return;
		}
		if (!this.IsTargetVisible())
		{
			this.controller.ClearTarget();
			this.isChasing = false;
			if (!this.rememberLoseSightPos)
			{
				this.controller.StopMoving();
			}
			return;
		}
		if (flag && this.isChasing)
		{
			this.isChasing = false;
			this.controller.StopMoving();
			return;
		}
		this.isChasing = true;
		this.controller.RequestDestination(this.controller.TargetPlayer.transform.position);
	}

	// Token: 0x06003D38 RID: 15672 RVA: 0x00145300 File Offset: 0x00143500
	private bool IsTargetVisible()
	{
		Vector3 startPos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		return this.controller.IsTargetVisible(startPos, this.controller.TargetPlayer, this.loseSightDist);
	}

	// Token: 0x06003D39 RID: 15673 RVA: 0x00145358 File Offset: 0x00143558
	private bool IsTargetInChaseRange(out bool withinStopDist)
	{
		withinStopDist = false;
		Vector3 vector;
		if (!this.controller.IsTargetInRange(this.controller.transform.position, this.controller.TargetPlayer, this.loseSightDistSq, out vector))
		{
			return false;
		}
		if (vector.sqrMagnitude < this.stopDistSq)
		{
			withinStopDist = true;
		}
		return true;
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x00002789 File Offset: 0x00000989
	public override void NetExecute()
	{
	}

	// Token: 0x06003D3B RID: 15675 RVA: 0x001453AD File Offset: 0x001435AD
	public override void ResetBehavior()
	{
		this.isChasing = false;
	}

	// Token: 0x06003D3C RID: 15676 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnTriggerEnter(Collider otherCollider)
	{
	}

	// Token: 0x04004DFB RID: 19963
	private NavMeshAgent navMeshAgent;

	// Token: 0x04004DFC RID: 19964
	private CustomMapsAIBehaviourController controller;

	// Token: 0x04004DFD RID: 19965
	private float loseSightDist;

	// Token: 0x04004DFE RID: 19966
	private float loseSightDistSq;

	// Token: 0x04004DFF RID: 19967
	private Vector3 sightOffset;

	// Token: 0x04004E00 RID: 19968
	private bool rememberLoseSightPos;

	// Token: 0x04004E01 RID: 19969
	private float stopDistSq;

	// Token: 0x04004E02 RID: 19970
	private bool isChasing;
}
