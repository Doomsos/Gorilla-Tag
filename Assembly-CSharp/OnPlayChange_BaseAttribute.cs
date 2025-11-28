using System;
using System.Reflection;

// Token: 0x02000C62 RID: 3170
public class OnPlayChange_BaseAttribute : Attribute
{
	// Token: 0x06004DA1 RID: 19873 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnEnterPlay(FieldInfo field)
	{
	}

	// Token: 0x06004DA2 RID: 19874 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnEnterPlay(MethodInfo method)
	{
	}
}
