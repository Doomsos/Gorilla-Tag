using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200007D RID: 125
public class Menagerie : MonoBehaviour
{
	// Token: 0x06000307 RID: 775 RVA: 0x00012BD8 File Offset: 0x00010DD8
	private void Start()
	{
		CrittersCageDeposit[] array = Object.FindObjectsByType<CrittersCageDeposit>(1, 0);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnDepositCritter += new Action<Menagerie.CritterData, int>(this.OnDepositCritter);
		}
		CrittersManager.CheckInitialize();
		this._totalPages = this.critterIndex.critterTypes.Count / this.collection.Length + ((this.critterIndex.critterTypes.Count % this.collection.Length == 0) ? 0 : 1);
		this.Load();
		MenagerieDepositBox donationBox = this.DonationBox;
		donationBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(donationBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInDonationBox));
		MenagerieDepositBox favoriteBox = this.FavoriteBox;
		favoriteBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(favoriteBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInFavoriteBox));
		MenagerieDepositBox collectionBox = this.CollectionBox;
		collectionBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(collectionBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInCollectionBox));
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00012CD0 File Offset: 0x00010ED0
	private void CritterDepositedInDonationBox(MenagerieCritter critter)
	{
		if (Enumerable.Contains<MenagerieSlot>(this.newCritterPen, critter.Slot))
		{
			critter.currentState = MenagerieCritter.MenagerieCritterState.Donating;
			this.DonateCritter(critter.CritterData);
			this._savedCritters.newCritters.Remove(critter.CritterData);
			this.DespawnCritterFromSlot(critter.Slot);
			this.Save();
			PlayerGameEvents.CritterEvent("Donate" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00012D58 File Offset: 0x00010F58
	private void CritterDepositedInFavoriteBox(MenagerieCritter critter)
	{
		if (Enumerable.Contains<MenagerieSlot>(this.collection, critter.Slot))
		{
			this._savedCritters.favoriteCritter = critter.CritterData.critterType;
			this.Save();
			this.UpdateFavoriteCritter();
			PlayerGameEvents.CritterEvent("Favorite" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00012DC4 File Offset: 0x00010FC4
	private void CritterDepositedInCollectionBox(MenagerieCritter critter)
	{
		if (Enumerable.Contains<MenagerieSlot>(this.newCritterPen, critter.Slot))
		{
			this.AddCritterToCollection(critter.CritterData);
			this._savedCritters.newCritters.Remove(critter.CritterData);
			this.DespawnCritterFromSlot(critter.Slot);
			this.Save();
			this.UpdateFavoriteCritter();
			PlayerGameEvents.CritterEvent("Collect" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00012E4C File Offset: 0x0001104C
	private void OnDepositCritter(Menagerie.CritterData depositedCritter, int playerID)
	{
		try
		{
			if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.AddCritterToNewCritterPen(depositedCritter);
				this.Save();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00012E8C File Offset: 0x0001108C
	private void AddCritterToNewCritterPen(Menagerie.CritterData critterData)
	{
		if (this._savedCritters.newCritters.Count < this.newCritterPen.Length)
		{
			foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
			{
				if (!menagerieSlot.critter)
				{
					this.SpawnCritterInSlot(menagerieSlot, critterData);
					this._savedCritters.newCritters.Add(critterData);
					return;
				}
			}
		}
		this.DonateCritter(critterData);
		this.Save();
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00012F00 File Offset: 0x00011100
	private void AddCritterToCollection(Menagerie.CritterData critterData)
	{
		Menagerie.CritterData critterData2;
		if (this._savedCritters.collectedCritters.TryGetValue(critterData.critterType, ref critterData2))
		{
			this.DonateCritter(critterData2);
		}
		this._savedCritters.collectedCritters[critterData.critterType] = critterData;
		this.SpawnCollectionCritterIfShowing(critterData);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00012F4C File Offset: 0x0001114C
	private void DonateCritter(Menagerie.CritterData critterData)
	{
		this._savedCritters.donatedCritterCount++;
		this.donationCounter.SetText(string.Format(this.DonationText, this._savedCritters.donatedCritterCount));
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00012F88 File Offset: 0x00011188
	private void SpawnCritterInSlot(MenagerieSlot slot, Menagerie.CritterData critterData)
	{
		if (slot.IsNull() || critterData == null)
		{
			return;
		}
		this.DespawnCritterFromSlot(slot);
		MenagerieCritter menagerieCritter = Object.Instantiate<MenagerieCritter>(this.prefab, slot.critterMountPoint);
		menagerieCritter.Slot = slot;
		menagerieCritter.ApplyCritterData(critterData);
		this._critters.Add(menagerieCritter);
		if (slot.label)
		{
			slot.label.text = this.critterIndex[critterData.critterType].critterName;
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00013004 File Offset: 0x00011204
	private void SpawnCollectionCritterIfShowing(Menagerie.CritterData critter)
	{
		int num = critter.critterType - this._collectionPageIndex * this.collection.Length;
		if (num < 0 || num >= this.collection.Length)
		{
			return;
		}
		this.SpawnCritterInSlot(this.collection[num], critter);
	}

	// Token: 0x06000311 RID: 785 RVA: 0x00013047 File Offset: 0x00011247
	private void UpdateMenagerie()
	{
		this.UpdateNewCritterPen();
		this.UpdateCollection();
		this.UpdateFavoriteCritter();
		this.donationCounter.SetText(string.Format(this.DonationText, this._savedCritters.donatedCritterCount));
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00013084 File Offset: 0x00011284
	private void UpdateNewCritterPen()
	{
		for (int i = 0; i < this.newCritterPen.Length; i++)
		{
			if (i < this._savedCritters.newCritters.Count)
			{
				this.SpawnCritterInSlot(this.newCritterPen[i], this._savedCritters.newCritters[i]);
			}
			else
			{
				this.DespawnCritterFromSlot(this.newCritterPen[i]);
			}
		}
	}

	// Token: 0x06000313 RID: 787 RVA: 0x000130E8 File Offset: 0x000112E8
	private void UpdateCollection()
	{
		int num = this._collectionPageIndex * this.collection.Length;
		for (int i = 0; i < this.collection.Length; i++)
		{
			int num2 = num + i;
			MenagerieSlot menagerieSlot = this.collection[i];
			Menagerie.CritterData critterData;
			if (this._savedCritters.collectedCritters.TryGetValue(num2, ref critterData))
			{
				this.SpawnCritterInSlot(menagerieSlot, critterData);
			}
			else
			{
				this.DespawnCritterFromSlot(menagerieSlot);
				CritterConfiguration critterConfiguration = this.critterIndex[num2];
				menagerieSlot.label.text = ((critterConfiguration == null) ? "" : "??????");
			}
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00013178 File Offset: 0x00011378
	private void UpdateFavoriteCritter()
	{
		Menagerie.CritterData critterData;
		if (this._savedCritters.collectedCritters.TryGetValue(this._savedCritters.favoriteCritter, ref critterData))
		{
			this.SpawnCritterInSlot(this.favoriteCritterSlot, critterData);
			return;
		}
		this.ClearSlot(this.favoriteCritterSlot);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x000131BE File Offset: 0x000113BE
	public void NextGroupCollectedCritters()
	{
		this._collectionPageIndex++;
		if (this._collectionPageIndex >= this._totalPages)
		{
			this._collectionPageIndex = 0;
		}
		this.UpdateCollection();
	}

	// Token: 0x06000316 RID: 790 RVA: 0x000131E9 File Offset: 0x000113E9
	public void PrevGroupCollectedCritters()
	{
		this._collectionPageIndex--;
		if (this._collectionPageIndex < 0)
		{
			this._collectionPageIndex = this._totalPages - 1;
		}
		this.UpdateCollection();
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00013216 File Offset: 0x00011416
	private void GenerateNewCritters()
	{
		this.GenerateNewCritterCount(Random.Range(Mathf.Min(1, this.newCritterPen.Length), this.newCritterPen.Length + 1));
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0001323C File Offset: 0x0001143C
	private void GenerateLegalNewCritters()
	{
		this.ClearNewCritterPen();
		for (int i = 0; i < this.newCritterPen.Length; i++)
		{
			int randomCritterType = this.critterIndex.GetRandomCritterType(null);
			if (randomCritterType < 0)
			{
				Debug.LogError("Failed to spawn valid critter. No critter configuration found.");
				return;
			}
			Menagerie.CritterData critterData = new Menagerie.CritterData(randomCritterType, this.critterIndex[randomCritterType].GenerateAppearance());
			this.AddCritterToNewCritterPen(critterData);
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x000132A0 File Offset: 0x000114A0
	private void GenerateNewCritterCount(int critterCount)
	{
		this.ClearNewCritterPen();
		for (int i = 0; i < critterCount; i++)
		{
			int num = Random.Range(0, this.critterIndex.critterTypes.Count);
			CritterConfiguration critterConfiguration = this.critterIndex[num];
			Menagerie.CritterData critterData = new Menagerie.CritterData(num, critterConfiguration.GenerateAppearance());
			this.AddCritterToNewCritterPen(critterData);
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x000132F8 File Offset: 0x000114F8
	private void GenerateCollectedCritters(float spawnChance)
	{
		this.ClearCollection();
		for (int i = 0; i < this.critterIndex.critterTypes.Count; i++)
		{
			if (Random.value <= spawnChance)
			{
				CritterConfiguration critterConfiguration = this.critterIndex[i];
				Menagerie.CritterData critterData = new Menagerie.CritterData(i, critterConfiguration.GenerateAppearance());
				this.AddCritterToCollection(critterData);
				critterData.instance;
			}
		}
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0001335C File Offset: 0x0001155C
	private void MoveNewCrittersToCollection()
	{
		foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
		{
			if (menagerieSlot.critter)
			{
				this.CritterDepositedInCollectionBox(menagerieSlot.critter);
			}
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0001339C File Offset: 0x0001159C
	private void DonateNewCritters()
	{
		foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
		{
			if (menagerieSlot.critter)
			{
				this.CritterDepositedInDonationBox(menagerieSlot.critter);
			}
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x000133DB File Offset: 0x000115DB
	private void ClearSlot(MenagerieSlot slot)
	{
		this.DespawnCritterFromSlot(slot);
		if (slot.label)
		{
			slot.label.text = "";
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00013404 File Offset: 0x00011604
	private void DespawnCritterFromSlot(MenagerieSlot slot)
	{
		if (slot.IsNull())
		{
			return;
		}
		if (!slot.critter)
		{
			return;
		}
		this._critters.Remove(slot.critter);
		Object.Destroy(slot.critter.gameObject);
		slot.critter = null;
		if (slot.label)
		{
			slot.label.text = "";
		}
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0001346E File Offset: 0x0001166E
	private void ClearNewCritterPen()
	{
		this._savedCritters.newCritters.Clear();
		this.UpdateNewCritterPen();
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00013486 File Offset: 0x00011686
	private void ClearCollection()
	{
		this._savedCritters.collectedCritters.Clear();
		this.UpdateCollection();
		this.UpdateFavoriteCritter();
	}

	// Token: 0x06000321 RID: 801 RVA: 0x000134A4 File Offset: 0x000116A4
	private void ClearAll()
	{
		this._savedCritters.Clear();
		this.UpdateMenagerie();
	}

	// Token: 0x06000322 RID: 802 RVA: 0x000134B7 File Offset: 0x000116B7
	private void ResetSavedCreatures()
	{
		this.ClearAll();
		this.Save();
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000134C8 File Offset: 0x000116C8
	private void Load()
	{
		this.ClearAll();
		string @string = PlayerPrefs.GetString("_SavedCritters", string.Empty);
		this.LoadCrittersFromJson(@string);
		this.UpdateMenagerie();
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000134F8 File Offset: 0x000116F8
	private void Save()
	{
		Debug.Log(string.Format("Saving {0} critters", this._critters.Count));
		string text = this.SaveCrittersToJson();
		PlayerPrefs.SetString("_SavedCritters", text);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00013538 File Offset: 0x00011738
	private void LoadCrittersFromJson(string jsonString)
	{
		this._savedCritters.Clear();
		if (!string.IsNullOrEmpty(jsonString))
		{
			try
			{
				this._savedCritters = JsonConvert.DeserializeObject<Menagerie.CritterSaveData>(jsonString);
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to deserialize critters from json: " + jsonString);
				Debug.LogException(ex);
			}
		}
		this.ValidateSaveData();
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00013594 File Offset: 0x00011794
	private string SaveCrittersToJson()
	{
		this.ValidateSaveData();
		string text = JsonConvert.SerializeObject(this._savedCritters, 1);
		Debug.Log("Critters save to JSON: " + text);
		return text;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x000135C8 File Offset: 0x000117C8
	private void ValidateSaveData()
	{
		if (this._savedCritters.newCritters.Count > this.newCritterPen.Length)
		{
			Debug.LogError(string.Format("Too many new critters in CrittersSaveData ({0} vs {1}) - correcting.", this._savedCritters.newCritters.Count, this.newCritterPen.Length));
			while (this._savedCritters.newCritters.Count > this.newCritterPen.Length)
			{
				this._savedCritters.newCritters.RemoveAt(this.newCritterPen.Length);
			}
			this.Save();
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0001365C File Offset: 0x0001185C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		MenagerieSlot[] array = this.newCritterPen;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].critterMountPoint.position, 0.1f);
		}
		array = this.collection;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].critterMountPoint.position, 0.1f);
		}
		Gizmos.DrawWireSphere(this.favoriteCritterSlot.critterMountPoint.position, 0.1f);
	}

	// Token: 0x040003AD RID: 941
	[FormerlySerializedAs("creatureIndex")]
	public CritterIndex critterIndex;

	// Token: 0x040003AE RID: 942
	public MenagerieCritter prefab;

	// Token: 0x040003AF RID: 943
	private List<MenagerieCritter> _critters = new List<MenagerieCritter>();

	// Token: 0x040003B0 RID: 944
	private Menagerie.CritterSaveData _savedCritters = new Menagerie.CritterSaveData();

	// Token: 0x040003B1 RID: 945
	public MenagerieSlot[] collection;

	// Token: 0x040003B2 RID: 946
	public MenagerieSlot[] newCritterPen;

	// Token: 0x040003B3 RID: 947
	public MenagerieSlot favoriteCritterSlot;

	// Token: 0x040003B4 RID: 948
	private int _collectionPageIndex;

	// Token: 0x040003B5 RID: 949
	private int _totalPages;

	// Token: 0x040003B6 RID: 950
	public MenagerieDepositBox DonationBox;

	// Token: 0x040003B7 RID: 951
	public MenagerieDepositBox FavoriteBox;

	// Token: 0x040003B8 RID: 952
	public MenagerieDepositBox CollectionBox;

	// Token: 0x040003B9 RID: 953
	public TextMeshPro donationCounter;

	// Token: 0x040003BA RID: 954
	public string DonationText = "DONATED:{0}";

	// Token: 0x040003BB RID: 955
	private const string CrittersSavePrefsKey = "_SavedCritters";

	// Token: 0x0200007E RID: 126
	public class CritterData
	{
		// Token: 0x0600032A RID: 810 RVA: 0x0001370E File Offset: 0x0001190E
		public CritterConfiguration GetConfiguration()
		{
			return CrittersManager.instance.creatureIndex[this.critterType];
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00002050 File Offset: 0x00000250
		public CritterData()
		{
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00013727 File Offset: 0x00011927
		public CritterData(CritterConfiguration config, CritterAppearance appearance)
		{
			this.critterType = CrittersManager.instance.creatureIndex.critterTypes.IndexOf(config);
			this.appearance = appearance;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00013753 File Offset: 0x00011953
		public CritterData(int critterType, CritterAppearance appearance)
		{
			this.critterType = critterType;
			this.appearance = appearance;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00013769 File Offset: 0x00011969
		public CritterData(CritterVisuals visuals)
		{
			this.critterType = visuals.critterType;
			this.appearance = visuals.Appearance;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00013789 File Offset: 0x00011989
		public CritterData(Menagerie.CritterData source)
		{
			this.critterType = source.critterType;
			this.appearance = source.appearance;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000137A9 File Offset: 0x000119A9
		public override string ToString()
		{
			return string.Format("{0} {1} [instance]", this.critterType, this.appearance);
		}

		// Token: 0x040003BC RID: 956
		public int critterType;

		// Token: 0x040003BD RID: 957
		public CritterAppearance appearance;

		// Token: 0x040003BE RID: 958
		[NonSerialized]
		public MenagerieCritter instance;
	}

	// Token: 0x0200007F RID: 127
	[Serializable]
	public class CritterSaveData
	{
		// Token: 0x06000331 RID: 817 RVA: 0x000137CB File Offset: 0x000119CB
		public void Clear()
		{
			this.newCritters.Clear();
			this.collectedCritters.Clear();
			this.donatedCritterCount = 0;
			this.favoriteCritter = -1;
		}

		// Token: 0x040003BF RID: 959
		public List<Menagerie.CritterData> newCritters = new List<Menagerie.CritterData>();

		// Token: 0x040003C0 RID: 960
		public Dictionary<int, Menagerie.CritterData> collectedCritters = new Dictionary<int, Menagerie.CritterData>();

		// Token: 0x040003C1 RID: 961
		public int donatedCritterCount;

		// Token: 0x040003C2 RID: 962
		public int favoriteCritter = -1;
	}
}
