using System;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class SpinRotation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06001090 RID: 4240 RVA: 0x00056784 File Offset: 0x00054984
	// (set) Token: 0x06001091 RID: 4241 RVA: 0x0005678C File Offset: 0x0005498C
	public bool TickRunning { get; set; }

	// Token: 0x06001092 RID: 4242 RVA: 0x00056795 File Offset: 0x00054995
	public void Tick()
	{
		base.transform.localRotation = Quaternion.Euler(this.rotationPerSecondEuler * (Time.time - this.baseTime)) * this.baseRotation;
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x000567C9 File Offset: 0x000549C9
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x000567DC File Offset: 0x000549DC
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.baseTime = Time.time;
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x04001497 RID: 5271
	[SerializeField]
	private Vector3 rotationPerSecondEuler;

	// Token: 0x04001498 RID: 5272
	private Quaternion baseRotation;

	// Token: 0x04001499 RID: 5273
	private float baseTime;
}
