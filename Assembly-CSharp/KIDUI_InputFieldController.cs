using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Valve.VR;

// Token: 0x02000AAF RID: 2735
public class KIDUI_InputFieldController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000673 RID: 1651
	// (get) Token: 0x0600448B RID: 17547 RVA: 0x00075546 File Offset: 0x00073746
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x0600448C RID: 17548 RVA: 0x0016AE14 File Offset: 0x00169014
	protected void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
		SteamVR_Events.System(1200).Listen(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(1201).Listen(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x0600448D RID: 17549 RVA: 0x0016AE7C File Offset: 0x0016907C
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		SteamVR_Events.System(1200).Remove(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(1201).Remove(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x0600448E RID: 17550 RVA: 0x0016AEE4 File Offset: 0x001690E4
	private void Update()
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		SteamVR.instance.overlay.GetKeyboardText(this._inputStringBuilder, 1024U);
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] String BUilder Says: [" + this._inputStringBuilder.ToString() + "]");
		this._inputField.text = this._inputBuffer;
		this._inputField.stringPosition = this._inputBuffer.Length;
	}

	// Token: 0x0600448F RID: 17551 RVA: 0x0016AF5C File Offset: 0x0016915C
	private void PostUpdate()
	{
		if (!this._inputField.interactable || !this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown)
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
			this.OnClickedInputField("");
		}
	}

	// Token: 0x06004490 RID: 17552 RVA: 0x0016B070 File Offset: 0x00169270
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
		if (!this._inputField.IsInteractable() || !this._inputField.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06004491 RID: 17553 RVA: 0x0016B0D7 File Offset: 0x001692D7
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06004492 RID: 17554 RVA: 0x0016B0E0 File Offset: 0x001692E0
	private void OnClickedInputField(string _ = "")
	{
		if (this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUT_FIELD_CONTROLLER] Selecting and Activating Input Field");
		EVROverlayError evroverlayError = OpenVR.Overlay.ShowKeyboard(0, 0, 1U, "Enter Email", 1024U, this._inputField.text ?? "", 0UL);
		if (evroverlayError != null)
		{
			Debug.LogError("[KID::INPUT_FIELD_CONTROLLER] Failed to open keyboard. Resulted with error: [" + evroverlayError.ToString() + "]");
			return;
		}
		this._inputBuffer = (this._inputField.text ?? "");
		this.keyboardShowing = true;
		HandRayController.Instance.DisableHandRays();
	}

	// Token: 0x06004493 RID: 17555 RVA: 0x0016B180 File Offset: 0x00169380
	private void OnChar(VREvent_t ev)
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		char c = ev.data.keyboard.cNewInput.get_Chars(0);
		if (c == '\b')
		{
			this._inputBuffer = this._inputBuffer.Remove(this._inputBuffer.Length - 1, 1);
			return;
		}
		if (this.IsIllegalChar(c))
		{
			return;
		}
		this._inputBuffer += c.ToString();
	}

	// Token: 0x06004494 RID: 17556 RVA: 0x0016B1F4 File Offset: 0x001693F4
	private void OnKeyboardClosed(VREvent_t ev)
	{
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Trying to close Keyboard");
		if (!this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Closing Keyboard");
		OpenVR.Overlay.HideKeyboard();
		this._inputField.text = this._inputBuffer;
		this._inputField.DeactivateInputField(false);
		HandRayController.Instance.EnableHandRays();
		this.keyboardShowing = false;
	}

	// Token: 0x06004495 RID: 17557 RVA: 0x0016B256 File Offset: 0x00169456
	private bool IsIllegalChar(char c)
	{
		return c == '\t' || c == '\n';
	}

	// Token: 0x04005630 RID: 22064
	[Header("Haptics")]
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04005631 RID: 22065
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04005632 RID: 22066
	[Header("Steam Settings")]
	[SerializeField]
	private TMP_InputField _inputField;

	// Token: 0x04005633 RID: 22067
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005634 RID: 22068
	public bool testMinimal;

	// Token: 0x04005635 RID: 22069
	public bool minimalMode;

	// Token: 0x04005636 RID: 22070
	private bool inside;

	// Token: 0x04005637 RID: 22071
	private bool keyboardShowing;

	// Token: 0x04005638 RID: 22072
	private bool _canTrigger = true;

	// Token: 0x04005639 RID: 22073
	private string _testStr = string.Empty;

	// Token: 0x0400563A RID: 22074
	private string previousStr = string.Empty;

	// Token: 0x0400563B RID: 22075
	private StringBuilder _inputStringBuilder = new StringBuilder(1024);

	// Token: 0x0400563C RID: 22076
	private string _inputBuffer = "";
}
