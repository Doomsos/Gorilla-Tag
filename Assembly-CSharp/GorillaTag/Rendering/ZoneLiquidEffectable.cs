using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001083 RID: 4227
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x060069EA RID: 27114 RVA: 0x00228583 File Offset: 0x00226783
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x060069EB RID: 27115 RVA: 0x00002789 File Offset: 0x00000989
		private void OnEnable()
		{
		}

		// Token: 0x060069EC RID: 27116 RVA: 0x00002789 File Offset: 0x00000989
		private void OnDisable()
		{
		}

		// Token: 0x04007948 RID: 31048
		public float radius = 1f;

		// Token: 0x04007949 RID: 31049
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x0400794A RID: 31050
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x0400794B RID: 31051
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
