using System;
using UnityEngine;

// Token: 0x02000C46 RID: 3142
public class DisableGameObjectDelayed : MonoBehaviour
{
	// Token: 0x06004D0A RID: 19722 RVA: 0x0018FFD2 File Offset: 0x0018E1D2
	private void OnEnable()
	{
		this.enabledTime = Time.time;
	}

	// Token: 0x06004D0B RID: 19723 RVA: 0x0018FFDF File Offset: 0x0018E1DF
	private void Update()
	{
		if (Time.time > this.enabledTime + this.delayTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004D0C RID: 19724 RVA: 0x00190001 File Offset: 0x0018E201
	public void EnableAndResetTimer()
	{
		base.gameObject.SetActive(true);
		this.OnEnable();
	}

	// Token: 0x04005CB2 RID: 23730
	public float delayTime = 1f;

	// Token: 0x04005CB3 RID: 23731
	public float enabledTime;
}
