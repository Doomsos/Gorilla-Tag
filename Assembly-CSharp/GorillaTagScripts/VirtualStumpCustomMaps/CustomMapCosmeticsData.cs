using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E12 RID: 3602
	[CreateAssetMenu(menuName = "ScriptableObjects/CustomMapCosmeticDataSO", order = 0)]
	[Serializable]
	public class CustomMapCosmeticsData : ScriptableObject
	{
		// Token: 0x060059E1 RID: 23009 RVA: 0x001CC0E3 File Offset: 0x001CA2E3
		public void OnEnable()
		{
			this.initializedFromTitleData = false;
		}

		// Token: 0x060059E2 RID: 23010 RVA: 0x001CC0EC File Offset: 0x001CA2EC
		public void OnDestroy()
		{
			if (PlayFabTitleDataCache.Instance.IsNotNull())
			{
				PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			}
		}

		// Token: 0x060059E3 RID: 23011 RVA: 0x001CC118 File Offset: 0x001CA318
		public bool TryGetItem(GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot, out CustomMapCosmeticItem foundItem)
		{
			if (!this.initializedFromTitleData)
			{
				this.UpdateFromTitleData();
			}
			foundItem = new CustomMapCosmeticItem
			{
				bustType = HeadModel_CosmeticStand.BustType.Disabled,
				playFabID = "INVALID"
			};
			for (int i = 0; i < this.customMapCosmeticItemList.Count; i++)
			{
				if (this.customMapCosmeticItemList[i].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.customMapCosmeticItemList[i];
					return true;
				}
			}
			for (int j = 0; j < this.fallbackItems.Count; j++)
			{
				if (this.fallbackItems[j].customMapItemSlot == customMapItemSlot)
				{
					foundItem = this.fallbackItems[j];
					return true;
				}
			}
			return false;
		}

		// Token: 0x060059E4 RID: 23012 RVA: 0x001CC1D4 File Offset: 0x001CA3D4
		private void UpdateFromTitleData()
		{
			if (this.initializedFromTitleData)
			{
				return;
			}
			if (PlayFabTitleDataCache.Instance.IsNull())
			{
				return;
			}
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdated));
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdated));
			if (PlayFabTitleDataCache.Instance == null)
			{
				Debug.LogError("[CustomMapCosmeticsData::UpdateFromTitleData] TitleData not available, using fallback item data.");
				this.initializedFromTitleData = true;
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.OnGetCosmeticsDataFromTitleData), new Action<PlayFabError>(this.OnPlayFabError), false);
			this.initializedFromTitleData = true;
		}

		// Token: 0x060059E5 RID: 23013 RVA: 0x001CC27C File Offset: 0x001CA47C
		private void OnTitleDataUpdated(string updatedKey)
		{
			if (updatedKey == this.titleDataKey)
			{
				this.initializedFromTitleData = false;
				this.UpdateFromTitleData();
			}
		}

		// Token: 0x060059E6 RID: 23014 RVA: 0x001CC29C File Offset: 0x001CA49C
		private void OnGetCosmeticsDataFromTitleData(string cosmeticsData)
		{
			string[] array = cosmeticsData.Split("|", 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = text;
				text2 = text2.RemoveAll('\\', 5);
				text2 = text2.Trim('"');
				CustomMapCosmeticItem itemFromJson = JsonUtility.FromJson<CustomMapCosmeticItem>(text2);
				this.customMapCosmeticItemList.RemoveAll((CustomMapCosmeticItem item) => item.customMapItemSlot == itemFromJson.customMapItemSlot);
				this.customMapCosmeticItemList.Add(itemFromJson);
			}
		}

		// Token: 0x060059E7 RID: 23015 RVA: 0x001CC316 File Offset: 0x001CA516
		private void OnPlayFabError(PlayFabError error)
		{
			Debug.LogError("[CustomMapCosmeticsData::OnPlayFabError] failed to retrieve CosmeticsData from PlayFab: " + error.ErrorMessage);
		}

		// Token: 0x040066F5 RID: 26357
		[SerializeField]
		private List<CustomMapCosmeticItem> fallbackItems;

		// Token: 0x040066F6 RID: 26358
		[SerializeField]
		private List<CustomMapCosmeticItem> customMapCosmeticItemList;

		// Token: 0x040066F7 RID: 26359
		public string titleDataKey = "CustomMapCosmeticData";

		// Token: 0x040066F8 RID: 26360
		private bool initializedFromTitleData;
	}
}
