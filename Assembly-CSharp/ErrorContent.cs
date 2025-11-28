using System;

// Token: 0x02000A1D RID: 2589
[Serializable]
public class ErrorContent
{
	// Token: 0x1700062C RID: 1580
	// (get) Token: 0x060041D7 RID: 16855 RVA: 0x0015CEAC File Offset: 0x0015B0AC
	// (set) Token: 0x060041D8 RID: 16856 RVA: 0x0015CEB4 File Offset: 0x0015B0B4
	public string Message { get; set; }

	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x060041D9 RID: 16857 RVA: 0x0015CEBD File Offset: 0x0015B0BD
	// (set) Token: 0x060041DA RID: 16858 RVA: 0x0015CEC5 File Offset: 0x0015B0C5
	public string Error { get; set; }

	// Token: 0x060041DB RID: 16859 RVA: 0x0015CECE File Offset: 0x0015B0CE
	public override string ToString()
	{
		return "Error: " + this.Error + ", Message: " + this.Message;
	}
}
