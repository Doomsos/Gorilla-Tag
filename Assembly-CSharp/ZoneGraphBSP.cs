using System;
using UnityEngine;

// Token: 0x02000D03 RID: 3331
public class ZoneGraphBSP : MonoBehaviour
{
	// Token: 0x1700078B RID: 1931
	// (get) Token: 0x060050E0 RID: 20704 RVA: 0x001A1A9A File Offset: 0x0019FC9A
	// (set) Token: 0x060050E1 RID: 20705 RVA: 0x001A1AA1 File Offset: 0x0019FCA1
	public static ZoneGraphBSP Instance { get; private set; }

	// Token: 0x060050E2 RID: 20706 RVA: 0x001A1AA9 File Offset: 0x0019FCA9
	private void Awake()
	{
		if (ZoneGraphBSP.Instance == null)
		{
			ZoneGraphBSP.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060050E3 RID: 20707 RVA: 0x001A1AC8 File Offset: 0x0019FCC8
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

	// Token: 0x060050E4 RID: 20708 RVA: 0x001A1B1C File Offset: 0x0019FD1C
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

	// Token: 0x060050E5 RID: 20709 RVA: 0x001A1B8A File Offset: 0x0019FD8A
	public ZoneDef FindZoneAtPoint(Vector3 worldPoint)
	{
		SerializableBSPTree serializableBSPTree = this.bspTree;
		if (serializableBSPTree == null)
		{
			return null;
		}
		return serializableBSPTree.FindZone(worldPoint);
	}

	// Token: 0x060050E6 RID: 20710 RVA: 0x001A1B9E File Offset: 0x0019FD9E
	public bool IsPointInAnyZone(Vector3 worldPoint)
	{
		return this.FindZoneAtPoint(worldPoint) != null;
	}

	// Token: 0x060050E7 RID: 20711 RVA: 0x001A1BAD File Offset: 0x0019FDAD
	public bool HasCompiledTree()
	{
		return this.bspTree != null && this.bspTree.nodes != null && this.bspTree.nodes.Length != 0;
	}

	// Token: 0x060050E8 RID: 20712 RVA: 0x001A1BD5 File Offset: 0x0019FDD5
	public SerializableBSPTree GetBSPTree()
	{
		return this.bspTree;
	}

	// Token: 0x04006021 RID: 24609
	[SerializeField]
	private SerializableBSPTree bspTree;
}
