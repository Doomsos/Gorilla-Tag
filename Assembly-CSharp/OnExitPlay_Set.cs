using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C6D RID: 3181
[AttributeUsage(256)]
public class OnExitPlay_Set : OnExitPlay_Attribute
{
	// Token: 0x06004DB9 RID: 19897 RVA: 0x001924F4 File Offset: 0x001906F4
	public OnExitPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06004DBA RID: 19898 RVA: 0x00192503 File Offset: 0x00190703
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04005CF7 RID: 23799
	private object value;
}
