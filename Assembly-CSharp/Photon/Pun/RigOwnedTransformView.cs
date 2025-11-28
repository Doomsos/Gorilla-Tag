using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000D92 RID: 3474
	[HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
	public class RigOwnedTransformView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x0600553D RID: 21821 RVA: 0x001AD30A File Offset: 0x001AB50A
		// (set) Token: 0x0600553E RID: 21822 RVA: 0x001AD312 File Offset: 0x001AB512
		public bool IsMine { get; private set; }

		// Token: 0x0600553F RID: 21823 RVA: 0x001AD31B File Offset: 0x001AB51B
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06005540 RID: 21824 RVA: 0x001AD324 File Offset: 0x001AB524
		public void Awake()
		{
			this.m_StoredPosition = base.transform.localPosition;
			this.m_NetworkPosition = Vector3.zero;
			this.m_networkScale = Vector3.one;
			this.m_NetworkRotation = Quaternion.identity;
		}

		// Token: 0x06005541 RID: 21825 RVA: 0x001AD358 File Offset: 0x001AB558
		private void Reset()
		{
			this.m_UseLocal = true;
		}

		// Token: 0x06005542 RID: 21826 RVA: 0x001AD361 File Offset: 0x001AB561
		private void OnEnable()
		{
			this.m_firstTake = true;
		}

		// Token: 0x06005543 RID: 21827 RVA: 0x001AD36C File Offset: 0x001AB56C
		public void Update()
		{
			Transform transform = base.transform;
			if (!this.IsMine && this.IsValid(this.m_NetworkPosition) && this.IsValid(this.m_NetworkRotation))
			{
				if (this.m_UseLocal)
				{
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					return;
				}
				transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
			}
		}

		// Token: 0x06005544 RID: 21828 RVA: 0x001AD460 File Offset: 0x001AB660
		private bool IsValid(Vector3 v)
		{
			return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
		}

		// Token: 0x06005545 RID: 21829 RVA: 0x001AD4C0 File Offset: 0x001AB6C0
		private bool IsValid(Quaternion q)
		{
			return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) && !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
		}

		// Token: 0x06005546 RID: 21830 RVA: 0x001AD538 File Offset: 0x001AB738
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			try
			{
				Transform transform = base.transform;
				if (stream.IsWriting)
				{
					if (this.m_SynchronizePosition)
					{
						if (this.m_UseLocal)
						{
							this.m_Direction = transform.localPosition - this.m_StoredPosition;
							this.m_StoredPosition = transform.localPosition;
							stream.SendNext(transform.localPosition);
							stream.SendNext(this.m_Direction);
						}
						else
						{
							this.m_Direction = transform.position - this.m_StoredPosition;
							this.m_StoredPosition = transform.position;
							stream.SendNext(transform.position);
							stream.SendNext(this.m_Direction);
						}
					}
					if (this.m_SynchronizeRotation)
					{
						if (this.m_UseLocal)
						{
							stream.SendNext(transform.localRotation);
						}
						else
						{
							stream.SendNext(transform.rotation);
						}
					}
					if (this.m_SynchronizeScale)
					{
						stream.SendNext(transform.localScale);
					}
				}
				else
				{
					if (this.m_SynchronizePosition)
					{
						Vector3 vector = (Vector3)stream.ReceiveNext();
						ref this.m_NetworkPosition.SetValueSafe(vector);
						vector = (Vector3)stream.ReceiveNext();
						ref this.m_Direction.SetValueSafe(vector);
						if (this.m_firstTake)
						{
							if (this.m_UseLocal)
							{
								transform.localPosition = this.m_NetworkPosition;
							}
							else
							{
								transform.position = this.m_NetworkPosition;
							}
							this.m_Distance = 0f;
						}
						else
						{
							float num = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
							this.m_NetworkPosition += this.m_Direction * num;
							if (this.m_UseLocal)
							{
								this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
							}
							else
							{
								this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
							}
						}
					}
					if (this.m_SynchronizeRotation)
					{
						Quaternion quaternion = (Quaternion)stream.ReceiveNext();
						ref this.m_NetworkRotation.SetValueSafe(quaternion);
						if (this.m_firstTake)
						{
							this.m_Angle = 0f;
							if (this.m_UseLocal)
							{
								transform.localRotation = this.m_NetworkRotation;
							}
							else
							{
								transform.rotation = this.m_NetworkRotation;
							}
						}
						else if (this.m_UseLocal)
						{
							this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
						}
						else
						{
							this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
						}
					}
					if (this.m_SynchronizeScale)
					{
						Vector3 vector = (Vector3)stream.ReceiveNext();
						ref this.m_networkScale.SetValueSafe(vector);
						transform.localScale = this.m_networkScale;
					}
					if (this.m_firstTake)
					{
						this.m_firstTake = false;
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06005547 RID: 21831 RVA: 0x001AD361 File Offset: 0x001AB561
		public void GTAddition_DoTeleport()
		{
			this.m_firstTake = true;
		}

		// Token: 0x0400622A RID: 25130
		private float m_Distance;

		// Token: 0x0400622B RID: 25131
		private float m_Angle;

		// Token: 0x0400622C RID: 25132
		private Vector3 m_Direction;

		// Token: 0x0400622D RID: 25133
		private Vector3 m_NetworkPosition;

		// Token: 0x0400622E RID: 25134
		private Vector3 m_StoredPosition;

		// Token: 0x0400622F RID: 25135
		private Vector3 m_networkScale;

		// Token: 0x04006230 RID: 25136
		private Quaternion m_NetworkRotation;

		// Token: 0x04006231 RID: 25137
		public bool m_SynchronizePosition = true;

		// Token: 0x04006232 RID: 25138
		public bool m_SynchronizeRotation = true;

		// Token: 0x04006233 RID: 25139
		public bool m_SynchronizeScale;

		// Token: 0x04006234 RID: 25140
		[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
		public bool m_UseLocal;

		// Token: 0x04006235 RID: 25141
		private bool m_firstTake;
	}
}
