using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009B RID: 155
public class DevWatchButton : MonoBehaviour
{
	// Token: 0x060003D7 RID: 983 RVA: 0x00017504 File Offset: 0x00015704
	public void OnTriggerEnter(Collider other)
	{
		this.SearchEvent.Invoke();
	}

	// Token: 0x04000457 RID: 1111
	public UnityEvent SearchEvent = new UnityEvent();
}
