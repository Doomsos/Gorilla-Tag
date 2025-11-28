using System;
using UnityEngine;

// Token: 0x020007EF RID: 2031
public class GorillaVRConstraint : MonoBehaviourTick
{
	// Token: 0x06003562 RID: 13666 RVA: 0x00121EC0 File Offset: 0x001200C0
	public override void Tick()
	{
		if (NetworkSystem.Instance.WrongVersion)
		{
			this.isConstrained = true;
		}
		if (this.isConstrained && Time.realtimeSinceStartup > this.angle)
		{
			GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
		}
	}

	// Token: 0x0400449B RID: 17563
	public bool isConstrained;

	// Token: 0x0400449C RID: 17564
	public float angle = 3600f;
}
