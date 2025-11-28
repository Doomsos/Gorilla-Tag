using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000A0C RID: 2572
[Serializable]
public class KIDDefaultSession
{
	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x060041B2 RID: 16818 RVA: 0x0015CDB6 File Offset: 0x0015AFB6
	// (set) Token: 0x060041B3 RID: 16819 RVA: 0x0015CDBE File Offset: 0x0015AFBE
	public List<Permission> Permissions { get; set; }

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x060041B4 RID: 16820 RVA: 0x0015CDC7 File Offset: 0x0015AFC7
	// (set) Token: 0x060041B5 RID: 16821 RVA: 0x0015CDCF File Offset: 0x0015AFCF
	public AgeStatusType AgeStatus { get; set; }
}
