using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000347 RID: 839
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestGorillaSlot : MonoBehaviour
{
	// Token: 0x0600141E RID: 5150 RVA: 0x00074153 File Offset: 0x00072353
	private void Start()
	{
		this.localStartPosition = base.transform.localPosition;
	}

	// Token: 0x04001EB8 RID: 7864
	public PerfTestGorillaSlot.SlotType slotType;

	// Token: 0x04001EB9 RID: 7865
	public Vector3 localStartPosition;

	// Token: 0x02000348 RID: 840
	public enum SlotType
	{
		// Token: 0x04001EBB RID: 7867
		VR_PLAYER,
		// Token: 0x04001EBC RID: 7868
		DUMMY
	}
}
