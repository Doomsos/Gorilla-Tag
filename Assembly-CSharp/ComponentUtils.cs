using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C3B RID: 3131
public static class ComponentUtils
{
	// Token: 0x06004CCE RID: 19662 RVA: 0x0018EBE4 File Offset: 0x0018CDE4
	public static T EnsureComponent<T>(this Component ctx, ref T target) where T : Component
	{
		if (ctx.AsNull<Component>() == null)
		{
			return default(T);
		}
		if (target.AsNull<T>() != null)
		{
			return target;
		}
		return target = ctx.GetComponent<T>();
	}

	// Token: 0x06004CCF RID: 19663 RVA: 0x0018EC37 File Offset: 0x0018CE37
	public static bool TryEnsureComponent<T>(this Component ctx, ref T target) where T : Component
	{
		if (ctx.AsNull<Component>() == null)
		{
			return false;
		}
		if (target.AsNull<T>() != null)
		{
			return true;
		}
		target = ctx.GetComponent<T>();
		return true;
	}

	// Token: 0x06004CD0 RID: 19664 RVA: 0x0018EC70 File Offset: 0x0018CE70
	public static T AddComponent<T>(this Component c) where T : Component
	{
		return c.gameObject.AddComponent<T>();
	}

	// Token: 0x06004CD1 RID: 19665 RVA: 0x0018EC7D File Offset: 0x0018CE7D
	public static void GetOrAddComponent<T>(this Component c, out T result) where T : Component
	{
		if (!c.TryGetComponent<T>(ref result))
		{
			result = c.gameObject.AddComponent<T>();
		}
	}

	// Token: 0x06004CD2 RID: 19666 RVA: 0x0018EC99 File Offset: 0x0018CE99
	public static bool GetComponentAndSetFieldIfNullElseLogAndDisable<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "Disabling.", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (c.GetComponentAndSetFieldIfNullElseLog(ref fieldRef, fieldName, fieldTypeName, msgSuffix, caller))
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	// Token: 0x06004CD3 RID: 19667 RVA: 0x0018ECB4 File Offset: 0x0018CEB4
	public static bool GetComponentAndSetFieldIfNullElseLog<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (fieldRef != null)
		{
			return true;
		}
		fieldRef = c.GetComponent<T>();
		if (fieldRef != null)
		{
			return true;
		}
		Debug.LogError(string.Concat(new string[]
		{
			caller,
			": Could not find ",
			fieldTypeName,
			" \"",
			fieldName,
			"\" on \"",
			c.name,
			"\". ",
			msgSuffix
		}), c);
		return false;
	}

	// Token: 0x06004CD4 RID: 19668 RVA: 0x0018ED45 File Offset: 0x0018CF45
	public static bool DisableIfNull<T>(this Behaviour c, T fieldRef, string fieldName, string fieldTypeName, [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Object
	{
		if (fieldRef != null)
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	// Token: 0x06004CD5 RID: 19669 RVA: 0x0018ED5F File Offset: 0x0018CF5F
	public static Hash128 ComputeStaticHash128(Component c, string k)
	{
		return ComponentUtils.ComputeStaticHash128(c, StaticHash.Compute(k));
	}

	// Token: 0x06004CD6 RID: 19670 RVA: 0x0018ED70 File Offset: 0x0018CF70
	public static Hash128 ComputeStaticHash128(Component c, int k = 0)
	{
		if (c == null)
		{
			return default(Hash128);
		}
		Transform transform = c.transform;
		Component[] components = c.gameObject.GetComponents(typeof(Component));
		uint[] array = ComponentUtils.kHashBits;
		int siblingIndex = transform.GetSiblingIndex();
		int num = components.Length;
		int num2 = 0;
		while (num2 < num && c != components[num2])
		{
			num2++;
		}
		int num3 = StaticHash.Compute(k + 2, 1);
		int num4 = StaticHash.Compute(siblingIndex + 4, num3);
		int num5 = StaticHash.Compute(num + 8, num4);
		int num6 = StaticHash.Compute(num2 + 16, num5);
		array[0] = (uint)num3;
		array[1] = (uint)num4;
		array[2] = (uint)num5;
		array[3] = (uint)num6;
		SRand srand = new SRand(StaticHash.Compute(num3, num4, num5, num6));
		srand.Shuffle<uint>(array);
		Hash128 result;
		result..ctor(array[0], array[1], array[2], array[3]);
		Hash128 hash = Hash128.Compute(c.GetType().FullName);
		Hash128 hash2 = TransformUtils.ComputePathHash(transform);
		Hash128 hash3 = transform.localToWorldMatrix.QuantizedHash128();
		HashUtilities.AppendHash(ref hash, ref result);
		HashUtilities.AppendHash(ref hash2, ref result);
		HashUtilities.AppendHash(ref hash3, ref result);
		return result;
	}

	// Token: 0x04005C84 RID: 23684
	private static readonly uint[] kHashBits = new uint[4];
}
