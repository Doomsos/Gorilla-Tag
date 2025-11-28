using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x020004E5 RID: 1253
public class PlayerColoredCosmetic : MonoBehaviour
{
	// Token: 0x06002044 RID: 8260 RVA: 0x000AB324 File Offset: 0x000A9524
	public void Awake()
	{
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Init();
		}
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x000AB358 File Offset: 0x000A9558
	private void InitIfNeeded()
	{
		if (!this.didInit)
		{
			this.didInit = true;
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && GorillaTagger.Instance != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.particleMains = new ParticleSystem.MainModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleMains[i] = this.particleSystems[i].main;
			}
		}
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000AB3EA File Offset: 0x000A95EA
	private void OnEnable()
	{
		this.InitIfNeeded();
		if (this.rig != null)
		{
			this.rig.OnColorChanged += new Action<Color>(this.UpdateColor);
			this.UpdateColor(this.rig.playerColor);
		}
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x000AB428 File Offset: 0x000A9628
	private void OnDisable()
	{
		if (this.rig != null)
		{
			this.rig.OnColorChanged -= new Action<Color>(this.UpdateColor);
		}
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x000AB450 File Offset: 0x000A9650
	public void UpdateColor(Color color)
	{
		this.InitIfNeeded();
		Color color2 = Color.Lerp(color, this.lerpToColor, this.lerpStrength);
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Apply(color2);
		}
		for (int j = 0; j < this.particleSystems.Length; j++)
		{
			this.particleMains[j].startColor = color2;
		}
	}

	// Token: 0x04002AAE RID: 10926
	private bool didInit;

	// Token: 0x04002AAF RID: 10927
	private VRRig rig;

	// Token: 0x04002AB0 RID: 10928
	[SerializeField]
	private Color lerpToColor = Color.white;

	// Token: 0x04002AB1 RID: 10929
	[SerializeField]
	[Range(0f, 1f)]
	private float lerpStrength;

	// Token: 0x04002AB2 RID: 10930
	[SerializeField]
	private PlayerColoredCosmetic.ColoringRule[] coloringRules;

	// Token: 0x04002AB3 RID: 10931
	[SerializeField]
	private ParticleSystem[] particleSystems;

	// Token: 0x04002AB4 RID: 10932
	private ParticleSystem.MainModule[] particleMains;

	// Token: 0x020004E6 RID: 1254
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x0600204A RID: 8266 RVA: 0x000AB4D8 File Offset: 0x000A96D8
		public void Init()
		{
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			List<Material> list;
			using (CollectionPool<List<Material>, Material>.Get(ref list))
			{
				this.meshRenderer.GetSharedMaterials(list);
				this.defaultMaterial = list[this.materialIndex];
				this.instancedMaterial = new Material(list[this.materialIndex]);
				list[this.materialIndex] = this.instancedMaterial;
				this.meshRenderer.SetSharedMaterials(list);
			}
		}

		// Token: 0x0600204B RID: 8267 RVA: 0x000AB574 File Offset: 0x000A9774
		public void Apply(Color color)
		{
			this.instancedMaterial.SetColor(this.hashId, color);
		}

		// Token: 0x04002AB5 RID: 10933
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04002AB6 RID: 10934
		private int hashId;

		// Token: 0x04002AB7 RID: 10935
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04002AB8 RID: 10936
		[SerializeField]
		private int materialIndex;

		// Token: 0x04002AB9 RID: 10937
		private Material instancedMaterial;

		// Token: 0x04002ABA RID: 10938
		private Material defaultMaterial;
	}
}
