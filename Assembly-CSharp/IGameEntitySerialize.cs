using System;
using System.IO;

// Token: 0x02000604 RID: 1540
public interface IGameEntitySerialize
{
	// Token: 0x060026D1 RID: 9937
	void OnGameEntitySerialize(BinaryWriter writer);

	// Token: 0x060026D2 RID: 9938
	void OnGameEntityDeserialize(BinaryReader reader);
}
