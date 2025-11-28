using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DA8 RID: 3496
	public class AttachPoint : MonoBehaviour
	{
		// Token: 0x060055ED RID: 21997 RVA: 0x001B0561 File Offset: 0x001AE761
		private void Start()
		{
			base.transform.parent.parent = null;
		}

		// Token: 0x060055EE RID: 21998 RVA: 0x001B0574 File Offset: 0x001AE774
		private void OnTriggerEnter(Collider other)
		{
			if (this.attachPoint.childCount == 0)
			{
				this.UpdateHookState(false);
			}
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || componentInParent.InHand())
			{
				return;
			}
			if (this.IsHooked())
			{
				return;
			}
			this.UpdateHookState(true);
			componentInParent.SnapItem(true, this.attachPoint.position);
		}

		// Token: 0x060055EF RID: 21999 RVA: 0x001B05D0 File Offset: 0x001AE7D0
		private void OnTriggerExit(Collider other)
		{
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || !componentInParent.InHand())
			{
				return;
			}
			this.UpdateHookState(false);
			componentInParent.SnapItem(false, Vector3.zero);
		}

		// Token: 0x060055F0 RID: 22000 RVA: 0x001B0609 File Offset: 0x001AE809
		private void UpdateHookState(bool isHooked)
		{
			this.SetIsHook(isHooked);
		}

		// Token: 0x060055F1 RID: 22001 RVA: 0x001B0612 File Offset: 0x001AE812
		internal void SetIsHook(bool isHooked)
		{
			this.isHooked = isHooked;
			UnityAction unityAction = this.onHookedChanged;
			if (unityAction == null)
			{
				return;
			}
			unityAction.Invoke();
		}

		// Token: 0x060055F2 RID: 22002 RVA: 0x001B062B File Offset: 0x001AE82B
		public bool IsHooked()
		{
			return this.isHooked || this.attachPoint.childCount != 0;
		}

		// Token: 0x04006312 RID: 25362
		public Transform attachPoint;

		// Token: 0x04006313 RID: 25363
		public UnityAction onHookedChanged;

		// Token: 0x04006314 RID: 25364
		private bool isHooked;

		// Token: 0x04006315 RID: 25365
		private bool wasHooked;

		// Token: 0x04006316 RID: 25366
		public bool inForest;
	}
}
