using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C64 RID: 3172
[AttributeUsage(256)]
public class OnEnterPlay_Set : OnEnterPlay_Attribute
{
	// Token: 0x06004DA6 RID: 19878 RVA: 0x001922CE File Offset: 0x001904CE
	public OnEnterPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06004DA7 RID: 19879 RVA: 0x001922E0 File Offset: 0x001904E0
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		if (field.FieldType == typeof(ushort))
		{
			field.SetValue(null, Convert.ToUInt16(this.value));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04005CF1 RID: 23793
	private object value;
}
