using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB2 RID: 3506
	public class BuilderFactory : MonoBehaviour
	{
		// Token: 0x06005635 RID: 22069 RVA: 0x001B1A09 File Offset: 0x001AFC09
		private void Awake()
		{
			this.InitIfNeeded();
		}

		// Token: 0x06005636 RID: 22070 RVA: 0x001B1A14 File Offset: 0x001AFC14
		public void InitIfNeeded()
		{
			if (this.initialized)
			{
				return;
			}
			this.buildItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnBuildItem));
			this.currPieceTypeIndex = 0;
			this.prevItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevItem));
			this.nextItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextItem));
			this.currPieceMaterialIndex = 0;
			this.prevMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevMaterial));
			this.nextMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextMaterial));
			this.pieceTypeToIndex = new Dictionary<int, int>(256);
			this.initialized = true;
			if (this.resourceCostUIs != null)
			{
				for (int i = 0; i < this.resourceCostUIs.Count; i++)
				{
					if (this.resourceCostUIs[i] != null)
					{
						this.resourceCostUIs[i].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06005637 RID: 22071 RVA: 0x001B1B0C File Offset: 0x001AFD0C
		public void Setup(BuilderTable tableOwner)
		{
			this.table = tableOwner;
			this.InitIfNeeded();
			List<BuilderPiece> list = this.pieceList;
			this.pieceTypes = new List<int>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				string name = list[i].name;
				int staticHash = name.GetStaticHash();
				int num;
				if (this.pieceTypeToIndex.TryAdd(staticHash, i))
				{
					this.pieceTypes.Add(staticHash);
				}
				else if (this.pieceTypeToIndex.TryGetValue(staticHash, ref num))
				{
					string text = "BuilderFactory: ERROR!! " + string.Format("Could not add pieceType \"{0}\" with hash {1} ", name, staticHash) + "to 'pieceTypeToIndex' Dictionary because because it was already added!";
					if (num < 0 || num >= list.Count)
					{
						text += " Also the index to the conflicting piece is out of range of the pieceList!";
					}
					else
					{
						BuilderPiece builderPiece = list[num];
						if (builderPiece != null)
						{
							if (name == builderPiece.name)
							{
								text += "The conflicting piece has the same name (as expected).";
							}
							else
							{
								text = text + "Also the conflicting pieceType has the same hash but different name \"" + builderPiece.name + "\"!";
							}
						}
						else
						{
							text += "And (should never happen) the piece at that slot is null!";
						}
					}
					Debug.LogError(text, this);
				}
			}
			int num2 = this.pieceTypes.Count;
			foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.GetAllPieceSets())
			{
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash2 = pieceInfo.piecePrefab.name.GetStaticHash();
						if (!this.pieceTypeToIndex.ContainsKey(staticHash2))
						{
							this.pieceList.Add(pieceInfo.piecePrefab);
							this.pieceTypes.Add(staticHash2);
							this.pieceTypeToIndex.Add(staticHash2, num2);
							num2++;
						}
					}
				}
			}
		}

		// Token: 0x06005638 RID: 22072 RVA: 0x001B1D70 File Offset: 0x001AFF70
		public void Show()
		{
			this.RefreshUI();
		}

		// Token: 0x06005639 RID: 22073 RVA: 0x001B1D78 File Offset: 0x001AFF78
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			int num;
			if (this.pieceTypeToIndex.TryGetValue(pieceType, ref num))
			{
				return this.pieceList[num];
			}
			Debug.LogErrorFormat("No Prefab found for type {0}", new object[]
			{
				pieceType
			});
			return null;
		}

		// Token: 0x0600563A RID: 22074 RVA: 0x001B1DBC File Offset: 0x001AFFBC
		public void OnBuildItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > this.currPieceTypeIndex)
			{
				int selectedMaterialType = this.GetSelectedMaterialType();
				this.table.RequestCreatePiece(this.pieceTypes[this.currPieceTypeIndex], this.spawnLocation.position, this.spawnLocation.rotation, selectedMaterialType);
				if (this.audioSource != null && this.buildPieceSound != null)
				{
					this.audioSource.GTPlayOneShot(this.buildPieceSound, 1f);
				}
			}
		}

		// Token: 0x0600563B RID: 22075 RVA: 0x001B1E50 File Offset: 0x001B0050
		public void OnPrevItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex - 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x0600563C RID: 22076 RVA: 0x001B1ED0 File Offset: 0x001B00D0
		public void OnNextItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex + 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x0600563D RID: 22077 RVA: 0x001B1F50 File Offset: 0x001B0150
		public void OnPrevMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex - 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x0600563E RID: 22078 RVA: 0x001B2020 File Offset: 0x001B0220
		public void OnNextMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex + 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x0600563F RID: 22079 RVA: 0x001B20F0 File Offset: 0x001B02F0
		private int GetSelectedMaterialType()
		{
			int result = -1;
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					result = materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode();
				}
			}
			return result;
		}

		// Token: 0x06005640 RID: 22080 RVA: 0x001B2174 File Offset: 0x001B0374
		private string GetSelectedMaterialName()
		{
			string result = "DEFAULT";
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					result = materialOptions.options[this.currPieceMaterialIndex].materialId;
				}
			}
			return result;
		}

		// Token: 0x06005641 RID: 22081 RVA: 0x001B21F4 File Offset: 0x001B03F4
		public bool CanBuildPieceType(int pieceType)
		{
			BuilderPiece piecePrefab = this.GetPiecePrefab(pieceType);
			return !(piecePrefab == null) && !piecePrefab.isBuiltIntoTable;
		}

		// Token: 0x06005642 RID: 22082 RVA: 0x00027DED File Offset: 0x00025FED
		public bool CanUseMaterialType(int materalType)
		{
			return true;
		}

		// Token: 0x06005643 RID: 22083 RVA: 0x001B2220 File Offset: 0x001B0420
		public void RefreshUI()
		{
			if (this.pieceList != null && this.pieceList.Count > this.currPieceTypeIndex)
			{
				this.itemLabel.SetText(this.pieceList[this.currPieceTypeIndex].displayName);
			}
			else
			{
				this.itemLabel.SetText("No Items");
			}
			if (this.previewPiece != null)
			{
				this.table.builderPool.DestroyPiece(this.previewPiece);
				this.previewPiece = null;
			}
			if (this.currPieceTypeIndex < 0 || this.currPieceTypeIndex >= this.pieceTypes.Count)
			{
				return;
			}
			int pieceType = this.pieceTypes[this.currPieceTypeIndex];
			this.previewPiece = this.table.builderPool.CreatePiece(pieceType, false);
			this.previewPiece.SetTable(this.table);
			this.previewPiece.pieceType = pieceType;
			string selectedMaterialName = this.GetSelectedMaterialName();
			this.materialLabel.SetText(selectedMaterialName);
			this.previewPiece.SetScale(this.table.pieceScale * 0.75f);
			this.previewPiece.SetupPiece(this.table.gridSize);
			int selectedMaterialType = this.GetSelectedMaterialType();
			this.previewPiece.SetMaterial(selectedMaterialType, true);
			this.previewPiece.transform.SetPositionAndRotation(this.previewMarker.position, this.previewMarker.rotation);
			this.previewPiece.SetState(BuilderPiece.State.Displayed, false);
			this.previewPiece.enabled = false;
			this.RefreshCostUI();
		}

		// Token: 0x06005644 RID: 22084 RVA: 0x001B23A8 File Offset: 0x001B05A8
		private void RefreshCostUI()
		{
			List<BuilderResourceQuantity> list = null;
			if (this.previewPiece != null)
			{
				list = this.previewPiece.cost.quantities;
			}
			for (int i = 0; i < this.resourceCostUIs.Count; i++)
			{
				if (!(this.resourceCostUIs[i] == null))
				{
					bool flag = list != null && i < list.Count;
					this.resourceCostUIs[i].gameObject.SetActive(flag);
					if (flag)
					{
						this.resourceCostUIs[i].SetResourceCost(list[i], this.table);
					}
				}
			}
		}

		// Token: 0x06005645 RID: 22085 RVA: 0x001B2448 File Offset: 0x001B0648
		public void OnAvailableResourcesChange()
		{
			this.RefreshCostUI();
		}

		// Token: 0x06005646 RID: 22086 RVA: 0x001B2450 File Offset: 0x001B0650
		public void CreateRandomPiece()
		{
			Debug.LogError("Create Random Piece No longer implemented");
		}

		// Token: 0x04006356 RID: 25430
		public Transform spawnLocation;

		// Token: 0x04006357 RID: 25431
		private List<int> pieceTypes;

		// Token: 0x04006358 RID: 25432
		public List<GameObject> itemList;

		// Token: 0x04006359 RID: 25433
		[HideInInspector]
		public List<BuilderPiece> pieceList;

		// Token: 0x0400635A RID: 25434
		public BuilderOptionButton buildItemButton;

		// Token: 0x0400635B RID: 25435
		public TextMeshPro itemLabel;

		// Token: 0x0400635C RID: 25436
		public BuilderOptionButton prevItemButton;

		// Token: 0x0400635D RID: 25437
		public BuilderOptionButton nextItemButton;

		// Token: 0x0400635E RID: 25438
		public TextMeshPro materialLabel;

		// Token: 0x0400635F RID: 25439
		public BuilderOptionButton prevMaterialButton;

		// Token: 0x04006360 RID: 25440
		public BuilderOptionButton nextMaterialButton;

		// Token: 0x04006361 RID: 25441
		public AudioSource audioSource;

		// Token: 0x04006362 RID: 25442
		public AudioClip buildPieceSound;

		// Token: 0x04006363 RID: 25443
		public Transform previewMarker;

		// Token: 0x04006364 RID: 25444
		public List<BuilderUIResource> resourceCostUIs;

		// Token: 0x04006365 RID: 25445
		private BuilderPiece previewPiece;

		// Token: 0x04006366 RID: 25446
		private int currPieceTypeIndex;

		// Token: 0x04006367 RID: 25447
		private int currPieceMaterialIndex;

		// Token: 0x04006368 RID: 25448
		private Dictionary<int, int> pieceTypeToIndex;

		// Token: 0x04006369 RID: 25449
		private BuilderTable table;

		// Token: 0x0400636A RID: 25450
		private bool initialized;
	}
}
