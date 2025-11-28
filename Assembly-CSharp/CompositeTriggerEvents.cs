using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000C3C RID: 3132
public class CompositeTriggerEvents : MonoBehaviour
{
	// Token: 0x17000732 RID: 1842
	// (get) Token: 0x06004CD8 RID: 19672 RVA: 0x0018EE7E File Offset: 0x0018D07E
	private Dictionary<Collider, int> CollderMasks
	{
		get
		{
			return this.overlapMask;
		}
	}

	// Token: 0x14000089 RID: 137
	// (add) Token: 0x06004CD9 RID: 19673 RVA: 0x0018EE88 File Offset: 0x0018D088
	// (remove) Token: 0x06004CDA RID: 19674 RVA: 0x0018EEC0 File Offset: 0x0018D0C0
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerEnter;

	// Token: 0x1400008A RID: 138
	// (add) Token: 0x06004CDB RID: 19675 RVA: 0x0018EEF8 File Offset: 0x0018D0F8
	// (remove) Token: 0x06004CDC RID: 19676 RVA: 0x0018EF30 File Offset: 0x0018D130
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerExit;

	// Token: 0x06004CDD RID: 19677 RVA: 0x0018EF68 File Offset: 0x0018D168
	private void Awake()
	{
		if (this.individualTriggerColliders.Count > 32)
		{
			Debug.LogError("The max number of triggers was exceeded in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
		}
		for (int i = 0; i < this.individualTriggerColliders.Count; i++)
		{
			TriggerEventNotifier triggerEventNotifier = this.individualTriggerColliders[i].gameObject.AddComponent<TriggerEventNotifier>();
			triggerEventNotifier.maskIndex = i;
			triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
			triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
			this.triggerEventNotifiers.Add(triggerEventNotifier);
		}
	}

	// Token: 0x06004CDE RID: 19678 RVA: 0x0018F008 File Offset: 0x0018D208
	public void AddCollider(Collider colliderToAdd)
	{
		if (this.individualTriggerColliders.Count >= 32)
		{
			Debug.LogError("The max number of triggers are already present in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
			return;
		}
		this.individualTriggerColliders.Add(colliderToAdd);
		TriggerEventNotifier triggerEventNotifier = colliderToAdd.gameObject.AddComponent<TriggerEventNotifier>();
		triggerEventNotifier.maskIndex = this.GetNextMaskIndex();
		triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
		triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
		this.triggerEventNotifiers.Add(triggerEventNotifier);
		this.triggerEventNotifiers.Sort((TriggerEventNotifier a, TriggerEventNotifier b) => a.maskIndex.CompareTo(b.maskIndex));
	}

	// Token: 0x06004CDF RID: 19679 RVA: 0x0018F0C4 File Offset: 0x0018D2C4
	public void RemoveCollider(Collider colliderToRemove)
	{
		TriggerEventNotifier component = colliderToRemove.gameObject.GetComponent<TriggerEventNotifier>();
		if (component.IsNotNull())
		{
			foreach (KeyValuePair<Collider, int> keyValuePair in new Dictionary<Collider, int>(this.overlapMask))
			{
				this.TriggerExitReceiver(component, keyValuePair.Key);
			}
			component.maskIndex = -1;
			component.TriggerEnterEvent -= this.TriggerEnterReceiver;
			component.TriggerExitEvent -= this.TriggerExitReceiver;
			this.triggerEventNotifiers.Remove(component);
		}
		this.individualTriggerColliders.Remove(colliderToRemove);
	}

	// Token: 0x06004CE0 RID: 19680 RVA: 0x0018F17C File Offset: 0x0018D37C
	public void ResetColliders(bool sendExitEvent = true)
	{
		this.individualTriggerColliders.Clear();
		for (int i = this.triggerEventNotifiers.Count - 1; i >= 0; i--)
		{
			if (this.triggerEventNotifiers[i].IsNull())
			{
				this.triggerEventNotifiers.RemoveAt(i);
			}
			else
			{
				this.triggerEventNotifiers[i].maskIndex = -1;
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
				this.triggerEventNotifiers.RemoveAt(i);
			}
		}
		if (sendExitEvent)
		{
			foreach (KeyValuePair<Collider, int> keyValuePair in this.overlapMask)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(keyValuePair.Key);
				}
			}
		}
		this.overlapMask.Clear();
	}

