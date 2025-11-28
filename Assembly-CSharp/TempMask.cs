using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class TempMask : MonoBehaviour
{
	// Token: 0x06000A08 RID: 2568 RVA: 0x00036644 File Offset: 0x00034844
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.netView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x000366B2 File Offset: 0x000348B2
	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0000528D File Offset: 0x0000348D
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x000366C1 File Offset: 0x000348C1
	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x04000C58 RID: 3160
	public int year;

	// Token: 0x04000C59 RID: 3161
	public int month;

	// Token: 0x04000C5A RID: 3162
	public int day;

	// Token: 0x04000C5B RID: 3163
	public DateTime dayOn;

	// Token: 0x04000C5C RID: 3164
	public MeshRenderer myRenderer;

	// Token: 0x04000C5D RID: 3165
	private DateTime myDate;

	// Token: 0x04000C5E RID: 3166
	private VRRig myRig;
}
