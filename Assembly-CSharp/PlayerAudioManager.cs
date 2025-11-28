using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200085C RID: 2140
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x0600385F RID: 14431 RVA: 0x0012D806 File Offset: 0x0012BA06
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x0012D80F File Offset: 0x0012BA0F
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04004769 RID: 18281
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x0400476A RID: 18282
	public AudioMixerSnapshot underwaterSnapshot;
}
