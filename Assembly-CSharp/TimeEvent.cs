using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000CB3 RID: 3251
public class TimeEvent : MonoBehaviour
{
	// Token: 0x06004F63 RID: 20323 RVA: 0x001992FB File Offset: 0x001974FB
	protected void StartEvent()
	{
		this._ongoing = true;
		UnityEvent unityEvent = this.onEventStart;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06004F64 RID: 20324 RVA: 0x00199314 File Offset: 0x00197514
	protected void StopEvent()
	{
		this._ongoing = false;
		UnityEvent unityEvent = this.onEventStop;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04005DE9 RID: 24041
	public UnityEvent onEventStart;

	// Token: 0x04005DEA RID: 24042
	public UnityEvent onEventStop;

	// Token: 0x04005DEB RID: 24043
	[SerializeField]
	protected bool _ongoing;
}
