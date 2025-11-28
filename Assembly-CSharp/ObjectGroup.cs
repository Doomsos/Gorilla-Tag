using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009EB RID: 2539
public class ObjectGroup : MonoBehaviour
{
	// Token: 0x060040BA RID: 16570 RVA: 0x0015A28C File Offset: 0x0015848C
	private void OnEnable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(true);
		}
	}

	// Token: 0x060040BB RID: 16571 RVA: 0x0015A29D File Offset: 0x0015849D
	private void OnDisable()
	{
		if (this.syncWithGroupState)
		{
			this.SetObjectStates(false);
		}
	}

	// Token: 0x060040BC RID: 16572 RVA: 0x0015A2B0 File Offset: 0x001584B0
	public void SetObjectStates(bool active)
	{
		int count = this.gameObjects.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.gameObjects[i];
			if (!(gameObject == null))
			{
				gameObject.SetActive(active);
			}
		}
		int count2 = this.behaviours.Count;
		for (int j = 0; j < count2; j++)
		{
			Behaviour behaviour = this.behaviours[j];
			if (!(behaviour == null))
			{
				behaviour.enabled = active;
			}
		}
		int count3 = this.renderers.Count;
		for (int k = 0; k < count3; k++)
		{
			Renderer renderer = this.renderers[k];
			if (!(renderer == null))
			{
				renderer.enabled = active;
			}
		}
		int count4 = this.colliders.Count;
		for (int l = 0; l < count4; l++)
		{
			Collider collider = this.colliders[l];
			if (!(collider == null))
			{
				collider.enabled = active;
			}
		}
	}

	// Token: 0x040051F8 RID: 20984
	public List<GameObject> gameObjects = new List<GameObject>(16);

	// Token: 0x040051F9 RID: 20985
	public List<Behaviour> behaviours = new List<Behaviour>(16);

	// Token: 0x040051FA RID: 20986
	public List<Renderer> renderers = new List<Renderer>(16);

	// Token: 0x040051FB RID: 20987
	public List<Collider> colliders = new List<Collider>(16);

	// Token: 0x040051FC RID: 20988
	public bool syncWithGroupState = true;
}
