using System;
using GorillaLocomotion;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000384 RID: 900
public class LckTabletSizeManager : MonoBehaviour
{
	// Token: 0x0600156E RID: 5486 RVA: 0x00078E7A File Offset: 0x0007707A
	private void Start()
	{
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Combine(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
		this._controller.OnHorizontalModeChanged += new UnityAction<bool>(this.OnHorizontalModeChanged);
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x00078EBA File Offset: 0x000770BA
	private void OnDestroy()
	{
		this._controller.OnHorizontalModeChanged -= new UnityAction<bool>(this.OnHorizontalModeChanged);
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Remove(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x00078EFA File Offset: 0x000770FA
	private void OnHorizontalModeChanged(bool mode)
	{
		this.UpdateCustomNearClip(0);
		this.UpdateCustomNearClip(1);
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x00078F0A File Offset: 0x0007710A
	private void UpdateCustomNearClip(CameraMode mode)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		switch (mode)
		{
		case 0:
			this.SetCustomNearClip(this._selfieCamera);
			return;
		case 1:
			this.SetCustomNearClip(this._firstPersonCamera);
			break;
		case 2:
		case 3:
			break;
		default:
			return;
		}
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x00078F4C File Offset: 0x0007714C
	private void SetCustomNearClip(Camera cam)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		Matrix4x4 projectionMatrix;
		if (this._controller.HorizontalMode)
		{
			projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, 1.777778f, this._customNearClip, cam.farClipPlane);
		}
		else
		{
			projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, 0.5625f, this._customNearClip, cam.farClipPlane);
		}
		cam.projectionMatrix = projectionMatrix;
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x00078FB6 File Offset: 0x000771B6
	private void ClearCustomNearClip()
	{
		this._selfieCamera.ResetProjectionMatrix();
		this._firstPersonCamera.ResetProjectionMatrix();
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x00078FD0 File Offset: 0x000771D0
	private void PlayerBecameSmall()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamShrinkPosition;
		this._tabletFollower.SetPlayerSizeModifier(false, this._shrinkSize);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
		}
		this.SetCustomNearClip(this._selfieCamera);
		this.SetCustomNearClip(this._firstPersonCamera);
	}

	// Token: 0x06001575 RID: 5493 RVA: 0x00079030 File Offset: 0x00077230
	private void PlayerBecameDefaultSize()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamDefaultPosition;
		this._tabletFollower.SetPlayerSizeModifier(true, 1f);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
		}
		this.ClearCustomNearClip();
	}

	// Token: 0x06001576 RID: 5494 RVA: 0x00079080 File Offset: 0x00077280
	private void SetCameraOnNeck()
	{
		GameObject gameObject = Camera.main.transform.Find("LCKBodyCameraSpawner(Clone)").gameObject;
		if (gameObject != null)
		{
			gameObject.GetComponent<LckBodyCameraSpawner>().ManuallySetCameraOnNeck();
		}
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x000790BC File Offset: 0x000772BC
	private void Update()
	{
		if (!GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = false;
			this.PlayerBecameSmall();
			return;
		}
		if (GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = true;
			this.PlayerBecameDefaultSize();
		}
	}

	// Token: 0x04001FE4 RID: 8164
	[SerializeField]
	private GTLckController _controller;

	// Token: 0x04001FE5 RID: 8165
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x04001FE6 RID: 8166
	[SerializeField]
	private GtTabletFollower _tabletFollower;

	// Token: 0x04001FE7 RID: 8167
	[SerializeField]
	private Camera _firstPersonCamera;

	// Token: 0x04001FE8 RID: 8168
	[SerializeField]
	private Camera _selfieCamera;

	// Token: 0x04001FE9 RID: 8169
	private Vector3 _firstPersonCamShrinkPosition = new Vector3(0f, 0f, -0.78f);

	// Token: 0x04001FEA RID: 8170
	private Vector3 _firstPersonCamDefaultPosition = Vector3.zero;

	// Token: 0x04001FEB RID: 8171
	private float _shrinkSize = 0.06f;

	// Token: 0x04001FEC RID: 8172
	private Vector3 _shrinkVector = new Vector3(0.06f, 0.06f, 0.06f);

	// Token: 0x04001FED RID: 8173
	private float _customNearClip = 0.0006f;

	// Token: 0x04001FEE RID: 8174
	private bool _isDefaultScale = true;
}
