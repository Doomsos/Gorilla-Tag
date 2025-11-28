using System;

// Token: 0x0200075C RID: 1884
internal interface IGorillaSerializeableScene : IGorillaSerializeable
{
	// Token: 0x060030C3 RID: 12483
	void OnSceneLinking(GorillaSerializerScene serializer);

	// Token: 0x060030C4 RID: 12484
	void OnNetworkObjectDisable();

	// Token: 0x060030C5 RID: 12485
	void OnNetworkObjectEnable();
}
