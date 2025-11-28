using System;
using UnityEngine;

// Token: 0x02000327 RID: 807
public static class UnityLayerExtensions
{
	// Token: 0x06001393 RID: 5011 RVA: 0x0007133E File Offset: 0x0006F53E
	public static int ToLayerMask(this UnityLayer self)
	{
		return 1 << (int)self;
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x00071346 File Offset: 0x0006F546
	public static int ToLayerIndex(this UnityLayer self)
	{
		return (int)self;
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x00071349 File Offset: 0x0006F549
	public static bool IsOnLayer(this GameObject obj, UnityLayer layer)
	{
		return obj.layer == (int)layer;
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x00071354 File Offset: 0x0006F554
	public static void SetLayer(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x00071360 File Offset: 0x0006F560
	public static void SetLayerRecursively(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
		foreach (object obj2 in obj.transform)
		{
			((Transform)obj2).gameObject.SetLayerRecursively(layer);
		}
	}
}
