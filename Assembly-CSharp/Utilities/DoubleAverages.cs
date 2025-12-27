using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	public class DoubleAverages : AverageCalculator<double>
	{
		public DoubleAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		[MethodImpl(256)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		[MethodImpl(256)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		[MethodImpl(256)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		[MethodImpl(256)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
