using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001084 RID: 4228
	public class ZoneLiquidEffectableManager : MonoBehaviour
	{
		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x060069EE RID: 27118 RVA: 0x00228585 File Offset: 0x00226785
		// (set) Token: 0x060069EF RID: 27119 RVA: 0x0022858C File Offset: 0x0022678C
		public static ZoneLiquidEffectableManager instance { get; private set; }

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x060069F0 RID: 27120 RVA: 0x00228594 File Offset: 0x00226794
		// (set) Token: 0x060069F1 RID: 27121 RVA: 0x0022859B File Offset: 0x0022679B
		public static bool hasInstance { get; private set; }

		// Token: 0x060069F2 RID: 27122 RVA: 0x002285A3 File Offset: 0x002267A3
		protected void Awake()
		{
			if (ZoneLiquidEffectableManager.hasInstance && ZoneLiquidEffectableManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			ZoneLiquidEffectableManager.SetInstance(this);
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x002285CB File Offset: 0x002267CB
		protected void OnDestroy()
		{
			if (ZoneLiquidEffectableManager.instance == this)
			{
				ZoneLiquidEffectableManager.hasInstance = false;
				ZoneLiquidEffectableManager.instance = null;
			}
		}

		// Token: 0x060069F4 RID: 27124 RVA: 0x002285E8 File Offset: 0x002267E8
		protected void LateUpdate()
		{
			int num = UnityLayer.Water.ToLayerMask();
			foreach (ZoneLiquidEffectable zoneLiquidEffectable in this.zoneLiquidEffectables)
			{
				Transform transform = zoneLiquidEffectable.transform;
				zoneLiquidEffectable.inLiquidVolume = Physics.CheckSphere(transform.position, zoneLiquidEffectable.radius * transform.lossyScale.x, num);
				if (zoneLiquidEffectable.inLiquidVolume != zoneLiquidEffectable.wasInLiquidVolume)
				{
					for (int i = 0; i < zoneLiquidEffectable.childRenderers.Length; i++)
					{
						if (zoneLiquidEffectable.inLiquidVolume)
						{
							zoneLiquidEffectable.childRenderers[i].material.EnableKeyword("_WATER_EFFECT");
							zoneLiquidEffectable.childRenderers[i].material.EnableKeyword("_HEIGHT_BASED_WATER_EFFECT");
						}
						else
						{
							zoneLiquidEffectable.childRenderers[i].material.DisableKeyword("_WATER_EFFECT");
							zoneLiquidEffectable.childRenderers[i].material.DisableKeyword("_HEIGHT_BASED_WATER_EFFECT");
						}
					}
				}
				zoneLiquidEffectable.wasInLiquidVolume = zoneLiquidEffectable.inLiquidVolume;
			}
		}

		// Token: 0x060069F5 RID: 27125 RVA: 0x0022870C File Offset: 0x0022690C
		private static void CreateManager()
		{
			ZoneLiquidEffectableManager.SetInstance(new GameObject("ZoneLiquidEffectableManager").AddComponent<ZoneLiquidEffectableManager>());
		}

		// Token: 0x060069F6 RID: 27126 RVA: 0x00228722 File Offset: 0x00226922
		private static void SetInstance(ZoneLiquidEffectableManager manager)
		{
			ZoneLiquidEffectableManager.instance = manager;
			ZoneLiquidEffectableManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x060069F7 RID: 27127 RVA: 0x00228740 File Offset: 0x00226940
		public static void Register(ZoneLiquidEffectable effect)
		{
			if (!ZoneLiquidEffectableManager.hasInstance)
			{
				ZoneLiquidEffectableManager.CreateManager();
			}
			if (effect == null)
			{
				return;
			}
			if (ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Contains(effect))
			{
				return;
			}
			ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Add(effect);
			effect.inLiquidVolume = false;
			for (int i = 0; i < effect.childRenderers.Length; i++)
			{
				if (!(effect.childRenderers[i] == null))
				{
					Material sharedMaterial = effect.childRenderers[i].sharedMaterial;
					if (!(sharedMaterial == null) || sharedMaterial.shader.keywordSpace.FindKeyword("_WATER_EFFECT").isValid)
					{
						effect.inLiquidVolume = (sharedMaterial.IsKeywordEnabled("_WATER_EFFECT") && sharedMaterial.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT"));
						return;
					}
				}
			}
		}

		// Token: 0x060069F8 RID: 27128 RVA: 0x0022880B File Offset: 0x00226A0B
		public static void Unregister(ZoneLiquidEffectable effect)
		{
			ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Remove(effect);
		}

		// Token: 0x0400794E RID: 31054
		private readonly List<ZoneLiquidEffectable> zoneLiquidEffectables = new List<ZoneLiquidEffectable>(32);
	}
}
