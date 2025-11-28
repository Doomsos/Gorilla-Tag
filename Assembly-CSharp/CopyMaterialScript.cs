using System;
using UnityEngine;

// Token: 0x02000BD3 RID: 3027
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x06004AED RID: 19181 RVA: 0x00187FCE File Offset: 0x001861CE
	private void Start()
	{
		if (Application.platform == 11)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004AEE RID: 19182 RVA: 0x00187FE5 File Offset: 0x001861E5
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x04005B1B RID: 23323
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x04005B1C RID: 23324
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
