using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004D2 RID: 1234
public class WardrobeInstance : MonoBehaviour
{
	// Token: 0x06001FD6 RID: 8150 RVA: 0x000A98FF File Offset: 0x000A7AFF
	public void Start()
	{
		CosmeticsController.instance.AddWardrobeInstance(this);
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x000A990E File Offset: 0x000A7B0E
	public void OnDestroy()
	{
		CosmeticsController.instance.RemoveWardrobeInstance(this);
	}

	// Token: 0x04002A2F RID: 10799
	public WardrobeItemButton[] wardrobeItemButtons;

	// Token: 0x04002A30 RID: 10800
	public HeadModel selfDoll;
}
