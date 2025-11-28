using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x0200114C RID: 4428
	public class CrittersKillVolume : MonoBehaviour
	{
		// Token: 0x06006FCD RID: 28621 RVA: 0x002466C0 File Offset: 0x002448C0
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody)
			{
				CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
				if (component.IsNotNull())
				{
					component.gameObject.SetActive(false);
				}
			}
		}
	}
}
