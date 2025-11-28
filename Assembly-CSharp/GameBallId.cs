using System;

// Token: 0x0200053E RID: 1342
public struct GameBallId
{
	// Token: 0x060021C8 RID: 8648 RVA: 0x000B06F9 File Offset: 0x000AE8F9
	public GameBallId(int index)
	{
		this.index = index;
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000B0702 File Offset: 0x000AE902
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000B0710 File Offset: 0x000AE910
	public static bool operator ==(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000B0720 File Offset: 0x000AE920
	public static bool operator !=(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000B0734 File Offset: 0x000AE934
	public override bool Equals(object obj)
	{
		GameBallId gameBallId = (GameBallId)obj;
		return this.index == gameBallId.index;
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000B0756 File Offset: 0x000AE956
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x04002C7E RID: 11390
	public static GameBallId Invalid = new GameBallId(-1);

	// Token: 0x04002C7F RID: 11391
	public int index;
}
