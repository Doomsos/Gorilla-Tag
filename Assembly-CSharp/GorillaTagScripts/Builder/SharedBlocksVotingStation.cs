using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E92 RID: 3730
	public class SharedBlocksVotingStation : MonoBehaviour
	{
		// Token: 0x06005D4B RID: 23883 RVA: 0x001DF36C File Offset: 0x001DD56C
		private void Start()
		{
			this.SetupLocalization();
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.table = builderTable;
				this.table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnLoadedMapChanged(this.table.GetCurrentMapID());
			}
			else
			{
				GTDev.LogWarning<string>("No Builder Table found for Voting Station", null);
			}
			base.GetComponentsInChildren<MeshRenderer>(false, this.meshes);
			this.upVoteButton.onPressButton.AddListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.AddListener(new UnityAction(this.OnDownVotePressed));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06005D4C RID: 23884 RVA: 0x001DF45C File Offset: 0x001DD65C
		private void OnDestroy()
		{
			this.upVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnDownVotePressed));
			if (this.table != null)
			{
				this.table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06005D4D RID: 23885 RVA: 0x001DF51C File Offset: 0x001DD71C
		private void SetupLocalization()
		{
			if (this._statusLocText == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Trying to set up Localization, but [_statusLocText] is NULL");
				return;
			}
			if (this._screenLocText == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Trying to set up Localization, but [_screenLocText] is NULL");
				return;
			}
			string text = "voting-status-index";
			string text2 = "map-name-index";
			string text3 = "map-name";
			this._statusIndexVar = (this._statusLocText.StringReference[text] as IntVariable);
			this._mapDisplayIndexVar = (this._screenLocText.StringReference[text2] as IntVariable);
			this._mapNameVar = (this._screenLocText.StringReference[text3] as StringVariable);
			if (this._statusIndexVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [IntVariable] with var-name [" + text + "]");
			}
			if (this._mapDisplayIndexVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [IntVariable] with var-name [" + text2 + "]");
			}
			if (this._mapNameVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [StringVariable] with var-name [" + text3 + "]");
			}
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x001DF618 File Offset: 0x001DD818
		private void OnZoneChanged()
		{
			bool enabled = ZoneManagement.instance.IsZoneActive(this.tableZone);
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				meshRenderer.enabled = enabled;
			}
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x001DF67C File Offset: 0x001DD87C
		private void OnUpVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this._statusIndexVar.Value = 2;
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.upVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, true, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.upVoteButton.pressedMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.upVoteButton.enabled = false;
				this.downVoteButton.enabled = true;
			}
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x001DF738 File Offset: 0x001DD938
		private void OnDownVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this._statusIndexVar.Value = 2;
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.downVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, false, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.downVoteButton.pressedMaterial;
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = false;
			}
		}

		// Token: 0x06005D51 RID: 23889 RVA: 0x001DF7F4 File Offset: 0x001DD9F4
		private void OnVoteResponse(bool success, string message)
		{
			this.voteInProgress = false;
			if (success)
			{
				this._statusIndexVar.Value = 0;
				this.statusText.gameObject.SetActive(true);
			}
			else
			{
				int value;
				if (int.TryParse(message, ref value))
				{
					this._statusIndexVar.Value = value;
				}
				else
				{
					this.statusText.text = message;
					Debug.Log("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] WARNING: Passing in a non-int value for the [message]. This will not be localized!");
				}
				this.statusText.gameObject.SetActive(true);
				if (!this.loadedMapID.IsNullOrEmpty())
				{
					this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.upVoteButton.enabled = true;
					this.downVoteButton.enabled = true;
				}
			}
			this.clearStatusTime = Time.time + this.clearStatusDelay;
			this.waitingToClearStatus = true;
		}

		// Token: 0x06005D52 RID: 23890 RVA: 0x001DF8D6 File Offset: 0x001DDAD6
		private void LateUpdate()
		{
			if (this.waitingToClearStatus && Time.time > this.clearStatusTime)
			{
				this.waitingToClearStatus = false;
				this._statusIndexVar.Value = 2;
				this.statusText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x001DF911 File Offset: 0x001DDB11
		private void OnLoadedMapChanged(string mapID)
		{
			this.loadedMapID = mapID;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x001DF931 File Offset: 0x001DDB31
		private void OnMapCleared()
		{
			this.loadedMapID = null;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x06005D55 RID: 23893 RVA: 0x001DF954 File Offset: 0x001DDB54
		private void UpdateScreen()
		{
			if (!this.loadedMapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(this.loadedMapID))
			{
				this._mapDisplayIndexVar.Value = 1;
				this._mapNameVar.Value = SharedBlocksTerminal.MapIDToDisplayedString(this.loadedMapID);
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = true;
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				return;
			}
			this._mapDisplayIndexVar.Value = 0;
			this.upVoteButton.enabled = false;
			this.downVoteButton.enabled = false;
			this.upVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
			this.downVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
		}

		// Token: 0x04006B1A RID: 27418
		public const int VOTING_STATUS_INDEX_SUCCESS = 0;

		// Token: 0x04006B1B RID: 27419
		public const int VOTING_STATUS_INDEX_NOT_LOGGED_IN = 1;

		// Token: 0x04006B1C RID: 27420
		public const int VOTING_STATUS_INDEX_EMPTY = 2;

		// Token: 0x04006B1D RID: 27421
		private const int MAP_DISPLAY_INDEX_NONE = 0;

		// Token: 0x04006B1E RID: 27422
		private const int MAP_DISPLAY_INDEX_NAMED_MAP = 1;

		// Token: 0x04006B1F RID: 27423
		[SerializeField]
		private TMP_Text screenText;

		// Token: 0x04006B20 RID: 27424
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04006B21 RID: 27425
		[SerializeField]
		private GorillaPressableButton upVoteButton;

		// Token: 0x04006B22 RID: 27426
		[SerializeField]
		private GorillaPressableButton downVoteButton;

		// Token: 0x04006B23 RID: 27427
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04006B24 RID: 27428
		[SerializeField]
		private Material buttonDefaultMaterial;

		// Token: 0x04006B25 RID: 27429
		[SerializeField]
		private Material buttonDisabledMaterial;

		// Token: 0x04006B26 RID: 27430
		[Header("Localization Setup")]
		[SerializeField]
		private LocalizedText _statusLocText;

		// Token: 0x04006B27 RID: 27431
		[SerializeField]
		private LocalizedText _screenLocText;

		// Token: 0x04006B28 RID: 27432
		private BuilderTable table;

		// Token: 0x04006B29 RID: 27433
		private string loadedMapID = string.Empty;

		// Token: 0x04006B2A RID: 27434
		private bool voteInProgress;

		// Token: 0x04006B2B RID: 27435
		private bool waitingToClearStatus;

		// Token: 0x04006B2C RID: 27436
		private float clearStatusTime;

		// Token: 0x04006B2D RID: 27437
		private float clearStatusDelay = 2f;

		// Token: 0x04006B2E RID: 27438
		private IntVariable _statusIndexVar;

		// Token: 0x04006B2F RID: 27439
		private IntVariable _mapDisplayIndexVar;

		// Token: 0x04006B30 RID: 27440
		private StringVariable _mapNameVar;

		// Token: 0x04006B31 RID: 27441
		private List<MeshRenderer> meshes = new List<MeshRenderer>(12);
	}
}
