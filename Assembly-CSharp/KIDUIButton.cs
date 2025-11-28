using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x02000A84 RID: 2692
public class KIDUIButton : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000663 RID: 1635
	// (get) Token: 0x06004399 RID: 17305 RVA: 0x00075546 File Offset: 0x00073746
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x0600439A RID: 17306 RVA: 0x00166CC2 File Offset: 0x00164EC2
	protected override void OnEnable()
	{
		base.OnEnable();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600439B RID: 17307 RVA: 0x00166CEC File Offset: 0x00164EEC
	private void PostUpdate()
	{
		if (!KIDUIButton._canTrigger)
		{
			KIDUIButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!base.interactable || !this.inside || !KIDUIButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown && !KIDUIButton._triggeredThisFrame)
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
			Button.ButtonClickedEvent onClick = base.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
			KIDUIButton._triggeredThisFrame = true;
			KIDUIButton._canTrigger = false;
		}
	}

	// Token: 0x0600439C RID: 17308 RVA: 0x00166E38 File Offset: 0x00165038
	private void LateUpdate()
	{
		if (KIDUIButton._triggeredThisFrame)
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
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x00166F1D File Offset: 0x0016511D
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.inside = false;
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x00166F2D File Offset: 0x0016512D
	public void ResetButton()
	{
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x0600439F RID: 17311 RVA: 0x00166F3C File Offset: 0x0016513C
	protected override void OnDisable()
	{
		this.FixStuckPressedState();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x060043A0 RID: 17312 RVA: 0x00166F66 File Offset: 0x00165166
	private void FixStuckPressedState()
	{
		this.InstantClearState();
		this._buttonText.color = (base.interactable ? this._normalTextColor : this._disabledTextColor);
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x060043A1 RID: 17313 RVA: 0x00166F9C File Offset: 0x0016519C
	protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		switch (state)
		{
		default:
			this._buttonText.color = this._normalTextColor;
			this.SetIcons(true, false);
			return;
		case 1:
			this._buttonText.color = this._highlightedTextColor;
			this.SetIcons(false, true);
			return;
		case 2:
			this._buttonText.color = this._pressedTextColor;
			this.SetIcons(true, false);
			return;
		case 3:
			this._buttonText.color = this._selectedTextColor;
			this.SetIcons(true, false);
			return;
		case 4:
			this._buttonText.color = this._disabledTextColor;
			this.SetIcons(true, false);
			return;
		}
	}

	// Token: 0x060043A2 RID: 17314 RVA: 0x0016704C File Offset: 0x0016524C
	private void SetIcons(bool normalEnabled, bool highlightedEnabled)
	{
		if (this._normalIcon == null || this._highlightedIcon == null)
		{
			return;
		}
		GameObject normalIcon = this._normalIcon;
		if (normalIcon != null)
		{
			normalIcon.SetActive(normalEnabled);
		}
		GameObject highlightedIcon = this._highlightedIcon;
		if (highlightedIcon == null)
		{
			return;
		}
		highlightedIcon.SetActive(highlightedEnabled);
	}

	// Token: 0x060043A3 RID: 17315 RVA: 0x0016709C File Offset: 0x0016529C
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.inside = true;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySound(KIDAudioManager.KIDSoundType.Hover);
		}
		Debug.Log("[KID::UIBUTTON::KIDAudioManager] Hover played");
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x060043A4 RID: 17316 RVA: 0x0016711C File Offset: 0x0016531C
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.inside = false;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySound(this.onClickSound);
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._pressedVibrationStrength, this._pressedVibrationDuration);
	}

	// Token: 0x060043A5 RID: 17317 RVA: 0x00167196 File Offset: 0x00165396
	public void SetText(string text)
	{
		this._buttonText.SetText(text);
	}

	// Token: 0x060043A6 RID: 17318 RVA: 0x001671A4 File Offset: 0x001653A4
	public void SetFont(TMP_FontAsset font)
	{
		this._buttonText.font = font;
	}

	// Token: 0x060043A7 RID: 17319 RVA: 0x001671B2 File Offset: 0x001653B2
	public string GetText()
	{
		return this._buttonText.text;
	}

	// Token: 0x060043A8 RID: 17320 RVA: 0x001671BF File Offset: 0x001653BF
	public void SetBorderImage(Sprite newImg)
	{
		this._borderImage.sprite = newImg;
	}

	// Token: 0x04005524 RID: 21796
	[SerializeField]
	private Image _borderImage;

	// Token: 0x04005525 RID: 21797
	[SerializeField]
	private RectTransform _fillImageRef;

	// Token: 0x04005526 RID: 21798
	[SerializeField]
	private TMP_Text _buttonText;

	// Token: 0x04005527 RID: 21799
	[Header("Transition States")]
	[Header("Normal")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalBorderColor;

	// Token: 0x04005528 RID: 21800
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalTextColor;

	// Token: 0x04005529 RID: 21801
	[SerializeField]
	private float _normalBorderSize;

	// Token: 0x0400552A RID: 21802
	[Header("Highlighted")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedBorderColor;

	// Token: 0x0400552B RID: 21803
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedTextColor;

	// Token: 0x0400552C RID: 21804
	[SerializeField]
	private float _highlightedBorderSize;

	// Token: 0x0400552D RID: 21805
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x0400552E RID: 21806
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x0400552F RID: 21807
	[Header("Pressed")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedBorderColor;

	// Token: 0x04005530 RID: 21808
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedTextColor;

	// Token: 0x04005531 RID: 21809
	[SerializeField]
	private float _pressedBorderSize;

	// Token: 0x04005532 RID: 21810
	[SerializeField]
	private float _pressedVibrationStrength = 0.5f;

	// Token: 0x04005533 RID: 21811
	[SerializeField]
	private float _pressedVibrationDuration = 0.1f;

	// Token: 0x04005534 RID: 21812
	[Header("Selected")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedBorderColor;

	// Token: 0x04005535 RID: 21813
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedTextColor;

	// Token: 0x04005536 RID: 21814
	[SerializeField]
	private float _selectedBorderSize;

	// Token: 0x04005537 RID: 21815
	[Header("Disabled")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledBorderColor;

	// Token: 0x04005538 RID: 21816
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledTextColor;

	// Token: 0x04005539 RID: 21817
	[SerializeField]
	private float _disabledBorderSize;

	// Token: 0x0400553A RID: 21818
	[Header("Audio")]
	[SerializeField]
	private KIDAudioManager.KIDSoundType onClickSound;

	// Token: 0x0400553B RID: 21819
	[Header("Icon Swap Settings")]
	[SerializeField]
	private GameObject _normalIcon;

	// Token: 0x0400553C RID: 21820
	[SerializeField]
	private GameObject _highlightedIcon;

	// Token: 0x0400553D RID: 21821
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x0400553E RID: 21822
	private bool inside;

	// Token: 0x0400553F RID: 21823
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005540 RID: 21824
	private static bool _canTrigger = true;
}
