using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	public class NetTimeAverages : DoubleAverages
	{
		public NetTimeAverages(int sampleCount) : base(sampleCount)
		{
		}

		[MethodImpl(256)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
