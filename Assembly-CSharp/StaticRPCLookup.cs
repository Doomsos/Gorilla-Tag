using System;
using System.Collections.Generic;

// Token: 0x020003F6 RID: 1014
public class StaticRPCLookup
{
	// Token: 0x060018E5 RID: 6373 RVA: 0x000857C8 File Offset: 0x000839C8
	public void Add(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		int count = this.entries.Count;
		this.entries.Add(new StaticRPCEntry(placeholder, code, lookupMethod));
		this.eventCodeEntryLookup.Add(code, count);
		this.placeholderEntryLookup.Add(placeholder, count);
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x0008580E File Offset: 0x00083A0E
	public NetworkSystem.StaticRPC CodeToMethod(byte code)
	{
		return this.entries[this.eventCodeEntryLookup[code]].lookupMethod;
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x0008582C File Offset: 0x00083A2C
	public byte PlaceholderToCode(NetworkSystem.StaticRPCPlaceholder placeholder)
	{
		return this.entries[this.placeholderEntryLookup[placeholder]].code;
	}

	// Token: 0x0400223E RID: 8766
	public List<StaticRPCEntry> entries = new List<StaticRPCEntry>();

	// Token: 0x0400223F RID: 8767
	private Dictionary<byte, int> eventCodeEntryLookup = new Dictionary<byte, int>();

	// Token: 0x04002240 RID: 8768
	private Dictionary<NetworkSystem.StaticRPCPlaceholder, int> placeholderEntryLookup = new Dictionary<NetworkSystem.StaticRPCPlaceholder, int>();
}
