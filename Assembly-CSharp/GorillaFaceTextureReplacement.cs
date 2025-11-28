using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class GorillaFaceTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000F88 RID: 3976 RVA: 0x000524AF File Offset: 0x000506AF
	// (set) Token: 0x06000F89 RID: 3977 RVA: 0x000524B7 File Offset: 0x000506B7
	public bool IsSpawned { get; set; }

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000F8A RID: 3978 RVA: 0x000524C0 File Offset: 0x000506C0
	// (set) Token: 0x06000F8B RID: 3979 RVA: 0x000524C8 File Offset: 0x000506C8
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000F8C RID: 3980 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x000524D1 File Offset: 0x000506D1
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x000524DC File Offset: 0x000506DC
	private void OnEnable()
	{
		Material sharedMaterial = this.myRig.GetComponent<GorillaMouthFlap>().SetFaceMaterialReplacement(this.newFaceMaterial);
		MeshRenderer[] array = this.alsoApplyFaceTo;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = sharedMaterial;
		}
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0005251E File Offset: 0x0005071E
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearFaceMaterialReplacement();
	}

	// Token: 0x0400132D RID: 4909
	[SerializeField]
	private Material newFaceMaterial;

	// Token: 0x0400132E RID: 4910
	private VRRig myRig;

	// Token: 0x0400132F RID: 4911
	[SerializeField]
	private MeshRenderer[] alsoApplyFaceTo;
}
