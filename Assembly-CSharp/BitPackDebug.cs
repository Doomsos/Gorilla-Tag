using System;
using UnityEngine;

// Token: 0x02000C32 RID: 3122
public class BitPackDebug : MonoBehaviour
{
	// Token: 0x04005C67 RID: 23655
	public bool debugPos;

	// Token: 0x04005C68 RID: 23656
	public Vector3 pos;

	// Token: 0x04005C69 RID: 23657
	public Vector3 min = Vector3.one * -2f;

	// Token: 0x04005C6A RID: 23658
	public Vector3 max = Vector3.one * 2f;

	// Token: 0x04005C6B RID: 23659
	public float rad = 4f;

	// Token: 0x04005C6C RID: 23660
	[Space]
	public bool debug32;

	// Token: 0x04005C6D RID: 23661
	public uint packed;

	// Token: 0x04005C6E RID: 23662
	public Vector3 unpacked;

	// Token: 0x04005C6F RID: 23663
	[Space]
	public bool debug16;

	// Token: 0x04005C70 RID: 23664
	public ushort packed16;

	// Token: 0x04005C71 RID: 23665
	public Vector3 unpacked16;
}
