using System;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class FingerFlagTwirlTest : MonoBehaviour
{
	// Token: 0x06000AAD RID: 2733 RVA: 0x00039F44 File Offset: 0x00038144
	protected void FixedUpdate()
	{
		this.animTimes += Time.deltaTime * this.rotAnimDurations;
		this.animTimes.x = this.animTimes.x % 1f;
		this.animTimes.y = this.animTimes.y % 1f;
		this.animTimes.z = this.animTimes.z % 1f;
		base.transform.localRotation = Quaternion.Euler(this.rotXAnimCurve.Evaluate(this.animTimes.x) * this.rotAnimAmplitudes.x, this.rotYAnimCurve.Evaluate(this.animTimes.y) * this.rotAnimAmplitudes.y, this.rotZAnimCurve.Evaluate(this.animTimes.z) * this.rotAnimAmplitudes.z);
	}

	// Token: 0x04000D0B RID: 3339
	public Vector3 rotAnimDurations = new Vector3(0.2f, 0.1f, 0.5f);

	// Token: 0x04000D0C RID: 3340
	public Vector3 rotAnimAmplitudes = Vector3.one * 360f;

	// Token: 0x04000D0D RID: 3341
	public AnimationCurve rotXAnimCurve;

	// Token: 0x04000D0E RID: 3342
	public AnimationCurve rotYAnimCurve;

	// Token: 0x04000D0F RID: 3343
	public AnimationCurve rotZAnimCurve;

	// Token: 0x04000D10 RID: 3344
	private Vector3 animTimes = Vector3.zero;
}
