using System;
using GorillaNetworking;
using LitJson;
using PlayFab;
using UnityEngine;

// Token: 0x02000A05 RID: 2565
public class AnnouncementManager : MonoBehaviour
{
	// Token: 0x060041A0 RID: 16800 RVA: 0x0015CA0E File Offset: 0x0015AC0E
	public bool ShowAnnouncement()
	{
		return this._showAnnouncement;
	}

	// Token: 0x1700061A RID: 1562
	// (get) Token: 0x060041A1 RID: 16801 RVA: 0x0015CA16 File Offset: 0x0015AC16
	// (set) Token: 0x060041A2 RID: 16802 RVA: 0x0015CA1E File Offset: 0x0015AC1E
	public bool _completedSetup { get; private set; }

	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x060041A3 RID: 16803 RVA: 0x0015CA27 File Offset: 0x0015AC27
	// (set) Token: 0x060041A4 RID: 16804 RVA: 0x0015CA2F File Offset: 0x0015AC2F
	public bool _announcementActive { get; private set; }

	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x060041A5 RID: 16805 RVA: 0x0015CA38 File Offset: 0x0015AC38
	public static AnnouncementManager Instance
	{
		get
		{
			if (AnnouncementManager._instance == null)
			{
				Debug.LogError("[KID::ANNOUNCEMENT] [_instance] is NULL, does it exist in the scene?");
			}
			return AnnouncementManager._instance;
		}
	}

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x060041A6 RID: 16806 RVA: 0x0015CA56 File Offset: 0x0015AC56
	private static string AnnouncementDPlayerPref
	{
		get
		{
			if (string.IsNullOrEmpty(AnnouncementManager._announcementIDPref))
			{
				AnnouncementManager._announcementIDPref = "announcement-id-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return AnnouncementManager._announcementIDPref;
		}
	}

	// Token: 0x060041A7 RID: 16807 RVA: 0x0015CA84 File Offset: 0x0015AC84
	private void Awake()
	{
		if (AnnouncementManager._instance != null)
		{
			Debug.LogError("[KID::ANNOUNCEMENT] [AnnouncementManager] has already been setup, does another already exist in the scene?");
			return;
		}
		AnnouncementManager._instance = this;
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Announcement Message Box has not been set. Announcement system will not work without it");
		}
	}

	// Token: 0x060041A8 RID: 16808 RVA: 0x0015CABC File Offset: 0x0015ACBC
	private void Start()
	{
		if (this._announcementMessageBox == null)
		{
			return;
		}
		this._announcementMessageBox.RightButton = "";
		this._announcementMessageBox.LeftButton = "Continue";
		PlayFabTitleDataCache.Instance.GetTitleData("AnnouncementData", new Action<string>(this.ConfigureAnnouncement), new Action<PlayFabError>(this.OnError), false);
	}

	// Token: 0x060041A9 RID: 16809 RVA: 0x0015CB20 File Offset: 0x0015AD20
	public void OnContinuePressed()
	{
		HandRayController.Instance.DisableHandRays();
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Message Box is null, Continue Button cannot work");
			return;
		}
		PrivateUIRoom.RemoveUI(this._announcementMessageBox.transform);
		this._announcementActive = false;
		PlayerPrefs.SetString(AnnouncementManager.AnnouncementDPlayerPref, this._announcementData.AnnouncementID);
		PlayerPrefs.Save();
	}

	// Token: 0x060041AA RID: 16810 RVA: 0x0015CB81 File Offset: 0x0015AD81
	private void OnError(PlayFabError error)
	{
		Debug.LogError("[ANNOUNCEMENT] Failed to Get Title Data for key [AnnouncementData]. Error:\n[" + error.ErrorMessage);
		this._completedSetup = true;
	}

	// Token: 0x060041AB RID: 16811 RVA: 0x0015CBA0 File Offset: 0x0015ADA0
	private void ConfigureAnnouncement(string data)
	{
		this._announcementString = data;
		this._announcementData = JsonMapper.ToObject<SAnnouncementData>(this._announcementString);
		if (!bool.TryParse(this._announcementData.ShowAnnouncement, ref this._showAnnouncement))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Failed to parse [ShowAnnouncement] with value [" + this._announcementData.ShowAnnouncement + "] to a bool, assuming false");
			return;
		}
		if (!this.ShowAnnouncement())
		{
			this._completedSetup = true;
			return;
		}
		if (string.IsNullOrEmpty(this._announcementData.AnnouncementID))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Announcement Version is empty or null. Will not show announcement");
			return;
		}
		string @string = PlayerPrefs.GetString(AnnouncementManager.AnnouncementDPlayerPref, "");
		if (this._announcementData.AnnouncementID == @string)
		{
			this._completedSetup = true;
			return;
		}
		PrivateUIRoom.ForceStartOverlay();
		HandRayController.Instance.EnableHandRays();
		this._announcementMessageBox.Header = this._announcementData.AnnouncementTitle;
		this._announcementMessageBox.Body = this._announcementData.Message;
		this._announcementActive = true;
		PrivateUIRoom.AddUI(this._announcementMessageBox.transform);
		this._completedSetup = true;
	}

	// Token: 0x04005282 RID: 21122
	private const string ANNOUNCEMENT_ID_PLAYERPREF_PREFIX = "announcement-id-";

	// Token: 0x04005283 RID: 21123
	private const string ANNOUNCEMENT_TITLE_DATA_KEY = "AnnouncementData";

	// Token: 0x04005284 RID: 21124
	private const string ANNOUNCEMENT_HEADING = "Announcement!";

	// Token: 0x04005285 RID: 21125
	private const string ANNOUNCEMENT_BUTTON_TEXT = "Continue";

	// Token: 0x04005286 RID: 21126
	[SerializeField]
	private MessageBox _announcementMessageBox;

	// Token: 0x04005287 RID: 21127
	private string _announcementString = string.Empty;

	// Token: 0x04005288 RID: 21128
	private SAnnouncementData _announcementData;

	// Token: 0x04005289 RID: 21129
	private bool _showAnnouncement;

	// Token: 0x0400528C RID: 21132
	private static AnnouncementManager _instance;

	// Token: 0x0400528D RID: 21133
	private static string _announcementIDPref = "";
}
