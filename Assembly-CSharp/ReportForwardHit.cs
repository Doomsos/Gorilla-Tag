using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000CC0 RID: 3264
public class ReportForwardHit : MonoBehaviour
{
	// Token: 0x06004FA2 RID: 20386 RVA: 0x00199F8D File Offset: 0x0019818D
	private void Start()
	{
		this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06004FA3 RID: 20387 RVA: 0x00199FAB File Offset: 0x001981AB
	private void OnEnable()
	{
		if (this.seekOnEnable)
		{
			this.seek();
		}
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x06004FA4 RID: 20388 RVA: 0x00199FE0 File Offset: 0x001981E0
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x06004FA5 RID: 20389 RVA: 0x0019A007 File Offset: 0x00198207
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x06004FA6 RID: 20390 RVA: 0x0019A010 File Offset: 0x00198210
	private void Update()
	{
		if (this.nsRand != null)
		{
			return;
		}
		this.timeSinceSeek += Time.deltaTime;
		if (this.timeSinceSeek > this.seekFreq)
		{
			this.seek();
			this.timeSinceSeek = 0f;
			this.seekFreq = ReportForwardHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06004FA7 RID: 20391 RVA: 0x0019A07C File Offset: 0x0019827C
	private void seek()
	{
		float num = Mathf.Max(new float[]
		{
			base.transform.lossyScale.x,
			base.transform.lossyScale.y,
			base.transform.lossyScale.z
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, ref raycastHit, this.maxRadias * num) && this.colliderFound != null)
		{
			this.colliderFound.Invoke(base.transform.position, raycastHit.point);
		}
	}

	// Token: 0x04005E22 RID: 24098
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04005E23 RID: 24099
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04005E24 RID: 24100
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04005E25 RID: 24101
	[SerializeField]
	private float maxRadias = 10f;

	// Token: 0x04005E26 RID: 24102
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04005E27 RID: 24103
	[SerializeField]
	private RandomDispatcher nsRand;

	// Token: 0x04005E28 RID: 24104
	private float timeSinceSeek;

	// Token: 0x04005E29 RID: 24105
	private float seekFreq;

	// Token: 0x04005E2A RID: 24106
	[SerializeField]
	private bool seekOnEnable;
}
