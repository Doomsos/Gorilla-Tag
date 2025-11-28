using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FF0 RID: 4080
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticTryOnNotifier : MonoBehaviour
	{
		// Token: 0x0600671C RID: 26396 RVA: 0x0021887C File Offset: 0x00216A7C
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(ref this.m_vrrigCollection))
			{
				this.m_vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this.m_vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this.m_vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x0600671D RID: 26397 RVA: 0x002188F4 File Offset: 0x00216AF4
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(true, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(playerRig, this.unlockList.Strings);
		}

		// Token: 0x0600671E RID: 26398 RVA: 0x0021892C File Offset: 0x00216B2C
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(false, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(playerRig, this.unlockList.Strings);
		}

		// Token: 0x040075A3 RID: 30115
		private VRRigCollection m_vrrigCollection;

		// Token: 0x040075A4 RID: 30116
		[SerializeField]
		private CosmeticTryOnNotifier.Mode mode;

		// Token: 0x040075A5 RID: 30117
		[SerializeField]
		private StringList unlockList;

		// Token: 0x02000FF1 RID: 4081
		private enum Mode
		{
			// Token: 0x040075A7 RID: 30119
			TRY_ON,
			// Token: 0x040075A8 RID: 30120
			ENABLE_LIST,
			// Token: 0x040075A9 RID: 30121
			ENABLE_LIST_TITLEDATA
		}
	}
}
