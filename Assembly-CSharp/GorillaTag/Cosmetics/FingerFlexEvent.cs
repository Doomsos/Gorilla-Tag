using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010F2 RID: 4338
	public class FingerFlexEvent : MonoBehaviourTick
	{
		// Token: 0x06006CC5 RID: 27845 RVA: 0x0023B6B3 File Offset: 0x002398B3
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		}

		// Token: 0x06006CC6 RID: 27846 RVA: 0x0023B6CD File Offset: 0x002398CD
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x06006CC7 RID: 27847 RVA: 0x0023B6EC File Offset: 0x002398EC
		public override void Tick()
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				FingerFlexEvent.Listener listener = this.eventListeners[i];
				this.FireEvents(listener);
			}
		}

		// Token: 0x06006CC8 RID: 27848 RVA: 0x0023B71C File Offset: 0x0023991C
		private void FireEvents(FingerFlexEvent.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			if (!this.ignoreTransferable && listener.fireOnlyWhileHeld && this.parentTransferable && !this.parentTransferable.InHand() && listener.eventType == FingerFlexEvent.EventType.OnFingerReleased)
			{
				if (listener.fingerRightLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(false, 0f);
					}
					listener.fingerRightLastValue = 0f;
				}
				if (listener.fingerLeftLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(true, 0f);
					}
					listener.fingerLeftLastValue = 0f;
				}
			}
			if (!this.ignoreTransferable && this.parentTransferable && listener.fireOnlyWhileHeld && !this.parentTransferable.InHand())
			{
				return;
			}
			switch (this.fingerType)
			{
			case FingerFlexEvent.FingerType.Thumb:
			{
				float calcT = this._rig.leftThumb.calcT;
				float calcT2 = this._rig.rightThumb.calcT;
				this.FireEvents(listener, calcT, calcT2);
				return;
			}
			case FingerFlexEvent.FingerType.Index:
			{
				float calcT3 = this._rig.leftIndex.calcT;
				float calcT4 = this._rig.rightIndex.calcT;
				this.FireEvents(listener, calcT3, calcT4);
				return;
			}
			case FingerFlexEvent.FingerType.Middle:
			{
				float calcT5 = this._rig.leftMiddle.calcT;
				float calcT6 = this._rig.rightMiddle.calcT;
				this.FireEvents(listener, calcT5, calcT6);
				return;
			}
			case FingerFlexEvent.FingerType.IndexAndMiddleMin:
			{
				float leftFinger = Mathf.Min(this._rig.leftIndex.calcT, this._rig.leftMiddle.calcT);
				float rightFinger = Mathf.Min(this._rig.rightIndex.calcT, this._rig.rightMiddle.calcT);
				this.FireEvents(listener, leftFinger, rightFinger);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06006CC9 RID: 27849 RVA: 0x0023B900 File Offset: 0x00239B00
		private void FireEvents(FingerFlexEvent.Listener listener, float leftFinger, float rightFinger)
		{
			if ((this.ignoreTransferable && listener.checkLeftHand) || (this.parentTransferable && this.FingerFlexValidation(true)))
			{
				this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
				return;
			}
			if ((this.ignoreTransferable && !listener.checkLeftHand) || (this.parentTransferable && this.FingerFlexValidation(false)))
			{
				this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
				return;
			}
			this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
			this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
		}

		// Token: 0x06006CCA RID: 27850 RVA: 0x0023B998 File Offset: 0x00239B98
		private void CheckFingerValue(FingerFlexEvent.Listener listener, float fingerValue, bool isLeft, ref float lastValue)
		{
			if (fingerValue > listener.fingerFlexValue)
			{
				listener.frameCounter++;
			}
			switch (listener.eventType)
			{
			case FingerFlexEvent.EventType.OnFingerFlexed:
				if (fingerValue > listener.fingerFlexValue && lastValue < listener.fingerFlexValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(isLeft, fingerValue);
					}
				}
				break;
			case FingerFlexEvent.EventType.OnFingerReleased:
				if (fingerValue <= listener.fingerReleaseValue && lastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			case FingerFlexEvent.EventType.OnFingerFlexStayed:
				if (fingerValue > listener.fingerFlexValue && lastValue >= listener.fingerFlexValue && listener.frameCounter % listener.frameInterval == 0)
				{
					UnityEvent<bool, float> listenerComponent3 = listener.listenerComponent;
					if (listenerComponent3 != null)
					{
						listenerComponent3.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			}
			lastValue = fingerValue;
		}

		// Token: 0x06006CCB RID: 27851 RVA: 0x0023BA7A File Offset: 0x00239C7A
		private bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.parentTransferable.InLeftHand() || isLeftHand) && (this.parentTransferable.InLeftHand() || !isLeftHand);
		}

		// Token: 0x04007DBA RID: 32186
		[SerializeField]
		public bool ignoreTransferable;

		// Token: 0x04007DBB RID: 32187
		[SerializeField]
		private FingerFlexEvent.FingerType fingerType = FingerFlexEvent.FingerType.Index;

		// Token: 0x04007DBC RID: 32188
		public FingerFlexEvent.Listener[] eventListeners = new FingerFlexEvent.Listener[0];

		// Token: 0x04007DBD RID: 32189
		private VRRig _rig;

		// Token: 0x04007DBE RID: 32190
		private TransferrableObject parentTransferable;

		// Token: 0x020010F3 RID: 4339
		[Serializable]
		public class Listener
		{
			// Token: 0x04007DBF RID: 32191
			public FingerFlexEvent.EventType eventType;

			// Token: 0x04007DC0 RID: 32192
			public UnityEvent<bool, float> listenerComponent;

			// Token: 0x04007DC1 RID: 32193
			public float fingerFlexValue = 0.75f;

			// Token: 0x04007DC2 RID: 32194
			public float fingerReleaseValue = 0.01f;

			// Token: 0x04007DC3 RID: 32195
			[Tooltip("How many frames should pass to fire a finger flex stayed event")]
			public int frameInterval = 20;

			// Token: 0x04007DC4 RID: 32196
			[Tooltip("This event will be fired for everyone in the room (synced) by default unless you uncheck this box so that it will be fired only for the local player.")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04007DC5 RID: 32197
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04007DC6 RID: 32198
			[Tooltip("Whether to check the left hand or the right hand, only works if \"ignoreTransferable\" is true.")]
			public bool checkLeftHand;

			// Token: 0x04007DC7 RID: 32199
			internal int frameCounter;

			// Token: 0x04007DC8 RID: 32200
			internal float fingerRightLastValue;

			// Token: 0x04007DC9 RID: 32201
			internal float fingerLeftLastValue;
		}

		// Token: 0x020010F4 RID: 4340
		public enum EventType
		{
			// Token: 0x04007DCB RID: 32203
			OnFingerFlexed,
			// Token: 0x04007DCC RID: 32204
			OnFingerReleased,
			// Token: 0x04007DCD RID: 32205
			OnFingerFlexStayed
		}

		// Token: 0x020010F5 RID: 4341
		private enum FingerType
		{
			// Token: 0x04007DCF RID: 32207
			Thumb,
			// Token: 0x04007DD0 RID: 32208
			Index,
			// Token: 0x04007DD1 RID: 32209
			Middle,
			// Token: 0x04007DD2 RID: 32210
			IndexAndMiddleMin
		}
	}
}
