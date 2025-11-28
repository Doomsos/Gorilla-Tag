using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000BFA RID: 3066
[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	// Token: 0x17000711 RID: 1809
	// (get) Token: 0x06004BCE RID: 19406 RVA: 0x0018C48B File Offset: 0x0018A68B
	public ExpectedUsersDecayTimer ExpectedUsersTimer
	{
		get
		{
			return this.expectedUsersTimer;
		}
	}

	// Token: 0x17000712 RID: 1810
	// (get) Token: 0x06004BCF RID: 19407 RVA: 0x0018C493 File Offset: 0x0018A693
	public TickSystemTimer ResyncNetworkTimeTimer
	{
		get
		{
			return this.resyncNetworkTimeTimer;
		}
	}

	// Token: 0x17000713 RID: 1811
	// (get) Token: 0x06004BD0 RID: 19408 RVA: 0x0018C49B File Offset: 0x0018A69B
	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	// Token: 0x17000714 RID: 1812
	// (get) Token: 0x06004BD1 RID: 19409 RVA: 0x0018C4A3 File Offset: 0x0018A6A3
	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	// Token: 0x17000715 RID: 1813
	// (get) Token: 0x06004BD2 RID: 19410 RVA: 0x0018C4AB File Offset: 0x0018A6AB
	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	// Token: 0x17000716 RID: 1814
	// (get) Token: 0x06004BD3 RID: 19411 RVA: 0x0018C4B3 File Offset: 0x0018A6B3
	public CallLimiterWithCooldown PlayerEffectLimiter
	{
		get
		{
			return this.playerEffectLimiter;
		}
	}

	// Token: 0x17000717 RID: 1815
	// (get) Token: 0x06004BD4 RID: 19412 RVA: 0x0018C4BB File Offset: 0x0018A6BB
	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	// Token: 0x17000718 RID: 1816
	// (get) Token: 0x06004BD5 RID: 19413 RVA: 0x0018C4C3 File Offset: 0x0018A6C3
	public List<RoomSystem.PlayerEffectConfig> PlayerEffects
	{
		get
		{
			return this.playerEffects;
		}
	}

	// Token: 0x17000719 RID: 1817
	// (get) Token: 0x06004BD6 RID: 19414 RVA: 0x0018C4CB File Offset: 0x0018A6CB
	public int PausedDCTimer
	{
		get
		{
			return this.pausedDCTimer;
		}
	}

	// Token: 0x04005BDA RID: 23514
	[SerializeField]
	private ExpectedUsersDecayTimer expectedUsersTimer;

	// Token: 0x04005BDB RID: 23515
	[SerializeField]
	private TickSystemTimer resyncNetworkTimeTimer;

	// Token: 0x04005BDC RID: 23516
	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	// Token: 0x04005BDD RID: 23517
	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	// Token: 0x04005BDE RID: 23518
	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	// Token: 0x04005BDF RID: 23519
	[SerializeField]
	private CallLimiterWithCooldown playerEffectLimiter;

	// Token: 0x04005BE0 RID: 23520
	[SerializeField]
	private GameObject playerImpactEffect;

	// Token: 0x04005BE1 RID: 23521
	[SerializeField]
	private List<RoomSystem.PlayerEffectConfig> playerEffects = new List<RoomSystem.PlayerEffectConfig>();

	// Token: 0x04005BE2 RID: 23522
	[SerializeField]
	private int pausedDCTimer;
}
