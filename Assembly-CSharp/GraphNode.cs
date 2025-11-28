using System;
using System.Collections.Generic;

// Token: 0x02000106 RID: 262
public class GraphNode<T>
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000694 RID: 1684 RVA: 0x000253DB File Offset: 0x000235DB
	// (set) Token: 0x06000695 RID: 1685 RVA: 0x000253E3 File Offset: 0x000235E3
	public T Value { get; set; }

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x06000696 RID: 1686 RVA: 0x000253EC File Offset: 0x000235EC
	public List<GraphNode<T>> Parents { get; } = new List<GraphNode<T>>();

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000697 RID: 1687 RVA: 0x000253F4 File Offset: 0x000235F4
	public List<GraphNode<T>> Children { get; } = new List<GraphNode<T>>();

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000698 RID: 1688 RVA: 0x000253FC File Offset: 0x000235FC
	public int ChildCount
	{
		get
		{
			return this.Children.Count;
		}
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00025409 File Offset: 0x00023609
	public GraphNode(T value)
	{
		this.Value = value;
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0002542E File Offset: 0x0002362E
	public GraphNode(T value, GraphNode<T> parent)
	{
		this.Value = value;
		this.Parents.Add(parent);
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00025460 File Offset: 0x00023660
	public int GetSubtreeWidth(int depthLimit = 2147483647)
	{
		if (this.ChildCount == 0 || depthLimit == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			num += graphNode.GetSubtreeWidth(depthLimit - 1);
		}
		return num;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x000254C8 File Offset: 0x000236C8
	public GraphNode<T> AddChild(T value)
	{
		return this.AddChild(new GraphNode<T>(value));
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x000254D6 File Offset: 0x000236D6
	public GraphNode<T> AddChild(GraphNode<T> child)
	{
		if (child.Parents.Contains(this))
		{
			throw new InvalidOperationException("Cannot add child more than once");
		}
		this.Children.Add(child);
		child.Parents.Add(this);
		return child;
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0002550A File Offset: 0x0002370A
	public void RemoveChild(GraphNode<T> child)
	{
		if (this.Children.Remove(child))
		{
			child.Parents.Remove(this);
		}
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00025527 File Offset: 0x00023727
	public IEnumerable<GraphNode<T>> TraversePreOrder()
	{
		yield return this;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			foreach (GraphNode<T> graphNode2 in graphNode.TraversePreOrder())
			{
				yield return graphNode2;
			}
			IEnumerator<GraphNode<T>> enumerator2 = null;
		}
		List<GraphNode<T>>.Enumerator enumerator = default(List<GraphNode<T>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00025537 File Offset: 0x00023737
	public IEnumerable<GraphNode<T>> TraversePreOrderDistinct(HashSet<GraphNode<T>> visited = null)
	{
		if (visited == null)
		{
			visited = new HashSet<GraphNode<T>>();
		}
		if (!visited.Contains(this))
		{
			yield return this;
			visited.Add(this);
			foreach (GraphNode<T> graphNode in this.Children)
			{
				foreach (GraphNode<T> graphNode2 in graphNode.TraversePreOrderDistinct(visited))
				{
					yield return graphNode2;
				}
				IEnumerator<GraphNode<T>> enumerator2 = null;
			}
			List<GraphNode<T>>.Enumerator enumerator = default(List<GraphNode<T>>.Enumerator);
		}
		yield break;
		yield break;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0002554E File Offset: 0x0002374E
	public IEnumerable<GraphNode<T>> TraverseBreadthFirst()
	{
		Queue<GraphNode<T>> queue = new Queue<GraphNode<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			GraphNode<T> current = queue.Dequeue();
			yield return current;
			foreach (GraphNode<T> graphNode in current.Children)
			{
				queue.Enqueue(graphNode);
			}
			current = null;
		}
		yield break;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0002555E File Offset: 0x0002375E
	public IEnumerable<GraphNode<T>> TraverseBreadthFirstDistinct()
	{
		Queue<GraphNode<T>> queue = new Queue<GraphNode<T>>();
		HashSet<GraphNode<T>> visited = new HashSet<GraphNode<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			GraphNode<T> current = queue.Dequeue();
			if (!visited.Contains(current))
			{
				visited.Add(current);
				yield return current;
				foreach (GraphNode<T> graphNode in current.Children)
				{
					queue.Enqueue(graphNode);
				}
				current = null;
			}
		}
		yield break;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00025570 File Offset: 0x00023770
	public int GetGraphDepth()
	{
		if (this.Children.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			num = Math.Max(num, graphNode.GetGraphDepth());
		}
		return num + 1;
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000255E0 File Offset: 0x000237E0
	public int GetNodeDepth()
	{
		if (this.Parents.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Parents)
		{
			num = Math.Max(num, graphNode.GetNodeDepth());
		}
		return num + 1;
	}
}
