using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200028B RID: 651
public class TriggerOnSpeed : MonoBehaviour, ITickSystemTick
{
	// Token: 0x060010BE RID: 4286 RVA: 0x0003688F File Offset: 0x00034A8F
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x00036897 File Offset: 0x00034A97
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x00057500 File Offset: 0x00055700
	public void Tick()
	{
		bool flag = this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold);
		if (flag != this.wasFaster)
		{
			if (flag)
			{
				this.onFaster.Invoke();
			}
			else
			{
				this.onSlower.Invoke();
			}
			this.wasFaster = flag;
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x060010C1 RID: 4289 RVA: 0x0005754F File Offset: 0x0005574F
	// (set) Token: 0x060010C2 RID: 4290 RVA: 0x00057557 File Offset: 0x00055757
	public bool TickRunning { get; set; }

	// Token: 0x040014E0 RID: 5344
	[SerializeField]
	private float speedThreshold;

	// Token: 0x040014E1 RID: 5345
	[SerializeField]
	private UnityEvent onFaster;

	// Token: 0x040014E2 RID: 5346
	[SerializeField]
	private UnityEvent onSlower;

	// Token: 0x040014E3 RID: 5347
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040014E4 RID: 5348
	private bool wasFaster;
}
