using System;

// Token: 0x02000CA9 RID: 3241
public class InvalidType : ProxyType
{
	// Token: 0x17000755 RID: 1877
	// (get) Token: 0x06004F18 RID: 20248 RVA: 0x001987D9 File Offset: 0x001969D9
	public override string Name
	{
		get
		{
			return this._self.Name;
		}
	}

	// Token: 0x17000756 RID: 1878
	// (get) Token: 0x06004F19 RID: 20249 RVA: 0x001987E6 File Offset: 0x001969E6
	public override string FullName
	{
		get
		{
			return this._self.FullName;
		}
	}

	// Token: 0x17000757 RID: 1879
	// (get) Token: 0x06004F1A RID: 20250 RVA: 0x001987F3 File Offset: 0x001969F3
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName;
		}
	}

	// Token: 0x04005DB3 RID: 23987
	private Type _self = typeof(InvalidType);
}
