using System;
using System.Collections;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200100A RID: 4106
	public class CustomMapTestingScript : GorillaPressableButton
	{
		// Token: 0x060067EC RID: 26604 RVA: 0x0021F048 File Offset: 0x0021D248
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x060067ED RID: 26605 RVA: 0x0021F05D File Offset: 0x0021D25D
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}
	}
}
