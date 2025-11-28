using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000346 RID: 838
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestGorillaHarness : MonoBehaviour
{
	// Token: 0x06001419 RID: 5145 RVA: 0x00073FD0 File Offset: 0x000721D0
	private void Awake()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in base.GetComponentsInChildren<PerfTestGorillaSlot>())
		{
			if (perfTestGorillaSlot.slotType == PerfTestGorillaSlot.SlotType.VR_PLAYER)
			{
				this._vrSlot = perfTestGorillaSlot;
			}
			else
			{
				this.dummySlots.Add(perfTestGorillaSlot);
			}
		}
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x00074014 File Offset: 0x00072214
	private void Update()
	{
		if (!this._isRecording)
		{
			return;
		}
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			float num = perfTestGorillaSlot.localStartPosition.y + Mathf.Sin(Time.time * this.bounceSpeed) * this.bounceAmplitude;
			perfTestGorillaSlot.transform.localPosition = new Vector3(perfTestGorillaSlot.localStartPosition.x, num, perfTestGorillaSlot.localStartPosition.z);
		}
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x000740B8 File Offset: 0x000722B8
	public void StartRecording()
	{
		this._isRecording = true;
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x000740C4 File Offset: 0x000722C4
	public void StopRecording()
	{
		foreach (PerfTestGorillaSlot perfTestGorillaSlot in this.dummySlots)
		{
			perfTestGorillaSlot.transform.localPosition = perfTestGorillaSlot.localStartPosition;
		}
		this._isRecording = false;
	}

	// Token: 0x04001EB2 RID: 7858
	public PerfTestGorillaSlot _vrSlot;

	// Token: 0x04001EB3 RID: 7859
	public List<PerfTestGorillaSlot> dummySlots = new List<PerfTestGorillaSlot>(9);

	// Token: 0x04001EB4 RID: 7860
	private bool _isRecording;

	// Token: 0x04001EB5 RID: 7861
	private float _nextRandomMoveTime;

	// Token: 0x04001EB6 RID: 7862
	private float bounceSpeed = 5f;

	// Token: 0x04001EB7 RID: 7863
	private float bounceAmplitude = 0.5f;
}
