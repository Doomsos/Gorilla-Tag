using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000956 RID: 2390
public class CustomMapTelemetryTrigger : MonoBehaviour
{
	// Token: 0x06003D26 RID: 15654 RVA: 0x00144B0F File Offset: 0x00142D0F
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x06003D27 RID: 15655 RVA: 0x00144B2F File Offset: 0x00142D2F
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.StartMapTracking();
		}
	}
}
