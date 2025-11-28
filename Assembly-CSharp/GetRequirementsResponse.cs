using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Token: 0x02000A20 RID: 2592
[Serializable]
public class GetRequirementsResponse
{
	// Token: 0x1700062E RID: 1582
	// (get) Token: 0x060041DF RID: 16863 RVA: 0x0015CECB File Offset: 0x0015B0CB
	// (set) Token: 0x060041E0 RID: 16864 RVA: 0x0015CED3 File Offset: 0x0015B0D3
	[JsonProperty("age")]
	public int? Age { get; set; }

	// Token: 0x1700062F RID: 1583
	// (get) Token: 0x060041E1 RID: 16865 RVA: 0x0015CEDC File Offset: 0x0015B0DC
	// (set) Token: 0x060041E2 RID: 16866 RVA: 0x0015CEE4 File Offset: 0x0015B0E4
	public int? PlatformMinimumAge { get; set; }

	// Token: 0x17000630 RID: 1584
	// (get) Token: 0x060041E3 RID: 16867 RVA: 0x0015CEED File Offset: 0x0015B0ED
	// (set) Token: 0x060041E4 RID: 16868 RVA: 0x0015CEF5 File Offset: 0x0015B0F5
	[JsonProperty("ageStatus")]
	public SessionStatus AgeStatus { get; set; }

	// Token: 0x17000631 RID: 1585
	// (get) Token: 0x060041E5 RID: 16869 RVA: 0x0015CEFE File Offset: 0x0015B0FE
	// (set) Token: 0x060041E6 RID: 16870 RVA: 0x0015CF06 File Offset: 0x0015B106
	[JsonProperty("digitalContentAge")]
	public int DigitalConsentAge { get; set; }

	// Token: 0x17000632 RID: 1586
	// (get) Token: 0x060041E7 RID: 16871 RVA: 0x0015CF0F File Offset: 0x0015B10F
	// (set) Token: 0x060041E8 RID: 16872 RVA: 0x0015CF17 File Offset: 0x0015B117
	[JsonProperty("minimumAge")]
	public int MinimumAge { get; set; }

	// Token: 0x17000633 RID: 1587
	// (get) Token: 0x060041E9 RID: 16873 RVA: 0x0015CF20 File Offset: 0x0015B120
	// (set) Token: 0x060041EA RID: 16874 RVA: 0x0015CF28 File Offset: 0x0015B128
	[JsonProperty("civilAge")]
	public int CivilAge { get; set; }

	// Token: 0x17000634 RID: 1588
	// (get) Token: 0x060041EB RID: 16875 RVA: 0x0015CF31 File Offset: 0x0015B131
	// (set) Token: 0x060041EC RID: 16876 RVA: 0x0015CF39 File Offset: 0x0015B139
	[JsonProperty("approvedAgeCollectionMethods")]
	public List<ApprovedAgeCollectionMethods> ApprovedAgeCollectionMethods { get; set; }
}
