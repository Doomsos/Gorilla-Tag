using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x0200035D RID: 861
[AddComponentMenu("UI/KIDUI Scrollbar", 37)]
public class KIDUIScrollbar : Scrollbar, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001479 RID: 5241 RVA: 0x00075546 File Offset: 0x00073746
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x0600147A RID: 5242 RVA: 0x00075557 File Offset: 0x00073757
	private KIDUIScrollbar.Axis axis
	{
		get
		{
			if (base.direction != null && base.direction != 1)
			{
				return KIDUIScrollbar.Axis.Vertical;
			}
			return KIDUIScrollbar.Axis.Horizontal;
		}
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x00075570 File Offset: 0x00073770
	protected override void OnEnable()
	{
		base.OnEnable();
		this.containerRect = base.handleRect.parent.GetComponent<RectTransform>();
		if (GorillaTagger.Instance)
		{
			this.thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		}
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x000755DD File Offset: 0x000737DD
	protected override void OnDisable()
	{
		base.OnDisable();
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this._isPointerInside = false;
		this._currentPointerData = null;
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x00075618 File Offset: 0x00073818
	private void PostUpdate()
	{
		if (!this._isPointerInside && !ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = false;
			return;
		}
		if (!base.interactable || !ControllerBehaviour.Instance.TriggerDown || this._currentPointerData == null)
		{
			return;
		}
		if (!this._isHolding && this._isPointerInside && ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = true;
		}
		if (!this._isHolding || !this.IsInteractable() || this.InputModule == null)
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(this._currentPointerData.pointerId) as XRRayInteractor;
		RaycastResult raycastResult;
		if (xrrayInteractor != null && xrrayInteractor.TryGetCurrentUIRaycastResult(ref raycastResult))
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.containerRect, raycastResult.screenPosition, this.thirdPersonCamera, ref vector);
			Vector2 zero = Vector2.zero;
			Vector2 handleCorner = vector - zero - this.containerRect.rect.position - (base.handleRect.rect.size - base.handleRect.sizeDelta) * 0.5f;
			float num = ((this.axis == KIDUIScrollbar.Axis.Horizontal) ? this.containerRect.rect.width : this.containerRect.rect.height) * (1f - base.size);
			if (num <= 0f)
			{
				return;
			}
			this.UpdateDrag(handleCorner, num);
		}
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x0007579C File Offset: 0x0007399C
	private void UpdateDrag(Vector2 handleCorner, float remainingSize)
	{
		switch (base.direction)
		{
		case 0:
			base.value = Mathf.Clamp01(handleCorner.x / remainingSize);
			return;
		case 1:
			base.value = Mathf.Clamp01(1f - handleCorner.x / remainingSize);
			return;
		case 2:
			base.value = Mathf.Clamp01(handleCorner.y / remainingSize);
			return;
		case 3:
			base.value = Mathf.Clamp01(1f - handleCorner.y / remainingSize);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x00075824 File Offset: 0x00073A24
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this._isPointerInside = true;
		this._currentPointerData = eventData;
		if (this.IsInteractable() && this.InputModule != null)
		{
			XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
			if (xrrayInteractor != null)
			{
				xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
			}
		}
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0007588E File Offset: 0x00073A8E
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this._isPointerInside = false;
	}

	// Token: 0x04001F19 RID: 7961
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04001F1A RID: 7962
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04001F1B RID: 7963
	private RectTransform containerRect;

	// Token: 0x04001F1C RID: 7964
	private bool _isPointerInside;

	// Token: 0x04001F1D RID: 7965
	private bool _isHolding;

	// Token: 0x04001F1E RID: 7966
	private PointerEventData _currentPointerData;

	// Token: 0x04001F1F RID: 7967
	private Camera thirdPersonCamera;

	// Token: 0x0200035E RID: 862
	private enum Axis
	{
		// Token: 0x04001F21 RID: 7969
		Horizontal,
		// Token: 0x04001F22 RID: 7970
		Vertical
	}
}
