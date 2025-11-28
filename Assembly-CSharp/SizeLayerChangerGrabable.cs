using System;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200082E RID: 2094
public class SizeLayerChangerGrabable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x06003717 RID: 14103 RVA: 0x0012905A File Offset: 0x0012725A
	public bool MomentaryGrabOnly()
	{
		return this.momentaryGrabOnly;
	}

	// Token: 0x06003718 RID: 14104 RVA: 0x00027DED File Offset: 0x00025FED
	bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06003719 RID: 14105 RVA: 0x00129064 File Offset: 0x00127264
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosiiton)
	{
		if (this.grabChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.grabbedSizeLayerMask.Mask;
		}
		grabbedObject = base.transform;
		grabbedLocalPosiiton = base.transform.InverseTransformPoint(g.transform.position);
	}

	// Token: 0x0600371A RID: 14106 RVA: 0x001290CC File Offset: 0x001272CC
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		if (this.releaseChangesSizeLayer)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer);
			rigContainer.Rig.sizeManager.currentSizeLayerMaskValue = this.releasedSizeLayerMask.Mask;
		}
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x00013E3B File Offset: 0x0001203B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04004688 RID: 18056
	[SerializeField]
	private bool grabChangesSizeLayer = true;

	// Token: 0x04004689 RID: 18057
	[SerializeField]
	private bool releaseChangesSizeLayer = true;

	// Token: 0x0400468A RID: 18058
	[SerializeField]
	private SizeLayerMask grabbedSizeLayerMask;

	// Token: 0x0400468B RID: 18059
	[SerializeField]
	private SizeLayerMask releasedSizeLayerMask;

	// Token: 0x0400468C RID: 18060
	[SerializeField]
	private bool momentaryGrabOnly = true;
}
