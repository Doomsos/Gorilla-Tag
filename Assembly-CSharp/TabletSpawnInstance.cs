using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000364 RID: 868
public class TabletSpawnInstance : IDisposable
{
	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06001494 RID: 5268 RVA: 0x00075BD8 File Offset: 0x00073DD8
	// (remove) Token: 0x06001495 RID: 5269 RVA: 0x00075C10 File Offset: 0x00073E10
	public event Action onGrabbed;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06001496 RID: 5270 RVA: 0x00075C48 File Offset: 0x00073E48
	// (remove) Token: 0x06001497 RID: 5271 RVA: 0x00075C80 File Offset: 0x00073E80
	public event Action onReleased;

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06001498 RID: 5272 RVA: 0x00075CB5 File Offset: 0x00073EB5
	public LckDirectGrabbable directGrabbable
	{
		get
		{
			return this._lckSocialCameraManager.lckDirectGrabbable;
		}
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x00075CC2 File Offset: 0x00073EC2
	public bool ResetLocalPose()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.localPosition = Vector3.zero;
		this._cameraSpawnInstanceTransform.localRotation = Quaternion.identity;
		return true;
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x00075CF5 File Offset: 0x00073EF5
	public bool ResetParent()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(this._cameraSpawnParentTransform);
		return true;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x00075D19 File Offset: 0x00073F19
	public bool SetParent(Transform transform)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(transform);
		return true;
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x0600149C RID: 5276 RVA: 0x00075D38 File Offset: 0x00073F38
	// (set) Token: 0x0600149D RID: 5277 RVA: 0x00075D40 File Offset: 0x00073F40
	public bool cameraActive
	{
		get
		{
			return this._cameraActive;
		}
		set
		{
			this._cameraActive = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.cameraActive = this._cameraActive;
			}
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x0600149E RID: 5278 RVA: 0x00075D68 File Offset: 0x00073F68
	// (set) Token: 0x0600149F RID: 5279 RVA: 0x00075D70 File Offset: 0x00073F70
	public bool uiVisible
	{
		get
		{
			return this._uiVisible;
		}
		set
		{
			this._uiVisible = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.uiVisible = this._uiVisible;
			}
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060014A0 RID: 5280 RVA: 0x00075D98 File Offset: 0x00073F98
	public bool isSpawned
	{
		get
		{
			return this._cameraGameObjectInstance != null;
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x00075DA6 File Offset: 0x00073FA6
	public TabletSpawnInstance(GameObject cameraSpawnPrefab, Transform cameraSpawnParentTransform)
	{
		this._cameraSpawnPrefab = cameraSpawnPrefab;
		this._cameraSpawnParentTransform = cameraSpawnParentTransform;
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x00075DBC File Offset: 0x00073FBC
	public void SpawnCamera()
	{
		if (!this.isSpawned)
		{
			this._cameraGameObjectInstance = Object.Instantiate<GameObject>(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
			this._lckSocialCameraManager = this._cameraGameObjectInstance.GetComponent<LckSocialCameraManager>();
			this._lckSocialCameraManager.lckDirectGrabbable.onGrabbed += delegate()
			{
				Action action = this.onGrabbed;
				if (action == null)
				{
					return;
				}
				action.Invoke();
			};
			this._lckSocialCameraManager.lckDirectGrabbable.onReleased += delegate()
			{
				Action action = this.onReleased;
				if (action == null)
				{
					return;
				}
				action.Invoke();
			};
			this._cameraSpawnInstanceTransform = this._cameraGameObjectInstance.transform;
			this.Controller = this._cameraGameObjectInstance.GetComponent<GTLckController>();
		}
		this.uiVisible = this.uiVisible;
		this.cameraActive = this.cameraActive;
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060014A3 RID: 5283 RVA: 0x00075E6E File Offset: 0x0007406E
	public Vector3 position
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Vector3.zero;
			}
			return this._cameraSpawnInstanceTransform.position;
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060014A4 RID: 5284 RVA: 0x00075E8F File Offset: 0x0007408F
	public Quaternion rotation
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Quaternion.identity;
			}
			return this._cameraSpawnInstanceTransform.rotation;
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x00075EB0 File Offset: 0x000740B0
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x00075ECE File Offset: 0x000740CE
	public void SetLocalScale(Vector3 scale)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.localScale = scale;
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x00075EEB File Offset: 0x000740EB
	public void Dispose()
	{
		if (this._cameraGameObjectInstance != null)
		{
			Object.Destroy(this._cameraGameObjectInstance);
			this._cameraGameObjectInstance = null;
		}
	}

	// Token: 0x04001F32 RID: 7986
	private GameObject _cameraGameObjectInstance;

	// Token: 0x04001F33 RID: 7987
	private GameObject _cameraSpawnPrefab;

	// Token: 0x04001F34 RID: 7988
	private GameEvents _GtCamera;

	// Token: 0x04001F35 RID: 7989
	private Transform _cameraSpawnParentTransform;

	// Token: 0x04001F36 RID: 7990
	private Transform _cameraSpawnInstanceTransform;

	// Token: 0x04001F37 RID: 7991
	public GTLckController Controller;

	// Token: 0x04001F38 RID: 7992
	private LckSocialCameraManager _lckSocialCameraManager;

	// Token: 0x04001F39 RID: 7993
	private bool _cameraActive;

	// Token: 0x04001F3A RID: 7994
	private bool _uiVisible;
}
