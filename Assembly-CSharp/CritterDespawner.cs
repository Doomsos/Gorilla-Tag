using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class CritterDespawner : MonoBehaviour
{
	// Token: 0x060000E0 RID: 224 RVA: 0x00005F7A File Offset: 0x0000417A
	public void DespawnAllCritters()
	{
		CrittersManager.instance.QueueDespawnAllCritters();
	}
}
