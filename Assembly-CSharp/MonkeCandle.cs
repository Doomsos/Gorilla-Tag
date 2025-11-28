using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004F1 RID: 1265
public class MonkeCandle : RubberDuck
{
	// Token: 0x06002081 RID: 8321 RVA: 0x000AC8A0 File Offset: 0x000AAAA0
	protected override void Start()
	{
		base.Start();
		if (!this.IsMyItem())
		{
			this.movingFxAudio.volume = this.movingFxAudio.volume * 0.5f;
			this.fxExplodeAudio.volume = this.fxExplodeAudio.volume * 0.5f;
		}
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x000AC8F4 File Offset: 0x000AAAF4
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (!this.particleFX.isPlaying)
		{
			return;
		}
		int particles = this.particleFX.GetParticles(this.fxParticleArray);
		if (particles <= 0)
		{
			this.movingFxAudio.GTStop();
			if (this.currentParticles.Count == 0)
			{
				return;
			}
		}
		for (int i = 0; i < particles; i++)
		{
			if (this.currentParticles.Contains(this.fxParticleArray[i].randomSeed))
			{
				this.currentParticles.Remove(this.fxParticleArray[i].randomSeed);
			}
		}
		foreach (uint num in this.currentParticles)
		{
			if (this.particleInfoDict.TryGetValue(num, ref this.outPosition))
			{
				this.fxExplodeAudio.transform.position = this.outPosition;
				this.fxExplodeAudio.GTPlayOneShot(this.fxExplodeAudio.clip, 1f);
				this.particleInfoDict.Remove(num);
			}
		}
		this.currentParticles.Clear();
		for (int j = 0; j < particles; j++)
		{
			if (j == 0)
			{
				this.movingFxAudio.transform.position = this.fxParticleArray[j].position;
			}
			if (this.particleInfoDict.TryGetValue(this.fxParticleArray[j].randomSeed, ref this.outPosition))
			{
				this.particleInfoDict[this.fxParticleArray[j].randomSeed] = this.fxParticleArray[j].position;
			}
			else
			{
				this.particleInfoDict.Add(this.fxParticleArray[j].randomSeed, this.fxParticleArray[j].position);
				if (j == 0 && !this.movingFxAudio.isPlaying)
				{
					this.movingFxAudio.GTPlay();
				}
			}
			this.currentParticles.Add(this.fxParticleArray[j].randomSeed);
		}
	}

	// Token: 0x04002B15 RID: 11029
	private ParticleSystem.Particle[] fxParticleArray = new ParticleSystem.Particle[20];

	// Token: 0x04002B16 RID: 11030
	public AudioSource movingFxAudio;

	// Token: 0x04002B17 RID: 11031
	public AudioSource fxExplodeAudio;

	// Token: 0x04002B18 RID: 11032
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04002B19 RID: 11033
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04002B1A RID: 11034
	private Vector3 outPosition;
}
