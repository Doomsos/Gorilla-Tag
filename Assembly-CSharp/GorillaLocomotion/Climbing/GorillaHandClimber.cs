using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000FAF RID: 4015
	public class GorillaHandClimber : MonoBehaviour
	{
		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x060064D0 RID: 25808 RVA: 0x0020ED0C File Offset: 0x0020CF0C
		public bool isClimbingOrGrabbing
		{
			get
			{
				return this.isClimbing || this.grabber.isGrabbing;
			}
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x0020ED23 File Offset: 0x0020CF23
		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
			this.grabber = base.GetComponent<GorillaGrabber>();
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x0020ED40 File Offset: 0x0020CF40
		public void CheckHandClimber()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				GorillaClimbable gorillaClimbable = this.potentialClimbables[i];
				if (gorillaClimbable == null || !gorillaClimbable.isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
				else if (gorillaClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && this.player.scale > 0.99f)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && this.CanInitiateClimb() && closestClimbable != this.dontReclimbLast)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
					}
					else
					{
						this.player.BeginClimbing(closestClimbable, this, null);
					}
				}
			}
			else if (grabRelease && this.canRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
			this.grabber.CheckGrabber(this.CanInitiateClimb() && grab);
		}

		// Token: 0x060064D3 RID: 25811 RVA: 0x0020EEB8 File Offset: 0x0020D0B8
		private bool CanInitiateClimb()
		{
			return !this.isClimbing && !this.equipmentInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.builderPieceInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.IsGrabDisabled(this.xrNode) && !GamePlayerLocal.IsHandHolding(this.xrNode) && !this.player.inOverlay;
		}

		// Token: 0x060064D4 RID: 25812 RVA: 0x0020EF28 File Offset: 0x0020D128
		public void SetCanRelease(bool canRelease)
		{
			this.canRelease = canRelease;
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x0020EF34 File Offset: 0x0020D134
		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			Bounds bounds = this.col.bounds;
			float num = 0.15f;
			GorillaClimbable result = null;
			foreach (GorillaClimbable gorillaClimbable in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable.colliderCache)
				{
					if (!bounds.Intersects(gorillaClimbable.colliderCache.bounds))
					{
						continue;
					}
					Vector3 vector = gorillaClimbable.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, vector);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable.transform.position);
				}
				if (num2 < num)
				{
					result = gorillaClimbable;
					num = num2;
				}
			}
			return result;
		}

		// Token: 0x060064D6 RID: 25814 RVA: 0x0020F034 File Offset: 0x0020D234
		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(ref gorillaClimbable))
			{
				this.potentialClimbables.Add(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(ref gorillaClimbableRef))
			{
				this.potentialClimbables.Add(gorillaClimbableRef);
			}
		}

		// Token: 0x060064D7 RID: 25815 RVA: 0x0020F070 File Offset: 0x0020D270
		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable gorillaClimbable;
			if (other.TryGetComponent<GorillaClimbable>(ref gorillaClimbable))
			{
				this.potentialClimbables.Remove(gorillaClimbable);
				return;
			}
			GorillaClimbableRef gorillaClimbableRef;
			if (other.TryGetComponent<GorillaClimbableRef>(ref gorillaClimbableRef))
			{
				this.potentialClimbables.Remove(gorillaClimbableRef);
			}
		}

		// Token: 0x060064D8 RID: 25816 RVA: 0x0020F0AC File Offset: 0x0020D2AC
		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		// Token: 0x04007490 RID: 29840
		[SerializeField]
		private GTPlayer player;

		// Token: 0x04007491 RID: 29841
		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		// Token: 0x04007492 RID: 29842
		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		// Token: 0x04007493 RID: 29843
		[Header("Non-hand input should have the component disabled")]
		public XRNode xrNode = 4;

		// Token: 0x04007494 RID: 29844
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x04007495 RID: 29845
		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		// Token: 0x04007496 RID: 29846
		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		// Token: 0x04007497 RID: 29847
		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		// Token: 0x04007498 RID: 29848
		public GorillaGrabber grabber;

		// Token: 0x04007499 RID: 29849
		public Transform handRoot;

		// Token: 0x0400749A RID: 29850
		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		// Token: 0x0400749B RID: 29851
		private const float DIST_FOR_GRAB = 0.15f;

		// Token: 0x0400749C RID: 29852
		private Collider col;

		// Token: 0x0400749D RID: 29853
		private bool canRelease = true;
	}
}
