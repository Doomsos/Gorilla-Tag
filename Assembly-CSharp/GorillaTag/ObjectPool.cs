using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x0200101A RID: 4122
	public class ObjectPool<T> where T : ObjectPoolEvents, new()
	{
		// Token: 0x0600684A RID: 26698 RVA: 0x0021FC99 File Offset: 0x0021DE99
		protected ObjectPool()
		{
		}

		// Token: 0x0600684B RID: 26699 RVA: 0x0021FCAC File Offset: 0x0021DEAC
		public ObjectPool(int amount) : this(amount, amount)
		{
		}

		// Token: 0x0600684C RID: 26700 RVA: 0x0021FCB6 File Offset: 0x0021DEB6
		public ObjectPool(int initialAmount, int maxAmount)
		{
			this.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x0600684D RID: 26701 RVA: 0x0021FCD4 File Offset: 0x0021DED4
		protected void InitializePool(int initialAmount, int maxAmount)
		{
			this.maxInstances = maxAmount;
			this.pool = new Stack<T>(initialAmount);
			for (int i = 0; i < initialAmount; i++)
			{
				this.pool.Push(this.CreateInstance());
			}
		}

		// Token: 0x0600684E RID: 26702 RVA: 0x0021FD14 File Offset: 0x0021DF14
		public T Take()
		{
			T result;
			if (this.pool.Count < 1)
			{
				result = this.CreateInstance();
			}
			else
			{
				result = this.pool.Pop();
			}
			result.OnTaken();
			return result;
		}

		// Token: 0x0600684F RID: 26703 RVA: 0x0021FD52 File Offset: 0x0021DF52
		public void Return(T instance)
		{
			instance.OnReturned();
			if (this.pool.Count == this.maxInstances)
			{
				return;
			}
			this.pool.Push(instance);
		}

		// Token: 0x06006850 RID: 26704 RVA: 0x0021FD81 File Offset: 0x0021DF81
		[MethodImpl(256)]
		public virtual T CreateInstance()
		{
			return Activator.CreateInstance<T>();
		}

		// Token: 0x040076FC RID: 30460
		private Stack<T> pool;

		// Token: 0x040076FD RID: 30461
		public int maxInstances = 500;
	}
}
