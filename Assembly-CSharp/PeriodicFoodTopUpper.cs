using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class PeriodicFoodTopUpper : MonoBehaviour
{
	// Token: 0x06000357 RID: 855 RVA: 0x00013EEC File Offset: 0x000120EC
	private void Awake()
	{
		this.food = base.GetComponentInParent<CrittersFood>();
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00013EFC File Offset: 0x000120FC
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.waitingToRefill && this.food.currentFood == 0f)
		{
			this.waitingToRefill = true;
			this.timeFoodEmpty = Time.time;
		}
		if (this.waitingToRefill && Time.time > this.timeFoodEmpty + this.waitToRefill)
		{
			this.waitingToRefill = false;
			this.food.Initialize();
		}
	}

	// Token: 0x040003DF RID: 991
	private CrittersFood food;

	// Token: 0x040003E0 RID: 992
	private float timeFoodEmpty;

	// Token: 0x040003E1 RID: 993
	private bool waitingToRefill;

	// Token: 0x040003E2 RID: 994
	public float waitToRefill = 10f;

	// Token: 0x040003E3 RID: 995
	public GameObject foodObject;
}
