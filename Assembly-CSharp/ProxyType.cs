using System;
using System.Globalization;
using System.Reflection;

// Token: 0x02000CAA RID: 3242
public class ProxyType : Type
{
	// Token: 0x06004F1C RID: 20252 RVA: 0x00198838 File Offset: 0x00196A38
	public ProxyType()
	{
	}

	// Token: 0x06004F1D RID: 20253 RVA: 0x00198850 File Offset: 0x00196A50
	public ProxyType(string typeName)
	{
		this._typeName = typeName;
	}

	// Token: 0x17000758 RID: 1880
	// (get) Token: 0x06004F1E RID: 20254 RVA: 0x0019886F File Offset: 0x00196A6F
	public override string Name
	{
		get
		{
			return this._typeName;
		}
	}

	// Token: 0x17000759 RID: 1881
	// (get) Token: 0x06004F1F RID: 20255 RVA: 0x00198877 File Offset: 0x00196A77
	public override string FullName
	{
		get
		{
			return ProxyType.kPrefix + this._typeName;
		}
	}

	// Token: 0x06004F20 RID: 20256 RVA: 0x0019888C File Offset: 0x00196A8C
	public static ProxyType Parse(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentNullException("input");
		}
		input = input.Trim();
		if (!input.Contains(ProxyType.kPrefix, 3))
		{
			return ProxyType.kInvalidType;
		}
		if (!input.StartsWith(ProxyType.kPrefix, 3))
		{
			return ProxyType.kInvalidType;
		}
		if (input.Contains(','))
		{
			input = input.Split(',', 0)[0];
		}
		string text = input.Split('.', 0)[1].Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return ProxyType.kInvalidType;
		}
		return new ProxyType(text);
	}

	// Token: 0x06004F21 RID: 20257 RVA: 0x00198918 File Offset: 0x00196B18
	public override string ToString()
	{
		return base.ToString() + "." + this._typeName;
	}

	// Token: 0x06004F22 RID: 20258 RVA: 0x00198930 File Offset: 0x00196B30
	public override object[] GetCustomAttributes(bool inherit)
	{
		return this._self.GetCustomAttributes(inherit);
	}

	// Token: 0x06004F23 RID: 20259 RVA: 0x0019893E File Offset: 0x00196B3E
	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return this._self.GetCustomAttributes(attributeType, inherit);
	}

	// Token: 0x06004F24 RID: 20260 RVA: 0x0019894D File Offset: 0x00196B4D
	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return this._self.IsDefined(attributeType, inherit);
	}

	// Token: 0x1700075A RID: 1882
	// (get) Token: 0x06004F25 RID: 20261 RVA: 0x0019895C File Offset: 0x00196B5C
	public override Module Module
	{
		get
		{
			return this._self.Module;
		}
	}

	// Token: 0x1700075B RID: 1883
	// (get) Token: 0x06004F26 RID: 20262 RVA: 0x00198969 File Offset: 0x00196B69
	public override string Namespace
	{
		get
		{
			return this._self.Namespace;
		}
	}

	// Token: 0x06004F27 RID: 20263 RVA: 0x00002076 File Offset: 0x00000276
	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return 0;
	}

	// Token: 0x06004F28 RID: 20264 RVA: 0x000743B1 File Offset: 0x000725B1
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06004F29 RID: 20265 RVA: 0x00198976 File Offset: 0x00196B76
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return this._self.GetConstructors(bindingAttr);
	}

	// Token: 0x06004F2A RID: 20266 RVA: 0x00198984 File Offset: 0x00196B84
	public override Type GetElementType()
	{
		return this._self.GetElementType();
	}

	// Token: 0x06004F2B RID: 20267 RVA: 0x00198991 File Offset: 0x00196B91
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		return this._self.GetEvent(name, bindingAttr);
	}

	// Token: 0x06004F2C RID: 20268 RVA: 0x001989A0 File Offset: 0x00196BA0
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return this._self.GetEvents(bindingAttr);
	}

	// Token: 0x06004F2D RID: 20269 RVA: 0x001989AE File Offset: 0x00196BAE
	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		return this._self.GetField(name, bindingAttr);
	}

	// Token: 0x06004F2E RID: 20270 RVA: 0x001989BD File Offset: 0x00196BBD
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return this._self.GetFields(bindingAttr);
	}

	// Token: 0x06004F2F RID: 20271 RVA: 0x001989CB File Offset: 0x00196BCB
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		return this._self.GetMembers(bindingAttr);
	}

	// Token: 0x06004F30 RID: 20272 RVA: 0x000743B1 File Offset: 0x000725B1
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06004F31 RID: 20273 RVA: 0x001989D9 File Offset: 0x00196BD9
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return this._self.GetMethods(bindingAttr);
	}

	// Token: 0x06004F32 RID: 20274 RVA: 0x001989E7 File Offset: 0x00196BE7
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return this._self.GetProperties(bindingAttr);
	}

	// Token: 0x06004F33 RID: 20275 RVA: 0x001989F8 File Offset: 0x00196BF8
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		return this._self.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
	}

	// Token: 0x1700075C RID: 1884
	// (get) Token: 0x06004F34 RID: 20276 RVA: 0x00198A1D File Offset: 0x00196C1D
	public override Type UnderlyingSystemType
	{
		get
		{
			return this._self.UnderlyingSystemType;
		}
	}

	// Token: 0x06004F35 RID: 20277 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsArrayImpl()
	{
		return false;
	}

	// Token: 0x06004F36 RID: 20278 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsByRefImpl()
	{
		return false;
	}

	// Token: 0x06004F37 RID: 20279 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsCOMObjectImpl()
	{
		return false;
	}

	// Token: 0x06004F38 RID: 20280 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPointerImpl()
	{
		return false;
	}

	// Token: 0x06004F39 RID: 20281 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	// Token: 0x1700075D RID: 1885
	// (get) Token: 0x06004F3A RID: 20282 RVA: 0x00198A2A File Offset: 0x00196C2A
	public override Assembly Assembly
	{
		get
		{
			return this._self.Assembly;
		}
	}

	// Token: 0x1700075E RID: 1886
	// (get) Token: 0x06004F3B RID: 20283 RVA: 0x00198A37 File Offset: 0x00196C37
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName.Replace("ProxyType", this.FullName);
		}
	}

	// Token: 0x1700075F RID: 1887
	// (get) Token: 0x06004F3C RID: 20284 RVA: 0x00198A54 File Offset: 0x00196C54
	public override Type BaseType
	{
		get
		{
			return this._self.BaseType;
		}
	}

	// Token: 0x17000760 RID: 1888
	// (get) Token: 0x06004F3D RID: 20285 RVA: 0x00198A61 File Offset: 0x00196C61
	public override Guid GUID
	{
		get
		{
			return this._self.GUID;
		}
	}

	// Token: 0x06004F3E RID: 20286 RVA: 0x000743B1 File Offset: 0x000725B1
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06004F3F RID: 20287 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool HasElementTypeImpl()
	{
		return false;
	}

	// Token: 0x06004F40 RID: 20288 RVA: 0x00198A6E File Offset: 0x00196C6E
	public override Type GetNestedType(string name, BindingFlags bindingAttr)
	{
		return this._self.GetNestedType(name, bindingAttr);
	}

	// Token: 0x06004F41 RID: 20289 RVA: 0x00198A7D File Offset: 0x00196C7D
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return this._self.GetNestedTypes(bindingAttr);
	}

	// Token: 0x06004F42 RID: 20290 RVA: 0x00198A8B File Offset: 0x00196C8B
	public override Type GetInterface(string name, bool ignoreCase)
	{
		return this._self.GetInterface(name, ignoreCase);
	}

	// Token: 0x06004F43 RID: 20291 RVA: 0x00198A9A File Offset: 0x00196C9A
	public override Type[] GetInterfaces()
	{
		return this._self.GetInterfaces();
	}

	// Token: 0x04005DB4 RID: 23988
	private Type _self = typeof(ProxyType);

	// Token: 0x04005DB5 RID: 23989
	private readonly string _typeName;

	// Token: 0x04005DB6 RID: 23990
	private static readonly string kPrefix = "ProxyType.";

	// Token: 0x04005DB7 RID: 23991
	private static InvalidType kInvalidType = new InvalidType();
}
