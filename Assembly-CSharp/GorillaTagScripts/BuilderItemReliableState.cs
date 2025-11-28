using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB5 RID: 3509
	public class BuilderItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06005661 RID: 22113 RVA: 0x001B29D4 File Offset: 0x001B0BD4
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.rightHandAttachPos);
				stream.SendNext(this.rightHandAttachRot);
				stream.SendNext(this.leftHandAttachPos);
				stream.SendNext(this.leftHandAttachRot);
				return;
			}
			this.rightHandAttachPos = (Vector3)stream.ReceiveNext();
			this.rightHandAttachRot = (Quaternion)stream.ReceiveNext();
			this.leftHandAttachPos = (Vector3)stream.ReceiveNext();
			this.leftHandAttachRot = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!this.rightHandAttachPos.IsValid(num))
			{
				this.rightHandAttachPos = Vector3.zero;
			}
			if (!this.rightHandAttachRot.IsValid())
			{
				this.rightHandAttachRot = quaternion.identity;
			}
			num = 10000f;
			if (!this.leftHandAttachPos.IsValid(num))
			{
				this.leftHandAttachPos = Vector3.zero;
			}
			if (!this.leftHandAttachRot.IsValid())
			{
				this.leftHandAttachRot = quaternion.identity;
			}
			this.dirty = true;
		}

		// Token: 0x04006386 RID: 25478
		public Vector3 rightHandAttachPos = Vector3.zero;

		// Token: 0x04006387 RID: 25479
		public Quaternion rightHandAttachRot = Quaternion.identity;

		// Token: 0x04006388 RID: 25480
		public Vector3 leftHandAttachPos = Vector3.zero;

		// Token: 0x04006389 RID: 25481
		public Quaternion leftHandAttachRot = Quaternion.identity;

		// Token: 0x0400638A RID: 25482
		public bool dirty;
	}
}
