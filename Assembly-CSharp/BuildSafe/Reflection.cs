using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000EAE RID: 3758
	public static class Reflection<T>
	{
		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06005DCB RID: 24011 RVA: 0x001E16EC File Offset: 0x001DF8EC
		public static Type Type { get; } = typeof(T);

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x06005DCC RID: 24012 RVA: 0x001E16F3 File Offset: 0x001DF8F3
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06005DCD RID: 24013 RVA: 0x001E16FA File Offset: 0x001DF8FA
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06005DCE RID: 24014 RVA: 0x001E1701 File Offset: 0x001DF901
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06005DCF RID: 24015 RVA: 0x001E1708 File Offset: 0x001DF908
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06005DD0 RID: 24016 RVA: 0x001E170F File Offset: 0x001DF90F
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Enumerable.ToArray<EventInfo>(RuntimeReflectionExtensions.GetRuntimeEvents(Reflection<T>.Type));
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x001E1733 File Offset: 0x001DF933
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Enumerable.ToArray<PropertyInfo>(RuntimeReflectionExtensions.GetRuntimeProperties(Reflection<T>.Type));
		}

		// Token: 0x06005DD2 RID: 24018 RVA: 0x001E1757 File Offset: 0x001DF957
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Enumerable.ToArray<MethodInfo>(RuntimeReflectionExtensions.GetRuntimeMethods(Reflection<T>.Type));
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x001E177B File Offset: 0x001DF97B
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Enumerable.ToArray<FieldInfo>(RuntimeReflectionExtensions.GetRuntimeFields(Reflection<T>.Type));
		}

		// Token: 0x04006BC1 RID: 27585
		private static Type gCachedType;

		// Token: 0x04006BC2 RID: 27586
		private static MethodInfo[] gMethodsCache;

		// Token: 0x04006BC3 RID: 27587
		private static FieldInfo[] gFieldsCache;

		// Token: 0x04006BC4 RID: 27588
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x04006BC5 RID: 27589
		private static EventInfo[] gEventsCache;
	}
}
