using System;
using UnityEngine;

// Token: 0x02000C8C RID: 3212
public class TriggerEventNotifier : MonoBehaviour
{
	// Token: 0x1400008B RID: 139
	// (add) Token: 0x06004E81 RID: 20097 RVA: 0x00196D80 File Offset: 0x00194F80
	// (remove) Token: 0x06004E82 RID: 20098 RVA: 0x00196DB8 File Offset: 0x00194FB8
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	// Token: 0x1400008C RID: 140
	// (add) Token: 0x06004E83 RID: 20099 RVA: 0x00196DF0 File Offset: 0x00194FF0
	// (remove) Token: 0x06004E84 RID: 20100 RVA: 0x00196E28 File Offset: 0x00195028
	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	// Token: 0x06004E85 RID: 20101 RVA: 0x00196E5D File Offset: 0x0019505D
	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	// Token: 0x06004E86 RID: 20102 RVA: 0x00196E71 File Offset: 0x00195071
	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	// Token: 0x04005D6D RID: 23917
	[HideInInspector]
	public int maskIndex;

	// Token: 0x02000C8D RID: 3213
	// (Invoke) Token: 0x06004E89 RID: 20105
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}
