using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A5 RID: 165
public class MaskCyclopsEye : MonoBehaviour
{
	// Token: 0x06000425 RID: 1061 RVA: 0x00018448 File Offset: 0x00016648
	private void OnEnable()
	{
		this.ScheduleNextBlink();
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDisable()
	{
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00018450 File Offset: 0x00016650
	public void Update()
	{
		if (Time.time >= this.nextBlinkTime)
		{
			UnityEvent onBlink = this.OnBlink;
			if (onBlink != null)
			{
				onBlink.Invoke();
			}
			this.ScheduleNextBlink();
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00018450 File Offset: 0x00016650
	public void Tick()
	{
		if (Time.time >= this.nextBlinkTime)
		{
			UnityEvent onBlink = this.OnBlink;
			if (onBlink != null)
			{
				onBlink.Invoke();
			}
			this.ScheduleNextBlink();
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00018478 File Offset: 0x00016678
	private void ScheduleNextBlink()
	{
		float num = Random.Range(this.minWaitTime, this.maxWaitTime);
		this.nextBlinkTime = Time.time + num;
	}

	// Token: 0x04000495 RID: 1173
	[Tooltip("Invoked when it's time to trigger a blink (e.g., play animation one-shot).")]
	public UnityEvent OnBlink;

	// Token: 0x04000496 RID: 1174
	[Tooltip("Minimum time in seconds between blinks.")]
	[SerializeField]
	private float minWaitTime = 3f;

	// Token: 0x04000497 RID: 1175
	[Tooltip("Maximum time in seconds between blinks.")]
	[SerializeField]
	private float maxWaitTime = 5f;

	// Token: 0x04000498 RID: 1176
	private float nextBlinkTime;
}
