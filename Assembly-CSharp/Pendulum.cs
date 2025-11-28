using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class Pendulum : MonoBehaviour
{
	// Token: 0x0600097F RID: 2431 RVA: 0x000333D8 File Offset: 0x000315D8
	private void Start()
	{
		this.pendulum = (this.ClockPendulum = base.gameObject.GetComponent<Transform>());
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00033400 File Offset: 0x00031600
	private void Update()
	{
		if (this.pendulum)
		{
			float num = this.MaxAngleDeflection * Mathf.Sin(Time.time * this.SpeedOfPendulum);
			this.pendulum.localRotation = Quaternion.Euler(0f, 0f, num);
			return;
		}
	}

	// Token: 0x04000B98 RID: 2968
	public float MaxAngleDeflection = 10f;

	// Token: 0x04000B99 RID: 2969
	public float SpeedOfPendulum = 1f;

	// Token: 0x04000B9A RID: 2970
	public Transform ClockPendulum;

	// Token: 0x04000B9B RID: 2971
	private Transform pendulum;
}
