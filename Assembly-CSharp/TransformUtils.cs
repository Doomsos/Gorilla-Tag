using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C8B RID: 3211
public static class TransformUtils
{
	// Token: 0x06004E7D RID: 20093 RVA: 0x00196C38 File Offset: 0x00194E38
	public static int ComputePathHashByInstance(Transform t)
	{
		if (t == null)
		{
			return 0;
		}
		int num = 0;
		Transform transform = t;
		while (transform != null)
		{
			num = StaticHash.Compute(num, transform.GetHashCode());
			transform = transform.parent;
		}
		return num;
	}

	// Token: 0x06004E7E RID: 20094 RVA: 0x00196C74 File Offset: 0x00194E74
	public static Hash128 ComputePathHash(Transform t)
	{
		if (t == null)
		{
			return default(Hash128);
		}
		Hash128 result = default(Hash128);
		Transform transform = t;
		while (transform != null)
		{
			Hash128 hash = Hash128.Compute(transform.name);
			HashUtilities.AppendHash(ref hash, ref result);
			transform = transform.parent;
		}
		return result;
	}

	// Token: 0x06004E7F RID: 20095 RVA: 0x00196CC8 File Offset: 0x00194EC8
	public static string GetScenePath(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform parent = t.parent;
		while (parent != null)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	// Token: 0x06004E80 RID: 20096 RVA: 0x00196D14 File Offset: 0x00194F14
	public static string GetScenePathReverse(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform parent = t.parent;
		Queue<string> queue = new Queue<string>(16);
		while (parent != null)
		{
			queue.Enqueue(parent.name);
			parent = parent.parent;
		}
		while (queue.Count > 0)
		{
			text = text + "/" + queue.Dequeue();
		}
		return text;
	}

	// Token: 0x04005D6A RID: 23914
	private const string kFwdSlash = "/";
}
