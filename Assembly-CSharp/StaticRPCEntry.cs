using System;

// Token: 0x020003F5 RID: 1013
public class StaticRPCEntry
{
	// Token: 0x060018E4 RID: 6372 RVA: 0x000857AB File Offset: 0x000839AB
	public StaticRPCEntry(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		this.placeholder = placeholder;
		this.code = code;
		this.lookupMethod = lookupMethod;
	}

	// Token: 0x0400223B RID: 8763
	public NetworkSystem.StaticRPCPlaceholder placeholder;

	// Token: 0x0400223C RID: 8764
	public byte code;

	// Token: 0x0400223D RID: 8765
	public NetworkSystem.StaticRPC lookupMethod;
}
