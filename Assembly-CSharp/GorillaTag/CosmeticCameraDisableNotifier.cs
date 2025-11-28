using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FEF RID: 4079
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticCameraDisableNotifier : MonoBehaviour
	{
		// Token: 0x06006718 RID: 26392 RVA: 0x002187D0 File Offset: 0x002169D0
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(ref this._vrrigCollection))
			{
				this._vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this._vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this._vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x06006719 RID: 26393 RVA: 0x00218845 File Offset: 0x00216A45
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = false;
			}
		}

		// Token: 0x0600671A RID: 26394 RVA: 0x00218860 File Offset: 0x00216A60
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = true;
			}
		}

		// Token: 0x040075A1 RID: 30113
		private VRRigCollection _vrrigCollection;

		// Token: 0x040075A2 RID: 30114
		[SerializeField]
		private Camera _cosmeticCamera;
	}
}
