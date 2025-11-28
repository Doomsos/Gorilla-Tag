using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class HandEffectsOverrideCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x0600142A RID: 5162 RVA: 0x000742FC File Offset: 0x000724FC
	// (set) Token: 0x0600142B RID: 5163 RVA: 0x00074304 File Offset: 0x00072504
	public bool IsSpawned { get; set; }

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x0600142C RID: 5164 RVA: 0x0007430D File Offset: 0x0007250D
	// (set) Token: 0x0600142D RID: 5165 RVA: 0x00074315 File Offset: 0x00072515
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600142E RID: 5166 RVA: 0x0007431E File Offset: 0x0007251E
	public void OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDespawn()
	{
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x00074327 File Offset: 0x00072527
	public void OnEnable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Add(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Add(this);
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x00074354 File Offset: 0x00072554
	public void OnDisable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Remove(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Remove(this);
	}

	// Token: 0x04001EC9 RID: 7881
	public HandEffectsOverrideCosmetic.HandEffectType handEffectType;

	// Token: 0x04001ECA RID: 7882
	public bool isLeftHand;

	// Token: 0x04001ECB RID: 7883
	public HandEffectsOverrideCosmetic.EffectsOverride firstPerson;

	// Token: 0x04001ECC RID: 7884
	public HandEffectsOverrideCosmetic.EffectsOverride thirdPerson;

	// Token: 0x04001ECD RID: 7885
	private VRRig _rig;

	// Token: 0x0200034F RID: 847
	[Serializable]
	public class EffectsOverride
	{
		// Token: 0x04001ED0 RID: 7888
		public GameObject effectVFX;

		// Token: 0x04001ED1 RID: 7889
		public bool playHaptics;

		// Token: 0x04001ED2 RID: 7890
		public float hapticStrength = 0.5f;

		// Token: 0x04001ED3 RID: 7891
		public float hapticDuration = 0.5f;

		// Token: 0x04001ED4 RID: 7892
		public bool parentEffect;
	}

	// Token: 0x02000350 RID: 848
	public enum HandEffectType
	{
		// Token: 0x04001ED6 RID: 7894
		None,
		// Token: 0x04001ED7 RID: 7895
		FistBump,
		// Token: 0x04001ED8 RID: 7896
		HighFive
	}
}
