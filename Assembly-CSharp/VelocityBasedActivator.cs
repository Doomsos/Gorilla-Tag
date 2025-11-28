using System;
using UnityEngine;

// Token: 0x02000CE9 RID: 3305
[RequireComponent(typeof(GorillaVelocityEstimator))]
public class VelocityBasedActivator : MonoBehaviour
{
	// Token: 0x0600505C RID: 20572 RVA: 0x0019D7EB File Offset: 0x0019B9EB
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x0600505D RID: 20573 RVA: 0x0019D7FC File Offset: 0x0019B9FC
	private void Update()
	{
		this.k += this.velocityEstimator.linearVelocity.sqrMagnitude;
		this.k = Mathf.Max(this.k - Time.deltaTime * this.decay, 0f);
		if (!this.active && this.k > this.threshold)
		{
			this.activate(true);
		}
		if (this.active && this.k < this.threshold)
		{
			this.activate(false);
		}
	}

	// Token: 0x0600505E RID: 20574 RVA: 0x0019D888 File Offset: 0x0019BA88
	private void activate(bool v)
	{
		this.active = v;
		for (int i = 0; i < this.activationTargets.Length; i++)
		{
			this.activationTargets[i].SetActive(v);
		}
	}

	// Token: 0x0600505F RID: 20575 RVA: 0x0019D8BD File Offset: 0x0019BABD
	private void OnDisable()
	{
		if (this.active)
		{
			this.activate(false);
		}
	}

	// Token: 0x04005F97 RID: 24471
	[SerializeField]
	private GameObject[] activationTargets;

	// Token: 0x04005F98 RID: 24472
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04005F99 RID: 24473
	private float k;

	// Token: 0x04005F9A RID: 24474
	private bool active;

	// Token: 0x04005F9B RID: 24475
	[SerializeField]
	private float decay = 1f;

	// Token: 0x04005F9C RID: 24476
	[SerializeField]
	private float threshold = 1f;
}
