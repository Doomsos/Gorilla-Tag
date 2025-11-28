using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000666 RID: 1638
[Serializable]
public class GameAbilityEvents
{
	// Token: 0x060029E8 RID: 10728 RVA: 0x000E27F0 File Offset: 0x000E09F0
	public void Reset()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].Reset();
		}
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000E2824 File Offset: 0x000E0A24
	public void TryPlay(float abilityTime, AudioSource audioSource)
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].TryPlay(abilityTime, audioSource);
		}
	}

	// Token: 0x0400361D RID: 13853
	public List<GameAbilityEvent> events;
}
