using System;
using GorillaTag;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x020002F8 RID: 760
public class BasicFireSpawner : MonoBehaviour
{
	// Token: 0x060012AC RID: 4780 RVA: 0x00061B2D File Offset: 0x0005FD2D
	private void Awake()
	{
		this.scale = this.fireScaleMinMax.y;
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x00061B40 File Offset: 0x0005FD40
	public void InterpolateScale(float f)
	{
		this.scale = Mathf.Lerp(this.fireScaleMinMax.x, this.fireScaleMinMax.y, f);
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x00061B64 File Offset: 0x0005FD64
	public void Spawn()
	{
		if (this.firePool == null)
		{
			this.firePool = ObjectPools.instance.GetPoolByHash(this.firePrefab);
		}
		FireManager.SpawnFire(this.firePool, base.transform.position, Vector3.up, this.scale);
	}

	// Token: 0x04001738 RID: 5944
	[SerializeField]
	private HashWrapper firePrefab;

	// Token: 0x04001739 RID: 5945
	[SerializeField]
	private Vector2 fireScaleMinMax = Vector2.one;

	// Token: 0x0400173A RID: 5946
	private SinglePool firePool;

	// Token: 0x0400173B RID: 5947
	private float scale;
}
