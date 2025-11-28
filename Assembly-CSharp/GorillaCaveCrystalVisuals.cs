using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000773 RID: 1907
public class GorillaCaveCrystalVisuals : MonoBehaviour
{
	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x060031B9 RID: 12729 RVA: 0x0010D986 File Offset: 0x0010BB86
	// (set) Token: 0x060031BA RID: 12730 RVA: 0x0010D98E File Offset: 0x0010BB8E
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

	// Token: 0x060031BB RID: 12731 RVA: 0x0010D998 File Offset: 0x0010BB98
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

	// Token: 0x060031BC RID: 12732 RVA: 0x0010DA14 File Offset: 0x0010BC14
	private void Start()
	{
		this.UpdateAlbedo();
		this.ForceUpdate();
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x0010DA24 File Offset: 0x0010BC24
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

	// Token: 0x060031BE RID: 12734 RVA: 0x0010DA99 File Offset: 0x0010BC99
	private void Awake()
	{
		this.UpdateAlbedo();
		this.Update();
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x0010DAA8 File Offset: 0x0010BCA8
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

	// Token: 0x060031C0 RID: 12736 RVA: 0x0010DB9E File Offset: 0x0010BD9E
	public void ForceUpdate()
	{
		this._lastState = 0;
		this.Update();
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x0010DBB0 File Offset: 0x0010BDB0
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
