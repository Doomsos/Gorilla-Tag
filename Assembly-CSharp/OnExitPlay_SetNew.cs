using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C6E RID: 3182
[AttributeUsage(256)]
public class OnExitPlay_SetNew : OnExitPlay_Attribute
{
	// Token: 0x06004DBB RID: 19899 RVA: 0x00192518 File Offset: 0x00190718
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
