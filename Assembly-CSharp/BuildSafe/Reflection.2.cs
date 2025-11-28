using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000EAF RID: 3759
	public static class Reflection
	{
		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x06005DD5 RID: 24021 RVA: 0x001E17B0 File Offset: 0x001DF9B0
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x06005DD6 RID: 24022 RVA: 0x001E17B7 File Offset: 0x001DF9B7
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x06005DD7 RID: 24023 RVA: 0x001E17BE File Offset: 0x001DF9BE
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x001E17CC File Offset: 0x001DF9CC
		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = Enumerable.ToArray<Assembly>(Enumerable.Where<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), (Assembly a) => a != null));
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x001E1820 File Offset: 0x001DFA20
		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = Enumerable.ToArray<Type>(Enumerable.Where<Type>(Enumerable.SelectMany<Assembly, Type>(Reflection.PreFetchAllAssemblies(), (Assembly a) => a.GetTypes()), (Type t) => t != null));
		}

		// Token: 0x06005DDA RID: 24026 RVA: 0x001E1894 File Offset: 0x001DFA94
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return Enumerable.ToArray<MethodInfo>(Enumerable.Where<MethodInfo>(Enumerable.SelectMany<Type, MethodInfo>(Reflection.AllTypes, (Type t) => RuntimeReflectionExtensions.GetRuntimeMethods(t)), (MethodInfo m) => m.GetCustomAttributes(typeof(T), false).Length != 0));
		}

		// Token: 0x04006BC7 RID: 27591
		private static Assembly[] gAssemblyCache;

		// Token: 0x04006BC8 RID: 27592
		private static Type[] gTypeCache;
	}
}
