using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000D7D RID: 3453
	public class NetTimeAverages : DoubleAverages
	{
		// Token: 0x060054B4 RID: 21684 RVA: 0x001AB77E File Offset: 0x001A997E
		public NetTimeAverages(int sampleCount) : base(sampleCount)
		{
		}

		// Token: 0x060054B5 RID: 21685 RVA: 0x001AB787 File Offset: 0x001A9987
		[MethodImpl(256)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
