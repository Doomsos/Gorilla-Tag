using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000CE5 RID: 3301
public static class GTUberShaderUtils
{
	// Token: 0x0600504E RID: 20558 RVA: 0x0019CBCA File Offset: 0x0019ADCA
	[MethodImpl(256)]
	public static void SetStencilComparison(this Material m, GTShaderStencilCompare cmp)
	{
		m.SetFloat(GTUberShaderUtils._StencilComparison, (float)cmp);
	}

	// Token: 0x0600504F RID: 20559 RVA: 0x0019CBDE File Offset: 0x0019ADDE
	[MethodImpl(256)]
	public static void SetStencilPassFrontOp(this Material m, GTShaderStencilOp op)
	{
		m.SetFloat(GTUberShaderUtils._StencilPassFront, (float)op);
	}

	// Token: 0x06005050 RID: 20560 RVA: 0x0019CBF2 File Offset: 0x0019ADF2
	[MethodImpl(256)]
	public static void SetStencilReferenceValue(this Material m, int value)
	{
		m.SetFloat(GTUberShaderUtils._StencilReference, (float)value);
	}

	// Token: 0x06005051 RID: 20561 RVA: 0x0019CC08 File Offset: 0x0019AE08
	public static void SetVisibleToXRay(this Material m, bool visible, bool saveToDisk = false)
	{
		GTShaderStencilCompare cmp = visible ? GTShaderStencilCompare.Equal : GTShaderStencilCompare.NotEqual;
		GTShaderStencilOp op = visible ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep;
		m.SetStencilComparison(cmp);
		m.SetStencilPassFrontOp(op);
		m.SetStencilReferenceValue(7);
	}

	// Token: 0x06005052 RID: 20562 RVA: 0x0019CC3C File Offset: 0x0019AE3C
	public static void SetRevealsXRay(this Material m, bool reveals, bool changeQueue = true, bool saveToDisk = false)
	{
		m.SetFloat(GTUberShaderUtils._ZWrite, (float)(reveals ? 0 : 1));
		m.SetFloat(GTUberShaderUtils._ColorMask_, (float)(reveals ? 0 : 14));
		m.SetStencilComparison(GTShaderStencilCompare.Disabled);
		m.SetStencilPassFrontOp(reveals ? GTShaderStencilOp.Replace : GTShaderStencilOp.Keep);
		m.SetStencilReferenceValue(reveals ? 7 : 0);
		if (changeQueue)
		{
			int renderQueue = m.renderQueue;
			m.renderQueue = renderQueue + (reveals ? -1 : 1);
		}
	}

	// Token: 0x06005053 RID: 20563 RVA: 0x0019CCB4 File Offset: 0x0019AEB4
	public static int GetNearestRenderQueue(this Material m, out RenderQueue queue)
	{
		int renderQueue = m.renderQueue;
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < GTUberShaderUtils.kRenderQueueInts.Length; i++)
		{
			int num3 = GTUberShaderUtils.kRenderQueueInts[i];
			int num4 = Math.Abs(num3 - renderQueue);
			if (num2 > num4)
			{
				num = num3;
				num2 = num4;
			}
		}
		queue = num;
		return num;
	}

	// Token: 0x06005054 RID: 20564 RVA: 0x0019CD05 File Offset: 0x0019AF05
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitOnLoad()
	{
		GTUberShaderUtils.kUberShader = Shader.Find("GorillaTag/UberShader");
	}

	// Token: 0x04005F3A RID: 24378
	private static Shader kUberShader;

	// Token: 0x04005F3B RID: 24379
	private static readonly ShaderHashId _StencilComparison = "_StencilComparison";

	// Token: 0x04005F3C RID: 24380
	private static readonly ShaderHashId _StencilPassFront = "_StencilPassFront";

	// Token: 0x04005F3D RID: 24381
	private static readonly ShaderHashId _StencilReference = "_StencilReference";

	// Token: 0x04005F3E RID: 24382
	private static readonly ShaderHashId _ColorMask_ = "_ColorMask_";

	// Token: 0x04005F3F RID: 24383
	private static readonly ShaderHashId _ManualZWrite = "_ManualZWrite";

	// Token: 0x04005F40 RID: 24384
	private static readonly ShaderHashId _ZWrite = "_ZWrite";

	// Token: 0x04005F41 RID: 24385
	private static readonly int[] kRenderQueueInts = new int[]
	{
		1000,
		2000,
		2450,
		2500,
		3000,
		4000
	};
}
