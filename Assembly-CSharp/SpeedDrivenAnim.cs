using System;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class SpeedDrivenAnim : MonoBehaviour
{
	// Token: 0x0600108D RID: 4237 RVA: 0x000566C7 File Offset: 0x000548C7
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.animator = base.GetComponent<Animator>();
		this.keyHash = Animator.StringToHash(this.animKey);
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x000566F4 File Offset: 0x000548F4
	private void Update()
	{
		float num = Mathf.InverseLerp(this.speed0, this.speed1, this.velocityEstimator.linearVelocity.magnitude);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, num, this.maxChangePerSecond * Time.deltaTime);
		this.animator.SetFloat(this.keyHash, this.currentBlend);
	}

	// Token: 0x0400148F RID: 5263
	[SerializeField]
	private float speed0;

	// Token: 0x04001490 RID: 5264
	[SerializeField]
	private float speed1 = 1f;

	// Token: 0x04001491 RID: 5265
	[SerializeField]
	private float maxChangePerSecond = 1f;

	// Token: 0x04001492 RID: 5266
	[SerializeField]
	private string animKey = "speed";

	// Token: 0x04001493 RID: 5267
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001494 RID: 5268
	private Animator animator;

	// Token: 0x04001495 RID: 5269
	private int keyHash;

	// Token: 0x04001496 RID: 5270
	private float currentBlend;
}
