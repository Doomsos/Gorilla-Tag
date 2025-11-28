using System;
using GorillaExtensions;
using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000408 RID: 1032
public class BalloonHoldable : TransferrableObject, IFXContext
{
	// Token: 0x0600194A RID: 6474 RVA: 0x0008787C File Offset: 0x00085A7C
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.balloonDynamics = base.GetComponent<ITetheredObjectBehavior>();
		if (this.mesh == null)
		{
			this.mesh = base.GetComponent<Renderer>();
		}
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.itemState = (TransferrableObject.ItemStates)0;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x000878D5 File Offset: 0x00085AD5
	protected override void Start()
	{
		base.Start();
		this.EnableDynamics(false, false, false);
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x000878E8 File Offset: 0x00085AE8
	internal override void OnEnable()
	{
		base.OnEnable();
		this.EnableDynamics(false, false, false);
		this.mesh.enabled = true;
		this.lineRenderer.enabled = false;
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.worldShareableInstance != null)
			{
				return;
			}
			base.SpawnTransferableObjectViews();
		}
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
		}
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x0008795A File Offset: 0x00085B5A
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.worldShareableInstance != null)
		{
			return;
		}
		base.SpawnTransferableObjectViews();
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x00087977 File Offset: 0x00085B77
	private bool ShouldSimulate()
	{
		return !base.Attached() && this.balloonState == BalloonHoldable.BalloonStates.Normal;
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x0008798C File Offset: 0x00085B8C
	internal override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false, false);
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x000879A9 File Offset: 0x00085BA9
	public override void PreDisable()
	{
		this.originalOwner = null;
		base.PreDisable();
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000879B8 File Offset: 0x00085BB8
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.balloonState = BalloonHoldable.BalloonStates.Normal;
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x000879D8 File Offset: 0x00085BD8
	protected override void OnWorldShareableItemSpawn()
	{
		WorldShareableItem worldShareableInstance = this.worldShareableInstance;
		if (worldShareableInstance != null)
		{
			worldShareableInstance.rpcCallBack = new Action(this.PopBalloonRemote);
			worldShareableInstance.onOwnerChangeCb = new WorldShareableItem.OnOwnerChangeDelegate(this.OnOwnerChangeCb);
			worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		this.originalOwner = worldShareableInstance.target.owner;
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x00087A38 File Offset: 0x00085C38
	public override void ResetToHome()
	{
		if (base.IsLocalObject() && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", 1, Array.Empty<object>());
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		this.PopBalloon();
		this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
		base.ResetToHome();
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x00087AB6 File Offset: 0x00085CB6
	protected override void PlayDestroyedOrDisabledEffect()
	{
		base.PlayDestroyedOrDisabledEffect();
		this.PlayPopBalloonFX();
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x00087AC4 File Offset: 0x00085CC4
	protected override void OnItemDestroyedOrDisabled()
	{
		this.PlayPopBalloonFX();
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
		}
		base.transform.parent = base.DefaultAnchor();
		base.OnItemDestroyedOrDisabled();
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x00087AF8 File Offset: 0x00085CF8
	private void PlayPopBalloonFX()
	{
		FXSystem.PlayFXForRig(FXType.BalloonPop, this, default(PhotonMessageInfoWrapped));
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x00087B18 File Offset: 0x00085D18
	private void EnableDynamics(bool enable, bool collider, bool forceKinematicOn = false)
	{
		bool kinematic = false;
		if (forceKinematicOn)
		{
			kinematic = true;
		}
		else if (NetworkSystem.Instance.InRoom && this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				kinematic = true;
			}
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.EnableDynamics(enable, collider, kinematic);
		}
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x00087B84 File Offset: 0x00085D84
	private void PopBalloon()
	{
		this.PlayPopBalloonFX();
		this.EnableDynamics(false, false, false);
		this.mesh.enabled = false;
		this.lineRenderer.enabled = false;
		if (this.gripInteractor != null)
		{
			this.gripInteractor.gameObject.SetActive(false);
		}
		if ((object.Equals(this.originalOwner, PhotonNetwork.LocalPlayer) || !NetworkSystem.Instance.InRoom) && NetworkSystem.Instance.InRoom && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
			this.EnableDynamics(false, false, false);
		}
		if (this.IsMyItem())
		{
			if (base.InLeftHand())
			{
				EquipmentInteractor.instance.ReleaseLeftHand();
			}
			if (base.InRightHand())
			{
				EquipmentInteractor.instance.ReleaseRightHand();
			}
		}
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x00087C7D File Offset: 0x00085E7D
	public void PopBalloonRemote()
	{
		if (this.ShouldSimulate())
		{
			this.balloonState = BalloonHoldable.BalloonStates.Pop;
		}
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x00002789 File Offset: 0x00000989
	public void OnOwnerChangeCb(NetPlayer newOwner, NetPlayer prevOwner)
	{
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x00087C90 File Offset: 0x00085E90
	public override void OnOwnershipTransferred(NetPlayer newOwner, NetPlayer prevOwner)
	{
		base.OnOwnershipTransferred(newOwner, prevOwner);
		if (object.Equals(prevOwner, NetworkSystem.Instance.LocalPlayer) && newOwner == null)
		{
			return;
		}
		if (!object.Equals(newOwner, NetworkSystem.Instance.LocalPlayer))
		{
			this.EnableDynamics(false, true, true);
			return;
		}
		if (this.ShouldSimulate() && this.balloonDynamics != null)
		{
			this.balloonDynamics.EnableDynamics(true, true, false);
		}
		if (!this.rb)
		{
			return;
		}
		if (!this.rb.isKinematic)
		{
			this.rb.AddForceAtPosition(this.forceAppliedAsRemote * this.rb.mass, this.collisionPtAsRemote, 1);
		}
		this.forceAppliedAsRemote = Vector3.zero;
		this.collisionPtAsRemote = Vector3.zero;
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x00087D50 File Offset: 0x00085F50
	private void OwnerPopBalloon()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", 1, Array.Empty<object>());
			}
		}
		this.balloonState = BalloonHoldable.BalloonStates.Pop;
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x00087D98 File Offset: 0x00085F98
	private void RunLocalPopSM()
	{
		switch (this.balloonState)
		{
		case BalloonHoldable.BalloonStates.Normal:
			break;
		case BalloonHoldable.BalloonStates.Pop:
			this.timer = Time.time;
			this.PopBalloon();
			this.balloonState = BalloonHoldable.BalloonStates.WaitForOwnershipTransfer;
			this.lastOwnershipRequest = Time.time;
			return;
		case BalloonHoldable.BalloonStates.Waiting:
			if (Time.time - this.timer >= this.poppedTimerLength)
			{
				this.timer = Time.time;
				this.mesh.enabled = true;
				this.balloonInflatSource.GTPlay();
				this.balloonState = BalloonHoldable.BalloonStates.Refilling;
				return;
			}
			base.transform.localScale = new Vector3(this.beginScale, this.beginScale, this.beginScale);
			return;
		case BalloonHoldable.BalloonStates.WaitForOwnershipTransfer:
			if (!NetworkSystem.Instance.InRoom)
			{
				this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
				base.ReDock();
				return;
			}
			if (this.worldShareableInstance != null)
			{
				WorldShareableItem worldShareableInstance = this.worldShareableInstance;
				NetPlayer owner = worldShareableInstance.Owner;
				if (worldShareableInstance != null && owner == this.originalOwner)
				{
					this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
					base.ReDock();
				}
				if (base.IsLocalObject() && this.lastOwnershipRequest + 5f < Time.time)
				{
					this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
					this.lastOwnershipRequest = Time.time;
					return;
				}
			}
			break;
		case BalloonHoldable.BalloonStates.WaitForReDock:
			if (base.Attached())
			{
				this.fullyInflatedScale = base.transform.localScale;
				base.ReDock();
				this.balloonState = BalloonHoldable.BalloonStates.Waiting;
				return;
			}
			break;
		case BalloonHoldable.BalloonStates.Refilling:
		{
			float num = Time.time - this.timer;
			if (num >= this.scaleTimerLength)
			{
				base.transform.localScale = this.fullyInflatedScale;
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
				if (this.gripInteractor != null)
				{
					this.gripInteractor.gameObject.SetActive(true);
				}
			}
			num = Mathf.Clamp01(num / this.scaleTimerLength);
			float num2 = Mathf.Lerp(this.beginScale, 1f, num);
			base.transform.localScale = this.fullyInflatedScale * num2;
			return;
		}
		case BalloonHoldable.BalloonStates.Returning:
			if (this.balloonDynamics.ReturnStep())
			{
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
				base.ReDock();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x00087FB0 File Offset: 0x000861B0
	protected override void OnStateChanged()
	{
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
			return;
		}
		if (base.OnShoulder())
		{
			if (this.balloonDynamics != null && this.balloonDynamics.IsEnabled())
			{
				this.EnableDynamics(false, false, false);
			}
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x00088010 File Offset: 0x00086210
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (Time.frameCount == this.enabledOnFrame)
		{
			this.OnStateChanged();
		}
		if (base.InHand() && this.detatchOnGrab)
		{
			float num = (this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f;
			base.transform.localScale = Vector3.one * num;
		}
		if (base.Dropped() && this.balloonState == BalloonHoldable.BalloonStates.Normal && this.maxDistanceFromOwner > 0f && (!NetworkSystem.Instance.InRoom || this.originalOwner.IsLocal) && (VRRig.LocalRig.transform.position - base.transform.position).IsLongerThan(this.maxDistanceFromOwner * base.transform.localScale.x))
		{
			this.OwnerPopBalloon();
		}
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		if (this.balloonState != BalloonHoldable.BalloonStates.Normal)
		{
			this.RunLocalPopSM();
		}
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x00088139 File Offset: 0x00086339
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x00088144 File Offset: 0x00086344
	private void Grab()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		if (this.detatchOnGrab)
		{
			float num = (this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f;
			base.transform.localScale = Vector3.one * num;
			this.EnableDynamics(true, true, false);
			this.balloonDynamics.EnableDistanceConstraints(true, num);
			this.lineRenderer.enabled = true;
			return;
		}
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x000881D8 File Offset: 0x000863D8
	private void Release()
	{
		if (this.disableRelease)
		{
			this.balloonState = BalloonHoldable.BalloonStates.Returning;
			return;
		}
		if (this.balloonDynamics == null)
		{
			return;
		}
		float num = (this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f;
		base.transform.localScale = Vector3.one * num;
		this.EnableDynamics(true, true, false);
		this.balloonDynamics.EnableDistanceConstraints(false, num);
		this.lineRenderer.enabled = false;
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x00088264 File Offset: 0x00086464
	public void OnTriggerEnter(Collider other)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		bool flag = false;
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.TriggerEnter(other, ref zero, ref zero2, ref flag);
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (flag)
		{
			RequestableOwnershipGuard component = PhotonView.Get(this.worldShareableInstance.gameObject).GetComponent<RequestableOwnershipGuard>();
			if (!component.isTrulyMine)
			{
				if (zero.magnitude > this.forceAppliedAsRemote.magnitude)
				{
					this.forceAppliedAsRemote = zero;
					this.collisionPtAsRemote = zero2;
				}
				component.RequestOwnershipImmediately(delegate
				{
				});
			}
		}
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x00088328 File Offset: 0x00086528
	public void OnCollisionEnter(Collision collision)
	{
		if (!this.ShouldSimulate() || this.disableCollisionHandling)
		{
			return;
		}
		this.balloonBopSource.GTPlay();
		if (!collision.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OwnerPopBalloon();
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (PhotonView.Get(this.worldShareableInstance.gameObject).IsMine)
		{
			this.OwnerPopBalloon();
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06001965 RID: 6501 RVA: 0x000883A0 File Offset: 0x000865A0
	FXSystemSettings IFXContext.settings
	{
		get
		{
			return this.ownerRig.fxSettings;
		}
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x000883B0 File Offset: 0x000865B0
	void IFXContext.OnPlayFX()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.balloonPopFXColor);
		}
	}

	// Token: 0x040022B2 RID: 8882
	private ITetheredObjectBehavior balloonDynamics;

	// Token: 0x040022B3 RID: 8883
	[SerializeField]
	private Renderer mesh;

	// Token: 0x040022B4 RID: 8884
	private LineRenderer lineRenderer;

	// Token: 0x040022B5 RID: 8885
	private Rigidbody rb;

	// Token: 0x040022B6 RID: 8886
	private NetPlayer originalOwner;

	// Token: 0x040022B7 RID: 8887
	public GameObject balloonPopFXPrefab;

	// Token: 0x040022B8 RID: 8888
	public Color balloonPopFXColor;

	// Token: 0x040022B9 RID: 8889
	private float timer;

	// Token: 0x040022BA RID: 8890
	public float scaleTimerLength = 2f;

	// Token: 0x040022BB RID: 8891
	public float poppedTimerLength = 2.5f;

	// Token: 0x040022BC RID: 8892
	public float beginScale = 0.1f;

	// Token: 0x040022BD RID: 8893
	public float bopSpeed = 1f;

	// Token: 0x040022BE RID: 8894
	private Vector3 fullyInflatedScale;

	// Token: 0x040022BF RID: 8895
	public AudioSource balloonBopSource;

	// Token: 0x040022C0 RID: 8896
	public AudioSource balloonInflatSource;

	// Token: 0x040022C1 RID: 8897
	private Vector3 forceAppliedAsRemote;

	// Token: 0x040022C2 RID: 8898
	private Vector3 collisionPtAsRemote;

	// Token: 0x040022C3 RID: 8899
	private WaterVolume waterVolume;

	// Token: 0x040022C4 RID: 8900
	[DebugReadout]
	private BalloonHoldable.BalloonStates balloonState;

	// Token: 0x040022C5 RID: 8901
	private float returnTimer;

	// Token: 0x040022C6 RID: 8902
	[SerializeField]
	private float maxDistanceFromOwner;

	// Token: 0x040022C7 RID: 8903
	public float lastOwnershipRequest;

	// Token: 0x040022C8 RID: 8904
	[SerializeField]
	private bool disableCollisionHandling;

	// Token: 0x040022C9 RID: 8905
	[SerializeField]
	private bool disableRelease;

	// Token: 0x02000409 RID: 1033
	private enum BalloonStates
	{
		// Token: 0x040022CB RID: 8907
		Normal,
		// Token: 0x040022CC RID: 8908
		Pop,
		// Token: 0x040022CD RID: 8909
		Waiting,
		// Token: 0x040022CE RID: 8910
		WaitForOwnershipTransfer,
		// Token: 0x040022CF RID: 8911
		WaitForReDock,
		// Token: 0x040022D0 RID: 8912
		Refilling,
		// Token: 0x040022D1 RID: 8913
		Returning
	}
}
