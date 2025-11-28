using System;
using KID.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x02000A85 RID: 2693
public class KIDUIFeatureSetting : MonoBehaviour
{
	// Token: 0x17000664 RID: 1636
	// (get) Token: 0x060043AB RID: 17323 RVA: 0x0016722F File Offset: 0x0016542F
	// (set) Token: 0x060043AC RID: 17324 RVA: 0x00167237 File Offset: 0x00165437
	public bool AlwaysCheckFeatureSetting { get; private set; }

	// Token: 0x060043AD RID: 17325 RVA: 0x00167240 File Offset: 0x00165440
	public void CreateNewFeatureSettingGuardianManaged(KIDUI_MainScreen.FeatureToggleSetup feature, bool isEnabled)
	{
		this.CreateNewFeatureSettingWithoutToggle(feature, false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
	}

	// Token: 0x060043AE RID: 17326 RVA: 0x00167265 File Offset: 0x00165465
	public KIDUIToggle CreateNewFeatureSettingWithToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool initialState = false, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, true);
		this._featureToggle.SetValue(initialState);
		KIDUIToggle featureToggle = this._featureToggle;
		if (featureToggle != null)
		{
			featureToggle.RegisterOnChangeEvent(new Action(this.SetFeatureName));
		}
		return this._featureToggle;
	}

	// Token: 0x060043AF RID: 17327 RVA: 0x0016729F File Offset: 0x0016549F
	public void CreateNewFeatureSettingWithoutToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, false);
	}

	// Token: 0x060043B0 RID: 17328 RVA: 0x001672AC File Offset: 0x001654AC
	private void SetFeatureData(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting, bool featureToggleEnabled)
	{
		string text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.enabledText, out text, "ON", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FEATURE_SETTING] Failed to get key for  k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.enabledText), this);
		}
		this._enabledTextStr = text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.disabledText, out text, "OFF", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FEATURE_SETTING] Failed to get key for  k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.disabledText), this);
		}
		this._disabledTextStr = text;
		this._hasToggle = featureToggleEnabled;
		this._featureType = feature.linkedFeature;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.featureName, out text, feature.permissionName, null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.disabledText), this);
		}
		this._featureName = text;
		this.SetFeatureName();
		GameObject gameObject = base.gameObject;
		string name = gameObject.name;
		string text2 = "_";
		LocalizedString featureName = feature.featureName;
		gameObject.name = name + text2 + ((featureName != null) ? featureName.ToString() : null);
		this._permissionName = feature.permissionName;
		this._featureToggle.gameObject.SetActive(featureToggleEnabled);
		this.AlwaysCheckFeatureSetting = alwaysCheckFeatureSetting;
		this._feature = feature;
	}

	// Token: 0x060043B1 RID: 17329 RVA: 0x001673D4 File Offset: 0x001655D4
	public void RefreshTextOnLanguageChanged()
	{
		string text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.enabledText, out text, "ON", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.enabledText));
		}
		this._enabledTextStr = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed enabled text: " + this._enabledTextStr);
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.disabledText, out text, "OFF", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.disabledText));
		}
		this._disabledTextStr = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed disabled text: " + this._disabledTextStr);
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.featureName, out text, this._feature.permissionName, null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.disabledText));
		}
		this._featureName = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed feature name text: " + this._featureName);
		this.SetFeatureName();
	}

	// Token: 0x060043B2 RID: 17330 RVA: 0x001674DD File Offset: 0x001656DD
	public void UnregisterOnToggleChangeEvent(Action action)
	{
		this._featureToggle.UnregisterOnChangeEvent(action);
	}

	// Token: 0x060043B3 RID: 17331 RVA: 0x001674EB File Offset: 0x001656EB
	public void RegisterToggleOnEvent(Action action)
	{
		this._featureToggle.RegisterToggleOnEvent(action);
	}

	// Token: 0x060043B4 RID: 17332 RVA: 0x001674F9 File Offset: 0x001656F9
	public void UnregisterToggleOnEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOnEvent(action);
	}

	// Token: 0x060043B5 RID: 17333 RVA: 0x00167507 File Offset: 0x00165707
	public void RegisterToggleOffEvent(Action action)
	{
		this._featureToggle.RegisterToggleOffEvent(action);
	}

	// Token: 0x060043B6 RID: 17334 RVA: 0x00167515 File Offset: 0x00165715
	public void UnregisterToggleOffEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOffEvent(action);
	}

	// Token: 0x060043B7 RID: 17335 RVA: 0x00167523 File Offset: 0x00165723
	public bool GetFeatureToggleState()
	{
		if (this._hasToggle)
		{
			return this._featureToggle.IsOn;
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._featureType);
		if (permissionDataByFeature.ManagedBy != 2)
		{
			Debug.LogError("[KID::FeatureSetting] GetToggleState: feature has no toggle AND is not managed by Guardian");
		}
		return permissionDataByFeature.Enabled;
	}

	// Token: 0x060043B8 RID: 17336 RVA: 0x0016755C File Offset: 0x0016575C
	public bool GetHasToggle()
	{
		return this._hasToggle;
	}

	// Token: 0x060043B9 RID: 17337 RVA: 0x00167564 File Offset: 0x00165764
	public void SetFeatureSettingVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	// Token: 0x060043BA RID: 17338 RVA: 0x00167572 File Offset: 0x00165772
	public void SetFeatureToggle(bool enableToggle)
	{
		this._featureToggle.interactable = enableToggle;
	}

	// Token: 0x060043BB RID: 17339 RVA: 0x00167580 File Offset: 0x00165780
	public void SetGuardianManagedState(bool isEnabled)
	{
		this._featureToggle.gameObject.SetActive(false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
		this.SetupGuardianManagedClickHandlers();
		this.SetFeatureName();
	}

	// Token: 0x060043BC RID: 17340 RVA: 0x001675BC File Offset: 0x001657BC
	public void SetPlayerManagedState(bool isInteractable, bool isOptedIn)
	{
		this._featureToggle.gameObject.SetActive(true);
		this._guardianManagedEnabled.SetActive(false);
		this._guardianManagedLocked.SetActive(false);
		this._featureToggle.interactable = isInteractable;
		this._featureToggle.SetValue(isOptedIn);
	}

	// Token: 0x060043BD RID: 17341 RVA: 0x0016760C File Offset: 0x0016580C
	private void SetFeatureName()
	{
		string text = this.GetFeatureToggleState() ? ("<b>(" + this._enabledTextStr + ")</b>") : ("<b>(" + this._disabledTextStr + ")</b>");
		this._featureNameTxt.text = "<b>" + this._featureName + "</b>";
		this._featureStatusTxt.text = (text ?? "");
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x00167683 File Offset: 0x00165883
	private void SetupGuardianManagedClickHandlers()
	{
		this.AddDeniedSoundHandler(this._guardianManagedEnabled);
		this.AddDeniedSoundHandler(this._guardianManagedLocked);
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x001676A0 File Offset: 0x001658A0
	private void AddDeniedSoundHandler(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		EventTrigger component = obj.GetComponent<EventTrigger>();
		if (component != null)
		{
			Object.DestroyImmediate(component);
		}
		EventTrigger eventTrigger = obj.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = 2;
		entry.callback.AddListener(delegate(BaseEventData data)
		{
			Debug.Log("[KIDUIFeatureSetting] Guardian-managed feature clicked - playing denied sound");
			KIDAudioManager instance = KIDAudioManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.PlaySound(KIDAudioManager.KIDSoundType.Denied);
		});
		eventTrigger.triggers.Add(entry);
		this.EnsureRaycastTarget(obj);
	}

	// Token: 0x060043C0 RID: 17344 RVA: 0x0016771C File Offset: 0x0016591C
	private void EnsureRaycastTarget(GameObject obj)
	{
		Graphic component = obj.GetComponent<Graphic>();
		if (component != null)
		{
			component.raycastTarget = true;
			return;
		}
		Image image = obj.GetComponent<Image>();
		if (image == null)
		{
			image = obj.AddComponent<Image>();
		}
		image.color = new Color(0f, 0f, 0f, 0f);
		image.raycastTarget = true;
	}

	// Token: 0x04005541 RID: 21825
	[SerializeField]
	private TMP_Text _featureNameTxt;

	// Token: 0x04005542 RID: 21826
	[SerializeField]
	private TMP_Text _featureStatusTxt;

	// Token: 0x04005543 RID: 21827
	[SerializeField]
	private KIDUIToggle _featureToggle;

	// Token: 0x04005544 RID: 21828
	[SerializeField]
	private GameObject _tickIcon;

	// Token: 0x04005545 RID: 21829
	[SerializeField]
	private GameObject _crossIcon;

	// Token: 0x04005546 RID: 21830
	[SerializeField]
	private GameObject _guardianManagedLocked;

	// Token: 0x04005547 RID: 21831
	[SerializeField]
	private GameObject _guardianManagedEnabled;

	// Token: 0x04005548 RID: 21832
	private bool _hasToggle;

	// Token: 0x04005549 RID: 21833
	private string _featureName;

	// Token: 0x0400554A RID: 21834
	private string _permissionName;

	// Token: 0x0400554B RID: 21835
	private string _enabledTextStr;

	// Token: 0x0400554C RID: 21836
	private string _disabledTextStr;

	// Token: 0x0400554D RID: 21837
	private EKIDFeatures _featureType;

	// Token: 0x0400554E RID: 21838
	private Action<EKIDFeatures> _onChangeCallback;

	// Token: 0x0400554F RID: 21839
	private KIDUI_MainScreen.FeatureToggleSetup _feature;
}
