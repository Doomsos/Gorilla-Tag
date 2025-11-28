using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E44 RID: 3652
	public class BuilderAnimateOnTap : BuilderPieceTappable
	{
		// Token: 0x06005AFF RID: 23295 RVA: 0x001D25EF File Offset: 0x001D07EF
		public override void OnTapReplicated()
		{
			base.OnTapReplicated();
			this.anim.Rewind();
			this.anim.Play();
		}

		// Token: 0x04006822 RID: 26658
		[SerializeField]
		private Animation anim;
	}
}
