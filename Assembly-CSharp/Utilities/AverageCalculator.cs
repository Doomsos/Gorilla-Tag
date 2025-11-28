using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000D7A RID: 3450
	public abstract class AverageCalculator<T> where T : struct
	{
		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060054A1 RID: 21665 RVA: 0x001AB61E File Offset: 0x001A981E
		public T Average
		{
			get
			{
				return this.m_average;
			}
		}

		// Token: 0x060054A2 RID: 21666 RVA: 0x001AB626 File Offset: 0x001A9826
		public AverageCalculator(int sampleCount)
		{
			this.m_samples = new T[sampleCount];
		}

		// Token: 0x060054A3 RID: 21667 RVA: 0x001AB63C File Offset: 0x001A983C
		public virtual void AddSample(T sample)
		{
			T sample2 = this.m_samples[this.m_index];
			this.m_total = this.MinusEquals(this.m_total, sample2);
			this.m_total = this.PlusEquals(this.m_total, sample);
			this.m_average = this.Divide(this.m_total, this.m_samples.Length);
			this.m_samples[this.m_index] = sample;
			int num = this.m_index + 1;
			this.m_index = num;
			this.m_index = num % this.m_samples.Length;
		}

		// Token: 0x060054A4 RID: 21668 RVA: 0x001AB6D0 File Offset: 0x001A98D0
		public virtual void Reset()
		{
			T t = this.DefaultTypeValue();
			for (int i = 0; i < this.m_samples.Length; i++)
			{
				this.m_samples[i] = t;
			}
			this.m_index = 0;
			this.m_average = t;
			this.m_total = this.Multiply(t, this.m_samples.Length);
		}

		// Token: 0x060054A5 RID: 21669 RVA: 0x001AB728 File Offset: 0x001A9928
		[MethodImpl(256)]
		protected virtual T DefaultTypeValue()
		{
			return default(T);
		}

		// Token: 0x060054A6 RID: 21670
		[MethodImpl(256)]
		protected abstract T PlusEquals(T value, T sample);

		// Token: 0x060054A7 RID: 21671
		[MethodImpl(256)]
		protected abstract T MinusEquals(T value, T sample);

		// Token: 0x060054A8 RID: 21672
		[MethodImpl(256)]
		protected abstract T Divide(T value, int sampleCount);

		// Token: 0x060054A9 RID: 21673
		[MethodImpl(256)]
		protected abstract T Multiply(T value, int sampleCount);

		// Token: 0x040061D1 RID: 25041
		private T[] m_samples;

		// Token: 0x040061D2 RID: 25042
		private T m_average;

		// Token: 0x040061D3 RID: 25043
		private T m_total;

		// Token: 0x040061D4 RID: 25044
		private int m_index;
	}
}
