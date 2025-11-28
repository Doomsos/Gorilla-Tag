using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class ColliderOffsetOverride : MonoBehaviour
{
	// Token: 0x060000BF RID: 191 RVA: 0x00005494 File Offset: 0x00003694
	private void Awake()
	{
		if (this.autoSearch)
		{
			this.FindColliders();
		}
		foreach (Collider collider in this.colliders)
		{
			if (collider != null)
			{
				collider.contactOffset = 0.01f * this.targetScale;
			}
		}
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x0000550C File Offset: 0x0000370C
	public void FindColliders()
	{
		foreach (Collider collider in Enumerable.ToList<Collider>(base.gameObject.GetComponents<Collider>()))
		{
			if (!this.colliders.Contains(collider))
			{
				this.colliders.Add(collider);
			}
		}
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0000557C File Offset: 0x0000377C
	public void FindCollidersRecursively()
	{
		foreach (Collider collider in Enumerable.ToList<Collider>(base.gameObject.GetComponentsInChildren<Collider>()))
		{
			if (!this.colliders.Contains(collider))
			{
				this.colliders.Add(collider);
			}
		}
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x000055EC File Offset: 0x000037EC
	private void AutoDisabled()
	{
		this.autoSearch = true;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000055F5 File Offset: 0x000037F5
	private void AutoEnabled()
	{
		this.autoSearch = false;
	}

	// Token: 0x040000D9 RID: 217
	public List<Collider> colliders;

	// Token: 0x040000DA RID: 218
	[HideInInspector]
	public bool autoSearch;

	// Token: 0x040000DB RID: 219
	public float targetScale = 1f;
}
