using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class GorillaSkinToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000FCB RID: 4043 RVA: 0x00053569 File Offset: 0x00051769
	public bool applied
	{
		get
		{
			return this._applied;
		}
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000FCC RID: 4044 RVA: 0x00053571 File Offset: 0x00051771
	// (set) Token: 0x06000FCD RID: 4045 RVA: 0x00053579 File Offset: 0x00051779
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000FCE RID: 4046 RVA: 0x00053582 File Offset: 0x00051782
	// (set) Token: 0x06000FCF RID: 4047 RVA: 0x0005358A File Offset: 0x0005178A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00053594 File Offset: 0x00051794
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = base.GetComponentInParent<VRRig>(true);
		if (this.coloringRules.Length != 0)
		{
			this._activeSkin = GorillaSkin.CopyWithInstancedMaterials(this._skin);
			for (int i = 0; i < this.coloringRules.Length; i++)
			{
				this.coloringRules[i].Init();
			}
			return;
		}
		this._activeSkin = this._skin;
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x000535FC File Offset: 0x000517FC
	private void OnPlayerColorChanged(Color playerColor)
	{
		foreach (GorillaSkinToggle.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(this._activeSkin, playerColor);
		}
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00053634 File Offset: 0x00051834
	private void OnEnable()
	{
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged += new Action<Color>(this.OnPlayerColorChanged);
			this.OnPlayerColorChanged(this._rig.playerColor);
		}
		this.Apply();
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x0005366D File Offset: 0x0005186D
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged -= new Action<Color>(this.OnPlayerColorChanged);
		}
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0005369D File Offset: 0x0005189D
	public void Apply()
	{
		GorillaSkin.ApplyToRig(this._rig, this._activeSkin, GorillaSkin.SkinType.cosmetic);
		this._applied = true;
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000536B8 File Offset: 0x000518B8
	public void ApplyToMannequin(GameObject mannequin, bool swapMesh = false)
	{
		if (this._skin.IsNull())
		{
			Debug.LogError("No skin set on GorillaSkinToggle");
			return;
		}
		if (mannequin.IsNull())
		{
			Debug.LogError("No mannequin set on GorillaSkinToggle");
			return;
		}
		this._skin.ApplySkinToMannequin(mannequin, swapMesh);
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x000536F4 File Offset: 0x000518F4
	public void Remove()
	{
		GorillaSkin.ApplyToRig(this._rig, null, GorillaSkin.SkinType.cosmetic);
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	// Token: 0x0400139A RID: 5018
	private VRRig _rig;

	// Token: 0x0400139B RID: 5019
	[SerializeField]
	private GorillaSkin _skin;

	// Token: 0x0400139C RID: 5020
	private GorillaSkin _activeSkin;

	// Token: 0x0400139D RID: 5021
	[SerializeField]
	private GorillaSkinToggle.ColoringRule[] coloringRules;

	// Token: 0x0400139E RID: 5022
	[Space]
	[SerializeField]
	private bool _applied;

	// Token: 0x02000265 RID: 613
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06000FD9 RID: 4057 RVA: 0x00053752 File Offset: 0x00051952
		public void Init()
		{
			if (string.IsNullOrEmpty(this.shaderColorProperty))
			{
				this.shaderColorProperty = "_BaseColor";
			}
			this.shaderHashId = new ShaderHashId(this.shaderColorProperty);
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x00053780 File Offset: 0x00051980
		public void Apply(GorillaSkin skin, Color color)
		{
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Body))
			{
				skin.bodyMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Chest))
			{
				skin.chestMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Scoreboard))
			{
				skin.scoreboardMaterial.SetColor(this.shaderHashId, color);
			}
		}

		// Token: 0x040013A1 RID: 5025
		public GorillaSkinMaterials colorMaterials;

		// Token: 0x040013A2 RID: 5026
		public string shaderColorProperty;

		// Token: 0x040013A3 RID: 5027
		private ShaderHashId shaderHashId;
	}
}
