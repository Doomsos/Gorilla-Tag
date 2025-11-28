using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class GorillaMouthTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000FB4 RID: 4020 RVA: 0x000530A9 File Offset: 0x000512A9
	// (set) Token: 0x06000FB5 RID: 4021 RVA: 0x000530B1 File Offset: 0x000512B1
	public bool IsSpawned { get; set; }

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06000FB6 RID: 4022 RVA: 0x000530BA File Offset: 0x000512BA
	// (set) Token: 0x06000FB7 RID: 4023 RVA: 0x000530C2 File Offset: 0x000512C2
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000FB8 RID: 4024 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x000530CB File Offset: 0x000512CB
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x000530D4 File Offset: 0x000512D4
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetMouthTextureReplacement(this.newMouthAtlas);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x000530EC File Offset: 0x000512EC
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearMouthTextureReplacement();
	}

	// Token: 0x04001382 RID: 4994
	[SerializeField]
	private Texture2D newMouthAtlas;

	// Token: 0x04001383 RID: 4995
	private VRRig myRig;
}
