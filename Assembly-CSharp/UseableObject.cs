using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004F7 RID: 1271
[RequireComponent(typeof(UseableObjectEvents))]
public class UseableObject : TransferrableObject
{
	// Token: 0x17000371 RID: 881
	// (get) Token: 0x060020AD RID: 8365 RVA: 0x000AD773 File Offset: 0x000AB973
	public bool isMidUse
	{
		get
		{
			return this._isMidUse;
		}
	}

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x060020AE RID: 8366 RVA: 0x000AD77B File Offset: 0x000AB97B
	public float useTimeElapsed
	{
		get
		{
			return this._useTimeElapsed;
		}
	}

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x060020AF RID: 8367 RVA: 0x000AD783 File Offset: 0x000AB983
	public bool justUsed
	{
		get
		{
			if (!this._justUsed)
			{
				return false;
			}
			this._justUsed = false;
			return true;
		}
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x000AD797 File Offset: 0x000AB997
	protected override void Awake()
	{
		base.Awake();
		this._events = base.gameObject.GetOrAddComponent<UseableObjectEvents>();
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000AD7B0 File Offset: 0x000AB9B0
	internal override void OnEnable()
	{
		base.OnEnable();
		UseableObjectEvents events = this._events;
		VRRig myOnlineRig = base.myOnlineRig;
		NetPlayer player;
		if ((player = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
		{
			VRRig myRig = base.myRig;
			player = ((myRig != null) ? myRig.creator : null);
		}
		events.Init(player);
		this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnObjectActivated);
		this._events.Deactivate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnObjectDeactivated);
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x000AD83A File Offset: 0x000ABA3A
	internal override void OnDisable()
	{
		base.OnDisable();
		Object.Destroy(this._events);
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000AD84D File Offset: 0x000ABA4D
	private void OnObjectActivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000AD84D File Offset: 0x000ABA4D
	private void OnObjectDeactivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000AD853 File Offset: 0x000ABA53
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isMidUse)
		{
			this._useTimeElapsed += Time.deltaTime;
		}
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000AD878 File Offset: 0x000ABA78
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onActivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._useTimeElapsed = 0f;
			this._isMidUse = true;
		}
		if (this._raiseActivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent activate = events.Activate;
			if (activate == null)
			{
				return;
			}
			activate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000AD8E0 File Offset: 0x000ABAE0
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onDeactivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._isMidUse = false;
			this._justUsed = true;
		}
		if (this._raiseDeactivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent deactivate = events.Deactivate;
			if (deactivate == null)
			{
				return;
			}
			deactivate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000AD941 File Offset: 0x000ABB41
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000AD94C File Offset: 0x000ABB4C
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002B46 RID: 11078
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002B47 RID: 11079
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04002B48 RID: 11080
	[SerializeField]
	private UseableObjectEvents _events;

	// Token: 0x04002B49 RID: 11081
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04002B4A RID: 11082
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04002B4B RID: 11083
	[NonSerialized]
	private DateTime _lastActivate;

	// Token: 0x04002B4C RID: 11084
	[NonSerialized]
	private DateTime _lastDeactivate;

	// Token: 0x04002B4D RID: 11085
	[NonSerialized]
	private bool _isMidUse;

	// Token: 0x04002B4E RID: 11086
	[NonSerialized]
	private float _useTimeElapsed;

	// Token: 0x04002B4F RID: 11087
	[NonSerialized]
	private bool _justUsed;

	// Token: 0x04002B50 RID: 11088
	[NonSerialized]
	private int tempHandPos;

	// Token: 0x04002B51 RID: 11089
	public UnityEvent onActivateLocal;

	// Token: 0x04002B52 RID: 11090
	public UnityEvent onDeactivateLocal;
}
