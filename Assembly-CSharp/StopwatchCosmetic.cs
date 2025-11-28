using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020004ED RID: 1261
public class StopwatchCosmetic : TransferrableObject
{
	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002064 RID: 8292 RVA: 0x000ABD77 File Offset: 0x000A9F77
	public bool isActivating
	{
		get
		{
			return this._isActivating;
		}
	}

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06002065 RID: 8293 RVA: 0x000ABD7F File Offset: 0x000A9F7F
	public float activeTimeElapsed
	{
		get
		{
			return this._activeTimeElapsed;
		}
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000ABD88 File Offset: 0x000A9F88
	protected override void Awake()
	{
		base.Awake();
		if (StopwatchCosmetic.gWatchToggleRPC == null)
		{
			StopwatchCosmetic.gWatchToggleRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchToggle"));
		}
		if (StopwatchCosmetic.gWatchResetRPC == null)
		{
			StopwatchCosmetic.gWatchResetRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchReset"));
		}
		this._watchToggle = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchReset);
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000ABE0C File Offset: 0x000AA00C
	internal override void OnEnable()
	{
		base.OnEnable();
		int i;
		if (!this.FetchMyViewID(out i))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = i.GetStaticHash();
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000ABE67 File Offset: 0x000AA067
	internal override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000ABE9C File Offset: 0x000AA09C
	private void OnWatchToggle(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchToggle");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int millis = (int)args[2];
		this._watchFace.SetMillisElapsed(millis, true);
		this._watchFace.WatchToggle();
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000ABF1C File Offset: 0x000AA11C
	private void OnWatchReset(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchReset");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		this._watchFace.WatchReset();
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000ABF7C File Offset: 0x000AA17C
	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			return false;
		}
		if (rigContainer.Rig.netView == null)
		{
			return false;
		}
		viewID = rigContainer.Rig.netView.ViewID;
		return true;
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000AC01B File Offset: 0x000AA21B
	public bool PollActivated()
	{
		if (!this._activated)
		{
			return false;
		}
		this._activated = false;
		return true;
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x000AC030 File Offset: 0x000AA230
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isActivating)
		{
			this._activeTimeElapsed += Time.deltaTime;
		}
		if (this._isActivating && this._activeTimeElapsed > 1f)
		{
			this._isActivating = false;
			this._watchFace.WatchReset(true);
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[]
			{
				this._photonID
			});
		}
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x000AC0A3 File Offset: 0x000AA2A3
	public override void OnActivate()
	{
		if (!this.CanActivate())
		{
			return;
		}
		base.OnActivate();
		if (this.IsMyItem())
		{
			this._activeTimeElapsed = 0f;
			this._isActivating = true;
		}
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000AC0D0 File Offset: 0x000AA2D0
	public override void OnDeactivate()
	{
		if (!this.CanDeactivate())
		{
			return;
		}
		base.OnDeactivate();
		if (!this.IsMyItem())
		{
			return;
		}
		this._isActivating = false;
		this._activated = true;
		this._watchFace.WatchToggle();
		StopwatchCosmetic.gWatchToggleRPC.RaiseOthers(new object[]
		{
			this._photonID,
			this._watchFace.watchActive,
			this._watchFace.millisElapsed
		});
		this._activated = false;
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000AC159 File Offset: 0x000AA359
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000AC164 File Offset: 0x000AA364
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002AEC RID: 10988
	[SerializeField]
	private StopwatchFace _watchFace;

	// Token: 0x04002AED RID: 10989
	[Space]
	[NonSerialized]
	private bool _isActivating;

	// Token: 0x04002AEE RID: 10990
	[NonSerialized]
	private float _activeTimeElapsed;

	// Token: 0x04002AEF RID: 10991
	[NonSerialized]
	private bool _activated;

	// Token: 0x04002AF0 RID: 10992
	[Space]
	[NonSerialized]
	private int _photonID = -1;

	// Token: 0x04002AF1 RID: 10993
	private static PhotonEvent gWatchToggleRPC;

	// Token: 0x04002AF2 RID: 10994
	private static PhotonEvent gWatchResetRPC;

	// Token: 0x04002AF3 RID: 10995
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchToggle;

	// Token: 0x04002AF4 RID: 10996
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchReset;

	// Token: 0x04002AF5 RID: 10997
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002AF6 RID: 10998
	[DebugOption]
	public bool disableDeactivation;
}
