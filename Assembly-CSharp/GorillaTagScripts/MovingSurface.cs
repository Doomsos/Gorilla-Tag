using System;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DAA RID: 3498
	[RequireComponent(typeof(Collider))]
	public class MovingSurface : MonoBehaviour
	{
		// Token: 0x06005604 RID: 22020 RVA: 0x001B0887 File Offset: 0x001AEA87
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterMovingSurface(this);
		}

		// Token: 0x06005605 RID: 22021 RVA: 0x001B08A0 File Offset: 0x001AEAA0
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterMovingSurface(this);
			}
		}

		// Token: 0x06005606 RID: 22022 RVA: 0x001B08BA File Offset: 0x001AEABA
		public int GetID()
		{
			return this.uniqueId;
		}

		// Token: 0x06005607 RID: 22023 RVA: 0x001B08C2 File Offset: 0x001AEAC2
		public void CopySettings(MovingSurfaceSettings movingSurfaceSettings)
		{
			this.uniqueId = movingSurfaceSettings.uniqueId;
		}

		// Token: 0x0400631E RID: 25374
		[SerializeField]
		private int uniqueId = -1;
	}
}
