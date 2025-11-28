using System;
using System.Collections.Generic;

// Token: 0x0200021C RID: 540
[Serializable]
public class FoundAllocatorsMapped
{
	// Token: 0x040011E1 RID: 4577
	public string path;

	// Token: 0x040011E2 RID: 4578
	public List<ViewsAndAllocator> allocators = new List<ViewsAndAllocator>();

	// Token: 0x040011E3 RID: 4579
	public List<FoundAllocatorsMapped> subGroups = new List<FoundAllocatorsMapped>();
}
