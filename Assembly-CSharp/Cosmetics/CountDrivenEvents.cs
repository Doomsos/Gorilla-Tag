using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	public class CountDrivenEvents : MonoBehaviour
	{
		public int CurrentCount
		{
			get
			{
				return this.currentCount;
			}
		}

		private void OnEnable()
		{
			if (this.evaluateOnEnable)
			{
				this.CheckTriggers(this.currentCount, this.currentCount);
			}
		}

		private void OnValidate()
		{
			if (this.triggers == null)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount < 0)
				{
					this.triggers[i].triggerCount = 0;
				}
			}
		}

		public void Increment()
		{
			this.SetCount(this.currentCount + 1);
		}

		public void Decrement()
		{
			this.SetCount(this.currentCount - 1);
		}

		public void SetCount(int newCount)
		{
			int num = this.currentCount;
			if (this.wrapCount)
			{
				int highestTriggerCount = this.GetHighestTriggerCount();
				if (highestTriggerCount > 0)
				{
					int num2 = highestTriggerCount + 1;
					newCount = (newCount % num2 + num2) % num2;
				}
				else if (newCount < 0)
				{
					newCount = 0;
				}
			}
			else if (newCount < 0)
			{
				newCount = 0;
			}
			if (newCount == num)
			{
				return;
			}
			this.currentCount = newCount;
			UnityEvent<int> unityEvent = this.onCountChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentCount);
			}
			if (this.currentCount > num)
			{
				UnityEvent<int> unityEvent2 = this.onCountIncreased;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this.currentCount);
				}
			}
			else if (this.currentCount < num)
			{
				UnityEvent<int> unityEvent3 = this.onCountDecreased;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(this.currentCount);
				}
			}
			this.CheckTriggers(num, this.currentCount);
			if (this.currentCount == 0)
			{
				UnityEvent unityEvent4 = this.onCountResetToZero;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke();
				}
			}
			int highestTriggerCount2 = this.GetHighestTriggerCount();
			if (highestTriggerCount2 > 0 && this.currentCount == highestTriggerCount2)
			{
				UnityEvent unityEvent5 = this.onReachedMaxTrigger;
				if (unityEvent5 == null)
				{
					return;
				}
				unityEvent5.Invoke();
			}
		}

		[Tooltip("Resets all 'triggerOnce' flags, allowing one-time triggers to fire again.\n\nUse this when restarting a sequence, resetting an object,\nor testing trigger behavior multiple times in play mode.")]
		public void ResetTriggers()
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				this.triggers[i].hasTriggered = false;
			}
		}

		private int GetHighestTriggerCount()
		{
			int num = 0;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount > num)
				{
					num = this.triggers[i].triggerCount;
				}
			}
			return num;
		}

		private void CheckTriggers(int oldCount, int newCount)
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				CountDrivenEvents.CountTrigger countTrigger = this.triggers[i];
				if (!countTrigger.triggerOnce || !countTrigger.hasTriggered)
				{
					bool flag = false;
					if (this.wrapCount)
					{
						if (newCount == countTrigger.triggerCount)
						{
							flag = true;
						}
					}
					else if (oldCount < countTrigger.triggerCount && newCount >= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount > countTrigger.triggerCount && newCount <= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount == newCount && newCount == countTrigger.triggerCount)
					{
						flag = true;
					}
					if (flag)
					{
						UnityEvent onCountReached = countTrigger.onCountReached;
						if (onCountReached != null)
						{
							onCountReached.Invoke();
						}
						if (countTrigger.triggerOnce)
						{
							countTrigger.hasTriggered = true;
						}
					}
				}
			}
		}

		[Header("General Settings")]
		[Tooltip("If true, triggers will be evaluated once on enable using the initial count.")]
		[SerializeField]
		private bool evaluateOnEnable;

		[Tooltip("If enabled, the counter value will loop between 0 and the highest triggerCount.")]
		[SerializeField]
		private bool wrapCount;

		[Header("Count Triggers")]
		[SerializeField]
		private List<CountDrivenEvents.CountTrigger> triggers = new List<CountDrivenEvents.CountTrigger>();

		[Header("General Events")]
		public UnityEvent<int> onCountChanged;

		public UnityEvent<int> onCountIncreased;

		public UnityEvent<int> onCountDecreased;

		public UnityEvent onCountResetToZero;

		public UnityEvent onReachedMaxTrigger;

		[Header("Debug - Counter Settings")]
		[SerializeField]
		private int currentCount;

		[Serializable]
		public class CountTrigger
		{
			[Tooltip("The count value that triggers this event")]
			public int triggerCount;

			[Tooltip("Events to invoke when count reaches this value")]
			public UnityEvent onCountReached;

			[Tooltip("Should this trigger fire every time the count passes through this value, or only once?")]
			public bool triggerOnce;

			[NonSerialized]
			public bool hasTriggered;
		}
	}
}
