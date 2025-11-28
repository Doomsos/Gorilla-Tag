using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010F1 RID: 4337
	public class FartBagThrowable : MonoBehaviour, IProjectile
	{
		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06006CB5 RID: 27829 RVA: 0x0023B0B7 File Offset: 0x002392B7
		// (set) Token: 0x06006CB6 RID: 27830 RVA: 0x0023B0BF File Offset: 0x002392BF
		public TransferrableObject ParentTransferable { get; set; }

		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06006CB7 RID: 27831 RVA: 0x0023B0C8 File Offset: 0x002392C8
		// (remove) Token: 0x06006CB8 RID: 27832 RVA: 0x0023B100 File Offset: 0x00239300
		public event Action<IProjectile> OnDeflated;

		// Token: 0x06006CB9 RID: 27833 RVA: 0x0023B138 File Offset: 0x00239338
		private void OnEnable()
		{
			this.placedOnFloor = false;
			this.deflated = false;
			this.handContactPoint = Vector3.negativeInfinity;
			this.handNormalVector = Vector3.zero;
			this.timeCreated = float.PositiveInfinity;
			this.placedOnFloorTime = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.ResetBlend();
			}
		}

		// Token: 0x06006CBA RID: 27834 RVA: 0x0023B197 File Offset: 0x00239397
		private void Update()
		{
			if (Time.time - this.timeCreated > this.forceDestroyAfterSec)
			{
				this.DeflateLocal();
			}
		}

		// Token: 0x06006CBB RID: 27835 RVA: 0x0023B1B4 File Offset: 0x002393B4
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rigidbody.linearVelocity = velocity;
			this.timeCreated = Time.time;
			this.InitialPhotonEvent();
		}

		// Token: 0x06006CBC RID: 27836 RVA: 0x0023B214 File Offset: 0x00239414
		private void InitialPhotonEvent()
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			if (this.ParentTransferable)
			{
				NetPlayer netPlayer = (this.ParentTransferable.myOnlineRig != null) ? this.ParentTransferable.myOnlineRig.creator : ((this.ParentTransferable.myRig != null) ? (this.ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (this._events != null && netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.DeflateEvent);
			}
		}

		// Token: 0x06006CBD RID: 27837 RVA: 0x0023B2E8 File Offset: 0x002394E8
		private void OnTriggerEnter(Collider other)
		{
			if ((this.handLayerMask.value & 1 << other.gameObject.layer) != 0)
			{
				if (!this.placedOnFloor)
				{
					return;
				}
				this.handContactPoint = other.ClosestPoint(base.transform.position);
				this.handNormalVector = (this.handContactPoint - base.transform.position).normalized;
				if (Time.time - this.placedOnFloorTime > 0.3f)
				{
					this.Deflate();
				}
			}
		}

		// Token: 0x06006CBE RID: 27838 RVA: 0x0023B370 File Offset: 0x00239570
		private void OnCollisionEnter(Collision other)
		{
			if ((this.floorLayerMask.value & 1 << other.gameObject.layer) != 0)
			{
				this.placedOnFloor = true;
				this.placedOnFloorTime = Time.time;
				Vector3 normal = other.contacts[0].normal;
				base.transform.position = other.contacts[0].point + normal * this.placementOffset;
				Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
				base.transform.rotation = rotation;
			}
		}

		// Token: 0x06006CBF RID: 27839 RVA: 0x0023B418 File Offset: 0x00239618
		private void Deflate()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					this.handContactPoint,
					this.handNormalVector
				});
			}
			this.DeflateLocal();
		}

		// Token: 0x06006CC0 RID: 27840 RVA: 0x0023B488 File Offset: 0x00239688
		private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "DeflateEvent");
			if (this.callLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 position = (Vector3)obj;
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						float num = 10000f;
						if (!vector.IsValid(num))
						{
							return;
						}
						num = 10000f;
						if (!position.IsValid(num) || !this.ParentTransferable.targetRig.IsPositionInRange(position, 4f))
						{
							return;
						}
						this.handNormalVector = vector;
						this.handContactPoint = position;
						this.DeflateLocal();
						return;
					}
				}
			}
		}

		// Token: 0x06006CC1 RID: 27841 RVA: 0x0023B538 File Offset: 0x00239738
		private void DeflateLocal()
		{
			if (this.deflated)
			{
				return;
			}
			GameObject gameObject = ObjectPools.instance.Instantiate(this.deflationEffect, this.handContactPoint, true);
			gameObject.transform.up = this.handNormalVector;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
			if (componentInChildren.soundBank)
			{
				componentInChildren.Play();
			}
			this.placedOnFloor = false;
			this.timeCreated = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.FullyBlend();
			}
			this.deflated = true;
			base.Invoke("DisableObject", this.destroyWhenDeflateDelay);
		}

		// Token: 0x06006CC2 RID: 27842 RVA: 0x0023B5E7 File Offset: 0x002397E7
		private void DisableObject()
		{
			Action<IProjectile> onDeflated = this.OnDeflated;
			if (onDeflated != null)
			{
				onDeflated.Invoke(this);
			}
			this.deflated = false;
		}

		// Token: 0x06006CC3 RID: 27843 RVA: 0x0023B604 File Offset: 0x00239804
		private void OnDestroy()
		{
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.DeflateEvent);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x04007DA8 RID: 32168
		[SerializeField]
		private GameObject deflationEffect;

		// Token: 0x04007DA9 RID: 32169
		[SerializeField]
		private float destroyWhenDeflateDelay = 3f;

		// Token: 0x04007DAA RID: 32170
		[SerializeField]
		private float forceDestroyAfterSec = 10f;

		// Token: 0x04007DAB RID: 32171
		[SerializeField]
		private float placementOffset = 0.2f;

		// Token: 0x04007DAC RID: 32172
		[SerializeField]
		private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;

		// Token: 0x04007DAD RID: 32173
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x04007DAE RID: 32174
		[SerializeField]
		private LayerMask handLayerMask;

		// Token: 0x04007DAF RID: 32175
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x04007DB0 RID: 32176
		private bool placedOnFloor;

		// Token: 0x04007DB1 RID: 32177
		private float placedOnFloorTime;

		// Token: 0x04007DB2 RID: 32178
		private float timeCreated;

		// Token: 0x04007DB3 RID: 32179
		private bool deflated;

		// Token: 0x04007DB4 RID: 32180
		private Vector3 handContactPoint;

		// Token: 0x04007DB5 RID: 32181
		private Vector3 handNormalVector;

		// Token: 0x04007DB6 RID: 32182
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04007DB9 RID: 32185
		private RubberDuckEvents _events;
	}
}
