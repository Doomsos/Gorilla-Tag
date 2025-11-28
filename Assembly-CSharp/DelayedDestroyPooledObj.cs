using System;
using UnityEngine;

// Token: 0x02000C41 RID: 3137
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x06004D01 RID: 19713 RVA: 0x0018FEFB File Offset: 0x0018E0FB
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06004D02 RID: 19714 RVA: 0x0018FF29 File Offset: 0x0018E129
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04005CB0 RID: 23728
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04005CB1 RID: 23729
	private float timeToDie = -1f;
}
