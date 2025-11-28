using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000A8B RID: 2699
public class KIDUIToggle : Slider
{
	// Token: 0x17000667 RID: 1639
	// (get) Token: 0x060043DB RID: 17371 RVA: 0x00167CAD File Offset: 0x00165EAD
	// (set) Token: 0x060043DC RID: 17372 RVA: 0x00167CB5 File Offset: 0x00165EB5
	public bool CurrentValue { get; private set; }

	// Token: 0x17000668 RID: 1640
	// (get) Token: 0x060043DD RID: 17373 RVA: 0x00167CBE File Offset: 0x00165EBE
	public bool IsOn
	{
		get
		{
			return this.CurrentValue;
		}
	}

	// Token: 0x060043DE RID: 17374 RVA: 0x00167CC6 File Offset: 0x00165EC6
	protected override void Awake()
	{
		base.Awake();
		this.SetupToggleComponent();
	}

	// Token: 0x060043DF RID: 17375 RVA: 0x00167CD4 File Offset: 0x00165ED4
	protected override void Start()
	{
		base.Start();
		base.interactable = false;
	}

	// Token: 0x060043E0 RID: 17376 RVA: 0x00167CE3 File Offset: 0x00165EE3
	protected override void OnEnable()
	{
		base.OnEnable();
		base.interactable = false;
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060043E1 RID: 17377 RVA: 0x00167D14 File Offset: 0x00165F14
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.Toggle();
	}

	// Token: 0x060043E2 RID: 17378 RVA: 0x00167D23 File Offset: 0x00165F23
	public override void OnPointerEnter(PointerEventData pointerEventData)
	{
		this.SetHighlighted();
		this.inside = true;
	}

	// Token: 0x060043E3 RID: 17379 RVA: 0x00167D32 File Offset: 0x00165F32
	public override void OnPointerExit(PointerEventData pointerEventData)
	{
		this.SetNormal();
		this.inside = false;
	}

	// Token: 0x060043E4 RID: 17380 RVA: 0x00167D44 File Offset: 0x00165F44
	protected virtual void SetupToggleComponent()
	{
		this.SetupSliderComponent();
		base.handleRect.anchorMin = new Vector2(0f, 0.5f);
		base.handleRect.anchorMax = new Vector3(0f, 0.5f);
		base.handleRect.pivot = new Vector2(0f, 0.5f);
		base.handleRect.sizeDelta = new Vector2(base.handleRect.sizeDelta.x, base.handleRect.sizeDelta.x);
	}

	// Token: 0x060043E5 RID: 17381 RVA: 0x00167DDC File Offset: 0x00165FDC
	protected virtual void SetupSliderComponent()
	{
		base.interactable = false;
		base.colors.disabledColor = Color.white;
		this.SetColors();
		base.transition = 0;
	}

