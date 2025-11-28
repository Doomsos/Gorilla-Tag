using System;
using UnityEngine;

// Token: 0x0200008A RID: 138
public class SpawnSoundOnEnable : MonoBehaviour
{
	// Token: 0x06000365 RID: 869 RVA: 0x000141F8 File Offset: 0x000123F8
	private void OnEnable()
	{
		if (CrittersManager.instance == null || !CrittersManager.instance.LocalAuthority() || !CrittersManager.instance.LocalInZone)
		{
			return;
		}
		if (!this.triggerOnFirstEnable && !this.firstEnabledOccured)
		{
			this.firstEnabledOccured = true;
			return;
		}
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
	}

	// Token: 0x040003F9 RID: 1017
	public int soundSubIndex = 3;

	// Token: 0x040003FA RID: 1018
	public bool triggerOnFirstEnable;

	// Token: 0x040003FB RID: 1019
	private bool firstEnabledOccured;
}
