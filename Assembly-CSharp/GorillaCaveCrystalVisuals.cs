using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000773 RID: 1907
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x060031B9 RID: 12729 RVA: 0x0010D966 File Offset: 0x0010BB66
	// (set) Token: 0x060031BA RID: 12730 RVA: 0x0010D96E File Offset: 0x0010BB6E
	public float lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			this._lerp = value;
		}
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x0010D978 File Offset: 0x0010BB78
	public void Setup()
	{
		base.TryGetComponent<MeshRenderer>(ref this._renderer);
		if (this._renderer == null)
		{
			return;
		}
		this._setup = GorillaCaveCrystalSetup.Instance;
		this._sharedMaterial = this._renderer.sharedMaterial;
		this._initialized = (this.crysalPreset != null && this._renderer != null && this._sharedMaterial != null);
		this.Update();
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x0010D9F4 File Offset: 0x0010BBF4
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x0010DA04 File Offset: 0x0010BC04
	public void UpdateAlbedo()
	{
		if (!this._initialized)
		{
			return;
		}
		if (this.instanceAlbedo == null)
		{
			return;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetTexture(GorillaCaveCrystalVisuals._MainTex, this.instanceAlbedo);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x0010DA79 File Offset: 0x0010BC79
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x0010DA88 File Offset: 0x0010BC88
	private void Update()
	{
		if (!this._initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = new ValueTuple<CrystalVisualsPreset, float>(this.crysalPreset, this._lerp).GetHashCode();
			if (this._lastState == hashCode)
			{
				return;
			}
			this._lastState = hashCode;
		}
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = this.crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = this.crysalPreset.stateB;
		Color color = Color.Lerp(stateA.albedo, stateB.albedo, this._lerp);
		Color color2 = Color.Lerp(stateA.emission, stateB.emission, this._lerp);
		this._renderer.GetPropertyBlock(this._block);
		this._block.SetColor(GorillaCaveCrystalVisuals._Color, color);
		this._block.SetColor(GorillaCaveCrystalVisuals._EmissionColor, color2);
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x0010DB7E File Offset: 0x0010BD7E
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x0010DB90 File Offset: 0x0010BD90
	private static void InitializeCrystals()
	{
		foreach (GorillaCaveCrystalVisuals gorillaCaveCrystalVisuals in Object.FindObjectsByType<GorillaCaveCrystalVisuals>(1, 1))
		{
			gorillaCaveCrystalVisuals.UpdateAlbedo();
			gorillaCaveCrystalVisuals.ForceUpdate();
			gorillaCaveCrystalVisuals._lastState = -1;
		}
	}

	// Token: 0x0400403C RID: 16444
	public CrystalVisualsPreset crysalPreset;

	// Token: 0x0400403D RID: 16445
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	// Token: 0x0400403E RID: 16446
	[Space]
	public MeshRenderer _renderer;

	// Token: 0x0400403F RID: 16447
	public Material _sharedMaterial;

	// Token: 0x04004040 RID: 16448
	[SerializeField]
	public Texture2D instanceAlbedo;

	// Token: 0x04004041 RID: 16449
	[SerializeField]
	private bool _initialized;

	// Token: 0x04004042 RID: 16450
	[SerializeField]
	private int _lastState;

	// Token: 0x04004043 RID: 16451
	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	// Token: 0x04004044 RID: 16452
	private MaterialPropertyBlock _block;

	// Token: 0x04004045 RID: 16453
	[NonSerialized]
	private bool _ranSetupOnce;

	// Token: 0x04004046 RID: 16454
	private static readonly ShaderHashId _Color = "_Color";

	// Token: 0x04004047 RID: 16455
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";

	// Token: 0x04004048 RID: 16456
	private static readonly ShaderHashId _MainTex = "_MainTex";
}
