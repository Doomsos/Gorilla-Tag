using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using GorillaTag;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200037E RID: 894
[NetworkBehaviourWeaved(1)]
public class LckSocialCamera : NetworkComponent, IGorillaSliceableSimple
{
	// Token: 0x1700020D RID: 525
	// (get) Token: 0x0600152D RID: 5421 RVA: 0x000780D5 File Offset: 0x000762D5
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe ref LckSocialCamera.CameraData _networkedData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing LckSocialCamera._networkedData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ref *(LckSocialCamera.CameraData*)(this.Ptr + 0);
		}
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x0600152E RID: 5422 RVA: 0x000780FA File Offset: 0x000762FA
	// (set) Token: 0x0600152F RID: 5423 RVA: 0x00078102 File Offset: 0x00076302
	public LCKSocialCameraFollower SocialCameraFollower { get; private set; }

	// Token: 0x06001530 RID: 5424 RVA: 0x0007810C File Offset: 0x0007630C
	public override void OnSpawned()
	{
		if (base.IsLocallyOwned)
		{
			this._localState = LckSocialCamera.CameraState.Empty;
			this.visible = false;
			this.recording = false;
			this.IsOnNeck = false;
			return;
		}
		if (base.Runner != null)
		{
			LckSocialCamera.CameraState currentState = this._networkedData.currentState;
			this.ApplyVisualState(currentState);
			this._previousRenderedState = currentState;
		}
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x00078166 File Offset: 0x00076366
	public unsafe override void WriteDataFusion()
	{
		*this._networkedData = new LckSocialCamera.CameraData(this._localState);
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x0007817E File Offset: 0x0007637E
	public override void ReadDataFusion()
	{
		if (this.m_isCorrupted)
		{
			return;
		}
		this.ReadDataShared(this._networkedData.currentState);
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x0007819A File Offset: 0x0007639A
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this._localState);
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x000781B0 File Offset: 0x000763B0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner || this.m_isCorrupted)
		{
			return;
		}
		LckSocialCamera.CameraState newState = (LckSocialCamera.CameraState)stream.ReceiveNext();
		this.ReadDataShared(newState);
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x000781EC File Offset: 0x000763EC
	private void ReadDataShared(LckSocialCamera.CameraState newState)
	{
		if (newState != this._previousRenderedState)
		{
			this.ApplyVisualState(newState);
			this._previousRenderedState = newState;
		}
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06001536 RID: 5430 RVA: 0x00078205 File Offset: 0x00076405
	// (set) Token: 0x06001537 RID: 5431 RVA: 0x00078223 File Offset: 0x00076423
	public bool IsOnNeck
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localState : this._previousRenderedState, LckSocialCamera.CameraState.OnNeck);
		}
		set
		{
			if (base.IsLocallyOwned)
			{
				this._localState = LckSocialCamera.SetFlag(this._localState, LckSocialCamera.CameraState.OnNeck, value);
			}
		}
	}

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06001538 RID: 5432 RVA: 0x00078240 File Offset: 0x00076440
	// (set) Token: 0x06001539 RID: 5433 RVA: 0x0007825E File Offset: 0x0007645E
	public bool visible
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localState : this._previousRenderedState, LckSocialCamera.CameraState.Visible);
		}
		set
		{
			if (base.IsLocallyOwned)
			{
				this._localState = LckSocialCamera.SetFlag(this._localState, LckSocialCamera.CameraState.Visible, value);
			}
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x0600153A RID: 5434 RVA: 0x0007827B File Offset: 0x0007647B
	// (set) Token: 0x0600153B RID: 5435 RVA: 0x00078299 File Offset: 0x00076499
	public bool recording
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localState : this._previousRenderedState, LckSocialCamera.CameraState.Recording);
		}
		set
		{
			if (base.IsLocallyOwned)
			{
				this._localState = LckSocialCamera.SetFlag(this._localState, LckSocialCamera.CameraState.Recording, value);
			}
		}
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x000782B8 File Offset: 0x000764B8
	private void ApplyVisualState(LckSocialCamera.CameraState newState)
	{
		if (this.m_isCorrupted)
		{
			return;
		}
		bool flag = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.Visible);
		bool flag2 = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.Recording);
		bool flag3 = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.OnNeck);
		if (!base.IsLocallyOwned)
		{
			IGtCameraVisuals cameraVisuals = this.m_CameraVisuals;
			if (cameraVisuals != null)
			{
				cameraVisuals.SetNetworkedVisualsActive(flag);
			}
			IGtCameraVisuals cameraVisuals2 = this.m_CameraVisuals;
			if (cameraVisuals2 != null)
			{
				cameraVisuals2.SetRecordingState(flag2);
			}
			if (this.m_cameraType == LckSocialCamera.CameraType.Tablet)
			{
				if (flag3)
				{
					this.SocialCameraFollower.SetParentToRig();
					return;
				}
				this.SocialCameraFollower.SetParentNull();
			}
			return;
		}
		IGtCameraVisuals cameraVisuals3 = this.m_CameraVisuals;
		if (cameraVisuals3 != null)
		{
			cameraVisuals3.SetVisualsActive(false);
		}
		IGtCameraVisuals cameraVisuals4 = this.m_CameraVisuals;
		if (cameraVisuals4 == null)
		{
			return;
		}
		cameraVisuals4.SetRecordingState(false);
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x00078359 File Offset: 0x00076559
	private static bool GetFlag(LckSocialCamera.CameraState currentState, LckSocialCamera.CameraState flag)
	{
		return currentState.HasFlag(flag);
	}

	// Token: 0x0600153E RID: 5438 RVA: 0x0007836C File Offset: 0x0007656C
	private static LckSocialCamera.CameraState SetFlag(LckSocialCamera.CameraState currentState, LckSocialCamera.CameraState flag, bool shouldBeSet)
	{
		if (shouldBeSet)
		{
			return currentState | flag;
		}
		return currentState & ~flag;
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x0007837C File Offset: 0x0007657C
	protected override void Awake()
	{
		base.Awake();
		if (this.CameraVisuals != null && !this.CameraVisuals.TryGetComponent<IGtCameraVisuals>(ref this.m_CameraVisuals))
		{
			Debug.LogError("LCK: LckSocialCamera failed to find IGtCameraVisuals component on CameraVisuals");
		}
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			return;
		}
		ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccesfullSpawn);
		succesfullSpawnEvent.Add(inAction);
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x00078400 File Offset: 0x00076600
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.m_lckDelegateRegistered)
		{
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Remove(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
		}
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00078430 File Offset: 0x00076630
	public unsafe void SetVisibility(bool isVisible)
	{
		LckSocialCamera.CameraData cameraData = *this._networkedData;
		cameraData.currentState = LckSocialCamera.SetFlag(cameraData.currentState, LckSocialCamera.CameraState.Visible, isVisible);
		*this._networkedData = cameraData;
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x0007846C File Offset: 0x0007666C
	private void OnSuccesfullSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		this._vrrig = rig.Rig;
		LCKSocialCameraFollower lcksocialCameraFollower = (this.m_cameraType == LckSocialCamera.CameraType.Cococam) ? rig.LckCococamFollower : rig.LCKTabletFollower;
		this._scaleTransform = lcksocialCameraFollower.ScaleTransform;
		this.CameraVisuals = lcksocialCameraFollower.CameraVisualsRoot;
		this.m_CameraVisuals = this.CameraVisuals.GetComponent<IGtCameraVisuals>();
		if (!base.IsLocallyOwned && lcksocialCameraFollower.GetComponent<ILckCosmeticDependantPlayerIdSupplier>() != null)
		{
			lcksocialCameraFollower.GetComponent<ILckCosmeticDependantPlayerIdSupplier>().UpdatePlayerId();
		}
		this.SocialCameraFollower = lcksocialCameraFollower;
		this.m_isCorrupted = false;
		if (!this._vrrig.isOfflineVRRig)
		{
			lcksocialCameraFollower.SetNetworkController(this);
			return;
		}
		LckSocialCameraManager instance = LckSocialCameraManager.Instance;
		if (!(instance != null))
		{
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Combine(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
			this.m_lckDelegateRegistered = true;
			return;
		}
		LckSocialCamera.CameraType cameraType = this.m_cameraType;
		if (cameraType == LckSocialCamera.CameraType.Cococam)
		{
			instance.SetLckSocialCococamCamera(this);
			return;
		}
		if (cameraType != LckSocialCamera.CameraType.Tablet)
		{
			throw new ArgumentOutOfRangeException();
		}
		instance.SetLckSocialTabletCamera(this);
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x00078562 File Offset: 0x00076762
	public void SliceUpdate()
	{
		if (this._vrrig.IsNull())
		{
			return;
		}
		this.CameraVisuals.transform.localScale = Vector3.one * this._vrrig.scaleFactor;
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x00078597 File Offset: 0x00076797
	public new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000785AC File Offset: 0x000767AC
	public new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.m_isCorrupted)
		{
			return;
		}
		if (this.SocialCameraFollower.IsNotNull())
		{
			this.SocialCameraFollower.RemoveNetworkController(this);
		}
		this._scaleTransform = null;
		this.CameraVisuals = null;
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000785FC File Offset: 0x000767FC
	private void OnManagerSpawned(LckSocialCameraManager manager)
	{
		LckSocialCamera.CameraType cameraType = this.m_cameraType;
		if (cameraType == LckSocialCamera.CameraType.Cococam)
		{
			manager.SetLckSocialCococamCamera(this);
			return;
		}
		if (cameraType != LckSocialCamera.CameraType.Tablet)
		{
			throw new ArgumentOutOfRangeException();
		}
		manager.SetLckSocialTabletCamera(this);
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x0007862E File Offset: 0x0007682E
	public void TurnOff()
	{
		this.m_isCorrupted = true;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00078652 File Offset: 0x00076852
	[WeaverGenerated]
	public unsafe override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		*this._networkedData = this.__networkedData;
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x0007866F File Offset: 0x0007686F
	[WeaverGenerated]
	public unsafe override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this.__networkedData = *this._networkedData;
	}

	// Token: 0x04001FB8 RID: 8120
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x04001FB9 RID: 8121
	[SerializeField]
	public GameObject CameraVisuals;

	// Token: 0x04001FBA RID: 8122
	[SerializeField]
	private VRRig _vrrig;

	// Token: 0x04001FBB RID: 8123
	[SerializeField]
	private VRRigSerializer m_rigNetworkController;

	// Token: 0x04001FBC RID: 8124
	[SerializeField]
	private LckSocialCamera.CameraType m_cameraType;

	// Token: 0x04001FBE RID: 8126
	private bool m_isCorrupted = true;

	// Token: 0x04001FBF RID: 8127
	private bool m_lckDelegateRegistered;

	// Token: 0x04001FC0 RID: 8128
	private IGtCameraVisuals m_CameraVisuals;

	// Token: 0x04001FC1 RID: 8129
	private LckSocialCamera.CameraState _localState;

	// Token: 0x04001FC2 RID: 8130
	private LckSocialCamera.CameraState _previousRenderedState;

	// Token: 0x04001FC3 RID: 8131
	[WeaverGenerated]
	[DefaultForProperty("_networkedData", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private LckSocialCamera.CameraData __networkedData;

	// Token: 0x0200037F RID: 895
	private enum CameraState
	{
		// Token: 0x04001FC5 RID: 8133
		Empty,
		// Token: 0x04001FC6 RID: 8134
		Visible,
		// Token: 0x04001FC7 RID: 8135
		Recording,
		// Token: 0x04001FC8 RID: 8136
		OnNeck = 4
	}

	// Token: 0x02000380 RID: 896
	private enum CameraType
	{
		// Token: 0x04001FCA RID: 8138
		Cococam,
		// Token: 0x04001FCB RID: 8139
		Tablet
	}

	// Token: 0x02000381 RID: 897
	[NetworkStructWeaved(1)]
	[StructLayout(2, Size = 4)]
	private struct CameraData : INetworkStruct
	{
		// Token: 0x0600154B RID: 5451 RVA: 0x00078688 File Offset: 0x00076888
		public CameraData(LckSocialCamera.CameraState state)
		{
			this.currentState = state;
		}

		// Token: 0x04001FCC RID: 8140
		[FieldOffset(0)]
		public LckSocialCamera.CameraState currentState;
	}
}
