using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001017 RID: 4119
	public class ListProcessor<T>
	{
		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x0600683A RID: 26682 RVA: 0x0021FA61 File Offset: 0x0021DC61
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x0600683B RID: 26683 RVA: 0x0021FA6E File Offset: 0x0021DC6E
		// (set) Token: 0x0600683C RID: 26684 RVA: 0x0021FA76 File Offset: 0x0021DC76
		public InAction<T> ItemProcessor
		{
			get
			{
				return this.m_itemProcessorDelegate;
			}
			set
			{
				this.m_itemProcessorDelegate = value;
			}
		}

		// Token: 0x0600683D RID: 26685 RVA: 0x0021FA7F File Offset: 0x0021DC7F
		public ListProcessor() : this(10, null)
		{
		}

		// Token: 0x0600683E RID: 26686 RVA: 0x0021FA8A File Offset: 0x0021DC8A
		public ListProcessor(int capacity, InAction<T> itemProcessorDelegate = null)
		{
			this.m_list = new List<T>(capacity);
			this.m_currentIndex = -1;
			this.m_listCount = -1;
			this.m_itemProcessorDelegate = itemProcessorDelegate;
		}

		// Token: 0x0600683F RID: 26687 RVA: 0x0021FAB3 File Offset: 0x0021DCB3
		public void Add(in T item)
		{
			this.m_listCount++;
			this.m_list.Add(item);
		}

		// Token: 0x06006840 RID: 26688 RVA: 0x0021FAD4 File Offset: 0x0021DCD4
		public void Remove(in T item)
		{
			int num = this.m_list.IndexOf(item);
			if (num < 0)
			{
				return;
			}
			if (num < this.m_currentIndex)
			{
				this.m_currentIndex--;
			}
			this.m_listCount--;
			this.m_list.RemoveAt(num);
		}

		// Token: 0x06006841 RID: 26689 RVA: 0x0021FB29 File Offset: 0x0021DD29
		public void Clear()
		{
			this.m_list.Clear();
			this.m_currentIndex = -1;
			this.m_listCount = -1;
		}

		// Token: 0x06006842 RID: 26690 RVA: 0x0021FB44 File Offset: 0x0021DD44
		public bool Contains(in T item)
		{
			return this.m_list.Contains(item);
		}

		// Token: 0x06006843 RID: 26691 RVA: 0x0021FB58 File Offset: 0x0021DD58
		public virtual void ProcessListSafe()
		{
			if (this.m_itemProcessorDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				try
				{
					InAction<T> itemProcessorDelegate = this.m_itemProcessorDelegate;
					T t = this.m_list[this.m_currentIndex];
					itemProcessorDelegate(t);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
				this.m_currentIndex++;
			}
		}

		// Token: 0x06006844 RID: 26692 RVA: 0x0021FBEC File Offset: 0x0021DDEC
		public virtual void ProcessList()
		{
			if (this.m_itemProcessorDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				InAction<T> itemProcessorDelegate = this.m_itemProcessorDelegate;
				T t = this.m_list[this.m_currentIndex];
				itemProcessorDelegate(t);
				this.m_currentIndex++;
			}
		}

		// Token: 0x040076F8 RID: 30456
		protected readonly List<T> m_list;

		// Token: 0x040076F9 RID: 30457
		protected int m_currentIndex;

		// Token: 0x040076FA RID: 30458
		protected int m_listCount;

		// Token: 0x040076FB RID: 30459
		protected InAction<T> m_itemProcessorDelegate;
	}
}
