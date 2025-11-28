using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class RandomizeWavePhaseOffset : MonoBehaviour
{
	// Token: 0x06000BFA RID: 3066 RVA: 0x00040AB0 File Offset: 0x0003ECB0
	private void Start()
	{
		Material material = base.GetComponent<MeshRenderer>().material;
		UberShader.VertexWavePhaseOffset.SetValue<float>(material, Random.Range(this.minPhaseOffset, this.maxPhaseOffset));
	}

	// Token: 0x04000EA7 RID: 3751
	[SerializeField]
	private float minPhaseOffset;

	// Token: 0x04000EA8 RID: 3752
	[SerializeField]
	private float maxPhaseOffset;
}
