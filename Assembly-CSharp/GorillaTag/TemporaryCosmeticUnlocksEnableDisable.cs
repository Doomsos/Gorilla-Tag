using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FF5 RID: 4085
	public class TemporaryCosmeticUnlocksEnableDisable : MonoBehaviour
	{
		// Token: 0x0600672E RID: 26414 RVA: 0x00218EC0 File Offset: 0x002170C0
		private void Awake()
		{
			if (this.m_wardrobe.IsNull() || this.m_cosmeticAreaTrigger.IsNull())
			{
				Debug.LogError("TemporaryCosmeticUnlocksEnableDisable: reference is null, disabling self");
				base.enabled = false;
			}
			if (CosmeticsController.instance.IsNull() || !this.m_wardrobe.WardrobeButtonsInitialized())
			{
				base.enabled = false;
				this.m_timer = new TickSystemTimer(0.05f, new Action(this.CheckWardrobeRady));
				this.m_timer.Start();
			}
		}

		// Token: 0x0600672F RID: 26415 RVA: 0x00218F44 File Offset: 0x00217144
		private void OnEnable()
		{
			bool tempUnlocksEnabled = PlayerCosmeticsSystem.TempUnlocksEnabled;
			this.m_wardrobe.UseTemporarySet = tempUnlocksEnabled;
			this.m_cosmeticAreaTrigger.SetActive(tempUnlocksEnabled);
		}

		// Token: 0x06006730 RID: 26416 RVA: 0x00218F70 File Offset: 0x00217170
		private void CheckWardrobeRady()
		{
			if (CosmeticsController.instance.IsNotNull() && this.m_wardrobe.WardrobeButtonsInitialized())
			{
				this.m_timer.Stop();
				this.m_timer = null;
				base.enabled = true;
				return;
			}
			this.m_timer.Start();
		}

		// Token: 0x040075BC RID: 30140
		[SerializeField]
		private CosmeticWardrobe m_wardrobe;

		// Token: 0x040075BD RID: 30141
		[SerializeField]
		private GameObject m_cosmeticAreaTrigger;

		// Token: 0x040075BE RID: 30142
		private TickSystemTimer m_timer;
	}
}
