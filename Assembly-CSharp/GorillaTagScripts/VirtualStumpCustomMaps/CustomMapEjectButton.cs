using System;
using System.Collections;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E14 RID: 3604
	public class CustomMapEjectButton : GorillaPressableButton
	{
		// Token: 0x060059EB RID: 23019 RVA: 0x001CC358 File Offset: 0x001CA558
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
			if (!this.processing)
			{
				this.HandleTeleport();
			}
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x001CC37B File Offset: 0x001CA57B
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}

		// Token: 0x060059ED RID: 23021 RVA: 0x001CC38C File Offset: 0x001CA58C
		private void HandleTeleport()
		{
			if (this.processing)
			{
				return;
			}
			this.processing = true;
			CustomMapEjectButton.EjectType ejectType = this.ejectType;
			if (ejectType != CustomMapEjectButton.EjectType.EjectFromVirtualStump)
			{
				if (ejectType == CustomMapEjectButton.EjectType.ReturnToVirtualStump)
				{
					CustomMapManager.ReturnToVirtualStump();
					this.processing = false;
					return;
				}
			}
			else
			{
				CustomMapManager.ExitVirtualStump(new Action<bool>(this.FinishTeleport));
			}
		}

		// Token: 0x060059EE RID: 23022 RVA: 0x001CC3D5 File Offset: 0x001CA5D5
		private void FinishTeleport(bool success = true)
		{
			if (!this.processing)
			{
				return;
			}
			this.processing = false;
		}

		// Token: 0x060059EF RID: 23023 RVA: 0x001CC3E7 File Offset: 0x001CA5E7
		public void CopySettings(CustomMapEjectButtonSettings customMapEjectButtonSettings)
		{
			this.ejectType = customMapEjectButtonSettings.ejectType;
		}

		// Token: 0x040066FA RID: 26362
		[SerializeField]
		private CustomMapEjectButton.EjectType ejectType;

		// Token: 0x040066FB RID: 26363
		private bool processing;

		// Token: 0x02000E15 RID: 3605
		public enum EjectType
		{
			// Token: 0x040066FD RID: 26365
			EjectFromVirtualStump,
			// Token: 0x040066FE RID: 26366
			ReturnToVirtualStump
		}
	}
}
