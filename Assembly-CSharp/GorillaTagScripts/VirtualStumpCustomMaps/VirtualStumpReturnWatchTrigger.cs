using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E25 RID: 3621
	public class VirtualStumpReturnWatchTrigger : MonoBehaviour
	{
		// Token: 0x06005A73 RID: 23155 RVA: 0x001CF94B File Offset: 0x001CDB4B
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider)
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(false);
			}
		}

		// Token: 0x06005A74 RID: 23156 RVA: 0x001CF96A File Offset: 0x001CDB6A
		public void OnTriggerExit(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(true);
			}
		}
	}
}
