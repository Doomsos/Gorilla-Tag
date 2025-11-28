using System;
using UnityEngine;

namespace Docking
{
	// Token: 0x02001147 RID: 4423
	public class Dockable : MonoBehaviour
	{
		// Token: 0x06006FB3 RID: 28595 RVA: 0x00246234 File Offset: 0x00244434
		protected virtual void OnTriggerEnter(Collider other)
		{
			Dock dock;
			if (other.TryGetComponent<Dock>(ref dock))
			{
				this.potentialDock = other.transform;
			}
		}

		// Token: 0x06006FB4 RID: 28596 RVA: 0x00246257 File Offset: 0x00244457
		protected virtual void OnTriggerExit(Collider other)
		{
			if (this.potentialDock == other.transform)
			{
				this.potentialDock = null;
			}
		}

		// Token: 0x06006FB5 RID: 28597 RVA: 0x00246274 File Offset: 0x00244474
		public virtual void Dock()
		{
			if (this.potentialDock == null)
			{
				return;
			}
			base.transform.position = this.potentialDock.position;
			base.transform.rotation = this.potentialDock.rotation;
			this.potentialDock = null;
		}

		// Token: 0x04008026 RID: 32806
		protected Transform potentialDock;
	}
}
