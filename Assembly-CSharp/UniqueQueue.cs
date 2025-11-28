using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000C90 RID: 3216
public class UniqueQueue<T> : IEnumerable<!0>, IEnumerable
{
	// Token: 0x17000749 RID: 1865
	// (get) Token: 0x06004E90 RID: 20112 RVA: 0x00196F8B File Offset: 0x0019518B
	public int Count
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x06004E91 RID: 20113 RVA: 0x00196F98 File Offset: 0x00195198
	public UniqueQueue()
	{
		this.queuedItems = new HashSet<T>();
		this.queue = new Queue<T>();
	}

	// Token: 0x06004E92 RID: 20114 RVA: 0x00196FB6 File Offset: 0x001951B6
	public UniqueQueue(int capacity)
	{
		this.queuedItems = new HashSet<T>(capacity);
		this.queue = new Queue<T>(capacity);
	}

	// Token: 0x06004E93 RID: 20115 RVA: 0x00196FD6 File Offset: 0x001951D6
	public void Clear()
	{
		this.queuedItems.Clear();
		this.queue.Clear();
	}

	// Token: 0x06004E94 RID: 20116 RVA: 0x00196FEE File Offset: 0x001951EE
	public bool Enqueue(T item)
	{
		if (!this.queuedItems.Add(item))
		{
			return false;
		}
		this.queue.Enqueue(item);
		return true;
	}

	// Token: 0x06004E95 RID: 20117 RVA: 0x00197010 File Offset: 0x00195210
	public T Dequeue()
	{
		T t = this.queue.Dequeue();
		this.queuedItems.Remove(t);
		return t;
	}

	// Token: 0x06004E96 RID: 20118 RVA: 0x00197037 File Offset: 0x00195237
	public bool TryDequeue(out T item)
	{
		if (this.queue.Count < 1)
		{
			item = default(T);
			return false;
		}
		item = this.Dequeue();
		return true;
	}

	// Token: 0x06004E97 RID: 20119 RVA: 0x0019705D File Offset: 0x0019525D
	public T Peek()
	{
		return this.queue.Peek();
	}

	// Token: 0x06004E98 RID: 20120 RVA: 0x0019706A File Offset: 0x0019526A
	public bool Contains(T item)
	{
		return this.queuedItems.Contains(item);
	}

	// Token: 0x06004E99 RID: 20121 RVA: 0x00197078 File Offset: 0x00195278
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x06004E9A RID: 20122 RVA: 0x00197078 File Offset: 0x00195278
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x04005D75 RID: 23925
	private HashSet<T> queuedItems;

	// Token: 0x04005D76 RID: 23926
	private Queue<T> queue;
}
