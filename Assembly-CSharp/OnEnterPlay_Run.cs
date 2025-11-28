using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000C67 RID: 3175
[AttributeUsage(64)]
public class OnEnterPlay_Run : OnEnterPlay_Attribute
{
	// Token: 0x06004DAC RID: 19884 RVA: 0x00192403 File Offset: 0x00190603
	public override void OnEnterPlay(MethodInfo method)
	{
		if (!method.IsStatic)
		{
			Debug.LogError(string.Format("Can't Run non-static method {0}.{1}", method.DeclaringType, method.Name));
			return;
		}
		method.Invoke(null, new object[0]);
	}
}
