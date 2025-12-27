using System;
using System.Diagnostics;
using UnityEngine;

namespace GorillaTag
{
	[AttributeUsage(384, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class VectorLabelTextAttribute : PropertyAttribute
	{
		public VectorLabelTextAttribute(params string[] labels) : this(-1, labels)
		{
		}

		public VectorLabelTextAttribute(int width, params string[] labels)
		{
		}
	}
}
