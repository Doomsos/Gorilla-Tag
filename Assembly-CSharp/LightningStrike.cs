using System;
using UnityEngine;

// Token: 0x02000CBE RID: 3262
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class LightningStrike : MonoBehaviour
{
	// Token: 0x06004F99 RID: 20377 RVA: 0x00199C38 File Offset: 0x00197E38
	private void Initialize()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.psMain = this.ps.main;
		this.psMain.playOnAwake = true;
		this.psMain.stopAction = 1;
		this.psShape = this.ps.shape;
		this.psTrails = this.ps.trails;
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.playOnAwake = true;
	}

	// Token: 0x06004F9A RID: 20378 RVA: 0x00199CB4 File Offset: 0x00197EB4
	public void Play(Vector3 p1, Vector3 p2, float beamWidthMultiplier, float audioVolume, float duration, Gradient colorOverLifetime)
	{
		if (this.ps == null)
		{
			this.Initialize();
		}
		base.transform.position = p1;
		base.transform.rotation = Quaternion.LookRotation(p1 - p2);
		this.psShape.radius = Vector3.Distance(p1, p2) * 0.5f;
		this.psShape.position = new Vector3(0f, 0f, -this.psShape.radius);
		this.psShape.randomPositionAmount = Mathf.Clamp(this.psShape.radius / 50f, 0f, 1f);
		this.psTrails.widthOverTrail = new ParticleSystem.MinMaxCurve(beamWidthMultiplier * 0.1f, beamWidthMultiplier);
		this.psTrails.colorOverLifetime = colorOverLifetime;
		this.psMain.duration = duration;
		this.audioSource.volume = Mathf.Clamp(this.psShape.radius / 5f, 0f, 1f) * audioVolume;
		base.gameObject.SetActive(true);
	}

	// Token: 0x04005E13 RID: 24083
	public static SRand rand = new SRand("LightningStrike");

	// Token: 0x04005E14 RID: 24084
	private ParticleSystem ps;

	// Token: 0x04005E15 RID: 24085
	private ParticleSystem.MainModule psMain;

	// Token: 0x04005E16 RID: 24086
	private ParticleSystem.ShapeModule psShape;

	// Token: 0x04005E17 RID: 24087
	private ParticleSystem.TrailModule psTrails;

	// Token: 0x04005E18 RID: 24088
	private AudioSource audioSource;
}
