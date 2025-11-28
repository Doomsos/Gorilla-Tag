using System;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class PlayerGameEventLocationTrigger : MonoBehaviour
{
	// Token: 0x06000D31 RID: 3377 RVA: 0x00046D06 File Offset: 0x00044F06
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			PlayerGameEvents.TriggerEnterLocation(this.locationName);
		}
	}

	// Token: 0x04001033 RID: 4147
	[SerializeField]
	private string locationName;
}
