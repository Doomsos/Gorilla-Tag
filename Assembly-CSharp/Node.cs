using System;
using System.Collections.Generic;

// Token: 0x0200010C RID: 268
public class Node<T>
{
	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060006D1 RID: 1745 RVA: 0x00025E7F File Offset: 0x0002407F
	// (set) Token: 0x060006D2 RID: 1746 RVA: 0x00025E87 File Offset: 0x00024087
	public T Value { get; set; }

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060006D3 RID: 1747 RVA: 0x00025E90 File Offset: 0x00024090
	// (set) Token: 0x060006D4 RID: 1748 RVA: 0x00025E98 File Offset: 0x00024098
	public Node<T> Parent { get; private set; }

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00025EA1 File Offset: 0x000240A1
	public List<Node<T>> Children { get; } = new List<Node<T>>();

	// Token: 0x060006D6 RID: 1750 RVA: 0x00025EA9 File Offset: 0x000240A9
	public Node(T value)
	{
		this.Value = value;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00025EC4 File Offset: 0x000240C4
	public Node<T> AddChild(T value)
	{
		Node<T> node = new Node<T>(value)
		{
			Parent = this
		};
		this.Children.Add(node);
		return node;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00025EEC File Offset: 0x000240EC
	public Node<T> AddChild(Node<T> child)
	{
		Node<T> parent = child.Parent;
		if (parent != null)
		{
			parent.RemoveChild(child);
		}
		this.Children.Add(child);
		child.Parent = this;
		return child;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00025F14 File Offset: 0x00024114
	public void RemoveChild(Node<T> child)
	{
		if (this.Children.Remove(child))
		{
			child.Parent = null;
		}
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00025F2B File Offset: 0x0002412B
	public IEnumerable<Node<T>> TraversePreOrder()
	{
		yield return this;
		foreach (Node<T> node in this.Children)
		{
			foreach (Node<T> node2 in node.TraversePreOrder())
			{
				yield return node2;
			}
			IEnumerator<Node<T>> enumerator2 = null;
		}
		List<Node<T>>.Enumerator enumerator = default(List<Node<T>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00025F3B File Offset: 0x0002413B
	public IEnumerable<Node<T>> TraverseBreadthFirst()
	{
		Queue<Node<T>> queue = new Queue<Node<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			Node<T> current = queue.Dequeue();
			yield return current;
			foreach (Node<T> node in current.Children)
			{
				queue.Enqueue(node);
			}
			current = null;
		}
		yield break;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00025F4C File Offset: 0x0002414C
	public List<Node<T>> GetPath()
	{
		List<Node<T>> list = new List<Node<T>>();
		list.Add(this);
		List<Node<T>> list2 = list;
		for (Node<T> parent = this.Parent; parent != null; parent = parent.Parent)
		{
			list2.Insert(0, parent);
		}
		return list2;
	}
}
