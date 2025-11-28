using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class PlayerSpeedBasedAudio : MonoBehaviour
{
	// Token: 0x06000F00 RID: 3840 RVA: 0x0004F973 File Offset: 0x0004DB73
	private void Start()
	{
		this.fadeRate = 1f / this.fadeTime;
		this.baseVolume = this.audioSource.volume;
		this.localPlayerVelocityEstimator.TryResolve<GorillaVelocityEstimator>(out this.velocityEstimator);
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x0004F9AC File Offset: 0x0004DBAC
	private void Update()
	{
		this.currentFadeLevel = Mathf.MoveTowards(this.currentFadeLevel, Mathf.InverseLerp(this.minVolumeSpeed, this.fullVolumeSpeed, this.velocityEstimator.linearVelocity.magnitude), this.fadeRate * Time.deltaTime);
		if (this.baseVolume == 0f || this.currentFadeLevel == 0f)
		{
			this.audioSource.volume = 0.0001f;
			return;
		}
		this.audioSource.volume = this.baseVolume * this.currentFadeLevel;
	}

	// Token: 0x0400124E RID: 4686
	[SerializeField]
	private float minVolumeSpeed;

	// Token: 0x0400124F RID: 4687
	[SerializeField]
	private float fullVolumeSpeed;

	// Token: 0x04001250 RID: 4688
	[SerializeField]
	private float fadeTime;

	// Token: 0x04001251 RID: 4689
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001252 RID: 4690
	[SerializeField]
	private XSceneRef localPlayerVelocityEstimator;

	// Token: 0x04001253 RID: 4691
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001254 RID: 4692
	private float baseVolume;

	// Token: 0x04001255 RID: 4693
	private float fadeRate;

	// Token: 0x04001256 RID: 4694
	private float currentFadeLevel;
}
