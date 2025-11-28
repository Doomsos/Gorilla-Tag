using System;

// Token: 0x02000C68 RID: 3176
public struct EnterPlayID
{
	// Token: 0x06004DAE RID: 19886 RVA: 0x00192437 File Offset: 0x00190637
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	// Token: 0x06004DAF RID: 19887 RVA: 0x00192448 File Offset: 0x00190648
	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	// Token: 0x1700073F RID: 1855
	// (get) Token: 0x06004DB0 RID: 19888 RVA: 0x0019246A File Offset: 0x0019066A
	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Token: 0x04005CF2 RID: 23794
	private static int currentID = 1;

	// Token: 0x04005CF3 RID: 23795
	private int id;
}
