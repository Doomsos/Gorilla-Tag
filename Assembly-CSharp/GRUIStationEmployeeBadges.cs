using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000747 RID: 1863
public class GRUIStationEmployeeBadges : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003025 RID: 12325 RVA: 0x001075F0 File Offset: 0x001057F0
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Setup(reactor, i);
		}
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x00107630 File Offset: 0x00105830
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.registeredBadges = new List<GRBadge>();
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].index = i;
			this.badgeDispensers[i].actorNr = -1;
		}
		this.dispenserForActorNr = new Dictionary<int, int>();
		VRRigCache.OnRigActivated += new Action<RigContainer>(this.UpdateRigs);
		VRRigCache.OnRigDeactivated += new Action<RigContainer>(this.UpdateRigs);
		RoomSystem.JoinedRoomEvent += new Action(this.UpdateRigs);
		this.UpdateRigs();
	}

	// Token: 0x06003027 RID: 12327 RVA: 0x001076D8 File Offset: 0x001058D8
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		VRRigCache.OnRigActivated -= new Action<RigContainer>(this.UpdateRigs);
		VRRigCache.OnRigDeactivated -= new Action<RigContainer>(this.UpdateRigs);
		RoomSystem.JoinedRoomEvent -= new Action(this.UpdateRigs);
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x00107729 File Offset: 0x00105929
	public void UpdateRigs(RigContainer container)
	{
		this.UpdateRigs();
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x00107731 File Offset: 0x00105931
	public void UpdateRigs()
	{
		GRUIStationEmployeeBadges.tempRigs.Clear();
		GRUIStationEmployeeBadges.tempRigs.Add(VRRig.LocalRig);
		if (VRRigCache.Instance != null)
		{
			VRRigCache.Instance.GetAllUsedRigs(GRUIStationEmployeeBadges.tempRigs);
		}
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x00107768 File Offset: 0x00105968
	public void RefreshBadgesAuthority()
	{
		for (int i = 0; i < GRUIStationEmployeeBadges.tempRigs.Count; i++)
		{
			NetPlayer netPlayer = GRUIStationEmployeeBadges.tempRigs[i].isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : GRUIStationEmployeeBadges.tempRigs[i].OwningNetPlayer;
			int num;
			if (netPlayer != null && netPlayer.ActorNumber != -1 && !this.dispenserForActorNr.TryGetValue(netPlayer.ActorNumber, ref num))
			{
				for (int j = 0; j < this.badgeDispensers.Count; j++)
				{
					if (this.badgeDispensers[j].actorNr == -1)
					{
						this.badgeDispensers[j].CreateBadge(netPlayer, this.reactor.grManager.gameEntityManager);
						break;
					}
				}
			}
		}
		for (int k = this.registeredBadges.Count - 1; k >= 0; k--)
		{
			int num2;
			if (NetworkSystem.Instance.GetNetPlayerByID(this.registeredBadges[k].actorNr) == null || !this.dispenserForActorNr.TryGetValue(this.registeredBadges[k].actorNr, ref num2) || num2 != this.registeredBadges[k].dispenserIndex)
			{
				this.reactor.grManager.gameEntityManager.RequestDestroyItem(this.registeredBadges[k].GetComponent<GameEntity>().id);
			}
		}
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x001078D4 File Offset: 0x00105AD4
	public void SliceUpdate()
	{
		if (this.reactor == null || this.reactor.grManager == null)
		{
			return;
		}
		if (!this.reactor.grManager.IsZoneActive())
		{
			return;
		}
		if (this.reactor.grManager.gameEntityManager.IsAuthority())
		{
			this.RefreshBadgesAuthority();
		}
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Refresh();
		}
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x0010795C File Offset: 0x00105B5C
	public void RemoveBadge(GRBadge badge)
	{
		if (this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Remove(badge);
		}
		if (this.badgeDispensers[badge.dispenserIndex].idBadge == badge)
		{
			this.dispenserForActorNr.Remove(badge.actorNr);
			this.badgeDispensers[badge.dispenserIndex].ClearBadge();
		}
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x001079CC File Offset: 0x00105BCC
	public void LinkBadgeToDispenser(GRBadge badge, long createData)
	{
		if (!this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Add(badge);
		}
		int num = (int)(createData % 100L);
		if (num > this.badgeDispensers.Count)
		{
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID((int)(createData / 100L));
		if (netPlayerByID != null)
		{
			this.dispenserForActorNr[netPlayerByID.ActorNumber] = num;
			this.badgeDispensers[num].AttachIDBadge(badge, netPlayerByID);
		}
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x00107A44 File Offset: 0x00105C44
	public GRUIEmployeeBadgeDispenser GetDispenserForPlayer(int actorNumber)
	{
		int num;
		if (!this.dispenserForActorNr.TryGetValue(actorNumber, ref num))
		{
			return null;
		}
		return this.badgeDispensers[num];
	}

	// Token: 0x04003F27 RID: 16167
	[SerializeField]
	public List<GRUIEmployeeBadgeDispenser> badgeDispensers;

	// Token: 0x04003F28 RID: 16168
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003F29 RID: 16169
	public Dictionary<int, int> dispenserForActorNr;

	// Token: 0x04003F2A RID: 16170
	public List<GRBadge> registeredBadges;

	// Token: 0x04003F2B RID: 16171
	private GhostReactor reactor;
}
