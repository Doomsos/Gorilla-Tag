using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	public static class Reflection<T>
	{
		public static Type Type { get; } = typeof(T);

		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Enumerable.ToArray<EventInfo>(RuntimeReflectionExtensions.GetRuntimeEvents(Reflection<T>.Type));
		}

		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Enumerable.ToArray<PropertyInfo>(RuntimeReflectionExtensions.GetRuntimeProperties(Reflection<T>.Type));
		}

		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Enumerable.ToArray<MethodInfo>(RuntimeReflectionExtensions.GetRuntimeMethods(Reflection<T>.Type));
		}

		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Enumerable.ToArray<FieldInfo>(RuntimeReflectionExtensions.GetRuntimeFields(Reflection<T>.Type));
		}

		private static Type gCachedType;

		private static MethodInfo[] gMethodsCache;

		private static FieldInfo[] gFieldsCache;

		private static PropertyInfo[] gPropertiesCache;

		private static EventInfo[] gEventsCache;
	}
}
