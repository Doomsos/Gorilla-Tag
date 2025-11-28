using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000A7A RID: 2682
public class AnimationPauser : StateMachineBehaviour
{
	// Token: 0x0600436B RID: 17259 RVA: 0x00165F84 File Offset: 0x00164184
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AnimationPauser.<OnStateEnter>d__4 <OnStateEnter>d__;
		<OnStateEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnStateEnter>d__.<>4__this = this;
		<OnStateEnter>d__.animator = animator;
		<OnStateEnter>d__.stateInfo = stateInfo;
		<OnStateEnter>d__.layerIndex = layerIndex;
		<OnStateEnter>d__.<>1__state = -1;
		<OnStateEnter>d__.<>t__builder.Start<AnimationPauser.<OnStateEnter>d__4>(ref <OnStateEnter>d__);
	}

	// Token: 0x040054ED RID: 21741
	[SerializeField]
	private int _maxTimeBetweenAnims = 5;

	// Token: 0x040054EE RID: 21742
	[SerializeField]
	private int _minTimeBetweenAnims = 1;

	// Token: 0x040054EF RID: 21743
	private int _animPauseDuration;

	// Token: 0x040054F0 RID: 21744
	private static readonly string Restart_Anim_Name = "RestartAnim";
}
