using System;
using Photon.Realtime;

// Token: 0x020003D9 RID: 985
public class NetEventOptions
{
	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06001816 RID: 6166 RVA: 0x000819AF File Offset: 0x0007FBAF
	public bool HasWebHooks
	{
		get
		{
			return this.Flags != WebFlags.Default;
		}
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x000819C1 File Offset: 0x0007FBC1
	public NetEventOptions()
	{
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x000819D4 File Offset: 0x0007FBD4
	public NetEventOptions(int reciever, int[] actors, byte flags)
	{
		this.Reciever = (NetEventOptions.RecieverTarget)reciever;
		this.TargetActors = actors;
		this.Flags = new WebFlags(flags);
	}

	// Token: 0x0400218A RID: 8586
	public NetEventOptions.RecieverTarget Reciever;

	// Token: 0x0400218B RID: 8587
	public int[] TargetActors;

	// Token: 0x0400218C RID: 8588
	public WebFlags Flags = WebFlags.Default;

	// Token: 0x020003DA RID: 986
	public enum RecieverTarget
	{
		// Token: 0x0400218E RID: 8590
		others,
		// Token: 0x0400218F RID: 8591
		all,
		// Token: 0x04002190 RID: 8592
		master
	}
}
