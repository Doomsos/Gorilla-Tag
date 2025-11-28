using System;
using UnityEngine;

// Token: 0x02000CEF RID: 3311
[Serializable]
public class VoiceLoudnessReactorParticleSystemTarget
{
	// Token: 0x17000778 RID: 1912
	// (get) Token: 0x0600506F RID: 20591 RVA: 0x0019E343 File Offset: 0x0019C543
	// (set) Token: 0x06005070 RID: 20592 RVA: 0x0019E34B File Offset: 0x0019C54B
	public float InitialSpeed
	{
		get
		{
			return this.initialSpeed;
		}
		set
		{
			this.initialSpeed = value;
		}
	}

	// Token: 0x17000779 RID: 1913
	// (get) Token: 0x06005071 RID: 20593 RVA: 0x0019E354 File Offset: 0x0019C554
	// (set) Token: 0x06005072 RID: 20594 RVA: 0x0019E35C File Offset: 0x0019C55C
	public float InitialRate
	{
		get
		{
			return this.initialRate;
		}
		set
		{
			this.initialRate = value;
		}
	}

	// Token: 0x1700077A RID: 1914
	// (get) Token: 0x06005073 RID: 20595 RVA: 0x0019E365 File Offset: 0x0019C565
	// (set) Token: 0x06005074 RID: 20596 RVA: 0x0019E36D File Offset: 0x0019C56D
	public float InitialSize
	{
		get
		{
			return this.initialSize;
		}
		set
		{
			this.initialSize = value;
		}
	}

	// Token: 0x04005FC4 RID: 24516
	public ParticleSystem particleSystem;

	// Token: 0x04005FC5 RID: 24517
	public bool UseSmoothedLoudness;

	// Token: 0x04005FC6 RID: 24518
	public float Scale = 1f;

	// Token: 0x04005FC7 RID: 24519
	private float initialSpeed;

	// Token: 0x04005FC8 RID: 24520
	private float initialRate;

	// Token: 0x04005FC9 RID: 24521
	private float initialSize;

	// Token: 0x04005FCA RID: 24522
	public AnimationCurve speed;

	// Token: 0x04005FCB RID: 24523
	public AnimationCurve rate;

	// Token: 0x04005FCC RID: 24524
	public AnimationCurve size;

	// Token: 0x04005FCD RID: 24525
	[HideInInspector]
	public ParticleSystem.MainModule Main;

	// Token: 0x04005FCE RID: 24526
	[HideInInspector]
	public ParticleSystem.EmissionModule Emission;
}
