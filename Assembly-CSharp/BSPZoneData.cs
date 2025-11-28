using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CFC RID: 3324
public class BSPZoneData : MonoBehaviour
{
	// Token: 0x17000782 RID: 1922
	// (get) Token: 0x060050CB RID: 20683 RVA: 0x001A167D File Offset: 0x0019F87D
	public int Priority
	{
		get
		{
			return this.priority;
		}
	}

	// Token: 0x17000783 RID: 1923
	// (get) Token: 0x060050CC RID: 20684 RVA: 0x001A1685 File Offset: 0x0019F885
	public string ZoneName
	{
		get
		{
			return base.gameObject.name;
		}
	}

	// Token: 0x04006006 RID: 24582
	[SerializeField]
	private int priority;

	// Token: 0x04006007 RID: 24583
	[NonSerialized]
	public List<BoxCollider> boxList;
}
