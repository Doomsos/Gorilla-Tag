using System;

// Token: 0x0200053E RID: 1342
public struct GameBallId
{
	// Token: 0x060021C8 RID: 8648 RVA: 0x000B06D9 File Offset: 0x000AE8D9
	public GameBallId(int index)
	{
		this.index = index;
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000B06E2 File Offset: 0x000AE8E2
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000B06F0 File Offset: 0x000AE8F0
	public static bool operator ==(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000B0700 File Offset: 0x000AE900
	public static bool operator !=(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000B0714 File Offset: 0x000AE914
	public override bool Equals(object obj)
	{
		GameBallId gameBallId = (GameBallId)obj;
		return this.index == gameBallId.index;
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000B0736 File Offset: 0x000AE936
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x04002C7E RID: 11390
	public static GameBallId Invalid = new GameBallId(-1);

	// Token: 0x04002C7F RID: 11391
	public int index;
}
