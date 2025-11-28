using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class EqualizerAnim : MonoBehaviour
{
	// Token: 0x06000F7E RID: 3966 RVA: 0x00052259 File Offset: 0x00050459
	private void Start()
	{
		this.inputColorHash = Shader.PropertyToID(this.inputColorProperty);
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0005226C File Offset: 0x0005046C
	private void Update()
	{
		if (EqualizerAnim.thisFrame == Time.frameCount)
		{
			if (EqualizerAnim.materialsUpdatedThisFrame.Contains(this.material))
			{
				return;
			}
		}
		else
		{
			EqualizerAnim.thisFrame = Time.frameCount;
			EqualizerAnim.materialsUpdatedThisFrame.Clear();
		}
		float num = Time.time % this.loopDuration;
		this.material.SetColor(this.inputColorHash, new Color(this.redCurve.Evaluate(num), this.greenCurve.Evaluate(num), this.blueCurve.Evaluate(num)));
		EqualizerAnim.materialsUpdatedThisFrame.Add(this.material);
	}

	// Token: 0x0400131C RID: 4892
	[SerializeField]
	private AnimationCurve redCurve;

	// Token: 0x0400131D RID: 4893
	[SerializeField]
	private AnimationCurve greenCurve;

	// Token: 0x0400131E RID: 4894
	[SerializeField]
	private AnimationCurve blueCurve;

	// Token: 0x0400131F RID: 4895
	[SerializeField]
	private float loopDuration;

	// Token: 0x04001320 RID: 4896
	[SerializeField]
	private Material material;

	// Token: 0x04001321 RID: 4897
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04001322 RID: 4898
	private int inputColorHash;

	// Token: 0x04001323 RID: 4899
	private static int thisFrame;

	// Token: 0x04001324 RID: 4900
	private static HashSet<Material> materialsUpdatedThisFrame = new HashSet<Material>();
}
