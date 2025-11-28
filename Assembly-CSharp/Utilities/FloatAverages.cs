using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	public class FloatAverages : AverageCalculator<float>
	{
		public FloatAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		[MethodImpl(256)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		[MethodImpl(256)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		[MethodImpl(256)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		[MethodImpl(256)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
