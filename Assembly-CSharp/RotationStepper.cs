using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class RotationStepper : MonoBehaviour
{
	// Token: 0x06000089 RID: 137 RVA: 0x00004C9D File Offset: 0x00002E9D
	public void OnEnable()
	{
		this.m_phase = 0f;
		Random.InitState(0);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00004CB0 File Offset: 0x00002EB0
	public void Update()
	{
		this.m_phase += this.Frequency * Time.deltaTime;
		RotationStepper.ModeEnum mode = this.Mode;
		if (mode == RotationStepper.ModeEnum.Fixed)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Repeat(this.m_phase, 2f) < 1f) ? -25f : 25f);
			return;
		}
		if (mode != RotationStepper.ModeEnum.Random)
		{
			return;
		}
		while (this.m_phase >= 1f)
		{
			Random.InitState(Time.frameCount);
			base.transform.rotation = Random.rotationUniform;
			this.m_phase -= 1f;
		}
	}

	// Token: 0x040000A7 RID: 167
	public RotationStepper.ModeEnum Mode;

	// Token: 0x040000A8 RID: 168
	[ConditionalField("Mode", RotationStepper.ModeEnum.Fixed, null, null, null, null, null)]
	public float Angle = 25f;

	// Token: 0x040000A9 RID: 169
	public float Frequency;

	// Token: 0x040000AA RID: 170
	private float m_phase;

	// Token: 0x02000026 RID: 38
	public enum ModeEnum
	{
		// Token: 0x040000AC RID: 172
		Fixed,
		// Token: 0x040000AD RID: 173
		Random
	}
}
