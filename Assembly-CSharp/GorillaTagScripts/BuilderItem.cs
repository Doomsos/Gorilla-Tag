using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB3 RID: 3507
	public class BuilderItem : TransferrableObject
	{
		// Token: 0x06005648 RID: 22088 RVA: 0x001B245C File Offset: 0x001B065C
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06005649 RID: 22089 RVA: 0x001B247C File Offset: 0x001B067C
		protected override void Awake()
		{
			base.Awake();
			this.parent = base.transform.parent;
			this.currTable = null;
			this.initialPosition = base.transform.position;
			this.initialRotation = base.transform.rotation;
			this.initialGrabInteractorScale = this.gripInteractor.transform.localScale;
		}

		// Token: 0x0600564A RID: 22090 RVA: 0x00099EE0 File Offset: 0x000980E0
		internal override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x0600564B RID: 22091 RVA: 0x0003CCC0 File Offset: 0x0003AEC0
		internal override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x0600564C RID: 22092 RVA: 0x001B24DF File Offset: 0x001B06DF
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x0600564D RID: 22093 RVA: 0x001B24FC File Offset: 0x001B06FC
		public void AttachPiece(BuilderPiece piece)
		{
			base.transform.SetPositionAndRotation(piece.transform.position, piece.transform.rotation);
			piece.transform.localScale = Vector3.one;
			piece.transform.SetParent(this.itemRoot.transform);
			Debug.LogFormat(piece.gameObject, "Attach Piece {0} to container {1}", new object[]
			{
				piece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = piece;
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x001B2594 File Offset: 0x001B0794
		public void DetachPiece(BuilderPiece piece)
		{
			if (piece != this.attachedPiece)
			{
				Debug.LogErrorFormat("Trying to detach piece {0} from a container containing {1}", new object[]
				{
					piece.pieceId,
					this.attachedPiece.pieceId
				});
				return;
			}
			piece.transform.SetParent(null);
			Debug.LogFormat(this.attachedPiece.gameObject, "Detach Piece {0} from container {1}", new object[]
			{
				this.attachedPiece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = null;
		}

		// Token: 0x0600564F RID: 22095 RVA: 0x001B263C File Offset: 0x001B083C
		private new void OnStateChanged()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				this.enableCollidersWhenReady = true;
				this.gripInteractor.transform.localScale = this.initialGrabInteractorScale * 2f;
				this.handsFreeOfCollidersTime = 0f;
				return;
			}
			this.enableCollidersWhenReady = false;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			this.handsFreeOfCollidersTime = 0f;
		}

		// Token: 0x06005650 RID: 22096 RVA: 0x001B26B0 File Offset: 0x001B08B0
		public override Matrix4x4 GetDefaultTransformationMatrix()
		{
			if (this.reliableState.dirty)
			{
				base.SetupHandMatrix(this.reliableState.leftHandAttachPos, this.reliableState.leftHandAttachRot, this.reliableState.rightHandAttachPos, this.reliableState.rightHandAttachRot);
				this.reliableState.dirty = false;
			}
			return base.GetDefaultTransformationMatrix();
		}

		// Token: 0x06005651 RID: 22097 RVA: 0x001B2710 File Offset: 0x001B0910
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			BuilderItem.BuilderItemState itemState = (BuilderItem.BuilderItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
			if (this.enableCollidersWhenReady)
			{
				bool flag = this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsRight) || this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsLeft);
				this.handsFreeOfCollidersTime += (flag ? 0f : Time.deltaTime);
				if (this.handsFreeOfCollidersTime > 0.1f)
				{
					this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
					this.enableCollidersWhenReady = false;
				}
			}
		}

		// Token: 0x06005652 RID: 22098 RVA: 0x001B27C8 File Offset: 0x001B09C8
		private bool IsOverlapping(List<InteractionPoint> interactionPoints)
		{
			if (interactionPoints == null)
			{
				return false;
			}
			for (int i = 0; i < interactionPoints.Count; i++)
			{
				if (interactionPoints[i] == this.gripInteractor)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005653 RID: 22099 RVA: 0x001B2802 File Offset: 0x001B0A02
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
		}

		// Token: 0x06005654 RID: 22100 RVA: 0x001B280A File Offset: 0x001B0A0A
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x06005655 RID: 22101 RVA: 0x001B2832 File Offset: 0x001B0A32
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			this.parentItem = null;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			return true;
		}

		// Token: 0x06005656 RID: 22102 RVA: 0x001B286D File Offset: 0x001B0A6D
		public void OnHoverOverTableStart(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x06005657 RID: 22103 RVA: 0x001B2876 File Offset: 0x001B0A76
		public void OnHoverOverTableEnd(BuilderTable table)
		{
			this.currTable = null;
		}

		// Token: 0x06005658 RID: 22104 RVA: 0x001B287F File Offset: 0x001B0A7F
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
		}

		// Token: 0x06005659 RID: 22105 RVA: 0x001B2888 File Offset: 0x001B0A88
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			base.transform.position = this.initialPosition;
			base.transform.rotation = this.initialRotation;
			if (this.worldShareableInstance != null)
			{
				this.worldShareableInstance.transform.position = this.initialPosition;
				this.worldShareableInstance.transform.rotation = this.initialRotation;
			}
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x0600565A RID: 22106 RVA: 0x000BB586 File Offset: 0x000B9786
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x0600565B RID: 22107 RVA: 0x001B290A File Offset: 0x001B0B0A
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

		// Token: 0x0600565C RID: 22108 RVA: 0x001B2943 File Offset: 0x001B0B43
		private bool ShouldPlayFX()
		{
			return this.previousItemState == BuilderItem.BuilderItemState.isHeld || this.previousItemState == BuilderItem.BuilderItemState.dropped;
		}

		// Token: 0x0600565D RID: 22109 RVA: 0x001B295A File Offset: 0x001B0B5A
		public static GameObject BuildEnvItem(int prefabHash, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(prefabHash, true);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x0600565E RID: 22110 RVA: 0x001B2978 File Offset: 0x001B0B78
		protected override void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
		{
			if (leftHand)
			{
				this.reliableState.leftHandAttachPos = localPosition;
				this.reliableState.leftHandAttachRot = localRotation;
			}
			else
			{
				this.reliableState.rightHandAttachPos = localPosition;
				this.reliableState.rightHandAttachRot = localRotation;
			}
			this.reliableState.dirty = true;
		}

		// Token: 0x0600565F RID: 22111 RVA: 0x001B29C6 File Offset: 0x001B0BC6
		public int GetPhotonViewId()
		{
			if (this.worldShareableInstance == null)
			{
				return -1;
			}
			return this.worldShareableInstance.ViewID;
		}

		// Token: 0x0400636B RID: 25451
		public BuilderItemReliableState reliableState;

		// Token: 0x0400636C RID: 25452
		public string builtItemPath;

		// Token: 0x0400636D RID: 25453
		public GameObject itemRoot;

		// Token: 0x0400636E RID: 25454
		private bool enableCollidersWhenReady;

		// Token: 0x0400636F RID: 25455
		private float handsFreeOfCollidersTime;

		// Token: 0x04006370 RID: 25456
		[NonSerialized]
		public BuilderPiece attachedPiece;

		// Token: 0x04006371 RID: 25457
		public List<Behaviour> onlyWhenPlacedBehaviours;

		// Token: 0x04006372 RID: 25458
		[NonSerialized]
		public BuilderItem parentItem;

		// Token: 0x04006373 RID: 25459
		public List<BuilderAttachGridPlane> gridPlanes;

		// Token: 0x04006374 RID: 25460
		public List<BuilderAttachEdge> edges;

		// Token: 0x04006375 RID: 25461
		private List<Collider> colliders;

		// Token: 0x04006376 RID: 25462
		private Transform parent;

		// Token: 0x04006377 RID: 25463
		private Vector3 initialPosition;

		// Token: 0x04006378 RID: 25464
		private Quaternion initialRotation;

		// Token: 0x04006379 RID: 25465
		private Vector3 initialGrabInteractorScale;

		// Token: 0x0400637A RID: 25466
		private BuilderTable currTable;

		// Token: 0x0400637B RID: 25467
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400637C RID: 25468
		public AudioClip snapAudio;

		// Token: 0x0400637D RID: 25469
		public AudioClip placeAudio;

		// Token: 0x0400637E RID: 25470
		public GameObject placeVFX;

		// Token: 0x0400637F RID: 25471
		private new BuilderItem.BuilderItemState previousItemState = BuilderItem.BuilderItemState.dropped;

		// Token: 0x02000DB4 RID: 3508
		private enum BuilderItemState
		{
			// Token: 0x04006381 RID: 25473
			isHeld = 1,
			// Token: 0x04006382 RID: 25474
			dropped,
			// Token: 0x04006383 RID: 25475
			placed = 4,
			// Token: 0x04006384 RID: 25476
			unused0 = 8,
			// Token: 0x04006385 RID: 25477
			none = 16
		}
	}
}
