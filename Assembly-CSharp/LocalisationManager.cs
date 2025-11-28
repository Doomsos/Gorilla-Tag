using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000AE0 RID: 2784
public class LocalisationManager : MonoBehaviour
{
	// Token: 0x17000684 RID: 1668
	// (get) Token: 0x06004557 RID: 17751 RVA: 0x0016F8AC File Offset: 0x0016DAAC
	public static LocalisationManager Instance
	{
		get
		{
			return LocalisationManager._instance;
		}
	}

	// Token: 0x17000685 RID: 1669
	// (get) Token: 0x06004558 RID: 17752 RVA: 0x0016F8B3 File Offset: 0x0016DAB3
	public static bool IsReady
	{
		get
		{
			return LocalisationManager.Instance != null && LocalisationManager._localeTablePairs.Count != 0;
		}
	}

	// Token: 0x17000686 RID: 1670
	// (get) Token: 0x06004559 RID: 17753 RVA: 0x0016F8D1 File Offset: 0x0016DAD1
	public static bool LanguageSet
	{
		get
		{
			return PlayerPrefs.GetInt("has-set-language", 0) == 1;
		}
	}

	// Token: 0x17000687 RID: 1671
	// (get) Token: 0x0600455A RID: 17754 RVA: 0x0016F8E1 File Offset: 0x0016DAE1
	public static Locale CurrentLanguage
	{
		get
		{
			return LocalizationSettings.SelectedLocale;
		}
	}

	// Token: 0x17000688 RID: 1672
	// (get) Token: 0x0600455B RID: 17755 RVA: 0x0016F8E8 File Offset: 0x0016DAE8
	private static string LanugageSetPlayerPrefKey
	{
		get
		{
			return "selected-locale";
		}
	}

	// Token: 0x17000689 RID: 1673
	// (get) Token: 0x0600455C RID: 17756 RVA: 0x0016F8EF File Offset: 0x0016DAEF
	public static bool ApplicationRunning
	{
		get
		{
			return Application.isPlaying && !ApplicationQuittingState.IsQuitting;
		}
	}

	// Token: 0x0600455D RID: 17757 RVA: 0x0016F904 File Offset: 0x0016DB04
	private void Awake()
	{
		if (LocalisationManager._instance != null)
		{
			Object.DestroyImmediate(this);
			return;
		}
		LocalisationManager._instance = this;
		Object.DontDestroyOnLoad(this);
		LocalisationManager._localisationFontDict.Clear();
		for (int i = 0; i < this._localisationFonts.Count; i++)
		{
			for (int j = 0; j < this._localisationFonts[i].locales.Count; j++)
			{
				if (!(this._localisationFonts[i].locales[j] == null) && !LocalisationManager._localisationFontDict.ContainsKey(this._localisationFonts[i].locales[j].Identifier.Code) && !(this._localisationFonts[i].fontAsset == null))
				{
					this._localisationFonts[i].fontAsset == null;
					LocalisationManager._localisationFontDict.Add(this._localisationFonts[i].locales[j].Identifier.Code, this._localisationFonts[i]);
					Debug.Log("[LOCALIZATION::MANAGER] Added new Locale-Font pair to Dictionary: [" + this._localisationFonts[i].locales[j].LocaleName + "]");
				}
			}
		}
		LocalisationManager._requestCancellationSource = new CancellationTokenSource();
	}

	// Token: 0x0600455E RID: 17758 RVA: 0x0016FA7C File Offset: 0x0016DC7C
	private void Start()
	{
		LocalisationManager.<Start>d__32 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<LocalisationManager.<Start>d__32>(ref <Start>d__);
	}

	// Token: 0x0600455F RID: 17759 RVA: 0x0016FAB3 File Offset: 0x0016DCB3
	private void OnDestroy()
	{
		LocalisationManager._requestCancellationSource.Cancel();
		LocalisationManager._onLanguageChanged = null;
	}

