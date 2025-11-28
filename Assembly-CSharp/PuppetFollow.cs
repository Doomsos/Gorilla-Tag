using System;
using UnityEngine;

// Token: 0x02000445 RID: 1093
public class PuppetFollow : MonoBehaviour
{
	// Token: 0x06001AD6 RID: 6870 RVA: 0x0008DA1C File Offset: 0x0008BC1C
	private void FixedUpdate()
	{
		base.transform.position = this.sourceTarget.position - this.sourceBase.position + this.puppetBase.position;
		base.transform.localRotation = this.sourceTarget.localRotation;
	}

	// Token: 0x04002460 RID: 9312
	public Transform sourceTarget;

	// Token: 0x04002461 RID: 9313
	public Transform sourceBase;

	// Token: 0x04002462 RID: 9314
	public Transform puppetBase;
}
