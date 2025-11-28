using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000140 RID: 320
[Serializable]
public class SITechTreePage
{
	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000885 RID: 2181 RVA: 0x0002DA86 File Offset: 0x0002BC86
	// (set) Token: 0x06000886 RID: 2182 RVA: 0x0002DA8E File Offset: 0x0002BC8E
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000887 RID: 2183 RVA: 0x0002DA98 File Offset: 0x0002BC98
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			if (edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				SITechTreeNode[] array = this.treeNodes;
				return array != null && array.Length != 0;
			}
			return false;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000888 RID: 2184 RVA: 0x0002DAC5 File Offset: 0x0002BCC5
	// (set) Token: 0x06000889 RID: 2185 RVA: 0x0002DACD File Offset: 0x0002BCCD
	public List<GraphNode<SITechTreeNode>> Roots { get; private set; }

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600088A RID: 2186 RVA: 0x0002DAD6 File Offset: 0x0002BCD6
	// (set) Token: 0x0600088B RID: 2187 RVA: 0x0002DADE File Offset: 0x0002BCDE
	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600088C RID: 2188 RVA: 0x0002DAE7 File Offset: 0x0002BCE7
	// (set) Token: 0x0600088D RID: 2189 RVA: 0x0002DAEF File Offset: 0x0002BCEF
	public List<SITechTreeNode> DispensableGadgets { get; private set; }

	// Token: 0x0600088E RID: 2190 RVA: 0x0002DAF8 File Offset: 0x0002BCF8
	public void ClearGraph()
	{
		this.Roots = null;
		this.AllNodes = null;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002DB08 File Offset: 0x0002BD08
	public void BuildGraph()
	{
		SITechTreePage.<>c__DisplayClass22_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.Roots = new List<GraphNode<SITechTreeNode>>();
		this.AllNodes = new List<GraphNode<SITechTreeNode>>();
		this.DispensableGadgets = new List<SITechTreeNode>();
		if (!this.IsValid)
		{
			return;
		}
		CS$<>8__locals1.nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && (sitechTreeNode.parentUpgrades == null || sitechTreeNode.parentUpgrades.Length == 0))
			{
				this.Roots.Add(this.<BuildGraph>g__PopulateGraph|22_0(sitechTreeNode, ref CS$<>8__locals1));
			}
		}
		foreach (GraphNode<SITechTreeNode> graphNode in this.AllNodes)
		{
			if (graphNode.Value.IsDispensableGadget)
			{
				this.DispensableGadgets.Add(graphNode.Value);
			}
		}
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0002DBFC File Offset: 0x0002BDFC
	public void PrintGraph()
	{
		foreach (GraphNode<SITechTreeNode> graphNode in this.Roots)
		{
			foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrderDistinct(null))
			{
				Debug.Log(string.Concat(new string[]
				{
					"[SI] Graph node: ",
					graphNode2.Value.nickName,
					" [",
					SITechTreePage.<PrintGraph>g__NodeListText|23_2(graphNode2.Parents),
					"]"
				}));
			}
		}
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0002DCD0 File Offset: 0x0002BED0
	[CompilerGenerated]
	private GraphNode<SITechTreeNode> <BuildGraph>g__PopulateGraph|22_0(SITechTreeNode node, ref SITechTreePage.<>c__DisplayClass22_0 A_2)
	{
		GraphNode<SITechTreeNode> graphNode;
		if (!A_2.nodeLookup.TryGetValue(node.upgradeType, ref graphNode))
		{
			graphNode = new GraphNode<SITechTreeNode>(node);
			A_2.nodeLookup.Add(node.upgradeType, graphNode);
			this.AllNodes.Add(graphNode);
		}
		SIUpgradeType upgradeType = node.upgradeType;
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && sitechTreeNode.parentUpgrades != null)
			{
				SIUpgradeType[] parentUpgrades = sitechTreeNode.parentUpgrades;
				for (int j = 0; j < parentUpgrades.Length; j++)
				{
					if (parentUpgrades[j] == upgradeType)
					{
						GraphNode<SITechTreeNode> graphNode2 = this.<BuildGraph>g__PopulateGraph|22_0(sitechTreeNode, ref A_2);
						if (!graphNode.Children.Contains(graphNode2))
						{
							graphNode.AddChild(graphNode2);
						}
					}
				}
			}
		}
		return graphNode;
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002DD92 File Offset: 0x0002BF92
	[CompilerGenerated]
	internal static string[] <PrintGraph>g__GetChildText|23_0(GraphNode<SITechTreeNode> root)
	{
		return Enumerable.ToArray<string>(Enumerable.Select<GraphNode<SITechTreeNode>, string>(root.TraversePreOrder(), (GraphNode<SITechTreeNode> n) => SITechTreePage.<PrintGraph>g__NodeText|23_1(n)));
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002DDC4 File Offset: 0x0002BFC4
	[CompilerGenerated]
	internal static string <PrintGraph>g__NodeText|23_1(GraphNode<SITechTreeNode> graphNode)
	{
		return string.Concat(new string[]
		{
			SITechTreePage.<PrintGraph>g__NodeListText|23_2(graphNode.Parents),
			" >> ",
			graphNode.Value.nickName,
			" << ",
			SITechTreePage.<PrintGraph>g__NodeListText|23_2(graphNode.Children)
		});
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0002DE16 File Offset: 0x0002C016
	[CompilerGenerated]
	internal static string <PrintGraph>g__NodeListText|23_2(List<GraphNode<SITechTreeNode>> nodes)
	{
		return string.Join("|", Enumerable.Select<GraphNode<SITechTreeNode>, string>(nodes, (GraphNode<SITechTreeNode> n) => n.Value.nickName));
	}

	// Token: 0x04000A53 RID: 2643
	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	// Token: 0x04000A54 RID: 2644
	public string nickName;

	// Token: 0x04000A55 RID: 2645
	public SITechTreePageId pageId;

	// Token: 0x04000A56 RID: 2646
	[SerializeField]
	private SITechTreeNode[] treeNodes;
}
