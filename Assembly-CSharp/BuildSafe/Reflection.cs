using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02000EAE RID: 3758
	public static class Reflection<T>
	{
		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06005DCB RID: 24011 RVA: 0x001E170C File Offset: 0x001DF90C
		public static Type Type { get; } = typeof(T);

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x06005DCC RID: 24012 RVA: 0x001E1713 File Offset: 0x001DF913
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06005DCD RID: 24013 RVA: 0x001E171A File Offset: 0x001DF91A
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06005DCE RID: 24014 RVA: 0x001E1721 File Offset: 0x001DF921
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06005DCF RID: 24015 RVA: 0x001E1728 File Offset: 0x001DF928
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06005DD0 RID: 24016 RVA: 0x001E172F File Offset: 0x001DF92F
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Enumerable.ToArray<EventInfo>(RuntimeReflectionExtensions.GetRuntimeEvents(Reflection<T>.Type));
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x001E1753 File Offset: 0x001DF953
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Enumerable.ToArray<PropertyInfo>(RuntimeReflectionExtensions.GetRuntimeProperties(Reflection<T>.Type));
		}

		// Token: 0x06005DD2 RID: 24018 RVA: 0x001E1777 File Offset: 0x001DF977
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Enumerable.ToArray<MethodInfo>(RuntimeReflectionExtensions.GetRuntimeMethods(Reflection<T>.Type));
		}

		// Token: 0x06005DD3 RID: 24019 RVA: 0x001E179B File Offset: 0x001DF99B
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
