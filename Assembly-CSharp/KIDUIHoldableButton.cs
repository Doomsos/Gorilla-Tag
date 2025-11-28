using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000A87 RID: 2695
public class KIDUIHoldableButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x17000665 RID: 1637
	// (get) Token: 0x060043C5 RID: 17349 RVA: 0x00167786 File Offset: 0x00165986
	// (set) Token: 0x060043C6 RID: 17350 RVA: 0x0016778E File Offset: 0x0016598E
	public KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x17000666 RID: 1638
	// (get) Token: 0x060043C7 RID: 17351 RVA: 0x00167797 File Offset: 0x00165997
	public float HoldPercentage
	{
		get
		{
			return this._elapsedTime / this._holdDuration;
		}
	}

	// Token: 0x060043C8 RID: 17352 RVA: 0x001677A8 File Offset: 0x001659A8
	private void OnEnable()
	{
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060043C9 RID: 17353 RVA: 0x001677FB File Offset: 0x001659FB
	private void Update()
	{
		this.ManageButtonInteraction(false);
	}

	// Token: 0x060043CA RID: 17354 RVA: 0x00167804 File Offset: 0x00165A04
	public void OnPointerDown(PointerEventData eventData)
	{
		this._isHoldingMouse = true;
		this.ToggleHoldingButton(true);
	}

	// Token: 0x060043CB RID: 17355 RVA: 0x00167814 File Offset: 0x00165A14
	public void OnPointerUp(PointerEventData eventData)
	{
		this._isHoldingMouse = false;
		this.ManageButtonInteraction(true);
		this.ToggleHoldingButton(false);
	}

	// Token: 0x060043CC RID: 17356 RVA: 0x0016782C File Offset: 0x00165A2C
	private void ToggleHoldingButton(bool isPointerDown)
	{
		this._isHoldingButton = (isPointerDown && this._button.interactable);
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (isPointerDown)
		{
			this._elapsedTime = 0f;
			KIDUIHoldableButton.ButtonHoldStartEvent onHoldStart = this.m_OnHoldStart;
			if (onHoldStart != null)
			{
				onHoldStart.Invoke();
			}
			KIDAudioManager.Instance.StartButtonHeldSound();
			return;
		}
		KIDUIHoldableButton.ButtonHoldReleaseEvent onHoldRelease = this.m_OnHoldRelease;
		if (onHoldRelease != null)
		{
			onHoldRelease.Invoke();
		}
		KIDAudioManager.Instance.StopButtonHeldSound();
	}

	// Token: 0x060043CD RID: 17357 RVA: 0x001678BC File Offset: 0x00165ABC
	private void ManageButtonInteraction(bool isPointerUp = false)
	{
		if (!this._isHoldingButton)
		{
			return;
		}
		if (isPointerUp)
		{
			return;
		}
		if (this._holdDuration <= 0f)
		{
			this.HoldComplete();
			return;
		}
		this._elapsedTime += Time.deltaTime;
		bool flag = this._elapsedTime > this._holdDuration;
		float num = this._elapsedTime / this._holdDuration;
		this._holdProgressFill.rectTransform.localScale = new Vector3(num, 1f, 1f);
		HandRayController.Instance.PulseActiveHandray(num, 0.1f);
		if (flag)
		{
			this.HoldComplete();
		}
	}

	// Token: 0x060043CE RID: 17358 RVA: 0x00167950 File Offset: 0x00165B50
	private void HoldComplete()
	{
		this.ToggleHoldingButton(false);
		KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete = this.m_OnHoldComplete;
		if (onHoldComplete != null)
		{
			onHoldComplete.Invoke();
		}
		Debug.Log("[HOLD_BUTTON " + base.name + " ]: Hold Complete");
		this.ResetButton();
	}

	// Token: 0x060043CF RID: 17359 RVA: 0x0016798A File Offset: 0x00165B8A
	private void ResetButton()
	{
		this._elapsedTime = 0f;
		this.inside = false;
		KIDUIHoldableButton._triggeredThisFrame = false;
		this._button.ResetButton();
	}

	// Token: 0x060043D0 RID: 17360 RVA: 0x001679AF File Offset: 0x00165BAF
	protected void Awake()
	{
		if (this._button != null)
		{
			return;
		}
		this._button = base.GetComponentInChildren<KIDUIButton>();
		if (this._button == null)
		{
			Debug.LogError("[KID::UI_BUTTON] Could not find [KIDUIButton] in children, trying to create a new one.");
			return;
		}
	}

	// Token: 0x060043D1 RID: 17361 RVA: 0x001679E8 File Offset: 0x00165BE8
	private void PostUpdate()
	{
		if (!KIDUIHoldableButton._canTrigger)
		{
			KIDUIHoldableButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!this._button.interactable || !KIDUIHoldableButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && this.inside)
			{
				if (!this._isHoldingButton)
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
					this.ToggleHoldingButton(true);
					KIDUIHoldableButton._triggeredThisFrame = true;
					KIDUIHoldableButton._canTrigger = false;
					return;
				}
			}
			else if (this._isHoldingButton && !this._isHoldingMouse)
			{
				this.ToggleHoldingButton(false);
			}
		}
	}

	// Token: 0x060043D2 RID: 17362 RVA: 0x00167B4C File Offset: 0x00165D4C
	private void LateUpdate()
	{
		if (KIDUIHoldableButton._triggeredThisFrame)
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
		KIDUIHoldableButton._triggeredThisFrame = false;
	}

	// Token: 0x060043D3 RID: 17363 RVA: 0x00167C31 File Offset: 0x00165E31
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
	}

	// Token: 0x060043D4 RID: 17364 RVA: 0x00167C3A File Offset: 0x00165E3A
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x060043D5 RID: 17365 RVA: 0x00167C43 File Offset: 0x00165E43
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x04005553 RID: 21843
	public KIDUIButton _button;

	// Token: 0x04005554 RID: 21844
	[SerializeField]
	private float _holdDuration;

	// Token: 0x04005555 RID: 21845
	[SerializeField]
	private Image _holdProgressFill;

	// Token: 0x04005556 RID: 21846
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005557 RID: 21847
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldCompleteEvent m_OnHoldComplete = new KIDUIHoldableButton.ButtonHoldCompleteEvent();

	// Token: 0x04005558 RID: 21848
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldStartEvent m_OnHoldStart = new KIDUIHoldableButton.ButtonHoldStartEvent();

	// Token: 0x04005559 RID: 21849
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldReleaseEvent m_OnHoldRelease = new KIDUIHoldableButton.ButtonHoldReleaseEvent();

	// Token: 0x0400555A RID: 21850
	private bool _isHoldingButton;

	// Token: 0x0400555B RID: 21851
	private float _elapsedTime;

	// Token: 0x0400555C RID: 21852
	private bool inside;

	// Token: 0x0400555D RID: 21853
	private bool _isHoldingMouse;

	// Token: 0x0400555E RID: 21854
	private static bool _triggeredThisFrame = false;

	// Token: 0x0400555F RID: 21855
	private static bool _canTrigger = true;

	// Token: 0x02000A88 RID: 2696
	[Serializable]
	public class ButtonHoldCompleteEvent : UnityEvent
	{
	}

	// Token: 0x02000A89 RID: 2697
	[Serializable]
	public class ButtonHoldStartEvent : UnityEvent
	{
	}

	// Token: 0x02000A8A RID: 2698
	[Serializable]
	public class ButtonHoldReleaseEvent : UnityEvent
	{
	}
}
