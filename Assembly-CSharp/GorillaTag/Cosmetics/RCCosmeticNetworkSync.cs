using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001099 RID: 4249
	public class RCCosmeticNetworkSync : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
	{
		// Token: 0x06006A4B RID: 27211 RVA: 0x0022C170 File Offset: 0x0022A370
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			if (info.Sender == null)
			{
				this.DestroyThis();
				return;
			}
			if (info.Sender != base.photonView.Owner || base.photonView.IsRoomView)
			{
				GorillaNot.instance.SendReport("spoofed rc instantiate", info.Sender.UserId, info.Sender.NickName);
				this.DestroyThis();
				return;
			}
			object[] instantiationData = info.photonView.InstantiationData;
			if (instantiationData != null && instantiationData.Length >= 1)
			{
				object obj = instantiationData[0];
				if (obj is int)
				{
					int num = (int)obj;
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer) && num > -1 && num < rigContainer.Rig.myBodyDockPositions.allObjects.Length)
					{
						this.rcRemote = (rigContainer.Rig.myBodyDockPositions.allObjects[num] as RCRemoteHoldable);
						if (this.rcRemote != null)
						{
							this.rcRemote.networkSync = this;
							this.rcRemote.WakeUpRemoteVehicle();
						}
					}
					if (this.rcRemote == null)
					{
						this.DestroyThis();
					}
					return;
				}
			}
			this.DestroyThis();
		}

		// Token: 0x06006A4C RID: 27212 RVA: 0x0022C2A0 File Offset: 0x0022A4A0
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != base.photonView.Owner)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.syncedState.state);
				stream.SendNext(this.syncedState.position);
				stream.SendNext((int)BitPackUtils.PackRotation(this.syncedState.rotation, true));
				stream.SendNext(this.syncedState.dataA);
				stream.SendNext(this.syncedState.dataB);
				stream.SendNext(this.syncedState.dataC);
				return;
			}
			if (stream.IsReading)
			{
				byte state = this.syncedState.state;
				this.syncedState.state = (byte)stream.ReceiveNext();
				Vector3 vector = (Vector3)stream.ReceiveNext();
				ref this.syncedState.position.SetValueSafe(vector);
				Quaternion quaternion = BitPackUtils.UnpackRotation((uint)((int)stream.ReceiveNext()));
				ref this.syncedState.rotation.SetValueSafe(quaternion);
				this.syncedState.dataA = (byte)stream.ReceiveNext();
				this.syncedState.dataB = (byte)stream.ReceiveNext();
				this.syncedState.dataC = (byte)stream.ReceiveNext();
				if (state != this.syncedState.state && this.rcRemote != null && this.rcRemote.Vehicle != null && !this.rcRemote.Vehicle.enabled)
				{
					this.rcRemote.WakeUpRemoteVehicle();
				}
			}
		}

		// Token: 0x06006A4D RID: 27213 RVA: 0x0022C454 File Offset: 0x0022A654
		[PunRPC]
		public void HitRCVehicleRPC(Vector3 hitVelocity, bool isProjectile, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "HitRCVehicleRPC");
			float num = 10000f;
			if (!hitVelocity.IsValid(num))
			{
				GorillaNot.instance.SendReport("nan rc hit", info.Sender.UserId, info.Sender.NickName);
				return;
			}
			if (this.rcRemote != null && this.rcRemote.Vehicle != null)
			{
				this.rcRemote.Vehicle.AuthorityApplyImpact(hitVelocity, isProjectile);
			}
		}

		// Token: 0x06006A4E RID: 27214 RVA: 0x0022C4D8 File Offset: 0x0022A6D8
		private void DestroyThis()
		{
			if (base.photonView.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x04007A05 RID: 31237
		public RCCosmeticNetworkSync.SyncedState syncedState;

		// Token: 0x04007A06 RID: 31238
		private RCRemoteHoldable rcRemote;

		// Token: 0x0200109A RID: 4250
		public struct SyncedState
		{
			// Token: 0x04007A07 RID: 31239
			public byte state;

			// Token: 0x04007A08 RID: 31240
			public Vector3 position;

			// Token: 0x04007A09 RID: 31241
			public Quaternion rotation;

			// Token: 0x04007A0A RID: 31242
			public byte dataA;

			// Token: 0x04007A0B RID: 31243
			public byte dataB;

			// Token: 0x04007A0C RID: 31244
			public byte dataC;
		}
	}
}
