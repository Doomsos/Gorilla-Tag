using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class NativeSizeChanger : MonoBehaviour
{
	// Token: 0x060015FF RID: 5631 RVA: 0x0007AB48 File Offset: 0x00078D48
	public void Activate(NativeSizeChangerSettings settings)
	{
		settings.WorldPosition = base.transform.position;
		settings.ActivationTime = Time.time;
		GTPlayer.Instance.SetNativeScale(settings);
	}
}
