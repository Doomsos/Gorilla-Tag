using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001106 RID: 4358
	[RequireComponent(typeof(Collider))]
	public class OnCollisionEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06006D18 RID: 27928 RVA: 0x0023D191 File Offset: 0x0023B391
		private bool IsMyItem()
		{
			return this.rig != null && this.rig.isOfflineVRRig;
		}

		// Token: 0x06006D19 RID: 27929 RVA: 0x0023D1B0 File Offset: 0x0023B3B0
		private void Awake()
		{
			this.myCollider = base.GetComponent<Collider>();
			if (this.myCollider == null)
			{
				Debug.LogError("OnCollisionEventsCosmetic requires a Collider on the same GameObject.");
				base.enabled = false;
				return;
			}
			if (this.myCollider.isTrigger)
			{
				Debug.LogWarning("OnCollisionEventsCosmetic: Collider is set to Trigger. OnCollision will not fire. Set it to non-trigger for collisions.");
			}
			this.rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			List<OnCollisionEventsCosmetic.Listener> list = new List<OnCollisionEventsCosmetic.Listener>();
			List<OnCollisionEventsCosmetic.Listener> list2 = new List<OnCollisionEventsCosmetic.Listener>();
			List<OnCollisionEventsCosmetic.Listener> list3 = new List<OnCollisionEventsCosmetic.Listener>();
			if (this.eventListeners != null)
			{
				for (int i = 0; i < this.eventListeners.Length; i++)
				{
					OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
					if (listener.tagSet == null)
					{
						if (listener.collisionTagsList != null && listener.collisionTagsList.Count > 0)
						{
							listener.tagSet = new HashSet<string>(listener.collisionTagsList);
						}
						else
						{
							listener.tagSet = new HashSet<string>();
						}
					}
					if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionEnter)
					{
						list.Add(listener);
					}
					else if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionStay)
					{
						list2.Add(listener);
					}
					else if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionExit)
					{
						list3.Add(listener);
					}
				}
			}
			this.enterListeners = ((list.Count > 0) ? list.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
			this.stayListeners = ((list2.Count > 0) ? list2.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
			this.exitListeners = ((list3.Count > 0) ? list3.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
		}

		// Token: 0x06006D1A RID: 27930 RVA: 0x0023D327 File Offset: 0x0023B527
		private void OnCollisionEnter(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.enterListeners, collision);
		}

		// Token: 0x06006D1B RID: 27931 RVA: 0x0023D33F File Offset: 0x0023B53F
		private void OnCollisionStay(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.stayListeners, collision);
		}

		// Token: 0x06006D1C RID: 27932 RVA: 0x0023D357 File Offset: 0x0023B557
		private void OnCollisionExit(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.exitListeners, collision);
		}

		// Token: 0x06006D1D RID: 27933 RVA: 0x0023D370 File Offset: 0x0023B570
		private static bool IsCollisionUsable(Collision collision)
		{
			if (collision == null)
			{
				return false;
			}
			Collider collider = collision.collider;
			if (collider == null)
			{
				return false;
			}
			GameObject gameObject = collider.gameObject;
			return !(gameObject == null) && gameObject.activeInHierarchy;
		}

		// Token: 0x06006D1E RID: 27934 RVA: 0x0023D3B0 File Offset: 0x0023B5B0
		private void Dispatch(OnCollisionEventsCosmetic.Listener[] listeners, Collision collision)
		{
			if (listeners == null || listeners.Length == 0)
			{
				return;
			}
			Collider collider = collision.collider;
			GameObject gameObject = (collider != null) ? collider.gameObject : null;
			if (gameObject == null)
			{
				return;
			}
			int layer = gameObject.layer;
			bool flag = this.parentTransferable && this.parentTransferable.InLeftHand();
			Vector3 vector = (this.myCollider != null) ? this.myCollider.bounds.center : base.transform.position;
			Vector3 vector2;
			if (collision.contactCount > 0)
			{
				vector2 = collision.GetContact(0).point;
			}
			else
			{
				vector2 = collider.ClosestPoint(vector);
			}
			foreach (OnCollisionEventsCosmetic.Listener listener in listeners)
			{
				if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnCollisionEventsCosmetic.CompareTagAny(gameObject, listener.tagSet)) && (1 << layer & listener.collisionLayerMask.value) != 0)
				{
					if (listener.listenerComponent != null)
					{
						listener.listenerComponent.Invoke(flag, collision);
					}
					if (listener.listenerComponentContactPoint != null)
					{
						listener.listenerComponentContactPoint.Invoke(vector2);
					}
					VRRig componentInParent = gameObject.GetComponentInParent<VRRig>();
					if (componentInParent != null && listener.onCollidedVRRig != null)
					{
						listener.onCollidedVRRig.Invoke(componentInParent);
					}
				}
			}
		}

		// Token: 0x06006D1F RID: 27935 RVA: 0x0023D550 File Offset: 0x0023B750
		private static bool CompareTagAny(GameObject go, HashSet<string> tagSet)
		{
			if (tagSet == null || tagSet.Count == 0)
			{
				return true;
			}
			foreach (string text in tagSet)
			{
				if (!string.IsNullOrEmpty(text) && go.CompareTag(text))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006D20 RID: 27936 RVA: 0x0023D5BC File Offset: 0x0023B7BC
		private bool IsTagValid(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return listener == null || (listener.tagSet == null || listener.tagSet.Count == 0) || OnCollisionEventsCosmetic.CompareTagAny(obj, listener.tagSet);
		}

		// Token: 0x04007E3A RID: 32314
		[Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to collide with (layers/tags), and which UnityEvents to fire.")]
		public OnCollisionEventsCosmetic.Listener[] eventListeners = new OnCollisionEventsCosmetic.Listener[0];

		// Token: 0x04007E3B RID: 32315
		private OnCollisionEventsCosmetic.Listener[] enterListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04007E3C RID: 32316
		private OnCollisionEventsCosmetic.Listener[] stayListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04007E3D RID: 32317
		private OnCollisionEventsCosmetic.Listener[] exitListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04007E3E RID: 32318
		private Collider myCollider;

		// Token: 0x04007E3F RID: 32319
		private VRRig rig;

		// Token: 0x04007E40 RID: 32320
		private TransferrableObject parentTransferable;

		// Token: 0x02001107 RID: 4359
		[Serializable]
		public class Listener
		{
			// Token: 0x04007E41 RID: 32321
			[Tooltip("Only collisions with objects on these layers will be considered.")]
			public LayerMask collisionLayerMask;

			// Token: 0x04007E42 RID: 32322
			[Tooltip("Optional tag whitelist. If non-empty, collisions must match at least one of these tags.")]
			public List<string> collisionTagsList = new List<string>();

			// Token: 0x04007E43 RID: 32323
			[Tooltip("Choose which collision phase triggers this listener: Enter, Stay, or Exit.")]
			public OnCollisionEventsCosmetic.EventType eventType;

			// Token: 0x04007E44 RID: 32324
			public UnityEvent<bool, Collision> listenerComponent;

			// Token: 0x04007E45 RID: 32325
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x04007E46 RID: 32326
			public UnityEvent<VRRig> onCollidedVRRig;

			// Token: 0x04007E47 RID: 32327
			[Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04007E48 RID: 32328
			[Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04007E49 RID: 32329
			[NonSerialized]
			public HashSet<string> tagSet;
		}

		// Token: 0x02001108 RID: 4360
		public enum EventType
		{
			// Token: 0x04007E4B RID: 32331
			CollisionEnter,
			// Token: 0x04007E4C RID: 32332
			CollisionStay,
			// Token: 0x04007E4D RID: 32333
			CollisionExit
		}
	}
}
