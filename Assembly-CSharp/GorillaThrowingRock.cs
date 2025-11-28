using System;
using Photon.Pun;

// Token: 0x02000931 RID: 2353
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06003C2A RID: 15402 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x04004CD0 RID: 19664
	public float bonkSpeedMin = 1f;

	// Token: 0x04004CD1 RID: 19665
	public float bonkSpeedMax = 5f;

	// Token: 0x04004CD2 RID: 19666
	public VRRig hitRig;
}
