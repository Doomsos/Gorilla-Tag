using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class Oscillator : MonoBehaviour
{
	// Token: 0x06000084 RID: 132 RVA: 0x00004AD1 File Offset: 0x00002CD1
	public void Init(Vector3 center, Vector3 radius, Vector3 frequency, Vector3 startPhase)
	{
		this.Center = center;
		this.Radius = radius;
		this.Frequency = frequency;
		this.Phase = startPhase;
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00004AF0 File Offset: 0x00002CF0
	private float SampleWave(float phase)
	{
		switch (this.WaveType)
		{
		case Oscillator.WaveTypeEnum.Sine:
			return Mathf.Sin(phase);
		case Oscillator.WaveTypeEnum.Square:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase >= 3.1415927f)
			{
				return -1f;
			}
			return 1f;
		case Oscillator.WaveTypeEnum.Triangle:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase < 1.5707964f)
			{
				return phase / 1.5707964f;
			}
			if (phase < 3.1415927f)
			{
				return 1f - (phase - 1.5707964f) / 1.5707964f;
			}
			if (phase < 4.712389f)
			{
				return (3.1415927f - phase) / 1.5707964f;
			}
			return (phase - 4.712389f) / 1.5707964f - 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00004BAB File Offset: 0x00002DAB
	public void OnEnable()
	{
		this.m_initCenter = base.transform.position;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00004BC0 File Offset: 0x00002DC0
	public void Update()
	{
		this.Phase += this.Frequency * 2f * 3.1415927f * Time.deltaTime;
		Vector3 position = this.UseCenter ? this.Center : this.m_initCenter;
		position.x += this.Radius.x * this.SampleWave(this.Phase.x);
		position.y += this.Radius.y * this.SampleWave(this.Phase.y);
		position.z += this.Radius.z * this.SampleWave(this.Phase.z);
		base.transform.position = position;
	}

	// Token: 0x0400009C RID: 156
	public Oscillator.WaveTypeEnum WaveType;

	// Token: 0x0400009D RID: 157
	private Vector3 m_initCenter;

	// Token: 0x0400009E RID: 158
	public bool UseCenter;

	// Token: 0x0400009F RID: 159
	public Vector3 Center;

	// Token: 0x040000A0 RID: 160
	public Vector3 Radius;

	// Token: 0x040000A1 RID: 161
	public Vector3 Frequency;

	// Token: 0x040000A2 RID: 162
	public Vector3 Phase;

	// Token: 0x02000024 RID: 36
	public enum WaveTypeEnum
	{
		// Token: 0x040000A4 RID: 164
		Sine,
		// Token: 0x040000A5 RID: 165
		Square,
		// Token: 0x040000A6 RID: 166
		Triangle
	}
}
