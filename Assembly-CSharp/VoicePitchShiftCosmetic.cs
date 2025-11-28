using System;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class VoicePitchShiftCosmetic : MonoBehaviour
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x060010C4 RID: 4292 RVA: 0x00057560 File Offset: 0x00055760
	// (set) Token: 0x060010C5 RID: 4293 RVA: 0x00057568 File Offset: 0x00055768
	public float Pitch
	{
		get
		{
			return this.pitch;
		}
		set
		{
			value = Mathf.Clamp(value, 0.6666667f, 1.5f);
			if (this.myRig == null)
			{
				this.pitch = value;
				return;
			}
			if (!Mathf.Approximately(value, this.pitch))
			{
				this.pitch = value;
				this.myRig.SetPitchShiftCosmeticsDirty();
			}
		}
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x000575BD File Offset: 0x000557BD
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this.myRig != null)
		{
			this.myRig.PitchShiftCosmetics.Add(this);
			this.myRig.SetPitchShiftCosmeticsDirty();
		}
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x000575FD File Offset: 0x000557FD
	private void OnDisable()
	{
		if (this.myRig != null)
		{
			this.myRig.PitchShiftCosmetics.Remove(this);
			this.myRig.SetPitchShiftCosmeticsDirty();
		}
	}

	// Token: 0x040014E6 RID: 5350
	private const float MIN = 0.6666667f;

	// Token: 0x040014E7 RID: 5351
	private const float MAX = 1.5f;

	// Token: 0x040014E8 RID: 5352
	[Tooltip("If multiple cosmetics are equipped that modify the pitch, their values will be averaged. Has a minimum pitch of 2/3 and a maximum of 1.5.")]
	[Range(0.6666667f, 1.5f)]
	[SerializeField]
	private float pitch = 1f;

	// Token: 0x040014E9 RID: 5353
	private VRRig myRig;
}
