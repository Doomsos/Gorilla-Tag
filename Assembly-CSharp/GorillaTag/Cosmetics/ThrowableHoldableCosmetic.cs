using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Shared.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B7 RID: 4279
	public class ThrowableHoldableCosmetic : TransferrableObject
	{
		// Token: 0x06006B27 RID: 27431 RVA: 0x00232384 File Offset: 0x00230584
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnThrowEvent);
			}
		}

		// Token: 0x06006B28 RID: 27432 RVA: 0x00232448 File Offset: 0x00230648
		protected override void Awake()
		{
			base.Awake();
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
			if (this.alternativeProjectilePrefab != null)
			{
				this.alternativeProjectileHash = PoolUtils.GameObjHashCode(this.alternativeProjectilePrefab);
			}
			this.currentProjectileHash = this.projectileHash;
			this.playersEffect = base.GetComponentInChildren<CosmeticEffectsOnPlayers>();
		}

		// Token: 0x06006B29 RID: 27433 RVA: 0x002324A3 File Offset: 0x002306A3
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (!this.disableWhenThrown.gameObject.activeSelf)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x06006B2A RID: 27434 RVA: 0x002324C0 File Offset: 0x002306C0
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return false;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					position,
					rotation,
					averageVelocity,
					scale
				});
			}
			this.OnThrowLocal(position, rotation, averageVelocity, this.ownerRig);
			return true;
		}

		// Token: 0x06006B2B RID: 27435 RVA: 0x002325BC File Offset: 0x002307BC
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnThrowEvent);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006B2C RID: 27436 RVA: 0x00232611 File Offset: 0x00230811
		public void UseAlternativeProjectile()
		{
			if (this.alternativeProjectilePrefab != null)
			{
				this.currentProjectileHash = this.alternativeProjectileHash;
			}
		}

		// Token: 0x06006B2D RID: 27437 RVA: 0x00232630 File Offset: 0x00230830
		private void OnThrowEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 4)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnThrowEvent");
			if (this.firecrackerCallLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Quaternion)
					{
						Quaternion rotation = (Quaternion)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 vector2 = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float value = (float)obj;
								vector2 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 40f, 100f);
								value.ClampSafe(0.01f, 1f);
								if (!rotation.IsValid())
								{
									return;
								}
								float num = 10000f;
								if (!vector.IsValid(num) || !this.targetRig.IsPositionInRange(vector, 4f))
								{
									return;
								}
								this.OnThrowLocal(vector, rotation, vector2, this.ownerRig);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x06006B2E RID: 27438 RVA: 0x00232744 File Offset: 0x00230944
		private void OnThrowLocal(Vector3 startPos, Quaternion rotation, Vector3 velocity, VRRig ownerRig)
		{
			this.disableWhenThrown.SetActive(false);
			IProjectile component = ObjectPools.instance.Instantiate(this.currentProjectileHash, true).GetComponent<IProjectile>();
			FirecrackerProjectile firecrackerProjectile = component as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
				{
					int state = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
					firecrackerProjectile.SetTransferrableState(this.networkedStateEvents, state);
				}
				firecrackerProjectile.OnDetonationComplete.AddListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				firecrackerProjectile.OnDetonationStart.AddListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
			}
			else
			{
				FartBagThrowable fartBagThrowable = component as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated += new Action<IProjectile>(this.HitComplete);
					fartBagThrowable.ParentTransferable = this;
				}
			}
			component.Launch(startPos, rotation, velocity, 1f, ownerRig, -1);
			this.currentProjectileHash = this.projectileHash;
		}

		// Token: 0x06006B2F RID: 27439 RVA: 0x0023280D File Offset: 0x00230A0D
		private void HitStart(FirecrackerProjectile firecracker, Vector3 contactPos)
		{
			if (firecracker == null)
			{
				return;
			}
			if (this.playersEffect == null)
			{
				return;
			}
			this.playersEffect.ApplyAllEffectsByDistance(contactPos);
		}

		// Token: 0x06006B30 RID: 27440 RVA: 0x00232834 File Offset: 0x00230A34
		private void HitComplete(IProjectile projectile)
		{
			if (projectile == null)
			{
				return;
			}
			if (base.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
			{
				TransferrableObject.SyncOptions networkedStateEvents = this.networkedStateEvents;
				if (networkedStateEvents != TransferrableObject.SyncOptions.Bool)
				{
					if (networkedStateEvents == TransferrableObject.SyncOptions.Int)
					{
						base.SetItemStateInt(0);
					}
				}
				else
				{
					base.ResetStateBools();
				}
			}
			this.disableWhenThrown.SetActive(true);
			FirecrackerProjectile firecrackerProjectile = projectile as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				firecrackerProjectile.OnDetonationStart.RemoveListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
				firecrackerProjectile.OnDetonationComplete.RemoveListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				ObjectPools.instance.Destroy(firecrackerProjectile.gameObject);
				return;
			}
			FartBagThrowable fartBagThrowable = projectile as FartBagThrowable;
			if (fartBagThrowable != null)
			{
				fartBagThrowable.OnDeflated -= new Action<IProjectile>(this.HitComplete);
				ObjectPools.instance.Destroy(fartBagThrowable.gameObject);
			}
		}

		// Token: 0x04007B7D RID: 31613
		[Tooltip("Projectile prefab from the global object pool that gets spawned when this object is thrown")]
		[FormerlySerializedAs("firecrackerProjectilePrefab")]
		[SerializeField]
		private GameObject projectilePrefab;

		// Token: 0x04007B7E RID: 31614
		[Tooltip(" A second projectile prefab that will be spawned if UseAlternativeProjectile is called")]
		[SerializeField]
		private GameObject alternativeProjectilePrefab;

		// Token: 0x04007B7F RID: 31615
		[Tooltip("Objects on the body that should be hidden when the projectile is spawned")]
		[SerializeField]
		private GameObject disableWhenThrown;

		// Token: 0x04007B80 RID: 31616
		private CallLimiter firecrackerCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04007B81 RID: 31617
		private CosmeticEffectsOnPlayers playersEffect;

		// Token: 0x04007B82 RID: 31618
		private int projectileHash;

		// Token: 0x04007B83 RID: 31619
		private int alternativeProjectileHash;

		// Token: 0x04007B84 RID: 31620
		private int currentProjectileHash;

		// Token: 0x04007B85 RID: 31621
		private RubberDuckEvents _events;
	}
}
