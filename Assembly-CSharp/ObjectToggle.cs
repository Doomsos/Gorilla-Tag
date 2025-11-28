using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009EC RID: 2540
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x060040BE RID: 16574 RVA: 0x0015A3E2 File Offset: 0x001585E2
	public void Toggle(bool initialState = true)
	{
		if (this._toggled == null)
		{
			if (initialState)
			{
				this.Enable();
				return;
			}
			this.Disable();
			return;
		}
		else
		{
			if (this._toggled.Value)
			{
				this.Disable();
				return;
			}
			this.Enable();
			return;
		}
	}

	// Token: 0x060040BF RID: 16575 RVA: 0x0015A41C File Offset: 0x0015861C
	public void Enable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(true);
				}
				else if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
		}
		this._toggled = new bool?(true);
	}

	// Token: 0x060040C0 RID: 16576 RVA: 0x0015A48C File Offset: 0x0015868C
	public void Disable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this._toggled = new bool?(false);
	}

	// Token: 0x040051FD RID: 20989
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x040051FE RID: 20990
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x040051FF RID: 20991
	[NonSerialized]
	private bool? _toggled;
}
