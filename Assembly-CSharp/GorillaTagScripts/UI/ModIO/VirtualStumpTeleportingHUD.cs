using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000E2F RID: 3631
	public class VirtualStumpTeleportingHUD : MonoBehaviour
	{
		// Token: 0x06005A99 RID: 23193 RVA: 0x001D0CA0 File Offset: 0x001CEEA0
		public void Initialize(bool isEntering)
		{
			this.isEnteringVirtualStump = isEntering;
			if (isEntering)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("VIRT_STUMP_HUD_ENTERING", out text, this.enteringVirtualStumpString))
				{
					Debug.LogError("[LOCALIZATION::VIRT_STUMP_TELEPORT_HUD] Failed to retrieve key [VIRT_STUMP_HUD_ENTERING] for locale [" + LocalisationManager.CurrentLanguage.LocaleName + "]");
				}
				this.teleportingStatusText.text = text;
				this.teleportingStatusText.gameObject.SetActive(true);
				return;
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("VIRT_STUMP_HUD_LEAVING", out text2, this.leavingVirtualStumpString))
			{
				Debug.LogError("[LOCALIZATION::VIRT_STUMP_TELEPORT_HUD] Failed to retrieve key [VIRT_STUMP_HUD_LEAVING] for locale [" + LocalisationManager.CurrentLanguage.LocaleName + "]");
			}
			this.teleportingStatusText.text = text2;
			this.teleportingStatusText.gameObject.SetActive(true);
		}

		// Token: 0x06005A9A RID: 23194 RVA: 0x001D0D58 File Offset: 0x001CEF58
		private void Update()
		{
			if (Time.time - this.lastTextUpdateTime > this.textUpdateInterval)
			{
				this.lastTextUpdateTime = Time.time;
				this.IncrementProgressDots();
				this.teleportingStatusText.text = (this.isEnteringVirtualStump ? this.enteringVirtualStumpString : this.leavingVirtualStumpString);
				for (int i = 0; i < this.numProgressDots; i++)
				{
					TMP_Text tmp_Text = this.teleportingStatusText;
					tmp_Text.text += ".";
				}
			}
		}

		// Token: 0x06005A9B RID: 23195 RVA: 0x001D0DD7 File Offset: 0x001CEFD7
		private void IncrementProgressDots()
		{
			this.numProgressDots++;
			if (this.numProgressDots > this.maxNumProgressDots)
			{
				this.numProgressDots = 0;
			}
		}

		// Token: 0x040067D7 RID: 26583
		private const string VIRT_STUMP_HUD_ENTERING_KEY = "VIRT_STUMP_HUD_ENTERING";

		// Token: 0x040067D8 RID: 26584
		private const string VIRT_STUMP_HUD_LEAVING_KEY = "VIRT_STUMP_HUD_LEAVING";

		// Token: 0x040067D9 RID: 26585
		[SerializeField]
		private string enteringVirtualStumpString = "Now Entering the Virtual Stump";

		// Token: 0x040067DA RID: 26586
		[SerializeField]
		private string leavingVirtualStumpString = "Now Leaving the Virtual Stump";

		// Token: 0x040067DB RID: 26587
		[SerializeField]
		private TMP_Text teleportingStatusText;

		// Token: 0x040067DC RID: 26588
		[SerializeField]
		private int maxNumProgressDots = 3;

		// Token: 0x040067DD RID: 26589
		[SerializeField]
		private float textUpdateInterval = 0.5f;

		// Token: 0x040067DE RID: 26590
		private float lastTextUpdateTime;

		// Token: 0x040067DF RID: 26591
		private int numProgressDots;

		// Token: 0x040067E0 RID: 26592
		private bool isEnteringVirtualStump;
	}
}
