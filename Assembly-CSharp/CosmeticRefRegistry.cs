using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class CosmeticRefRegistry : MonoBehaviour
{
	// Token: 0x06000F22 RID: 3874 RVA: 0x00050690 File Offset: 0x0004E890
	private void Awake()
	{
		foreach (CosmeticRefTarget cosmeticRefTarget in this.builtInRefTargets)
		{
			this.Register(cosmeticRefTarget.id, cosmeticRefTarget.gameObject);
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x000506C8 File Offset: 0x0004E8C8
	public void Register(CosmeticRefID partID, GameObject part)
	{
		this.partsTable[(int)partID] = part;
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x000506D3 File Offset: 0x0004E8D3
	public GameObject Get(CosmeticRefID partID)
	{
		return this.partsTable[(int)partID];
	}

	// Token: 0x0400128D RID: 4749
	private GameObject[] partsTable = new GameObject[8];

	// Token: 0x0400128E RID: 4750
	[SerializeField]
	private CosmeticRefTarget[] builtInRefTargets;
}
