using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DDB RID: 3547
	public class DecorativeItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06005814 RID: 22548 RVA: 0x001C23A8 File Offset: 0x001C05A8
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.isSnapped);
				stream.SendNext(this.snapPosition);
				stream.SendNext(this.respawnPosition);
				stream.SendNext(this.respawnRotation);
				return;
			}
			this.isSnapped = (bool)stream.ReceiveNext();
			this.snapPosition = (Vector3)stream.ReceiveNext();
			this.respawnPosition = (Vector3)stream.ReceiveNext();
			this.respawnRotation = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!this.snapPosition.IsValid(num))
			{
				this.snapPosition = Vector3.zero;
			}
			num = 10000f;
			if (!this.respawnPosition.IsValid(num))
			{
				this.respawnPosition = Vector3.zero;
			}
			if (!this.respawnRotation.IsValid())
			{
				this.respawnRotation = quaternion.identity;
			}
		}

		// Token: 0x04006565 RID: 25957
		public bool isSnapped;

		// Token: 0x04006566 RID: 25958
		public Vector3 snapPosition = Vector3.zero;

		// Token: 0x04006567 RID: 25959
		public Vector3 respawnPosition = Vector3.zero;

		// Token: 0x04006568 RID: 25960
		public Quaternion respawnRotation = Quaternion.identity;
	}
}
