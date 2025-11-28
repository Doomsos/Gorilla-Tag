using System;
using System.IO;
using UnityEngine;

// Token: 0x02000581 RID: 1409
[Serializable]
public struct SnapBounds
{
	// Token: 0x06002372 RID: 9074 RVA: 0x000B999A File Offset: 0x000B7B9A
	public SnapBounds(Vector2Int min, Vector2Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000B99AA File Offset: 0x000B7BAA
	public SnapBounds(int minX, int minY, int maxX, int maxY)
	{
		this.min = new Vector2Int(minX, minY);
		this.max = new Vector2Int(maxX, maxY);
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x000B99C7 File Offset: 0x000B7BC7
	public void Clear()
	{
		this.min = new Vector2Int(int.MinValue, int.MinValue);
		this.max = new Vector2Int(int.MinValue, int.MinValue);
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000B99F4 File Offset: 0x000B7BF4
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.min.x);
		writer.Write(this.min.y);
		writer.Write(this.max.x);
		writer.Write(this.max.y);
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000B9A48 File Offset: 0x000B7C48
	public void Read(BinaryReader reader)
	{
		this.min.x = reader.ReadInt32();
		this.min.y = reader.ReadInt32();
		this.max.x = reader.ReadInt32();
		this.max.y = reader.ReadInt32();
	}

	// Token: 0x04002E43 RID: 11843
	public Vector2Int min;

	// Token: 0x04002E44 RID: 11844
	public Vector2Int max;
}
