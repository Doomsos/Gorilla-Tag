using System;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class ReparentOnAwakeWithRenderer : MonoBehaviour, IBuildValidation
{
	// Token: 0x060012E1 RID: 4833 RVA: 0x0006E265 File Offset: 0x0006C465
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() != null && this.myRenderer == null)
		{
			Debug.Log(base.name + " needs a reference to its renderer since it has one - ");
			return false;
		}
		return true;
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x0006E29C File Offset: 0x0006C49C
	private void OnEnable()
	{
		base.transform.SetParent(this.newParent, true);
		if (this.sortLast)
		{
			base.transform.SetAsLastSibling();
		}
		else
		{
			base.transform.SetAsFirstSibling();
		}
		if (this.myRenderer != null)
		{
			this.myRenderer.reflectionProbeUsage = 0;
			this.myRenderer.lightProbeUsage = 4;
			this.myRenderer.probeAnchor = this.newParent;
		}
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x0006E312 File Offset: 0x0006C512
	[ContextMenu("Set Renderer")]
	public void SetMyRenderer()
	{
		this.myRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x04001C9D RID: 7325
	public Transform newParent;

	// Token: 0x04001C9E RID: 7326
	public MeshRenderer myRenderer;

	// Token: 0x04001C9F RID: 7327
	[Tooltip("We're mostly using this for UI elements like text and images, so this will help you separate these in whatever target parent object.Keep images and texts together, otherwise you'll get extra draw calls. Put images above text or they'll overlap weird tho lol")]
	public bool sortLast;
}
