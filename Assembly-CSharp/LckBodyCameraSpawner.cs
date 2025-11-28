using System;
using GorillaLocomotion;
using Liv.Lck;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000365 RID: 869
public class LckBodyCameraSpawner : MonoBehaviourTick
{
	// Token: 0x060014AA RID: 5290 RVA: 0x00075F31 File Offset: 0x00074131
	public void SetFollowTransform(Transform transform)
	{
		this._followTransform = transform;
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060014AB RID: 5291 RVA: 0x00075F3A File Offset: 0x0007413A
	public TabletSpawnInstance tabletSpawnInstance
	{
		get
		{
			return this._tabletSpawnInstance;
		}
	}

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x060014AC RID: 5292 RVA: 0x00075F44 File Offset: 0x00074144
	// (remove) Token: 0x060014AD RID: 5293 RVA: 0x00075F78 File Offset: 0x00074178
	public static event LckBodyCameraSpawner.CameraStateDelegate OnCameraStateChange;

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060014AE RID: 5294 RVA: 0x00075FAB File Offset: 0x000741AB
	// (set) Token: 0x060014AF RID: 5295 RVA: 0x00075FB4 File Offset: 0x000741B4
	public LckBodyCameraSpawner.CameraState cameraState
	{
		get
		{
			return this._cameraState;
		}
		set
		{
			switch (value)
			{
			case LckBodyCameraSpawner.CameraState.CameraDisabled:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.NotVisible;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = false;
				this.ResetCameraModel();
				this.cameraVisible = false;
				this._shouldMoveCameraToNeck = false;
				break;
			case LckBodyCameraSpawner.CameraState.CameraOnNeck:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				if (this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor())
				{
					this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor().ResetToDefaultAndTriggerButton();
					this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.ClearAllTriggers();
				}
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = true;
				this.ResetCameraModel();
				if (Application.platform == 11)
				{
					this.SetPreviewActive(false);
				}
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetDummyTabletBodyState(true);
				break;
			case LckBodyCameraSpawner.CameraState.CameraSpawned:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = true;
				this._tabletSpawnInstance.cameraActive = true;
				if (Application.platform == 11)
				{
					this.SetPreviewActive(true);
				}
				this.ResetCameraModel();
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetDummyTabletBodyState(false);
				break;
			}
			this._cameraState = value;
			LckBodyCameraSpawner.CameraStateDelegate onCameraStateChange = LckBodyCameraSpawner.OnCameraStateChange;
			if (onCameraStateChange == null)
			{
				return;
			}
			onCameraStateChange(this._cameraState);
		}
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x00076118 File Offset: 0x00074318
	private void SetPreviewActive(bool isActive)
	{
		LckResult<LckService> service = LckService.GetService();
		if (!service.Success)
		{
			Debug.LogError("LCK Could not get Service" + service.Error.ToString());
			return;
		}
		LckService result = service.Result;
		if (result == null)
		{
			return;
		}
		result.SetPreviewActive(isActive);
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060014B1 RID: 5297 RVA: 0x00076169 File Offset: 0x00074369
	// (set) Token: 0x060014B2 RID: 5298 RVA: 0x00076174 File Offset: 0x00074374
	public LckBodyCameraSpawner.CameraPosition cameraPosition
	{
		get
		{
			return this._cameraPosition;
		}
		set
		{
			if (this._cameraModelTransform != null && this._cameraPosition != value)
			{
				switch (value)
				{
				case LckBodyCameraSpawner.CameraPosition.CameraDefault:
					this.ChangeCameraModelParent(this._cameraPositionDefault);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
					return;
				case LckBodyCameraSpawner.CameraPosition.CameraSlingshot:
					this.ChangeCameraModelParent(this._cameraPositionSlingshot);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
					break;
				case LckBodyCameraSpawner.CameraPosition.NotVisible:
					break;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060014B3 RID: 5299 RVA: 0x000761D2 File Offset: 0x000743D2
	// (set) Token: 0x060014B4 RID: 5300 RVA: 0x000761E4 File Offset: 0x000743E4
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.enabled = value;
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x00076203 File Offset: 0x00074403
	private void Awake()
	{
		this._tabletSpawnInstance = new TabletSpawnInstance(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0007621C File Offset: 0x0007441C
	private new void OnEnable()
	{
		base.OnEnable();
		this.InitCameraStrap();
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
		if (this._tabletSpawnInstance.Controller != null)
		{
			this._previousMode = this._tabletSpawnInstance.Controller.CurrentCameraMode;
		}
		ZoneManagement.OnZoneChange += this.OnZoneChanged;
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x00076300 File Offset: 0x00074500
	private new void OnDisable()
	{
		base.OnDisable();
		ZoneManagement.OnZoneChange -= this.OnZoneChanged;
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000763A8 File Offset: 0x000745A8
	public override void Tick()
	{
		if (this._followTransform != null && base.transform.parent != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.parent.localToWorldMatrix;
			Vector3 vector = localToWorldMatrix.MultiplyPoint(this._followTransform.localPosition + this._followTransform.localRotation * new Vector3(0f, -0.05f, 0.1f));
			Quaternion quaternion = Quaternion.LookRotation(localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.forward), localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.up));
			base.transform.SetPositionAndRotation(vector, quaternion);
		}
		LckBodyCameraSpawner.CameraState cameraState = this._cameraState;
		if (cameraState != LckBodyCameraSpawner.CameraState.CameraOnNeck)
		{
			if (cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned)
			{
				this.UpdateCameraStrap();
				if (this._cameraModelGrabbable.isGrabbed)
				{
					GorillaGrabber grabber = this._cameraModelGrabbable.grabber;
					Transform transform = grabber.transform;
					if (this.ShouldSpawnCamera(transform))
					{
						this.SpawnCamera(grabber, transform);
					}
				}
				else
				{
					this.ResetCameraModel();
				}
				if (this._tabletSpawnInstance.isSpawned)
				{
					Transform transform3;
					if (this._tabletSpawnInstance.directGrabbable.isGrabbed)
					{
						GorillaGrabber grabber2 = this._tabletSpawnInstance.directGrabbable.grabber;
						Transform transform2 = grabber2.transform;
						if (!this.ShouldSpawnCamera(transform2))
						{
							this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
							this._cameraModelGrabbable.target.SetPositionAndRotation(transform2.position, transform2.rotation * Quaternion.Euler(this._chestSpawnRotationOffset.x, this._chestSpawnRotationOffset.y, this._chestSpawnRotationOffset.z));
							this._tabletSpawnInstance.directGrabbable.ForceRelease();
							this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
							this._tabletSpawnInstance.ResetLocalPose();
							this._cameraModelGrabbable.ForceGrab(grabber2);
							this._cameraModelGrabbable.onReleased += new Action(this.OnCameraModelReleased);
							this._previousMode = this._tabletSpawnInstance.Controller.CurrentCameraMode;
							if (this._previousMode == null)
							{
								this._tabletSpawnInstance.Controller.SetCameraMode(1);
							}
						}
					}
					else if (this._shouldMoveCameraToNeck && GtTag.TryGetTransform(1, ref transform3) && Vector3.SqrMagnitude(transform3.position - this.tabletSpawnInstance.position) >= this._snapToNeckDistance * this._snapToNeckDistance)
					{
						this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
						this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
						this._tabletSpawnInstance.ResetLocalPose();
						this._shouldMoveCameraToNeck = false;
					}
				}
			}
		}
		else
		{
			this.UpdateCameraStrap();
			if (this._cameraModelGrabbable.isGrabbed)
			{
				GorillaGrabber grabber3 = this._cameraModelGrabbable.grabber;
				Transform transform4 = grabber3.transform;
				if (this.ShouldSpawnCamera(transform4))
				{
					this.SpawnCamera(grabber3, transform4);
				}
			}
			else
			{
				this.ResetCameraModel();
			}
		}
		if (!this.IsSlingshotActiveInHierarchy())
		{
			this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
			return;
		}
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x000766C0 File Offset: 0x000748C0
	private void OnZoneChanged(ZoneData[] zones)
	{
		if (!this._tabletSpawnInstance.isSpawned || this._tabletSpawnInstance.directGrabbable.isGrabbed)
		{
			return;
		}
		if (Vector3.Distance(this._tabletSpawnInstance.Controller.transform.position, base.transform.position) > 6f)
		{
			this.ManuallySetCameraOnNeck();
		}
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x0007671F File Offset: 0x0007491F
	private void OnDestroy()
	{
		this._tabletSpawnInstance.Dispose();
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x0007672C File Offset: 0x0007492C
	[ContextMenu("Put tablet on neck")]
	public void ManuallySetCameraOnNeck()
	{
		if (this.cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck || this.cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled)
		{
			return;
		}
		if (this._tabletSpawnInstance.isSpawned)
		{
			this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
			this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
			this._tabletSpawnInstance.ResetLocalPose();
			this._shouldMoveCameraToNeck = false;
			this._previousMode = this._tabletSpawnInstance.Controller.CurrentCameraMode;
			if (this._previousMode == null)
			{
				this._tabletSpawnInstance.Controller.SetCameraMode(1);
			}
		}
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x000767B3 File Offset: 0x000749B3
	private void OnCameraModelReleased()
	{
		this._cameraModelGrabbable.onReleased -= new Action(this.OnCameraModelReleased);
		this.ResetCameraModel();
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x000767D4 File Offset: 0x000749D4
	public void SpawnCamera(GorillaGrabber overrideGorillaGrabber, Transform transform)
	{
		if (!this._tabletSpawnInstance.isSpawned)
		{
			this._tabletSpawnInstance.SpawnCamera();
		}
		if (this._previousMode == null)
		{
			this._tabletSpawnInstance.Controller.SetCameraMode(0);
			this._previousMode = 0;
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraSpawned;
		this._cameraModelGrabbable.ForceRelease();
		this._tabletSpawnInstance.ResetParent();
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		vector2 = this._rotationOffsetWindows;
		XRNode xrNode = overrideGorillaGrabber.XrNode;
		if (xrNode != 4)
		{
			if (xrNode == 5)
			{
				vector = this._rightHandSpawnOffsetWindows;
				vector2.z = -12f;
			}
		}
		else
		{
			vector = this._leftHandSpawnOffsetWindows;
			vector2.z = 12f;
		}
		if (!GTPlayer.Instance.IsDefaultScale)
		{
			vector *= 0.06f;
		}
		vector = transform.rotation * vector;
		this._tabletSpawnInstance.SetPositionAndRotation(transform.position + vector, transform.rotation * Quaternion.Euler(vector2));
		this._tabletSpawnInstance.directGrabbable.ForceGrab(overrideGorillaGrabber);
		this._tabletSpawnInstance.SetLocalScale(Vector3.one);
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x000768F4 File Offset: 0x00074AF4
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 vector = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 vector2 = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(vector - vector2) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x0007694C File Offset: 0x00074B4C
	private void ChangeCameraModelParent(Transform transform)
	{
		if (this._cameraModelTransform != null)
		{
			this._cameraModelGrabbable.SetOriginalTargetParent(transform);
			if (!this._cameraModelGrabbable.isGrabbed)
			{
				this._cameraModelTransform.transform.parent = transform;
				this._cameraModelTransform.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x000769A6 File Offset: 0x00074BA6
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x000769D0 File Offset: 0x00074BD0
	private void UpdateCameraStrap()
	{
		for (int i = 0; i < this._cameraStrapPoints.Length; i++)
		{
			this._cameraStrapPositions[i] = this._cameraStrapPoints[i].position;
		}
		this._cameraStrapRenderer.SetPositions(this._cameraStrapPositions);
		Vector3 lossyScale = base.transform.lossyScale;
		float num = (lossyScale.x + lossyScale.y + lossyScale.z) * 0.3333333f;
		this._cameraStrapRenderer.widthMultiplier = num * 0.02f;
		Color color = (this.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned) ? this._ghostColor : this._normalColor;
		this._cameraStrapRenderer.startColor = color;
		this._cameraStrapRenderer.endColor = color;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x00076A83 File Offset: 0x00074C83
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x00076AA5 File Offset: 0x00074CA5
	private VRRig GetLocalRig()
	{
		if (this._localRig == null)
		{
			this._localRig = VRRigCache.Instance.localRig.Rig;
		}
		return this._localRig;
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x00076AD0 File Offset: 0x00074CD0
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig localRig = this.GetLocalRig();
		if (localRig == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = localRig.projectileWeapon.InLeftHand();
		rightHand = localRig.projectileWeapon.InRightHand();
		return localRig.projectileWeapon.InHand();
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x00076B1C File Offset: 0x00074D1C
	private bool IsSlingshotActiveInHierarchy()
	{
		VRRig localRig = this.GetLocalRig();
		return !(localRig == null) && !(localRig.projectileWeapon == null) && localRig.projectileWeapon.gameObject.activeInHierarchy;
	}

	// Token: 0x04001F3B RID: 7995
	[SerializeField]
	private GameObject _cameraSpawnPrefab;

	// Token: 0x04001F3C RID: 7996
	[SerializeField]
	private Transform _cameraSpawnParentTransform;

	// Token: 0x04001F3D RID: 7997
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x04001F3E RID: 7998
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x04001F3F RID: 7999
	[SerializeField]
	private LckDirectGrabbable _cameraModelGrabbable;

	// Token: 0x04001F40 RID: 8000
	[SerializeField]
	private Transform _cameraPositionDefault;

	// Token: 0x04001F41 RID: 8001
	[SerializeField]
	private Transform _cameraPositionSlingshot;

	// Token: 0x04001F42 RID: 8002
	private Vector3 _chestSpawnRotationOffset = new Vector3(90f, 0f, 0f);

	// Token: 0x04001F43 RID: 8003
	private Vector3 _rightHandSpawnOffsetAndroid = new Vector3(-0.265f, 0.02f, -0.065f);

	// Token: 0x04001F44 RID: 8004
	private Vector3 _leftHandSpawnOffsetAndroid = new Vector3(0.245f, 0.022f, -0.12f);

	// Token: 0x04001F45 RID: 8005
	private Vector3 _rotationOffsetAndroid = new Vector3(-90f, 60f, 125f);

	// Token: 0x04001F46 RID: 8006
	private Vector3 _rotationOffsetWindows = new Vector3(-70f, -180f, 0f);

	// Token: 0x04001F47 RID: 8007
	private Vector3 _rightHandSpawnOffsetWindows = new Vector3(-0.23f, -0.035f, -0.225f);

	// Token: 0x04001F48 RID: 8008
	private Vector3 _leftHandSpawnOffsetWindows = new Vector3(0.23f, -0.035f, -0.225f);

	// Token: 0x04001F49 RID: 8009
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x04001F4A RID: 8010
	[SerializeField]
	private float _snapToNeckDistance = 15f;

	// Token: 0x04001F4B RID: 8011
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x04001F4C RID: 8012
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x04001F4D RID: 8013
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x04001F4E RID: 8014
	[SerializeField]
	private Color _ghostColor = Color.gray;

	// Token: 0x04001F4F RID: 8015
	[Header("Cosmetics References")]
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x04001F50 RID: 8016
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapTablet;

	// Token: 0x04001F51 RID: 8017
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapEmobi;

	// Token: 0x04001F52 RID: 8018
	private Transform _followTransform;

	// Token: 0x04001F53 RID: 8019
	private Vector3[] _cameraStrapPositions;

	// Token: 0x04001F54 RID: 8020
	private TabletSpawnInstance _tabletSpawnInstance;

	// Token: 0x04001F55 RID: 8021
	private VRRig _localRig;

	// Token: 0x04001F56 RID: 8022
	private bool _shouldMoveCameraToNeck;

	// Token: 0x04001F57 RID: 8023
	private CameraMode _previousMode;

	// Token: 0x04001F59 RID: 8025
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x04001F5A RID: 8026
	private LckBodyCameraSpawner.CameraPosition _cameraPosition;

	// Token: 0x02000366 RID: 870
	public enum CameraState
	{
		// Token: 0x04001F5C RID: 8028
		CameraDisabled,
		// Token: 0x04001F5D RID: 8029
		CameraOnNeck,
		// Token: 0x04001F5E RID: 8030
		CameraSpawned
	}

	// Token: 0x02000367 RID: 871
	public enum CameraPosition
	{
		// Token: 0x04001F60 RID: 8032
		CameraDefault,
		// Token: 0x04001F61 RID: 8033
		CameraSlingshot,
		// Token: 0x04001F62 RID: 8034
		NotVisible
	}

	// Token: 0x02000368 RID: 872
	// (Invoke) Token: 0x060014C8 RID: 5320
	public delegate void CameraStateDelegate(LckBodyCameraSpawner.CameraState state);
}
