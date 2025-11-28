using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000755 RID: 1877
[NetworkBehaviourWeaved(15)]
internal class GorillaNetworkTransform : NetworkComponent, ITickSystemTick
{
	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x0600307B RID: 12411 RVA: 0x00109518 File Offset: 0x00107718
	public bool RespectOwnership
	{
		get
		{
			return this.respectOwnership;
		}
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x0600307C RID: 12412 RVA: 0x00109520 File Offset: 0x00107720
	// (set) Token: 0x0600307D RID: 12413 RVA: 0x00109528 File Offset: 0x00107728
	public bool TickRunning { get; set; }

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x0600307E RID: 12414 RVA: 0x00109531 File Offset: 0x00107731
	// (set) Token: 0x0600307F RID: 12415 RVA: 0x0010955B File Offset: 0x0010775B
	[Networked]
	[NetworkedWeaved(0, 15)]
	private unsafe GorillaNetworkTransform.NetTransformData data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x00109588 File Offset: 0x00107788
	public new void Awake()
	{
		this.m_StoredPosition = base.transform.localPosition;
		this.m_NetworkPosition = Vector3.zero;
		this.m_NetworkScale = Vector3.zero;
		this.m_NetworkRotation = Quaternion.identity;
		this.maxDistanceSquare = this.maxDistance * this.maxDistance;
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x001095DC File Offset: 0x001077DC
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.m_firstTake = true;
		if (this.clampToSpawn)
		{
			this.clampOriginPoint = (this.m_UseLocal ? base.transform.localPosition : base.transform.position);
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x0010962A File Offset: 0x0010782A
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06003083 RID: 12419 RVA: 0x00109638 File Offset: 0x00107838
	public void Tick()
	{
		if (!base.IsLocallyOwned)
		{
			if (this.m_UseLocal)
			{
				base.transform.SetLocalPositionAndRotation(Vector3.MoveTowards(base.transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
				return;
			}
			base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
		}
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x00109728 File Offset: 0x00107928
	public override void WriteDataFusion()
	{
		GorillaNetworkTransform.NetTransformData data = this.SharedWrite();
		double sentTime = NetworkSystem.Instance.SimTick / 1000.0;
		data.SentTime = sentTime;
		this.data = data;
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x00109762 File Offset: 0x00107962
	public override void ReadDataFusion()
	{
		this.SharedRead(this.data);
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x00109770 File Offset: 0x00107970
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		if (this.m_SynchronizePosition)
		{
			stream.SendNext(netTransformData.position);
			stream.SendNext(netTransformData.velocity);
		}
		if (this.m_SynchronizeRotation)
		{
			stream.SendNext(netTransformData.rotation);
		}
		if (this.m_SynchronizeScale)
		{
			stream.SendNext(netTransformData.scale);
		}
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x00109804 File Offset: 0x00107A04
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData data = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			data.position = (Vector3)stream.ReceiveNext();
			data.velocity = (Vector3)stream.ReceiveNext();
		}
		if (this.m_SynchronizeRotation)
		{
			data.rotation = (Quaternion)stream.ReceiveNext();
		}
		if (this.m_SynchronizeScale)
		{
			data.scale = (Vector3)stream.ReceiveNext();
		}
		data.SentTime = (double)((float)info.SentServerTime);
		this.SharedRead(data);
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x001098B4 File Offset: 0x00107AB4
	private void SharedRead(GorillaNetworkTransform.NetTransformData data)
	{
		if (this.m_SynchronizePosition)
		{
			ref this.m_NetworkPosition.SetValueSafe(data.position);
			ref this.m_Velocity.SetValueSafe(data.velocity);
			if (this.clampDistanceFromSpawn && Vector3.SqrMagnitude(this.clampOriginPoint - this.m_NetworkPosition) > this.maxDistanceSquare)
			{
				this.m_NetworkPosition = this.clampOriginPoint + this.m_Velocity.normalized * this.maxDistance;
				this.m_Velocity = Vector3.zero;
			}
			if (this.m_firstTake)
			{
				if (this.m_UseLocal)
				{
					base.transform.localPosition = this.m_NetworkPosition;
				}
				else
				{
					base.transform.position = this.m_NetworkPosition;
				}
				this.m_Distance = 0f;
			}
			else
			{
				float num = Mathf.Abs((float)(NetworkSystem.Instance.SimTime - data.SentTime));
				this.m_NetworkPosition += this.m_Velocity * num;
				if (this.m_UseLocal)
				{
					this.m_Distance = Vector3.Distance(base.transform.localPosition, this.m_NetworkPosition);
				}
				else
				{
					this.m_Distance = Vector3.Distance(base.transform.position, this.m_NetworkPosition);
				}
			}
		}
		if (this.m_SynchronizeRotation)
		{
			ref this.m_NetworkRotation.SetValueSafe(data.rotation);
			if (this.m_firstTake)
			{
				this.m_Angle = 0f;
				if (this.m_UseLocal)
				{
					base.transform.localRotation = this.m_NetworkRotation;
				}
				else
				{
					base.transform.rotation = this.m_NetworkRotation;
				}
			}
			else if (this.m_UseLocal)
			{
				this.m_Angle = Quaternion.Angle(base.transform.localRotation, this.m_NetworkRotation);
			}
			else
			{
				this.m_Angle = Quaternion.Angle(base.transform.rotation, this.m_NetworkRotation);
			}
		}
		if (this.m_SynchronizeScale)
		{
			ref this.m_NetworkScale.SetValueSafe(data.scale);
			base.transform.localScale = this.m_NetworkScale;
		}
		if (this.m_firstTake)
		{
			this.m_firstTake = false;
		}
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x00109ADC File Offset: 0x00107CDC
	private GorillaNetworkTransform.NetTransformData SharedWrite()
	{
		GorillaNetworkTransform.NetTransformData result = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			if (this.m_UseLocal)
			{
				this.m_Velocity = base.transform.localPosition - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.localPosition;
				result.position = base.transform.localPosition;
				result.velocity = this.m_Velocity;
			}
			else
			{
				this.m_Velocity = base.transform.position - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.position;
				result.position = base.transform.position;
				result.velocity = this.m_Velocity;
			}
		}
		if (this.m_SynchronizeRotation)
		{
			if (this.m_UseLocal)
			{
				result.rotation = base.transform.localRotation;
			}
			else
			{
				result.rotation = base.transform.rotation;
			}
		}
		if (this.m_SynchronizeScale)
		{
			result.scale = base.transform.localScale;
		}
		return result;
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x00109BEF File Offset: 0x00107DEF
	public void GTAddition_DoTeleport()
	{
		this.m_firstTake = true;
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x00109C27 File Offset: 0x00107E27
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.data = this._data;
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x00109C3F File Offset: 0x00107E3F
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._data = this.data;
	}

	// Token: 0x04003F85 RID: 16261
	[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
	public bool m_UseLocal;

	// Token: 0x04003F86 RID: 16262
	[SerializeField]
	private bool respectOwnership;

	// Token: 0x04003F87 RID: 16263
	[SerializeField]
	private bool clampDistanceFromSpawn = true;

	// Token: 0x04003F88 RID: 16264
	[SerializeField]
	private float maxDistance = 100f;

	// Token: 0x04003F89 RID: 16265
	private float maxDistanceSquare;

	// Token: 0x04003F8A RID: 16266
	[SerializeField]
	private bool clampToSpawn = true;

	// Token: 0x04003F8B RID: 16267
	[Tooltip("Use this if clampToSpawn is false, to set the center point to check the synced position against")]
	[SerializeField]
	private Vector3 clampOriginPoint;

	// Token: 0x04003F8C RID: 16268
	public bool m_SynchronizePosition = true;

	// Token: 0x04003F8D RID: 16269
	public bool m_SynchronizeRotation = true;

	// Token: 0x04003F8E RID: 16270
	public bool m_SynchronizeScale;

	// Token: 0x04003F8F RID: 16271
	private float m_Distance;

	// Token: 0x04003F90 RID: 16272
	private float m_Angle;

	// Token: 0x04003F91 RID: 16273
	private Vector3 m_Velocity;

	// Token: 0x04003F92 RID: 16274
	private Vector3 m_NetworkPosition;

	// Token: 0x04003F93 RID: 16275
	private Vector3 m_StoredPosition;

	// Token: 0x04003F94 RID: 16276
	private Vector3 m_NetworkScale;

	// Token: 0x04003F95 RID: 16277
	private Quaternion m_NetworkRotation;

	// Token: 0x04003F96 RID: 16278
	private bool m_firstTake;

	// Token: 0x04003F98 RID: 16280
	[WeaverGenerated]
	[DefaultForProperty("data", 0, 15)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private GorillaNetworkTransform.NetTransformData _data;

	// Token: 0x02000756 RID: 1878
	[NetworkStructWeaved(15)]
	[StructLayout(2, Size = 60)]
	private struct NetTransformData : INetworkStruct
	{
		// Token: 0x04003F99 RID: 16281
		[FieldOffset(0)]
		public Vector3 position;

		// Token: 0x04003F9A RID: 16282
		[FieldOffset(12)]
		public Vector3 velocity;

		// Token: 0x04003F9B RID: 16283
		[FieldOffset(24)]
		public Quaternion rotation;

		// Token: 0x04003F9C RID: 16284
		[FieldOffset(40)]
		public Vector3 scale;

		// Token: 0x04003F9D RID: 16285
		[FieldOffset(52)]
		public double SentTime;
	}
}
