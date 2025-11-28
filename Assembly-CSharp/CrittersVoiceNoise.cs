using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class CrittersVoiceNoise : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060002DC RID: 732 RVA: 0x000113F5 File Offset: 0x0000F5F5
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00011418 File Offset: 0x0000F618
	public void SliceUpdate()
	{
		float num = 0f;
		if (this.speaker.IsSpeaking)
		{
			num = this.speaker.Loudness;
		}
		if (num > this.minTriggerThreshold && CrittersManager.instance.IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.rig].rigActors[4].actorSet;
			if (crittersLoudNoise.IsNotNull() && !crittersLoudNoise.soundEnabled)
			{
				float volume = Mathf.Lerp(this.noiseVolumeMin, this.noisVolumeMax, Mathf.Clamp01((num - this.minTriggerThreshold) / this.maxTriggerThreshold));
				crittersLoudNoise.PlayVoiceSpeechLocal(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), 0.016666668f, volume);
			}
		}
	}

	// Token: 0x04000345 RID: 837
	[SerializeField]
	private GorillaSpeakerLoudness speaker;

	// Token: 0x04000346 RID: 838
	[SerializeField]
	private VRRig rig;

	// Token: 0x04000347 RID: 839
	[SerializeField]
	private float minTriggerThreshold = 0.01f;

	// Token: 0x04000348 RID: 840
	[SerializeField]
	private float maxTriggerThreshold = 0.3f;

	// Token: 0x04000349 RID: 841
	[SerializeField]
	private float noiseVolumeMin = 1f;

	// Token: 0x0400034A RID: 842
	[SerializeField]
	private float noisVolumeMax = 9f;
}
