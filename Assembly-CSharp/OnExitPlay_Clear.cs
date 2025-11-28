using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C6F RID: 3183
[AttributeUsage(256)]
public class OnExitPlay_Clear : OnExitPlay_Attribute
{
	// Token: 0x06004DBD RID: 19901 RVA: 0x00192570 File Offset: 0x00190770
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}
}
