using System;
using UnityEngine;

// Token: 0x0200051C RID: 1308
public class GorillaCameraSceneTrigger : MonoBehaviour
{
	// Token: 0x0600214B RID: 8523 RVA: 0x000AF3A8 File Offset: 0x000AD5A8
	public void ChangeScene(GorillaCameraTriggerIndex triggerLeft)
	{
		if (triggerLeft == this.currentSceneTrigger || this.currentSceneTrigger == null)
		{
			if (this.mostRecentSceneTrigger != this.currentSceneTrigger)
			{
				this.sceneCamera.SetSceneCamera(this.mostRecentSceneTrigger.sceneTriggerIndex);
				this.currentSceneTrigger = this.mostRecentSceneTrigger;
				return;
			}
			this.currentSceneTrigger = null;
		}
	}

	// Token: 0x04002BDB RID: 11227
	public GorillaSceneCamera sceneCamera;

	// Token: 0x04002BDC RID: 11228
	public GorillaCameraTriggerIndex currentSceneTrigger;

	// Token: 0x04002BDD RID: 11229
	public GorillaCameraTriggerIndex mostRecentSceneTrigger;
}
