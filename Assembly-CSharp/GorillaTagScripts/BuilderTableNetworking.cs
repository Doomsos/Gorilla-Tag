using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DD0 RID: 3536
	public class BuilderTableNetworking : MonoBehaviourPunCallbacks, ITickSystemTick
	{
		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x060057A0 RID: 22432 RVA: 0x001BF1BF File Offset: 0x001BD3BF
		// (set) Token: 0x060057A1 RID: 22433 RVA: 0x001BF1C7 File Offset: 0x001BD3C7
		public bool TickRunning { get; set; }

		// Token: 0x060057A2 RID: 22434 RVA: 0x001BF1D0 File Offset: 0x001BD3D0
		private void Awake()
		{
			this.masterClientTableInit = new List<BuilderTableNetworking.PlayerTableInitState>(10);
			this.masterClientTableValidators = new List<BuilderTableNetworking.PlayerTableInitState>(10);
			this.localClientTableInit = new BuilderTableNetworking.PlayerTableInitState();
			this.localValidationTable = new BuilderTableNetworking.PlayerTableInitState();
			this.callLimiters = new CallLimiter[26];
			this.callLimiters[0] = new CallLimiter(20, 30f, 0.5f);
			this.callLimiters[1] = new CallLimiter(200, 1f, 0.5f);
			this.callLimiters[2] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[3] = new CallLimiter(2, 1f, 0.5f);
			this.callLimiters[4] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[5] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[6] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[7] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[8] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[9] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[10] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[11] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[12] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[13] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[14] = new CallLimiter(100, 1f, 0.5f);
			this.callLimiters[15] = new CallLimiter(100, 1f, 0.5f);
			this.callLimiters[16] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[17] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[18] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[19] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[20] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[21] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[22] = new CallLimiter(20, 1f, 0.5f);
			this.callLimiters[23] = new CallLimiter(20, 1f, 0.5f);
			this.callLimiters[24] = new CallLimiter(3, 30f, 0.5f);
			this.callLimiters[25] = new CallLimiter(10, 1f, 0.5f);
			this.armShelfRequests = new List<Player>(10);
		}

		// Token: 0x060057A3 RID: 22435 RVA: 0x001BF4C3 File Offset: 0x001BD6C3
		private void OnEnable()
		{
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060057A4 RID: 22436 RVA: 0x001BF4D1 File Offset: 0x001BD6D1
		private void OnDisable()
		{
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060057A5 RID: 22437 RVA: 0x001BF4DF File Offset: 0x001BD6DF
		public void SetTable(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x060057A6 RID: 22438 RVA: 0x001BF4E8 File Offset: 0x001BD6E8
		private BuilderTable GetTable()
		{
			return this.currTable;
		}

		// Token: 0x060057A7 RID: 22439 RVA: 0x001BF4F0 File Offset: 0x001BD6F0
		private int CreateLocalCommandId()
		{
			int result = this.nextLocalCommandId;
			this.nextLocalCommandId++;
			return result;
		}

		// Token: 0x060057A8 RID: 22440 RVA: 0x001BF506 File Offset: 0x001BD706
		public BuilderTableNetworking.PlayerTableInitState GetLocalTableInit()
		{
			return this.localClientTableInit;
		}

		// Token: 0x060057A9 RID: 22441 RVA: 0x001BF510 File Offset: 0x001BD710
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (!newMasterClient.IsLocal)
			{
				this.localClientTableInit.Reset();
				BuilderTable table = this.GetTable();
				if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
				{
					if (table.GetTableState() == BuilderTable.TableState.Ready)
					{
						table.SetTableState(BuilderTable.TableState.WaitForMasterResync);
					}
					else if (table.GetTableState() == BuilderTable.TableState.WaitForMasterResync || table.GetTableState() == BuilderTable.TableState.ReceivingMasterResync)
					{
						table.SetTableState(BuilderTable.TableState.WaitForMasterResync);
					}
					else
					{
						table.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
					}
					this.PlayerEnterBuilder();
				}
				return;
			}
			this.masterClientTableInit.Clear();
			this.localClientTableInit.Reset();
			BuilderTable table2 = this.GetTable();
			BuilderTable.TableState tableState = table2.GetTableState();
			bool flag = (tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.WaitingForZoneAndRoom && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.ReceivingMasterResync) || table2.pieces.Count <= 0;
			if (!flag)
			{
				flag |= (table2.pieces.Count <= 0);
			}
			if (flag)
			{
				table2.ClearTable();
				table2.ClearQueuedCommands();
				table2.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
				return;
			}
			for (int i = 0; i < table2.pieces.Count; i++)
			{
				BuilderPiece builderPiece = table2.pieces[i];
				Player player = PhotonNetwork.CurrentRoom.GetPlayer(builderPiece.heldByPlayerActorNumber, false);
				if (table2.pieces[i].state == BuilderPiece.State.Grabbed && player == null)
				{
					Vector3 position = builderPiece.transform.position;
					Quaternion rotation = builderPiece.transform.rotation;
					Debug.LogErrorFormat("We have a piece {0} {1} held by an invalid player {2} dropping", new object[]
					{
						builderPiece.name,
						builderPiece.pieceId,
						builderPiece.heldByPlayerActorNumber
					});
					this.CreateLocalCommandId();
					builderPiece.ClearParentHeld();
					builderPiece.ClearParentPiece(false);
					builderPiece.transform.localScale = Vector3.one;
					builderPiece.SetState(BuilderPiece.State.Dropped, false);
					builderPiece.transform.SetLocalPositionAndRotation(position, rotation);
					if (builderPiece.rigidBody != null)
					{
						builderPiece.rigidBody.position = position;
						builderPiece.rigidBody.rotation = rotation;
						builderPiece.rigidBody.linearVelocity = Vector3.zero;
						builderPiece.rigidBody.angularVelocity = Vector3.zero;
					}
				}
			}
			table2.ClearQueuedCommands();
			table2.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x060057AA RID: 22442 RVA: 0x001BF74C File Offset: 0x001BD94C
		public override void OnPlayerLeftRoom(Player player)
		{
			Debug.LogFormat("Player {0} left room", new object[]
			{
				player.ActorNumber
			});
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				if (table.isTableMutable)
				{
					if (!PhotonNetwork.IsMasterClient)
					{
						table.DropAllPiecesForPlayerLeaving(player.ActorNumber);
					}
					else
					{
						table.RecycleAllPiecesForPlayerLeaving(player.ActorNumber);
					}
				}
				table.PlayerLeftRoom(player.ActorNumber);
			}
			if (!table.isTableMutable && table.linkedTerminal != null && table.linkedTerminal.IsPlayerDriver(player))
			{
				table.linkedTerminal.ResetTerminalControl();
				if (NetworkSystem.Instance.IsMasterClient)
				{
					base.photonView.RPC("SetBlocksTerminalDriverRPC", 0, new object[]
					{
						-2
					});
				}
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			table.RemoveArmShelfForPlayer(player);
			table.VerifySetSelections();
			if (player != PhotonNetwork.LocalPlayer)
			{
				this.DestroyPlayerTableInit(player);
			}
		}

		// Token: 0x060057AB RID: 22443 RVA: 0x001BF83B File Offset: 0x001BDA3B
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			BuilderTable table = this.GetTable();
			table.SetPendingMap(null);
			table.SetInRoom(true);
		}

		// Token: 0x060057AC RID: 22444 RVA: 0x001BF856 File Offset: 0x001BDA56
		public override void OnLeftRoom()
		{
			this.PlayerExitBuilder();
			BuilderTable table = this.GetTable();
			table.SetPendingMap(null);
			table.SetInRoom(false);
			this.armShelfRequests.Clear();
		}

		// Token: 0x060057AD RID: 22445 RVA: 0x001BF87C File Offset: 0x001BDA7C
		public void Tick()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				this.UpdateNewPlayerInit();
			}
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x001BF88C File Offset: 0x001BDA8C
		public void PlayerEnterBuilder()
		{
			this.tablePhotonView.RPC("PlayerEnterBuilderRPC", PhotonNetwork.MasterClient, new object[]
			{
				PhotonNetwork.LocalPlayer,
				true
			});
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x060057AF RID: 22447 RVA: 0x001BF900 File Offset: 0x001BDB00
		[PunRPC]
		public void PlayerEnterBuilderRPC(Player player, bool entered, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerEnterBuilderRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlayerEnterMaster, info))
			{
				return;
			}
			if (player == null || !player.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (entered)
			{
				BuilderTable.TableState tableState = table.GetTableState();
				if (tableState == BuilderTable.TableState.WaitingForInitalBuild || (this.IsPrivateMasterClient() && tableState == BuilderTable.TableState.WaitingForZoneAndRoom))
				{
					table.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
				}
				if (player != PhotonNetwork.LocalPlayer)
				{
					this.CreateSerializedTableForNewPlayerInit(player);
				}
				if (table.isTableMutable)
				{
					this.RequestCreateArmShelfForPlayer(player);
					return;
				}
				if (table.linkedTerminal != null)
				{
					base.photonView.RPC("SetBlocksTerminalDriverRPC", player, new object[]
					{
						table.linkedTerminal.GetDriverID
					});
					return;
				}
			}
			else
			{
				if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.DestroyPlayerTableInit(player);
				}
				if (table.isTableMutable)
				{
					table.RemoveArmShelfForPlayer(player);
				}
			}
		}

		// Token: 0x060057B0 RID: 22448 RVA: 0x001BF9E4 File Offset: 0x001BDBE4
		public void PlayerExitBuilder()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.tablePhotonView.RPC("PlayerEnterBuilderRPC", PhotonNetwork.MasterClient, new object[]
				{
					PhotonNetwork.LocalPlayer,
					false
				});
			}
			BuilderTable table = this.GetTable();
			table.ClearTable();
			table.ClearQueuedCommands();
			this.localClientTableInit.Reset();
			this.armShelfRequests.Clear();
			this.masterClientTableInit.Clear();
		}

		// Token: 0x060057B1 RID: 22449 RVA: 0x001BFA5B File Offset: 0x001BDC5B
		public bool IsPrivateMasterClient()
		{
			return PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient && NetworkSystem.Instance.SessionIsPrivate;
		}

		// Token: 0x060057B2 RID: 22450 RVA: 0x001BFA78 File Offset: 0x001BDC78
		private void UpdateNewPlayerInit()
		{
			if (this.GetTable().GetTableState() == BuilderTable.TableState.Ready)
			{
				for (int i = 0; i < this.masterClientTableInit.Count; i++)
				{
					if (this.masterClientTableInit[i].waitForInitTimeRemaining >= 0f)
					{
						this.masterClientTableInit[i].waitForInitTimeRemaining -= Time.deltaTime;
						if (this.masterClientTableInit[i].waitForInitTimeRemaining <= 0f)
						{
							this.StartCreatingSerializedTable(this.masterClientTableInit[i].player);
							this.masterClientTableInit[i].waitForInitTimeRemaining = -1f;
							this.masterClientTableInit[i].sendNextChunkTimeRemaining = 0f;
						}
					}
					else if (this.masterClientTableInit[i].sendNextChunkTimeRemaining >= 0f)
					{
						this.masterClientTableInit[i].sendNextChunkTimeRemaining -= Time.deltaTime;
						if (this.masterClientTableInit[i].sendNextChunkTimeRemaining <= 0f)
						{
							this.SendNextTableData(this.masterClientTableInit[i].player);
							if (this.masterClientTableInit[i].numSerializedBytes < this.masterClientTableInit[i].totalSerializedBytes)
							{
								this.masterClientTableInit[i].sendNextChunkTimeRemaining = 0f;
							}
							else
							{
								this.masterClientTableInit[i].sendNextChunkTimeRemaining = -1f;
							}
						}
					}
				}
			}
		}

		// Token: 0x060057B3 RID: 22451 RVA: 0x001BFC08 File Offset: 0x001BDE08
		private void StartCreatingSerializedTable(Player newPlayer)
		{
			BuilderTable table = this.GetTable();
			BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(newPlayer);
			playerTableInit.totalSerializedBytes = table.SerializeTableState(playerTableInit.serializedTableState, 1048576);
			byte[] array = GZipStream.CompressBuffer(playerTableInit.serializedTableState);
			playerTableInit.totalSerializedBytes = array.Length;
			Array.Copy(array, 0, playerTableInit.serializedTableState, 0, playerTableInit.totalSerializedBytes);
			playerTableInit.numSerializedBytes = 0;
			this.tablePhotonView.RPC("StartBuildTableRPC", newPlayer, new object[]
			{
				playerTableInit.totalSerializedBytes
			});
		}

		// Token: 0x060057B4 RID: 22452 RVA: 0x001BFC90 File Offset: 0x001BDE90
		[PunRPC]
		public void StartBuildTableRPC(int totalBytes, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "StartBuildTableRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.TableDataStart, info))
			{
				return;
			}
			if (totalBytes <= 0 || totalBytes > 1048576)
			{
				Debug.LogError("Builder Table Bytes is too large: " + totalBytes.ToString());
				return;
			}
			BuilderTable table = this.GetTable();
			GTDev.Log<string>("StartBuildTableRPC with current state " + table.GetTableState().ToString(), null);
			if (table.GetTableState() != BuilderTable.TableState.WaitForMasterResync && table.GetTableState() != BuilderTable.TableState.WaitingForInitalBuild)
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.WaitForMasterResync)
			{
				table.SetTableState(BuilderTable.TableState.ReceivingMasterResync);
			}
			else
			{
				table.SetTableState(BuilderTable.TableState.ReceivingInitialBuild);
			}
			this.localClientTableInit.Reset();
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.localClientTableInit;
			playerTableInitState.player = PhotonNetwork.LocalPlayer;
			playerTableInitState.totalSerializedBytes = totalBytes;
			table.ClearQueuedCommands();
		}

		// Token: 0x060057B5 RID: 22453 RVA: 0x001BFD6C File Offset: 0x001BDF6C
		private void SendNextTableData(Player requestingPlayer)
		{
			BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(requestingPlayer);
			if (playerTableInit == null)
			{
				Debug.LogErrorFormat("No Table init found for player {0}", new object[]
				{
					requestingPlayer.ActorNumber
				});
				return;
			}
			int num = Mathf.Min(1000, playerTableInit.totalSerializedBytes - playerTableInit.numSerializedBytes);
			if (num <= 0)
			{
				return;
			}
			Array.Copy(playerTableInit.serializedTableState, playerTableInit.numSerializedBytes, playerTableInit.chunk, 0, num);
			playerTableInit.numSerializedBytes += num;
			this.tablePhotonView.RPC("SendTableDataRPC", requestingPlayer, new object[]
			{
				num,
				playerTableInit.chunk
			});
		}

		// Token: 0x060057B6 RID: 22454 RVA: 0x001BFE10 File Offset: 0x001BE010
		[PunRPC]
		public void SendTableDataRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SendTableDataRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (numBytes <= 0 || numBytes > 1000 || numBytes > bytes.Length)
			{
				Debug.LogErrorFormat("Builder Table Send Data numBytes is too large {0}", new object[]
				{
					numBytes
				});
				return;
			}
			if (bytes.Length > 1000)
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.TableData, info))
			{
				return;
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.localClientTableInit;
			if (playerTableInitState.numSerializedBytes + numBytes > 1048576)
			{
				Debug.LogErrorFormat("Builder Table serialized bytes is larger than buffer {0}", new object[]
				{
					playerTableInitState.numSerializedBytes + numBytes
				});
				return;
			}
			Array.Copy(bytes, 0, playerTableInitState.serializedTableState, playerTableInitState.numSerializedBytes, numBytes);
			playerTableInitState.numSerializedBytes += numBytes;
			if (playerTableInitState.numSerializedBytes >= playerTableInitState.totalSerializedBytes)
			{
				this.GetTable().SetTableState(BuilderTable.TableState.InitialBuild);
			}
		}

		// Token: 0x060057B7 RID: 22455 RVA: 0x001BFEF4 File Offset: 0x001BE0F4
		private bool DoesTableInitExist(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060057B8 RID: 22456 RVA: 0x001BFF38 File Offset: 0x001BE138
		private BuilderTableNetworking.PlayerTableInitState CreatePlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					this.masterClientTableInit[i].Reset();
					return this.masterClientTableInit[i];
				}
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = new BuilderTableNetworking.PlayerTableInitState();
			playerTableInitState.player = player;
			this.masterClientTableInit.Add(playerTableInitState);
			return playerTableInitState;
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x001BFFB4 File Offset: 0x001BE1B4
		public void ResetSerializedTableForAllPlayers()
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				this.masterClientTableInit[i].waitForInitTimeRemaining = 1f;
				this.masterClientTableInit[i].sendNextChunkTimeRemaining = -1f;
				this.masterClientTableInit[i].numSerializedBytes = 0;
				this.masterClientTableInit[i].totalSerializedBytes = 0;
			}
		}

		// Token: 0x060057BA RID: 22458 RVA: 0x001C0027 File Offset: 0x001BE227
		private void CreateSerializedTableForNewPlayerInit(Player newPlayer)
		{
			if (this.DoesTableInitExist(newPlayer))
			{
				return;
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.CreatePlayerTableInit(newPlayer);
			playerTableInitState.waitForInitTimeRemaining = 1f;
			playerTableInitState.sendNextChunkTimeRemaining = -1f;
		}

		// Token: 0x060057BB RID: 22459 RVA: 0x001C0050 File Offset: 0x001BE250
		private void DestroyPlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					this.masterClientTableInit.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x060057BC RID: 22460 RVA: 0x001C00A4 File Offset: 0x001BE2A4
		private BuilderTableNetworking.PlayerTableInitState GetPlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					return this.masterClientTableInit[i];
				}
			}
			return null;
		}

		// Token: 0x060057BD RID: 22461 RVA: 0x001C00F4 File Offset: 0x001BE2F4
		private bool ValidateMasterClientIsReady(Player player)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return false;
			}
			if (player != null && !player.IsMasterClient)
			{
				BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(player);
				if (playerTableInit != null && playerTableInit.numSerializedBytes < playerTableInit.totalSerializedBytes)
				{
					return false;
				}
			}
			return this.GetTable().GetTableState() == BuilderTable.TableState.Ready;
		}

		// Token: 0x060057BE RID: 22462 RVA: 0x001C0144 File Offset: 0x001BE344
		private bool ValidateCallLimits(BuilderTableNetworking.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= BuilderTableNetworking.RPC.PlayerEnterMaster && rpcCall < BuilderTableNetworking.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x060057BF RID: 22463 RVA: 0x001C0172 File Offset: 0x001BE372
		[PunRPC]
		public void RequestFailedRPC(int localCommandId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestFailedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RequestFailed, info))
			{
				return;
			}
			this.GetTable().RollbackFailedCommand(localCommandId);
		}

		// Token: 0x060057C0 RID: 22464 RVA: 0x00002789 File Offset: 0x00000989
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x060057C1 RID: 22465 RVA: 0x00002789 File Offset: 0x00000989
		public void RequestCreatePieceRPC(int newPieceType, long packedPosition, int packedRotation, int materialType, PhotonMessageInfo info)
		{
		}

		// Token: 0x060057C2 RID: 22466 RVA: 0x00002789 File Offset: 0x00000989
		public void PieceCreatedRPC(int pieceType, int pieceId, long packedPosition, int packedRotation, int materialType, Player creatingPlayer, PhotonMessageInfo info)
		{
		}

		// Token: 0x060057C3 RID: 22467 RVA: 0x001C01A8 File Offset: 0x001BE3A8
		public void CreateShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, int shelfID)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			BuilderPiece piecePrefab = table.GetPiecePrefab(pieceType);
			if (!table.HasEnoughResources(piecePrefab))
			{
				Debug.Log("Not Enough Resources");
				return;
			}
			if (state != BuilderPiece.State.OnShelf)
			{
				if (state != BuilderPiece.State.OnConveyor)
				{
					return;
				}
				if (shelfID < 0 || shelfID >= table.conveyors.Count)
				{
					return;
				}
			}
			else if (shelfID < 0 || shelfID >= table.dispenserShelves.Count)
			{
				return;
			}
			int num = table.CreatePieceId();
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			base.photonView.RPC("PieceCreatedByShelfRPC", 0, new object[]
			{
				pieceType,
				num,
				num2,
				num3,
				materialType,
				(byte)state,
				shelfID,
				PhotonNetwork.LocalPlayer
			});
		}

		// Token: 0x060057C4 RID: 22468 RVA: 0x001C02A4 File Offset: 0x001BE4A4
		[PunRPC]
		public void PieceCreatedByShelfRPC(int pieceType, int pieceId, long packedPosition, int packedRotation, int materialType, byte state, int shelfID, Player creatingPlayer, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.CreateShelfPieceMaster, info))
			{
				return;
			}
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(packedPosition);
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRotation);
			if (!table.ValidatePieceWorldTransform(position, rotation))
			{
				return;
			}
			if (state == 4)
			{
				table.CreateDispenserShelfPiece(pieceType, pieceId, position, rotation, materialType, shelfID);
				return;
			}
			if (state != 7)
			{
				return;
			}
			table.CreateConveyorPiece(pieceType, pieceId, position, rotation, materialType, shelfID, info.SentServerTimestamp);
		}

		// Token: 0x060057C5 RID: 22469 RVA: 0x001C0328 File Offset: 0x001BE528
		public void RequestRecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			if (!table.isTableMutable)
			{
				return;
			}
			float num = 10000f;
			if (!position.IsValid(num) || !rotation.IsValid())
			{
				return;
			}
			if (recyclerID > 32767 || recyclerID < -1)
			{
				return;
			}
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			base.photonView.RPC("PieceDestroyedRPC", 0, new object[]
			{
				pieceId,
				num2,
				num3,
				playFX,
				(short)recyclerID
			});
		}

		// Token: 0x060057C6 RID: 22470 RVA: 0x001C03D8 File Offset: 0x001BE5D8
		[PunRPC]
		public void PieceDestroyedRPC(int pieceId, long packedPosition, int packedRotation, bool playFX, short recyclerID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceDestroyedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RecyclePieceMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(packedPosition);
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRotation);
			float num = 10000f;
			if (!position.IsValid(num) || !rotation.IsValid())
			{
				return;
			}
			table.RecyclePiece(pieceId, position, rotation, playFX, (int)recyclerID, info.Sender);
		}

		// Token: 0x060057C7 RID: 22471 RVA: 0x001C045C File Offset: 0x001BE65C
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (piece == null)
			{
				return;
			}
			int pieceId = piece.pieceId;
			int num = (parentPiece != null) ? parentPiece.pieceId : -1;
			int num2 = (attachPiece != null) ? attachPiece.pieceId : -1;
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidatePlacePieceParams(pieceId, num2, bumpOffsetX, bumpOffsetZ, twist, num, attachIndex, parentAttachIndex, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			int num3 = this.CreateLocalCommandId();
			attachPiece.requestedParentPiece = parentPiece;
			table.UpdatePieceData(attachPiece);
			table.PlacePiece(num3, pieceId, num2, bumpOffsetX, bumpOffsetZ, twist, num, attachIndex, parentAttachIndex, NetPlayer.Get(PhotonNetwork.LocalPlayer), PhotonNetwork.ServerTimestamp, true);
			int num4 = BuilderTable.PackPiecePlacement(twist, bumpOffsetX, bumpOffsetZ);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestPlacePieceRPC", 2, new object[]
				{
					num3,
					pieceId,
					num2,
					num4,
					num,
					attachIndex,
					parentAttachIndex,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x060057C8 RID: 22472 RVA: 0x001C0584 File Offset: 0x001BE784
		[PunRPC]
		public void RequestPlacePieceRPC(int localCommandId, int pieceId, int attachPieceId, int placement, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestPlacePieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlacePieceMaster, info) || placedByPlayer == null || !placedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			bool isMasterClient = info.Sender.IsMasterClient;
			byte twist;
			sbyte bumpOffsetX;
			sbyte bumpOffsetZ;
			BuilderTable.UnpackPiecePlacement(placement, out twist, out bumpOffsetX, out bumpOffsetZ);
			bool flag = isMasterClient || table.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, NetPlayer.Get(placedByPlayer));
			if (flag)
			{
				flag &= (isMasterClient || table.ValidatePlacePieceState(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer));
			}
			if (flag)
			{
				BuilderPiece piece = table.GetPiece(parentPieceId);
				BuilderPiecePrivatePlot builderPiecePrivatePlot;
				if (piece != null && piece.TryGetPlotComponent(out builderPiecePrivatePlot) && !builderPiecePrivatePlot.IsPlotClaimed())
				{
					base.photonView.RPC("PlotClaimedRPC", 0, new object[]
					{
						parentPieceId,
						placedByPlayer,
						true
					});
				}
				base.photonView.RPC("PiecePlacedRPC", 0, new object[]
				{
					localCommandId,
					pieceId,
					attachPieceId,
					placement,
					parentPieceId,
					attachIndex,
					parentAttachIndex,
					placedByPlayer,
					info.SentServerTimestamp
				});
				return;
			}
			base.photonView.RPC("RequestFailedRPC", info.Sender, new object[]
			{
				localCommandId
			});
		}

		// Token: 0x060057C9 RID: 22473 RVA: 0x001C0738 File Offset: 0x001BE938
		[PunRPC]
		public void PiecePlacedRPC(int localCommandId, int pieceId, int attachPieceId, int placement, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer, int timeStamp, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PiecePlacedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlacePiece, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (placedByPlayer == null)
			{
				return;
			}
			if ((ulong)(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout) || (ulong)(info.SentServerTimestamp - timeStamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout))
			{
				timeStamp = PhotonNetwork.ServerTimestamp;
			}
			byte twist;
			sbyte bumpOffsetX;
			sbyte bumpOffsetZ;
			BuilderTable.UnpackPiecePlacement(placement, out twist, out bumpOffsetX, out bumpOffsetZ);
			table.PlacePiece(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, NetPlayer.Get(placedByPlayer), timeStamp, false);
		}

		// Token: 0x060057CA RID: 22474 RVA: 0x001C07F0 File Offset: 0x001BE9F0
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (piece == null)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateGrabPieceParams(piece.pieceId, isLefHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				this.CheckForFreedPlot(piece.pieceId, PhotonNetwork.LocalPlayer);
			}
			int num = this.CreateLocalCommandId();
			table.GrabPiece(num, piece.pieceId, isLefHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer), true);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				long num2 = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
				base.photonView.RPC("RequestGrabPieceRPC", 2, new object[]
				{
					num,
					piece.pieceId,
					isLefHand,
					num2,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x060057CB RID: 22475 RVA: 0x001C08CC File Offset: 0x001BEACC
		[PunRPC]
		public void RequestGrabPieceRPC(int localCommandId, int pieceId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestGrabPieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.GrabPieceMaster, info) || !grabbedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 localPosition;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				bool isMasterClient = info.Sender.IsMasterClient;
				bool flag = isMasterClient || table.ValidateGrabPieceParams(pieceId, isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer));
				if (flag)
				{
					flag &= (isMasterClient || table.ValidateGrabPieceState(pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer));
				}
				if (flag)
				{
					if (!info.Sender.IsMasterClient)
					{
						this.CheckForFreedPlot(pieceId, grabbedByPlayer);
					}
					base.photonView.RPC("PieceGrabbedRPC", 0, new object[]
					{
						localCommandId,
						pieceId,
						isLeftHand,
						packedPosRot,
						grabbedByPlayer
					});
					return;
				}
				base.photonView.RPC("RequestFailedRPC", info.Sender, new object[]
				{
					localCommandId
				});
			}
		}

		// Token: 0x060057CC RID: 22476 RVA: 0x001C0A04 File Offset: 0x001BEC04
		private void CheckForFreedPlot(int pieceId, Player grabbedByPlayer)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderPiece piece = this.GetTable().GetPiece(pieceId);
			if (piece != null && piece.parentPiece != null && piece.parentPiece.IsPrivatePlot() && piece.parentPiece.firstChildPiece.Equals(piece) && piece.nextSiblingPiece == null)
			{
				base.photonView.RPC("PlotClaimedRPC", 0, new object[]
				{
					piece.parentPiece.pieceId,
					grabbedByPlayer,
					false
				});
			}
		}

		// Token: 0x060057CD RID: 22477 RVA: 0x001C0AA4 File Offset: 0x001BECA4
		[PunRPC]
		public void PieceGrabbedRPC(int localCommandId, int pieceId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceGrabbedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.GrabPiece, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 localPosition;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
			table.GrabPiece(localCommandId, pieceId, isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer), false);
		}

		// Token: 0x060057CE RID: 22478 RVA: 0x001C0B08 File Offset: 0x001BED08
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (piece == null)
			{
				return;
			}
			int pieceId = piece.pieceId;
			float num = 10000f;
			if (velocity.IsValid(num) && velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
			{
				velocity = velocity.normalized * BuilderTable.MAX_DROP_VELOCITY;
			}
			num = 10000f;
			if (angVelocity.IsValid(num) && angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
			{
				angVelocity = angVelocity.normalized * BuilderTable.MAX_DROP_ANG_VELOCITY;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateDropPieceParams(pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			int num2 = this.CreateLocalCommandId();
			table.DropPiece(num2, pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer), true);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestDropPieceRPC", 2, new object[]
				{
					num2,
					pieceId,
					position,
					rotation,
					velocity,
					angVelocity,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x060057CF RID: 22479 RVA: 0x001C0C40 File Offset: 0x001BEE40
		[PunRPC]
		public void RequestDropPieceRPC(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestDropPieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.DropPieceMaster, info) || !droppedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			bool isMasterClient = info.Sender.IsMasterClient;
			bool flag = isMasterClient || table.ValidateDropPieceParams(pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(droppedByPlayer));
			if (flag)
			{
				flag &= (isMasterClient || table.ValidateDropPieceState(pieceId, position, rotation, velocity, angVelocity, droppedByPlayer));
			}
			if (flag)
			{
				base.photonView.RPC("PieceDroppedRPC", 0, new object[]
				{
					localCommandId,
					pieceId,
					position,
					rotation,
					velocity,
					angVelocity,
					droppedByPlayer
				});
				return;
			}
			base.photonView.RPC("RequestFailedRPC", info.Sender, new object[]
			{
				localCommandId
			});
		}

		// Token: 0x060057D0 RID: 22480 RVA: 0x001C0D6C File Offset: 0x001BEF6C
		[PunRPC]
		public void PieceDroppedRPC(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceDroppedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.DropPiece, info))
			{
				return;
			}
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3))
					{
						BuilderTable table = this.GetTable();
						if (!table.isTableMutable)
						{
							return;
						}
						table.DropPiece(localCommandId, pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(droppedByPlayer), false);
						return;
					}
				}
			}
		}

		// Token: 0x060057D1 RID: 22481 RVA: 0x001C0E08 File Offset: 0x001BF008
		public void PieceEnteredDropZone(BuilderPiece piece, BuilderDropZone.DropType dropType, int dropZoneId)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			BuilderPiece rootPiece = piece.GetRootPiece();
			if (!table.ValidateRepelPiece(rootPiece))
			{
				return;
			}
			long num = BitPackUtils.PackWorldPosForNetwork(rootPiece.transform.position);
			int num2 = BitPackUtils.PackQuaternionForNetwork(rootPiece.transform.rotation);
			base.photonView.RPC("PieceEnteredDropZoneRPC", 0, new object[]
			{
				rootPiece.pieceId,
				num,
				num2,
				dropZoneId
			});
		}

		// Token: 0x060057D2 RID: 22482 RVA: 0x001C0EA0 File Offset: 0x001BF0A0
		[PunRPC]
		public void PieceEnteredDropZoneRPC(int pieceId, long position, int rotation, int dropZoneId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceEnteredDropZoneRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PieceDropZone, info))
			{
				return;
			}
			Vector3 worldPos = BitPackUtils.UnpackWorldPosFromNetwork(position);
			float num = 10000f;
			if (!worldPos.IsValid(num))
			{
				return;
			}
			Quaternion worldRot = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
			if (!worldRot.IsValid())
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			table.PieceEnteredDropZone(pieceId, worldPos, worldRot, dropZoneId);
		}

		// Token: 0x060057D3 RID: 22483 RVA: 0x001C0F1C File Offset: 0x001BF11C
		[PunRPC]
		public void PlotClaimedRPC(int pieceId, Player claimingPlayer, bool claimed, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlotClaimedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlotClaimedMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (claimed)
			{
				table.PlotClaimed(pieceId, claimingPlayer);
				return;
			}
			table.PlotFreed(pieceId, claimingPlayer);
		}

		// Token: 0x060057D4 RID: 22484 RVA: 0x001C0F78 File Offset: 0x001BF178
		public void RequestCreateArmShelfForPlayer(Player player)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				if (!this.armShelfRequests.Contains(player))
				{
					this.armShelfRequests.Add(player);
				}
				return;
			}
			if (table.playerToArmShelfLeft.ContainsKey(player.ActorNumber))
			{
				return;
			}
			int num = table.CreatePieceId();
			int num2 = table.CreatePieceId();
			int staticHash = table.armShelfPieceType.name.GetStaticHash();
			base.photonView.RPC("ArmShelfCreatedRPC", 0, new object[]
			{
				num,
				num2,
				staticHash,
				player
			});
		}

		// Token: 0x060057D5 RID: 22485 RVA: 0x001C102C File Offset: 0x001BF22C
		[PunRPC]
		public void ArmShelfCreatedRPC(int pieceIdLeft, int pieceIdRight, int pieceType, Player owningPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ArmShelfCreatedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ArmShelfCreated, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (pieceType != table.armShelfPieceType.name.GetStaticHash())
			{
				return;
			}
			table.CreateArmShelf(pieceIdLeft, pieceIdRight, pieceType, owningPlayer);
		}

		// Token: 0x060057D6 RID: 22486 RVA: 0x001C1090 File Offset: 0x001BF290
		public void RequestShelfSelection(int shelfID, int groupID, bool isConveyor)
		{
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (isConveyor)
			{
				if (shelfID < 0 || shelfID >= table.conveyors.Count)
				{
					return;
				}
			}
			else if (shelfID < 0 || shelfID >= table.dispenserShelves.Count)
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestShelfSelectionRPC", 2, new object[]
				{
					shelfID,
					groupID,
					isConveyor
				});
			}
		}

		// Token: 0x060057D7 RID: 22487 RVA: 0x001C1114 File Offset: 0x001BF314
		[PunRPC]
		public void RequestShelfSelectionRPC(int shelfId, int setId, bool isConveyor, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestShelfSelectionRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ShelfSelection, info))
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateShelfSelectionParams(shelfId, setId, isConveyor, info.Sender))
			{
				return;
			}
			base.photonView.RPC("ShelfSelectionChangedRPC", 0, new object[]
			{
				shelfId,
				setId,
				isConveyor,
				info.Sender
			});
		}

		// Token: 0x060057D8 RID: 22488 RVA: 0x001C11B4 File Offset: 0x001BF3B4
		[PunRPC]
		public void ShelfSelectionChangedRPC(int shelfId, int setId, bool isConveyor, Player caller, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ShelfSelectionChangedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ShelfSelectionMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (shelfId < 0 || ((!isConveyor || shelfId >= table.conveyors.Count) && (isConveyor || shelfId >= table.dispenserShelves.Count)))
			{
				return;
			}
			table.ChangeSetSelection(shelfId, setId, isConveyor);
		}

		// Token: 0x060057D9 RID: 22489 RVA: 0x001C1234 File Offset: 0x001BF434
		public void RequestFunctionalPieceStateChange(int pieceID, byte state)
		{
			BuilderTable table = this.GetTable();
			if (!table.ValidateFunctionalPieceState(pieceID, state, NetworkSystem.Instance.LocalPlayer))
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestFunctionalPieceStateChangeRPC", 2, new object[]
				{
					pieceID,
					state
				});
			}
		}

		// Token: 0x060057DA RID: 22490 RVA: 0x001C1290 File Offset: 0x001BF490
		[PunRPC]
		public void RequestFunctionalPieceStateChangeRPC(int pieceID, byte state, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestFunctionalPieceStateChangeRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetFunctionalState, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(info.Sender)))
			{
				table.OnFunctionalStateRequest(pieceID, state, NetPlayer.Get(info.Sender), info.SentServerTimestamp);
			}
		}

		// Token: 0x060057DB RID: 22491 RVA: 0x001C130C File Offset: 0x001BF50C
		public void FunctionalPieceStateChangeMaster(int pieceID, byte state, Player instigator, int timeStamp)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(instigator)) && state != table.GetPiece(pieceID).functionalPieceState)
			{
				base.photonView.RPC("FunctionalPieceStateChangeRPC", 0, new object[]
				{
					pieceID,
					state,
					instigator,
					timeStamp
				});
			}
		}

		// Token: 0x060057DC RID: 22492 RVA: 0x001C1380 File Offset: 0x001BF580
		[PunRPC]
		public void FunctionalPieceStateChangeRPC(int pieceID, byte state, Player caller, int timeStamp, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "FunctionalPieceStateChangeRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetFunctionalStateMaster, info))
			{
				return;
			}
			if (caller == null)
			{
				return;
			}
			if ((ulong)(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout) || (ulong)(info.SentServerTimestamp - timeStamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout))
			{
				timeStamp = PhotonNetwork.ServerTimestamp;
			}
			BuilderTable table = this.GetTable();
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(info.Sender)))
			{
				table.SetFunctionalPieceState(pieceID, state, NetPlayer.Get(caller), timeStamp);
			}
		}

		// Token: 0x060057DD RID: 22493 RVA: 0x001C142C File Offset: 0x001BF62C
		public void RequestBlocksTerminalControl(bool locked)
		{
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			if (table.linkedTerminal.IsTerminalLocked == locked)
			{
				return;
			}
			base.photonView.RPC("RequestBlocksTerminalControlRPC", 2, new object[]
			{
				locked
			});
		}

		// Token: 0x060057DE RID: 22494 RVA: 0x001C1488 File Offset: 0x001BF688
		[PunRPC]
		private void RequestBlocksTerminalControlRPC(bool lockedStatus, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestBlocksTerminalControlRPC");
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RequestTerminalControl, info))
			{
				return;
			}
			if (info.Sender == null)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			RigContainer rigContainer;
			if (!(VRRigCache.Instance != null) || !VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if ((table.linkedTerminal.transform.position - rigContainer.Rig.bodyTransform.position).sqrMagnitude > 9f)
			{
				return;
			}
			if (table.linkedTerminal.ValidateTerminalControlRequest(lockedStatus, info.Sender.ActorNumber))
			{
				int num = lockedStatus ? info.Sender.ActorNumber : -2;
				base.photonView.RPC("SetBlocksTerminalDriverRPC", 0, new object[]
				{
					num
				});
			}
		}

		// Token: 0x060057DF RID: 22495 RVA: 0x001C1584 File Offset: 0x001BF784
		[PunRPC]
		private void SetBlocksTerminalDriverRPC(int driver, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SetBlocksTerminalDriverRPC");
			if (info.Sender == null || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (driver != -2 && NetworkSystem.Instance.GetPlayer(driver) == null)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetTerminalDriver, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			table.linkedTerminal.SetTerminalDriver(driver);
		}

		// Token: 0x060057E0 RID: 22496 RVA: 0x001C15FB File Offset: 0x001BF7FB
		public void RequestLoadSharedBlocksMap(string mapID)
		{
			base.photonView.RPC("LoadSharedBlocksMapRPC", 2, new object[]
			{
				mapID
			});
		}

		// Token: 0x060057E1 RID: 22497 RVA: 0x001C1618 File Offset: 0x001BF818
		[PunRPC]
		private void LoadSharedBlocksMapRPC(string mapID, PhotonMessageInfo info)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "LoadSharedBlocksMapRPC");
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.LoadSharedBlocksMap, info))
			{
				return;
			}
			if (info.Sender == null || mapID.IsNullOrEmpty())
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			if (!table.linkedTerminal.ValidateLoadMapRequest(mapID, info.Sender.ActorNumber))
			{
				GTDev.LogWarning<string>("SharedBlocks ValidateLoadMapRequest fail", null);
				return;
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (tableState == BuilderTable.TableState.Ready || tableState == BuilderTable.TableState.BadData)
			{
				table.SetPendingMap(mapID);
				base.photonView.RPC("SharedTableEventRPC", 1, new object[]
				{
					0,
					mapID
				});
				this.localClientTableInit.Reset();
				UnityEvent onMapCleared = table.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
				table.SetTableState(BuilderTable.TableState.WaitingForSharedMapLoad);
				table.FindAndLoadSharedBlocksMap(mapID);
				return;
			}
			GTDev.LogWarning<string>("SharedBlocks Invalid state " + tableState.ToString(), null);
			this.LoadSharedBlocksFailedMaster(mapID);
		}

		// Token: 0x060057E2 RID: 22498 RVA: 0x001C172D File Offset: 0x001BF92D
		public void LoadSharedBlocksFailedMaster(string mapID)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (mapID.Length > 8)
			{
				return;
			}
			base.photonView.RPC("SharedTableEventRPC", 0, new object[]
			{
				1,
				mapID
			});
		}

		// Token: 0x060057E3 RID: 22499 RVA: 0x001C176A File Offset: 0x001BF96A
		public void SharedBlocksOutOfBoundsMaster(string mapID)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (mapID.Length > 8)
			{
				return;
			}
			base.photonView.RPC("SharedTableEventRPC", 0, new object[]
			{
				2,
				mapID
			});
		}

		// Token: 0x060057E4 RID: 22500 RVA: 0x001C17A8 File Offset: 0x001BF9A8
		[PunRPC]
		private void SharedTableEventRPC(byte eventType, string mapID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SharedTableEventRPC");
			if (eventType >= 3)
			{
				return;
			}
			if (!SharedBlocksManager.IsMapIDValid(mapID) && eventType != 1)
			{
				GTDev.LogWarning<string>("BuilderTableNetworking SharedTableEventRPC Invalid Map ID", null);
				return;
			}
			if (info.Sender == null || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SharedTableEvent, info))
			{
				GTDev.LogError<string>("SharedTableEventRPC Failed call limits", null);
				return;
			}
			if (this.GetTable().isTableMutable)
			{
				return;
			}
			switch (eventType)
			{
			case 0:
				this.OnSharedBlocksLoadStarted(mapID);
				return;
			case 1:
				this.OnLoadSharedBlocksFailed(mapID);
				return;
			case 2:
				this.OnSharedBlocksOutOfBounds(mapID);
				return;
			default:
				return;
			}
		}

		// Token: 0x060057E5 RID: 22501 RVA: 0x001C1844 File Offset: 0x001BFA44
		private void OnSharedBlocksLoadStarted(string mapID)
		{
			this.localClientTableInit.Reset();
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				table.ClearTable();
				table.ClearQueuedCommands();
				table.SetPendingMap(mapID);
				table.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.PlayerEnterBuilder();
			}
		}

		// Token: 0x060057E6 RID: 22502 RVA: 0x001C188C File Offset: 0x001BFA8C
		private void OnLoadSharedBlocksFailed(string mapID)
		{
			BuilderTable table = this.GetTable();
			string pendingMap = table.GetPendingMap();
			if (!pendingMap.IsNullOrEmpty() && !pendingMap.Equals(mapID))
			{
				GTDev.LogWarning<string>("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected map ID " + mapID, null);
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (!NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.WaitingForInitalBuild && tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected table state {0}", tableState), null);
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitingForSharedMapLoad && tableState != BuilderTable.TableState.WaitForInitialBuildMaster && tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected table state {0}", tableState), null);
				return;
			}
			table.SetPendingMap(null);
			if (table != null && !table.isTableMutable && table.linkedTerminal != null)
			{
				if (!SharedBlocksManager.IsMapIDValid(mapID))
				{
					UnityEvent<string> onMapLoadFailed = table.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("BAD MAP ID");
					return;
				}
				else
				{
					UnityEvent<string> onMapLoadFailed2 = table.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("LOAD FAILED");
				}
			}
		}

		// Token: 0x060057E7 RID: 22503 RVA: 0x001C1994 File Offset: 0x001BFB94
		private void OnSharedBlocksOutOfBounds(string mapID)
		{
			BuilderTable table = this.GetTable();
			string pendingMap = table.GetPendingMap();
			if (!pendingMap.IsNullOrEmpty() && !pendingMap.Equals(mapID))
			{
				GTDev.LogWarning<string>("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected map ID " + mapID, null);
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (!NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.WaitingForInitalBuild)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected table state {0}", tableState), null);
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForInitialBuildMaster && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected table state {0}", tableState), null);
				return;
			}
			table.SetPendingMap(null);
			if (table != null && !table.isTableMutable && table.linkedTerminal != null)
			{
				UnityEvent<string> onMapLoadFailed = table.OnMapLoadFailed;
				if (onMapLoadFailed == null)
				{
					return;
				}
				onMapLoadFailed.Invoke("BLOCKS ARE OUT OF BOUNDS FOR SHARED BLOCKS ROOM");
			}
		}

		// Token: 0x060057E8 RID: 22504 RVA: 0x00002789 File Offset: 0x00000989
		public void RequestPaintPiece(int pieceID, int materialType)
		{
		}

		// Token: 0x040064FD RID: 25853
		public PhotonView tablePhotonView;

		// Token: 0x040064FE RID: 25854
		private const int MAX_TABLE_BYTES = 1048576;

		// Token: 0x040064FF RID: 25855
		private const int MAX_TABLE_CHUNK_BYTES = 1000;

		// Token: 0x04006500 RID: 25856
		private const float DELAY_CLIENT_TABLE_CREATION_TIME = 1f;

		// Token: 0x04006501 RID: 25857
		private const float SEND_INIT_DATA_COOLDOWN = 0f;

		// Token: 0x04006502 RID: 25858
		private const int PIECE_SYNC_BYTES = 128;

		// Token: 0x04006503 RID: 25859
		private BuilderTable currTable;

		// Token: 0x04006504 RID: 25860
		private int nextLocalCommandId;

		// Token: 0x04006506 RID: 25862
		private List<BuilderTableNetworking.PlayerTableInitState> masterClientTableInit;

		// Token: 0x04006507 RID: 25863
		private List<BuilderTableNetworking.PlayerTableInitState> masterClientTableValidators;

		// Token: 0x04006508 RID: 25864
		private BuilderTableNetworking.PlayerTableInitState localClientTableInit;

		// Token: 0x04006509 RID: 25865
		private BuilderTableNetworking.PlayerTableInitState localValidationTable;

		// Token: 0x0400650A RID: 25866
		[HideInInspector]
		public List<Player> armShelfRequests;

		// Token: 0x0400650B RID: 25867
		private CallLimiter[] callLimiters;

		// Token: 0x02000DD1 RID: 3537
		public class PlayerTableInitState
		{
			// Token: 0x060057EA RID: 22506 RVA: 0x001C1A6A File Offset: 0x001BFC6A
			public PlayerTableInitState()
			{
				this.serializedTableState = new byte[1048576];
				this.chunk = new byte[1000];
				this.Reset();
			}

			// Token: 0x060057EB RID: 22507 RVA: 0x001C1A98 File Offset: 0x001BFC98
			public void Reset()
			{
				this.player = null;
				this.numSerializedBytes = 0;
				this.totalSerializedBytes = 0;
			}

			// Token: 0x0400650C RID: 25868
			public Player player;

			// Token: 0x0400650D RID: 25869
			public int numSerializedBytes;

			// Token: 0x0400650E RID: 25870
			public int totalSerializedBytes;

			// Token: 0x0400650F RID: 25871
			public byte[] serializedTableState;

			// Token: 0x04006510 RID: 25872
			public byte[] chunk;

			// Token: 0x04006511 RID: 25873
			public float waitForInitTimeRemaining;

			// Token: 0x04006512 RID: 25874
			public float sendNextChunkTimeRemaining;
		}

		// Token: 0x02000DD2 RID: 3538
		private enum RPC
		{
			// Token: 0x04006514 RID: 25876
			PlayerEnterMaster,
			// Token: 0x04006515 RID: 25877
			TableDataMaster,
			// Token: 0x04006516 RID: 25878
			TableData,
			// Token: 0x04006517 RID: 25879
			TableDataStart,
			// Token: 0x04006518 RID: 25880
			PlacePieceMaster,
			// Token: 0x04006519 RID: 25881
			PlacePiece,
			// Token: 0x0400651A RID: 25882
			GrabPieceMaster,
			// Token: 0x0400651B RID: 25883
			GrabPiece,
			// Token: 0x0400651C RID: 25884
			DropPieceMaster,
			// Token: 0x0400651D RID: 25885
			DropPiece,
			// Token: 0x0400651E RID: 25886
			RequestFailed,
			// Token: 0x0400651F RID: 25887
			PieceDropZone,
			// Token: 0x04006520 RID: 25888
			CreatePiece,
			// Token: 0x04006521 RID: 25889
			CreatePieceMaster,
			// Token: 0x04006522 RID: 25890
			CreateShelfPieceMaster,
			// Token: 0x04006523 RID: 25891
			RecyclePieceMaster,
			// Token: 0x04006524 RID: 25892
			PlotClaimedMaster,
			// Token: 0x04006525 RID: 25893
			ArmShelfCreated,
			// Token: 0x04006526 RID: 25894
			ShelfSelection,
			// Token: 0x04006527 RID: 25895
			ShelfSelectionMaster,
			// Token: 0x04006528 RID: 25896
			SetFunctionalState,
			// Token: 0x04006529 RID: 25897
			SetFunctionalStateMaster,
			// Token: 0x0400652A RID: 25898
			RequestTerminalControl,
			// Token: 0x0400652B RID: 25899
			SetTerminalDriver,
			// Token: 0x0400652C RID: 25900
			LoadSharedBlocksMap,
			// Token: 0x0400652D RID: 25901
			SharedTableEvent,
			// Token: 0x0400652E RID: 25902
			Count
		}

		// Token: 0x02000DD3 RID: 3539
		private enum SharedTableEventTypes
		{
			// Token: 0x04006530 RID: 25904
			LOAD_STARTED,
			// Token: 0x04006531 RID: 25905
			LOAD_FAILED,
			// Token: 0x04006532 RID: 25906
			OUT_OF_BOUNDS,
			// Token: 0x04006533 RID: 25907
			COUNT
		}
	}
}
