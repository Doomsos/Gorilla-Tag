using System;
using UnityEngine;

// Token: 0x02000525 RID: 1317
public class GorillaSceneCamera : MonoBehaviour
{
	// Token: 0x06002165 RID: 8549 RVA: 0x000AF66E File Offset: 0x000AD86E
	public void SetSceneCamera(int sceneIndex)
	{
		base.transform.position = this.sceneTransforms[sceneIndex].scenePosition;
		base.transform.eulerAngles = this.sceneTransforms[sceneIndex].sceneRotation;
	}

	// Token: 0x04002C1E RID: 11294
	public GorillaSceneTransform[] sceneTransforms;
}
