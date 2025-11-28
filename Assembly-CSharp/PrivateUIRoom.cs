using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

// Token: 0x02000406 RID: 1030
public class PrivateUIRoom : MonoBehaviourTick
{
	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06001921 RID: 6433 RVA: 0x00085C54 File Offset: 0x00083E54
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x00086778 File Offset: 0x00084978
	private void Awake()
	{
		if (PrivateUIRoom.instance == null)
		{
			PrivateUIRoom.instance = this;
			this.occluder.SetActive(false);
			this.leftHandObject.SetActive(false);
			this.rightHandObject.SetActive(false);
			this.ui = new List<Transform>();
			this.uiParents = new Dictionary<Transform, Transform>();
			this.backgroundDirectionPropertyID = Shader.PropertyToID(this.backgroundDirectionPropertyName);
			this._uiRoot = new GameObject("UIRoot").transform;
			this._uiRoot.parent = base.transform;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x00086810 File Offset: 0x00084A10
	private new void OnEnable()
	{
		base.OnEnable();
		SteamVR_Events.System(406).Listen(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x00086833 File Offset: 0x00084A33
	private new void OnDisable()
	{
		base.OnDisable();
		SteamVR_Events.System(406).Remove(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x00086858 File Offset: 0x00084A58
	private static bool FindShoulderCamera()
	{
		if (PrivateUIRoom._shoulderCameraReference.IsNotNull())
		{
			return true;
		}
		if (GorillaTagger.Instance.IsNull())
		{
			return false;
		}
		PrivateUIRoom._shoulderCameraReference = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		if (PrivateUIRoom._shoulderCameraReference == null)
		{
			Debug.LogError("[PRIVATE_UI_ROOMS] Could not find Shoulder Camera");
			return false;
		}
		PrivateUIRoom._virtualCameraReference = PrivateUIRoom._shoulderCameraReference.GetComponentInChildren<CinemachineVirtualCamera>();
		return true;
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x000868C0 File Offset: 0x00084AC0
	private void ToggleHands(VREvent_t ev)
	{
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] Toggling hands visibility. Event: {0} ({1})", ev.eventType, ev.eventType));
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] _handsShowing: {0}", PrivateUIRoom.instance.rightHandObject.activeSelf));
		if (PrivateUIRoom.instance.rightHandObject.activeSelf)
		{
			this.HideHands();
			return;
		}
		this.ShowHands();
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x00086933 File Offset: 0x00084B33
	private void HideHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu shown, disabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x0008695F File Offset: 0x00084B5F
	private void ShowHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu hidden, re-enabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x0008698C File Offset: 0x00084B8C
	private void ToggleLevelVisibility(bool levelShouldBeVisible)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (levelShouldBeVisible)
		{
			component.cullingMask = this.savedCullingLayers;
			if (this.savedCullingLayersShoudlerCam != null)
			{
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.savedCullingLayersShoudlerCam.Value;
				this.savedCullingLayersShoudlerCam = default(int?);
				return;
			}
		}
		else
		{
			this.savedCullingLayers = component.cullingMask;
			component.cullingMask = this.visibleLayers;
			if (PrivateUIRoom.FindShoulderCamera())
			{
				this.savedCullingLayersShoudlerCam = new int?(PrivateUIRoom._shoulderCameraReference.cullingMask);
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.visibleLayers;
				PrivateUIRoom._virtualCameraReference.enabled = false;
			}
		}
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x00086A40 File Offset: 0x00084C40
	private static void StopOverlay()
	{
		PrivateUIRoom.instance.localPlayer.inOverlay = false;
		PrivateUIRoom.instance.inOverlay = false;
		PrivateUIRoom.instance.localPlayer.disableMovement = false;
		PrivateUIRoom.instance.localPlayer.InReportMenu = false;
		PrivateUIRoom.instance.ToggleLevelVisibility(true);
		PrivateUIRoom.instance.occluder.SetActive(false);
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
		PrivateUIRoom._virtualCameraReference.enabled = true;
		KIDAudioManager.Instance.SetKIDUIAudioActive(false);
		Debug.Log("[PrivateUIRoom::StopOverlay] Re-enabling Game Audio");
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x00086AE4 File Offset: 0x00084CE4
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * Vector3.zero * scale.x;
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x00086B70 File Offset: 0x00084D70
	private static void AssignShoulderCameraToCanvases(Transform focus)
	{
		Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] setting up canvases with shoulder camera.");
		if (!PrivateUIRoom.FindShoulderCamera())
		{
			return;
		}
		Canvas componentInChildren = focus.GetComponentInChildren<Canvas>(true);
		if (componentInChildren != null)
		{
			componentInChildren.worldCamera = PrivateUIRoom._shoulderCameraReference;
			Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] Assigned shoulder camera to Canvas: " + componentInChildren.name);
			return;
		}
		Debug.LogError("[KID::PrivateUIRoom::CanvasCameraAssigner] No Canvas component found on this GameObject.");
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x00086BCC File Offset: 0x00084DCC
	public static void AddUI(Transform focus)
	{
		if (PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		PrivateUIRoom.AssignShoulderCameraToCanvases(focus);
		PrivateUIRoom.instance.uiParents.Add(focus, focus.parent);
		focus.gameObject.SetActive(false);
		focus.parent = PrivateUIRoom.instance._uiRoot;
		focus.localPosition = Vector3.zero;
		focus.localRotation = Quaternion.identity;
		PrivateUIRoom.instance.ui.Add(focus);
		if (PrivateUIRoom.instance.ui.Count == 1 && PrivateUIRoom.instance.focusTransform == null)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			if (!PrivateUIRoom.instance.inOverlay)
			{
				PrivateUIRoom.StartOverlay();
			}
		}
		PrivateUIRoom.instance.UpdateUIPositionAndRotation();
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x00086CB8 File Offset: 0x00084EB8
	public static void RemoveUI(Transform focus)
	{
		if (!PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		focus.gameObject.SetActive(false);
		PrivateUIRoom.instance.ui.Remove(focus);
		if (PrivateUIRoom.instance.focusTransform == focus)
		{
			PrivateUIRoom.instance.focusTransform = null;
		}
		if (PrivateUIRoom.instance.uiParents[focus] != null)
		{
			focus.parent = PrivateUIRoom.instance.uiParents[focus];
			PrivateUIRoom.instance.uiParents.Remove(focus);
		}
		else
		{
			Object.Destroy(focus.gameObject);
		}
		if (PrivateUIRoom.instance.ui.Count > 0)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			return;
		}
		if (!PrivateUIRoom.instance.overlayForcedActive)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x00086DB1 File Offset: 0x00084FB1
	public static void ForceStartOverlay()
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedActive = true;
		if (PrivateUIRoom.instance.inOverlay)
		{
			return;
		}
		PrivateUIRoom.StartOverlay();
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x00086DDE File Offset: 0x00084FDE
	public static void StopForcedOverlay()
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedActive = false;
		if (PrivateUIRoom.instance.ui.Count == 0 && PrivateUIRoom.instance.inOverlay)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x00086E1C File Offset: 0x0008501C
	private static void StartOverlay()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 localScale;
		PrivateUIRoom.instance.GetIdealScreenPositionRotation(out vector, out quaternion, out localScale);
		PrivateUIRoom.instance.leftHandObject.transform.localScale = localScale;
		PrivateUIRoom.instance.rightHandObject.transform.localScale = localScale;
		PrivateUIRoom.instance.occluder.transform.localScale = localScale;
		PrivateUIRoom.instance.localPlayer.InReportMenu = true;
		PrivateUIRoom.instance.localPlayer.disableMovement = true;
		PrivateUIRoom.instance.occluder.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.ToggleLevelVisibility(false);
		PrivateUIRoom.instance.localPlayer.inOverlay = true;
		PrivateUIRoom.instance.inOverlay = true;
		KIDAudioManager.Instance.SetKIDUIAudioActive(true);
		Debug.Log("[PrivateUIRoom::StartOverlay] Muting Game Audio");
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x00086F04 File Offset: 0x00085104
	public override void Tick()
	{
		if (!this.localPlayer.InReportMenu)
		{
			return;
		}
		this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
		Transform controllerTransform = this.localPlayer.GetControllerTransform(true);
		Transform controllerTransform2 = this.localPlayer.GetControllerTransform(false);
		this.rightHandObject.transform.SetPositionAndRotation(controllerTransform2.position, controllerTransform2.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(controllerTransform.position, controllerTransform.rotation);
		if (this.ShouldUpdateRotation())
		{
			this.UpdateUIPositionAndRotation();
			return;
		}
		if (this.ShouldUpdatePosition())
		{
			this.UpdateUIPosition();
		}
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x00086FB4 File Offset: 0x000851B4
	private bool ShouldUpdateRotation()
	{
		float magnitude = (GorillaTagger.Instance.mainCamera.transform.position - this.lastStablePosition).X_Z().magnitude;
		Quaternion quaternion = Quaternion.Euler(0f, GorillaTagger.Instance.mainCamera.transform.rotation.eulerAngles.y, 0f);
		float num = Quaternion.Angle(this.lastStableRotation, quaternion);
		return magnitude > this.lateralPlay || num >= this.rotationalPlay;
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x00087041 File Offset: 0x00085241
	private bool ShouldUpdatePosition()
	{
		return Mathf.Abs(GorillaTagger.Instance.mainCamera.transform.position.y - this.lastStablePosition.y) > this.verticalPlay;
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x00087078 File Offset: 0x00085278
	private void UpdateUIPositionAndRotation()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this.lastStableRotation = transform.rotation;
		Vector3 normalized = transform.forward.X_Z().normalized;
		this._uiRoot.SetPositionAndRotation(this.lastStablePosition + normalized * 0.02f, Quaternion.LookRotation(normalized));
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
		PrivateUIRoom._shoulderCameraReference.transform.rotation = this._uiRoot.rotation;
		this.backgroundRenderer.material.SetVector(this.backgroundDirectionPropertyID, this.backgroundRenderer.transform.InverseTransformDirection(normalized));
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x00087148 File Offset: 0x00085348
	private void UpdateUIPosition()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this._uiRoot.position = this.lastStablePosition + this.lastStableRotation * new Vector3(0f, 0f, 0.02f);
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x000871C0 File Offset: 0x000853C0
	public static bool GetInOverlay()
	{
		return !(PrivateUIRoom.instance == null) && PrivateUIRoom.instance.inOverlay;
	}

	// Token: 0x04002281 RID: 8833
	[SerializeField]
	private GameObject occluder;

	// Token: 0x04002282 RID: 8834
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x04002283 RID: 8835
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x04002284 RID: 8836
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x04002285 RID: 8837
	[SerializeField]
	private MeshRenderer backgroundRenderer;

	// Token: 0x04002286 RID: 8838
	[SerializeField]
	private string backgroundDirectionPropertyName = "_SpotDirection";

	// Token: 0x04002287 RID: 8839
	private int backgroundDirectionPropertyID;

	// Token: 0x04002288 RID: 8840
	private int savedCullingLayers;

	// Token: 0x04002289 RID: 8841
	private Transform _uiRoot;

	// Token: 0x0400228A RID: 8842
	private Transform focusTransform;

	// Token: 0x0400228B RID: 8843
	private List<Transform> ui;

	// Token: 0x0400228C RID: 8844
	private Dictionary<Transform, Transform> uiParents;

	// Token: 0x0400228D RID: 8845
	private float _initialAudioVolume;

	// Token: 0x0400228E RID: 8846
	private bool inOverlay;

	// Token: 0x0400228F RID: 8847
	private bool overlayForcedActive;

	// Token: 0x04002290 RID: 8848
	private static PrivateUIRoom instance;

	// Token: 0x04002291 RID: 8849
	private Vector3 lastStablePosition;

	// Token: 0x04002292 RID: 8850
	private Quaternion lastStableRotation;

	// Token: 0x04002293 RID: 8851
	[SerializeField]
	private float verticalPlay = 0.1f;

	// Token: 0x04002294 RID: 8852
	[SerializeField]
	private float lateralPlay = 0.5f;

	// Token: 0x04002295 RID: 8853
	[SerializeField]
	private float rotationalPlay = 45f;

	// Token: 0x04002296 RID: 8854
	private int? savedCullingLayersShoudlerCam;

	// Token: 0x04002297 RID: 8855
	private static Camera _shoulderCameraReference;

	// Token: 0x04002298 RID: 8856
	private static CinemachineVirtualCamera _virtualCameraReference;
}
