using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// Token: 0x020003A3 RID: 931
public class FusionNetPlayer : NetPlayer
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06001645 RID: 5701 RVA: 0x0007BEAD File Offset: 0x0007A0AD
	// (set) Token: 0x06001646 RID: 5702 RVA: 0x0007BEB5 File Offset: 0x0007A0B5
	public PlayerRef PlayerRef { get; private set; }

	// Token: 0x06001647 RID: 5703 RVA: 0x0007BEC0 File Offset: 0x0007A0C0
	public FusionNetPlayer()
	{
		this.PlayerRef = default(PlayerRef);
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x0007BEE2 File Offset: 0x0007A0E2
	public FusionNetPlayer(PlayerRef playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06001649 RID: 5705 RVA: 0x0007BEF1 File Offset: 0x0007A0F1
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x0600164A RID: 5706 RVA: 0x0007BF04 File Offset: 0x0007A104
	public override bool IsValid
	{
		get
		{
			return this.validPlayer && this.PlayerRef.IsRealPlayer;
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x0600164B RID: 5707 RVA: 0x0007BF2C File Offset: 0x0007A12C
	public override int ActorNumber
	{
		get
		{
			return this.PlayerRef.PlayerId;
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x0600164C RID: 5708 RVA: 0x0007BF48 File Offset: 0x0007A148
	public override string UserId
	{
		get
		{
			return NetworkSystem.Instance.GetUserID(this.PlayerRef.PlayerId);
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x0600164D RID: 5709 RVA: 0x0007BF70 File Offset: 0x0007A170
	public override bool IsMasterClient
	{
		get
		{
			if (!(this.runner == null))
			{
				return (this.IsLocal && this.runner.IsSharedModeMasterClient) || NetworkSystem.Instance.MasterClient == this;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x0600164E RID: 5710 RVA: 0x0007BFC4 File Offset: 0x0007A1C4
	public override bool IsLocal
	{
		get
		{
			if (!(this.runner == null))
			{
				return this.PlayerRef == this.runner.LocalPlayer;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x0600164F RID: 5711 RVA: 0x0007C00A File Offset: 0x0007A20A
	public override bool IsNull
	{
		get
		{
			PlayerRef playerRef = this.PlayerRef;
			return false;
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06001650 RID: 5712 RVA: 0x0007C014 File Offset: 0x0007A214
	public override string NickName
	{
		get
		{
			return NetworkSystem.Instance.GetNickName(this);
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06001651 RID: 5713 RVA: 0x0007C024 File Offset: 0x0007A224
	public override string DefaultName
	{
		get
		{
			if (string.IsNullOrEmpty(this._defaultName))
			{
				this._defaultName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			}
			return this._defaultName;
		}
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06001652 RID: 5714 RVA: 0x0007C070 File Offset: 0x0007A270
	public override bool InRoom
	{
		get
		{
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.PlayerRef)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x0007C0D0 File Offset: 0x0007A2D0
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((FusionNetPlayer)myPlayer).PlayerRef.Equals(((FusionNetPlayer)other).PlayerRef);
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x0007C103 File Offset: 0x0007A303
	public void InitPlayer(PlayerRef player)
	{
		this.PlayerRef = player;
		this.validPlayer = true;
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x0007C114 File Offset: 0x0007A314
	public override void OnReturned()
	{
		base.OnReturned();
		this.PlayerRef = default(PlayerRef);
		if (this.PlayerRef.PlayerId != -1)
		{
			Debug.LogError("Returned Player to pool but isnt -1, broken");
		}
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x0007C151 File Offset: 0x0007A351
	public override void OnTaken()
	{
		base.OnTaken();
	}

	// Token: 0x04002065 RID: 8293
	private string _defaultName;

	// Token: 0x04002066 RID: 8294
	private bool validPlayer;
}
