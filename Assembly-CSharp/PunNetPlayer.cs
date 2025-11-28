using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020003F4 RID: 1012
[Serializable]
public class PunNetPlayer : NetPlayer
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060018D4 RID: 6356 RVA: 0x000856BF File Offset: 0x000838BF
	// (set) Token: 0x060018D5 RID: 6357 RVA: 0x000856C7 File Offset: 0x000838C7
	public Player PlayerRef { get; private set; }

	// Token: 0x060018D7 RID: 6359 RVA: 0x000856D8 File Offset: 0x000838D8
	public void InitPlayer(Player playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060018D8 RID: 6360 RVA: 0x000856E1 File Offset: 0x000838E1
	public override bool IsValid
	{
		get
		{
			return !this.PlayerRef.IsInactive;
		}
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060018D9 RID: 6361 RVA: 0x000856F1 File Offset: 0x000838F1
	public override int ActorNumber
	{
		get
		{
			Player playerRef = this.PlayerRef;
			if (playerRef == null)
			{
				return -1;
			}
			return playerRef.ActorNumber;
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x060018DA RID: 6362 RVA: 0x00085704 File Offset: 0x00083904
	public override string UserId
	{
		get
		{
			return this.PlayerRef.UserId;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x060018DB RID: 6363 RVA: 0x00085711 File Offset: 0x00083911
	public override bool IsMasterClient
	{
		get
		{
			return this.PlayerRef.IsMasterClient;
		}
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060018DC RID: 6364 RVA: 0x0008571E File Offset: 0x0008391E
	public override bool IsLocal
	{
		get
		{
			return this.PlayerRef == PhotonNetwork.LocalPlayer;
		}
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060018DD RID: 6365 RVA: 0x0008572D File Offset: 0x0008392D
	public override bool IsNull
	{
		get
		{
			return this.PlayerRef == null;
		}
	}

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x060018DE RID: 6366 RVA: 0x00085738 File Offset: 0x00083938
	public override string NickName
	{
		get
		{
			return this.PlayerRef.NickName;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x060018DF RID: 6367 RVA: 0x00085745 File Offset: 0x00083945
	public override string DefaultName
	{
		get
		{
			return this.PlayerRef.DefaultName;
		}
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x060018E0 RID: 6368 RVA: 0x00085752 File Offset: 0x00083952
	public override bool InRoom
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && currentRoom.Players.ContainsValue(this.PlayerRef);
		}
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x0008576F File Offset: 0x0008396F
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).PlayerRef.Equals(((PunNetPlayer)other).PlayerRef);
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x00085794 File Offset: 0x00083994
	public override void OnReturned()
	{
		base.OnReturned();
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x0008579C File Offset: 0x0008399C
	public override void OnTaken()
	{
		base.OnTaken();
		this.PlayerRef = null;
	}
}
