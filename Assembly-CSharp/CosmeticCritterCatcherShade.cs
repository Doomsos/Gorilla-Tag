using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class CosmeticCritterCatcherShade : CosmeticCritterCatcher
{
	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000460 RID: 1120 RVA: 0x00019288 File Offset: 0x00017488
	// (set) Token: 0x06000461 RID: 1121 RVA: 0x00019290 File Offset: 0x00017490
	public Vector3 LastTargetPosition { get; private set; }

	// Token: 0x06000462 RID: 1122 RVA: 0x00019299 File Offset: 0x00017499
	public float GetActionTimeFrac()
	{
		return this.targetHoldTime / this.maxHoldTime;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x000192A8 File Offset: 0x000174A8
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 0.25f, 0.5f);
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000192BC File Offset: 0x000174BC
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (this.heartbeatCooldown > 0.5f || (this.currentTarget != null && this.currentTarget != critter))
		{
			return CosmeticCritterAction.None;
		}
		if (critter is CosmeticCritterShadeFleeing && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 0f))
		{
			if (this.targetHoldTime >= this.minSecondsLockedToCatch && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
		else
		{
			if (!(critter is CosmeticCritterShadeHidden) || !this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 0f))
			{
				return CosmeticCritterAction.None;
			}
			if (this.targetHoldTime >= this.secondsToReveal)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00019388 File Offset: 0x00017588
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		if (!base.ValidateRemoteCatchAction(critter, catchAction, serverTime))
		{
			return false;
		}
		if (critter is CosmeticCritterShadeFleeing)
		{
			if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius + 1f && this.targetHoldTime >= this.minSecondsLockedToCatch * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 2f))
			{
				return true;
			}
		}
		else if (critter is CosmeticCritterShadeHidden)
		{
			if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None && this.targetHoldTime >= this.secondsToReveal * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 2f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001945C File Offset: 0x0001765C
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.currentTarget = critter;
		float num = PhotonNetwork.InRoom ? ((float)(PhotonNetwork.Time - serverTime)) : 0f;
		this.heartbeatCooldown = 1f + num;
		this.targetHoldTime += num;
		if (!(critter is CosmeticCritterShadeFleeing))
		{
			if (critter is CosmeticCritterShadeHidden)
			{
				this.maxHoldTime = this.secondsToReveal;
				if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None)
				{
					(this.optionalLinkedSpawner as CosmeticCritterSpawnerShadeFleeing).SetSpawnPosition(critter.transform.position);
					this.currentTarget = null;
					this.targetHoldTime = 0f;
				}
			}
			return;
		}
		this.maxHoldTime = this.minSecondsLockedToCatch;
		if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
		{
			this.shadeRevealer.ShadeCaught();
			this.currentTarget = null;
			this.targetHoldTime = 0f;
			return;
		}
		CosmeticCritterAction cosmeticCritterAction = catchAction & CosmeticCritterAction.ShadeHeartbeat;
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00019526 File Offset: 0x00017726
	protected override void Awake()
	{
		base.Awake();
		this.shadeRevealer = (this.transferrableObject as ShadeRevealer);
		this.maxHoldTime = Mathf.Max(this.secondsToReveal, this.minSecondsLockedToCatch);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00019558 File Offset: 0x00017758
	protected void LateUpdate()
	{
		if (this.heartbeatCooldown > 0f)
		{
			this.heartbeatCooldown -= Time.deltaTime;
			if (this.heartbeatCooldown < 0f)
			{
				this.heartbeatCooldown = 0f;
				this.currentTarget = null;
				return;
			}
			this.targetHoldTime = Mathf.Min(this.targetHoldTime + Time.deltaTime, this.maxHoldTime);
			if (this.currentTarget is CosmeticCritterShadeFleeing)
			{
				if (!base.IsLocal || this.heartbeatCooldown > 0.4f)
				{
					this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.LOCKED);
				}
				Vector3 normalized = (this.catchOrigin.position - this.currentTarget.transform.position).normalized;
				(this.currentTarget as CosmeticCritterShadeFleeing).pullVector += this.vacuumSpeed * Time.deltaTime * normalized;
				return;
			}
			if (this.currentTarget is CosmeticCritterShadeHidden && (!base.IsLocal || this.heartbeatCooldown > 0.4f))
			{
				this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.TRACKING);
				return;
			}
		}
		else if (this.targetHoldTime > 0f)
		{
			this.targetHoldTime = Mathf.Max(this.targetHoldTime - Time.deltaTime, 0f);
		}
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x000196A1 File Offset: 0x000178A1
	protected override void OnEnable()
	{
		base.OnEnable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x000196C6 File Offset: 0x000178C6
	protected override void OnDisable()
	{
		base.OnDisable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x040004F7 RID: 1271
	[SerializeField]
	private float secondsToReveal = 1f;

	// Token: 0x040004F8 RID: 1272
	[SerializeField]
	private float minSecondsLockedToCatch = 1f;

	// Token: 0x040004F9 RID: 1273
	[SerializeField]
	private Transform catchOrigin;

	// Token: 0x040004FA RID: 1274
	[SerializeField]
	private float catchRadius = 1f;

	// Token: 0x040004FB RID: 1275
	[SerializeField]
	private float vacuumSpeed = 3f;

	// Token: 0x040004FC RID: 1276
	private ShadeRevealer shadeRevealer;

	// Token: 0x040004FD RID: 1277
	private CosmeticCritter currentTarget;

	// Token: 0x040004FE RID: 1278
	private float targetHoldTime;

	// Token: 0x040004FF RID: 1279
	private float maxHoldTime;

	// Token: 0x04000501 RID: 1281
	private const float HEARTBEAT_DELAY = 1f;

	// Token: 0x04000502 RID: 1282
	private float heartbeatCooldown;
}
