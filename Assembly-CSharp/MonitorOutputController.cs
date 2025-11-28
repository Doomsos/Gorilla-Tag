using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x02000388 RID: 904
public class MonitorOutputController : MonoBehaviour
{
	// Token: 0x06001595 RID: 5525 RVA: 0x00079A12 File Offset: 0x00077C12
	private void Awake()
	{
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x00079A25 File Offset: 0x00077C25
	private void OnEnable()
	{
		this._gtLckController.OnCameraModeChanged += new GTLckController.CameraModeDelegate(this.OnCameraModeChanged);
		LckBodyCameraSpawner.OnCameraStateChange += this.CameraStateChanged;
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x00079A50 File Offset: 0x00077C50
	private void Update()
	{
		if (Application.platform == 11)
		{
			Object.Destroy(this);
		}
		if (this._shoulderCamera == null)
		{
			this.FindShoulderCamera();
		}
		if (this._lckCamera != null)
		{
			this._shoulderCamera.transform.position = this._lckCamera.transform.position;
			this._shoulderCamera.transform.rotation = this._lckCamera.transform.rotation;
			this._shoulderCamera.fieldOfView = this._lckCamera.fieldOfView;
			return;
		}
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x00079AF6 File Offset: 0x00077CF6
	private void CameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		switch (state)
		{
		case LckBodyCameraSpawner.CameraState.CameraDisabled:
			this.RestoreShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraOnNeck:
			this.TakeOverShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraSpawned:
			this.TakeOverShoulderCamera();
			return;
		default:
			return;
		}
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x00079B1F File Offset: 0x00077D1F
	private void OnDisable()
	{
		this._gtLckController.OnCameraModeChanged -= new GTLckController.CameraModeDelegate(this.OnCameraModeChanged);
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		LckBodyCameraSpawner.OnCameraStateChange -= this.CameraStateChanged;
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x00079B5F File Offset: 0x00077D5F
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x00079B74 File Offset: 0x00077D74
	private void TakeOverShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = false;
		this._shoulderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LCKHide"));
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x00079BB4 File Offset: 0x00077DB4
	private void RestoreShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		this._shoulderCamera.cullingMask |= 1 << LayerMask.NameToLayer("LCKHide");
		this._shoulderCamera.fieldOfView = this._shoulderCameraFov;
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x00079C10 File Offset: 0x00077E10
	private void FindShoulderCamera()
	{
		if (this._shoulderCamera != null)
		{
			return;
		}
		if (!GorillaTagger.hasInstance || !base.isActiveAndEnabled)
		{
			return;
		}
		this._shoulderCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		this._shoulderCameraFov = this._shoulderCamera.fieldOfView;
	}

	// Token: 0x04002007 RID: 8199
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x04002008 RID: 8200
	private Camera _lckCamera;

	// Token: 0x04002009 RID: 8201
	private CameraMode _lckActiveCameraMode;

	// Token: 0x0400200A RID: 8202
	private Camera _shoulderCamera;

	// Token: 0x0400200B RID: 8203
	private float _shoulderCameraFov;
}
