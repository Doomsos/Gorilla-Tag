using System;
using System.Collections.Generic;

// Token: 0x020002B4 RID: 692
public static class DelegateExtensions
{
	// Token: 0x0600112A RID: 4394 RVA: 0x0005BCD0 File Offset: 0x00059ED0
	public static List<string> ToStringList(this Delegate[] invocationList)
	{
		List<string> list = new List<string>();
		if (invocationList != null)
		{
			foreach (Delegate @delegate in invocationList)
			{
				string name = @delegate.Method.Name;
				string text = (@delegate.Target != null) ? @delegate.Target.GetType().FullName : "Static Method";
				list.Add(text + "." + name);
			}
		}
		return list;
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x0005BD3D File Offset: 0x00059F3D
	public static string ToText(this Delegate[] invocationList)
	{
		return string.Join(", ", invocationList.ToStringList());
	}
}