	// Token: 0x060043E6 RID: 17382 RVA: 0x00167E10 File Offset: 0x00166010
	public void RegisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.AddListener(delegate()
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2.Invoke();
		});
	}

	// Token: 0x060043E7 RID: 17383 RVA: 0x00167E44 File Offset: 0x00166044
	public void UnregisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.RemoveListener(delegate()
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2.Invoke();
		});
	}

	// Token: 0x060043E8 RID: 17384 RVA: 0x00167E78 File Offset: 0x00166078
	public void RegisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.AddListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2.Invoke();
		});
	}

	// Token: 0x060043E9 RID: 17385 RVA: 0x00167EAC File Offset: 0x001660AC
	public void UnregisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.RemoveListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2.Invoke();
		});
	}

	// Token: 0x060043EA RID: 17386 RVA: 0x00167EE0 File Offset: 0x001660E0
	public void RegisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.AddListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2.Invoke();
		});
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x00167F14 File Offset: 0x00166114
	public void UnregisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.RemoveListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2.Invoke();
		});
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x00167F45 File Offset: 0x00166145
	private void SetColors()
	{
		base.colors = this._fillColors;
	}

	// Token: 0x060043ED RID: 17389 RVA: 0x00167F53 File Offset: 0x00166153
	private void Toggle()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetStateAndStartAnimation(!this.CurrentValue, false);
	}

	// Token: 0x060043EE RID: 17390 RVA: 0x00167F6E File Offset: 0x0016616E
	public void SetValue(bool newValue)
	{
		if (newValue == this.CurrentValue)
		{
			return;
		}
		this.SetStateAndStartAnimation(newValue, false);
	}

	// Token: 0x060043EF RID: 17391 RVA: 0x00167F84 File Offset: 0x00166184
	private void SetStateAndStartAnimation(bool state, bool skipAnim = false)
	{
		if (this.CurrentValue == state)
		{
			Debug.Log("IS SAME STATE, WILL NOT CHANGE");
			return;
		}
		this.CurrentValue = state;
		UnityEvent onToggleChanged = this._onToggleChanged;
		if (onToggleChanged != null)
		{
			onToggleChanged.Invoke();
		}
		if (this.CurrentValue)
		{
			UnityEvent onToggleOn = this._onToggleOn;
			if (onToggleOn != null)
			{
				onToggleOn.Invoke();
			}
			KIDAudioManager.Instance.PlaySound(KIDAudioManager.KIDSoundType.Success);
		}
		else
		{
			UnityEvent onToggleOff = this._onToggleOff;
			if (onToggleOff != null)
			{
				onToggleOff.Invoke();
			}
			KIDAudioManager.Instance.PlaySound(KIDAudioManager.KIDSoundType.TurnOffPermission);
		}
		if (this._animationCoroutine != null)
		{
			base.StopCoroutine(this._animationCoroutine);
		}
		this._handleUnlockIcon.gameObject.SetActive(this.CurrentValue);
		this._handleLockIcon.gameObject.SetActive(!this.CurrentValue);
		if (this._animationDuration == 0f || skipAnim)
		{
			Debug.Log("[KID::UI::SetStateAndStartAnimation] Skipping animation. Setting value to " + (this.CurrentValue ? "1f" : "0f"));
			this.value = (this.CurrentValue ? 1f : 0f);
			return;
		}
		this._animationCoroutine = base.StartCoroutine(this.AnimateSlider());
	}

	// Token: 0x060043F0 RID: 17392 RVA: 0x001680A3 File Offset: 0x001662A3
	private IEnumerator AnimateSlider()
	{
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] is {1}", base.name, this.CurrentValue));
		float startValue = this.CurrentValue ? 0f : 1f;
		float endValue = this.CurrentValue ? 1f : 0f;
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] Start: {1}, End: {2}, Value: {3}", new object[]
		{
			base.name,
			startValue,
			endValue,
			this.value
		}));
		float time = 0f;
		while (time < this._animationDuration)
		{
			time += Time.deltaTime;
			float num = this._toggleEase.Evaluate(time / this._animationDuration);
			this.value = Mathf.Lerp(startValue, endValue, num);
			yield return null;
		}
		this.value = endValue;
		yield break;
	}

	// Token: 0x060043F1 RID: 17393 RVA: 0x001680B4 File Offset: 0x001662B4
	private void PostUpdate()
	{
		if (!this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && KIDUIToggle._canTrigger)
			{
				string text = string.Concat(new string[]
				{
					"[",
					base.transform.parent.parent.parent.name,
					".",
					base.transform.parent.parent.name,
					".",
					base.transform.parent.name,
					".",
					base.transform.name,
					"]"
				});
				Debug.Log(string.Concat(new string[]
				{
					"[KID::UIBUTTON::DEBUG] ",
					text,
					" - STEAM - OnClick is pressed. Time: [",
					Time.time.ToString(),
					"]"
				}), this);
				this.Toggle();
				KIDUIToggle._triggeredThisFrame = true;
				KIDUIToggle._canTrigger = false;
				return;
			}
			if (!ControllerBehaviour.Instance.TriggerDown)
			{
				KIDUIToggle._canTrigger = true;
			}
		}
	}

	// Token: 0x060043F2 RID: 17394 RVA: 0x001681E0 File Offset: 0x001663E0
	private void LateUpdate()
	{
		if (KIDUIToggle._triggeredThisFrame)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnLateUpdate triggered and Triggered Frame Reset. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
		}
		KIDUIToggle._triggeredThisFrame = false;
	}

	// Token: 0x060043F3 RID: 17395 RVA: 0x001682C5 File Offset: 0x001664C5
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x060043F4 RID: 17396 RVA: 0x001682F0 File Offset: 0x001664F0
	private void SetDisabled(bool isLockedButEnabled)
	{
		this.SetSwitchColors(this._borderColors.disabledColor, this._handleColors.disabledColor, this._fillColors.disabledColor);
		this.SetBorderSize(this._disabledBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x060043F5 RID: 17397 RVA: 0x0016832C File Offset: 0x0016652C
	private void SetNormal()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.normalColor, this._handleColors.normalColor, this._fillColors.normalColor);
		this.SetBorderSize(this._normalBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x060043F6 RID: 17398 RVA: 0x0016837C File Offset: 0x0016657C
	private void SetSelected()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.selectedColor, this._handleColors.selectedColor, this._fillColors.selectedColor);
		this.SetBorderSize(this._selectedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x060043F7 RID: 17399 RVA: 0x001683CC File Offset: 0x001665CC
	private void SetHighlighted()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.highlightedColor, this._handleColors.highlightedColor, this._fillColors.highlightedColor);
		this.SetBorderSize(this._highlightedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x060043F8 RID: 17400 RVA: 0x0016841C File Offset: 0x0016661C
	private void SetPressed()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.pressedColor, this._handleColors.pressedColor, this._fillColors.pressedColor);
		this.SetBorderSize(this._pressedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x060043F9 RID: 17401 RVA: 0x0016846C File Offset: 0x0016666C
	private void SetSwitchColors(Color borderColor, Color handleColor, Color fillColor)
	{
		this._borderImg.color = borderColor;
		this._handleImg.color = handleColor;
	}

	// Token: 0x060043FA RID: 17402 RVA: 0x00168486 File Offset: 0x00166686
	private void SetBorderSize(float borderScale)
	{
		this._borderImgRef.offsetMin = new Vector2(-borderScale, -borderScale * this._borderHeightRatio);
		this._borderImgRef.offsetMax = new Vector2(borderScale, borderScale * this._borderHeightRatio);
	}

	// Token: 0x060043FB RID: 17403 RVA: 0x001684BC File Offset: 0x001666BC
	private void SetBackgroundActive(bool isActive)
	{
		this._fillImg.gameObject.SetActive(isActive);
		this._fillInactiveImg.gameObject.SetActive(!isActive);
		this.SetBackgroundLocksActive(isActive);
	}

	// Token: 0x060043FC RID: 17404 RVA: 0x001684EC File Offset: 0x001666EC
	private void SetBackgroundLocksActive(bool isActive)
	{
		Color color = isActive ? this._lockActiveColor : this._lockInactiveColor;
		this._lockIcon.color = color;
		this._unlockIcon.color = color;
	}

	// Token: 0x04005560 RID: 21856
	[Header("Toggle Setup")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _initValue;

	// Token: 0x04005561 RID: 21857
	[SerializeField]
	private Image _borderImg;

	// Token: 0x04005562 RID: 21858
	[SerializeField]
	private float _borderHeightRatio = 2f;

	// Token: 0x04005563 RID: 21859
	[SerializeField]
	private Image _fillImg;

	// Token: 0x04005564 RID: 21860
	[SerializeField]
	private Image _fillInactiveImg;

	// Token: 0x04005565 RID: 21861
	[SerializeField]
	private Image _handleImg;

	// Token: 0x04005566 RID: 21862
	[SerializeField]
	private Image _lockIcon;

	// Token: 0x04005567 RID: 21863
	[SerializeField]
	private Image _unlockIcon;

	// Token: 0x04005568 RID: 21864
	[SerializeField]
	private Image _handleLockIcon;

	// Token: 0x04005569 RID: 21865
	[SerializeField]
	private Image _handleUnlockIcon;

	// Token: 0x0400556A RID: 21866
	[SerializeField]
	private Color _lockActiveColor;

	// Token: 0x0400556B RID: 21867
	[SerializeField]
	private Color _lockInactiveColor;

	// Token: 0x0400556C RID: 21868
	[SerializeField]
	private RectTransform _borderImgRef;

	// Token: 0x0400556D RID: 21869
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x0400556E RID: 21870
	[Header("Animation")]
	[SerializeField]
	private float _animationDuration = 0.15f;

	// Token: 0x0400556F RID: 21871
	[SerializeField]
	private AnimationCurve _toggleEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005570 RID: 21872
	[Header("Fill Colors")]
	[SerializeField]
	private ColorBlock _fillColors;

	// Token: 0x04005571 RID: 21873
	[Header("Border Colors")]
	[SerializeField]
	private ColorBlock _borderColors;

	// Token: 0x04005572 RID: 21874
	[Header("Borders")]
	[SerializeField]
	private float _normalBorderSize = 1f;

	// Token: 0x04005573 RID: 21875
	[SerializeField]
	private float _disabledBorderSize = 1f;

	// Token: 0x04005574 RID: 21876
	[SerializeField]
	private float _highlightedBorderSize = 1f;

	// Token: 0x04005575 RID: 21877
	[SerializeField]
	private float _pressedBorderSize = 1f;

	// Token: 0x04005576 RID: 21878
	[SerializeField]
	private float _selectedBorderSize = 1f;

	// Token: 0x04005577 RID: 21879
	[Header("Handle Colors")]
	[SerializeField]
	private ColorBlock _handleColors;

	// Token: 0x04005578 RID: 21880
	[Header("Events")]
	[SerializeField]
	private UnityEvent _onToggleOn;

	// Token: 0x04005579 RID: 21881
	[SerializeField]
	private UnityEvent _onToggleOff;

	// Token: 0x0400557A RID: 21882
	[SerializeField]
	private UnityEvent _onToggleChanged;

	// Token: 0x0400557B RID: 21883
	private bool _previousValue;

	// Token: 0x0400557C RID: 21884
	private bool _isDisabled;

	// Token: 0x0400557D RID: 21885
	private Coroutine _animationCoroutine;

	// Token: 0x0400557F RID: 21887
	private bool inside;

	// Token: 0x04005580 RID: 21888
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005581 RID: 21889
	private static bool _canTrigger = true;
}
