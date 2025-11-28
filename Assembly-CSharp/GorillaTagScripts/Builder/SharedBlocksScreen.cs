using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E8B RID: 3723
	public class SharedBlocksScreen : MonoBehaviour
	{
		// Token: 0x06005CFB RID: 23803 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void OnUpPressed()
		{
		}

		// Token: 0x06005CFC RID: 23804 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void OnDownPressed()
		{
		}

		// Token: 0x06005CFD RID: 23805 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void OnSelectPressed()
		{
		}

		// Token: 0x06005CFE RID: 23806 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void OnDeletePressed()
		{
		}

		// Token: 0x06005CFF RID: 23807 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void OnNumberPressed(int number)
		{
		}

		// Token: 0x06005D00 RID: 23808 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void OnLetterPressed(string letter)
		{
		}

		// Token: 0x06005D01 RID: 23809 RVA: 0x001DDB6E File Offset: 0x001DBD6E
		public virtual void Show()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005D02 RID: 23810 RVA: 0x001DDB89 File Offset: 0x001DBD89
		public virtual void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04006AB9 RID: 27321
		public SharedBlocksTerminal.ScreenType screenType;

		// Token: 0x04006ABA RID: 27322
		public SharedBlocksTerminal terminal;
	}
}
