using System;
using Docking;
using UnityEngine;

// Token: 0x0200038B RID: 907
[ExecuteAlways]
public class LivCameraDockPreviewSync : MonoBehaviour
{
	// Token: 0x04002010 RID: 8208
	private LivCameraDock dock;

	// Token: 0x04002011 RID: 8209
	private Camera parentCamera;

	// Token: 0x04002012 RID: 8210
	private float _lastCameraFOV = -1f;

	// Token: 0x04002013 RID: 8211
	private float _lastDockFOV = -1f;
}
