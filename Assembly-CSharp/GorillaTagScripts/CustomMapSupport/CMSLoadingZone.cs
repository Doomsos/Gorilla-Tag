using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E07 RID: 3591
	public class CMSLoadingZone : MonoBehaviour
	{
		// Token: 0x060059A3 RID: 22947 RVA: 0x001CA9F6 File Offset: 0x001C8BF6
		private void Start()
		{
			base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
		}

		// Token: 0x060059A4 RID: 22948 RVA: 0x001CAA0C File Offset: 0x001C8C0C
		public void SetupLoadingZone(LoadZoneSettings settings, in string[] assetBundleSceneFilePaths)
		{
			this.scenesToLoad = this.GetSceneIndexes(settings.scenesToLoad, assetBundleSceneFilePaths);
			this.scenesToUnload = this.CleanSceneUnloadArray(settings.scenesToUnload, settings.scenesToLoad, assetBundleSceneFilePaths);
			this.useDynamicLighting = settings.useDynamicLighting;
			this.dynamicLightingAmbientColor = settings.UberShaderAmbientDynamicLight;
			base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x060059A5 RID: 22949 RVA: 0x001CAA94 File Offset: 0x001C8C94
		private int[] GetSceneIndexes(List<string> sceneNames, in string[] assetBundleSceneFilePaths)
		{
			int[] array = new int[sceneNames.Count];
			for (int i = 0; i < sceneNames.Count; i++)
			{
				for (int j = 0; j < assetBundleSceneFilePaths.Length; j++)
				{
					if (string.Equals(sceneNames[i], this.GetSceneNameFromFilePath(assetBundleSceneFilePaths[j])))
					{
						array[i] = j;
						break;
					}
				}
			}
			return array;
		}

		// Token: 0x060059A6 RID: 22950 RVA: 0x001CAAEC File Offset: 0x001C8CEC
		private int[] CleanSceneUnloadArray(List<string> unload, List<string> load, in string[] assetBundleSceneFilePaths)
		{
			for (int i = 0; i < load.Count; i++)
			{
				if (unload.Contains(load[i]))
				{
					unload.Remove(load[i]);
				}
			}
			return this.GetSceneIndexes(unload, assetBundleSceneFilePaths);
		}

		// Token: 0x060059A7 RID: 22951 RVA: 0x001CAB30 File Offset: 0x001C8D30
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.bodyCollider)
			{
				if (this.useDynamicLighting)
				{
					GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
					GameLightingManager.instance.SetAmbientLightDynamic(this.dynamicLightingAmbientColor);
				}
				else
				{
					GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
					GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				}
				CustomMapManager.LoadZoneTriggered(this.scenesToLoad, this.scenesToUnload);
			}
		}

		// Token: 0x060059A8 RID: 22952 RVA: 0x001CABA7 File Offset: 0x001C8DA7
		private string GetSceneNameFromFilePath(string filePath)
		{
			string[] array = filePath.Split("/", 0);
			return array[array.Length - 1].Split(".", 0)[0];
		}

		// Token: 0x040066C7 RID: 26311
		private int[] scenesToLoad;

		// Token: 0x040066C8 RID: 26312
		private int[] scenesToUnload;

		// Token: 0x040066C9 RID: 26313
		private bool useDynamicLighting;

		// Token: 0x040066CA RID: 26314
		private Color dynamicLightingAmbientColor;
	}
}
