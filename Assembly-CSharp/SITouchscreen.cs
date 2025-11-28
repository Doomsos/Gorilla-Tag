using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class SITouchscreen : MonoBehaviour
{
	// Token: 0x060008D6 RID: 2262 RVA: 0x0002FE2C File Offset: 0x0002E02C
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerStay(other);
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0002FE38 File Offset: 0x0002E038
	private void OnTriggerStay(Collider other)
	{
		Transform indicator = this.GetIndicator(other);
		if (indicator != null)
		{
			this.controllingTransform = indicator;
			this.lastTouched = Time.time;
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0002FE68 File Offset: 0x0002E068
	private void OnTriggerExit(Collider other)
	{
		if (this.controllingTransform == null || this.GetIndicator(other) != this.controllingTransform)
		{
			return;
		}
		this.controllingTransform = null;
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0002FE94 File Offset: 0x0002E094
	private Transform GetIndicator(Collider other)
	{
		if (this.notFingerTouchDict.Contains(other))
		{
			return null;
		}
		GorillaTriggerColliderHandIndicator componentInParent;
		if (!this.fingerTouchDict.TryGetValue(other, ref componentInParent))
		{
			componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				this.notFingerTouchDict.Add(other);
				return null;
			}
			this.fingerTouchDict.Add(other, componentInParent);
		}
		return componentInParent.transform;
	}

	// Token: 0x04000AC3 RID: 2755
	public Transform controllingTransform;

	// Token: 0x04000AC4 RID: 2756
	public float lastTouched;

	// Token: 0x04000AC5 RID: 2757
	public Vector3 lastPosition;

	// Token: 0x04000AC6 RID: 2758
	private Dictionary<Collider, GorillaTriggerColliderHandIndicator> fingerTouchDict = new Dictionary<Collider, GorillaTriggerColliderHandIndicator>();

	// Token: 0x04000AC7 RID: 2759
	private HashSet<Collider> notFingerTouchDict = new HashSet<Collider>();
}
