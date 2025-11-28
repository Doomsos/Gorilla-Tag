using System;
using System.Collections.Generic;

// Token: 0x020009F2 RID: 2546
public class RingBuffer<T>
{
	// Token: 0x17000606 RID: 1542
	// (get) Token: 0x060040DE RID: 16606 RVA: 0x0015A94E File Offset: 0x00158B4E
	public int Size
	{
		get
		{
			return this._size;
		}
	}

	// Token: 0x17000607 RID: 1543
	// (get) Token: 0x060040DF RID: 16607 RVA: 0x0015A956 File Offset: 0x00158B56
	public int Capacity
	{
		get
		{
			return this._capacity;
		}
	}

	// Token: 0x17000608 RID: 1544
	// (get) Token: 0x060040E0 RID: 16608 RVA: 0x0015A95E File Offset: 0x00158B5E
	public bool IsFull
	{
		get
		{
			return this._size == this._capacity;
		}
	}

	// Token: 0x17000609 RID: 1545
	// (get) Token: 0x060040E1 RID: 16609 RVA: 0x0015A96E File Offset: 0x00158B6E
	public bool IsEmpty
	{
		get
		{
			return this._size == 0;
		}
	}

	// Token: 0x060040E2 RID: 16610 RVA: 0x0015A979 File Offset: 0x00158B79
	public RingBuffer(int capacity)
	{
		if (capacity < 1)
		{
			throw new ArgumentException("Can't be zero or negative", "capacity");
		}
		this._size = 0;
		this._capacity = capacity;
		this._items = new T[capacity];
	}

	// Token: 0x060040E3 RID: 16611 RVA: 0x0015A9AF File Offset: 0x00158BAF
	public RingBuffer(IList<T> list) : this(list.Count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		list.CopyTo(this._items, 0);
	}

	// Token: 0x060040E4 RID: 16612 RVA: 0x0015A9D8 File Offset: 0x00158BD8
	public ref T PeekFirst()
	{
		return ref this._items[this._head];
	}

	// Token: 0x060040E5 RID: 16613 RVA: 0x0015A9EB File Offset: 0x00158BEB
	public ref T PeekLast()
	{
		return ref this._items[this._tail];
	}

	// Token: 0x060040E6 RID: 16614 RVA: 0x0015AA00 File Offset: 0x00158C00
	public bool Push(T item)
	{
		if (this._size == this._capacity)
		{
			return false;
		}
		this._items[this._tail] = item;
		this._tail = (this._tail + 1) % this._capacity;
		this._size++;
		return true;
	}

	// Token: 0x060040E7 RID: 16615 RVA: 0x0015AA54 File Offset: 0x00158C54
	public T Pop()
	{
		if (this._size == 0)
		{
			return default(T);
		}
		T result = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return result;
	}

	// Token: 0x060040E8 RID: 16616 RVA: 0x0015AAA8 File Offset: 0x00158CA8
	public bool TryPop(out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return true;
	}

	// Token: 0x060040E9 RID: 16617 RVA: 0x0015AB01 File Offset: 0x00158D01
	public void Clear()
	{
		this._head = 0;
		this._tail = 0;
		this._size = 0;
		Array.Clear(this._items, 0, this._capacity);
	}

	// Token: 0x060040EA RID: 16618 RVA: 0x0015AB2A File Offset: 0x00158D2A
	public bool TryGet(int i, out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head + i % this._size];
		return true;
	}

	// Token: 0x060040EB RID: 16619 RVA: 0x0015AB5E File Offset: 0x00158D5E
	public ArraySegment<T> AsSegment()
	{
		return new ArraySegment<T>(this._items);
	}

	// Token: 0x04005213 RID: 21011
	private T[] _items;

	// Token: 0x04005214 RID: 21012
	private int _head;

	// Token: 0x04005215 RID: 21013
	private int _tail;

	// Token: 0x04005216 RID: 21014
	private int _size;

	// Token: 0x04005217 RID: 21015
	private readonly int _capacity;
}
