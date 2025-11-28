using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200105C RID: 4188
	[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		// Token: 0x06006950 RID: 26960 RVA: 0x00027DED File Offset: 0x00025FED
		private bool ShowPropHuntWeight()
		{
			return true;
		}

		// Token: 0x06006951 RID: 26961 RVA: 0x0022433B File Offset: 0x0022253B
		public void OnEnable()
		{
			this.info.debugCosmeticSOName = base.name;
		}

		// Token: 0x04007836 RID: 30774
		public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");

		// Token: 0x04007837 RID: 30775
		public int propHuntWeight = 1;
	}
}
