using System;

// Token: 0x020002AD RID: 685
public class ComponentMember
{
	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x0600111C RID: 4380 RVA: 0x0005BBA9 File Offset: 0x00059DA9
	public string Name { get; }

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x0600111D RID: 4381 RVA: 0x0005BBB1 File Offset: 0x00059DB1
	public string Value
	{
		get
		{
			return this.getValue.Invoke();
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x0600111E RID: 4382 RVA: 0x0005BBBE File Offset: 0x00059DBE
	public bool IsStarred { get; }

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x0600111F RID: 4383 RVA: 0x0005BBC6 File Offset: 0x00059DC6
	public string Color { get; }

	// Token: 0x06001120 RID: 4384 RVA: 0x0005BBCE File Offset: 0x00059DCE
	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	// Token: 0x040015A9 RID: 5545
	private Func<string> getValue;

	// Token: 0x040015AA RID: 5546
	public string computedPrefix;

	// Token: 0x040015AB RID: 5547
	public string computedSuffix;
}
