using System;
using Fusion;
using UnityEngine;

// Token: 0x02000768 RID: 1896
[NetworkBehaviourWeaved(0)]
internal class VrrigReliableSerializer : GorillaWrappedSerializer
{
	// Token: 0x0600314B RID: 12619 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnBeforeDespawn()
	{
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x0010C1D4 File Offset: 0x0010A3D4
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (wrappedInfo.punInfo.Sender != wrappedInfo.punInfo.photonView.Owner || wrappedInfo.punInfo.photonView.IsRoomView)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(wrappedInfo.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x00109E0D File Offset: 0x0010800D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x00109E19 File Offset: 0x00108019
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
