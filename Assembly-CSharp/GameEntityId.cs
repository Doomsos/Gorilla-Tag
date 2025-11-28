using System;

// Token: 0x0200060C RID: 1548
public struct GameEntityId
{
	// Token: 0x0600271E RID: 10014 RVA: 0x000CEA79 File Offset: 0x000CCC79
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000CEA87 File Offset: 0x000CCC87
	public static bool operator ==(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000CEA97 File Offset: 0x000CCC97
	public static bool operator !=(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000CEAAC File Offset: 0x000CCCAC
	public override bool Equals(object obj)
	{
		GameEntityId gameEntityId = (GameEntityId)obj;
		return this.index == gameEntityId.index;
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000CEACE File Offset: 0x000CCCCE
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
