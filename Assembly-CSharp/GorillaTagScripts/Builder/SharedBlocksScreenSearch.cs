using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E8D RID: 3725
	public class SharedBlocksScreenSearch : SharedBlocksScreen, IGorillaSliceableSimple
	{
		// Token: 0x06005D0A RID: 23818 RVA: 0x001DDC20 File Offset: 0x001DBE20
		public override void OnSelectPressed()
		{
			if (SharedBlocksManager.IsMapIDValid(this.currentMapCode))
			{
				this.savedMapCode = this.currentMapCode;
				this.terminal.SelectMapIDAndOpenInfo(this.savedMapCode);
				return;
			}
			if (this.currentMapCode.Length < 8)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH", out text, "INVALID MAP ID LENGTH"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH]");
				}
				this.terminal.SetStatusText(text);
				return;
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID", out text2, "INVALID MAP ID"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID]");
			}
			this.terminal.SetStatusText(text2);
		}

		// Token: 0x06005D0B RID: 23819 RVA: 0x001DDCB9 File Offset: 0x001DBEB9
		public override void OnDeletePressed()
		{
			if (this.currentMapCode.Length > 0)
			{
				this.currentMapCode = this.currentMapCode.Substring(0, this.currentMapCode.Length - 1);
				this.UpdateInput();
			}
		}

		// Token: 0x06005D0C RID: 23820 RVA: 0x001DDCEE File Offset: 0x001DBEEE
		public override void OnNumberPressed(int number)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += number.ToString();
				this.UpdateInput();
			}
		}

		// Token: 0x06005D0D RID: 23821 RVA: 0x001DDD1C File Offset: 0x001DBF1C
		public override void OnLetterPressed(string letter)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += letter;
				this.UpdateInput();
			}
		}

		// Token: 0x06005D0E RID: 23822 RVA: 0x001DDD44 File Offset: 0x001DBF44
		public override void Show()
		{
			SharedBlocksManager.OnRecentMapIdsUpdated += new Action(this.DrawScreen);
			this.currentMapCode = string.Empty;
			this.DrawScreen();
			base.Show();
			this.RefreshPlayerCounter();
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnMapLoaded(table.GetCurrentMapID());
			}
		}

		// Token: 0x06005D0F RID: 23823 RVA: 0x001DDDD0 File Offset: 0x001DBFD0
		public override void Hide()
		{
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			SharedBlocksManager.OnRecentMapIdsUpdated -= new Action(this.DrawScreen);
			base.Hide();
		}

		// Token: 0x06005D10 RID: 23824 RVA: 0x001DDE58 File Offset: 0x001DC058
		private void OnMapLoaded(string mapID)
		{
			string defaultResult = "LOADED MAP : " + (SharedBlocksManager.IsMapIDValid(mapID) ? SharedBlocksTerminal.MapIDToDisplayedString(mapID) : "NONE");
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale(SharedBlocksManager.IsMapIDValid(mapID) ? "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL" : "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE", out text, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL]");
			}
			text = text.Replace("{mapDisplayName}", SharedBlocksTerminal.MapIDToDisplayedString(mapID));
			this.loadedMap.text = text;
		}

		// Token: 0x06005D11 RID: 23825 RVA: 0x001DDECC File Offset: 0x001DC0CC
		private void OnMapCleared()
		{
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE", out text, "LOADED MAP : NONE"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE]");
			}
			this.loadedMap.text = text;
		}

		// Token: 0x06005D12 RID: 23826 RVA: 0x001DDF04 File Offset: 0x001DC104
		private void UpdateInput()
		{
			string defaultResult = "MAP SEARCH : ";
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH", out text, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH]");
			}
			text += SharedBlocksTerminal.MapIDToDisplayedString(this.currentMapCode);
			this.inputText.text = text;
		}

		// Token: 0x06005D13 RID: 23827 RVA: 0x001DDF4E File Offset: 0x001DC14E
		public void SetMapCode(string mapCode)
		{
			if (mapCode == null)
			{
				this.currentMapCode = string.Empty;
			}
			else
			{
				this.currentMapCode = mapCode;
			}
			this.UpdateInput();
		}

		// Token: 0x06005D14 RID: 23828 RVA: 0x001DDF6D File Offset: 0x001DC16D
		public void SetInputTextEnabled(bool enabled)
		{
			if (enabled)
			{
				this.inputText.color = Color.white;
				return;
			}
			this.inputText.color = Color.gray;
		}

		// Token: 0x06005D15 RID: 23829 RVA: 0x001DDF94 File Offset: 0x001DC194
		private void DrawScreen()
		{
			this.UpdateInput();
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_VOTES", out text, "RECENT VOTES"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_VOTES]");
			}
			this.sb.Clear();
			this.sb.Append(text + "\n");
			foreach (string mapID in SharedBlocksManager.GetRecentUpVotes())
			{
				if (SharedBlocksManager.IsMapIDValid(mapID))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(mapID));
					this.sb.Append("\n");
				}
			}
			this.recentList.text = this.sb.ToString();
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL", out text, "MY MAPS"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL]");
			}
			this.sb.Clear();
			this.sb.Append(text + "\n");
			foreach (string mapID2 in SharedBlocksManager.GetLocalMapIDs())
			{
				if (SharedBlocksManager.IsMapIDValid(mapID2))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(mapID2));
					this.sb.Append("\n");
				}
			}
			this.myScanList.text = this.sb.ToString();
		}

		// Token: 0x06005D16 RID: 23830 RVA: 0x001DE124 File Offset: 0x001DC324
		private void RefreshPlayerCounter()
		{
			this.terminal.RefreshLobbyCount();
			this.playerCountText.text = this.terminal.GetLobbyText();
			this.playersInLobbyWarning.gameObject.SetActive(!this.terminal.AreAllPlayersInLobby());
		}

		// Token: 0x06005D17 RID: 23831 RVA: 0x001DE170 File Offset: 0x001DC370
		public void SliceUpdate()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06005D18 RID: 23832 RVA: 0x001DE178 File Offset: 0x001DC378
		public void OnEnable()
		{
			if (!this.updating)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = true;
			}
			this.RefreshPlayerCounter();
			RoomSystem.PlayersChangedEvent += new Action(this.PlayersChangedEvent);
		}

		// Token: 0x06005D19 RID: 23833 RVA: 0x001DE170 File Offset: 0x001DC370
		private void PlayersChangedEvent()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06005D1A RID: 23834 RVA: 0x001DE1B1 File Offset: 0x001DC3B1
		public void OnDisable()
		{
			if (this.updating)
			{
				GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = false;
			}
			RoomSystem.PlayersChangedEvent -= new Action(this.PlayersChangedEvent);
		}

		// Token: 0x04006ABC RID: 27324
		[SerializeField]
		private TMP_Text loadedMap;

		// Token: 0x04006ABD RID: 27325
		[SerializeField]
		private TMP_Text inputText;

		// Token: 0x04006ABE RID: 27326
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04006ABF RID: 27327
		[SerializeField]
		private TMP_Text recentList;

		// Token: 0x04006AC0 RID: 27328
		[SerializeField]
		private TMP_Text myScanList;

		// Token: 0x04006AC1 RID: 27329
		[SerializeField]
		private TMP_Text playerCountText;

		// Token: 0x04006AC2 RID: 27330
		[SerializeField]
		private TMP_Text playersInLobbyWarning;

		// Token: 0x04006AC3 RID: 27331
		private string currentMapCode;

		// Token: 0x04006AC4 RID: 27332
		private string savedMapCode;

		// Token: 0x04006AC5 RID: 27333
		private StringBuilder sb = new StringBuilder();

		// Token: 0x04006AC6 RID: 27334
		private bool updating;
	}
}
