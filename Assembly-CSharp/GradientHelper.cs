using System;
using UnityEngine;

// Token: 0x020009D8 RID: 2520
public static class GradientHelper
{
	// Token: 0x0600403D RID: 16445 RVA: 0x00158BC4 File Offset: 0x00156DC4
	public static Gradient FromColor(Color color)
	{
		float a = color.a;
		Color color2 = color;
		color2.a = 1f;
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(color2, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(a, 1f)
			}
		};
	}
}
