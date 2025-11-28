using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200059F RID: 1439
public class BuilderScanKiosk : MonoBehaviourTick
{
	// Token: 0x06002444 RID: 9284 RVA: 0x000C2712 File Offset: 0x000C0912
	public static bool IsSaveSlotValid(int slot)
	{
		return slot >= 0 && slot < BuilderScanKiosk.NUM_SAVE_SLOTS;
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000C2724 File Offset: 0x000C0924
	private void Start()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.AddListener(new UnityAction(this.OnSavePressed));
		}
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.AddListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveSuccess.AddListener(new UnityAction(this.OnSaveSuccess));
			this.targetTable.OnSaveFailure.AddListener(new UnityAction<string>(this.OnSaveFail));
			SharedBlocksManager.OnSaveTimeUpdated += new Action(this.OnSaveTimeUpdated);
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.AddListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			gorillaPressableButton.onPressed += new Action<GorillaPressableButton, bool>(this.OnScanButtonPressed);
		}
		this.scanTriangle = this.scanAnimation.GetComponent<MeshRenderer>();
		this.scanTriangle.enabled = false;
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.LoadPlayerPrefs();
		this.UpdateUI();
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000C2878 File Offset: 0x000C0A78
	private new void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.UpdateUI));
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x000C288B File Offset: 0x000C0A8B
	private new void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.UpdateUI));
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x000C28A0 File Offset: 0x000C0AA0
	private void OnDestroy()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.RemoveListener(new UnityAction(this.OnSavePressed));
		}
		SharedBlocksManager.OnSaveTimeUpdated -= new Action(this.OnSaveTimeUpdated);
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.RemoveListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveFailure.RemoveListener(new UnityAction<string>(this.OnSaveFail));
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.RemoveListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= new Action<GorillaPressableButton, bool>(this.OnScanButtonPressed);
			}
		}
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x000C29B4 File Offset: 0x000C0BB4
	private void OnNoneButtonPressed()
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		if (this.targetTable.CurrentSaveSlot != -1)
		{
			this.targetTable.CurrentSaveSlot = -1;
			this.SavePlayerPrefs();
			this.UpdateUI();
		}
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x000C2A08 File Offset: 0x000C0C08
	private void OnScanButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		int i = 0;
		while (i < this.scanButtons.Count)
		{
			if (button.Equals(this.scanButtons[i]))
			{
				if (i != this.targetTable.CurrentSaveSlot)
				{
					this.targetTable.CurrentSaveSlot = i;
					this.SavePlayerPrefs();
					this.UpdateUI();
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDevScanPressed()
	{
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000C2A88 File Offset: 0x000C0C88
	private void LoadPlayerPrefs()
	{
		int @int = PlayerPrefs.GetInt(BuilderScanKiosk.playerPrefKey, -1);
		this.targetTable.CurrentSaveSlot = @int;
		this.UpdateUI();
	}

	// Token: 0x0600244D RID: 9293 RVA: 0x000C2AB3 File Offset: 0x000C0CB3
	private void SavePlayerPrefs()
	{
		PlayerPrefs.SetInt(BuilderScanKiosk.playerPrefKey, this.targetTable.CurrentSaveSlot);
		PlayerPrefs.Save();
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x000C2AD0 File Offset: 0x000C0CD0
	private void ToggleSaveButton(bool enabled)
	{
		if (enabled)
		{
			this.saveButton.enabled = true;
			this.saveButton.buttonRenderer.material = this.saveButton.unpressedMaterial;
			return;
		}
		this.saveButton.enabled = false;
		this.saveButton.buttonRenderer.material = this.saveButton.pressedMaterial;
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x000C2B30 File Offset: 0x000C0D30
	public override void Tick()
	{
		if (this.isAnimating)
		{
			if (this.scanAnimation == null)
			{
				this.isAnimating = false;
			}
			else if ((double)Time.time > this.scanCompleteTime)
			{
				this.scanTriangle.enabled = false;
				this.isAnimating = false;
			}
		}
		if (this.coolingDown && (double)Time.time > this.coolDownCompleteTime)
		{
			this.coolingDown = false;
			this.UpdateUI();
		}
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x000C2BA0 File Offset: 0x000C0DA0
	private void OnSavePressed()
	{
		if (this.targetTable == null || !this.isDirty || this.coolingDown)
		{
			return;
		}
		BuilderScanKiosk.ScannerState scannerState = this.scannerState;
		if (scannerState == BuilderScanKiosk.ScannerState.IDLE)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.CONFIRMATION;
			this.UpdateUI();
			return;
		}
		if (scannerState != BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			return;
		}
		this.scannerState = BuilderScanKiosk.ScannerState.SAVING;
		if (this.scanAnimation != null)
		{
			this.scanCompleteTime = (double)(Time.time + this.scanAnimation.clip.length);
			this.scanTriangle.enabled = true;
			this.scanAnimation.Rewind();
			this.scanAnimation.Play();
		}
		if (this.soundBank != null)
		{
			this.soundBank.Play();
		}
		this.isAnimating = true;
		this.saveError = false;
		this.errorMsg = string.Empty;
		this.coolDownCompleteTime = (double)(Time.time + this.saveCooldownSeconds);
		this.coolingDown = true;
		this.UpdateUI();
		string busyStr;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY", out busyStr, "BUSY");
		string blocksErrStr;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS", out blocksErrStr, "PLEASE REMOVE BLOCKS CONNECTED OUTSIDE OF TABLE PLATFORM");
		this.targetTable.SaveTableForPlayer(busyStr, blocksErrStr);
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x000C2CC4 File Offset: 0x000C0EC4
	private string GetSavePath()
	{
		return string.Concat(new string[]
		{
			this.GetSaveFolder(),
			Path.DirectorySeparatorChar.ToString(),
			BuilderScanKiosk.SAVE_FILE,
			"_",
			this.targetTable.CurrentSaveSlot.ToString(),
			".png"
		});
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x000C2D20 File Offset: 0x000C0F20
	private string GetSaveFolder()
	{
		return Application.persistentDataPath + Path.DirectorySeparatorChar.ToString() + BuilderScanKiosk.SAVE_FOLDER;
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000C2D3B File Offset: 0x000C0F3B
	private void OnSaveDirtyChanged(bool dirty)
	{
		this.isDirty = dirty;
		this.UpdateUI();
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000C2D4A File Offset: 0x000C0F4A
	private void OnSaveTimeUpdated()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000C2D4A File Offset: 0x000C0F4A
	private void OnSaveSuccess()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000C2D60 File Offset: 0x000C0F60
	private void OnSaveFail(string errorMsg)
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = true;
		this.errorMsg = errorMsg;
		this.UpdateUI();
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x000C2D80 File Offset: 0x000C0F80
	private void UpdateUI()
	{
		this.screenText.text = this.GetTextForScreen();
		this.ToggleSaveButton(BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot) && !this.coolingDown);
		this.noneButton.buttonRenderer.material = ((!BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot)) ? this.noneButton.pressedMaterial : this.noneButton.unpressedMaterial);
		for (int i = 0; i < this.scanButtons.Count; i++)
		{
			this.scanButtons[i].buttonRenderer.material = ((this.targetTable.CurrentSaveSlot == i) ? this.scanButtons[i].pressedMaterial : this.scanButtons[i].unpressedMaterial);
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON", out text, "YES UPDATE SCAN");
			this.saveButton.myTmpText.text = text;
			return;
		}
		string text2;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON", out text2, "UPDATE SCAN");
		this.saveButton.myTmpText.text = text2;
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x000C2EAC File Offset: 0x000C10AC
	private string GetTextForScreen()
	{
		if (this.targetTable == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = "";
		int currentSaveSlot = this.targetTable.CurrentSaveSlot;
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT", out text, "<b><color=red>NONE</color></b>");
			stringBuilder.Append(text);
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			stringBuilder.Append("<b><color=red>");
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out text, "SCAN ");
			stringBuilder.Append(text);
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
			SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(currentSaveSlot);
			DateTime dateTime = DateTime.FromBinary(publishInfoForSlot.publishTime);
			if (dateTime > DateTime.MinValue)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL", out text, "UPDATED ");
				stringBuilder.Append(": ");
				stringBuilder.Append(text);
				stringBuilder.Append(dateTime.ToString());
				stringBuilder.Append("\n");
			}
			if (SharedBlocksManager.IsMapIDValid(publishInfoForSlot.mapID))
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL", out text, "MAP ID: ");
				stringBuilder.Append(text);
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(0, 4));
				stringBuilder.Append("-");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(4));
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS", out text, "\nUSE THIS CODE IN THE SHARE MY BLOCKS ROOM");
				stringBuilder.Append(text);
			}
		}
		stringBuilder.Append("\n");
		switch (this.scannerState)
		{
		case BuilderScanKiosk.ScannerState.IDLE:
			if (this.saveError)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR", out text, "ERROR WHILE SCANNING: ");
				stringBuilder.Append(text);
				stringBuilder.Append(this.errorMsg);
			}
			else if (this.coolingDown)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN", out text, "COOLING DOWN...");
				stringBuilder.Append(text);
			}
			else if (!this.isDirty)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES", out text, "NO UNSAVED CHANGES");
				stringBuilder.Append(text);
			}
			break;
		case BuilderScanKiosk.ScannerState.CONFIRMATION:
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE", out text, "YOU ARE ABOUT TO REPLACE ");
			if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
			{
				stringBuilder.Append(text);
				stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
			}
			else
			{
				stringBuilder.Append(text);
				stringBuilder.Append("<b><color=red>");
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out text, "SCAN ");
				stringBuilder.Append(text);
				stringBuilder.Append(currentSaveSlot + 1);
				stringBuilder.Append("</color></b>");
			}
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION", out text, " ARE YOU SURE YOU WANT TO SCAN?");
			stringBuilder.Append(text);
			break;
		case BuilderScanKiosk.ScannerState.SAVING:
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING", out text, "SCANNING BUILD...");
			stringBuilder.Append(text);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		stringBuilder.Append("\n\n\n");
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS", out text, "CREATE A <b><color=red>NEW</color></b> PRIVATE ROOM TO LOAD ");
		stringBuilder.Append(text);
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE", out text, "<b><color=red>AN EMPTY TABLE</color></b>");
			stringBuilder.Append(text);
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out text, "SCAN ");
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append(text);
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002FAE RID: 12206
	private const string MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT_KEY = "MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT";

	// Token: 0x04002FAF RID: 12207
	private const string MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL";

	// Token: 0x04002FB0 RID: 12208
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL";

	// Token: 0x04002FB1 RID: 12209
	private const string MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL";

	// Token: 0x04002FB2 RID: 12210
	private const string MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS";

	// Token: 0x04002FB3 RID: 12211
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR";

	// Token: 0x04002FB4 RID: 12212
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY";

	// Token: 0x04002FB5 RID: 12213
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS";

	// Token: 0x04002FB6 RID: 12214
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN";

	// Token: 0x04002FB7 RID: 12215
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES";

	// Token: 0x04002FB8 RID: 12216
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE";

	// Token: 0x04002FB9 RID: 12217
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION";

	// Token: 0x04002FBA RID: 12218
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING";

	// Token: 0x04002FBB RID: 12219
	private const string MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS";

	// Token: 0x04002FBC RID: 12220
	private const string MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE";

	// Token: 0x04002FBD RID: 12221
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_NONE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_NONE";

	// Token: 0x04002FBE RID: 12222
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_ONE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_ONE";

	// Token: 0x04002FBF RID: 12223
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_TWO_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_TWO";

	// Token: 0x04002FC0 RID: 12224
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_THREE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_THREE";

	// Token: 0x04002FC1 RID: 12225
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON";

	// Token: 0x04002FC2 RID: 12226
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON";

	// Token: 0x04002FC3 RID: 12227
	[SerializeField]
	private GorillaPressableButton saveButton;

	// Token: 0x04002FC4 RID: 12228
	[SerializeField]
	private GorillaPressableButton noneButton;

	// Token: 0x04002FC5 RID: 12229
	[SerializeField]
	private List<GorillaPressableButton> scanButtons;

	// Token: 0x04002FC6 RID: 12230
	[SerializeField]
	private BuilderTable targetTable;

	// Token: 0x04002FC7 RID: 12231
	[SerializeField]
	private float saveCooldownSeconds = 5f;

	// Token: 0x04002FC8 RID: 12232
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x04002FC9 RID: 12233
	[SerializeField]
	private SoundBankPlayer soundBank;

	// Token: 0x04002FCA RID: 12234
	[SerializeField]
	private Animation scanAnimation;

	// Token: 0x04002FCB RID: 12235
	private MeshRenderer scanTriangle;

	// Token: 0x04002FCC RID: 12236
	private bool isAnimating;

	// Token: 0x04002FCD RID: 12237
	private static string playerPrefKey = "BuilderSaveSlot";

	// Token: 0x04002FCE RID: 12238
	private static string SAVE_FOLDER = "MonkeBlocks";

	// Token: 0x04002FCF RID: 12239
	private static string SAVE_FILE = "MyBuild";

	// Token: 0x04002FD0 RID: 12240
	public static int NUM_SAVE_SLOTS = 3;

	// Token: 0x04002FD1 RID: 12241
	public static int DEV_SAVE_SLOT = -2;

	// Token: 0x04002FD2 RID: 12242
	private Texture2D buildCaptureTexture;

	// Token: 0x04002FD3 RID: 12243
	private bool isDirty;

	// Token: 0x04002FD4 RID: 12244
	private bool saveError;

	// Token: 0x04002FD5 RID: 12245
	private string errorMsg = string.Empty;

	// Token: 0x04002FD6 RID: 12246
	private bool coolingDown;

	// Token: 0x04002FD7 RID: 12247
	private double coolDownCompleteTime;

	// Token: 0x04002FD8 RID: 12248
	private double scanCompleteTime;

	// Token: 0x04002FD9 RID: 12249
	private BuilderScanKiosk.ScannerState scannerState;

	// Token: 0x020005A0 RID: 1440
	private enum ScannerState
	{
		// Token: 0x04002FDB RID: 12251
		IDLE,
		// Token: 0x04002FDC RID: 12252
		CONFIRMATION,
		// Token: 0x04002FDD RID: 12253
		SAVING
	}
}
