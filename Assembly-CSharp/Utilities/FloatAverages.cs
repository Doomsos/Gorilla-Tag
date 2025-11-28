using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000D7C RID: 3452
	public class FloatAverages : AverageCalculator<float>
	{
		// Token: 0x060054AF RID: 21679 RVA: 0x001AB783 File Offset: 0x001A9983
		public FloatAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x060054B0 RID: 21680 RVA: 0x001AB76D File Offset: 0x001A996D
		[MethodImpl(256)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		// Token: 0x060054B1 RID: 21681 RVA: 0x001AB772 File Offset: 0x001A9972
		[MethodImpl(256)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		// Token: 0x060054B2 RID: 21682 RVA: 0x001AB792 File Offset: 0x001A9992
		[MethodImpl(256)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		// Token: 0x060054B3 RID: 21683 RVA: 0x001AB798 File Offset: 0x001A9998
		[MethodImpl(256)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