	// Token: 0x06004560 RID: 17760 RVA: 0x0016FAC5 File Offset: 0x0016DCC5
	[RuntimeInitializeOnLoadMethod(3)]
	private static void InitialiseLocTables()
	{
		CultureInfo.CurrentCulture = new CultureInfo("en");
		LocalisationManager.CacheLocTables();
	}

	// Token: 0x06004561 RID: 17761 RVA: 0x0016FADC File Offset: 0x0016DCDC
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitialiseLanguage()
	{
		LocalisationManager._hasInitialised = false;
		string @string = PlayerPrefs.GetString(LocalisationManager.LanugageSetPlayerPrefKey, "");
		Locale locale = null;
		if (!string.IsNullOrEmpty(@string) && LocalisationManager.LanguageSet)
		{
			LocalisationManager.LoadPreviousLanguage(@string, out locale);
		}
		else
		{
			LocalisationManager.DefaultLocaleFallback(out locale);
		}
		MothershipClientApiUnity.SetLanguage(locale.Identifier.Code);
		LocalisationManager._initLocale = locale;
		LocalisationManager._hasInitialised = true;
	}

	// Token: 0x06004562 RID: 17762 RVA: 0x0016FB40 File Offset: 0x0016DD40
	private static void CacheLocTables()
	{
		LocalisationManager._localeTablePairs.Clear();
		float time = Time.time;
		foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
		{
			AsyncOperationHandle<IList<StringTable>> allTables = LocalizationSettings.StringDatabase.GetAllTables(locale);
			allTables.WaitForCompletion();
			IList<StringTable> result = allTables.Result;
			if (result.Count != 0)
			{
				int count = result.Count;
				LocalisationManager._localeTablePairs.Add(locale.Identifier.Code, result[0]);
			}
		}
	}

	// Token: 0x06004563 RID: 17763 RVA: 0x0016FBF0 File Offset: 0x0016DDF0
	public void OnLanguageButtonPressed(string langCode, bool saveLanguage)
	{
		Locale newLocale;
		if (!LocalisationManager.TryGetLocaleFromCode(langCode, out newLocale))
		{
			return;
		}
		this.TryUpdateLanguage(newLocale, saveLanguage);
	}

	// Token: 0x06004564 RID: 17764 RVA: 0x0016FC10 File Offset: 0x0016DE10
	private void ReconstructBindings()
	{
		int num = 1;
		LocalisationManager._localeDisplayBinding.Clear();
		foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
		{
			LocalisationManager._localeDisplayBinding.Add(num, locale);
			num++;
		}
	}

	// Token: 0x06004565 RID: 17765 RVA: 0x0016FC7C File Offset: 0x0016DE7C
	private static void LoadPreviousLanguage(string languageCode, out Locale result)
	{
		if (!LocalisationManager.TryGetLocaleFromCode(languageCode, out result))
		{
			LocalisationManager.DefaultLocaleFallback(out result);
			return;
		}
		PlayerPrefs.SetString(LocalisationManager.LanugageSetPlayerPrefKey, result.Identifier.Code);
		PlayerPrefs.SetInt("has-set-language", 1);
		PlayerPrefs.Save();
	}

