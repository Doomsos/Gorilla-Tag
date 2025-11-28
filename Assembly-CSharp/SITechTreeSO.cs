using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class SITechTreeSO : ScriptableObject
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000865 RID: 2149 RVA: 0x0002D56A File Offset: 0x0002B76A
	// (set) Token: 0x06000866 RID: 2150 RVA: 0x0002D572 File Offset: 0x0002B772
	public List<SITechTreePage> TreePages { get; private set; }

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000867 RID: 2151 RVA: 0x0002D57B File Offset: 0x0002B77B
	// (set) Token: 0x06000868 RID: 2152 RVA: 0x0002D583 File Offset: 0x0002B783
	public int TreePageCount { get; private set; }

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000869 RID: 2153 RVA: 0x0002D58C File Offset: 0x0002B78C
	// (set) Token: 0x0600086A RID: 2154 RVA: 0x0002D594 File Offset: 0x0002B794
	public int[] TreeNodeCounts { get; private set; }

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x0600086B RID: 2155 RVA: 0x0002D59D File Offset: 0x0002B79D
	// (set) Token: 0x0600086C RID: 2156 RVA: 0x0002D5A5 File Offset: 0x0002B7A5
	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x0600086D RID: 2157 RVA: 0x0002D5AE File Offset: 0x0002B7AE
	// (set) Token: 0x0600086E RID: 2158 RVA: 0x0002D5B6 File Offset: 0x0002B7B6
	public bool Initialized { get; private set; }

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x0600086F RID: 2159 RVA: 0x0002D5BF File Offset: 0x0002B7BF
	public List<GameEntity> SpawnableEntities
	{
		get
		{
			this.EnsureInitialized();
			return this._spawnableEntities;
		}
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x0002D5CD File Offset: 0x0002B7CD
	public bool TryGetNode(SIUpgradeType upgradeType, out GraphNode<SITechTreeNode> node)
	{
		return this._nodeLookup.TryGetValue(upgradeType, ref node);
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0002D5DC File Offset: 0x0002B7DC
	public bool IsValidPage(SITechTreePageId id)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0002D640 File Offset: 0x0002B840
	public SITechTreePage GetTreePage(SITechTreePageId id)
	{
		SITechTreePage result;
		if (!this.TryGetTreePage(id, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x0002D65C File Offset: 0x0002B85C
	public bool TryGetTreePage(SITechTreePageId id, out SITechTreePage treePage)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				treePage = sitechTreePage;
				return true;
			}
		}
		treePage = null;
		return false;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0002D6C8 File Offset: 0x0002B8C8
	public bool IsValidNode(int pageId, int nodeId)
	{
		return this.IsValidNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002D6D7 File Offset: 0x0002B8D7
	public bool IsValidNode(SIUpgradeType upgradeType)
	{
		return this._nodeLookup.ContainsKey(upgradeType);
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0002D6E5 File Offset: 0x0002B8E5
	public SITechTreeNode GetTreeNode(int pageId, int nodeId)
	{
		return this.GetTreeNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0002D6F4 File Offset: 0x0002B8F4
	public SITechTreeNode GetTreeNode(SIUpgradeType upgradeType)
	{
		GraphNode<SITechTreeNode> graphNode;
		if (this._nodeLookup.TryGetValue(upgradeType, ref graphNode))
		{
			return graphNode.Value;
		}
		return null;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0002D719 File Offset: 0x0002B919
	public void EnsureInitialized()
	{
		if (!this.Initialized)
		{
			this.InitTechTree();
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0002D72C File Offset: 0x0002B92C
	private void InitTechTree()
	{
		Debug.Log("[SI] SITechTreeSO.InitTechTree");
		this.ClearTechTree();
		this.TreePages = new List<SITechTreePage>();
		this._spawnableEntities = new List<GameEntity>();
		foreach (SITechTreePage sitechTreePage in this.treePages)
		{
			if (sitechTreePage.IsValid)
			{
				sitechTreePage.BuildGraph();
				foreach (GraphNode<SITechTreeNode> graphNode in sitechTreePage.Roots)
				{
					foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrder())
					{
						if (!this._nodeLookup.ContainsKey(graphNode2.Value.upgradeType))
						{
							this._nodeLookup.Add(graphNode2.Value.upgradeType, graphNode2);
						}
					}
				}
				foreach (SITechTreeNode sitechTreeNode in sitechTreePage.DispensableGadgets)
				{
					this.AddSpawnableGadget(sitechTreeNode.unlockedGadgetPrefab);
				}
				if (sitechTreePage.Roots.Count > 0)
				{
					this.TreePages.Add(sitechTreePage);
				}
			}
		}
		this.AllNodes = new List<GraphNode<SITechTreeNode>>(this._nodeLookup.Values);
		this.TreePageCount = Enumerable.Max(Enumerable.Select<SIUpgradeType, int>((SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType)), (SIUpgradeType v) => v.GetPageId())) + 1;
		this.TreeNodeCounts = new int[this.TreePageCount];
		foreach (SIUpgradeType self in (SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType)))
		{
			int pageId = self.GetPageId();
			int nodeId = self.GetNodeId();
			this.TreeNodeCounts[pageId] = Mathf.Max(this.TreeNodeCounts[pageId], nodeId + 1);
		}
		this.Initialized = true;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0002D964 File Offset: 0x0002BB64
	private void AddSpawnableGadget(GameEntity entity)
	{
		this._spawnableEntities.Add(entity);
		IPrefabRequirements component = entity.GetComponent<IPrefabRequirements>();
		if (component != null)
		{
			foreach (GameEntity gameEntity in component.RequiredPrefabs)
			{
				this._spawnableEntities.Add(gameEntity);
			}
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0002D9CC File Offset: 0x0002BBCC
	private void ClearTechTree()
	{
		SITechTreePage[] array = this.treePages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClearGraph();
		}
		this._nodeLookup.Clear();
		this.Initialized = false;
	}

	// Token: 0x04000A3F RID: 2623
	private const string preLog = "[SITechTreeSO]  ";

	// Token: 0x04000A40 RID: 2624
	private const string preErr = "[SITechTreeSO]  ERROR!!!  ";

	// Token: 0x04000A41 RID: 2625
	[HideInInspector]
	[SerializeField]
	private SITechTreePage[] treePages;

	// Token: 0x04000A42 RID: 2626
	private readonly Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>> _nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();

	// Token: 0x04000A48 RID: 2632
	private List<GameEntity> _spawnableEntities;
}
