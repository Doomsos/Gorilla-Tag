using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009E9 RID: 2537
[HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/material-instance")]
[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
public class MaterialInstance : MonoBehaviour
{
	// Token: 0x0600409F RID: 16543 RVA: 0x00159DD5 File Offset: 0x00157FD5
	public Material AcquireMaterial(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		Material[] array = this.instanceMaterials;
		if (array != null && array.Length != 0)
		{
			return this.instanceMaterials[0];
		}
		return null;
	}

	// Token: 0x060040A0 RID: 16544 RVA: 0x00159E13 File Offset: 0x00158013
	public Material[] AcquireMaterials(Object owner = null, bool instance = true)
	{
		if (owner != null)
		{
			this.materialOwners.Add(owner);
		}
		if (instance)
		{
			this.AcquireInstances();
		}
		base.gameObject.GetComponent<Material>();
		return this.instanceMaterials;
	}

	// Token: 0x060040A1 RID: 16545 RVA: 0x00159E46 File Offset: 0x00158046
	public void ReleaseMaterial(Object owner, bool autoDestroy = true)
	{
		this.materialOwners.Remove(owner);
		if (autoDestroy && this.materialOwners.Count == 0)
		{
			MaterialInstance.DestroySafe(this);
			if (!base.gameObject.activeInHierarchy)
			{
				this.RestoreRenderer();
			}
		}
	}

	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x060040A2 RID: 16546 RVA: 0x00159E7E File Offset: 0x0015807E
	public Material Material
	{
		get
		{
			return this.AcquireMaterial(null, true);
		}
	}

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x060040A3 RID: 16547 RVA: 0x00159E88 File Offset: 0x00158088
	public Material[] Materials
	{
		get
		{
			return this.AcquireMaterials(null, true);
		}
	}

	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x060040A4 RID: 16548 RVA: 0x00159E92 File Offset: 0x00158092
	// (set) Token: 0x060040A5 RID: 16549 RVA: 0x00159E9A File Offset: 0x0015809A
	public bool CacheSharedMaterialsFromRenderer
	{
		get
		{
			return this.cacheSharedMaterialsFromRenderer;
		}
		set
		{
			if (this.cacheSharedMaterialsFromRenderer != value)
			{
				if (value)
				{
					this.cachedSharedMaterials = this.CachedRenderer.sharedMaterials;
				}
				else
				{
					this.cachedSharedMaterials = null;
				}
				this.cacheSharedMaterialsFromRenderer = value;
			}
		}
	}

	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x060040A6 RID: 16550 RVA: 0x00159EC9 File Offset: 0x001580C9
	private Renderer CachedRenderer
	{
		get
		{
			if (this.cachedRenderer == null)
			{
				this.cachedRenderer = base.GetComponent<Renderer>();
				if (this.CacheSharedMaterialsFromRenderer)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
			}
			return this.cachedRenderer;
		}
	}

	// Token: 0x170005FD RID: 1533
	// (get) Token: 0x060040A7 RID: 16551 RVA: 0x00159F04 File Offset: 0x00158104
	// (set) Token: 0x060040A8 RID: 16552 RVA: 0x00159F39 File Offset: 0x00158139
	private Material[] CachedRendererSharedMaterials
	{
		get
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				if (this.cachedSharedMaterials == null)
				{
					this.cachedSharedMaterials = this.cachedRenderer.sharedMaterials;
				}
				return this.cachedSharedMaterials;
			}
			return this.cachedRenderer.sharedMaterials;
		}
		set
		{
			if (this.CacheSharedMaterialsFromRenderer)
			{
				this.cachedSharedMaterials = value;
			}
			this.cachedRenderer.sharedMaterials = value;
		}
	}

	// Token: 0x060040A9 RID: 16553 RVA: 0x00159F56 File Offset: 0x00158156
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060040AA RID: 16554 RVA: 0x00159F5E File Offset: 0x0015815E
	private void OnDestroy()
	{
		this.RestoreRenderer();
	}

	// Token: 0x060040AB RID: 16555 RVA: 0x00159F66 File Offset: 0x00158166
	private void RestoreRenderer()
	{
		if (this.CachedRenderer != null && this.defaultMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.defaultMaterials;
		}
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = null;
	}

	// Token: 0x060040AC RID: 16556 RVA: 0x00159F9C File Offset: 0x0015819C
	private void Initialize()
	{
		if (!this.initialized && this.CachedRenderer != null)
		{
			if (!MaterialInstance.HasValidMaterial(this.defaultMaterials))
			{
				this.defaultMaterials = this.CachedRendererSharedMaterials;
			}
			else if (!this.materialsInstanced)
			{
				this.CachedRendererSharedMaterials = this.defaultMaterials;
			}
			this.initialized = true;
		}
	}

	// Token: 0x060040AD RID: 16557 RVA: 0x00159FF5 File Offset: 0x001581F5
	private void AcquireInstances()
	{
		if (this.CachedRenderer != null && !MaterialInstance.MaterialsMatch(this.CachedRendererSharedMaterials, this.instanceMaterials))
		{
			this.CreateInstances();
		}
	}

	// Token: 0x060040AE RID: 16558 RVA: 0x0015A020 File Offset: 0x00158220
	private void CreateInstances()
	{
		this.Initialize();
		MaterialInstance.DestroyMaterials(this.instanceMaterials);
		this.instanceMaterials = MaterialInstance.InstanceMaterials(this.defaultMaterials);
		if (this.CachedRenderer != null && this.instanceMaterials != null)
		{
			this.CachedRendererSharedMaterials = this.instanceMaterials;
		}
		this.materialsInstanced = true;
	}

	// Token: 0x060040AF RID: 16559 RVA: 0x0015A078 File Offset: 0x00158278
	private static bool MaterialsMatch(Material[] a, Material[] b)
	{
		int? num = (a != null) ? new int?(a.Length) : default(int?);
		int? num2 = (b != null) ? new int?(b.Length) : default(int?);
		if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)))
		{
			return false;
		}
		int num3 = 0;
		for (;;)
		{
			int num4 = num3;
			num2 = ((a != null) ? new int?(a.Length) : default(int?));
			if (!(num4 < num2.GetValueOrDefault() & num2 != null))
			{
				return true;
			}
			if (a[num3] != b[num3])
			{
				break;
			}
			num3++;
		}
		return false;
	}

	// Token: 0x060040B0 RID: 16560 RVA: 0x0015A11C File Offset: 0x0015831C
	private static Material[] InstanceMaterials(Material[] source)
	{
		if (source == null)
		{
			return null;
		}
		Material[] array = new Material[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i] != null)
			{
				if (MaterialInstance.IsInstanceMaterial(source[i]))
				{
					Debug.LogWarning("A material (" + source[i].name + ") which is already instanced was instanced multiple times.");
				}
				array[i] = new Material(source[i]);
				Material material = array[i];
				material.name += " (Instance)";
			}
		}
		return array;
	}

	// Token: 0x060040B1 RID: 16561 RVA: 0x0015A19C File Offset: 0x0015839C
	private static void DestroyMaterials(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				MaterialInstance.DestroySafe(materials[i]);
			}
		}
	}

	// Token: 0x060040B2 RID: 16562 RVA: 0x0015A1C2 File Offset: 0x001583C2
	private static bool IsInstanceMaterial(Material material)
	{
		return material != null && material.name.Contains(" (Instance)");
	}

	// Token: 0x060040B3 RID: 16563 RVA: 0x0015A1E0 File Offset: 0x001583E0
	private static bool HasValidMaterial(Material[] materials)
	{
		if (materials != null)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060040B4 RID: 16564 RVA: 0x0015A20E File Offset: 0x0015840E
	private static void DestroySafe(Object toDestroy)
	{
		if (toDestroy != null && Application.isPlaying)
		{
			Object.Destroy(toDestroy);
		}
	}

	// Token: 0x040051EE RID: 20974
	private Renderer cachedRenderer;

	// Token: 0x040051EF RID: 20975
	[SerializeField]
	[HideInInspector]
	private Material[] defaultMaterials;

	// Token: 0x040051F0 RID: 20976
	private Material[] instanceMaterials;

	// Token: 0x040051F1 RID: 20977
	private Material[] cachedSharedMaterials;

	// Token: 0x040051F2 RID: 20978
	private bool initialized;

	// Token: 0x040051F3 RID: 20979
	private bool materialsInstanced;

	// Token: 0x040051F4 RID: 20980
	[SerializeField]
	[Tooltip("Whether to use a cached copy of cachedRenderer.sharedMaterials or call sharedMaterials on the Renderer directly. Enabling the option will lead to better performance but you must turn it off before modifying sharedMaterials of the Renderer.")]
	private bool cacheSharedMaterialsFromRenderer;

	// Token: 0x040051F5 RID: 20981
	private readonly HashSet<Object> materialOwners = new HashSet<Object>();

	// Token: 0x040051F6 RID: 20982
	private const string instancePostfix = " (Instance)";
}
