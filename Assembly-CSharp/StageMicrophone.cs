using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020008E8 RID: 2280
public class StageMicrophone : MonoBehaviour
{
	// Token: 0x06003A5D RID: 14941 RVA: 0x0013412F File Offset: 0x0013232F
	private void Awake()
	{
		StageMicrophone.Instance = this;
	}

	// Token: 0x06003A5E RID: 14942 RVA: 0x00134137 File Offset: 0x00132337
	public bool IsPlayerAmplified(VRRig player)
	{
		return (player.GetMouthPosition() - base.transform.position).IsShorterThan(this.PickupRadius);
	}

	// Token: 0x06003A5F RID: 14943 RVA: 0x0013415A File Offset: 0x0013235A
	public float GetPlayerSpatialBlend(VRRig player)
	{
		if (!this.IsPlayerAmplified(player))
		{
			return 0.9f;
		}
		return this.AmplifiedSpatialBlend;
	}

	// Token: 0x040049A7 RID: 18855
	public static StageMicrophone Instance;

	// Token: 0x040049A8 RID: 18856
	[SerializeField]
	private float PickupRadius;

	// Token: 0x040049A9 RID: 18857
	[SerializeField]
	private float AmplifiedSpatialBlend;
}
