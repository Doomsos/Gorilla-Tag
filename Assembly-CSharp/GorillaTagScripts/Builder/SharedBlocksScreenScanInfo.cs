using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E8C RID: 3724
	public class SharedBlocksScreenScanInfo : SharedBlocksScreen
	{
		// Token: 0x06005D04 RID: 23812 RVA: 0x00002789 File Offset: 0x00000989
		public override void OnUpPressed()
		{
		}

		// Token: 0x06005D05 RID: 23813 RVA: 0x00002789 File Offset: 0x00000989
		public override void OnDownPressed()
		{
		}

		// Token: 0x06005D06 RID: 23814 RVA: 0x001DDBA4 File Offset: 0x001DBDA4
		public override void OnSelectPressed()
		{
			this.terminal.OnLoadMapPressed();
		}

		// Token: 0x06005D07 RID: 23815 RVA: 0x001DDBB1 File Offset: 0x001DBDB1
		public override void Show()
		{
			base.Show();
			this.DrawScreen();
		}

		// Token: 0x06005D08 RID: 23816 RVA: 0x001DDBC0 File Offset: 0x001DBDC0
		private void DrawScreen()
		{
			if (this.terminal.SelectedMap == null)
			{
				this.mapIDText.text = "MAP ID: NONE";
				return;
			}
			this.mapIDText.text = "MAP ID: " + SharedBlocksTerminal.MapIDToDisplayedString(this.terminal.SelectedMap.MapID);
		}

		// Token: 0x04006ABB RID: 27323
		[SerializeField]
		private TMP_Text mapIDText;
	}
}
