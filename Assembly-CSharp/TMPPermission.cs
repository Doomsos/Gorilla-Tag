using System;
using Newtonsoft.Json;

// Token: 0x02000A15 RID: 2581
[Serializable]
public class TMPPermission
{
	// Token: 0x17000620 RID: 1568
	// (get) Token: 0x060041B7 RID: 16823 RVA: 0x0015CDB8 File Offset: 0x0015AFB8
	// (set) Token: 0x060041B8 RID: 16824 RVA: 0x0015CDC0 File Offset: 0x0015AFC0
	[JsonProperty("name")]
	public string Name { get; set; }

	// Token: 0x17000621 RID: 1569
	// (get) Token: 0x060041B9 RID: 16825 RVA: 0x0015CDC9 File Offset: 0x0015AFC9
	// (set) Token: 0x060041BA RID: 16826 RVA: 0x0015CDD1 File Offset: 0x0015AFD1
	[JsonProperty("enabled")]
	public bool Enabled { get; set; }

	// Token: 0x17000622 RID: 1570
	// (get) Token: 0x060041BB RID: 16827 RVA: 0x0015CDDA File Offset: 0x0015AFDA
	// (set) Token: 0x060041BC RID: 16828 RVA: 0x0015CDE2 File Offset: 0x0015AFE2
	[JsonProperty("managedBy")]
	public ManagedBy ManagedBy { get; set; }
}
