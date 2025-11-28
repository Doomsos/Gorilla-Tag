using System;
using Fusion;
using Photon.Pun;

[NetworkBehaviourWeaved(0)]
public class NetworkComponentCallbacks : NetworkComponent
{
	public override void ReadDataFusion()
	{
		this.ReadData.Invoke();
	}

	public override void WriteDataFusion()
	{
		this.WriteData.Invoke();
	}

	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.ReadPunData.Invoke(stream, info);
	}

	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.WritePunData.Invoke(stream, info);
	}

	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	public Action ReadData;

	public Action WriteData;

	public Action<PhotonStream, PhotonMessageInfo> ReadPunData;

	public Action<PhotonStream, PhotonMessageInfo> WritePunData;
}
