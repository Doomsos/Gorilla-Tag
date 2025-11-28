using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000D7B RID: 3451
	public class DoubleAverages : AverageCalculator<double>
	{
		// Token: 0x060054AA RID: 21674 RVA: 0x001AB75E File Offset: 0x001A995E
		public DoubleAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x060054AB RID: 21675 RVA: 0x001AB76D File Offset: 0x001A996D
		[MethodImpl(256)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		// Token: 0x060054AC RID: 21676 RVA: 0x001AB772 File Offset: 0x001A9972
		[MethodImpl(256)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		// Token: 0x060054AD RID: 21677 RVA: 0x001AB777 File Offset: 0x001A9977
		[MethodImpl(256)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		// Token: 0x060054AE RID: 21678 RVA: 0x001AB77D File Offset: 0x001A997D
		[MethodImpl(256)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
