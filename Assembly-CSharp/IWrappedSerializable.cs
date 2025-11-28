using System;
using Fusion;
using Photon.Pun;

// Token: 0x0200075D RID: 1885
internal interface IWrappedSerializable : INetworkStruct
{
	// Token: 0x060030C6 RID: 12486
	void OnSerializeRead(object newData);

	// Token: 0x060030C7 RID: 12487
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060030C8 RID: 12488
	object OnSerializeWrite();

	// Token: 0x060030C9 RID: 12489
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
