using System;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public class SetStateIfNoOverlaps : SetStateConditional
{
	// Token: 0x06002190 RID: 8592 RVA: 0x000AFDDB File Offset: 0x000ADFDB
	protected override void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this._volume = animator.GetComponent<VolumeCast>();
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000AFDE9 File Offset: 0x000ADFE9
	protected override bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		bool flag = this._volume.CheckOverlaps();
		if (flag)
		{
			this._sinceEnter = 0f;
		}
		return !flag;
	}

	// Token: 0x04002C4A RID: 11338
	public VolumeCast _volume;
}
