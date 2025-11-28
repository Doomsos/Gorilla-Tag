using System;
using UnityEngine;

// Token: 0x02000D03 RID: 3331
public class ZoneGraphBSP : MonoBehaviour
{
	// Token: 0x1700078B RID: 1931
	// (get) Token: 0x060050E0 RID: 20704 RVA: 0x001A1A7A File Offset: 0x0019FC7A
	// (set) Token: 0x060050E1 RID: 20705 RVA: 0x001A1A81 File Offset: 0x0019FC81
	public static ZoneGraphBSP Instance { get; private set; }

	// Token: 0x060050E2 RID: 20706 RVA: 0x001A1A89 File Offset: 0x0019FC89
	private void Awake()
	{
		if (ZoneGraphBSP.Instance == null)
		{
			ZoneGraphBSP.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060050E3 RID: 20707 RVA: 0x001A1AA8 File Offset: 0x0019FCA8
	public void Preprocess()
	{
		BoxCollider[] componentsInChildren = base.GetComponentsInChildren<BoxCollider>(true);
		if (componentsInChildren != null)
		{
			foreach (BoxCollider boxCollider in componentsInChildren)
			{
				if (boxCollider.transform.GetComponent<ZoneDef>() != null)
				{
					Object.Destroy(boxCollider);
				}
				else
				{
					Object.Destroy(boxCollider.gameObject);
				}
			}
		}
	}

	// Token: 0x060050E4 RID: 20708 RVA: 0x001A1AFC File Offset: 0x0019FCFC
	public void CompileBSP()
	{
		ZoneDef[] componentsInChildren = base.gameObject.GetComponentsInChildren<ZoneDef>();
		this.bspTree = BSPTreeBuilder.BuildTree(componentsInChildren);
		if (this.bspTree != null && this.bspTree.nodes != null)
		{
			Debug.Log(string.Format("BSP Tree compiled with {0} zones, {1} nodes", componentsInChildren.Length, this.bspTree.nodes.Length));
			return;
		}
		Debug.Log("BSP Tree compilation failed - no zones found");
	}

	// Token: 0x060050E5 RID: 20709 RVA: 0x001A1B6A File Offset: 0x0019FD6A
	public ZoneDef FindZoneAtPoint(Vector3 worldPoint)
	{
		SerializableBSPTree serializableBSPTree = this.bspTree;
		if (serializableBSPTree == null)
		{
			return null;
		}
		return serializableBSPTree.FindZone(worldPoint);
	}

	// Token: 0x060050E6 RID: 20710 RVA: 0x001A1B7E File Offset: 0x0019FD7E
	public bool IsPointInAnyZone(Vector3 worldPoint)
	{
		return this.FindZoneAtPoint(worldPoint) != null;
	}

	// Token: 0x060050E7 RID: 20711 RVA: 0x001A1B8D File Offset: 0x0019FD8D
	public bool HasCompiledTree()
	{
		return this.bspTree != null && this.bspTree.nodes != null && this.bspTree.nodes.Length != 0;
	}

	// Token: 0x060050E8 RID: 20712 RVA: 0x001A1BB5 File Offset: 0x0019FDB5
	public SerializableBSPTree GetBSPTree()
	{
		return this.bspTree;
	}

	// Token: 0x04006021 RID: 24609
	[SerializeField]
	private SerializableBSPTree bspTree;
}
