using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020008E6 RID: 2278
public class SpoonClacker : MonoBehaviour
{
	// Token: 0x06003A57 RID: 14935 RVA: 0x00133F6A File Offset: 0x0013216A
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06003A58 RID: 14936 RVA: 0x00133F74 File Offset: 0x00132174
	private void Setup()
	{
		JointLimits limits = this.hingeJoint.limits;
		this.hingeMin = limits.min;
		this.hingeMax = limits.max;
	}

	// Token: 0x06003A59 RID: 14937 RVA: 0x00133FA8 File Offset: 0x001321A8
	private void Update()
	{
		if (!this.transferObject)
		{
			return;
		}
		TransferrableObject.PositionState currentState = this.transferObject.currentState;
		if (currentState != TransferrableObject.PositionState.InLeftHand && currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		float num = MathUtils.Linear(this.hingeJoint.angle, this.hingeMin, this.hingeMax, 0f, 1f);
		float num2 = (this.invertOut ? (1f - num) : num) * 100f;
		this.skinnedMesh.SetBlendShapeWeight(this.targetBlendShape, num2);
		if (!this._lockMin && num <= this.minThreshold)
		{
			this.OnHitMin.Invoke();
			this._lockMin = true;
		}
		else if (!this._lockMax && num >= 1f - this.maxThreshold)
		{
			this.OnHitMax.Invoke();
			this._lockMax = true;
			if (this._sincelastHit.HasElapsed(this.multiHitCutoff, true))
			{
				this.soundsSingle.Play();
			}
			else
			{
				this.soundsMulti.Play();
			}
		}
		if (this._lockMin && num > this.minThreshold * this.hysterisisFactor)
		{
			this._lockMin = false;
		}
		if (this._lockMax && num < 1f - this.maxThreshold * this.hysterisisFactor)
		{
			this._lockMax = false;
		}
	}

	// Token: 0x04004992 RID: 18834
	public TransferrableObject transferObject;

	// Token: 0x04004993 RID: 18835
	public SkinnedMeshRenderer skinnedMesh;

	// Token: 0x04004994 RID: 18836
	public HingeJoint hingeJoint;

	// Token: 0x04004995 RID: 18837
	public int targetBlendShape;

	// Token: 0x04004996 RID: 18838
	public float hingeMin;

	// Token: 0x04004997 RID: 18839
	public float hingeMax;

	// Token: 0x04004998 RID: 18840
	public bool invertOut;

	// Token: 0x04004999 RID: 18841
	public float minThreshold = 0.01f;

	// Token: 0x0400499A RID: 18842
	public float maxThreshold = 0.01f;

	// Token: 0x0400499B RID: 18843
	public float hysterisisFactor = 4f;

	// Token: 0x0400499C RID: 18844
	public UnityEvent OnHitMin;

	// Token: 0x0400499D RID: 18845
	public UnityEvent OnHitMax;

	// Token: 0x0400499E RID: 18846
	private bool _lockMin;

	// Token: 0x0400499F RID: 18847
	private bool _lockMax;

	// Token: 0x040049A0 RID: 18848
	public SoundBankPlayer soundsSingle;

	// Token: 0x040049A1 RID: 18849
	public SoundBankPlayer soundsMulti;

	// Token: 0x040049A2 RID: 18850
	private TimeSince _sincelastHit;

	// Token: 0x040049A3 RID: 18851
	[FormerlySerializedAs("multiHitInterval")]
	public float multiHitCutoff = 0.1f;
}
