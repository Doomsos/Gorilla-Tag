using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200039A RID: 922
public class NativeSizeVolume : MonoBehaviour
{
	// Token: 0x06001608 RID: 5640 RVA: 0x0007ABC8 File Offset: 0x00078DC8
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onEnterAction = this.OnEnterAction;
		if (onEnterAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onEnterAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x0007AC20 File Offset: 0x00078E20
	private void OnTriggerExit(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onExitAction = this.OnExitAction;
		if (onExitAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onExitAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x0400204D RID: 8269
	[SerializeField]
	private Collider triggerVolume;

	// Token: 0x0400204E RID: 8270
	[SerializeField]
	private NativeSizeChangerSettings settings;

	// Token: 0x0400204F RID: 8271
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnEnterAction;

	// Token: 0x04002050 RID: 8272
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnExitAction;

	// Token: 0x0200039B RID: 923
	[Serializable]
	private enum NativeSizeVolumeAction
	{
		// Token: 0x04002052 RID: 8274
		None,
		// Token: 0x04002053 RID: 8275
		ApplySettings,
		// Token: 0x04002054 RID: 8276
		ResetSize
	}
}
