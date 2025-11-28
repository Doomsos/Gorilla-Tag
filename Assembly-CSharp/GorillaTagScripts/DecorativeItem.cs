using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DD9 RID: 3545
	public class DecorativeItem : TransferrableObject
	{
		// Token: 0x06005803 RID: 22531 RVA: 0x001B245C File Offset: 0x001B065C
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06005804 RID: 22532 RVA: 0x001C2046 File Offset: 0x001C0246
		public override void OnSpawn(VRRig rig)
		{
			base.OnSpawn(rig);
			this.parent = base.transform.parent;
		}

		// Token: 0x06005805 RID: 22533 RVA: 0x001B24DF File Offset: 0x001B06DF
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06005806 RID: 22534 RVA: 0x001C2060 File Offset: 0x001C0260
		private new void OnStateChanged()
		{
			TransferrableObject.ItemStates itemState = this.itemState;
			if (itemState == TransferrableObject.ItemStates.State2)
			{
				this.SnapItem(this.reliableState.isSnapped, this.reliableState.snapPosition);
				return;
			}
			if (itemState != TransferrableObject.ItemStates.State3)
			{
				return;
			}
			this.Respawn(this.reliableState.respawnPosition, this.reliableState.respawnRotation);
		}

		// Token: 0x06005807 RID: 22535 RVA: 0x001C20B8 File Offset: 0x001C02B8
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			DecorativeItem.DecorativeItemState itemState = (DecorativeItem.DecorativeItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
		}

		// Token: 0x06005808 RID: 22536 RVA: 0x001C20F7 File Offset: 0x001C02F7
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.itemState == TransferrableObject.ItemStates.State4 && this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine)
			{
				this.InvokeRespawn();
			}
		}

		// Token: 0x06005809 RID: 22537 RVA: 0x001C212E File Offset: 0x001C032E
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x0600580A RID: 22538 RVA: 0x001C213F File Offset: 0x001C033F
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			return true;
		}

		// Token: 0x0600580B RID: 22539 RVA: 0x0019B984 File Offset: 0x00199B84
		private void SetWillTeleport()
		{
			this.worldShareableInstance.SetWillTeleport();
		}

		// Token: 0x0600580C RID: 22540 RVA: 0x001C2160 File Offset: 0x001C0360
		public void Respawn(Vector3 randPosition, Quaternion randRotation)
		{
			if (base.InHand())
			{
				return;
			}
			if (this.shatterVFX && this.ShouldPlayFX())
			{
				this.PlayVFX(this.shatterVFX);
			}
			this.itemState = TransferrableObject.ItemStates.State3;
			this.SetWillTeleport();
			Transform transform = base.transform;
			transform.position = randPosition;
			transform.rotation = randRotation;
			if (this.reliableState)
			{
				this.reliableState.respawnPosition = randPosition;
				this.reliableState.respawnRotation = randRotation;
			}
		}

		// Token: 0x0600580D RID: 22541 RVA: 0x000BB586 File Offset: 0x000B9786
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x0600580E RID: 22542 RVA: 0x001C21DC File Offset: 0x001C03DC
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x0600580F RID: 22543 RVA: 0x001C2218 File Offset: 0x001C0418
		public void SnapItem(bool snap, Vector3 attachPoint)
		{
			if (!this.reliableState)
			{
				return;
			}
			if (snap)
			{
				AttachPoint currentAttachPointByPosition = DecorativeItemsManager.Instance.getCurrentAttachPointByPosition(attachPoint);
				if (!currentAttachPointByPosition)
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				Transform attachPoint2 = currentAttachPointByPosition.attachPoint;
				if (!this.Reparent(attachPoint2))
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				this.itemState = TransferrableObject.ItemStates.State2;
				base.transform.parent.localPosition = Vector3.zero;
				base.transform.localPosition = Vector3.zero;
				this.reliableState.isSnapped = true;
				if (this.audioSource && this.snapAudio && this.ShouldPlayFX())
				{
					this.audioSource.GTPlayOneShot(this.snapAudio, 1f);
				}
				currentAttachPointByPosition.SetIsHook(true);
			}
			else
			{
				this.Reparent(null);
				this.reliableState.isSnapped = false;
			}
			this.reliableState.snapPosition = attachPoint;
		}

		// Token: 0x06005810 RID: 22544 RVA: 0x001C2333 File Offset: 0x001C0533
		private void InvokeRespawn()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				return;
			}
			UnityAction<DecorativeItem> unityAction = this.respawnItem;
			if (unityAction == null)
			{
				return;
			}
			unityAction.Invoke(this);
		}

		// Token: 0x06005811 RID: 22545 RVA: 0x001C2350 File Offset: 0x001C0550
		private bool ShouldPlayFX()
		{
			return this.previousItemState == DecorativeItem.DecorativeItemState.isHeld || this.previousItemState == DecorativeItem.DecorativeItemState.dropped;
		}

		// Token: 0x06005812 RID: 22546 RVA: 0x001C2367 File Offset: 0x001C0567
		private void OnCollisionEnter(Collision other)
		{
			if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
			{
				return;
			}
			this.InvokeRespawn();
		}

		// Token: 0x04006553 RID: 25939
		public DecorativeItemReliableState reliableState;

		// Token: 0x04006554 RID: 25940
		public UnityAction<DecorativeItem> respawnItem;

		// Token: 0x04006555 RID: 25941
		public LayerMask breakItemLayerMask;

		// Token: 0x04006556 RID: 25942
		private Coroutine respawnTimer;

		// Token: 0x04006557 RID: 25943
		private Transform parent;

		// Token: 0x04006558 RID: 25944
		private float _respawnTimestamp;

		// Token: 0x04006559 RID: 25945
		private bool isSnapped;

		// Token: 0x0400655A RID: 25946
		private Vector3 currentPosition;

		// Token: 0x0400655B RID: 25947
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400655C RID: 25948
		public AudioClip snapAudio;

		// Token: 0x0400655D RID: 25949
		public GameObject shatterVFX;

		// Token: 0x0400655E RID: 25950
		private new DecorativeItem.DecorativeItemState previousItemState = DecorativeItem.DecorativeItemState.dropped;

		// Token: 0x02000DDA RID: 3546
		private enum DecorativeItemState
		{
			// Token: 0x04006560 RID: 25952
			isHeld = 1,
			// Token: 0x04006561 RID: 25953
			dropped,
			// Token: 0x04006562 RID: 25954
			snapped = 4,
			// Token: 0x04006563 RID: 25955
			respawn = 8,
			// Token: 0x04006564 RID: 25956
			none = 16
		}
	}
}
