using System;
using UnityEngine;

// Token: 0x02000772 RID: 1906
public class GorillaCaveCrystal : Tappable
{
	// Token: 0x060031B5 RID: 12725 RVA: 0x0010D8CD File Offset: 0x0010BACD
	private void Awake()
	{
		if (this.tapScript == null)
		{
			this.tapScript = base.GetComponent<TapInnerGlow>();
		}
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x0010D8E9 File Offset: 0x0010BAE9
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x0010D8F8 File Offset: 0x0010BAF8
	private void AnimateCrystal()
	{
		if (this.tapScript)
		{
			this.tapScript.Tap();
		}
	}

	// Token: 0x04004031 RID: 16433
	public bool overrideSoundAndMaterial;

	// Token: 0x04004032 RID: 16434
	public CrystalOctave octave;

	// Token: 0x04004033 RID: 16435
	public CrystalNote note;

	// Token: 0x04004034 RID: 16436
	[SerializeField]
	private MeshRenderer _crystalRenderer;

	// Token: 0x04004035 RID: 16437
	public TapInnerGlow tapScript;

	// Token: 0x04004036 RID: 16438
	[HideInInspector]
	public GorillaCaveCrystalVisuals visuals;

	// Token: 0x04004037 RID: 16439
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04004038 RID: 16440
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04004039 RID: 16441
	[HideInInspector]
	[SerializeField]
	private bool _animating;

	// Token: 0x0400403A RID: 16442
	[HideInInspector]
	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	// Token: 0x0400403B RID: 16443
	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
