using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005B5 RID: 1461
public class ConditionalTrigger : MonoBehaviour, IRigAware
{
	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x060024CF RID: 9423 RVA: 0x000C6520 File Offset: 0x000C4720
	private int intValue
	{
		get
		{
			return (int)this._tracking;
		}
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000C6528 File Offset: 0x000C4728
	public void SetProximityFromRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._from = this._rig.transform;
		}
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000C6566 File Offset: 0x000C4766
	public void SetProximityToRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._to = this._rig.transform;
		}
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000C65A4 File Offset: 0x000C47A4
	public void SetProximityFrom(Transform from)
	{
		this._from = from;
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x000C65AD File Offset: 0x000C47AD
	public void SetProxmityTo(Transform to)
	{
		this._to = to;
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000C65B6 File Offset: 0x000C47B6
	public void TrackedSet(TriggerCondition conditions)
	{
		this._tracking = conditions;
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000C65BF File Offset: 0x000C47BF
	public void TrackedAdd(TriggerCondition conditions)
	{
		this._tracking |= conditions;
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x000C65CF File Offset: 0x000C47CF
	public void TrackedRemove(TriggerCondition conditions)
	{
		this._tracking &= ~conditions;
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000C65B6 File Offset: 0x000C47B6
	public void TrackedSet(int conditions)
	{
		this._tracking = (TriggerCondition)conditions;
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000C65BF File Offset: 0x000C47BF
	public void TrackedAdd(int conditions)
	{
		this._tracking |= (TriggerCondition)conditions;
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000C65CF File Offset: 0x000C47CF
	public void TrackedRemove(int conditions)
	{
		this._tracking &= (TriggerCondition)(~(TriggerCondition)conditions);
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x000C65E0 File Offset: 0x000C47E0
	public void TrackedClear()
	{
		this._tracking = TriggerCondition.None;
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000C65E9 File Offset: 0x000C47E9
	private void OnEnable()
	{
		this._timeSince = 0f;
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000C65FB File Offset: 0x000C47FB
	private void Update()
	{
		if (this.IsTracking(TriggerCondition.TimeElapsed))
		{
			this.TrackTimeElapsed();
		}
		if (this.IsTracking(TriggerCondition.Proximity))
		{
			this.TrackProximity();
			return;
		}
		this._distance = 0f;
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000C6627 File Offset: 0x000C4827
	private void TrackTimeElapsed()
	{
		if (this._timeSince.HasElapsed(this._interval, true))
		{
			UnityEvent unityEvent = this.onTimeElapsed;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000C6650 File Offset: 0x000C4850
	private void TrackProximity()
	{
		if (!this._from || !this._to)
		{
			this._distance = 0f;
			return;
		}
		this._distance = Vector3.Distance(this._to.position, this._from.position);
		if (this._distance >= this._maxDistance)
		{
			UnityEvent unityEvent = this.onMaxDistance;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000C66C2 File Offset: 0x000C48C2
	private bool IsTracking(TriggerCondition condition)
	{
		return (this._tracking & condition) == condition;
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000C66CF File Offset: 0x000C48CF
	private static void FindRig(out VRRig rig)
	{
		if (PhotonNetwork.InRoom)
		{
			rig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.Get(PhotonNetwork.LocalPlayer));
			return;
		}
		rig = VRRig.LocalRig;
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x000C66F1 File Offset: 0x000C48F1
	public void SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x0400306B RID: 12395
	[Space]
	[SerializeField]
	private TriggerCondition _tracking;

	// Token: 0x0400306C RID: 12396
	[Space]
	[SerializeField]
	private Transform _from;

	// Token: 0x0400306D RID: 12397
	[SerializeField]
	private Transform _to;

	// Token: 0x0400306E RID: 12398
	[SerializeField]
	private float _maxDistance;

	// Token: 0x0400306F RID: 12399
	[NonSerialized]
	private float _distance;

	// Token: 0x04003070 RID: 12400
	[Space]
	public UnityEvent onMaxDistance;

	// Token: 0x04003071 RID: 12401
	[SerializeField]
	private float _interval = 1f;

	// Token: 0x04003072 RID: 12402
	[NonSerialized]
	private TimeSince _timeSince;

	// Token: 0x04003073 RID: 12403
	[Space]
	public UnityEvent onTimeElapsed;

	// Token: 0x04003074 RID: 12404
	[Space]
	private VRRig _rig;
}
