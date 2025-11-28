using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000260 RID: 608
public class GorillaSkin : ScriptableObject
{
	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06000FBD RID: 4029 RVA: 0x000530FE File Offset: 0x000512FE
	public Mesh bodyMesh
	{
		get
		{
			return this._bodyMesh;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000FBE RID: 4030 RVA: 0x00053106 File Offset: 0x00051306
	public bool allowHeadless
	{
		get
		{
			return !this._disableHeadless;
		}
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00053114 File Offset: 0x00051314
	public static GorillaSkin CopyWithInstancedMaterials(GorillaSkin basis)
	{
		GorillaSkin gorillaSkin = ScriptableObject.CreateInstance<GorillaSkin>();
		gorillaSkin._chestMaterial = ((basis._chestMaterial != null) ? new Material(basis._chestMaterial) : null);
		gorillaSkin._bodyMaterial = ((basis._bodyMaterial != null) ? new Material(basis._bodyMaterial) : null);
		gorillaSkin._scoreboardMaterial = ((basis._scoreboardMaterial != null) ? new Material(basis._scoreboardMaterial) : null);
		gorillaSkin._bodyMesh = basis.bodyMesh;
		return gorillaSkin;
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x00053198 File Offset: 0x00051398
	public Material bodyMaterial
	{
		get
		{
			return this._bodyMaterial;
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x000531A0 File Offset: 0x000513A0
	public Material chestMaterial
	{
		get
		{
			return this._chestMaterial;
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000FC2 RID: 4034 RVA: 0x000531A8 File Offset: 0x000513A8
	public Material scoreboardMaterial
	{
		get
		{
			return this._scoreboardMaterial;
		}
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x000531B0 File Offset: 0x000513B0
	public static void ShowActiveSkin(VRRig rig)
	{
		bool useDefaultBodySkin;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out useDefaultBodySkin);
		GorillaSkin.ShowSkin(rig, activeSkin, useDefaultBodySkin);
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x000531D0 File Offset: 0x000513D0
	public void ApplySkinToMannequin(GameObject mannequin, bool swapMesh = false)
	{
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (!mannequin.TryGetComponent<SkinnedMeshRenderer>(ref skinnedMeshRenderer))
		{
			MeshRenderer meshRenderer;
			if (mannequin.TryGetComponent<MeshRenderer>(ref meshRenderer))
			{
				meshRenderer.GetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
				GorillaSkin._g_sharedMaterialsCache[0] = this.bodyMaterial;
				GorillaSkin._g_sharedMaterialsCache[1] = this.chestMaterial;
				meshRenderer.SetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
			}
			return;
		}
		int subMeshCount = skinnedMeshRenderer.sharedMesh.subMeshCount;
		if (swapMesh && this.bodyMesh != null)
		{
			skinnedMeshRenderer.sharedMesh = this.bodyMesh;
		}
		int subMeshCount2 = skinnedMeshRenderer.sharedMesh.subMeshCount;
		skinnedMeshRenderer.GetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
		if (subMeshCount == subMeshCount2)
		{
			GorillaSkin._g_sharedMaterialsCache[0] = this.bodyMaterial;
			if (subMeshCount > 2)
			{
				GorillaSkin._g_sharedMaterialsCache[1] = this.chestMaterial;
			}
			skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_sharedMaterialsCache);
			return;
		}
		if (GorillaSkin._g_sharedMaterialsCache.Count == subMeshCount)
		{
			if (subMeshCount2 == 2 && subMeshCount > subMeshCount2)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(GorillaSkin._g_sharedMaterialsCache[2]);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			if (subMeshCount2 == 3 && subMeshCount < subMeshCount2 && GorillaSkin._g_sharedMaterialsCache.Count > 1)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(this.chestMaterial);
				GorillaSkin._g_materialsWriteCache.Add(GorillaSkin._g_sharedMaterialsCache[1]);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			Debug.LogError(string.Format("Unexpected Submesh count {0} {1}", subMeshCount, subMeshCount2));
			return;
		}
		else
		{
			if (subMeshCount2 == 2)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			if (subMeshCount2 == 3)
			{
				GorillaSkin._g_materialsWriteCache.Clear();
				GorillaSkin._g_materialsWriteCache.Add(this.bodyMaterial);
				GorillaSkin._g_materialsWriteCache.Add(this.chestMaterial);
				skinnedMeshRenderer.SetSharedMaterials(GorillaSkin._g_materialsWriteCache);
				return;
			}
			Debug.LogError(string.Format("Unexpected Submesh count {0}", subMeshCount2));
			return;
		}
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x000533EC File Offset: 0x000515EC
	public static GorillaSkin GetActiveSkin(VRRig rig, out bool useDefaultBodySkin)
	{
		if (rig.CurrentModeSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentModeSkin;
		}
		if (rig.TemporaryEffectSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.TemporaryEffectSkin;
		}
		if (rig.CurrentCosmeticSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentCosmeticSkin;
		}
		useDefaultBodySkin = true;
		return rig.defaultSkin;
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x00053448 File Offset: 0x00051648
	public static void ShowSkin(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
	{
		if (skin.bodyMesh != null)
		{
			rig.bodyRenderer.SetCosmeticBodyMesh(skin.bodyMesh);
		}
		else
		{
			rig.bodyRenderer.ClearCosmeticBodyMesh();
		}
		if (useDefaultBodySkin)
		{
			rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
		}
		else
		{
			rig.materialsToChangeTo[0] = skin.bodyMaterial;
		}
		rig.bodyRenderer.SetSkinMaterials(rig.materialsToChangeTo[rig.setMatIndex], skin.chestMaterial, skin.allowHeadless);
		rig.scoreboardMaterial = skin.scoreboardMaterial;
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x000534D4 File Offset: 0x000516D4
	public static void ApplyToRig(VRRig rig, GorillaSkin skin, GorillaSkin.SkinType type)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		switch (type)
		{
		case GorillaSkin.SkinType.cosmetic:
			rig.CurrentCosmeticSkin = skin;
			break;
		case GorillaSkin.SkinType.gameMode:
			rig.CurrentModeSkin = skin;
			break;
		case GorillaSkin.SkinType.temporaryEffect:
			rig.TemporaryEffectSkin = skin;
			break;
		default:
			Debug.LogError("Unknown skin slot");
			break;
		}
		bool useDefaultBodySkin;
		GorillaSkin activeSkin2 = GorillaSkin.GetActiveSkin(rig, out useDefaultBodySkin);
		if (activeSkin != activeSkin2)
		{
			GorillaSkin.ShowSkin(rig, activeSkin2, useDefaultBodySkin);
		}
	}

	// Token: 0x04001386 RID: 4998
	[FormerlySerializedAs("chestMaterial")]
	[FormerlySerializedAs("chestEarsMaterial")]
	[SerializeField]
	private Material _chestMaterial;

	// Token: 0x04001387 RID: 4999
	[FormerlySerializedAs("bodyMaterial")]
	[SerializeField]
	private Material _bodyMaterial;

	// Token: 0x04001388 RID: 5000
	[SerializeField]
	private Material _scoreboardMaterial;

	// Token: 0x04001389 RID: 5001
	[Tooltip("Check this if skin materials are incompatible with HeadlessMonkeRig mesh")]
	[SerializeField]
	private bool _disableHeadless;

	// Token: 0x0400138A RID: 5002
	[Space]
	[SerializeField]
	private Mesh _bodyMesh;

	// Token: 0x0400138B RID: 5003
	[Space]
	[NonSerialized]
	private Material _bodyRuntime;

	// Token: 0x0400138C RID: 5004
	[NonSerialized]
	private Material _chestRuntime;

	// Token: 0x0400138D RID: 5005
	[NonSerialized]
	private Material _scoreRuntime;

	// Token: 0x0400138E RID: 5006
	private static List<Material> _g_sharedMaterialsCache = new List<Material>(2);

	// Token: 0x0400138F RID: 5007
	private static List<Material> _g_materialsWriteCache = new List<Material>(3);

	// Token: 0x02000261 RID: 609
	public enum SkinType
	{
		// Token: 0x04001391 RID: 5009
		cosmetic,
		// Token: 0x04001392 RID: 5010
		gameMode,
		// Token: 0x04001393 RID: 5011
		temporaryEffect
	}
}
