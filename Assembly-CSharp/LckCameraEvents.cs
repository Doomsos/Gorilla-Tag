using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x02000369 RID: 873
public class LckCameraEvents : MonoBehaviour
{
	// Token: 0x060014CB RID: 5323 RVA: 0x00076C51 File Offset: 0x00074E51
	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += new Action<ScriptableRenderContext, Camera>(this.RenderPipelineManagerOnbeginCameraRendering);
		RenderPipelineManager.endCameraRendering += new Action<ScriptableRenderContext, Camera>(this.RenderPipelineManagerOnendCameraRendering);
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x00076C75 File Offset: 0x00074E75
	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= new Action<ScriptableRenderContext, Camera>(this.RenderPipelineManagerOnbeginCameraRendering);
		RenderPipelineManager.endCameraRendering -= new Action<ScriptableRenderContext, Camera>(this.RenderPipelineManagerOnendCameraRendering);
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x00076C99 File Offset: 0x00074E99
	private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPreRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x00076CBA File Offset: 0x00074EBA
	private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPostRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04001F63 RID: 8035
	[SerializeField]
	private Camera _camera;

	// Token: 0x04001F64 RID: 8036
	public UnityEvent onPreRender;

	// Token: 0x04001F65 RID: 8037
	public UnityEvent onPostRender;
}
