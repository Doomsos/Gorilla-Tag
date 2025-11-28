using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class CritterVisuals : MonoBehaviour
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x060002F7 RID: 759 RVA: 0x000127B7 File Offset: 0x000109B7
	public CritterAppearance Appearance
	{
		get
		{
			return this._appearance;
		}
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x000127C0 File Offset: 0x000109C0
	public void SetAppearance(CritterAppearance appearance)
	{
		this._appearance = appearance;
		float num = this._appearance.size.ClampSafe(0.25f, 1.5f);
		this.bodyRoot.localScale = new Vector3(num, num, num);
		if (!string.IsNullOrEmpty(appearance.hatName))
		{
			foreach (GameObject gameObject in this.hats)
			{
				gameObject.SetActive(gameObject.name == this._appearance.hatName);
			}
			this.hatRoot.gameObject.SetActive(true);
			return;
		}
		this.hatRoot.gameObject.SetActive(false);
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00012865 File Offset: 0x00010A65
	public void ApplyMesh(Mesh newMesh)
	{
		this.myMeshFilter.sharedMesh = newMesh;
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00012873 File Offset: 0x00010A73
	public void ApplyMaterial(Material mat)
	{
		this.myRenderer.sharedMaterial = mat;
	}

	// Token: 0x04000395 RID: 917
	public int critterType;

	// Token: 0x04000396 RID: 918
	[Header("Visuals")]
	public Transform bodyRoot;

	// Token: 0x04000397 RID: 919
	public MeshRenderer myRenderer;

	// Token: 0x04000398 RID: 920
	public MeshFilter myMeshFilter;

	// Token: 0x04000399 RID: 921
	public Transform hatRoot;

	// Token: 0x0400039A RID: 922
	public GameObject[] hats;

	// Token: 0x0400039B RID: 923
	private CritterAppearance _appearance;
}
