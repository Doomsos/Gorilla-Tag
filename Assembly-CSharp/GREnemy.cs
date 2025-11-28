using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class GREnemy : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002BAE RID: 11182 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityInit()
	{
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x000EAAC0 File Offset: 0x000E8CC0
	public static void HideRenderers(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002BB2 RID: 11186 RVA: 0x000EAB04 File Offset: 0x000E8D04
	public static void HideObjects(List<GameObject> objects, bool hide)
	{
		if (objects == null)
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null)
			{
				objects[i].SetActive(!hide);
			}
		}
	}
}
