using System;
using UnityEngine;

namespace CosmeticRoom.ItemScripts
{
	// Token: 0x02000EA4 RID: 3748
	public class TrickTreatHoldable : TransferrableObject
	{
		// Token: 0x06005DB7 RID: 23991 RVA: 0x001E169D File Offset: 0x001DF89D
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.candyCollider)
			{
				this.candyCollider.enabled = (this.IsMyItem() && this.IsHeld());
			}
		}

		// Token: 0x04006BBF RID: 27583
		public MeshCollider candyCollider;
	}
}
