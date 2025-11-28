using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Token: 0x02000A7C RID: 2684
public class HandRayController : MonoBehaviour
{
	// Token: 0x17000661 RID: 1633
	// (get) Token: 0x06004371 RID: 17265 RVA: 0x0016610A File Offset: 0x0016430A
	public static HandRayController Instance
	{
		get
		{
			if (HandRayController.instance == null)
			{
				HandRayController.instance = Object.FindAnyObjectByType<HandRayController>();
				if (HandRayController.instance == null)
				{
					Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Not found in scene", Array.Empty<object>());
				}
			}
			return HandRayController.instance;
		}
	}

	// Token: 0x06004372 RID: 17266 RVA: 0x00166144 File Offset: 0x00164344
	private void Awake()
	{
		if (HandRayController.instance != null && HandRayController.instance != this)
		{
			Debug.LogErrorFormat(base.gameObject, "[KID::UI::HAND_RAY_CONTROLLER] Duplicate instance of HandRayController", Array.Empty<object>());
			Object.DestroyImmediate(this);
			return;
		}
		HandRayController.instance = this;
	}

	// Token: 0x06004373 RID: 17267 RVA: 0x00166184 File Offset: 0x00164384
	private void Start()
	{
		this._leftHandRay.attachTransform = (this._leftHandRay.rayOriginTransform = KIDHandReference.LeftHand.transform);
		this._rightHandRay.attachTransform = (this._rightHandRay.rayOriginTransform = KIDHandReference.RightHand.transform);
		this.DisableHandRays();
		this._activationCounter = 0;
	}

	// Token: 0x06004374 RID: 17268 RVA: 0x001661E4 File Offset: 0x001643E4
	private void OnDisable()
	{
		this.DisableHandRays();
	}

	// Token: 0x06004375 RID: 17269 RVA: 0x001661EC File Offset: 0x001643EC
	public void EnableHandRays()
	{
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			this.ToggleHands();
		}
		this._activationCounter++;
	}

	// Token: 0x06004376 RID: 17270 RVA: 0x0016622C File Offset: 0x0016442C
	public void DisableHandRays()
	{
		this._activationCounter--;
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.HideHands();
		}
	}

	// Token: 0x06004377 RID: 17271 RVA: 0x0016626C File Offset: 0x0016446C
	public void PulseActiveHandray(float vibrationStrength, float vibrationDuration)
	{
		if (this._activeHandRay == null)
		{
			return;
		}
		this._activeHandRay.SendHapticImpulse(vibrationStrength, vibrationDuration);
	}

	// Token: 0x06004378 RID: 17272 RVA: 0x0016628B File Offset: 0x0016448B
	private void PostUpdate()
	{
		if (!this._hasInitialised)
		{
			return;
		}
		if (this.ActiveHand == HandRayController.HandSide.Left)
		{
			if (ControllerBehaviour.Instance.RightButtonDown)
			{
				this.ToggleHands();
			}
			return;
		}
		if (ControllerBehaviour.Instance.LeftButtonDown)
		{
			this.ToggleHands();
		}
	}

	// Token: 0x06004379 RID: 17273 RVA: 0x001662C4 File Offset: 0x001644C4
	private void ToggleRightHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] RIGHT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._rightHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._rightHandRay;
		}
	}

	// Token: 0x0600437A RID: 17274 RVA: 0x00166320 File Offset: 0x00164520
	private void ToggleLeftHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] LEFT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._leftHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._leftHandRay;
		}
	}

	// Token: 0x0600437B RID: 17275 RVA: 0x0016637C File Offset: 0x0016457C
	private void InitialiseHands()
	{
		Debug.Log("[KID::UI::HAND_RAY_CONTROLLER] Initialising Hands");
		this.ToggleRightHandRay(this.ActiveHand == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(this.ActiveHand == HandRayController.HandSide.Left);
		this._hasInitialised = true;
	}

	// Token: 0x0600437C RID: 17276 RVA: 0x001663B0 File Offset: 0x001645B0
	private void ToggleHands()
	{
		if (!this._hasInitialised)
		{
			this.InitialiseHands();
			return;
		}
		HandRayController.HandSide handSide = (this.ActiveHand == HandRayController.HandSide.Left) ? HandRayController.HandSide.Right : HandRayController.HandSide.Left;
		Debug.LogFormat(string.Concat(new string[]
		{
			"[KID::UI::HAND_RAY_CONTROLLER] Setting ActiveHand FROM: [",
			this.ActiveHand.ToString(),
			"] TO: [",
			handSide.ToString(),
			"]"
		}), Array.Empty<object>());
		this.ActiveHand = handSide;
		this.ToggleRightHandRay(handSide == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(handSide == HandRayController.HandSide.Left);
	}

	// Token: 0x0600437D RID: 17277 RVA: 0x00166445 File Offset: 0x00164645
	private void HideHands()
	{
		this.ToggleRightHandRay(false);
		this.ToggleLeftHandRay(false);
		this._hasInitialised = false;
		this._activeHandRay = null;
	}

	// Token: 0x040054F8 RID: 21752
	[OnEnterPlay_SetNull]
	private static HandRayController instance;

	// Token: 0x040054F9 RID: 21753
	[SerializeField]
	private XRRayInteractor _leftHandRay;

	// Token: 0x040054FA RID: 21754
	[SerializeField]
	private XRRayInteractor _rightHandRay;

	// Token: 0x040054FB RID: 21755
	private bool _hasInitialised;

	// Token: 0x040054FC RID: 21756
	private HandRayController.HandSide ActiveHand = HandRayController.HandSide.Right;

	// Token: 0x040054FD RID: 21757
	private XRRayInteractor _activeHandRay;

	// Token: 0x040054FE RID: 21758
	private int _activationCounter;

	// Token: 0x02000A7D RID: 2685
	private enum HandSide
	{
		// Token: 0x04005500 RID: 21760
		Left,
		// Token: 0x04005501 RID: 21761
		Right
	}
}
