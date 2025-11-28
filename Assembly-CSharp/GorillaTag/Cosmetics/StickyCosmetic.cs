using System;
using GorillaExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200111A RID: 4378
	public class StickyCosmetic : MonoBehaviour
	{
		// Token: 0x06006D9B RID: 28059 RVA: 0x0023FDBF File Offset: 0x0023DFBF
		private void Start()
		{
			this.endRigidbody.isKinematic = false;
			this.endRigidbody.useGravity = false;
			this.UpdateState(StickyCosmetic.ObjectState.Idle);
		}

		// Token: 0x06006D9C RID: 28060 RVA: 0x0023FDE0 File Offset: 0x0023DFE0
		public void Extend()
		{
			if (this.currentState == StickyCosmetic.ObjectState.Idle || this.currentState == StickyCosmetic.ObjectState.Extending)
			{
				this.UpdateState(StickyCosmetic.ObjectState.Extending);
			}
		}

		// Token: 0x06006D9D RID: 28061 RVA: 0x0023FDFA File Offset: 0x0023DFFA
		public void Retract()
		{
			this.UpdateState(StickyCosmetic.ObjectState.Retracting);
		}

		// Token: 0x06006D9E RID: 28062 RVA: 0x0023FE04 File Offset: 0x0023E004
		private void Extend_Internal()
		{
			if (this.endRigidbody.isKinematic)
			{
				return;
			}
			this.rayLength = Mathf.Lerp(0f, this.maxObjectLength, this.blendShapeCosmetic.GetBlendValue() / this.blendShapeCosmetic.maxBlendShapeWeight);
			this.endRigidbody.MovePosition(this.startPosition.position + this.startPosition.forward * this.rayLength);
		}

		// Token: 0x06006D9F RID: 28063 RVA: 0x0023FE80 File Offset: 0x0023E080
		private void Retract_Internal()
		{
			this.endRigidbody.isKinematic = false;
			Vector3 vector = Vector3.MoveTowards(this.endRigidbody.position, this.startPosition.position, this.retractSpeed * Time.fixedDeltaTime);
			this.endRigidbody.MovePosition(vector);
		}

		// Token: 0x06006DA0 RID: 28064 RVA: 0x0023FED0 File Offset: 0x0023E0D0
		private void FixedUpdate()
		{
			switch (this.currentState)
			{
			case StickyCosmetic.ObjectState.Extending:
			{
				if (Time.time - this.extendingStartedTime > this.retractAfterSecond)
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoRetract);
				}
				this.Extend_Internal();
				RaycastHit raycastHit;
				if (Physics.Raycast(this.rayOrigin.position, this.rayOrigin.forward, ref raycastHit, this.rayLength, this.collisionLayers))
				{
					this.endRigidbody.isKinematic = true;
					this.endRigidbody.transform.parent = null;
					UnityEvent unityEvent = this.onStick;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.UpdateState(StickyCosmetic.ObjectState.Stuck);
				}
				break;
			}
			case StickyCosmetic.ObjectState.Retracting:
				if (Vector3.Distance(this.endRigidbody.position, this.startPosition.position) <= 0.01f)
				{
					this.endRigidbody.position = this.startPosition.position;
					Transform transform = this.endRigidbody.transform;
					transform.parent = this.endPositionParent;
					transform.localRotation = quaternion.identity;
					transform.localScale = Vector3.one;
					if (this.lastState == StickyCosmetic.ObjectState.AutoUnstuck || this.lastState == StickyCosmetic.ObjectState.AutoRetract)
					{
						this.UpdateState(StickyCosmetic.ObjectState.JustRetracted);
					}
					else
					{
						this.UpdateState(StickyCosmetic.ObjectState.Idle);
					}
				}
				else
				{
					this.Retract_Internal();
				}
				break;
			case StickyCosmetic.ObjectState.Stuck:
				if (this.endRigidbody.isKinematic && (this.endRigidbody.position - this.startPosition.position).IsLongerThan(this.autoRetractThreshold))
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoUnstuck);
				}
				break;
			case StickyCosmetic.ObjectState.AutoUnstuck:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			case StickyCosmetic.ObjectState.AutoRetract:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			}
			Debug.DrawRay(this.rayOrigin.position, this.rayOrigin.forward * this.rayLength, Color.red);
		}

		// Token: 0x06006DA1 RID: 28065 RVA: 0x002400B0 File Offset: 0x0023E2B0
		private void UpdateState(StickyCosmetic.ObjectState newState)
		{
			this.lastState = this.currentState;
			if (this.lastState == StickyCosmetic.ObjectState.Stuck && newState != this.currentState)
			{
				this.onUnstick.Invoke();
			}
			if (this.lastState != StickyCosmetic.ObjectState.Extending && newState == StickyCosmetic.ObjectState.Extending)
			{
				this.extendingStartedTime = Time.time;
			}
			this.currentState = newState;
		}

		// Token: 0x04007EFD RID: 32509
		[Tooltip("Optional reference to an UpdateBlendShapeCosmetic component. Used to drive extension length based on blend shape weight (e.g. finger flex input).")]
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04007EFE RID: 32510
		[Tooltip("Defines which physics layers this sticky object can attach to when extending (checked via raycast).")]
		[SerializeField]
		private LayerMask collisionLayers;

		// Token: 0x04007EFF RID: 32511
		[Tooltip("Transform origin from which the raycast will be fired forward to detect stickable surfaces.")]
		[SerializeField]
		private Transform rayOrigin;

		// Token: 0x04007F00 RID: 32512
		[Tooltip("Transform representing the start or base position of the sticky object (where extension originates).")]
		[SerializeField]
		private Transform startPosition;

		// Token: 0x04007F01 RID: 32513
		[Tooltip("Rigidbody controlling the physical end of the sticky object (the part that extends and can attach).")]
		[SerializeField]
		private Rigidbody endRigidbody;

		// Token: 0x04007F02 RID: 32514
		[Tooltip("Parent transform the end object will reattach to when fully retracted. This keeps local transform resets consistent.")]
		[SerializeField]
		private Transform endPositionParent;

		// Token: 0x04007F03 RID: 32515
		[Tooltip("Maximum distance the object can extend from its start position (in meters).")]
		[SerializeField]
		private float maxObjectLength = 0.7f;

		// Token: 0x04007F04 RID: 32516
		[Tooltip("If the sticky object remains stuck but the distance from start exceeds this threshold, it will automatically unstuck and begin retracting.")]
		[SerializeField]
		private float autoRetractThreshold = 1f;

		// Token: 0x04007F05 RID: 32517
		[Tooltip("Speed (units per second) at which the end rigidbody retracts toward its start position when returning.")]
		[SerializeField]
		private float retractSpeed = 5f;

		// Token: 0x04007F06 RID: 32518
		[Tooltip("If the sticky end remains extended but doesn’t stick to anything, it will automatically start retracting after this many seconds.")]
		[SerializeField]
		private float retractAfterSecond = 2f;

		// Token: 0x04007F07 RID: 32519
		[Tooltip("Invoked when the sticky object successfully attaches to a surface.")]
		public UnityEvent onStick;

		// Token: 0x04007F08 RID: 32520
		[Tooltip("Invoked when the sticky object becomes unstuck — either manually or automatically.")]
		public UnityEvent onUnstick;

		// Token: 0x04007F09 RID: 32521
		private StickyCosmetic.ObjectState currentState;

		// Token: 0x04007F0A RID: 32522
		private float rayLength;

		// Token: 0x04007F0B RID: 32523
		private bool stick;

		// Token: 0x04007F0C RID: 32524
		private StickyCosmetic.ObjectState lastState;

		// Token: 0x04007F0D RID: 32525
		private float extendingStartedTime;

		// Token: 0x0200111B RID: 4379
		private enum ObjectState
		{
			// Token: 0x04007F0F RID: 32527
			Extending,
			// Token: 0x04007F10 RID: 32528
			Retracting,
			// Token: 0x04007F11 RID: 32529
			Stuck,
			// Token: 0x04007F12 RID: 32530
			JustRetracted,
			// Token: 0x04007F13 RID: 32531
			Idle,
			// Token: 0x04007F14 RID: 32532
			AutoUnstuck,
			// Token: 0x04007F15 RID: 32533
			AutoRetract
		}
	}
}
