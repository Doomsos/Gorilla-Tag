using System;
using System.Collections.Generic;

// Token: 0x02000343 RID: 835
[Serializable]
public class SerializablePerformanceReport<T>
{
	// Token: 0x04001EA6 RID: 7846
	public string reportDate;

	// Token: 0x04001EA7 RID: 7847
	public string version;

	// Token: 0x04001EA8 RID: 7848
	public List<T> results;
}
