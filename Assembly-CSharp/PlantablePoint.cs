using System;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class PlantablePoint : MonoBehaviour
{
	// Token: 0x0600123D RID: 4669 RVA: 0x0005FF1E File Offset: 0x0005E11E
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.SetPlanted(true);
		}
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x0005FF4A File Offset: 0x0005E14A
	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.SetPlanted(false);
		}
	}

	// Token: 0x040016D5 RID: 5845
	public bool shouldBeSet;

	// Token: 0x040016D6 RID: 5846
	public LayerMask floorMask;

	// Token: 0x040016D7 RID: 5847
	public PlantableObject plantableObject;
}
