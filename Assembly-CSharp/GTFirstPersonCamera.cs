using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002F9 RID: 761
[DefaultExecutionOrder(-2147483648)]
public class GTFirstPersonCamera : MonoBehaviour
{
	// Token: 0x170001CE RID: 462
	// (get) Token: 0x060012B0 RID: 4784 RVA: 0x00061BC8 File Offset: 0x0005FDC8
	// (set) Token: 0x060012B1 RID: 4785 RVA: 0x00061BCF File Offset: 0x0005FDCF
	public static Camera camera { get; private set; }

	// Token: 0x060012B2 RID: 4786 RVA: 0x00061BD7 File Offset: 0x0005FDD7
	public void Awake()
	{
		GTFirstPersonCamera.camera = base.GetComponent<Camera>();
		if (GTFirstPersonCamera.camera == null)
		{
			Debug.LogError("[GTFirstPersonCamera]  ERROR!!!  Could not find Camera on same GameObject!");
			return;
		}
		RenderPipelineManager.beginCameraRendering += new Action<ScriptableRenderContext, Camera>(this._OnPreRender);
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x00061C0D File Offset: 0x0005FE0D
	private void _OnPreRender(ScriptableRenderContext context, Camera cam)
	{
		if (cam == GTFirstPersonCamera.camera)
		{
			Action onPreRenderEvent = GTFirstPersonCamera.OnPreRenderEvent;
			if (onPreRenderEvent == null)
			{
				return;
			}
			onPreRenderEvent.Invoke();
		}
	}

	// Token: 0x0400173C RID: 5948
	private const string preLog = "[GTFirstPersonCamera]  ";

	// Token: 0x0400173D RID: 5949
	private const string preErr = "[GTFirstPersonCamera]  ERROR!!!  ";

	// Token: 0x0400173F RID: 5951
	public static Action OnPreRenderEvent;
}
