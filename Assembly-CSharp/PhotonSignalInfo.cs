using System;
using Photon.Pun;

// Token: 0x02000BBE RID: 3006
[Serializable]
public struct PhotonSignalInfo
{
	// Token: 0x06004A31 RID: 18993 RVA: 0x00185763 File Offset: 0x00183963
	public PhotonSignalInfo(NetPlayer sender, int timestamp)
	{
		this.sender = sender;
		this.timestamp = timestamp;
	}

	// Token: 0x170006E2 RID: 1762
	// (get) Token: 0x06004A32 RID: 18994 RVA: 0x00185773 File Offset: 0x00183973
	public double sentServerTime
	{
		get
		{
			return this.timestamp / 1000.0;
		}
	}

	// Token: 0x06004A33 RID: 18995 RVA: 0x00185787 File Offset: 0x00183987
	public override string ToString()
	{
		return string.Format("[{0}: Sender = '{1}' sentTime = {2}]", "PhotonSignalInfo", this.sender.ActorNumber, this.sentServerTime);
	}

	// Token: 0x06004A34 RID: 18996 RVA: 0x001857B3 File Offset: 0x001839B3
	public static implicit operator PhotonMessageInfo(PhotonSignalInfo psi)
	{
		return new PhotonMessageInfo(psi.sender.GetPlayerRef(), psi.timestamp, null);
	}

	// Token: 0x04005ACA RID: 23242
	public readonly int timestamp;

	// Token: 0x04005ACB RID: 23243
	public readonly NetPlayer sender;
}
