using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000D91 RID: 3473
	[RequireComponent(typeof(Rigidbody))]
	public class RigOwnedRigidbodyView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x06005536 RID: 21814 RVA: 0x001ACFB4 File Offset: 0x001AB1B4
		// (set) Token: 0x06005537 RID: 21815 RVA: 0x001ACFBC File Offset: 0x001AB1BC
		public bool IsMine { get; private set; }

		// Token: 0x06005538 RID: 21816 RVA: 0x001ACFC5 File Offset: 0x001AB1C5
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06005539 RID: 21817 RVA: 0x001ACFCE File Offset: 0x001AB1CE
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody>();
			this.m_NetworkPosition = default(Vector3);
			this.m_NetworkRotation = default(Quaternion);
		}

		// Token: 0x0600553A RID: 21818 RVA: 0x001ACFF4 File Offset: 0x001AB1F4
		public void FixedUpdate()
		{
			if (!this.IsMine)
			{
				this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
			}
		}

		// Token: 0x0600553B RID: 21819 RVA: 0x001AD074 File Offset: 0x001AB274
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			try
			{
				if (stream.IsWriting)
				{
					stream.SendNext(this.m_Body.position);
					stream.SendNext(this.m_Body.rotation);
					if (this.m_SynchronizeVelocity)
					{
						stream.SendNext(this.m_Body.linearVelocity);
					}
					if (this.m_SynchronizeAngularVelocity)
					{
						stream.SendNext(this.m_Body.angularVelocity);
					}
					stream.SendNext(this.m_Body.IsSleeping());
				}
				else
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					ref this.m_NetworkPosition.SetValueSafe(vector);
					Quaternion quaternion = (Quaternion)stream.ReceiveNext();
					ref this.m_NetworkRotation.SetValueSafe(quaternion);
					if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
					{
						this.m_Body.position = this.m_NetworkPosition;
					}
					if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
					{
						float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
						if (this.m_SynchronizeVelocity)
						{
							Vector3 linearVelocity = (Vector3)stream.ReceiveNext();
							float num2 = 10000f;
							if (!linearVelocity.IsValid(num2))
							{
								linearVelocity = Vector3.zero;
							}
							if (!this.m_Body.isKinematic)
							{
								this.m_Body.linearVelocity = linearVelocity;
							}
							this.m_NetworkPosition += this.m_Body.linearVelocity * num;
							this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
						}
						if (this.m_SynchronizeAngularVelocity)
						{
							Vector3 angularVelocity = (Vector3)stream.ReceiveNext();
							float num2 = 10000f;
							if (!angularVelocity.IsValid(num2))
							{
								angularVelocity = Vector3.zero;
							}
							this.m_Body.angularVelocity = angularVelocity;
							this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * num) * this.m_NetworkRotation;
							this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
						}
					}
					if ((bool)stream.ReceiveNext())
					{
						this.m_Body.Sleep();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x04006220 RID: 25120
		private float m_Distance;

		// Token: 0x04006221 RID: 25121
		private float m_Angle;

		// Token: 0x04006222 RID: 25122
		private Rigidbody m_Body;

		// Token: 0x04006223 RID: 25123
		private Vector3 m_NetworkPosition;

		// Token: 0x04006224 RID: 25124
		private Quaternion m_NetworkRotation;

		// Token: 0x04006225 RID: 25125
		public bool m_SynchronizeVelocity = true;

		// Token: 0x04006226 RID: 25126
		public bool m_SynchronizeAngularVelocity;

		// Token: 0x04006227 RID: 25127
		public bool m_TeleportEnabled;

		// Token: 0x04006228 RID: 25128
		public float m_TeleportIfDistanceGreaterThan = 3f;
	}
}
