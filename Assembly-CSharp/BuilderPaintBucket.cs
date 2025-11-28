using System;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class BuilderPaintBucket : MonoBehaviour
{
	// Token: 0x060022B3 RID: 8883 RVA: 0x000B5A24 File Offset: 0x000B3C24
	private void Awake()
	{
		if (string.IsNullOrEmpty(this.materialId))
		{
			return;
		}
		this.materialType = this.materialId.GetHashCode();
		if (this.bucketMaterialOptions != null && this.paintBucketRenderer != null)
		{
			Material material;
			int num;
			this.bucketMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.paintBucketRenderer.material = material;
			}
		}
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000B5A98 File Offset: 0x000B3C98
	private void OnTriggerEnter(Collider other)
	{
		if (this.materialType == -1)
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			BuilderPaintBrush component = attachedRigidbody.GetComponent<BuilderPaintBrush>();
			if (component != null)
			{
				component.SetBrushMaterial(this.materialType);
			}
		}
	}

	// Token: 0x04002D5B RID: 11611
	[SerializeField]
	private BuilderMaterialOptions bucketMaterialOptions;

	// Token: 0x04002D5C RID: 11612
	[SerializeField]
	private MeshRenderer paintBucketRenderer;

	// Token: 0x04002D5D RID: 11613
	[SerializeField]
	private string materialId;

	// Token: 0x04002D5E RID: 11614
	private int materialType = -1;
}
