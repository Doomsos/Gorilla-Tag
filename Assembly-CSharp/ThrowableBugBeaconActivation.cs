using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000CCD RID: 3277
public class ThrowableBugBeaconActivation : MonoBehaviour
{
	// Token: 0x06004FEF RID: 20463 RVA: 0x0019B634 File Offset: 0x00199834
	private void Awake()
	{
		this.tbb = base.GetComponent<ThrowableBugBeacon>();
	}

	// Token: 0x06004FF0 RID: 20464 RVA: 0x0019B642 File Offset: 0x00199842
	private void OnEnable()
	{
		base.StartCoroutine(this.SendSignals());
	}

	// Token: 0x06004FF1 RID: 20465 RVA: 0x0000528D File Offset: 0x0000348D
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06004FF2 RID: 20466 RVA: 0x0019B651 File Offset: 0x00199851
	private IEnumerator SendSignals()
	{
		uint count = 0U;
		while (this.signalCount == 0U || count < this.signalCount)
		{
			yield return new WaitForSeconds(Random.Range(this.minCallTime, this.maxCallTime));
			switch (this.mode)
			{
			case ThrowableBugBeaconActivation.ActivationMode.CALL:
				this.tbb.Call();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
				this.tbb.Dismiss();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.LOCK:
				this.tbb.Lock();
				break;
			}
			uint num = count;
			count = num + 1U;
		}
		yield break;
	}

	// Token: 0x04005E8D RID: 24205
	[SerializeField]
	private float minCallTime = 1f;

	// Token: 0x04005E8E RID: 24206
	[SerializeField]
	private float maxCallTime = 5f;

	// Token: 0x04005E8F RID: 24207
	[SerializeField]
	private uint signalCount;

	// Token: 0x04005E90 RID: 24208
	[SerializeField]
	private ThrowableBugBeaconActivation.ActivationMode mode;

	// Token: 0x04005E91 RID: 24209
	private ThrowableBugBeacon tbb;

	// Token: 0x02000CCE RID: 3278
	private enum ActivationMode
	{
		// Token: 0x04005E93 RID: 24211
		CALL,
		// Token: 0x04005E94 RID: 24212
		DISMISS,
		// Token: 0x04005E95 RID: 24213
		LOCK
	}
}
