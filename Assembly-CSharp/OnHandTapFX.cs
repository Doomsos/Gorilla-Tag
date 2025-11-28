using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200076B RID: 1899
internal struct OnHandTapFX : IFXEffectContext<HandEffectContext>
{
	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x06003154 RID: 12628 RVA: 0x0010C24C File Offset: 0x0010A44C
	public HandEffectContext effectContext
	{
		get
		{
			HandEffectContext handEffect = this.rig.GetHandEffect(this.isLeftHand, this.stiltID);
			this.rig.SetHandEffectData(handEffect, this.surfaceIndex, this.isDownTap, this.isLeftHand, this.stiltID, this.volume, this.speed, this.tapDir);
			return handEffect;
		}
	}

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x06003155 RID: 12629 RVA: 0x0010C2A8 File Offset: 0x0010A4A8
	public FXSystemSettings settings
	{
		get
		{
			return this.rig.fxSettings;
		}
	}

	// Token: 0x04003FFB RID: 16379
	public VRRig rig;

	// Token: 0x04003FFC RID: 16380
	public Vector3 tapDir;

	// Token: 0x04003FFD RID: 16381
	public bool isDownTap;

	// Token: 0x04003FFE RID: 16382
	public bool isLeftHand;

	// Token: 0x04003FFF RID: 16383
	public StiltID stiltID;

	// Token: 0x04004000 RID: 16384
	public int surfaceIndex;

	// Token: 0x04004001 RID: 16385
	public float volume;

	// Token: 0x04004002 RID: 16386
	public float speed;
}
