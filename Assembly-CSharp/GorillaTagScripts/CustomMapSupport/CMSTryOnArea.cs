using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000E10 RID: 3600
	public class CMSTryOnArea : MonoBehaviour
	{
		// Token: 0x060059DD RID: 23005 RVA: 0x001CC0B6 File Offset: 0x001CA2B6
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene)
		{
			this.originalScene = customMapScene;
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.AddCollider(this.tryOnAreaCollider);
		}

		// Token: 0x060059DE RID: 23006 RVA: 0x001CC0D9 File Offset: 0x001CA2D9
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.tryOnAreaCollider);
		}

		// Token: 0x060059DF RID: 23007 RVA: 0x001CC0F5 File Offset: 0x001CA2F5
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x040066F0 RID: 26352
		private Scene originalScene;

		// Token: 0x040066F1 RID: 26353
		public BoxCollider tryOnAreaCollider;
	}
}
