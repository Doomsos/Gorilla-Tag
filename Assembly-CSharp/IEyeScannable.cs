using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public interface IEyeScannable
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x0600040D RID: 1037
	int scannableId { get; }

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x0600040E RID: 1038
	Vector3 Position { get; }

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x0600040F RID: 1039
	Bounds Bounds { get; }

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000410 RID: 1040
	IList<KeyValueStringPair> Entries { get; }

	// Token: 0x06000411 RID: 1041
	void OnEnable();

	// Token: 0x06000412 RID: 1042
	void OnDisable();

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000413 RID: 1043
	// (remove) Token: 0x06000414 RID: 1044
	event Action OnDataChange;
}
