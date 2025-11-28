using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000382 RID: 898
public class LCKSocialCameraFollower : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000212 RID: 530
	// (get) Token: 0x0600154C RID: 5452 RVA: 0x00078691 File Offset: 0x00076891
	public Transform ScaleTransform
	{
		get
		{
			return this._scaleTransform;
		}
	}

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x0600154D RID: 5453 RVA: 0x00078699 File Offset: 0x00076899
	public GameObject CameraVisualsRoot
	{
		get
		{
			return this._cameraVisualsRoot;
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x0600154E RID: 5454 RVA: 0x000786A1 File Offset: 0x000768A1
	public List<GameObject> VisualObjects
	{
		get
		{
			return this._visualObjects;
		}
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x000786AC File Offset: 0x000768AC
	private void Awake()
	{
		this._initialScale = base.transform.localScale;
		this.m_gtCameraVisuals = this._cameraVisualsRoot.GetComponent<IGtCameraVisuals>();
		if (this.m_rigContainer.Rig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			return;
		}
		ListProcessor<Action<RigContainer>> disableEvent = this.m_rigContainer.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.PreRigDisable);
		disableEvent.Add(action);
		ListProcessor<Action<RigContainer>> enableEvent = this.m_rigContainer.RigEvents.enableEvent;
		action = new Action<RigContainer>(this.PostRigEnable);
		enableEvent.Add(action);
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x00078742 File Offset: 0x00076942
	private void Start()
	{
		if (!this.isParentedToRig)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x00078758 File Offset: 0x00076958
	public void SetParentToRig()
	{
		this.isParentedToRig = true;
		base.transform.parent = this.m_rigContainer.transform;
		base.transform.localPosition = new Vector3(0f, -0.2f, 0.132f);
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = this._initialScale * 0.3f;
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x000787CC File Offset: 0x000769CC
	public void SetParentNull()
	{
		this.isParentedToRig = false;
		base.transform.parent = null;
		base.transform.localScale = this._initialScale;
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x000787F2 File Offset: 0x000769F2
	private void PostRigEnable(RigContainer _)
	{
		base.gameObject.SetActive(true);
		this.m_gtCameraVisuals.SetNetworkedVisualsActive(false);
		this.m_gtCameraVisuals.SetRecordingState(false);
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x000396A0 File Offset: 0x000378A0
	private void PreRigDisable(RigContainer _)
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x00078818 File Offset: 0x00076A18
	public void SetNetworkController(LckSocialCamera networkController)
	{
		if (this.m_networkController.IsNotNull() && this.m_networkController != networkController)
		{
			this.m_networkController.TurnOff();
		}
		this.m_networkController = networkController;
		this.m_transformToFollow = this.m_networkController.transform;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x00078869 File Offset: 0x00076A69
	public void RemoveNetworkController(LckSocialCamera networkController)
	{
		if (this.m_networkController != networkController)
		{
			return;
		}
		this.m_transformToFollow = null;
		this.m_networkController = null;
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06001557 RID: 5463 RVA: 0x0007888E File Offset: 0x00076A8E
	// (set) Token: 0x06001558 RID: 5464 RVA: 0x00078896 File Offset: 0x00076A96
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06001559 RID: 5465 RVA: 0x0007889F File Offset: 0x00076A9F
	void ITickSystemTick.Tick()
	{
		if (!this.isParentedToRig)
		{
			base.transform.position = this.m_transformToFollow.position;
			base.transform.root.rotation = this.m_transformToFollow.rotation;
		}
	}

	// Token: 0x04001FCD RID: 8141
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x04001FCE RID: 8142
	[FormerlySerializedAs("_coconutCamera")]
	[SerializeField]
	private GameObject _cameraVisualsRoot;

	// Token: 0x04001FCF RID: 8143
	[SerializeField]
	private List<GameObject> _visualObjects;

	// Token: 0x04001FD0 RID: 8144
	[SerializeField]
	private RigContainer m_rigContainer;

	// Token: 0x04001FD1 RID: 8145
	private Transform m_transformToFollow;

	// Token: 0x04001FD2 RID: 8146
	private LckSocialCamera m_networkController;

	// Token: 0x04001FD3 RID: 8147
	private IGtCameraVisuals m_gtCameraVisuals;

	// Token: 0x04001FD4 RID: 8148
	private Vector3 _initialScale = Vector3.one;

	// Token: 0x04001FD5 RID: 8149
	private bool isParentedToRig;
}
