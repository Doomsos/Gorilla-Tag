using System;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class PeriodicNoiseGenerator : MonoBehaviour
{
	// Token: 0x0600035A RID: 858 RVA: 0x00013F85 File Offset: 0x00012185
	private void Awake()
	{
		this.noiseActor = base.GetComponentInParent<CrittersLoudNoise>();
		this.lastTime = Time.time;
		this.mR = base.GetComponentInChildren<MeshRenderer>();
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00013FAC File Offset: 0x000121AC
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.time > this.lastTime + this.sleepDuration)
		{
			this.lastTime = Time.time + this.randomDuration * Random.value;
			this.noiseActor.SetTimeEnabled();
			this.noiseActor.soundEnabled = true;
			this.mR.sharedMaterial = this.solid;
		}
		if (!this.noiseActor.soundEnabled && this.mR.sharedMaterial != this.transparent)
		{
			this.mR.sharedMaterial = this.transparent;
		}
	}

	// Token: 0x040003E4 RID: 996
	public float sleepDuration;

	// Token: 0x040003E5 RID: 997
	public float randomDuration;

	// Token: 0x040003E6 RID: 998
	public float lastTime;

	// Token: 0x040003E7 RID: 999
	private CrittersLoudNoise noiseActor;

	// Token: 0x040003E8 RID: 1000
	public Material transparent;

	// Token: 0x040003E9 RID: 1001
	public Material solid;

	// Token: 0x040003EA RID: 1002
	private MeshRenderer mR;
}
