using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class LocalActivateOnDateRange : MonoBehaviour
{
	// Token: 0x060011ED RID: 4589 RVA: 0x0005E748 File Offset: 0x0005C948
	private void Awake()
	{
		GameObject[] array = this.gameObjectsToActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0005E773 File Offset: 0x0005C973
	private void OnEnable()
	{
		this.InitActiveTimes();
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0005E77C File Offset: 0x0005C97C
	private void InitActiveTimes()
	{
		this.activationTime = new DateTime(this.activationYear, this.activationMonth, this.activationDay, this.activationHour, this.activationMinute, this.activationSecond, 1);
		this.deactivationTime = new DateTime(this.deactivationYear, this.deactivationMonth, this.deactivationDay, this.deactivationHour, this.deactivationMinute, this.deactivationSecond, 1);
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0005E7EC File Offset: 0x0005C9EC
	private void LateUpdate()
	{
		DateTime utcNow = DateTime.UtcNow;
		this.dbgTimeUntilActivation = (this.activationTime - utcNow).TotalSeconds;
		this.dbgTimeUntilDeactivation = (this.deactivationTime - utcNow).TotalSeconds;
		bool flag = utcNow >= this.activationTime && utcNow <= this.deactivationTime;
		if (flag != this.isActive)
		{
			GameObject[] array = this.gameObjectsToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			this.isActive = flag;
		}
	}

	// Token: 0x04001687 RID: 5767
	[Header("Activation Date and Time (UTC)")]
	public int activationYear = 2023;

	// Token: 0x04001688 RID: 5768
	public int activationMonth = 4;

	// Token: 0x04001689 RID: 5769
	public int activationDay = 1;

	// Token: 0x0400168A RID: 5770
	public int activationHour = 7;

	// Token: 0x0400168B RID: 5771
	public int activationMinute;

	// Token: 0x0400168C RID: 5772
	public int activationSecond;

	// Token: 0x0400168D RID: 5773
	[Header("Deactivation Date and Time (UTC)")]
	public int deactivationYear = 2023;

	// Token: 0x0400168E RID: 5774
	public int deactivationMonth = 4;

	// Token: 0x0400168F RID: 5775
	public int deactivationDay = 2;

	// Token: 0x04001690 RID: 5776
	public int deactivationHour = 7;

	// Token: 0x04001691 RID: 5777
	public int deactivationMinute;

	// Token: 0x04001692 RID: 5778
	public int deactivationSecond;

	// Token: 0x04001693 RID: 5779
	public GameObject[] gameObjectsToActivate;

	// Token: 0x04001694 RID: 5780
	private bool isActive;

	// Token: 0x04001695 RID: 5781
	private DateTime activationTime;

	// Token: 0x04001696 RID: 5782
	private DateTime deactivationTime;

	// Token: 0x04001697 RID: 5783
	[DebugReadout]
	public double dbgTimeUntilActivation;

	// Token: 0x04001698 RID: 5784
	[DebugReadout]
	public double dbgTimeUntilDeactivation;
}
