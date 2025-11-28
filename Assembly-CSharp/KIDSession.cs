using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000A17 RID: 2583
[Serializable]
public class KIDSession
{
	// Token: 0x17000623 RID: 1571
	// (get) Token: 0x060041BF RID: 16831 RVA: 0x0015CE0B File Offset: 0x0015B00B
	// (set) Token: 0x060041C0 RID: 16832 RVA: 0x0015CE13 File Offset: 0x0015B013
	public SessionStatus SessionStatus { get; set; }

	// Token: 0x17000624 RID: 1572
	// (get) Token: 0x060041C1 RID: 16833 RVA: 0x0015CE1C File Offset: 0x0015B01C
	// (set) Token: 0x060041C2 RID: 16834 RVA: 0x0015CE24 File Offset: 0x0015B024
	public GTAgeStatusType AgeStatus { get; set; }

	// Token: 0x17000625 RID: 1573
	// (get) Token: 0x060041C3 RID: 16835 RVA: 0x0015CE2D File Offset: 0x0015B02D
	// (set) Token: 0x060041C4 RID: 16836 RVA: 0x0015CE35 File Offset: 0x0015B035
	public Guid SessionId { get; set; }

	// Token: 0x17000626 RID: 1574
	// (get) Token: 0x060041C5 RID: 16837 RVA: 0x0015CE3E File Offset: 0x0015B03E
	// (set) Token: 0x060041C6 RID: 16838 RVA: 0x0015CE46 File Offset: 0x0015B046
	public string KUID { get; set; }

	// Token: 0x17000627 RID: 1575
	// (get) Token: 0x060041C7 RID: 16839 RVA: 0x0015CE4F File Offset: 0x0015B04F
	// (set) Token: 0x060041C8 RID: 16840 RVA: 0x0015CE57 File Offset: 0x0015B057
	public string etag { get; set; }

	// Token: 0x17000628 RID: 1576
	// (get) Token: 0x060041C9 RID: 16841 RVA: 0x0015CE60 File Offset: 0x0015B060
	// (set) Token: 0x060041CA RID: 16842 RVA: 0x0015CE68 File Offset: 0x0015B068
	public List<Permission> Permissions { get; set; }

	// Token: 0x17000629 RID: 1577
	// (get) Token: 0x060041CB RID: 16843 RVA: 0x0015CE71 File Offset: 0x0015B071
	// (set) Token: 0x060041CC RID: 16844 RVA: 0x0015CE79 File Offset: 0x0015B079
	public DateTime DateOfBirth { get; set; }

	// Token: 0x1700062A RID: 1578
	// (get) Token: 0x060041CD RID: 16845 RVA: 0x0015CE82 File Offset: 0x0015B082
	// (set) Token: 0x060041CE RID: 16846 RVA: 0x0015CE8A File Offset: 0x0015B08A
	public string Jurisdiction { get; set; }
}
