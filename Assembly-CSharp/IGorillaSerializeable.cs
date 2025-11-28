using System;
using Photon.Pun;

// Token: 0x0200075B RID: 1883
public interface IGorillaSerializeable
{
	// Token: 0x060030C1 RID: 12481
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060030C2 RID: 12482
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
