using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010BF RID: 4287
	public class BackpackGrabbableCosmetic : HoldableObject
	{
		// Token: 0x06006B71 RID: 27505 RVA: 0x0023474B File Offset: 0x0023294B
		private void Awake()
		{
			this.currentItemsCount = this.startItemsCount;
			this.canGrab = true;
		}

		// Token: 0x06006B72 RID: 27506 RVA: 0x00002789 File Offset: 0x00000989
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x06006B73 RID: 27507 RVA: 0x00002789 File Offset: 0x00000989
		public override void DropItemCleanup()
		{
		}

		// Token: 0x06006B74 RID: 27508 RVA: 0x00234760 File Offset: 0x00232960
		public void Update()
		{
			if (!this.canGrab && Time.time - this.lastGrabTime >= this.coolDownTimer)
			{
				this.canGrab = true;
			}
		}

		// Token: 0x06006B75 RID: 27509 RVA: 0x00234788 File Offset: 0x00232988
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (this.IsEmpty())
			{
				Debug.LogWarning("Can't remove item, Backpack is empty, need to refill.");
				return;
			}
			if (!this.canGrab)
			{
				return;
			}
			this.lastGrabTime = Time.time;
			this.canGrab = false;
			SnowballThrowable snowballThrowable;
			((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
			this.RemoveItem();
		}

		// Token: 0x06006B76 RID: 27510 RVA: 0x002347F7 File Offset: 0x002329F7
		public void AddItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.maxCapacity <= this.currentItemsCount)
			{
				Debug.LogWarning("Can't add item, backpack is at full capacity.");
				return;
			}
			this.currentItemsCount++;
			this.UpdateState();
		}

		// Token: 0x06006B77 RID: 27511 RVA: 0x0023482F File Offset: 0x00232A2F
		public void RemoveItem()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount < 0)
			{
				Debug.LogWarning("Can't remove item, Backpack is empty.");
				return;
			}
			this.currentItemsCount--;
			this.UpdateState();
		}

		// Token: 0x06006B78 RID: 27512 RVA: 0x00234862 File Offset: 0x00232A62
		public void RefillBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.startItemsCount)
			{
				return;
			}
			this.currentItemsCount = this.startItemsCount;
			this.UpdateState();
		}

		// Token: 0x06006B79 RID: 27513 RVA: 0x0023488E File Offset: 0x00232A8E
		public void EmptyBackpack()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == 0)
			{
				return;
			}
			this.currentItemsCount = 0;
			this.UpdateState();
		}

		// Token: 0x06006B7A RID: 27514 RVA: 0x002348AF File Offset: 0x00232AAF
		public bool IsFull()
		{
			return !this.useCapacity || this.maxCapacity == this.currentItemsCount;
		}

		// Token: 0x06006B7B RID: 27515 RVA: 0x002348CA File Offset: 0x00232ACA
		public bool IsEmpty()
		{
			return this.useCapacity && this.currentItemsCount == 0;
		}

		// Token: 0x06006B7C RID: 27516 RVA: 0x002348E0 File Offset: 0x00232AE0
		private void UpdateState()
		{
			if (!this.useCapacity)
			{
				return;
			}
			if (this.currentItemsCount == this.maxCapacity)
			{
				UnityEvent onReachedMaxCapacity = this.OnReachedMaxCapacity;
				if (onReachedMaxCapacity == null)
				{
					return;
				}
				onReachedMaxCapacity.Invoke();
				return;
			}
			else
			{
				if (this.currentItemsCount != 0)
				{
					if (this.currentItemsCount == this.startItemsCount)
					{
						UnityEvent onRefilled = this.OnRefilled;
						if (onRefilled == null)
						{
							return;
						}
						onRefilled.Invoke();
					}
					return;
				}
				UnityEvent onFullyEmptied = this.OnFullyEmptied;
				if (onFullyEmptied == null)
				{
					return;
				}
				onFullyEmptied.Invoke();
				return;
			}
		}

		// Token: 0x04007BE9 RID: 31721
		[GorillaSoundLookup]
		public int materialIndex;

		// Token: 0x04007BEA RID: 31722
		[SerializeField]
		private bool useCapacity = true;

		// Token: 0x04007BEB RID: 31723
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04007BEC RID: 31724
		[SerializeField]
		private int maxCapacity;

		// Token: 0x04007BED RID: 31725
		[SerializeField]
		private int startItemsCount;

		// Token: 0x04007BEE RID: 31726
		[Space]
		public UnityEvent OnReachedMaxCapacity;

		// Token: 0x04007BEF RID: 31727
		public UnityEvent OnFullyEmptied;

		// Token: 0x04007BF0 RID: 31728
		public UnityEvent OnRefilled;

		// Token: 0x04007BF1 RID: 31729
		private int currentItemsCount;

		// Token: 0x04007BF2 RID: 31730
		private bool canGrab;

		// Token: 0x04007BF3 RID: 31731
		private float lastGrabTime;
	}
}
