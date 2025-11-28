using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	public static class Reflection
	{
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = Enumerable.ToArray<Assembly>(Enumerable.Where<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), (Assembly a) => a != null));
		}

		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = Enumerable.ToArray<Type>(Enumerable.Where<Type>(Enumerable.SelectMany<Assembly, Type>(Reflection.PreFetchAllAssemblies(), (Assembly a) => a.GetTypes()), (Type t) => t != null));
		}

		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return Enumerable.ToArray<MethodInfo>(Enumerable.Where<MethodInfo>(Enumerable.SelectMany<Type, MethodInfo>(Reflection.AllTypes, (Type t) => RuntimeReflectionExtensions.GetRuntimeMethods(t)), (MethodInfo m) => m.GetCustomAttributes(typeof(T), false).Length != 0));
		}

		private static Assembly[] gAssemblyCache;

		private static Type[] gTypeCache;
	}
}
