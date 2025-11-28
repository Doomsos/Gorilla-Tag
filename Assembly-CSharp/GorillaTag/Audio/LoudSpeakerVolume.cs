using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02001076 RID: 4214
	public class LoudSpeakerVolume : MonoBehaviour
	{
		// Token: 0x060069C8 RID: 27080 RVA: 0x00226898 File Offset: 0x00224A98
		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("GorillaPlayer"))
			{
				VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
				if (component != null && component.creator != null)
				{
					if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
					{
						this._trigger.OnPlayerEnter(component);
						return;
					}
				}
				else
				{
					Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerEnter no colliding rig found!");
				}
			}
		}

		// Token: 0x060069C9 RID: 27081 RVA: 0x00226908 File Offset: 0x00224B08
		public void OnTriggerExit(Collider other)
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
				{
					this._trigger.OnPlayerExit(component);
					return;
				}
			}
			else
			{
				Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerExit no colliding rig found!");
			}
		}

		// Token: 0x0400790E RID: 30990
		[SerializeField]
		private LoudSpeakerTrigger _trigger;
	}
}
