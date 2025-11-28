using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000665 RID: 1637
[Serializable]
public class GameAbilityEvent
{
	// Token: 0x060029E5 RID: 10725 RVA: 0x000E277C File Offset: 0x000E097C
	public void Reset()
	{
		this.played = false;
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x000E2788 File Offset: 0x000E0988
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		if (abilityTime < this.time || this.played)
		{
			return;
		}
		this.played = true;
		if (this.sound.IsValid())
		{
			this.sound.Play(audioSource);
		}
		for (int i = 0; i < this.triggerEvent.Count; i++)
		{
			this.triggerEvent[i].Invoke();
		}
	}

	// Token: 0x04003619 RID: 13849
	public float time;

	// Token: 0x0400361A RID: 13850
	public AbilitySound sound;

	// Token: 0x0400361B RID: 13851
	public List<UnityEvent> triggerEvent;

	// Token: 0x0400361C RID: 13852
	[NonSerialized]
	public bool played;
}
