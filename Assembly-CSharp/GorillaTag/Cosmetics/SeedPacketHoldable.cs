using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B3 RID: 4275
	[RequireComponent(typeof(TransferrableObject))]
	public class SeedPacketHoldable : MonoBehaviour
	{
		// Token: 0x06006B00 RID: 27392 RVA: 0x002319BB File Offset: 0x0022FBBB
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.flowerEffectHash = PoolUtils.GameObjHashCode(this.flowerEffectPrefab);
		}

		// Token: 0x06006B01 RID: 27393 RVA: 0x002319DC File Offset: 0x0022FBDC
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SyncTriggerEffect);
			}
		}

		// Token: 0x06006B02 RID: 27394 RVA: 0x00231AA4 File Offset: 0x0022FCA4
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SyncTriggerEffect);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006B03 RID: 27395 RVA: 0x00231AF3 File Offset: 0x0022FCF3
		private void OnDestroy()
		{
			this.pooledObjects.Clear();
		}

		// Token: 0x06006B04 RID: 27396 RVA: 0x00231B00 File Offset: 0x0022FD00
		private void Update()
		{
			if (!this.transferrableObject.InHand())
			{
				return;
			}
			if (!this.isPouring && Vector3.Angle(base.transform.up, Vector3.down) <= this.pouringAngle)
			{
				this.StartPouring();
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, Vector3.down, ref raycastHit, this.pouringRaycastDistance, this.raycastLayerMask))
				{
					this.hitPoint = raycastHit.point;
					base.Invoke("SpawnEffect", raycastHit.distance * this.placeEffectDelayMultiplier);
				}
			}
			if (this.isPouring && Time.time - this.pouringStartedTime >= this.cooldown)
			{
				this.isPouring = false;
			}
		}

		// Token: 0x06006B05 RID: 27397 RVA: 0x00231BB9 File Offset: 0x0022FDB9
		private void StartPouring()
		{
			if (this.particles)
			{
				this.particles.Play();
			}
			this.isPouring = true;
			this.pouringStartedTime = Time.time;
		}

		// Token: 0x06006B06 RID: 27398 RVA: 0x00231BE8 File Offset: 0x0022FDE8
		private void SpawnEffect()
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(this.flowerEffectHash, true);
			gameObject.transform.position = this.hitPoint;
			SeedPacketTriggerHandler seedPacketTriggerHandler;
			if (gameObject.TryGetComponent<SeedPacketTriggerHandler>(ref seedPacketTriggerHandler))
			{
				this.pooledObjects.Add(seedPacketTriggerHandler);
				seedPacketTriggerHandler.onTriggerEntered.AddListener(new UnityAction<SeedPacketTriggerHandler>(this.SyncTriggerEffectForOthers));
			}
		}

		// Token: 0x06006B07 RID: 27399 RVA: 0x00231C44 File Offset: 0x0022FE44
		private void SyncTriggerEffectForOthers(SeedPacketTriggerHandler seedPacketTriggerHandlerTriggerHandlerEvent)
		{
			int num = this.pooledObjects.IndexOf(seedPacketTriggerHandlerTriggerHandlerEvent);
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					num
				});
			}
		}

		// Token: 0x06006B08 RID: 27400 RVA: 0x00231CA8 File Offset: 0x0022FEA8
		private void SyncTriggerEffect(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "SyncTriggerEffect");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			int num = (int)args[0];
			if (num < 0 && num >= this.pooledObjects.Count)
			{
				return;
			}
			this.pooledObjects[num].ToggleEffects();
		}

		// Token: 0x04007B4A RID: 31562
		[SerializeField]
		private float cooldown;

		// Token: 0x04007B4B RID: 31563
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04007B4C RID: 31564
		[SerializeField]
		private float pouringAngle;

		// Token: 0x04007B4D RID: 31565
		[SerializeField]
		private float pouringRaycastDistance = 5f;

		// Token: 0x04007B4E RID: 31566
		[SerializeField]
		private LayerMask raycastLayerMask;

		// Token: 0x04007B4F RID: 31567
		[SerializeField]
		private float placeEffectDelayMultiplier = 10f;

		// Token: 0x04007B50 RID: 31568
		[SerializeField]
		private GameObject flowerEffectPrefab;

		// Token: 0x04007B51 RID: 31569
		private List<SeedPacketTriggerHandler> pooledObjects = new List<SeedPacketTriggerHandler>();

		// Token: 0x04007B52 RID: 31570
		private CallLimiter callLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04007B53 RID: 31571
		private int flowerEffectHash;

		// Token: 0x04007B54 RID: 31572
		private Vector3 hitPoint;

		// Token: 0x04007B55 RID: 31573
		private TransferrableObject transferrableObject;

		// Token: 0x04007B56 RID: 31574
		private bool isPouring = true;

		// Token: 0x04007B57 RID: 31575
		private float pouringStartedTime;

		// Token: 0x04007B58 RID: 31576
		private RubberDuckEvents _events;
	}
}
