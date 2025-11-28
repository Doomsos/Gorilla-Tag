using System;
using UnityEngine;

// Token: 0x02000CBA RID: 3258
public class LightningDispatcher : MonoBehaviour
{
	// Token: 0x1400008D RID: 141
	// (add) Token: 0x06004F8B RID: 20363 RVA: 0x00199A24 File Offset: 0x00197C24
	// (remove) Token: 0x06004F8C RID: 20364 RVA: 0x00199A58 File Offset: 0x00197C58
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	// Token: 0x06004F8D RID: 20365 RVA: 0x00199A8C File Offset: 0x00197C8C
	public void DispatchLightning(Vector3 p1, Vector3 p2)
	{
		if (LightningDispatcher.RequestLightningStrike != null)
		{
			LightningStrike lightningStrike = LightningDispatcher.RequestLightningStrike(p1, p2);
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			lightningStrike.Play(p1, p2, this.beamWidthCM * 0.01f * num, this.soundVolumeMultiplier / num, LightningStrike.rand.NextFloat(this.minDuration, this.maxDuration), this.colorOverLifetime);
		}
	}

	// Token: 0x04005E09 RID: 24073
	[SerializeField]
	private float beamWidthCM = 1f;

	// Token: 0x04005E0A RID: 24074
	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	// Token: 0x04005E0B RID: 24075
	[SerializeField]
	private float minDuration = 0.05f;

	// Token: 0x04005E0C RID: 24076
	[SerializeField]
	private float maxDuration = 0.12f;

	// Token: 0x04005E0D RID: 24077
	[SerializeField]
	private Gradient colorOverLifetime;

	// Token: 0x02000CBB RID: 3259
	// (Invoke) Token: 0x06004F90 RID: 20368
	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
