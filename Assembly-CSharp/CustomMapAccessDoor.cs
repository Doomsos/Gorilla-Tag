using System;
using UnityEngine;

// Token: 0x02000937 RID: 2359
public class CustomMapAccessDoor : MonoBehaviour
{
	// Token: 0x06003C45 RID: 15429 RVA: 0x0013E75A File Offset: 0x0013C95A
	public void OpenDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(true);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(false);
		}
	}

	// Token: 0x06003C46 RID: 15430 RVA: 0x0013E790 File Offset: 0x0013C990
	public void CloseDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(false);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(true);
		}
	}

	// Token: 0x04004CF6 RID: 19702
	public GameObject openDoorObject;

	// Token: 0x04004CF7 RID: 19703
	public GameObject closedDoorObject;
}