	// Token: 0x06004CE1 RID: 19681 RVA: 0x0018F290 File Offset: 0x0018D490
	public int GetNumColliders()
	{
		return this.individualTriggerColliders.Count;
	}

	// Token: 0x06004CE2 RID: 19682 RVA: 0x0018F2A0 File Offset: 0x0018D4A0
	public int GetNextMaskIndex()
	{
		if (this.individualTriggerColliders.Count >= 32)
		{
			Debug.LogError("The max number of triggers are already present in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
			return -1;
		}
		int num = 0;
		int num2 = 0;
		while (num2 < this.triggerEventNotifiers.Count && this.triggerEventNotifiers[num2].maskIndex == num)
		{
			num++;
			num2++;
		}
		return num;
	}

	// Token: 0x06004CE3 RID: 19683 RVA: 0x0018F310 File Offset: 0x0018D510
	private void OnDestroy()
	{
		for (int i = 0; i < this.triggerEventNotifiers.Count; i++)
		{
			if (this.triggerEventNotifiers[i] != null)
			{
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
			}
		}
	}

	// Token: 0x06004CE4 RID: 19684 RVA: 0x0018F384 File Offset: 0x0018D584
	public void TriggerEnterReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, ref num))
		{
			num = this.SetMaskIndexTrue(num, notifier.maskIndex);
			this.overlapMask[other] = num;
			return;
		}
		int num2 = this.SetMaskIndexTrue(0, notifier.maskIndex);
		this.overlapMask.Add(other, num2);
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x06004CE5 RID: 19685 RVA: 0x0018F3EC File Offset: 0x0018D5EC
	public void TriggerExitReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, ref num))
		{
			num = this.SetMaskIndexFalse(num, notifier.maskIndex);
			if (num == 0)
			{
				this.overlapMask.Remove(other);
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit == null)
				{
					return;
				}
				compositeTriggerExit(other);
				return;
			}
			else
			{
				this.overlapMask[other] = num;
			}
		}
	}

	// Token: 0x06004CE6 RID: 19686 RVA: 0x0018F448 File Offset: 0x0018D648
	public void ResetColliderMask(Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, ref num))
		{
			if (num != 0)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(other);
				}
			}
			this.overlapMask.Remove(other);
		}
	}

	// Token: 0x06004CE7 RID: 19687 RVA: 0x0018F487 File Offset: 0x0018D687
	public void CompositeTriggerEnterReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x06004CE8 RID: 19688 RVA: 0x0018F49A File Offset: 0x0018D69A
	public void CompositeTriggerExitReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
		if (compositeTriggerExit == null)
		{
			return;
		}
		compositeTriggerExit(other);
	}

	// Token: 0x06004CE9 RID: 19689 RVA: 0x0018F4AD File Offset: 0x0018D6AD
	private bool TestMaskIndex(int mask, int index)
	{
		return (mask & 1 << index) != 0;
	}

	// Token: 0x06004CEA RID: 19690 RVA: 0x0018F4BA File Offset: 0x0018D6BA
	private int SetMaskIndexTrue(int mask, int index)
	{
		return mask | 1 << index;
	}

	// Token: 0x06004CEB RID: 19691 RVA: 0x0018F4C4 File Offset: 0x0018D6C4
	private int SetMaskIndexFalse(int mask, int index)
	{
		return mask & ~(1 << index);
	}

	// Token: 0x06004CEC RID: 19692 RVA: 0x0018F4D0 File Offset: 0x0018D6D0
	private string MaskToString(int mask)
	{
		string text = "";
		for (int i = 31; i >= 0; i--)
		{
			text += (this.TestMaskIndex(mask, i) ? "1" : "0");
		}
		return text;
	}

	// Token: 0x04005C87 RID: 23687
	[SerializeField]
	private List<Collider> individualTriggerColliders = new List<Collider>();

	// Token: 0x04005C88 RID: 23688
	private List<TriggerEventNotifier> triggerEventNotifiers = new List<TriggerEventNotifier>();

	// Token: 0x04005C89 RID: 23689
	private Dictionary<Collider, int> overlapMask = new Dictionary<Collider, int>();

	// Token: 0x02000C3D RID: 3133
	// (Invoke) Token: 0x06004CEF RID: 19695
	public delegate void TriggerEvent(Collider collider);
}
