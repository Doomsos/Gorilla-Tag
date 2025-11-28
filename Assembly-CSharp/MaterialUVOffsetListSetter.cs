using System;
using System.Collections.Generic;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x02000C59 RID: 3161
[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour, IBuildValidation
{
	// Token: 0x06004D56 RID: 19798 RVA: 0x00190A40 File Offset: 0x0018EC40
	private void Awake()
	{
		this.matPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.GetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06004D57 RID: 19799 RVA: 0x00190A6C File Offset: 0x0018EC6C
	public void SetUVOffset(int listIndex)
	{
		if (listIndex >= this.uvOffsetList.Count || listIndex < 0)
		{
			Debug.LogError("Invalid uv offset list index provided.");
			return;
		}
		if (this.matPropertyBlock == null || this.meshRenderer == null)
		{
			Debug.LogError("MaterialUVOffsetListSetter settings are incorrect somehow, please fix", base.gameObject);
			this.Awake();
			return;
		}
		Vector2 vector = this.uvOffsetList[listIndex];
		this.matPropertyBlock.SetVector(ShaderProps._BaseMap_ST, new Vector4(1f, 1f, vector.x, vector.y));
		this.meshRenderer.SetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06004D58 RID: 19800 RVA: 0x00190B0C File Offset: 0x0018ED0C
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() == null)
		{
			Debug.LogError("missing a mesh renderer for the materialuvoffsetlistsetter", base.gameObject);
			return false;
		}
		if (base.GetComponentInParent<EdMeshCombinerMono>() != null && base.GetComponentInParent<EdDoNotMeshCombine>() == null)
		{
			Debug.LogError("the meshrenderer is going to getcombined, that will likely cause issues for the materialuvoffsetlistsetter", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x04005CDE RID: 23774
	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	// Token: 0x04005CDF RID: 23775
	private MeshRenderer meshRenderer;

	// Token: 0x04005CE0 RID: 23776
	private MaterialPropertyBlock matPropertyBlock;
}
