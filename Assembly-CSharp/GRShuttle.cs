using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000709 RID: 1801
public class GRShuttle : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002E1A RID: 11802 RVA: 0x000FAB94 File Offset: 0x000F8D94
	public void Awake()
	{
		this.shuttleUI.Setup(null, null);
		if (this.entryCardScanner != null)
		{
			this.entryCardScanner.requireSpecificPlayer = true;
			this.entryCardScanner.restrictToPlayer = null;
		}
		if (this.departCardScanner != null)
		{
			this.departCardScanner.requireSpecificPlayer = true;
			this.departCardScanner.restrictToPlayer = null;
		}
		this.state = GRShuttleState.Docked;
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000FAC01 File Offset: 0x000F8E01
	public void Init(int shuttleId)
	{
		this.shuttleId = shuttleId;
		this.StopMoveFx();
	}

	// Token: 0x06002E1E RID: 11806 RVA: 0x000FAC10 File Offset: 0x000F8E10
	public void SetBay(GRBay bay)
	{
		this.shuttleBay = bay;
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x000FAC19 File Offset: 0x000F8E19
	public void SetReactor(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x000FAC22 File Offset: 0x000F8E22
	public void SetLocation(GRShuttleGroupLoc location)
	{
		this.location = location;
		this.targetSection = this.ClampTargetSection(this.targetSection);
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x000FAC3D File Offset: 0x000F8E3D
	public void Setup(GhostReactor reactor, GRShuttleGroupLoc location, int employeeIndex)
	{
		this.reactor = reactor;
		this.location = location;
		this.employeeIndex = employeeIndex;
		this.SetOwner(null);
		this.targetSection = this.ClampTargetSection(this.targetSection);
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000FAC70 File Offset: 0x000F8E70
	public int GetTargetFloor()
	{
		if (this.specificDestinationShuttle != null)
		{
			return this.specificDestinationShuttle.specificFloor;
		}
		if (this.targetSection < 0 || this.targetSection >= GRShuttle.sectionFloors.Length)
		{
			return 0;
		}
		return GRShuttle.sectionFloors[this.targetSection];
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x000FACBD File Offset: 0x000F8EBD
	public GRShuttleState GetState()
	{
		return this.state;
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x000FACC5 File Offset: 0x000F8EC5
	public NetPlayer GetOwner()
	{
		return this.shuttleOwner;
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000FACD0 File Offset: 0x000F8ED0
	public void SetOwner(NetPlayer player)
	{
		this.shuttleOwner = player;
		this.shuttleUI.Setup(this.reactor, player);
		this.entryCardScanner.restrictToPlayer = player;
		this.departCardScanner.restrictToPlayer = player;
		if (this.shuttleBay != null)
		{
			this.shuttleBay.Refresh();
		}
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000FAD27 File Offset: 0x000F8F27
	public void SliceUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000FAD2F File Offset: 0x000F8F2F
	public void Refresh()
	{
		this.shuttleUI.RefreshUI();
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x00002789 File Offset: 0x00000989
	public void JoinShuttleRoomLocalPlayer(GRShuttle sourceShuttle, GRShuttle destShuttle)
	{
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x000FAD3C File Offset: 0x000F8F3C
	public static void TeleportLocalPlayer(GRShuttle sourceShuttle, GRShuttle destShuttle)
	{
		sourceShuttle.friendCollider.RefreshPlayersInSphere();
		if (!sourceShuttle.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		VRRig localRig = VRRig.LocalRig;
		float num = destShuttle.transform.rotation.eulerAngles.y - sourceShuttle.transform.rotation.eulerAngles.y;
		Vector3 vector = localRig.transform.position - instance.transform.position;
		Vector3 vector2 = sourceShuttle.transform.InverseTransformPoint(instance.transform.position);
		vector2.x *= 0.8f;
		vector2.z *= 0.8f;
		Vector3 position = destShuttle.transform.TransformPoint(vector2);
		instance.TeleportTo(position, instance.transform.rotation, false, false);
		instance.turnParent.transform.RotateAround(instance.headCollider.transform.position, sourceShuttle.transform.up, num);
		localRig.transform.position = instance.transform.position + vector;
		instance.InitializeValues();
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x000FAE78 File Offset: 0x000F9078
	public void SetState(GRShuttleState newState, bool force = false)
	{
		if (this.state == newState && !force)
		{
			return;
		}
		switch (this.state)
		{
		case GRShuttleState.Docked:
			if (this.shuttleBay != null)
			{
				this.shuttleBay.Refresh();
			}
			break;
		case GRShuttleState.PostMove:
			if (this.specificDestinationShuttle != null)
			{
				this.OpenDoorLocal();
			}
			else
			{
				this.CloseDoorLocal();
			}
			break;
		case GRShuttleState.PostArrive:
			this.OpenDoorLocal();
			break;
		}
		this.state = newState;
		this.stateStartTime = Time.timeAsDouble;
		switch (this.state)
		{
		case GRShuttleState.Docked:
			if (this.shuttleBay != null)
			{
				this.shuttleBay.Refresh();
			}
			this.StopMoveFx();
			break;
		case GRShuttleState.PreMove:
			this.CloseDoorLocal();
			this.takeOffSound.Play(null);
			if (this.specificDestinationShuttle != null)
			{
				GRPlayer grplayer = GRPlayer.Get(GRElevatorManager.LowestActorNumberInElevator(this.friendCollider, this.specificDestinationShuttle.friendCollider));
				this.shuttleOwner = grplayer.gamePlayer.rig.OwningNetPlayer;
			}
			GRShuttle.TryStartLocalPlayerShuttleMove(this.shuttleId, this.shuttleOwner);
			this.StartMoveFx();
			return;
		case GRShuttleState.Moving:
			this.moveSound.Play(null);
			return;
		case GRShuttleState.PostMove:
			break;
		case GRShuttleState.Arriving:
			this.CloseDoorLocal();
			this.moveSound.Play(null);
			return;
		case GRShuttleState.PostArrive:
			this.landSound.Play(null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x000FAFEC File Offset: 0x000F91EC
	private void UpdateState()
	{
		double timeAsDouble = Time.timeAsDouble;
		switch (this.state)
		{
		case GRShuttleState.PreMove:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Moving, false);
				return;
			}
			break;
		case GRShuttleState.Moving:
			if (timeAsDouble > this.stateStartTime + 5.0)
			{
				this.SetState(GRShuttleState.PostMove, false);
				return;
			}
			break;
		case GRShuttleState.PostMove:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Docked, false);
				return;
			}
			break;
		case GRShuttleState.Arriving:
			if (timeAsDouble > this.stateStartTime + 2.0)
			{
				this.SetState(GRShuttleState.PostArrive, false);
				return;
			}
			break;
		case GRShuttleState.PostArrive:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Docked, false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x000FB0AE File Offset: 0x000F92AE
	public void RequestArrival()
	{
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleArrive, this.shuttleId);
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x000FB0C8 File Offset: 0x000F92C8
	private void StartMoveFx()
	{
		if (this.windowFx != null)
		{
			this.windowFx.Play();
		}
		for (int i = 0; i < this.hideOnMove.Count; i++)
		{
			this.hideOnMove[i].SetActive(false);
		}
		for (int j = 0; j < this.showOnMove.Count; j++)
		{
			this.showOnMove[j].SetActive(true);
		}
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x000FB140 File Offset: 0x000F9340
	private void StopMoveFx()
	{
		if (this.windowFx != null)
		{
			this.windowFx.Stop();
		}
		for (int i = 0; i < this.hideOnMove.Count; i++)
		{
			this.hideOnMove[i].SetActive(true);
		}
		for (int j = 0; j < this.showOnMove.Count; j++)
		{
			this.showOnMove[j].SetActive(false);
		}
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x000FB1B8 File Offset: 0x000F93B8
	public bool IsPodUnlocked()
	{
		if (this.specificDestinationShuttle != null)
		{
			return true;
		}
		if (this.shuttleOwner == null)
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(this.shuttleOwner);
		return !(grplayer == null) && grplayer.IsDropPodUnlocked();
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x000FB1FC File Offset: 0x000F93FC
	public int GetMaxDropFloor()
	{
		if (this.shuttleOwner == null)
		{
			return 0;
		}
		GRPlayer grplayer = GRPlayer.Get(this.shuttleOwner);
		if (grplayer == null)
		{
			return 0;
		}
		return grplayer.GetMaxDropFloor();
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x000FB230 File Offset: 0x000F9430
	public void OnShuttleMove()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleLaunch, this.shuttleId);
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000FB254 File Offset: 0x000F9454
	public void OnShuttleMoveActorNr(int actorNr)
	{
		if (this.state != GRShuttleState.Docked || actorNr != this.shuttleOwner.ActorNumber || this.GetTargetFloor() > this.GetMaxDropFloor())
		{
			this.departCardScanner.onFailed.Invoke();
			return;
		}
		this.departCardScanner.onSucceeded.Invoke();
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleLaunch, this.shuttleId);
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000FB2BE File Offset: 0x000F94BE
	public void TargetLevelUp()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleTargetLevelUp, this.shuttleId);
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x000FB2E1 File Offset: 0x000F94E1
	public void TargetLevelDown()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleTargetLevelDown, this.shuttleId);
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x000FB304 File Offset: 0x000F9504
	private GRShuttle GetTargetShuttle()
	{
		if (this.specificDestinationShuttle != null)
		{
			return this.specificDestinationShuttle;
		}
		if (this.shuttleOwner == null)
		{
			return null;
		}
		GRShuttle drillShuttleForPlayer = GRElevatorManager._instance.GetDrillShuttleForPlayer(this.shuttleOwner.ActorNumber);
		GRShuttle stagingShuttleForPlayer = GRElevatorManager._instance.GetStagingShuttleForPlayer(this.shuttleOwner.ActorNumber);
		if (this.location != GRShuttleGroupLoc.Drill)
		{
			return drillShuttleForPlayer;
		}
		return stagingShuttleForPlayer;
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x000FB368 File Offset: 0x000F9568
	public bool IsPlayerOwner(GRPlayer player)
	{
		return GRPlayer.Get(this.GetOwner()) == player;
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x000FB37C File Offset: 0x000F957C
	public bool IsShuttleInteractableByPlayer(GRPlayer player, bool ignoreOwnership)
	{
		if (!ignoreOwnership && !this.IsPlayerOwner(player) && this.specificDestinationShuttle == null)
		{
			return false;
		}
		if (this.entryCardScanner == null)
		{
			return true;
		}
		if (this.departCardScanner == null)
		{
			return true;
		}
		bool flag = GameEntityManager.IsPlayerHandNearPosition(player.gamePlayer, this.entryCardScanner.transform.position, false, true, 16f);
		bool flag2 = GameEntityManager.IsPlayerHandNearPosition(player.gamePlayer, this.departCardScanner.transform.position, false, true, 16f);
		return flag || flag2;
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x000FB40C File Offset: 0x000F960C
	public bool IsPlayerOwner(NetPlayer player)
	{
		return this.GetOwner() == player;
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000FB418 File Offset: 0x000F9618
	public void ToggleDoor()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		if (this.entryDoor.doorState == GRDoor.DoorState.Closed)
		{
			this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleOpen, this.shuttleId);
			return;
		}
		if (this.entryDoor.doorState == GRDoor.DoorState.Open)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble > this.lastCloseTime + 5.0)
			{
				this.lastCloseTime = timeAsDouble;
				this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleClose, this.shuttleId);
			}
		}
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x000FB49C File Offset: 0x000F969C
	public void ToggleDoorActorNr(int actorNr)
	{
		if (this.state == GRShuttleState.Docked && this.GetOwner() != null && this.GetOwner().ActorNumber == actorNr && GRPlayer.Get(this.shuttleOwner).IsDropPodUnlocked())
		{
			IDCardScanner idcardScanner = this.entryCardScanner;
			if (idcardScanner != null)
			{
				idcardScanner.onSucceeded.Invoke();
			}
			this.ToggleDoor();
			return;
		}
		IDCardScanner idcardScanner2 = this.entryCardScanner;
		if (idcardScanner2 == null)
		{
			return;
		}
		idcardScanner2.onFailed.Invoke();
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000FB50C File Offset: 0x000F970C
	public void EmergencyOpenDoor()
	{
		if (this.state == GRShuttleState.Docked)
		{
			if (PhotonNetwork.InRoom)
			{
				this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleOpen, this.shuttleId);
				return;
			}
			this.OpenDoorLocal();
		}
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x000FB53C File Offset: 0x000F973C
	public void OnOpenDoor()
	{
		if (this.entryDoor.doorState == GRDoor.DoorState.Closed && this.entryCardScanner != null)
		{
			this.entryCardScanner.onSucceeded.Invoke();
		}
		this.OpenDoorLocal();
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000FB56F File Offset: 0x000F976F
	public void OpenDoorLocal()
	{
		if (this.entryDoor != null && this.entryDoor.doorState == GRDoor.DoorState.Closed)
		{
			this.entryDoor.SetDoorState(GRDoor.DoorState.Open);
		}
		if (this.shuttleBay != null)
		{
			this.shuttleBay.SetOpen(true);
		}
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000FB5AC File Offset: 0x000F97AC
	public void CloseDoorLocal()
	{
		if (this.entryDoor != null && this.entryDoor.doorState == GRDoor.DoorState.Open)
		{
			this.entryDoor.SetDoorState(GRDoor.DoorState.Closed);
		}
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000FB5D0 File Offset: 0x000F97D0
	public void OnCloseDoor()
	{
		if (this.entryDoor.doorState == GRDoor.DoorState.Open && this.entryCardScanner != null)
		{
			this.entryCardScanner.onSucceeded.Invoke();
		}
		this.CloseDoorLocal();
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000FB604 File Offset: 0x000F9804
	public void OnLaunch()
	{
		if (this.GetTargetFloor() > this.GetMaxDropFloor())
		{
			return;
		}
		this.SetState(GRShuttleState.PreMove, false);
		if (this.departCardScanner != null)
		{
			this.departCardScanner.onSucceeded.Invoke();
		}
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000FB640 File Offset: 0x000F9840
	public void OnArrive()
	{
		this.SetState(GRShuttleState.Arriving, false);
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000FB64A File Offset: 0x000F984A
	public void OnTargetLevelUp()
	{
		this.targetSection = this.ClampTargetSection(this.targetSection - 1);
		if (this.shuttleUI != null)
		{
			this.shuttleUI.RefreshUI();
		}
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000FB673 File Offset: 0x000F9873
	public void OnTargetLevelDown()
	{
		this.targetSection = this.ClampTargetSection(this.targetSection + 1);
		if (this.shuttleUI != null)
		{
			this.shuttleUI.RefreshUI();
		}
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x000FB69C File Offset: 0x000F989C
	private int ClampTargetSection(int newTargetSection)
	{
		if (this.location == GRShuttleGroupLoc.Staging)
		{
			newTargetSection = Mathf.Clamp(newTargetSection, 1, GRShuttle.sectionFloors.Length - 1);
		}
		else
		{
			newTargetSection = 0;
		}
		return newTargetSection;
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000FB6C0 File Offset: 0x000F98C0
	public static void TryStartLocalPlayerShuttleMove(int currShuttleId, NetPlayer shuttleOwner)
	{
		GRPlayer local = GRPlayer.GetLocal();
		if (local == null)
		{
			return;
		}
		GRShuttle shuttle = GRElevatorManager.GetShuttle(currShuttleId);
		if (shuttle == null)
		{
			return;
		}
		if (!GRElevatorManager.IsPlayerInShuttle(local.gamePlayer.rig.OwningNetPlayer.ActorNumber, shuttle, null))
		{
			return;
		}
		if (shuttleOwner != null && shuttleOwner.GetPlayerRef() != null)
		{
			local.shuttleData.ownerUserId = shuttleOwner.UserId;
		}
		else
		{
			local.shuttleData.ownerUserId = VRRig.LocalRig.OwningNetPlayer.UserId;
		}
		local.shuttleData.currShuttleId = currShuttleId;
		local.shuttleData.targetShuttleId = -1;
		local.shuttleData.targetLevel = shuttle.GetTargetFloor();
		GRShuttle.SetPlayerShuttleState(local, GRPlayer.ShuttleState.Moving);
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000FB778 File Offset: 0x000F9978
	public static void UpdateGRPlayerShuttle(GRPlayer player)
	{
		if (player == null)
		{
			return;
		}
		GRPlayer.ShuttleData shuttleData = player.shuttleData;
		if (shuttleData == null || shuttleData.state == GRPlayer.ShuttleState.Idle)
		{
			return;
		}
		if (!player.gamePlayer.IsLocal())
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		double num = shuttleData.stateStartTime;
		if (shuttleData.state != GRPlayer.ShuttleState.Idle && timeAsDouble > num + 10.0)
		{
			GRShuttle.CancelPlayerShuttle(player);
			return;
		}
		switch (shuttleData.state)
		{
		case GRPlayer.ShuttleState.Moving:
			if (timeAsDouble > num + 3.0)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.JoinRoom);
				return;
			}
			break;
		case GRPlayer.ShuttleState.WaitForLeaveRoom:
			if (!PhotonNetwork.InRoom)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeadPlayer);
				return;
			}
			break;
		case GRPlayer.ShuttleState.JoinRoom:
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeadPlayer);
				return;
			}
			GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeaveRoom);
			return;
		case GRPlayer.ShuttleState.WaitForLeadPlayer:
			player.shuttleData.targetShuttleId = -1;
			if (PhotonNetwork.InRoom)
			{
				player.shuttleData.targetShuttleId = GRShuttle.CalcTargetShuttleId(player.shuttleData.currShuttleId, player.shuttleData.ownerUserId);
			}
			if (player.shuttleData.targetShuttleId != -1)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Teleport);
				return;
			}
			break;
		case GRPlayer.ShuttleState.Teleport:
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId).zone);
			if (timeAsDouble > num + 1.0 && (managerForZone == null || managerForZone.IsZoneActive()))
			{
				int num2 = GRShuttle.CalcTargetShuttleId(player.shuttleData.currShuttleId, player.shuttleData.ownerUserId);
				if (num2 == player.shuttleData.targetShuttleId)
				{
					GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.PostTeleport);
					return;
				}
				if (num2 != -1)
				{
					player.shuttleData.currShuttleId = player.shuttleData.targetShuttleId;
					player.shuttleData.targetShuttleId = num2;
					GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.TeleportToMyShuttleSafety);
					return;
				}
			}
			break;
		}
		case GRPlayer.ShuttleState.TeleportToMyShuttleSafety:
			GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.PostTeleport);
			return;
		case GRPlayer.ShuttleState.PostTeleport:
			if (timeAsDouble > num + 1.0)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000FB968 File Offset: 0x000F9B68
	public static int CalcTargetShuttleId(int currShuttleId, string ownerUserId)
	{
		GRShuttle shuttle = GRElevatorManager.GetShuttle(currShuttleId);
		if (shuttle.specificDestinationShuttle != null)
		{
			return shuttle.specificDestinationShuttle.shuttleId;
		}
		GRPlayer fromUserId = GRPlayer.GetFromUserId(ownerUserId);
		if (fromUserId != null)
		{
			bool isOnDrillovator = shuttle.GetTargetFloor() >= 0;
			GRShuttle assignedShuttle = fromUserId.GetAssignedShuttle(isOnDrillovator);
			if (assignedShuttle != null)
			{
				return assignedShuttle.shuttleId;
			}
		}
		return -1;
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000FB9CC File Offset: 0x000F9BCC
	public static void CancelPlayerShuttle(GRPlayer player)
	{
		GRPlayer.ShuttleState shuttleState = player.shuttleData.state;
		if (shuttleState - GRPlayer.ShuttleState.Moving > 3)
		{
			if (shuttleState - GRPlayer.ShuttleState.Teleport <= 2)
			{
				GRShuttle shuttle = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
				if (shuttle != null)
				{
					shuttle.OpenDoorLocal();
				}
			}
		}
		else
		{
			GRShuttle shuttle2 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			if (shuttle2 != null)
			{
				shuttle2.OpenDoorLocal();
			}
		}
		GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Idle);
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000FBA3C File Offset: 0x000F9C3C
	public static void SetPlayerShuttleState(GRPlayer player, GRPlayer.ShuttleState newState)
	{
		GRPlayer.ShuttleData shuttleData = player.shuttleData;
		GRPlayer.ShuttleState shuttleState = shuttleData.state;
		shuttleData.state = newState;
		shuttleData.stateStartTime = Time.timeAsDouble;
		switch (shuttleData.state)
		{
		case GRPlayer.ShuttleState.Moving:
		case GRPlayer.ShuttleState.WaitForLeaveRoom:
		case GRPlayer.ShuttleState.WaitForLeadPlayer:
			break;
		case GRPlayer.ShuttleState.JoinRoom:
		{
			GRShuttle shuttle = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle targetShuttle = shuttle.GetTargetShuttle();
			if (targetShuttle != null && !NetworkSystem.Instance.SessionIsPrivate && shuttle.shuttleOwner.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				GRElevatorManager.LeadShuttleJoin(shuttle.friendCollider, targetShuttle.friendCollider, targetShuttle.joinTrigger, shuttle.GetTargetFloor());
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.Teleport:
		{
			GRShuttle shuttle2 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle shuttle3 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle3 != null)
			{
				GRShuttle.TeleportLocalPlayer(shuttle2, shuttle3);
				shuttle3.CloseDoorLocal();
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.TeleportToMyShuttleSafety:
		{
			GRShuttle shuttle4 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle shuttle5 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle5 != null)
			{
				GRShuttle.TeleportLocalPlayer(shuttle4, shuttle5);
				shuttle5.CloseDoorLocal();
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.PostTeleport:
		{
			GRShuttle shuttle6 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle6 != null)
			{
				shuttle6.RequestArrival();
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x04003C50 RID: 15440
	public const int InvalidId = -1;

	// Token: 0x04003C51 RID: 15441
	private const int MAX_DEPTH = 29;

	// Token: 0x04003C52 RID: 15442
	public GTZone zone;

	// Token: 0x04003C53 RID: 15443
	public GRShuttleUI shuttleUI;

	// Token: 0x04003C54 RID: 15444
	public GRDoor entryDoor;

	// Token: 0x04003C55 RID: 15445
	private GRShuttleGroupLoc location;

	// Token: 0x04003C56 RID: 15446
	private int employeeIndex;

	// Token: 0x04003C57 RID: 15447
	public AbilitySound takeOffSound;

	// Token: 0x04003C58 RID: 15448
	public AbilitySound moveSound;

	// Token: 0x04003C59 RID: 15449
	public AbilitySound landSound;

	// Token: 0x04003C5A RID: 15450
	public GorillaFriendCollider friendCollider;

	// Token: 0x04003C5B RID: 15451
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04003C5C RID: 15452
	public GRShuttle specificDestinationShuttle;

	// Token: 0x04003C5D RID: 15453
	public int specificFloor = -1;

	// Token: 0x04003C5E RID: 15454
	public ParticleSystem windowFx;

	// Token: 0x04003C5F RID: 15455
	public List<GameObject> hideOnMove;

	// Token: 0x04003C60 RID: 15456
	public List<GameObject> showOnMove;

	// Token: 0x04003C61 RID: 15457
	public BoxCollider inShuttleVolume;

	// Token: 0x04003C62 RID: 15458
	public IDCardScanner entryCardScanner;

	// Token: 0x04003C63 RID: 15459
	public IDCardScanner departCardScanner;

	// Token: 0x04003C64 RID: 15460
	[NonSerialized]
	public int shuttleId;

	// Token: 0x04003C65 RID: 15461
	private GhostReactor reactor;

	// Token: 0x04003C66 RID: 15462
	private int targetSection;

	// Token: 0x04003C67 RID: 15463
	private GRShuttleState state;

	// Token: 0x04003C68 RID: 15464
	private double stateStartTime;

	// Token: 0x04003C69 RID: 15465
	private GRBay shuttleBay;

	// Token: 0x04003C6A RID: 15466
	private NetPlayer shuttleOwner;

	// Token: 0x04003C6B RID: 15467
	private double lastCloseTime;

	// Token: 0x04003C6C RID: 15468
	private static int[] sectionFloors = new int[]
	{
		-1,
		0,
		4,
		9,
		14,
		19,
		24,
		29
	};
}
