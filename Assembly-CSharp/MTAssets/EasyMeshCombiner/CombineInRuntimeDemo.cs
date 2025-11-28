using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000F59 RID: 3929
	public class CombineInRuntimeDemo : MonoBehaviour
	{
		// Token: 0x0600627B RID: 25211 RVA: 0x001FB3FC File Offset: 0x001F95FC
		private void Update()
		{
			if (!this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(true);
				this.undoButton.SetActive(false);
			}
			if (this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(false);
				this.undoButton.SetActive(true);
			}
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x001FB453 File Offset: 0x001F9653
		public void CombineMeshes()
		{
			this.runtimeCombiner.CombineMeshes();
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x001FB461 File Offset: 0x001F9661
		public void UndoMerge()
		{
			this.runtimeCombiner.UndoMerge();
		}

		// Token: 0x04007107 RID: 28935
		public GameObject combineButton;

		// Token: 0x04007108 RID: 28936
		public GameObject undoButton;

		// Token: 0x04007109 RID: 28937
		public RuntimeMeshCombiner runtimeCombiner;
	}
}
