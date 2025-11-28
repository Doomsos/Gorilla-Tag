using System;
using UnityEngine;

// Token: 0x02000533 RID: 1331
public class SetStateConditional : StateMachineBehaviour
{
	// Token: 0x0600218A RID: 8586 RVA: 0x000AFD1B File Offset: 0x000ADF1B
	private void OnValidate()
	{
		this._setToID = this.setToState;
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000AFD2E File Offset: 0x000ADF2E
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this._didSetup)
		{
			this.parentAnimator = animator;
			this.Setup(animator, stateInfo, layerIndex);
			this._didSetup = true;
		}
		this._sinceEnter = TimeSince.Now();
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000AFD5C File Offset: 0x000ADF5C
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.delay > 0f && !this._sinceEnter.HasElapsed(this.delay, true))
		{
			return;
		}
		if (!this.CanSetState(animator, stateInfo, layerIndex))
		{
			return;
		}
		animator.Play(this._setToID);
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x00027DED File Offset: 0x00025FED
	protected virtual bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		return true;
	}

	// Token: 0x04002C44 RID: 11332
	public Animator parentAnimator;

	// Token: 0x04002C45 RID: 11333
	public string setToState;

	// Token: 0x04002C46 RID: 11334
	[SerializeField]
	private AnimStateHash _setToID;

	// Token: 0x04002C47 RID: 11335
	public float delay = 1f;

	// Token: 0x04002C48 RID: 11336
	protected TimeSince _sinceEnter;

	// Token: 0x04002C49 RID: 11337
	[NonSerialized]
	private bool _didSetup;
}
