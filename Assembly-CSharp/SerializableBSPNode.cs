using System;
using UnityEngine;

// Token: 0x02000CFF RID: 3327
[Serializable]
public struct SerializableBSPNode
{
	// Token: 0x17000784 RID: 1924
	// (get) Token: 0x060050CE RID: 20686 RVA: 0x001A1672 File Offset: 0x0019F872
	public int matrixIndex
	{
		get
		{
			return (int)this.leftChildIndex;
		}
	}

	// Token: 0x17000785 RID: 1925
	// (get) Token: 0x060050CF RID: 20687 RVA: 0x001A167A File Offset: 0x0019F87A
	public int outsideChildIndex
	{
		get
		{
			return (int)this.rightChildIndex;
		}
	}

	// Token: 0x17000786 RID: 1926
	// (get) Token: 0x060050D0 RID: 20688 RVA: 0x001A1672 File Offset: 0x0019F872
	public int zoneIndex
	{
		get
		{
			return (int)this.leftChildIndex;
		}
	}

	// Token: 0x0400600C RID: 24588
	[SerializeField]
	public SerializableBSPNode.Axis axis;

	// Token: 0x0400600D RID: 24589
	[SerializeField]
	public float splitValue;

	// Token: 0x0400600E RID: 24590
	[SerializeField]
	public short leftChildIndex;

	// Token: 0x0400600F RID: 24591
	[SerializeField]
	public short rightChildIndex;

	// Token: 0x02000D00 RID: 3328
	public enum Axis
	{
		// Token: 0x04006011 RID: 24593
		X,
		// Token: 0x04006012 RID: 24594
		Y,
		// Token: 0x04006013 RID: 24595
		Z,
		// Token: 0x04006014 RID: 24596
		MatrixChain,
		// Token: 0x04006015 RID: 24597
		MatrixFinal,
		// Token: 0x04006016 RID: 24598
		Zone
	}
}
