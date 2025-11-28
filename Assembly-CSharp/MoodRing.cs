using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class MoodRing : MonoBehaviour, ISpawnable
{
	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06002012 RID: 8210 RVA: 0x000AA2CB File Offset: 0x000A84CB
	// (set) Token: 0x06002013 RID: 8211 RVA: 0x000AA2D3 File Offset: 0x000A84D3
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06002014 RID: 8212 RVA: 0x000AA2DC File Offset: 0x000A84DC
	// (set) Token: 0x06002015 RID: 8213 RVA: 0x000AA2E4 File Offset: 0x000A84E4
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06002016 RID: 8214 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x000AA2ED File Offset: 0x000A84ED
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000AA2F8 File Offset: 0x000A84F8
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			if (!this.isCycling)
			{
				this.animRedValue = this.myRig.playerColor.r;
				this.animGreenValue = this.myRig.playerColor.g;
				this.animBlueValue = this.myRig.playerColor.b;
			}
			this.isCycling = true;
			this.RainbowCycle(ref this.animRedValue, ref this.animGreenValue, ref this.animBlueValue);
			this.myRig.InitializeNoobMaterialLocal(this.animRedValue, this.animGreenValue, this.animBlueValue);
			return;
		}
		if (this.isCycling)
		{
			this.isCycling = false;
			if (this.myRig.isOfflineVRRig)
			{
				this.animRedValue = Mathf.Round(this.animRedValue * 9f) / 9f;
				this.animGreenValue = Mathf.Round(this.animGreenValue * 9f) / 9f;
				this.animBlueValue = Mathf.Round(this.animBlueValue * 9f) / 9f;
				GorillaTagger.Instance.UpdateColor(this.animRedValue, this.animGreenValue, this.animBlueValue);
				if (NetworkSystem.Instance.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
					{
						this.animRedValue,
						this.animGreenValue,
						this.animBlueValue
					});
				}
				PlayerPrefs.SetFloat("redValue", this.animRedValue);
				PlayerPrefs.SetFloat("greenValue", this.animGreenValue);
				PlayerPrefs.SetFloat("blueValue", this.animBlueValue);
				PlayerPrefs.Save();
			}
		}
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000AA4DC File Offset: 0x000A86DC
	private void RainbowCycle(ref float r, ref float g, ref float b)
	{
		float num = this.furCycleSpeed * Time.deltaTime;
		if (r == 1f)
		{
			if (b > 0f)
			{
				b = Mathf.Clamp01(b - num);
				return;
			}
			if (g < 1f)
			{
				g = Mathf.Clamp01(g + num);
				return;
			}
			r = Mathf.Clamp01(r - num);
			return;
		}
		else if (g == 1f)
		{
			if (r > 0f)
			{
				r = Mathf.Clamp01(r - num);
				return;
			}
			if (b < 1f)
			{
				b = Mathf.Clamp01(b + num);
				return;
			}
			g = Mathf.Clamp01(g - num);
			return;
		}
		else
		{
			if (b != 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			if (g > 0f)
			{
				g = Mathf.Clamp01(g - num);
				return;
			}
			if (r < 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			b = Mathf.Clamp01(b - num);
			return;
		}
	}

	// Token: 0x04002A6C RID: 10860
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002A6D RID: 10861
	private VRRig myRig;

	// Token: 0x04002A6E RID: 10862
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002A6F RID: 10863
	[SerializeField]
	private float furCycleSpeed;

	// Token: 0x04002A70 RID: 10864
	private float nextFurCycleTimestamp;

	// Token: 0x04002A71 RID: 10865
	private float animRedValue;

	// Token: 0x04002A72 RID: 10866
	private float animGreenValue;

	// Token: 0x04002A73 RID: 10867
	private float animBlueValue;

	// Token: 0x04002A74 RID: 10868
	private bool isCycling;
}
