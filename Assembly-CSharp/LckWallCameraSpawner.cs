using System;
using System.Collections;
using GorillaLocomotion;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000385 RID: 901
public class LckWallCameraSpawner : MonoBehaviour
{
	// Token: 0x06001579 RID: 5497 RVA: 0x00079190 File Offset: 0x00077390
	private LckBodyCameraSpawner GetOrCreateBodyCameraSpawner()
	{
		if (LckWallCameraSpawner._bodySpawner != null)
		{
			return LckWallCameraSpawner._bodySpawner;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("Unable to find Player!");
			return null;
		}
		LckWallCameraSpawner.AddGTag(Camera.main.gameObject, 1);
		LckWallCameraSpawner.AddGTag(instance.gameObject, 0);
		Transform transform = instance.bodyCollider.transform;
		GameObject gameObject = Object.Instantiate<GameObject>(this._lckBodySpawnerPrefab, transform.parent);
		Transform transform2 = gameObject.transform;
		transform2.localPosition = Vector3.zero;
		transform2.localRotation = Quaternion.identity;
		transform2.localScale = Vector3.one;
		LckWallCameraSpawner._bodySpawner = gameObject.GetComponent<LckBodyCameraSpawner>();
		LckWallCameraSpawner._bodySpawner.SetFollowTransform(transform);
		GorillaTagger instance2 = GorillaTagger.Instance;
		if (instance2 != null)
		{
			LckWallCameraSpawner.AddGTag(instance2.leftHandTriggerCollider, 2);
			LckWallCameraSpawner.AddGTag(instance2.rightHandTriggerCollider, 3);
		}
		else
		{
			Debug.LogError("Unable to find GorillaTagger!");
		}
		return LckWallCameraSpawner._bodySpawner;
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x00079277 File Offset: 0x00077477
	private static void AddGTag(GameObject go, GtTagType gtTagType)
	{
		if (go.GetComponent<GtTag>())
		{
			return;
		}
		GtTag gtTag = go.AddComponent<GtTag>();
		gtTag.gtTagType = gtTagType;
		gtTag.enabled = true;
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x0600157B RID: 5499 RVA: 0x0007929A File Offset: 0x0007749A
	// (set) Token: 0x0600157C RID: 5500 RVA: 0x000792A4 File Offset: 0x000774A4
	public LckWallCameraSpawner.WallSpawnerState wallSpawnerState
	{
		get
		{
			return this._wallSpawnerState;
		}
		set
		{
			switch (value)
			{
			case LckWallCameraSpawner.WallSpawnerState.CameraOnHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			case LckWallCameraSpawner.WallSpawnerState.CameraOffHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			}
			this._wallSpawnerState = value;
		}
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000792F4 File Offset: 0x000774F4
	private void Awake()
	{
		this.InitCameraStrap();
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x000792FC File Offset: 0x000774FC
	private void OnEnable()
	{
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
		this._cameraHandleGrabbable.onGrabbed += new Action(this.OnGrabbed);
		this._cameraHandleGrabbable.onReleased += new Action(this.OnReleased);
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x000793C0 File Offset: 0x000775C0
	private void Start()
	{
		this.CreatePrewarmCamera();
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x000793C8 File Offset: 0x000775C8
	private void Update()
	{
		LckWallCameraSpawner.WallSpawnerState wallSpawnerState = this._wallSpawnerState;
		if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraOnHook)
		{
			if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraDragging)
			{
				return;
			}
			this.UpdateCameraStrap();
			if (this.ShouldSpawnCamera(this._cameraHandleGrabbable.grabber.transform))
			{
				this.SpawnCamera(this._cameraHandleGrabbable.grabber);
			}
		}
		else
		{
			if (this.GetOrCreateBodyCameraSpawner() == null)
			{
				Debug.LogError("Lck, Unable to find LckBodyCameraSpawner");
				base.gameObject.SetActive(false);
				return;
			}
			if (LckWallCameraSpawner._bodySpawner.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.isSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable.isGrabbed)
			{
				LckDirectGrabbable directGrabbable = LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable;
				GorillaGrabber grabber = directGrabbable.grabber;
				if (!this.ShouldSpawnCamera(grabber.transform))
				{
					directGrabbable.ForceRelease();
					LckWallCameraSpawner._bodySpawner.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
					this._cameraHandleGrabbable.target.SetPositionAndRotation(grabber.transform.position, grabber.transform.rotation * Quaternion.Euler(this._spawnRotationOffsetWindows, 180f, 0f));
					this._cameraHandleGrabbable.ForceGrab(grabber);
					return;
				}
			}
		}
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x00079500 File Offset: 0x00077700
	private void OnDisable()
	{
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
		this._cameraHandleGrabbable.onGrabbed -= new Action(this.OnGrabbed);
		this._cameraHandleGrabbable.onReleased -= new Action(this.OnReleased);
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06001582 RID: 5506 RVA: 0x000795BD File Offset: 0x000777BD
	// (set) Token: 0x06001583 RID: 5507 RVA: 0x000795CF File Offset: 0x000777CF
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.gameObject.SetActive(value);
		}
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000795F4 File Offset: 0x000777F4
	private void SpawnCamera(GorillaGrabber lastGorillaGrabber)
	{
		if (LckWallCameraSpawner._bodySpawner == null)
		{
			Debug.LogError("Lck, unable to spawn camera, body spawner is null!");
			return;
		}
		if (LckWallCameraSpawner._bodySpawner.tabletSpawnInstance != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor())
		{
			LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor().ResetToDefaultAndTriggerButton();
			LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.ClearAllTriggers();
		}
		this.cameraVisible = false;
		this._cameraHandleGrabbable.ForceRelease();
		LckWallCameraSpawner._bodySpawner.SpawnCamera(lastGorillaGrabber, lastGorillaGrabber.transform);
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000796D5 File Offset: 0x000778D5
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x00079700 File Offset: 0x00077900
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
		this._cameraStrapRenderer.startColor = (this._cameraStrapRenderer.endColor = this._normalColor);
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x000797A2 File Offset: 0x000779A2
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x000797C4 File Offset: 0x000779C4
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 vector = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 vector2 = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(vector - vector2) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x0007981A File Offset: 0x00077A1A
	private void OnGrabbed()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraDragging;
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x00079823 File Offset: 0x00077A23
	private void OnReleased()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x0007982C File Offset: 0x00077A2C
	private void CreatePrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("prewarm camera");
		gameObject.transform.SetParent(base.transform);
		LckWallCameraSpawner._prewarmCamera = gameObject.AddComponent<Camera>();
		Camera main = Camera.main;
		LckWallCameraSpawner._prewarmCamera.clearFlags = main.clearFlags;
		LckWallCameraSpawner._prewarmCamera.fieldOfView = main.fieldOfView;
		LckWallCameraSpawner._prewarmCamera.nearClipPlane = main.nearClipPlane;
		LckWallCameraSpawner._prewarmCamera.farClipPlane = main.farClipPlane;
		LckWallCameraSpawner._prewarmCamera.cullingMask = main.cullingMask;
		LckWallCameraSpawner._prewarmCamera.tag = "Untagged";
		LckWallCameraSpawner._prewarmCamera.stereoTargetEye = 0;
		LckWallCameraSpawner._prewarmCamera.targetTexture = new RenderTexture(32, 32, 8, 94);
		LckWallCameraSpawner._prewarmCamera.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
		base.StartCoroutine(this.DestroyPrewarmCameraDelayed());
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x00079924 File Offset: 0x00077B24
	private IEnumerator DestroyPrewarmCameraDelayed()
	{
		yield return new WaitForSeconds(1f);
		this.DestroyPrewarmCamera();
		yield break;
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x00079933 File Offset: 0x00077B33
	private void DestroyPrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera == null)
		{
			return;
		}
		RenderTexture targetTexture = LckWallCameraSpawner._prewarmCamera.targetTexture;
		LckWallCameraSpawner._prewarmCamera.targetTexture = null;
		targetTexture.Release();
		Object.Destroy(LckWallCameraSpawner._prewarmCamera.gameObject);
		LckWallCameraSpawner._prewarmCamera = null;
	}

	// Token: 0x04001FEF RID: 8175
	[SerializeField]
	private GameObject _lckBodySpawnerPrefab;

	// Token: 0x04001FF0 RID: 8176
	[SerializeField]
	private LckDirectGrabbable _cameraHandleGrabbable;

	// Token: 0x04001FF1 RID: 8177
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x04001FF2 RID: 8178
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x04001FF3 RID: 8179
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x04001FF4 RID: 8180
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x04001FF5 RID: 8181
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x04001FF6 RID: 8182
	private Vector3[] _cameraStrapPositions;

	// Token: 0x04001FF7 RID: 8183
	private float _spawnRotationOffsetAndroid = -80f;

	// Token: 0x04001FF8 RID: 8184
	private float _spawnRotationOffsetWindows = -55f;

	// Token: 0x04001FF9 RID: 8185
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x04001FFA RID: 8186
	[Header("Cosmetics References")]
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x04001FFB RID: 8187
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapTablet;

	// Token: 0x04001FFC RID: 8188
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapEmobi;

	// Token: 0x04001FFD RID: 8189
	private static LckBodyCameraSpawner _bodySpawner;

	// Token: 0x04001FFE RID: 8190
	private static Camera _prewarmCamera;

	// Token: 0x04001FFF RID: 8191
	private LckWallCameraSpawner.WallSpawnerState _wallSpawnerState;

	// Token: 0x02000386 RID: 902
	public enum WallSpawnerState
	{
		// Token: 0x04002001 RID: 8193
		CameraOnHook,
		// Token: 0x04002002 RID: 8194
		CameraDragging,
		// Token: 0x04002003 RID: 8195
		CameraOffHook
	}
}
