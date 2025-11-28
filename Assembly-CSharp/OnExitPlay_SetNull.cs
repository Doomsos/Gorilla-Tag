using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C6C RID: 3180
[AttributeUsage(256)]
public class OnExitPlay_SetNull : OnExitPlay_Attribute
{
	// Token: 0x06004DB7 RID: 19895 RVA: 0x00192298 File Offset: 0x00190498
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
