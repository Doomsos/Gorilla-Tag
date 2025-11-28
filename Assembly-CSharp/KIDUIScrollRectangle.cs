using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x0200035F RID: 863
[AddComponentMenu("UI/KIDUI Scroll Rect", 37)]
public class KIDUIScrollRectangle : ScrollRect, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001482 RID: 5250 RVA: 0x00075546 File Offset: 0x00073746
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x000758BC File Offset: 0x00073ABC
	protected override void OnEnable()
	{
		base.OnEnable();
		this.thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x000758FC File Offset: 0x00073AFC
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

	// Token: 0x06001485 RID: 5253 RVA: 0x00075938 File Offset: 0x00073B38
	private void PostUpdate()
	{
		if (this._currentPointerData == null || this.InputModule == null)
		{
			return;
		}
		if (this._currentPointerData.hovered.Contains(base.viewport.gameObject) && !this._currentPointerData.hovered.Contains(base.verticalScrollbar.gameObject))
		{
			this._isPointerInside = true;
		}
		else
		{
			this._isPointerInside = false;
		}
		if (!ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = false;
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(this._currentPointerData.pointerId) as XRRayInteractor;
		if (xrrayInteractor == null)
		{
			return;
		}
		XRRayInteractor xrrayInteractor2 = xrrayInteractor;
		RaycastResult raycastResult;
		if (!xrrayInteractor2.TryGetCurrentUIRaycastResult(ref raycastResult))
		{
			return;
		}
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.viewRect, raycastResult.screenPosition, this.thirdPersonCamera, ref vector);
		if (!this._isHolding && this._isPointerInside && ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = true;
			this.m_PointerStartLocalCursor = vector;
			this.m_ContentStartPosition = base.content.anchoredPosition;
		}
		if (!this._isHolding)
		{
			return;
		}
		base.UpdateBounds();
		Vector2 vector2 = vector - this.m_PointerStartLocalCursor;
		Vector2 contentAnchoredPosition = this.m_ContentStartPosition + vector2;
		this.SetContentAnchoredPosition(contentAnchoredPosition);
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x00075A74 File Offset: 0x00073C74
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.hovered.Contains(base.viewport.gameObject))
		{
			this._isPointerInside = true;
			this._currentPointerData = eventData;
		}
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x00075A9C File Offset: 0x00073C9C
	public void OnPointerExit(PointerEventData eventData)
	{
		this._isPointerInside = false;
	}

	// Token: 0x04001F23 RID: 7971
	private bool _isPointerInside;

	// Token: 0x04001F24 RID: 7972
	private bool _isHolding;

	// Token: 0x04001F25 RID: 7973
	private PointerEventData _currentPointerData;

	// Token: 0x04001F26 RID: 7974
	private Vector2 m_PointerStartLocalCursor = Vector2.zero;

	// Token: 0x04001F27 RID: 7975
	private Camera thirdPersonCamera;
}
