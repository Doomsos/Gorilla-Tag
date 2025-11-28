using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020007F5 RID: 2037
public class GTSignalListener : MonoBehaviour
{
	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06003587 RID: 13703 RVA: 0x0012254F File Offset: 0x0012074F
	// (set) Token: 0x06003588 RID: 13704 RVA: 0x00122557 File Offset: 0x00120757
	public int rigActorID { get; private set; } = -1;

	// Token: 0x06003589 RID: 13705 RVA: 0x00122560 File Offset: 0x00120760
	private void Awake()
	{
		this.OnListenerAwake();
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x00122568 File Offset: 0x00120768
	private void OnEnable()
	{
		this.RefreshActorID();
		this.OnListenerEnable();
		GTSignalRelay.Register(this);
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x0012257C File Offset: 0x0012077C
	private void OnDisable()
	{
		GTSignalRelay.Unregister(this);
		this.OnListenerDisable();
	}

	// Token: 0x0600358C RID: 13708 RVA: 0x0012258A File Offset: 0x0012078A
	private void RefreshActorID()
	{
		this.rig = base.GetComponentInParent<VRRig>(true);
		int rigActorID;
		if (!(this.rig == null))
		{
			NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
			rigActorID = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : -1);
		}
		else
		{
			rigActorID = -1;
		}
		this.rigActorID = rigActorID;
	}

	// Token: 0x0600358D RID: 13709 RVA: 0x001225C7 File Offset: 0x001207C7
	public virtual bool IsReady()
	{
		return this._callLimits.CheckCallTime(Time.time);
	}

	// Token: 0x0600358E RID: 13710 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnListenerAwake()
	{
	}

	// Token: 0x0600358F RID: 13711 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnListenerEnable()
	{
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnListenerDisable()
	{
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void HandleSignalReceived(int sender, object[] args)
	{
	}

	// Token: 0x040044AF RID: 17583
	[Space]
	public GTSignalID signal;

	// Token: 0x040044B0 RID: 17584
	[Space]
	public VRRig rig;

	// Token: 0x040044B2 RID: 17586
	[Space]
	public bool deafen;

	// Token: 0x040044B3 RID: 17587
	[FormerlySerializedAs("listenToRigOnly")]
	public bool listenToSelfOnly;

	// Token: 0x040044B4 RID: 17588
	public bool ignoreSelf;

	// Token: 0x040044B5 RID: 17589
	[Space]
	public bool callUnityEvent = true;

	// Token: 0x040044B6 RID: 17590
	[Space]
	[SerializeField]
	private CallLimiter _callLimits = new CallLimiter(10, 0.25f, 0.5f);

	// Token: 0x040044B7 RID: 17591
	[Space]
	public UnityEvent onSignalReceived;
}
