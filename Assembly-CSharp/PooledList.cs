using System;
using System.Collections.Generic;
using GorillaTag;

// Token: 0x02000C98 RID: 3224
public class PooledList<T> : ObjectPoolEvents
{
	// Token: 0x06004EB3 RID: 20147 RVA: 0x00002789 File Offset: 0x00000989
	void ObjectPoolEvents.OnTaken()
	{
	}

	// Token: 0x06004EB4 RID: 20148 RVA: 0x00197453 File Offset: 0x00195653
	void ObjectPoolEvents.OnReturned()
	{
		this.List.Clear();
	}

	// Token: 0x04005D88 RID: 23944
	public List<T> List = new List<T>();
}
