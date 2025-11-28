using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
[NetworkBehaviourWeaved(0)]
public class GRElevatorManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x06002B6E RID: 11118 RVA: 0x000E8AAE File Offset: 0x000E6CAE
	public bool InPrivateRoom
	{
		get
		{
			return NetworkSystem.Instance.SessionIsPrivate;
		}
	}

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x06002B6F RID: 11119 RVA: 0x000E8ABA File Offset: 0x000E6CBA
	// (set) Token: 0x06002B70 RID: 11120 RVA: 0x000E8AC2 File Offset: 0x000E6CC2
	public bool TickRunning { get; set; }

	// Token: 0x06002B71 RID: 11121 RVA: 0x000E8ACC File Offset: 0x000E6CCC
	protected override void Awake()
	{
		base.Awake();
		if (GRElevatorManager._instance != null)
		{
			Debug.LogError("Multiple elevator managers! This should never happen!");
			return;
		}
		GRElevatorManager._instance = this;
		this.currentState = GRElevatorManager.ElevatorSystemState.InLocation;
		this.currentLocation = GRElevatorManager.ElevatorLocation.Stump;
		this.destination = GRElevatorManager.ElevatorLocation.Stump;
		this.elevatorByLocation = new Dictionary<GRElevatorManager.ElevatorLocation, GRElevator>();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.elevatorByLocation[this.allElevators[i].location] = this.allElevators[i];
		}
		this.actorIds = new List<int>();
		this.mainStagingShuttle.specificFloor = -1;
		this.mainDrillShuttle.specificFloor = 0;
		this.allShuttles = new List<GRShuttle>(64);
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			GRElevatorManager.GRShuttleGroup grshuttleGroup = this.shuttleGroups[j];
			for (int k = 0; k < grshuttleGroup.ghostReactorStagingShuttles.Count; k++)
			{
				this.allShuttles.Add(grshuttleGroup.ghostReactorStagingShuttles[k]);
				grshuttleGroup.ghostReactorStagingShuttles[k].SetLocation(grshuttleGroup.location);
			}
		}
		this.allShuttles.Add(this.mainStagingShuttle);
		this.allShuttles.Add(this.mainDrillShuttle);
		for (int l = 0; l < this.allShuttles.Count; l++)
		{
			this.allShuttles[l].Init(l);
		}
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x000E8C44 File Offset: 0x000E6E44
	protected override void Start()
	{
		base.Start();
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnLeftRoom);
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.OnPlayerAdded);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerRemoved);
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x000E8CBC File Offset: 0x000E6EBC
	protected void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnLeftRoom);
		NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(this.OnPlayerAdded);
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.OnPlayerRemoved);
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x0000B3F9 File Offset: 0x000095F9
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x0000B40D File Offset: 0x0000960D
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x000E8D34 File Offset: 0x000E6F34
	public void Tick()
	{
		if (!this.cosmeticsInitialized)
		{
			this.CheckInitializationState();
			return;
		}
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].PhysicalElevatorUpdate();
		}
		this.ProcessElevatorSystemState();
		if (this.justTeleported)
		{
			this.justTeleported = false;
			GTPlayer.Instance.disableMovement = false;
		}
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000E8D97 File Offset: 0x000E6F97
	private void CheckInitializationState()
	{
		this.cosmeticsInitialized = true;
		if (GRElevatorManager.InControlOfElevator())
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, GRElevatorManager.ElevatorLocation.Stump);
		}
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x000E8DB0 File Offset: 0x000E6FB0
	public void ProcessElevatorSystemState()
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			if (this.currentLocation == this.destination && this.waitForZoneLoadFallbackTimer >= 0f && this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.waitForZoneLoadFallbackTimer += Time.deltaTime;
				if (this.waitForZoneLoadFallbackTimer >= this.waitForZoneLoadFallbackMaxTime)
				{
					this.OnReachedDestination();
				}
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
		{
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			double time = this.GetTime();
			if (this.elevatorByLocation[this.currentLocation].DoorsFullyClosed() && time >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.WaitingToTeleport, GRElevatorManager.ElevatorLocation.None);
				return;
			}
			if (time >= this.destinationButtonLastPressedTime + (double)this.destinationButtonlastPressedDelay && !this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.destinationButtonLastPressedTime = time;
				this.CloseAllElevators();
				return;
			}
			break;
		}
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			if (this.GetTime() >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay && !this.waitingForRemoteTeleport)
			{
				this.ActivateElevating();
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x000E8EE0 File Offset: 0x000E70E0
	public void ActivateElevating()
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("RemoteActivateTeleport", 0, new object[]
			{
				(int)this.currentLocation,
				(int)this.destination,
				GRElevatorManager.LowestActorNumberInElevator()
			});
			return;
		}
		this.ActivateTeleport(this.currentLocation, this.destination, -1, this.GetTime());
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x000E8F50 File Offset: 0x000E7150
	public void LeadElevatorJoin()
	{
		GRElevatorManager.LeadElevatorJoin(this.elevatorByLocation[this.currentLocation].friendCollider, this.elevatorByLocation[this.destination].friendCollider, this.elevatorByLocation[this.destination].joinTrigger);
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x000E8FA4 File Offset: 0x000E71A4
	public static void LeadElevatorJoin(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			sourceFriendCollider.RefreshPlayersInSphere();
			destinationFriendCollider.RefreshPlayersInSphere();
			PhotonNetworkController.Instance.FriendIDList = new List<string>(sourceFriendCollider.playerIDsCurrentlyTouching);
			PhotonNetworkController.Instance.FriendIDList.AddRange(destinationFriendCollider.playerIDsCurrentlyTouching);
			foreach (string text in PhotonNetworkController.Instance.FriendIDList)
			{
			}
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			RoomSystem.SendElevatorFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, sourceFriendCollider, destinationFriendCollider);
			PhotonNetwork.SendAllOutgoingCommands();
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.JoinWithElevator, null);
		}
		GRElevatorManager.JoinPublicRoom();
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x000E90E4 File Offset: 0x000E72E4
	public static void LeadShuttleJoin(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger, int targetLevel)
	{
		sourceFriendCollider.RefreshPlayersInSphere();
		destinationFriendCollider.RefreshPlayersInSphere();
		GorillaComputer.instance.friendJoinCollider = destinationFriendCollider;
		GorillaComputer.instance.UpdateScreen();
		if (NetworkSystem.Instance.InRoom)
		{
			PhotonNetworkController.Instance.FriendIDList = new List<string>(sourceFriendCollider.playerIDsCurrentlyTouching);
			PhotonNetworkController.Instance.FriendIDList.AddRange(destinationFriendCollider.playerIDsCurrentlyTouching);
			foreach (string text in PhotonNetworkController.Instance.FriendIDList)
			{
			}
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			Debug.Log("Send Shuttle Join");
			RoomSystem.SendShuttleFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, sourceFriendCollider, destinationFriendCollider);
			PhotonNetwork.SendAllOutgoingCommands();
			List<ValueTuple<string, string>> list = null;
			if (targetLevel >= 0)
			{
				int joinDepthSectionFromLevel = GhostReactor.GetJoinDepthSectionFromLevel(targetLevel);
				List<ValueTuple<string, string>> list2 = new List<ValueTuple<string, string>>();
				list2.Add(new ValueTuple<string, string>("ghostReactorDepth", joinDepthSectionFromLevel.ToString()));
				list = list2;
				Debug.LogFormat("GR Room Param Join {0} {1}", new object[]
				{
					list[0].Item1,
					list[0].Item2
				});
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.JoinWithElevator, list);
		}
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.Solo, null);
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x000E92A8 File Offset: 0x000E74A8
	public void UpdateElevatorState(GRElevatorManager.ElevatorSystemState newState, GRElevatorManager.ElevatorLocation location = GRElevatorManager.ElevatorLocation.None)
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.elevatorByLocation[this.currentLocation].PlayDing();
				this.OpenElevator(this.destination);
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.maxDoorClosingTime = this.GetTime();
				this.destinationButtonLastPressedTime = this.GetTime();
				this.doorsFullyClosedTime = this.GetTime();
				if (this.destination != this.currentLocation)
				{
					this.destination = location;
				}
				this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				if (location == this.currentLocation)
				{
					this.OpenElevator(this.currentLocation);
				}
				else
				{
					this.CloseAllElevators();
				}
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.destinationButtonLastPressedTime = this.GetTime();
					this.maxDoorClosingTime = this.GetTime();
				}
				else
				{
					if (this.elevatorByLocation[this.destination].DoorIsClosing())
					{
						this.OpenElevator(this.currentLocation);
					}
					newState = this.currentState;
				}
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.OpenElevator(location);
				this.elevatorByLocation[this.currentLocation].PlayDing();
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
				}
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.doorsFullyClosedTime = this.GetTime();
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
					this.elevatorByLocation[this.currentLocation].PlayElevatorMusic(0f);
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.elevatorByLocation[this.destination].PlayElevatorStopped();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnReachedDestination));
				this.waitForZoneLoadFallbackTimer = 0.01f;
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.currentLocation = location;
				break;
			}
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (location != this.currentLocation)
				{
					this.destination = location;
				}
				else
				{
					this.OpenElevator(location);
					newState = GRElevatorManager.ElevatorSystemState.InLocation;
				}
				break;
			}
			break;
		}
		this.currentState = newState;
		this.UpdateUI();
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000E9624 File Offset: 0x000E7824
	public void UpdateUI()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].outerText.text = "ELEVATOR LOCATION:\n" + this.currentLocation.ToString().ToUpper();
			GRElevatorManager.ElevatorSystemState elevatorSystemState = this.currentState;
			if (elevatorSystemState > GRElevatorManager.ElevatorSystemState.InLocation)
			{
				if (elevatorSystemState - GRElevatorManager.ElevatorSystemState.DestinationPressed <= 1)
				{
					if (this.destination != this.currentLocation)
					{
						this.allElevators[i].innerText.text = "NEXT STOP:\n" + this.destination.ToString().ToUpper();
					}
					else
					{
						this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
					}
				}
			}
			else
			{
				this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
			}
		}
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000E9714 File Offset: 0x000E7914
	public static void RegisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = elevator;
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x000E973A File Offset: 0x000E793A
	public static void DeregisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = null;
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x000E9760 File Offset: 0x000E7960
	public static void ElevatorButtonPressed(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		if (GRElevatorManager._instance != null)
		{
			GRElevatorManager._instance.ElevatorButtonPressedInternal(type, location);
			if (!GRElevatorManager._instance.IsMine && NetworkSystem.Instance.InRoom)
			{
				GRElevatorManager._instance.photonView.RPC("RemoteElevatorButtonPress", 2, new object[]
				{
					(int)type,
					(int)location
				});
			}
		}
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x000E97CB File Offset: 0x000E79CB
	private void ElevatorButtonPressedInternal(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		this.elevatorByLocation[location].PressButtonVisuals(type);
		this.elevatorByLocation[location].PlayButtonPress();
		if (base.IsMine)
		{
			this.ProcessElevatorButtonPress(type, location);
		}
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x000E9800 File Offset: 0x000E7A00
	public void ProcessElevatorButtonPress(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		switch (type)
		{
		case GRElevator.ButtonType.Stump:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.Stump);
				return;
			}
			break;
		case GRElevator.ButtonType.City:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.City);
				return;
			}
			break;
		case GRElevator.ButtonType.GhostReactor:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.GhostReactor);
				return;
			}
			break;
		case GRElevator.ButtonType.Open:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				if (this.currentState == GRElevatorManager.ElevatorSystemState.DestinationPressed)
				{
					if (this.GetTime() >= this.maxDoorClosingTime + (double)this.doorMaxClosingDelay)
					{
						break;
					}
					this.destinationButtonLastPressedTime = this.GetTime();
					this.doorsFullyClosedTime = this.GetTime();
				}
				this.OpenElevator(location);
				return;
			}
			break;
		case GRElevator.ButtonType.Close:
			this.CloseAllElevators();
			break;
		case GRElevator.ButtonType.Summon:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport && this.currentState != GRElevatorManager.ElevatorSystemState.DestinationPressed)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, location);
				return;
			}
			break;
		case GRElevator.ButtonType.MonkeBlocks:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.MonkeBlocks);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x000E98EC File Offset: 0x000E7AEC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.doorsFullyClosedTime);
		stream.SendNext(this.destinationButtonLastPressedTime);
		stream.SendNext(this.maxDoorClosingTime);
		stream.SendNext((int)this.currentLocation);
		stream.SendNext((int)this.destination);
		stream.SendNext((int)this.currentState);
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			stream.SendNext((int)this.allElevators[i].state);
		}
		for (int j = 0; j < this.allShuttles.Count; j++)
		{
			stream.SendNext((byte)this.allShuttles[j].GetState());
			bool flag = this.allShuttles[j].specificDestinationShuttle == null;
			NetPlayer owner = this.allShuttles[j].GetOwner();
			int num = (!flag || owner == null) ? -1 : owner.ActorNumber;
			stream.SendNext(num);
		}
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x000E9A08 File Offset: 0x000E7C08
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		double num = (double)stream.ReceiveNext();
		if (!double.IsNaN(num) && !double.IsInfinity(num))
		{
			this.doorsFullyClosedTime = num;
		}
		num = (double)stream.ReceiveNext();
		if (!double.IsNaN(num) && !double.IsInfinity(num))
		{
			this.destinationButtonLastPressedTime = num;
		}
		num = (double)stream.ReceiveNext();
		if (!double.IsNaN(num) && !double.IsInfinity(num))
		{
			this.maxDoorClosingTime = num;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation = this.currentLocation;
		int num2 = (int)stream.ReceiveNext();
		if (num2 >= 0 && num2 <= 4)
		{
			this.currentLocation = (GRElevatorManager.ElevatorLocation)num2;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation2 = this.destination;
		num2 = (int)stream.ReceiveNext();
		if (num2 >= 0 && num2 <= 4)
		{
			this.destination = (GRElevatorManager.ElevatorLocation)num2;
		}
		num2 = (int)stream.ReceiveNext();
		if (num2 >= 0 && num2 < 5)
		{
			this.currentState = (GRElevatorManager.ElevatorSystemState)num2;
		}
		this.UpdateUI();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			num2 = (int)stream.ReceiveNext();
			if (num2 >= 0 && num2 < 8)
			{
				this.allElevators[i].UpdateRemoteState((GRElevator.ElevatorState)num2);
			}
		}
		for (int j = 0; j < this.allShuttles.Count; j++)
		{
			byte b = (byte)stream.ReceiveNext();
			int num3 = (int)stream.ReceiveNext();
			if (b >= 0 && b < 7)
			{
				this.allShuttles[j].SetState((GRShuttleState)b, false);
			}
			if (this.allShuttles[j].specificDestinationShuttle == null && num3 != -1)
			{
				NetPlayer owner = NetPlayer.Get(num3);
				this.allShuttles[j].SetOwner(owner);
			}
		}
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000E9BB8 File Offset: 0x000E7DB8
	[PunRPC]
	public void RemoteElevatorButtonPress(int elevatorButtonPressed, int elevatorLocation, PhotonMessageInfo info)
	{
		if (!base.IsMine || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteElevatorButtonPress))
		{
			return;
		}
		if (elevatorLocation < 0 || elevatorLocation >= 4)
		{
			return;
		}
		if (elevatorButtonPressed < 0 || elevatorButtonPressed >= 8)
		{
			return;
		}
		this.ElevatorButtonPressedInternal((GRElevator.ButtonType)elevatorButtonPressed, (GRElevatorManager.ElevatorLocation)elevatorLocation);
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x000E9BEC File Offset: 0x000E7DEC
	[PunRPC]
	public void RemoteActivateTeleport(int elevatorStartLocation, int elevatorDestinationLocation, int lowestActorNumber, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteActivateTeleport))
		{
			return;
		}
		if (elevatorStartLocation < 0 || elevatorStartLocation >= 4 || elevatorDestinationLocation < 0 || elevatorDestinationLocation >= 4)
		{
			return;
		}
		if (!this.waitingForRemoteTeleport)
		{
			base.StartCoroutine(this.TeleportDelay((GRElevatorManager.ElevatorLocation)elevatorStartLocation, (GRElevatorManager.ElevatorLocation)elevatorDestinationLocation, lowestActorNumber, info.SentServerTime));
		}
	}

	// Token: 0x06002B88 RID: 11144 RVA: 0x000E9C46 File Offset: 0x000E7E46
	private IEnumerator TeleportDelay(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double sentServerTime)
	{
		this.timeLastTeleported = (double)Time.time;
		this.waitingForRemoteTeleport = true;
		this.lastTeleportSource = start;
		yield return new WaitForSeconds((float)(PhotonNetwork.Time - (sentServerTime + 0.75)));
		this.RefreshTeleportingPlayersJoinTime();
		yield return new WaitForSeconds(0.25f);
		this.waitingForRemoteTeleport = false;
		this.ActivateTeleport(start, destination, lowestActorNumber, sentServerTime);
		yield break;
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x000E9C74 File Offset: 0x000E7E74
	public void ActivateTeleport(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double photonServerTime)
	{
		GRElevator grelevator = this.elevatorByLocation[start];
		GRElevator grelevator2 = this.elevatorByLocation[destination];
		if (grelevator == null || grelevator2 == null)
		{
			return;
		}
		grelevator.friendCollider.RefreshPlayersInSphere();
		if (!PhotonNetwork.InRoom)
		{
			this.RefreshTeleportingPlayersJoinTime();
		}
		if (!grelevator.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
			return;
		}
		this.elevatorByLocation[destination].collidersAndVisuals.SetActive(true);
		float num = grelevator2.transform.rotation.eulerAngles.y - grelevator.transform.rotation.eulerAngles.y;
		GTPlayer instance = GTPlayer.Instance;
		VRRig localRig = VRRig.LocalRig;
		Vector3 vector = localRig.transform.position - instance.transform.position;
		Vector3 vector2 = instance.headCollider.transform.position - instance.transform.position;
		Vector3 vector3 = grelevator2.transform.TransformPoint(grelevator.transform.InverseTransformPoint(instance.transform.position));
		Vector3 vector4 = localRig.transform.position - grelevator.transform.position;
		vector4.x *= 0.8f;
		vector4.z *= 0.8f;
		vector3 = grelevator2.transform.position + (Quaternion.Euler(0f, num, 0f) * vector4 - vector) + localRig.headConstraint.rotation * localRig.head.trackingPositionOffset;
		Vector3 vector5 = Vector3.zero;
		Vector3 vector6 = grelevator2.transform.position + (Quaternion.Euler(0f, num, 0f) * vector4 - vector) + vector2 - grelevator2.transform.position;
		float magnitude = vector6.magnitude;
		vector6 = vector6.normalized;
		if (Physics.SphereCastNonAlloc(grelevator2.transform.position, instance.headCollider.radius * 1.5f, vector6, this.correctionRaycastHit, magnitude * 1.05f, this.correctionRaycastMask) > 0)
		{
			vector5 = vector6 * instance.headCollider.radius * -1.5f;
		}
		instance.TeleportTo(vector3 + vector5, instance.transform.rotation, false, false);
		instance.turnParent.transform.RotateAround(instance.headCollider.transform.position, base.transform.up, num);
		localRig.transform.position = instance.transform.position + vector;
		instance.InitializeValues();
		this.justTeleported = true;
		instance.disableMovement = true;
		GorillaComputer.instance.allowedMapsToJoin = this.elevatorByLocation[destination].joinTrigger.myCollider.myAllowedMapsToJoin;
		this.lastTeleportSource = start;
		this.lastLowestActorNr = lowestActorNumber;
		if (!this.InPrivateRoom && lowestActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.LeadElevatorJoin();
		}
		this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
		grelevator2.PlayElevatorMusic(grelevator.musicAudio.time);
	}

	// Token: 0x06002B8A RID: 11146 RVA: 0x000E9FE0 File Offset: 0x000E81E0
	public void CloseAllElevators()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			if (!this.allElevators[i].DoorIsClosing())
			{
				this.allElevators[i].UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
			}
		}
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x000EA028 File Offset: 0x000E8228
	public void OpenElevator(GRElevatorManager.ElevatorLocation location)
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].UpdateLocalState((this.allElevators[i].location == location) ? GRElevator.ElevatorState.DoorBeginOpening : GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x000EA074 File Offset: 0x000E8274
	public double GetTime()
	{
		double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
		if (this.doorsFullyClosedTime > num || this.destinationButtonLastPressedTime > num || this.maxDoorClosingTime > num || num - this.doorsFullyClosedTime > 10.0 || num - this.destinationButtonLastPressedTime > 10.0 || num - this.maxDoorClosingTime > 20.0)
		{
			this.doorsFullyClosedTime = num;
			this.destinationButtonLastPressedTime = num;
			this.maxDoorClosingTime = num;
		}
		return num;
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x000EA100 File Offset: 0x000E8300
	public static bool ValidElevatorNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (RoomSystem.WasRoomPrivate)
		{
			return false;
		}
		if (actorNr == GRElevatorManager._instance.lastLowestActorNr)
		{
			return true;
		}
		if (GRElevatorManager._instance.lastTeleportSource == GRElevatorManager.ElevatorLocation.None)
		{
			return false;
		}
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.lastTeleportSource].friendCollider;
		if ((double)Time.time < GRElevatorManager._instance.timeLastTeleported + 3.0)
		{
			friendCollider.RefreshPlayersInSphere();
			friendCollider2.RefreshPlayersInSphere();
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		return netPlayer != null && (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId));
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x000EA1DC File Offset: 0x000E83DC
	public static bool ValidShuttleNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (RoomSystem.WasRoomPrivate)
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(actorNr);
		if (grplayer == null)
		{
			return false;
		}
		GRShuttle shuttle = GRElevatorManager.GetShuttle(grplayer.shuttleData.currShuttleId);
		GRShuttle shuttle2 = GRElevatorManager.GetShuttle(grplayer.shuttleData.targetShuttleId);
		if (shuttle == null)
		{
			return false;
		}
		if (shuttle2 == null)
		{
			shuttle2 = GRElevatorManager.GetShuttle(GRShuttle.CalcTargetShuttleId(grplayer.shuttleData.currShuttleId, grplayer.shuttleData.ownerUserId));
			if (shuttle2 == null)
			{
				return false;
			}
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		if (netPlayer == null)
		{
			return false;
		}
		if (netPlayer == shuttle.GetOwner())
		{
			return true;
		}
		GorillaFriendCollider friendCollider = shuttle2.friendCollider;
		GorillaFriendCollider friendCollider2 = shuttle.friendCollider;
		friendCollider.RefreshPlayersInSphere();
		friendCollider2.RefreshPlayersInSphere();
		return friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x000EA2D0 File Offset: 0x000E84D0
	public static bool IsPlayerInShuttle(int actorNr, GRShuttle currShuttle, GRShuttle targetShuttle)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		if (netPlayer == null)
		{
			return false;
		}
		bool flag = false;
		if (currShuttle != null)
		{
			GorillaFriendCollider friendCollider = currShuttle.friendCollider;
			if (friendCollider != null)
			{
				friendCollider.RefreshPlayersInSphere();
			}
			flag = friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
		}
		bool flag2 = false;
		if (targetShuttle != null)
		{
			GorillaFriendCollider friendCollider2 = targetShuttle.friendCollider;
			if (friendCollider2 != null)
			{
				friendCollider2.RefreshPlayersInSphere();
			}
			friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
		}
		return flag || flag2;
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x000EA368 File Offset: 0x000E8568
	public static int LowestActorNumberInElevator()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		friendCollider.RefreshPlayersInSphere();
		friendCollider2.RefreshPlayersInSphere();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x06002B91 RID: 11153 RVA: 0x000EA428 File Offset: 0x000E8628
	public static int LowestActorNumberInElevator(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider)
	{
		sourceFriendCollider.RefreshPlayersInSphere();
		destinationFriendCollider.RefreshPlayersInSphere();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || destinationFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x06002B92 RID: 11154 RVA: 0x000EA4A0 File Offset: 0x000E86A0
	private void RefreshTeleportingPlayersJoinTime()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		this.actorIds.Clear();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			RigContainer rigContainer;
			if (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) && VRRigCache.Instance.TryGetVrrig(allNetPlayers[i], out rigContainer))
			{
				rigContainer.Rig.ResetTimeSpawned();
			}
		}
	}

	// Token: 0x06002B93 RID: 11155 RVA: 0x000EA521 File Offset: 0x000E8721
	public static bool InControlOfElevator()
	{
		return !NetworkSystem.Instance.InRoom || GRElevatorManager._instance.IsMine;
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x000EA53B File Offset: 0x000E873B
	public static void JoinPublicRoom()
	{
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].joinTrigger, JoinType.Solo, null);
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x000EA56C File Offset: 0x000E876C
	public void OnReachedDestination()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnReachedDestination));
		this.elevatorByLocation[this.destination].PlayElevatorStopped();
		if (this.currentLocation == this.destination)
		{
			this.OpenElevator(this.currentLocation);
			this.elevatorByLocation[this.currentLocation].PlayDing();
		}
		this.waitForZoneLoadFallbackTimer = -1f;
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x000EA5F0 File Offset: 0x000E87F0
	public static GRShuttle GetShuttle(int shuttleId)
	{
		if (GRElevatorManager._instance == null)
		{
			return null;
		}
		return GRElevatorManager._instance.GetShuttleById(shuttleId);
	}

	// Token: 0x06002B97 RID: 11159 RVA: 0x000EA60C File Offset: 0x000E880C
	public void InitShuttles(GhostReactor reactor)
	{
		for (int i = 0; i < this.allShuttles.Count; i++)
		{
			this.allShuttles[i].SetReactor(reactor);
		}
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x000EA644 File Offset: 0x000E8844
	public GRShuttle GetPlayerShuttle(GRShuttleGroupLoc shuttleGroupLoc, int shuttleIndex)
	{
		int i = 0;
		while (i < this.shuttleGroups.Count)
		{
			if (this.shuttleGroups[i].location == shuttleGroupLoc)
			{
				if (shuttleIndex < 0 || shuttleIndex >= this.shuttleGroups[i].ghostReactorStagingShuttles.Count)
				{
					Debug.LogErrorFormat("Invalid Shuttle Index {0} of {1}", new object[]
					{
						shuttleIndex,
						this.shuttleGroups[i].ghostReactorStagingShuttles.Count
					});
					return null;
				}
				return this.shuttleGroups[i].ghostReactorStagingShuttles[shuttleIndex];
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x000EA6F0 File Offset: 0x000E88F0
	public GRShuttle GetDrillShuttleForPlayer(int actorNumber)
	{
		return this.GetShuttleForPlayer(actorNumber, GRShuttleGroupLoc.Drill);
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x000EA6FA File Offset: 0x000E88FA
	public GRShuttle GetStagingShuttleForPlayer(int actorNumber)
	{
		return this.GetShuttleForPlayer(actorNumber, GRShuttleGroupLoc.Staging);
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x000EA704 File Offset: 0x000E8904
	public GRShuttle GetShuttleForPlayer(int actorNumber, GRShuttleGroupLoc shuttleGroupLoc)
	{
		for (int i = 0; i < this.shuttleGroups.Count; i++)
		{
			if (this.shuttleGroups[i].location == shuttleGroupLoc)
			{
				for (int j = 0; j < this.shuttleGroups[i].ghostReactorStagingShuttles.Count; j++)
				{
					GRShuttle grshuttle = this.shuttleGroups[i].ghostReactorStagingShuttles[j];
					if (!(grshuttle == null))
					{
						NetPlayer owner = grshuttle.GetOwner();
						if (owner != null && owner.ActorNumber == actorNumber)
						{
							return grshuttle;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x000EA794 File Offset: 0x000E8994
	public GRShuttle GetShuttleById(int shuttleId)
	{
		for (int i = 0; i < this.allShuttles.Count; i++)
		{
			if (this.allShuttles[i].shuttleId == shuttleId)
			{
				return this.allShuttles[i];
			}
		}
		return null;
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x000EA7DC File Offset: 0x000E89DC
	private int AddPlayer(NetPlayer netPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return -1;
		}
		int num = -1;
		List<GRShuttle> ghostReactorStagingShuttles = this.shuttleGroups[0].ghostReactorStagingShuttles;
		for (int i = 0; i < ghostReactorStagingShuttles.Count; i++)
		{
			if (ghostReactorStagingShuttles[i].GetOwner() == null)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return -1;
		}
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			this.shuttleGroups[j].ghostReactorStagingShuttles[num].SetOwner(netPlayer);
		}
		return num;
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x000EA864 File Offset: 0x000E8A64
	private void RemovePlayer(NetPlayer netPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		int num = -1;
		List<GRShuttle> ghostReactorStagingShuttles = this.shuttleGroups[0].ghostReactorStagingShuttles;
		for (int i = 0; i < ghostReactorStagingShuttles.Count; i++)
		{
			if (ghostReactorStagingShuttles[i].GetOwner() == netPlayer)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return;
		}
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			this.shuttleGroups[j].ghostReactorStagingShuttles[num].SetOwner(null);
		}
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x000EA8EC File Offset: 0x000E8AEC
	public void OnLeftRoom()
	{
		for (int i = 0; i < this.shuttleGroups.Count; i++)
		{
			for (int j = 0; j < this.shuttleGroups[i].ghostReactorStagingShuttles.Count; j++)
			{
				GRShuttle grshuttle = this.shuttleGroups[i].ghostReactorStagingShuttles[j];
				if (!(grshuttle == null))
				{
					grshuttle.SetOwner(null);
				}
			}
		}
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x000EA958 File Offset: 0x000E8B58
	public void OnPlayerAdded(NetPlayer player)
	{
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			return;
		}
		this.AddPlayer(player);
	}

	// Token: 0x06002BA1 RID: 11169 RVA: 0x000EA977 File Offset: 0x000E8B77
	public void OnPlayerRemoved(NetPlayer player)
	{
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			return;
		}
		this.RemovePlayer(player);
	}

	// Token: 0x06002BA2 RID: 11170 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04003805 RID: 14341
	public PhotonView photonView;

	// Token: 0x04003806 RID: 14342
	public static GRElevatorManager _instance;

	// Token: 0x04003807 RID: 14343
	public Dictionary<GRElevatorManager.ElevatorLocation, GRElevator> elevatorByLocation;

	// Token: 0x04003808 RID: 14344
	public List<GRElevator> allElevators;

	// Token: 0x04003809 RID: 14345
	[SerializeField]
	private GRElevatorManager.ElevatorLocation destination;

	// Token: 0x0400380A RID: 14346
	[SerializeField]
	private GRElevatorManager.ElevatorLocation currentLocation;

	// Token: 0x0400380B RID: 14347
	private GRElevatorManager.ElevatorLocation lastTeleportSource = GRElevatorManager.ElevatorLocation.None;

	// Token: 0x0400380C RID: 14348
	public GRElevatorManager.ElevatorSystemState currentState;

	// Token: 0x0400380D RID: 14349
	private double timeLastTeleported;

	// Token: 0x0400380E RID: 14350
	private bool cosmeticsInitialized;

	// Token: 0x0400380F RID: 14351
	[SerializeField]
	private List<GRElevatorManager.GRShuttleGroup> shuttleGroups;

	// Token: 0x04003810 RID: 14352
	public GRShuttle mainStagingShuttle;

	// Token: 0x04003811 RID: 14353
	public GRShuttle mainDrillShuttle;

	// Token: 0x04003812 RID: 14354
	private List<GRShuttle> allShuttles;

	// Token: 0x04003813 RID: 14355
	public float destinationButtonlastPressedDelay = 3f;

	// Token: 0x04003814 RID: 14356
	public float doorsFullyClosedDelay = 3f;

	// Token: 0x04003815 RID: 14357
	public float doorMaxClosingDelay = 12f;

	// Token: 0x04003816 RID: 14358
	public double destinationButtonLastPressedTime;

	// Token: 0x04003817 RID: 14359
	public double doorsFullyClosedTime;

	// Token: 0x04003818 RID: 14360
	public double maxDoorClosingTime;

	// Token: 0x04003819 RID: 14361
	private List<int> actorIds;

	// Token: 0x0400381A RID: 14362
	public CallLimitersList<CallLimiter, GRElevatorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GRElevatorManager.RPC>();

	// Token: 0x0400381B RID: 14363
	private bool justTeleported;

	// Token: 0x0400381C RID: 14364
	private bool waitingForRemoteTeleport;

	// Token: 0x0400381D RID: 14365
	private int lastLowestActorNr;

	// Token: 0x0400381E RID: 14366
	private RaycastHit[] correctionRaycastHit = new RaycastHit[1];

	// Token: 0x0400381F RID: 14367
	public LayerMask correctionRaycastMask;

	// Token: 0x04003820 RID: 14368
	private float waitForZoneLoadFallbackTimer;

	// Token: 0x04003821 RID: 14369
	public float waitForZoneLoadFallbackMaxTime = 5f;

	// Token: 0x020006A7 RID: 1703
	[Serializable]
	public class GRShuttleGroup
	{
		// Token: 0x04003823 RID: 14371
		public GRShuttleGroupLoc location;

		// Token: 0x04003824 RID: 14372
		public List<GRShuttle> ghostReactorStagingShuttles;
	}

	// Token: 0x020006A8 RID: 1704
	public enum ElevatorSystemState
	{
		// Token: 0x04003826 RID: 14374
		Dormant,
		// Token: 0x04003827 RID: 14375
		InLocation,
		// Token: 0x04003828 RID: 14376
		DestinationPressed,
		// Token: 0x04003829 RID: 14377
		WaitingToTeleport,
		// Token: 0x0400382A RID: 14378
		Teleporting,
		// Token: 0x0400382B RID: 14379
		None
	}

	// Token: 0x020006A9 RID: 1705
	public enum RPC
	{
		// Token: 0x0400382D RID: 14381
		RemoteElevatorButtonPress,
		// Token: 0x0400382E RID: 14382
		RemoteActivateTeleport
	}

	// Token: 0x020006AA RID: 1706
	public enum ElevatorLocation
	{
		// Token: 0x04003830 RID: 14384
		Stump,
		// Token: 0x04003831 RID: 14385
		City,
		// Token: 0x04003832 RID: 14386
		GhostReactor,
		// Token: 0x04003833 RID: 14387
		MonkeBlocks,
		// Token: 0x04003834 RID: 14388
		None
	}
}
