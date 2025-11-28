using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x02000A26 RID: 2598
public class VerifyAgeResponse
{
	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x060041F3 RID: 16883 RVA: 0x0015CF62 File Offset: 0x0015B162
	// (set) Token: 0x060041F4 RID: 16884 RVA: 0x0015CF6A File Offset: 0x0015B16A
	public SessionStatus Status { get; set; }

	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x060041F5 RID: 16885 RVA: 0x0015CF73 File Offset: 0x0015B173
	// (set) Token: 0x060041F6 RID: 16886 RVA: 0x0015CF7B File Offset: 0x0015B17B
	[Nullable(2)]
	public Session Session { [NullableContext(2)] get; [NullableContext(2)] set; }

	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x060041F7 RID: 16887 RVA: 0x0015CF84 File Offset: 0x0015B184
	// (set) Token: 0x060041F8 RID: 16888 RVA: 0x0015CF8C File Offset: 0x0015B18C
	public KIDDefaultSession DefaultSession { get; set; }
}
