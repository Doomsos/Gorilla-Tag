using System;
using System.Collections.Generic;
using Modio.Mods;
using UnityEngine;

// Token: 0x02000998 RID: 2456
public class CustomMapsGalleryView : MonoBehaviour
{
	// Token: 0x06003EA4 RID: 16036 RVA: 0x0014F310 File Offset: 0x0014D510
	public void ResetGallery()
	{
		for (int i = 0; i < this.modTiles.Count; i++)
		{
			this.modTiles[i].DeactivateTile();
		}
	}

	// Token: 0x06003EA5 RID: 16037 RVA: 0x0014F344 File Offset: 0x0014D544
	public bool DisplayGallery(List<Mod> mods, bool useMapName, out string error)
	{
		if (mods.Count > this.modTiles.Count)
		{
			GTDev.LogError<string>("Displayed Mod list is longer than the number of mod tiles in the gallery", null);
			error = "Displayed Mod list is longer than the number of mod tiles in the gallery";
			return false;
		}
		for (int i = 0; i < mods.Count; i++)
		{
			this.modTiles[i].SetMod(mods[i], useMapName);
		}
		error = string.Empty;
		return true;
	}

	// Token: 0x06003EA6 RID: 16038 RVA: 0x0014F3AC File Offset: 0x0014D5AC
	public void ShowTileText(bool show, bool useMapName)
	{
		for (int i = 0; i < this.modTiles.Count; i++)
		{
			this.modTiles[i].ShowTileText(show, useMapName);
		}
	}

	// Token: 0x06003EA7 RID: 16039 RVA: 0x0014F3E2 File Offset: 0x0014D5E2
	public void ShowDetailsForEntry(int entryIndex)
	{
		if (this.modTiles.Count > entryIndex)
		{
			this.modTiles[entryIndex].ShowDetails();
		}
	}

	// Token: 0x06003EA8 RID: 16040 RVA: 0x0014F403 File Offset: 0x0014D603
	public void HighlightTileAtIndex(int tileIndex)
	{
		if (tileIndex > this.modTiles.Count)
		{
			return;
		}
		this.modTiles[tileIndex].HighlightTile();
	}

	// Token: 0x04004FA1 RID: 20385
	[SerializeField]
	private List<CustomMapsModTile> modTiles = new List<CustomMapsModTile>();
}
