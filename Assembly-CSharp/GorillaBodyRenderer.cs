using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200076E RID: 1902
public class GorillaBodyRenderer : MonoBehaviour
{
	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06003194 RID: 12692 RVA: 0x0010D148 File Offset: 0x0010B348
	// (set) Token: 0x06003195 RID: 12693 RVA: 0x0010D150 File Offset: 0x0010B350
	public GorillaBodyType bodyType
	{
		get
		{
			return this._bodyType;
		}
		set
		{
			this.SetBodyType(value);
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06003196 RID: 12694 RVA: 0x0010D159 File Offset: 0x0010B359
	public bool renderFace
	{
		get
		{
			return this._renderFace;
		}
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x06003197 RID: 12695 RVA: 0x0010D161 File Offset: 0x0010B361
	public static bool ForceSkeleton
	{
		get
		{
			return GorillaBodyRenderer.oopsAllSkeletons;
		}
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06003198 RID: 12696 RVA: 0x0010D168 File Offset: 0x0010B368
	// (set) Token: 0x06003199 RID: 12697 RVA: 0x0010D170 File Offset: 0x0010B370
	public GorillaBodyType gameModeBodyType { get; private set; }

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x0600319A RID: 12698 RVA: 0x0010D179 File Offset: 0x0010B379
	// (set) Token: 0x0600319B RID: 12699 RVA: 0x0010D181 File Offset: 0x0010B381
	public Material myDefaultSkinMaterialInstance { get; private set; }

	// Token: 0x0600319C RID: 12700 RVA: 0x0010D18C File Offset: 0x0010B38C
	public SkinnedMeshRenderer GetBody(GorillaBodyType type)
	{
		if (type < GorillaBodyType.Default || type >= (GorillaBodyType)this._renderersCache.Length)
		{
			return null;
		}
		return this._renderersCache[(int)type];
	}

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x0600319D RID: 12701 RVA: 0x0010D1B4 File Offset: 0x0010B3B4
	public SkinnedMeshRenderer ActiveBody
	{
		get
		{
			return this.GetBody(this._bodyType);
		}
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x0010D1C4 File Offset: 0x0010B3C4
	public static void SetAllSkeletons(bool allSkeletons)
	{
		GorillaBodyRenderer.oopsAllSkeletons = allSkeletons;
		GorillaTagger.Instance.offlineVRRig.bodyRenderer.Refresh();
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			vrrig.bodyRenderer.Refresh();
		}
	}

	// Token: 0x0600319F RID: 12703 RVA: 0x0010D23C File Offset: 0x0010B43C
	public void SetSkeletonBodyActive(bool active)
	{
		this.bodySkeleton.gameObject.SetActive(active);
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x0010D24F File Offset: 0x0010B44F
	public void SetGameModeBodyType(GorillaBodyType bodyType)
	{
		if (this.gameModeBodyType == bodyType)
		{
			return;
		}
		this.gameModeBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x0010D268 File Offset: 0x0010B468
	public void SetCosmeticBodyType(GorillaBodyType bodyType)
	{
		if (this.cosmeticBodyType == bodyType)
		{
			return;
		}
		this.cosmeticBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x0010D281 File Offset: 0x0010B481
	public void SetDefaults()
	{
		this.gameModeBodyType = GorillaBodyType.Default;
		this.cosmeticBodyType = GorillaBodyType.Default;
		this.Refresh();
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x0010D297 File Offset: 0x0010B497
	private void Refresh()
	{
		this.SetBodyType(this.GetActiveBodyType());
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x0010D2A8 File Offset: 0x0010B4A8
	public void SetMaterialIndex(int materialIndex)
	{
		this._lastMatIndex = materialIndex;
		switch (this.bodyType)
		{
		case GorillaBodyType.Default:
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
			return;
		case GorillaBodyType.NoHead:
			if (materialIndex == 0 && !this._applySkinToHeadlessMesh)
			{
				this.bodyNoHead.sharedMaterial = this.myDefaultSkinMaterialInstance;
				return;
			}
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
			return;
		case GorillaBodyType.Skeleton:
			this.rig.skeleton.SetMaterialIndex(materialIndex);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x0010D338 File Offset: 0x0010B538
	public void SetSkinMaterials(Material bodyMat, Material chestMat, bool allowHeadless)
	{
		this.EnsureInstantiatedMaterial();
		if (chestMat == null)
		{
			if (this._cachedSkinMaterials.Length != 1)
			{
				this._cachedSkinMaterials = new Material[1];
			}
			this._cachedSkinMaterials[0] = bodyMat;
		}
		else
		{
			if (this._cachedSkinMaterials.Length < 2)
			{
				this._cachedSkinMaterials = new Material[2];
			}
			this._cachedSkinMaterials[0] = bodyMat;
			this._cachedSkinMaterials[1] = chestMat;
		}
		this._applySkinToHeadlessMesh = allowHeadless;
		GorillaBodyType bodyType = this.bodyType;
		if (bodyType == GorillaBodyType.Default)
		{
			this.bodyDefault.sharedMaterials = this._cachedSkinMaterials;
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			return;
		}
		if (bodyType != GorillaBodyType.NoHead)
		{
			return;
		}
		if (this._applySkinToHeadlessMesh)
		{
			this.bodyNoHead.sharedMaterials = this._cachedSkinMaterials;
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			return;
		}
		this.bodyNoHead.sharedMaterials = this._defaultSkinMaterials;
		if (this._lastMatIndex != 0)
		{
			this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
		}
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x0010D451 File Offset: 0x0010B651
	public void SetupAsLocalPlayerBody()
	{
		this.faceRenderer.gameObject.layer = 22;
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x0010D465 File Offset: 0x0010B665
	public GorillaBodyType GetActiveBodyType()
	{
		if (GorillaBodyRenderer.oopsAllSkeletons)
		{
			return GorillaBodyType.Skeleton;
		}
		if (this.gameModeBodyType == GorillaBodyType.Default)
		{
			return this.cosmeticBodyType;
		}
		return this.gameModeBodyType;
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x0010D488 File Offset: 0x0010B688
	private void SetBodyType(GorillaBodyType type)
	{
		if (this._bodyType == type)
		{
			return;
		}
		this.SetBodyEnabled(this._bodyType, false);
		this._bodyType = type;
		this.SetBodyEnabled(type, true);
		this._renderFace = (this._bodyType != GorillaBodyType.NoHead && this._bodyType != GorillaBodyType.Skeleton && this._bodyType != GorillaBodyType.Invisible);
		if (this.faceRenderer != null)
		{
			this.faceRenderer.enabled = this._renderFace;
		}
		switch (type)
		{
		case GorillaBodyType.Default:
			this.bodyDefault.sharedMaterials = this._cachedSkinMaterials;
			this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			this.UpdateBodyMaterialColor(this.rig.playerColor);
			return;
		case GorillaBodyType.NoHead:
			if (this._applySkinToHeadlessMesh)
			{
				this.bodyNoHead.sharedMaterials = this._cachedSkinMaterials;
				this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
			}
			else
			{
				this.bodyNoHead.sharedMaterials = this._defaultSkinMaterials;
				if (this._lastMatIndex != 0)
				{
					this.bodyNoHead.sharedMaterial = this.rig.materialsToChangeTo[this._lastMatIndex];
				}
			}
			this.UpdateBodyMaterialColor(this.rig.playerColor);
			return;
		case GorillaBodyType.Skeleton:
			this.rig.skeleton.SetMaterialIndex(this._lastMatIndex);
			this.rig.skeleton.UpdateColor(this.rig.playerColor);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031A9 RID: 12713 RVA: 0x0010D605 File Offset: 0x0010B805
	public void SetCosmeticBodyMesh(Mesh mesh)
	{
		if (this.defaultBodyMesh == null)
		{
			this.defaultBodyMesh = this.bodyDefault.sharedMesh;
		}
		this.bodyDefault.sharedMesh = mesh;
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x0010D632 File Offset: 0x0010B832
	public void ClearCosmeticBodyMesh()
	{
		if (this.defaultBodyMesh != null)
		{
			this.bodyDefault.sharedMesh = this.defaultBodyMesh;
		}
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x0010D654 File Offset: 0x0010B854
	private void SetBodyEnabled(GorillaBodyType bodyType, bool enabled)
	{
		SkinnedMeshRenderer body = this.GetBody(bodyType);
		if (body == null)
		{
			return;
		}
		body.enabled = enabled;
		Transform[] bones = body.bones;
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i].gameObject.SetActive(enabled);
		}
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x0010D69D File Offset: 0x0010B89D
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x0010D6A5 File Offset: 0x0010B8A5
	public void SharedStart()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		this.EnsureInstantiatedMaterial();
	}

	// Token: 0x060031AE RID: 12718 RVA: 0x0010D6C8 File Offset: 0x0010B8C8
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		this._renderersCache = new SkinnedMeshRenderer[EnumData<GorillaBodyType>.Shared.Values.Length];
		this._renderersCache[0] = this.bodyDefault;
		this._renderersCache[1] = this.bodyNoHead;
		this._renderersCache[2] = this.bodySkeleton;
		this.SetBodyEnabled(GorillaBodyType.Default, true);
		this.SetBodyEnabled(GorillaBodyType.NoHead, false);
		this.SetBodyEnabled(GorillaBodyType.Skeleton, false);
		this._cachedSkinMaterials = this.bodyDefault.sharedMaterials;
		this._bodyType = GorillaBodyType.Default;
		this._bodyType = GorillaBodyType.Default;
		this.defaultBodyMesh = this.bodyDefault.sharedMesh;
		this.EnsureInstantiatedMaterial();
		this.UpdateColor(this.rig.playerColor);
		this.Refresh();
	}

	// Token: 0x060031AF RID: 12719 RVA: 0x0010D798 File Offset: 0x0010B998
	public void EnsureInstantiatedMaterial()
	{
		if (this.myDefaultSkinMaterialInstance == null)
		{
			this.myDefaultSkinMaterialInstance = Object.Instantiate<Material>(this.rig.materialsToChangeTo[0]);
			this.rig.materialsToChangeTo[0] = this.myDefaultSkinMaterialInstance;
		}
		if (this._defaultSkinMaterials.Length == 0)
		{
			this._defaultSkinMaterials = new Material[2];
			this._defaultSkinMaterials[0] = this.myDefaultSkinMaterialInstance;
			this._defaultSkinMaterials[1] = this.rig.defaultSkin.chestMaterial;
		}
	}

	// Token: 0x060031B0 RID: 12720 RVA: 0x0010D81C File Offset: 0x0010BA1C
	public void ResetBodyMaterial()
	{
		this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[0];
		this.bodyNoHead.sharedMaterial = (this._applySkinToHeadlessMesh ? this.rig.materialsToChangeTo[0] : this.myDefaultSkinMaterialInstance);
	}

	// Token: 0x060031B1 RID: 12721 RVA: 0x0010D869 File Offset: 0x0010BA69
	public void UpdateColor(Color color)
	{
		this.UpdateBodyMaterialColor(color);
		if (this.bodyType == GorillaBodyType.Skeleton)
		{
			this.rig.skeleton.UpdateColor(color);
		}
	}

	// Token: 0x060031B2 RID: 12722 RVA: 0x0010D88C File Offset: 0x0010BA8C
	private void UpdateBodyMaterialColor(Color color)
	{
		this.EnsureInstantiatedMaterial();
		if (this.myDefaultSkinMaterialInstance != null)
		{
			this.myDefaultSkinMaterialInstance.color = color;
		}
	}

	// Token: 0x0400400E RID: 16398
	[SerializeField]
	private GorillaBodyType _bodyType;

	// Token: 0x0400400F RID: 16399
	[SerializeField]
	private bool _renderFace = true;

	// Token: 0x04004010 RID: 16400
	public MeshRenderer faceRenderer;

	// Token: 0x04004011 RID: 16401
	[SerializeField]
	private SkinnedMeshRenderer bodyDefault;

	// Token: 0x04004012 RID: 16402
	[SerializeField]
	private SkinnedMeshRenderer bodyNoHead;

	// Token: 0x04004013 RID: 16403
	[SerializeField]
	private SkinnedMeshRenderer bodySkeleton;

	// Token: 0x04004014 RID: 16404
	private int _lastMatIndex;

	// Token: 0x04004015 RID: 16405
	private Mesh defaultBodyMesh;

	// Token: 0x04004016 RID: 16406
	private static bool oopsAllSkeletons;

	// Token: 0x04004018 RID: 16408
	private GorillaBodyType cosmeticBodyType;

	// Token: 0x0400401A RID: 16410
	[SerializeField]
	private Material[] _cachedSkinMaterials = new Material[0];

	// Token: 0x0400401B RID: 16411
	[SerializeField]
	private Material[] _defaultSkinMaterials = new Material[0];

	// Token: 0x0400401C RID: 16412
	private bool _applySkinToHeadlessMesh;

	// Token: 0x0400401D RID: 16413
	[Space]
	[NonSerialized]
	private SkinnedMeshRenderer[] _renderersCache = new SkinnedMeshRenderer[0];

	// Token: 0x0400401E RID: 16414
	private static readonly List<Material> gEmptyDefaultMats = new List<Material>();

	// Token: 0x0400401F RID: 16415
	[Space]
	public VRRig rig;
}
