using System;
using UnityEngine;

// Token: 0x0200033D RID: 829
[Serializable]
public struct XSceneRef
{
	// Token: 0x060013F5 RID: 5109 RVA: 0x000737D0 File Offset: 0x000719D0
	public bool TryResolve(out XSceneRefTarget result)
	{
		if (this.TargetID == 0)
		{
			result = null;
			return true;
		}
		if (this.didCache && this.cached != null)
		{
			result = this.cached;
			return true;
		}
		XSceneRefTarget xsceneRefTarget;
		if (!XSceneRefGlobalHub.TryResolve(this.TargetScene, this.TargetID, out xsceneRefTarget))
		{
			result = null;
			return false;
		}
		this.cached = xsceneRefTarget;
		this.didCache = true;
		result = xsceneRefTarget;
		return true;
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x00073838 File Offset: 0x00071A38
	public bool TryResolve(out GameObject result)
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? null : xsceneRefTarget.gameObject);
			return true;
		}
		result = null;
		return false;
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0007386C File Offset: 0x00071A6C
	public bool TryResolve<T>(out T result) where T : Component
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? default(T) : xsceneRefTarget.GetComponent<T>());
			return true;
		}
		result = default(T);
		return false;
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x000738AD File Offset: 0x00071AAD
	public void AddCallbackOnLoad(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneLoad(callback);
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x000738BB File Offset: 0x00071ABB
	public void RemoveCallbackOnLoad(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneLoad(callback);
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x000738C9 File Offset: 0x00071AC9
	public void AddCallbackOnUnload(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneUnload(callback);
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x000738D7 File Offset: 0x00071AD7
	public void RemoveCallbackOnUnload(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneUnload(callback);
	}

	// Token: 0x04001E91 RID: 7825
	public SceneIndex TargetScene;

	// Token: 0x04001E92 RID: 7826
	public int TargetID;

	// Token: 0x04001E93 RID: 7827
	private XSceneRefTarget cached;

	// Token: 0x04001E94 RID: 7828
	private bool didCache;
}
