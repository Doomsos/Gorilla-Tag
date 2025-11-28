using System;

// Token: 0x02000A1D RID: 2589
[Serializable]
public class ErrorContent
{
	// Token: 0x1700062C RID: 1580
	// (get) Token: 0x060041D7 RID: 16855 RVA: 0x0015CE8C File Offset: 0x0015B08C
	// (set) Token: 0x060041D8 RID: 16856 RVA: 0x0015CE94 File Offset: 0x0015B094
	public string Message { get; set; }

	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x060041D9 RID: 16857 RVA: 0x0015CE9D File Offset: 0x0015B09D
	// (set) Token: 0x060041DA RID: 16858 RVA: 0x0015CEA5 File Offset: 0x0015B0A5
	public string Error { get; set; }

	// Token: 0x060041DB RID: 16859 RVA: 0x0015CEAE File Offset: 0x0015B0AE
	public override string ToString()
	{
		return "Error: " + this.Error + ", Message: " + this.Message;
	}
}
