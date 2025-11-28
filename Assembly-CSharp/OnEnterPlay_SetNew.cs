using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C65 RID: 3173
[AttributeUsage(256)]
public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	// Token: 0x06004DA8 RID: 19880 RVA: 0x00192350 File Offset: 0x00190550
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNew non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		object obj = field.FieldType.GetConstructor(new Type[0]).Invoke(new object[0]);
		field.SetValue(null, obj);
	}
}
