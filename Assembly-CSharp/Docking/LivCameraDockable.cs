using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

namespace Docking
{
	// Token: 0x02001149 RID: 4425
	public class LivCameraDockable : Dockable
	{
		// Token: 0x06006FBA RID: 28602 RVA: 0x00246330 File Offset: 0x00244530
		protected override void OnTriggerEnter(Collider other)
		{
			LivCameraDock livCameraDock;
			if (other.TryGetComponent<LivCameraDock>(ref livCameraDock))
			{
				this.livDock = livCameraDock;
				this.potentialDock = other.transform;
			}
		}

		// Token: 0x06006FBB RID: 28603 RVA: 0x0024635A File Offset: 0x0024455A
		protected override void OnTriggerExit(Collider other)
		{
			if (this.livDock != null && other.transform == this.potentialDock.transform)
			{
				this.potentialDock = null;
				this.livDock = null;
			}
		}

		// Token: 0x06006FBC RID: 28604 RVA: 0x00246390 File Offset: 0x00244590
		public override void Dock()
		{
			base.Dock();
			if (this.livDock == null)
			{
				return;
			}
			GTLckController gtlckController = base.GetComponent<GTLckController>() ?? base.GetComponentInParent<GTLckController>();
			if (gtlckController != null)
			{
				gtlckController.ApplyCameraSettings(this.livDock.cameraSettings);
			}
			this.livDock = null;
		}

		// Token: 0x04008028 RID: 32808
		private LivCameraDock livDock;
	}
}
