using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000C93 RID: 3219
public static class UnsafeUtils
{
	// Token: 0x06004EAC RID: 20140 RVA: 0x00197418 File Offset: 0x00195618
	public unsafe static ref readonly T[] GetInternalArray<T>(this List<T> list)
	{
		if (list == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return ref Unsafe.As<List<T>, StrongBox<T[]>>(ref list)->Value;
	}

	// Token: 0x06004EAD RID: 20141 RVA: 0x00197430 File Offset: 0x00195630
	public unsafe static ref readonly T[] GetInvocationListUnsafe<T>(this T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return Unsafe.As<Delegate[], T[]>(ref Unsafe.As<T, UnsafeUtils._MultiDelegateFields>(ref @delegate)->delegates);
	}

	// Token: 0x02000C94 RID: 3220
	[StructLayout(0)]
	private class _MultiDelegateFields : UnsafeUtils._DelegateFields
	{
		// Token: 0x04005D77 RID: 23927
		public Delegate[] delegates;
	}

	// Token: 0x02000C95 RID: 3221
	[StructLayout(0)]
	private class _DelegateFields
	{
		// Token: 0x04005D78 RID: 23928
		public IntPtr method_ptr;

		// Token: 0x04005D79 RID: 23929
		public IntPtr invoke_impl;

		// Token: 0x04005D7A RID: 23930
		public object m_target;

		// Token: 0x04005D7B RID: 23931
		public IntPtr method;

		// Token: 0x04005D7C RID: 23932
		public IntPtr delegate_trampoline;

		// Token: 0x04005D7D RID: 23933
		public IntPtr extra_arg;

		// Token: 0x04005D7E RID: 23934
		public IntPtr method_code;

		// Token: 0x04005D7F RID: 23935
		public IntPtr interp_method;

		// Token: 0x04005D80 RID: 23936
		public IntPtr interp_invoke_impl;

		// Token: 0x04005D81 RID: 23937
		public MethodInfo method_info;

		// Token: 0x04005D82 RID: 23938
		public MethodInfo original_method_info;

		// Token: 0x04005D83 RID: 23939
		public UnsafeUtils._DelegateData data;

		// Token: 0x04005D84 RID: 23940
		public bool method_is_virtual;
	}

	// Token: 0x02000C96 RID: 3222
	[StructLayout(0)]
	private class _DelegateData
	{
		// Token: 0x04005D85 RID: 23941
		public Type target_type;

		// Token: 0x04005D86 RID: 23942
		public string method_name;

		// Token: 0x04005D87 RID: 23943
		public bool curried_first_arg;
	}
}
