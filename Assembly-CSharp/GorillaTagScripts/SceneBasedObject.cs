using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000E05 RID: 3589
	public class SceneBasedObject : MonoBehaviour
	{
		// Token: 0x0600599D RID: 22941 RVA: 0x001CA96B File Offset: 0x001C8B6B
		public bool IsLocalPlayerInScene()
		{
			return (ZoneManagement.instance.GetAllLoadedScenes().Count <= 1 || this.zone != GTZone.forest) && ZoneManagement.instance.IsSceneLoaded(this.zone);
		}

		// Token: 0x040066C4 RID: 26308
		public GTZone zone;
	}
}
