using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TagEffects;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class TagEffectsPackToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x17000196 RID: 406
	// (get) Token: 0x060010A9 RID: 4265 RVA: 0x00056F99 File Offset: 0x00055199
	// (set) Token: 0x060010AA RID: 4266 RVA: 0x00056FA1 File Offset: 0x000551A1
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x060010AB RID: 4267 RVA: 0x00056FAA File Offset: 0x000551AA
	// (set) Token: 0x060010AC RID: 4268 RVA: 0x00056FB2 File Offset: 0x000551B2
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060010AD RID: 4269 RVA: 0x00056FBB File Offset: 0x000551BB
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x00056FC4 File Offset: 0x000551C4
	private void OnEnable()
	{
		this.Apply();
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x00056FCC File Offset: 0x000551CC
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x00056FDC File Offset: 0x000551DC
	public void Apply()
	{
		this._rig.CosmeticEffectPack = this.tagEffectPack;
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00056FEF File Offset: 0x000551EF
	public void Remove()
	{
		this._rig.CosmeticEffectPack = null;
	}

	// Token: 0x040014C7 RID: 5319
	private VRRig _rig;

	// Token: 0x040014C8 RID: 5320
	[SerializeField]
	private TagEffectPack tagEffectPack;
}
