using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class CommonActions : MonoBehaviour
{
	// Token: 0x060000C5 RID: 197 RVA: 0x00005611 File Offset: 0x00003811
	public void LoadSavedOutfit(int index)
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.LoadSavedOutfit(index);
		}
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0000562E File Offset: 0x0000382E
	public void LoadPrevOutfit()
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.PressWardrobeScrollOutfit(false);
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000564B File Offset: 0x0000384B
	public void LoadNextOutfit()
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.PressWardrobeScrollOutfit(true);
		}
	}
}
