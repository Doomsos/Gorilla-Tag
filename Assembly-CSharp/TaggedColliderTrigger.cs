using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009F8 RID: 2552
public class TaggedColliderTrigger : MonoBehaviour
{
	// Token: 0x0600413E RID: 16702 RVA: 0x0015B722 File Offset: 0x00159922
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastEnter.HasElapsed(this.enterHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x0600413F RID: 16703 RVA: 0x0015B758 File Offset: 0x00159958
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastExit.HasElapsed(this.exitHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x0400522C RID: 21036
	public UnityTag tag;

	// Token: 0x0400522D RID: 21037
	public UnityEvent<Collider> onEnter = new UnityEvent<Collider>();

	// Token: 0x0400522E RID: 21038
	public UnityEvent<Collider> onExit = new UnityEvent<Collider>();

	// Token: 0x0400522F RID: 21039
	public float enterHysteresis = 0.125f;

	// Token: 0x04005230 RID: 21040
	public float exitHysteresis = 0.125f;

	// Token: 0x04005231 RID: 21041
	private TimeSince _sinceLastEnter;

	// Token: 0x04005232 RID: 21042
	private TimeSince _sinceLastExit;
}
