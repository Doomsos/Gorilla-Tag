using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class LckSocialCameraManager : MonoBehaviour
{
	// Token: 0x17000216 RID: 534
	// (get) Token: 0x0600155B RID: 5467 RVA: 0x000788ED File Offset: 0x00076AED
	public LckDirectGrabbable lckDirectGrabbable
	{
		get
		{
			return this._lckDirectGrabbable;
		}
	}

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x0600155C RID: 5468 RVA: 0x000788F5 File Offset: 0x00076AF5
	public static LckSocialCameraManager Instance
	{
		get
		{
			return LckSocialCameraManager._instance;
		}
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000788FC File Offset: 0x00076AFC
	public void SetForceHidden(bool hidden)
	{
		this._forceHidden = hidden;
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x00078905 File Offset: 0x00076B05
	private void Awake()
	{
		this.SetManagerInstance();
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x0007891E File Offset: 0x00076B1E
	public void SetLckSocialCococamCamera(LckSocialCamera socialCamera)
	{
		this._socialCameraCococamInstance = socialCamera;
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x00078927 File Offset: 0x00076B27
	public void SetLckSocialTabletCamera(LckSocialCamera socialCameraTablet)
	{
		this._socialCameraTabletInstance = socialCameraTablet;
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x00078930 File Offset: 0x00076B30
	private void SetManagerInstance()
	{
		LckSocialCameraManager._instance = this;
		Action<LckSocialCameraManager> onManagerSpawned = LckSocialCameraManager.OnManagerSpawned;
		if (onManagerSpawned == null)
		{
			return;
		}
		onManagerSpawned.Invoke(this);
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x00078948 File Offset: 0x00076B48
	private void OnEnable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted += new Action<LckResult>(this.OnRecordingStarted);
			service.Result.OnStreamingStarted += new Action<LckResult>(this.OnRecordingStarted);
			service.Result.OnRecordingStopped += new Action<LckResult>(this.OnRecordingStopped);
			service.Result.OnStreamingStopped += new Action<LckResult>(this.OnRecordingStopped);
		}
		LckBodyCameraSpawner.OnCameraStateChange += this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged += new GTLckController.CameraModeDelegate(this.OnCameraModeChanged);
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000789E8 File Offset: 0x00076BE8
	private void OnBodyCameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		if (this._socialCameraTabletInstance == null)
		{
			return;
		}
		if (this._forceHidden)
		{
			this._socialCameraTabletInstance.visible = false;
			this._socialCameraCococamInstance.visible = false;
			return;
		}
		switch (state)
		{
		case LckBodyCameraSpawner.CameraState.CameraDisabled:
			this._socialCameraTabletInstance.visible = false;
			this._socialCameraCococamInstance.visible = false;
			this._socialCameraTabletInstance.IsOnNeck = false;
			return;
		case LckBodyCameraSpawner.CameraState.CameraOnNeck:
			this._socialCameraTabletInstance.visible = true;
			this._socialCameraTabletInstance.IsOnNeck = true;
			return;
		case LckBodyCameraSpawner.CameraState.CameraSpawned:
			this._socialCameraTabletInstance.visible = true;
			this._socialCameraTabletInstance.IsOnNeck = false;
			if (this._lckActiveCameraMode == 2)
			{
				this._socialCameraCococamInstance.visible = true;
			}
			return;
		default:
			return;
		}
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00078AA4 File Offset: 0x00076CA4
	private void Update()
	{
		if (this._socialCameraCococamInstance != null && this._socialCameraTabletInstance != null && this._lckCamera != null)
		{
			Transform transform = this._lckCamera.transform;
			this._socialCameraCococamInstance.transform.position = transform.position;
			this._socialCameraCococamInstance.transform.rotation = transform.rotation;
			this._socialCameraTabletInstance.transform.position = base.transform.position;
			this._socialCameraTabletInstance.transform.rotation = base.transform.rotation;
			Camera main = Camera.main;
			if (main != null)
			{
				this._lckCamera.nearClipPlane = main.nearClipPlane;
				this._lckCamera.farClipPlane = main.farClipPlane;
			}
		}
		if (this.CoconutCamera.gameObject.activeSelf)
		{
			CameraMode lckActiveCameraMode = this._lckActiveCameraMode;
			if (lckActiveCameraMode != null)
			{
				if (lckActiveCameraMode - 2 <= 1)
				{
					this.CoconutCamera.SetVisualsActive(this.cameraActive);
				}
				else
				{
					this.CoconutCamera.SetVisualsActive(false);
				}
			}
			else
			{
				this.CoconutCamera.SetVisualsActive(false);
			}
			this.CoconutCamera.SetRecordingState(this._recording);
		}
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x00078BE4 File Offset: 0x00076DE4
	private void OnDisable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted -= new Action<LckResult>(this.OnRecordingStarted);
			service.Result.OnRecordingStopped -= new Action<LckResult>(this.OnRecordingStopped);
			service.Result.OnStreamingStopped -= new Action<LckResult>(this.OnRecordingStopped);
			service.Result.OnStreamingStopped -= new Action<LckResult>(this.OnRecordingStopped);
		}
		LckBodyCameraSpawner.OnCameraStateChange -= this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged -= new GTLckController.CameraModeDelegate(this.OnCameraModeChanged);
	}

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06001566 RID: 5478 RVA: 0x00078C83 File Offset: 0x00076E83
	// (set) Token: 0x06001567 RID: 5479 RVA: 0x00078C90 File Offset: 0x00076E90
	public bool cameraActive
	{
		get
		{
			return this._localCameras.activeSelf;
		}
		set
		{
			this._localCameras.SetActive(value);
			if (!value)
			{
				this._gtLckController.StopRecording();
			}
		}
	}

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06001568 RID: 5480 RVA: 0x00078CAD File Offset: 0x00076EAD
	// (set) Token: 0x06001569 RID: 5481 RVA: 0x00078CBA File Offset: 0x00076EBA
	public bool uiVisible
	{
		get
		{
			return this._localUi.activeSelf;
		}
		set
		{
			this._localUi.SetActive(value);
		}
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00078CC8 File Offset: 0x00076EC8
	private void OnRecordingStarted(LckResult result)
	{
		this._recording = result.Success;
		if (this._socialCameraCococamInstance != null && this._socialCameraTabletInstance != null)
		{
			this._socialCameraCococamInstance.recording = result.Success;
			this._socialCameraTabletInstance.recording = result.Success;
		}
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x00078D1F File Offset: 0x00076F1F
	private void OnRecordingStopped(LckResult result)
	{
		this._recording = false;
		if (this._socialCameraCococamInstance != null && this._socialCameraTabletInstance != null)
		{
			this._socialCameraCococamInstance.recording = false;
			this._socialCameraTabletInstance.recording = false;
		}
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00078D5C File Offset: 0x00076F5C
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
		if (this._socialCameraCococamInstance == null || this._socialCameraTabletInstance == null)
		{
			return;
		}
		if (this._forceHidden)
		{
			this._socialCameraTabletInstance.visible = false;
			this._socialCameraCococamInstance.visible = false;
			return;
		}
		switch (this._lckActiveCameraMode)
		{
		case 0:
			if (this._socialCameraCococamInstance.visible)
			{
				this._socialCameraCococamInstance.visible = false;
				return;
			}
			break;
		case 1:
			if (this._socialCameraCococamInstance.visible)
			{
				this._socialCameraCococamInstance.visible = false;
				return;
			}
			break;
		case 2:
			if (!this._socialCameraCococamInstance.visible)
			{
				this._socialCameraCococamInstance.visible = true;
				return;
			}
			break;
		case 3:
			this._socialCameraCococamInstance.visible = (!this._forceHidden && this.cameraActive);
			this._socialCameraTabletInstance.visible = this.cameraActive;
			return;
		default:
			this._socialCameraCococamInstance.visible = this.cameraActive;
			this._socialCameraTabletInstance.visible = this.cameraActive;
			break;
		}
	}

	// Token: 0x04001FD7 RID: 8151
	[SerializeField]
	private GameObject _localUi;

	// Token: 0x04001FD8 RID: 8152
	[SerializeField]
	private GameObject _localCameras;

	// Token: 0x04001FD9 RID: 8153
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x04001FDA RID: 8154
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x04001FDB RID: 8155
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x04001FDC RID: 8156
	private LckSocialCamera _socialCameraCococamInstance;

	// Token: 0x04001FDD RID: 8157
	private LckSocialCamera _socialCameraTabletInstance;

	// Token: 0x04001FDE RID: 8158
	private Camera _lckCamera;

	// Token: 0x04001FDF RID: 8159
	private CameraMode _lckActiveCameraMode;

	// Token: 0x04001FE0 RID: 8160
	[OnEnterPlay_SetNull]
	private static LckSocialCameraManager _instance;

	// Token: 0x04001FE1 RID: 8161
	public static Action<LckSocialCameraManager> OnManagerSpawned;

	// Token: 0x04001FE2 RID: 8162
	private bool _recording;

	// Token: 0x04001FE3 RID: 8163
	private bool _forceHidden;
}
