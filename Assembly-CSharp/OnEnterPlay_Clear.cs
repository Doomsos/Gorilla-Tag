using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C66 RID: 3174
[AttributeUsage(256)]
public class OnEnterPlay_Clear : OnEnterPlay_Attribute
{
	// Token: 0x06004DAA RID: 19882 RVA: 0x001923C8 File Offset: 0x001905C8
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		MethodInfo method = field.FieldType.GetMethod("Clear");
		object value = field.GetValue(null);
		if (value != null)
		{
			method.Invoke(value, new object[0]);
		}
	}
}
