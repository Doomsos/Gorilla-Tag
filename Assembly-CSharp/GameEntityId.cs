using System;

// Token: 0x0200060C RID: 1548
public struct GameEntityId
{
	// Token: 0x0600271E RID: 10014 RVA: 0x000CEA99 File Offset: 0x000CCC99
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000CEAA7 File Offset: 0x000CCCA7
	public static bool operator ==(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000CEAB7 File Offset: 0x000CCCB7
	public static bool operator !=(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000CEACC File Offset: 0x000CCCCC
	public override bool Equals(object obj)
	{
		GameEntityId gameEntityId = (GameEntityId)obj;
		return this.index == gameEntityId.index;
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000CEAEE File Offset: 0x000CCCEE
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x040032CE RID: 13006
	public static GameEntityId Invalid = new GameEntityId
	{
		index = -1
	};

	// Token: 0x040032CF RID: 13007
	public int index;
}
