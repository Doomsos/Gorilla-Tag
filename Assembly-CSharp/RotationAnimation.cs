using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public class RotationAnimation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001AB6 RID: 6838 RVA: 0x0008D339 File Offset: 0x0008B539
	// (set) Token: 0x06001AB7 RID: 6839 RVA: 0x0008D341 File Offset: 0x0008B541
	public bool TickRunning { get; set; }

	// Token: 0x06001AB8 RID: 6840 RVA: 0x0008D34C File Offset: 0x0008B54C
	public void Tick()
	{
		Vector3 vector = Vector3.zero;
		vector.x = this.amplitude.x * this.x.Evaluate((Time.time - this.baseTime) * this.period.x % 1f);
		vector.y = this.amplitude.y * this.y.Evaluate((Time.time - this.baseTime) * this.period.y % 1f);
		vector.z = this.amplitude.z * this.z.Evaluate((Time.time - this.baseTime) * this.period.z % 1f);
		if (this.releaseSet)
		{
			float num = this.release.Evaluate(Time.time - this.releaseTime);
			vector *= num;
			if (num < Mathf.Epsilon)
			{
				base.enabled = false;
			}
		}
		base.transform.localRotation = Quaternion.Euler(vector) * this.baseRotation;
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x0008D466 File Offset: 0x0008B666
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x0008D479 File Offset: 0x0008B679
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.releaseSet = false;
		this.baseTime = Time.time;
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x0008D493 File Offset: 0x0008B693
	public void ReleaseToDisable()
	{
		this.releaseSet = true;
		this.releaseTime = Time.time;
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x0008D4A7 File Offset: 0x0008B6A7
	public void CancelRelease()
	{
		this.releaseSet = false;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x0008D4B0 File Offset: 0x0008B6B0
	private void OnDisable()
	{
		base.transform.localRotation = this.baseRotation;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x04002428 RID: 9256
	[SerializeField]
	private AnimationCurve x;

	// Token: 0x04002429 RID: 9257
	[SerializeField]
	private AnimationCurve y;

	// Token: 0x0400242A RID: 9258
	[SerializeField]
	private AnimationCurve z;

	// Token: 0x0400242B RID: 9259
	[SerializeField]
	private AnimationCurve attack;

	// Token: 0x0400242C RID: 9260
	[SerializeField]
	private AnimationCurve release;

	// Token: 0x0400242D RID: 9261
	[SerializeField]
	private Vector3 amplitude = Vector3.one;

	// Token: 0x0400242E RID: 9262
	[SerializeField]
	private Vector3 period = Vector3.one;

	// Token: 0x0400242F RID: 9263
	private Quaternion baseRotation;

	// Token: 0x04002430 RID: 9264
	private float baseTime;

	// Token: 0x04002431 RID: 9265
	private float releaseTime;

	// Token: 0x04002432 RID: 9266
	private bool releaseSet;
}
