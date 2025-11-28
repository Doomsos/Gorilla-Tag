using System;
using NetSynchrony;
using UnityEngine;

// Token: 0x02000CC1 RID: 3265
public class ReportTargetHit : MonoBehaviour
{
	// Token: 0x06004FAA RID: 20394 RVA: 0x0019A154 File Offset: 0x00198354
	private void Start()
	{
		this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
	}

	// Token: 0x06004FAB RID: 20395 RVA: 0x0019A172 File Offset: 0x00198372
	private void OnEnable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch += this.NsRand_Dispatch;
		}
	}

	// Token: 0x06004FAC RID: 20396 RVA: 0x0019A199 File Offset: 0x00198399
	private void OnDisable()
	{
		if (this.nsRand != null)
		{
			this.nsRand.Dispatch -= this.NsRand_Dispatch;
		}
	}

	// Token: 0x06004FAD RID: 20397 RVA: 0x0019A1C0 File Offset: 0x001983C0
	private void NsRand_Dispatch(RandomDispatcher randomDispatcher)
	{
		this.seek();
	}

	// Token: 0x06004FAE RID: 20398 RVA: 0x0019A1C8 File Offset: 0x001983C8
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
			this.seekFreq = ReportTargetHit.rand.NextFloat(this.minseekFreq, this.maxseekFreq);
		}
	}

	// Token: 0x06004FAF RID: 20399 RVA: 0x0019A234 File Offset: 0x00198434
	private void seek()
	{
		if (this.targets.Length != 0)
		{
			Vector3 vector = this.targets[ReportTargetHit.rand.NextInt(this.targets.Length)].position - base.transform.position;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, vector, ref raycastHit) && this.colliderFound != null)
			{
				this.colliderFound.Invoke(base.transform.position, raycastHit.point);
			}
		}
	}

	// Token: 0x04005E2B RID: 24107
	private static SRand rand = new SRand("ReportForwardHit");

	// Token: 0x04005E2C RID: 24108
	[SerializeField]
	private float minseekFreq = 3f;

	// Token: 0x04005E2D RID: 24109
	[SerializeField]
	private float maxseekFreq = 6f;

	// Token: 0x04005E2E RID: 24110
	[SerializeField]
	private Transform[] targets;

	// Token: 0x04005E2F RID: 24111
	[SerializeField]
	private LightningDispatcherEvent colliderFound;

	// Token: 0x04005E30 RID: 24112
	private float timeSinceSeek;

	// Token: 0x04005E31 RID: 24113
	private float seekFreq;

	// Token: 0x04005E32 RID: 24114
	[SerializeField]
	private RandomDispatcher nsRand;
}
