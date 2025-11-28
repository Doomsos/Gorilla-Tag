using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F44 RID: 3908
	public class StoreController : MonoBehaviour
	{
		// Token: 0x060061EF RID: 25071 RVA: 0x001F8E9F File Offset: 0x001F709F
		public void Awake()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = this;
				return;
			}
			if (StoreController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x060061F0 RID: 25072 RVA: 0x00002789 File Offset: 0x00000989
		public void Start()
		{
		}

		// Token: 0x060061F1 RID: 25073 RVA: 0x001F8ED4 File Offset: 0x001F70D4
		public void CreateDynamicCosmeticStandsDictionatary()
		{
			this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				if (!storeDepartment.departmentName.IsNullOrEmpty())
				{
					foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
					{
						if (!storeDisplay.displayName.IsNullOrEmpty())
						{
							foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
							{
								if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
								{
									if (!this.CosmeticStandsDict.ContainsKey(string.Concat(new string[]
									{
										storeDepartment.departmentName,
										"|",
										storeDisplay.displayName,
										"|",
										dynamicCosmeticStand.StandName
									})))
									{
										this.CosmeticStandsDict.Add(string.Concat(new string[]
										{
											storeDepartment.departmentName,
											"|",
											storeDisplay.displayName,
											"|",
											dynamicCosmeticStand.StandName
										}), dynamicCosmeticStand);
									}
									else
									{
										Debug.LogError(string.Concat(new string[]
										{
											"StoreStuff: Duplicate Stand Name: ",
											storeDepartment.departmentName,
											"|",
											storeDisplay.displayName,
											"|",
											dynamicCosmeticStand.StandName,
											" Please Fix Gameobject : ",
											dynamicCosmeticStand.gameObject.GetPath(),
											dynamicCosmeticStand.gameObject.name
										}));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060061F2 RID: 25074 RVA: 0x001F90B0 File Offset: 0x001F72B0
		private void Create_StandsByPlayfabIDDictionary()
		{
			this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				this.AddStandToPlayfabIDDictionary(dynamicCosmeticStand);
			}
		}

		// Token: 0x060061F3 RID: 25075 RVA: 0x001F9114 File Offset: 0x001F7314
		public void AddStandToPlayfabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
			{
				if (dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
				{
					return;
				}
				if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
				{
					this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
					return;
				}
				Dictionary<string, List<DynamicCosmeticStand>> standsByPlayfabID = this.StandsByPlayfabID;
				string thisCosmeticName = dynamicCosmeticStand.thisCosmeticName;
				List<DynamicCosmeticStand> list = new List<DynamicCosmeticStand>();
				list.Add(dynamicCosmeticStand);
				standsByPlayfabID.Add(thisCosmeticName, list);
			}
		}

		// Token: 0x060061F4 RID: 25076 RVA: 0x001F9184 File Offset: 0x001F7384
		public void RemoveStandFromPlayFabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			List<DynamicCosmeticStand> list;
			if (this.StandsByPlayfabID.TryGetValue(dynamicCosmeticStand.thisCosmeticName, ref list))
			{
				list.Remove(dynamicCosmeticStand);
			}
		}

		// Token: 0x060061F5 RID: 25077 RVA: 0x00002789 File Offset: 0x00000989
		public void ExportCosmeticStandLayoutWithItems()
		{
		}

		// Token: 0x060061F6 RID: 25078 RVA: 0x00002789 File Offset: 0x00000989
		public void ExportCosmeticStandLayoutWITHOUTItems()
		{
		}

		// Token: 0x060061F7 RID: 25079 RVA: 0x00002789 File Offset: 0x00000989
		public void ImportCosmeticStandLayout()
		{
		}

		// Token: 0x060061F8 RID: 25080 RVA: 0x001F91AE File Offset: 0x001F73AE
		private void InitializeFromTitleData()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("StoreLayoutData", delegate(string data)
			{
				this.ImportCosmeticStandLayoutFromTitleData(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting StoreLayoutData data: {0}", e));
			}, false);
		}

		// Token: 0x060061F9 RID: 25081 RVA: 0x001F91EC File Offset: 0x001F73EC
		private void ImportCosmeticStandLayoutFromTitleData(string TSVData)
		{
			StandImport standImport = new StandImport();
			standImport.DecomposeFromTitleDataString(TSVData);
			foreach (StandTypeData standTypeData in standImport.standData)
			{
				string text = string.Concat(new string[]
				{
					standTypeData.departmentID,
					"|",
					standTypeData.displayID,
					"|",
					standTypeData.standID
				});
				if (this.CosmeticStandsDict.ContainsKey(text))
				{
					Debug.Log(string.Concat(new string[]
					{
						"StoreStuff: Stand Updated: ",
						standTypeData.departmentID,
						"|",
						standTypeData.displayID,
						"|",
						standTypeData.standID,
						"|",
						standTypeData.bustType,
						"|",
						standTypeData.playFabID,
						"|"
					}));
					this.CosmeticStandsDict[text].SetStandTypeString(standTypeData.bustType);
					Debug.Log("Manually Initializing Stand: " + text + " |||| " + standTypeData.playFabID);
					this.CosmeticStandsDict[text].SpawnItemOntoStand(standTypeData.playFabID);
					this.CosmeticStandsDict[text].InitializeCosmetic();
				}
			}
		}

		// Token: 0x060061FA RID: 25082 RVA: 0x001F9368 File Offset: 0x001F7568
		public void InitalizeCosmeticStands()
		{
			this.CreateDynamicCosmeticStandsDictionatary();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.InitializeCosmetic();
			}
			this.Create_StandsByPlayfabIDDictionary();
			if (this.LoadFromTitleData)
			{
				this.InitializeFromTitleData();
			}
		}

		// Token: 0x060061FB RID: 25083 RVA: 0x001F93D8 File Offset: 0x001F75D8
		public void LoadCosmeticOntoStand(string standID, string playFabId)
		{
			if (this.CosmeticStandsDict.ContainsKey(standID))
			{
				this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
				Debug.Log("StoreStuff: Cosmetic Loaded Onto Stand: " + standID + " | " + playFabId);
			}
		}

		// Token: 0x060061FC RID: 25084 RVA: 0x001F9410 File Offset: 0x001F7610
		public void ClearCosmetics()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				StoreDisplay[] displays = storeDepartment.Displays;
				for (int i = 0; i < displays.Length; i++)
				{
					DynamicCosmeticStand[] stands = displays[i].Stands;
					for (int j = 0; j < stands.Length; j++)
					{
						stands[j].ClearCosmetics();
					}
				}
			}
		}

		// Token: 0x060061FD RID: 25085 RVA: 0x001F9494 File Offset: 0x001F7694
		public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
		}

		// Token: 0x060061FE RID: 25086 RVA: 0x001F94C4 File Offset: 0x001F76C4
		public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (dynamicCosmeticStand.thisCosmeticName == PlayFabID)
				{
					return dynamicCosmeticStand;
				}
			}
			return null;
		}

		// Token: 0x060061FF RID: 25087 RVA: 0x001F952C File Offset: 0x001F772C
		public void FindAllDepartments()
		{
			this.Departments = Enumerable.ToList<StoreDepartment>(Object.FindObjectsByType<StoreDepartment>(0));
		}

		// Token: 0x06006200 RID: 25088 RVA: 0x001F9540 File Offset: 0x001F7740
		public void SaveAllCosmeticsPositions()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
					{
						Debug.Log(string.Concat(new string[]
						{
							"StoreStuff: Saving Items mount transform: ",
							storeDepartment.departmentName,
							"|",
							storeDisplay.displayName,
							"|",
							dynamicCosmeticStand.StandName,
							"|",
							dynamicCosmeticStand.DisplayHeadModel.bustType.ToString(),
							"|",
							dynamicCosmeticStand.thisCosmeticName
						}));
						dynamicCosmeticStand.UpdateCosmeticsMountPositions();
					}
				}
			}
		}

		// Token: 0x06006201 RID: 25089 RVA: 0x001F9660 File Offset: 0x001F7860
		public static void SetForGame()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			StoreController.instance.CreateDynamicCosmeticStandsDictionatary();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
				dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
			}
		}

		// Token: 0x040070A8 RID: 28840
		public static volatile StoreController instance;

		// Token: 0x040070A9 RID: 28841
		public List<StoreDepartment> Departments;

		// Token: 0x040070AA RID: 28842
		private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;

		// Token: 0x040070AB RID: 28843
		public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;

		// Token: 0x040070AC RID: 28844
		public AllCosmeticsArraySO AllCosmeticsArraySO;

		// Token: 0x040070AD RID: 28845
		public bool LoadFromTitleData;

		// Token: 0x040070AE RID: 28846
		private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";
	}
}
