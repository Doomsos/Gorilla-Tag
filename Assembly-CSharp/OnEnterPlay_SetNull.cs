using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C63 RID: 3171
[AttributeUsage(256)]
public class OnEnterPlay_SetNull : OnEnterPlay_Attribute
{
	// Token: 0x06004DA4 RID: 19876 RVA: 0x001922B8 File Offset: 0x001904B8
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNull non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, null);
	}
}
