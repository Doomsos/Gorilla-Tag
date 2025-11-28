using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class PositionVolumeModifier : MonoBehaviour
{
	// Token: 0x06001242 RID: 4674 RVA: 0x0005FF91 File Offset: 0x0005E191
	public void OnTriggerStay(Collider other)
	{
		this.audioToMod.isModified = true;
	}

	// Token: 0x040016D9 RID: 5849
	public TimeOfDayDependentAudio audioToMod;
}
