using System;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class DelayedDestroyCrittersPooledObject : MonoBehaviour
{
	// Token: 0x06000301 RID: 769 RVA: 0x00012B2F File Offset: 0x00010D2F
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00012B5D File Offset: 0x00010D5D
	protected void LateUpdate()
	{
		if (Time.time >= this.timeToDie)
		{
			CrittersPool.Return(base.gameObject);
		}
	}

	// Token: 0x040003A9 RID: 937
	public float destroyDelay = 1f;

	// Token: 0x040003AA RID: 938
	private float timeToDie = -1f;
}
