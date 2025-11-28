using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FCC RID: 4044
	[AttributeUsage(384, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		// Token: 0x0600667F RID: 26239 RVA: 0x002163DE File Offset: 0x002145DE
		public VectorLabelTextAttribute(params string[] labels) : this(-1, labels)
		{
		}

		// Token: 0x06006680 RID: 26240 RVA: 0x0001265B File Offset: 0x0001085B
		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