	// Token: 0x06004566 RID: 17766 RVA: 0x0016FCC4 File Offset: 0x0016DEC4
	private static void DefaultLocaleFallback(out Locale result)
	{
		if (LocalisationManager.SysLangToLoc(Application.systemLanguage, out result))
		{
			PlayerPrefs.SetString(LocalisationManager.LanugageSetPlayerPrefKey, result.Identifier.Code);
			PlayerPrefs.SetInt("has-set-language", 1);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06004567 RID: 17767 RVA: 0x0016FD08 File Offset: 0x0016DF08
	private static bool SysLangToLoc(SystemLanguage sysLanguage, out Locale language)
	{
		if (sysLanguage <= 14)
		{
			if (sysLanguage == 10)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en");
				return language != null;
			}
			if (sysLanguage == 14)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("fr");
				return language != null;
			}
		}
		else
		{
			if (sysLanguage == 15)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("de");
				return language != null;
			}
			if (sysLanguage == 34)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("es");
				return language != null;
			}
		}
		language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en");
		language == null;
		return false;
	}

	// Token: 0x06004568 RID: 17768 RVA: 0x0016FDE8 File Offset: 0x0016DFE8
	private void TryUpdateLanguage(Locale newLocale, bool saveLanguage = true)
	{
		if (this._updateLangCoroutine != null)
		{
			base.StopCoroutine(this._updateLangCoroutine);
		}
		this._updateLangCoroutine = base.StartCoroutine(this.UpdateLanguage(newLocale, saveLanguage));
	}

	// Token: 0x06004569 RID: 17769 RVA: 0x0016FE12 File Offset: 0x0016E012
	private IEnumerator UpdateLanguage(Locale newLocale, bool saveLanguage)
	{
		if (!this._cachedHasInitialised)
		{
			yield return LocalizationSettings.InitializationOperation;
		}
		this._cachedHasInitialised = true;
		if (LocalisationManager.CurrentLanguage.Identifier.Code == newLocale.Identifier.Code)
		{
			yield break;
		}
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "language_changed";
		telemetryData.CustomTags = new string[]
		{
			LocalizationTelemetry.GameVersionCustomTag
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("starting_language", LocalisationManager.CurrentLanguage.Identifier.Code);
		dictionary.Add("new_language", newLocale.Identifier.Code);
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		MothershipClientApiUnity.SetLanguage(newLocale.Identifier.Code);
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		LocalizationSettings.SelectedLocale = newLocale;
		UnityEvent languageEvent = GameEvents.LanguageEvent;
		if (languageEvent != null)
		{
			languageEvent.Invoke();
		}
		Action onLanguageChanged = LocalisationManager._onLanguageChanged;
		if (onLanguageChanged != null)
		{
			onLanguageChanged.Invoke();
		}
		if (!saveLanguage)
		{
			yield break;
		}
		LocalisationManager.OnSaveLanguage();
		yield break;
	}

	// Token: 0x0600456A RID: 17770 RVA: 0x0016FE2F File Offset: 0x0016E02F
	public static bool TryGetLocaleFromCode(string code, out Locale result)
	{
		result = LocalizationSettings.AvailableLocales.GetLocale(code);
		return result != null;
	}

	// Token: 0x0600456B RID: 17771 RVA: 0x0016FE4B File Offset: 0x0016E04B
	public static void RegisterOnLanguageChanged(Action callback)
	{
		LocalisationManager._onLanguageChanged = (Action)Delegate.Combine(LocalisationManager._onLanguageChanged, callback);
	}

	// Token: 0x0600456C RID: 17772 RVA: 0x0016FE62 File Offset: 0x0016E062
	public static void UnregisterOnLanguageChanged(Action callback)
	{
		LocalisationManager._onLanguageChanged = (Action)Delegate.Remove(LocalisationManager._onLanguageChanged, callback);
	}

	// Token: 0x0600456D RID: 17773 RVA: 0x0016FE7C File Offset: 0x0016E07C
	public static bool GetFontAssetForCurrentLocale(out LocalisationFontPair result)
	{
		result = default(LocalisationFontPair);
		if (LocalisationManager.Instance == null)
		{
			bool applicationRunning = LocalisationManager.ApplicationRunning;
			return false;
		}
		if (!LocalisationManager._localisationFontDict.ContainsKey(LocalisationManager.CurrentLanguage.Identifier.Code))
		{
			float time = Time.time;
			return false;
		}
		result = LocalisationManager._localisationFontDict[LocalisationManager.CurrentLanguage.Identifier.Code];
		return true;
	}

	// Token: 0x0600456E RID: 17774 RVA: 0x0016FEF4 File Offset: 0x0016E0F4
	public static void OnSaveLanguage()
	{
		PlayerPrefs.SetString(LocalisationManager.LanugageSetPlayerPrefKey, LocalisationManager.CurrentLanguage.Identifier.Code);
		PlayerPrefs.SetInt("has-set-language", 1);
		PlayerPrefs.Save();
	}

	// Token: 0x0600456F RID: 17775 RVA: 0x0016FF30 File Offset: 0x0016E130
	public static bool TryGetLocaleBinding(int binding, out Locale loc)
	{
		loc = null;
		if (LocalisationManager.Instance == null)
		{
			return false;
		}
		if (LocalisationManager._localeDisplayBinding.Count != LocalizationSettings.AvailableLocales.Locales.Count)
		{
			LocalisationManager.Instance.ReconstructBindings();
		}
		return LocalisationManager._localeDisplayBinding.TryGetValue(binding, ref loc);
	}

	// Token: 0x06004570 RID: 17776 RVA: 0x0016FF80 File Offset: 0x0016E180
	public static Dictionary<int, Locale> GetAllBindings()
	{
		if (LocalisationManager._localeDisplayBinding.Count != LocalizationSettings.AvailableLocales.Locales.Count)
		{
			LocalisationManager.Instance.ReconstructBindings();
		}
		return LocalisationManager._localeDisplayBinding;
	}

	// Token: 0x06004571 RID: 17777 RVA: 0x0016FFAC File Offset: 0x0016E1AC
	public static bool TryGetKeyForCurrentLocale(string key, out string result, string defaultResult = "")
	{
		result = defaultResult;
		if (LocalisationManager._localeTablePairs.Count == 0)
		{
			return false;
		}
		StringTable stringTable;
		if (!LocalisationManager._localeTablePairs.TryGetValue(LocalisationManager.CurrentLanguage.Identifier.Code, ref stringTable))
		{
			return false;
		}
		TableEntry entry = stringTable.GetEntry(key);
		if (entry == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(entry.LocalizedValue))
		{
			result = defaultResult;
			return true;
		}
		result = entry.LocalizedValue;
		return true;
	}

	// Token: 0x06004572 RID: 17778 RVA: 0x00170014 File Offset: 0x0016E214
	public static bool TryGetKeyForEnglishString(string englishString, out string result)
	{
		result = "";
		if (LocalisationManager._localeTablePairs.Count == 0)
		{
			return false;
		}
		StringTable stringTable;
		if (!LocalisationManager._localeTablePairs.TryGetValue("en", ref stringTable))
		{
			return false;
		}
		foreach (StringTableEntry stringTableEntry in stringTable.Values)
		{
			if (!englishString.Contains(stringTableEntry.LocalizedValue))
			{
				result = stringTableEntry.LocalizedValue;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004573 RID: 17779 RVA: 0x001700A4 File Offset: 0x0016E2A4
	public static bool TryGetTranslationForCurrentLocaleWithLocString(LocalizedString key, out string result, string defaultResult = "", Object context = null)
	{
		result = defaultResult;
		key.TableReference;
		StringTable table = LocalizationSettings.StringDatabase.GetTable(key.TableReference, null);
		if (table == null)
		{
			return false;
		}
		TableEntry entryFromReference = table.GetEntryFromReference(key.TableEntryReference);
		if (entryFromReference == null)
		{
			return false;
		}
		result = entryFromReference.LocalizedValue;
		return true;
	}

	// Token: 0x06004574 RID: 17780 RVA: 0x001700F8 File Offset: 0x0016E2F8
	public static string LocaleToFriendlyString(Locale locale = null, bool forceEnglishChars = false)
	{
		if (locale == null)
		{
			locale = LocalisationManager.CurrentLanguage;
		}
		string code = locale.Identifier.Code;
		if (code == "en")
		{
			return "English";
		}
		if (code == "fr")
		{
			return "Français";
		}
		if (code == "de")
		{
			return "Deutsch";
		}
		if (code == "es")
		{
			return "Español";
		}
		if (!(code == "ja"))
		{
			return "English";
		}
		if (forceEnglishChars)
		{
			return "Nihongo";
		}
		return "日本語";
	}

	// Token: 0x06004575 RID: 17781 RVA: 0x00170194 File Offset: 0x0016E394
	public static string LocaleDisplayNameToFriendlyString(string locTextName, bool forceEnglishChar = false)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(locTextName);
		if (num > 2645429922U)
		{
			if (num <= 3560075306U)
			{
				if (num != 3159852254U)
				{
					if (num != 3560075306U)
					{
						goto IL_103;
					}
					if (!(locTextName == "JAPANESE"))
					{
						goto IL_103;
					}
					goto IL_121;
				}
				else if (!(locTextName == "ESPAÑOL"))
				{
					goto IL_103;
				}
			}
			else if (num != 3567715190U)
			{
				if (num != 3825731007U)
				{
					if (num != 4169853379U)
					{
						goto IL_103;
					}
					if (!(locTextName == "ESPANOL"))
					{
						goto IL_103;
					}
				}
				else
				{
					if (!(locTextName == "NIHONGO"))
					{
						goto IL_103;
					}
					goto IL_121;
				}
			}
			else
			{
				if (!(locTextName == "FRANÇAIS"))
				{
					goto IL_103;
				}
				goto IL_10F;
			}
			return "Español";
		}
		if (num <= 1409693518U)
		{
			if (num != 1157811451U)
			{
				if (num == 1409693518U)
				{
					if (locTextName == "日本語")
					{
						goto IL_121;
					}
				}
			}
			else if (locTextName == "ENGLISH")
			{
				return "English";
			}
		}
		else if (num != 2572742563U)
		{
			if (num == 2645429922U)
			{
				if (locTextName == "FRANCAIS")
				{
					goto IL_10F;
				}
			}
		}
		else if (locTextName == "DEUTSCH")
		{
			return "Deutsch";
		}
		IL_103:
		return "English";
		IL_10F:
		return "Français";
		IL_121:
		if (forceEnglishChar)
		{
			return "Nihongo";
		}
		return "日本語";
	}

	// Token: 0x04005762 RID: 22370
	public const string ENGLISH_IDENTIFIER = "en";

	// Token: 0x04005763 RID: 22371
	public const string FRENCH_IDENTIFIER = "fr";

	// Token: 0x04005764 RID: 22372
	public const string GERMAN_IDENTIFIER = "de";

	// Token: 0x04005765 RID: 22373
	public const string ITALIAN_IDENTIFIER = "it";

	// Token: 0x04005766 RID: 22374
	public const string SPANISH_IDENTIFIER = "es";

	// Token: 0x04005767 RID: 22375
	public const string JAPENESE_IDENTIFIER = "ja";

	// Token: 0x04005768 RID: 22376
	private static LocalisationManager _instance;

	// Token: 0x04005769 RID: 22377
	[SerializeField]
	private List<LocalisationFontPair> _localisationFonts = new List<LocalisationFontPair>();

	// Token: 0x0400576A RID: 22378
	private bool _cachedHasInitialised;

	// Token: 0x0400576B RID: 22379
	private static bool _hasInitialised = false;

	// Token: 0x0400576C RID: 22380
	private const string LANGUAGE_SET_PLAYER_PREF = "has-set-language";

	// Token: 0x0400576D RID: 22381
	private const string LOC_SYSTEM_PLAYER_PREF = "selected-locale";

	// Token: 0x0400576E RID: 22382
	private static Locale _initLocale;

	// Token: 0x0400576F RID: 22383
	private static Action _onLanguageChanged;

	// Token: 0x04005770 RID: 22384
	private Coroutine _updateLangCoroutine;

	// Token: 0x04005771 RID: 22385
	private static CancellationTokenSource _requestCancellationSource;

	// Token: 0x04005772 RID: 22386
	private static Dictionary<int, Locale> _localeDisplayBinding = new Dictionary<int, Locale>();

	// Token: 0x04005773 RID: 22387
	private static Dictionary<string, StringTable> _localeTablePairs = new Dictionary<string, StringTable>();

	// Token: 0x04005774 RID: 22388
	private static Dictionary<string, LocalisationFontPair> _localisationFontDict = new Dictionary<string, LocalisationFontPair>();
}
