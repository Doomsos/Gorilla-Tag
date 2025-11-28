using System;

// Token: 0x020002A8 RID: 680
[AttributeUsage(384)]
public class DevInspectorColor : Attribute
{
	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06001116 RID: 4374 RVA: 0x0005BB78 File Offset: 0x00059D78
	public string Color { get; }

	// Token: 0x06001117 RID: 4375 RVA: 0x0005BB80 File Offset: 0x00059D80
	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}
