using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020007EA RID: 2026
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06003556 RID: 13654 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x04004489 RID: 17545
	public Material redMaterial;

	// Token: 0x0400448A RID: 17546
	public Material blueMaterial;

	// Token: 0x0400448B RID: 17547
	public Material greenMaterial;

	// Token: 0x0400448C RID: 17548
	public Material transparentBlueMaterial;

	// Token: 0x0400448D RID: 17549
	public Material transparentRedMaterial;

	// Token: 0x0400448E RID: 17550
	public Material transparentGreenMaterial;

	// Token: 0x0400448F RID: 17551
	public MeshRenderer smoothTurnBox;

	// Token: 0x04004490 RID: 17552
	public MeshRenderer snapTurnBox;

	// Token: 0x04004491 RID: 17553
	public MeshRenderer noTurnBox;

	// Token: 0x04004492 RID: 17554
	public GorillaSnapTurn snapTurn;

	// Token: 0x04004493 RID: 17555
	public string currentChoice;

	// Token: 0x04004494 RID: 17556
	public float currentSpeed;
}
