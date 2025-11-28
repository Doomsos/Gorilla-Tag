using System;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010A0 RID: 4256
	public class RCVehicle : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06006A7B RID: 27259 RVA: 0x0022E95C File Offset: 0x0022CB5C
		public bool HasLocalAuthority
		{
			get
			{
				return !PhotonNetwork.InRoom || (this.networkSync != null && this.networkSync.photonView.IsMine);
			}
		}

		// Token: 0x06006A7C RID: 27260 RVA: 0x0022E988 File Offset: 0x0022CB88
		public virtual void WakeUpRemote(RCCosmeticNetworkSync sync)
		{
			this.networkSync = sync;
			this.hasNetworkSync = (sync != null);
			if (this.HasLocalAuthority)
			{
				return;
			}
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				this.localStatePrev = RCVehicle.State.Disabled;
				base.enabled = true;
				base.gameObject.SetActive(true);
				this.RemoteUpdate(Time.deltaTime);
			}
		}

		// Token: 0x06006A7D RID: 27261 RVA: 0x0022E9EC File Offset: 0x0022CBEC
		public virtual void StartConnection(RCRemoteHoldable remote, RCCosmeticNetworkSync sync)
		{
			this.connectedRemote = remote;
			this.networkSync = sync;
			this.hasNetworkSync = (sync != null);
			base.enabled = true;
			base.gameObject.SetActive(true);
			this.useLeftDock = (remote.XRNode == 4);
			if (this.HasLocalAuthority && this.localState != RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginDocked();
			}
		}

		// Token: 0x06006A7E RID: 27262 RVA: 0x0022EA4D File Offset: 0x0022CC4D
		public virtual void EndConnection()
		{
			this.connectedRemote = null;
			this.activeInput = default(RCRemoteHoldable.RCInput);
			this.disconnectionTime = Time.time;
		}

		// Token: 0x06006A7F RID: 27263 RVA: 0x0022EA70 File Offset: 0x0022CC70
		protected virtual void ResetToSpawnPosition()
		{
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			if (this.rb != null)
			{
				this.rb.isKinematic = true;
			}
			base.transform.parent = (this.useLeftDock ? this.leftDockParent : this.rightDockParent);
			base.transform.SetLocalPositionAndRotation(this.useLeftDock ? this.dockLeftOffset.pos : this.dockRightOffset.pos, this.useLeftDock ? this.dockLeftOffset.rot : this.dockRightOffset.rot);
			base.transform.localScale = (this.useLeftDock ? this.dockLeftOffset.scale : this.dockRightOffset.scale);
		}

		// Token: 0x06006A80 RID: 27264 RVA: 0x0022EB48 File Offset: 0x0022CD48
		protected virtual void AuthorityBeginDocked()
		{
			this.localState = (this.useLeftDock ? RCVehicle.State.DockedLeft : RCVehicle.State.DockedRight);
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			this.waitingForTriggerRelease = true;
			this.ResetToSpawnPosition();
			if (this.connectedRemote == null)
			{
				this.SetDisabledState();
			}
		}

		// Token: 0x06006A81 RID: 27265 RVA: 0x0022EBB8 File Offset: 0x0022CDB8
		protected virtual void AuthorityBeginMobilization()
		{
			this.localState = RCVehicle.State.Mobilized;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			base.transform.parent = null;
			this.rb.isKinematic = false;
		}

		// Token: 0x06006A82 RID: 27266 RVA: 0x0022EC14 File Offset: 0x0022CE14
		protected virtual void AuthorityBeginCrash()
		{
			this.localState = RCVehicle.State.Crashed;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
		}

		// Token: 0x06006A83 RID: 27267 RVA: 0x0022EC50 File Offset: 0x0022CE50
		protected virtual void SetDisabledState()
		{
			this.localState = RCVehicle.State.Disabled;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.ResetToSpawnPosition();
			base.enabled = false;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06006A84 RID: 27268 RVA: 0x0022ECA2 File Offset: 0x0022CEA2
		protected virtual void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06006A85 RID: 27269 RVA: 0x00002789 File Offset: 0x00000989
		protected virtual void OnEnable()
		{
		}

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06006A86 RID: 27270 RVA: 0x0022ECB0 File Offset: 0x0022CEB0
		// (set) Token: 0x06006A87 RID: 27271 RVA: 0x0022ECB8 File Offset: 0x0022CEB8
		bool ISpawnable.IsSpawned { get; set; }

		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06006A88 RID: 27272 RVA: 0x0022ECC1 File Offset: 0x0022CEC1
		// (set) Token: 0x06006A89 RID: 27273 RVA: 0x0022ECC9 File Offset: 0x0022CEC9
		ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

		// Token: 0x06006A8A RID: 27274 RVA: 0x0022ECD4 File Offset: 0x0022CED4
		void ISpawnable.OnSpawn(VRRig rig)
		{
			if (rig == null)
			{
				GTDev.LogError<string>("RCVehicle: Could not find VRRig in parents. If you are trying to make this a world item rather than a cosmetic then you'll have to refactor how it teleports back to the arms.", this, null);
				return;
			}
			string text;
			if (!GTHardCodedBones.TryGetBoneXforms(rig, out this._vrRigBones, out text))
			{
				Debug.LogError("RCVehicle: " + text, this);
				return;
			}
			if (this.leftDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockLeftOffset.bone, out this.leftDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find left dock transform.", this, null);
			}
			if (this.rightDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockRightOffset.bone, out this.rightDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find right dock transform.", this, null);
			}
		}

		// Token: 0x06006A8B RID: 27275 RVA: 0x00002789 File Offset: 0x00000989
		void ISpawnable.OnDespawn()
		{
		}

		// Token: 0x06006A8C RID: 27276 RVA: 0x0022ED8F File Offset: 0x0022CF8F
		protected virtual void OnDisable()
		{
			this.localState = RCVehicle.State.Disabled;
			this.localStatePrev = RCVehicle.State.Disabled;
		}

		// Token: 0x06006A8D RID: 27277 RVA: 0x0022EDA0 File Offset: 0x0022CFA0
		public void ApplyRemoteControlInput(RCRemoteHoldable.RCInput rcInput)
		{
			this.activeInput.joystick.y = Mathf.Sign(rcInput.joystick.y) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.y)));
			this.activeInput.joystick.x = Mathf.Sign(rcInput.joystick.x) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.x)));
			this.activeInput.trigger = Mathf.Clamp(rcInput.trigger, -1f, 1f);
			this.activeInput.buttons = rcInput.buttons;
		}

		// Token: 0x06006A8E RID: 27278 RVA: 0x0022EE80 File Offset: 0x0022D080
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (this.HasLocalAuthority)
			{
				this.AuthorityUpdate(deltaTime);
			}
			else
			{
				this.RemoteUpdate(deltaTime);
			}
			this.SharedUpdate(deltaTime);
			this.localStatePrev = this.localState;
		}

		// Token: 0x06006A8F RID: 27279 RVA: 0x0022EEC0 File Offset: 0x0022D0C0
		protected virtual void AuthorityUpdate(float dt)
		{
			switch (this.localState)
			{
			default:
				if (this.localState != this.localStatePrev)
				{
					this.ResetToSpawnPosition();
				}
				if (this.connectedRemote == null)
				{
					this.SetDisabledState();
					return;
				}
				if (this.waitingForTriggerRelease && this.activeInput.trigger < 0.25f)
				{
					this.waitingForTriggerRelease = false;
				}
				if (!this.waitingForTriggerRelease && this.activeInput.trigger > 0.25f)
				{
					this.AuthorityBeginMobilization();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.networkSync != null)
				{
					this.networkSync.syncedState.position = base.transform.position;
					this.networkSync.syncedState.rotation = base.transform.rotation;
				}
				bool flag = (base.transform.position - this.leftDockParent.position).sqrMagnitude > this.maxRange * this.maxRange;
				bool flag2 = this.connectedRemote == null && Time.time - this.disconnectionTime > this.maxDisconnectionTime;
				if (flag || flag2)
				{
					this.AuthorityBeginCrash();
					return;
				}
				break;
			}
			case RCVehicle.State.Crashed:
				if (Time.time > this.stateStartTime + this.crashRespawnDelay)
				{
					this.AuthorityBeginDocked();
				}
				break;
			}
		}

		// Token: 0x06006A90 RID: 27280 RVA: 0x0022F020 File Offset: 0x0022D220
		protected virtual void RemoteUpdate(float dt)
		{
			if (this.networkSync == null)
			{
				this.SetDisabledState();
				return;
			}
			this.localState = (RCVehicle.State)this.networkSync.syncedState.state;
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				this.SetDisabledState();
				break;
			default:
				if (this.localStatePrev != RCVehicle.State.DockedLeft)
				{
					this.useLeftDock = true;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.useLeftDock = false;
					this.ResetToSpawnPosition();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.rb.isKinematic = true;
					base.transform.parent = null;
				}
				base.transform.position = Vector3.Lerp(this.networkSync.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				base.transform.rotation = Quaternion.Slerp(this.networkSync.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				return;
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.rb.isKinematic = false;
					base.transform.parent = null;
					if (this.localStatePrev != RCVehicle.State.Mobilized)
					{
						base.transform.position = this.networkSync.syncedState.position;
						base.transform.rotation = this.networkSync.syncedState.rotation;
						return;
					}
				}
				break;
			}
		}

		// Token: 0x06006A91 RID: 27281 RVA: 0x00002789 File Offset: 0x00000989
		protected virtual void SharedUpdate(float dt)
		{
		}

		// Token: 0x06006A92 RID: 27282 RVA: 0x0022F1A8 File Offset: 0x0022D3A8
		public virtual void AuthorityApplyImpact(Vector3 hitVelocity, bool isProjectile)
		{
			if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				float num = isProjectile ? this.projectileVelocityTransfer : this.hitVelocityTransfer;
				this.rb.AddForce(Vector3.ClampMagnitude(hitVelocity * num, this.hitMaxHitSpeed) * this.rb.mass, 1);
				if (isProjectile || (this.crashOnHit && hitVelocity.sqrMagnitude > this.crashOnHitSpeedThreshold * this.crashOnHitSpeedThreshold))
				{
					this.AuthorityBeginCrash();
				}
			}
			UnityEvent onHitImpact = this.OnHitImpact;
			if (onHitImpact == null)
			{
				return;
			}
			onHitImpact.Invoke();
		}

		// Token: 0x06006A93 RID: 27283 RVA: 0x00184251 File Offset: 0x00182451
		protected float NormalizeAngle180(float angle)
		{
			angle = (angle + 180f) % 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle - 180f;
		}

		// Token: 0x06006A94 RID: 27284 RVA: 0x0022F240 File Offset: 0x0022D440
		protected static void AddScaledGravityCompensationForce(Rigidbody rb, float scaleFactor, float gravityCompensation)
		{
			Vector3 gravity = Physics.gravity;
			Vector3 vector = -gravity * gravityCompensation;
			Vector3 vector2 = gravity + vector;
			Vector3 vector3 = vector2 * scaleFactor - vector2;
			rb.AddForce((vector + vector3) * rb.mass, 0);
		}

		// Token: 0x04007A90 RID: 31376
		[SerializeField]
		private Transform leftDockParent;

		// Token: 0x04007A91 RID: 31377
		[SerializeField]
		private Transform rightDockParent;

		// Token: 0x04007A92 RID: 31378
		[SerializeField]
		private float maxRange = 100f;

		// Token: 0x04007A93 RID: 31379
		[SerializeField]
		private float maxDisconnectionTime = 10f;

		// Token: 0x04007A94 RID: 31380
		[SerializeField]
		private float crashRespawnDelay = 3f;

		// Token: 0x04007A95 RID: 31381
		[SerializeField]
		private bool crashOnHit;

		// Token: 0x04007A96 RID: 31382
		[SerializeField]
		private float crashOnHitSpeedThreshold = 5f;

		// Token: 0x04007A97 RID: 31383
		[SerializeField]
		[Range(0f, 1f)]
		private float hitVelocityTransfer = 0.5f;

		// Token: 0x04007A98 RID: 31384
		[SerializeField]
		[Range(0f, 1f)]
		private float projectileVelocityTransfer = 0.1f;

		// Token: 0x04007A99 RID: 31385
		[SerializeField]
		private float hitMaxHitSpeed = 4f;

		// Token: 0x04007A9A RID: 31386
		[SerializeField]
		[Range(0f, 1f)]
		private float joystickDeadzone = 0.1f;

		// Token: 0x04007A9B RID: 31387
		[Header("RCVehicle - Shared Event")]
		public UnityEvent OnHitImpact;

		// Token: 0x04007A9C RID: 31388
		protected RCVehicle.State localState;

		// Token: 0x04007A9D RID: 31389
		protected RCVehicle.State localStatePrev;

		// Token: 0x04007A9E RID: 31390
		protected float stateStartTime;

		// Token: 0x04007A9F RID: 31391
		protected RCRemoteHoldable connectedRemote;

		// Token: 0x04007AA0 RID: 31392
		protected RCCosmeticNetworkSync networkSync;

		// Token: 0x04007AA1 RID: 31393
		protected bool hasNetworkSync;

		// Token: 0x04007AA2 RID: 31394
		protected RCRemoteHoldable.RCInput activeInput;

		// Token: 0x04007AA3 RID: 31395
		protected Rigidbody rb;

		// Token: 0x04007AA4 RID: 31396
		private bool waitingForTriggerRelease;

		// Token: 0x04007AA5 RID: 31397
		private float disconnectionTime;

		// Token: 0x04007AA6 RID: 31398
		private bool useLeftDock;

		// Token: 0x04007AA7 RID: 31399
		private BoneOffset dockLeftOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_L, new Vector3(-0.062f, 0.283f, -0.136f), new Vector3(275f, 0f, 25f));

		// Token: 0x04007AA8 RID: 31400
		private BoneOffset dockRightOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_R, new Vector3(0.069f, 0.265f, -0.128f), new Vector3(275f, 0f, 335f));

		// Token: 0x04007AA9 RID: 31401
		private float networkSyncFollowRateExp = 2f;

		// Token: 0x04007AAA RID: 31402
		private Transform[] _vrRigBones;

		// Token: 0x020010A1 RID: 4257
		protected enum State
		{
			// Token: 0x04007AAE RID: 31406
			Disabled,
			// Token: 0x04007AAF RID: 31407
			DockedLeft,
			// Token: 0x04007AB0 RID: 31408
			DockedRight,
			// Token: 0x04007AB1 RID: 31409
			Mobilized,
			// Token: 0x04007AB2 RID: 31410
			Crashed
		}
	}
}
