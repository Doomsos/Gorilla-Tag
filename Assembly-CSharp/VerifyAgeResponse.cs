using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x02000A26 RID: 2598
public class VerifyAgeResponse
{
	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x060041F3 RID: 16883 RVA: 0x0015CF42 File Offset: 0x0015B142
	// (set) Token: 0x060041F4 RID: 16884 RVA: 0x0015CF4A File Offset: 0x0015B14A
	public SessionStatus Status { get; set; }

	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x060041F5 RID: 16885 RVA: 0x0015CF53 File Offset: 0x0015B153
	// (set) Token: 0x060041F6 RID: 16886 RVA: 0x0015CF5B File Offset: 0x0015B15B
	[Nullable(2)]
	public Session Session { [NullableContext(2)] get; [NullableContext(2)] set; }

	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x060041F7 RID: 16887 RVA: 0x0015CF64 File Offset: 0x0015B164
	// (set) Token: 0x060041F8 RID: 16888 RVA: 0x0015CF6C File Offset: 0x0015B16C
	public KIDDefaultSession DefaultSession { get; set; }
}
