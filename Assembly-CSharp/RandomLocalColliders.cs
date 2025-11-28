using System;
using UnityEngine;

// Token: 0x02000CBF RID: 3263
public class RandomLocalColliders : MonoBehaviour
{
	// Token: 0x06004F9D RID: 20381 RVA: 0x00199DC3 File Offset: 0x00197FC3
	private void Start()
	{
		this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06004F9E RID: 20382 RVA: 0x00199DE4 File Offset: 0x00197FE4
	private void Update()
	{
		if (this.colliderFound == null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = RandomLocalColliders.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06004F9F RID: 20383 RVA: 0x00199E48 File Offset: 0x00198048
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		int num2 = Physics.RaycastNonAlloc(base.transform.position, RandomLocalColliders.rand.NextPointOnSphere(1f), this.raycastHits, this.maxRadias * num);
		if (num2 <= 0)
		{
			return;
		}
		int num3 = RandomLocalColliders.rand.NextInt(num2);
		for (int i = 0; i < num2; i++)
		{
			if (this.raycastHits[(i + num3) % num2].distance >= this.minRadias * num)
			{
				this.colliderFound.Invoke(base.transform.position, this.raycastHits[(i + num3) % num2].point);
				return;
			}
		}
	}

	// Token: 0x04005E19 RID: 24089
	private static SRand rand = new SRand("RandomLocalColliders");

	// Token: 0x04005E1A RID: 24090
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04005E1B RID: 24091
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04005E1C RID: 24092
	[SerializeField]
	private float minRadias = 1f;

	// Token: 0x04005E1D RID: 24093
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04005E1E RID: 24094
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04005E1F RID: 24095
	private float timeSinceSeek;

	// Token: 0x04005E20 RID: 24096
	private float seekFreq;

	// Token: 0x04005E21 RID: 24097
	private RaycastHit[] raycastHits = new RaycastHit[100];
}
