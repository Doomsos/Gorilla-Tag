using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DFE RID: 3582
	public class PlayerTimerManager : MonoBehaviourPunCallbacks
	{
		// Token: 0x0600596C RID: 22892 RVA: 0x001C9A44 File Offset: 0x001C7C44
		private void Awake()
		{
			if (PlayerTimerManager.instance == null)
			{
				PlayerTimerManager.instance = this;
			}
			else if (PlayerTimerManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.callLimiters = new CallLimiter[2];
			this.callLimiters[0] = new CallLimiter(10, 1f, 0.5f);
			this.callLimiters[1] = new CallLimiter(30, 1f, 0.5f);
			this.playerTimerData = new Dictionary<int, PlayerTimerManager.PlayerTimerData>(10);
			this.timerToggleLimiters = new Dictionary<int, CallLimiter>(10);
			this.limiterPool = new List<CallLimiter>(10);
			this.serializedTimerData = new byte[256];
		}

		// Token: 0x0600596D RID: 22893 RVA: 0x001C9AF4 File Offset: 0x001C7CF4
		private CallLimiter CreateLimiterFromPool()
		{
			if (this.limiterPool.Count > 0)
			{
				CallLimiter result = this.limiterPool[this.limiterPool.Count - 1];
				this.limiterPool.RemoveAt(this.limiterPool.Count - 1);
				return result;
			}
			return new CallLimiter(5, 1f, 0.5f);
		}

		// Token: 0x0600596E RID: 22894 RVA: 0x001C9B50 File Offset: 0x001C7D50
		private void ReturnCallLimiterToPool(CallLimiter limiter)
		{
			if (limiter == null)
			{
				return;
			}
			limiter.Reset();
			this.limiterPool.Add(limiter);
		}

		// Token: 0x0600596F RID: 22895 RVA: 0x001C9B68 File Offset: 0x001C7D68
		public void RegisterTimerBoard(PlayerTimerBoard board)
		{
			if (!PlayerTimerManager.timerBoards.Contains(board))
			{
				PlayerTimerManager.timerBoards.Add(board);
				this.UpdateTimerBoard(board);
			}
		}

		// Token: 0x06005970 RID: 22896 RVA: 0x001C9B89 File Offset: 0x001C7D89
		public void UnregisterTimerBoard(PlayerTimerBoard board)
		{
			PlayerTimerManager.timerBoards.Remove(board);
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x001C9B98 File Offset: 0x001C7D98
		public bool IsLocalTimerStarted()
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			return this.playerTimerData.TryGetValue(NetworkSystem.Instance.LocalPlayer.ActorNumber, ref playerTimerData) && playerTimerData.isStarted;
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x001C9BCC File Offset: 0x001C7DCC
		public float GetTimeForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (!this.playerTimerData.TryGetValue(actorNumber, ref playerTimerData))
			{
				return 0f;
			}
			if (playerTimerData.isStarted)
			{
				return Mathf.Clamp((PhotonNetwork.ServerTimestamp - playerTimerData.startTimeStamp) / 1000f, 0f, 3599.99f);
			}
			return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
		}

		// Token: 0x06005973 RID: 22899 RVA: 0x001C9C38 File Offset: 0x001C7E38
		public float GetLastDurationForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(actorNumber, ref playerTimerData))
			{
				return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
			}
			return -1f;
		}

		// Token: 0x06005974 RID: 22900 RVA: 0x001C9C78 File Offset: 0x001C7E78
		[PunRPC]
		private void InitTimersMasterRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "InitTimersMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.InitTimersMaster, info))
			{
				return;
			}
			if (this.areTimersInitialized)
			{
				return;
			}
			this.DeserializeTimerState(bytes.Length, bytes);
			this.areTimersInitialized = true;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06005975 RID: 22901 RVA: 0x001C9CCC File Offset: 0x001C7ECC
		private int SerializeTimerState()
		{
			Array.Clear(this.serializedTimerData, 0, this.serializedTimerData.Length);
			MemoryStream memoryStream = new MemoryStream(this.serializedTimerData);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.playerTimerData.Count > 10)
			{
				this.ClearOldPlayerData();
			}
			binaryWriter.Write(this.playerTimerData.Count);
			foreach (KeyValuePair<int, PlayerTimerManager.PlayerTimerData> keyValuePair in this.playerTimerData)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.startTimeStamp);
				binaryWriter.Write(keyValuePair.Value.endTimeStamp);
				binaryWriter.Write(keyValuePair.Value.isStarted ? 1 : 0);
				binaryWriter.Write(keyValuePair.Value.lastTimerDuration);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x06005976 RID: 22902 RVA: 0x001C9DC8 File Offset: 0x001C7FC8
		private void DeserializeTimerState(int numBytes, byte[] bytes)
		{
			if (numBytes <= 0 || numBytes > 256)
			{
				return;
			}
			if (bytes == null || bytes.Length < numBytes)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			this.playerTimerData.Clear();
			try
			{
				List<Player> list = Enumerable.ToList<Player>(PhotonNetwork.PlayerList);
				if (bytes.Length < 4)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num = binaryReader.ReadInt32();
				if (num < 0 || num > 10)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num2 = 17;
				if (memoryStream.Position + (long)(num2 * num) > (long)bytes.Length)
				{
					this.playerTimerData.Clear();
					return;
				}
				for (int i = 0; i < num; i++)
				{
					int actorNum = binaryReader.ReadInt32();
					int startTimeStamp = binaryReader.ReadInt32();
					int endTimeStamp = binaryReader.ReadInt32();
					bool isStarted = binaryReader.ReadByte() > 0;
					uint lastTimerDuration = binaryReader.ReadUInt32();
					if (list.FindIndex((Player x) => x.ActorNumber == actorNum) >= 0)
					{
						PlayerTimerManager.PlayerTimerData playerTimerData = new PlayerTimerManager.PlayerTimerData
						{
							startTimeStamp = startTimeStamp,
							endTimeStamp = endTimeStamp,
							isStarted = isStarted,
							lastTimerDuration = lastTimerDuration
						};
						this.playerTimerData.TryAdd(actorNum, playerTimerData);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				this.playerTimerData.Clear();
			}
			if (Time.time - this.requestSendTime < 5f && this.IsLocalTimerStarted() != this.localPlayerRequestedStart)
			{
				this.timerPV.RPC("RequestTimerToggleRPC", 2, new object[]
				{
					this.localPlayerRequestedStart
				});
			}
		}

		// Token: 0x06005977 RID: 22903 RVA: 0x001C9F8C File Offset: 0x001C818C
		private void ClearOldPlayerData()
		{
			List<int> list = new List<int>(this.playerTimerData.Count);
			List<Player> list2 = Enumerable.ToList<Player>(PhotonNetwork.PlayerList);
			using (Dictionary<int, PlayerTimerManager.PlayerTimerData>.KeyCollection.Enumerator enumerator = this.playerTimerData.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int actorNum = enumerator.Current;
					if (list2.FindIndex((Player x) => x.ActorNumber == actorNum) < 0)
					{
						list.Add(actorNum);
					}
				}
			}
			foreach (int num in list)
			{
				this.playerTimerData.Remove(num);
			}
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x001CA06C File Offset: 0x001C826C
		public void RequestTimerToggle(bool startTimer)
		{
			this.requestSendTime = Time.time;
			this.localPlayerRequestedStart = startTimer;
			this.timerPV.RPC("RequestTimerToggleRPC", 2, new object[]
			{
				startTimer
			});
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x001CA0A0 File Offset: 0x001C82A0
		[PunRPC]
		private void RequestTimerToggleRPC(bool startTimer, PhotonMessageInfo info)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "RequestTimerToggleRPC");
			CallLimiter callLimiter;
			if (this.timerToggleLimiters.TryGetValue(info.Sender.ActorNumber, ref callLimiter))
			{
				if (!callLimiter.CheckCallTime(Time.time))
				{
					return;
				}
			}
			else
			{
				CallLimiter callLimiter2 = this.CreateLimiterFromPool();
				this.timerToggleLimiters.Add(info.Sender.ActorNumber, callLimiter2);
				callLimiter2.CheckCallTime(Time.time);
			}
			if (info.Sender == null)
			{
				return;
			}
			PlayerTimerManager.PlayerTimerData playerTimerData;
			bool flag = this.playerTimerData.TryGetValue(info.Sender.ActorNumber, ref playerTimerData);
			if (!startTimer && !flag)
			{
				return;
			}
			if (flag && !startTimer && !playerTimerData.isStarted)
			{
				return;
			}
			int num = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.timerPV.RPC("TimerToggledMasterRPC", 0, new object[]
			{
				startTimer,
				num,
				info.Sender
			});
		}

		// Token: 0x0600597A RID: 22906 RVA: 0x001CA1B8 File Offset: 0x001C83B8
		[PunRPC]
		private void TimerToggledMasterRPC(bool startTimer, int toggleTimeStamp, Player player, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "TimerToggledMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.ToggleTimerMaster, info))
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (!this.areTimersInitialized)
			{
				return;
			}
			int num = toggleTimeStamp;
			int num2 = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num2 > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num2 = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			if (num2 - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = num2 - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.OnToggleTimerForPlayer(startTimer, player, num);
		}

		// Token: 0x0600597B RID: 22907 RVA: 0x001CA260 File Offset: 0x001C8460
		private void OnToggleTimerForPlayer(bool startTimer, Player player, int toggleTime)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(player.ActorNumber, ref playerTimerData))
			{
				if (startTimer && !playerTimerData.isStarted)
				{
					playerTimerData.startTimeStamp = toggleTime;
					playerTimerData.isStarted = true;
					UnityEvent<int> onTimerStartedForPlayer = this.OnTimerStartedForPlayer;
					if (onTimerStartedForPlayer != null)
					{
						onTimerStartedForPlayer.Invoke(player.ActorNumber);
					}
					if (player.IsLocal)
					{
						UnityEvent onLocalTimerStarted = this.OnLocalTimerStarted;
						if (onLocalTimerStarted != null)
						{
							onLocalTimerStarted.Invoke();
						}
					}
				}
				else if (!startTimer && playerTimerData.isStarted)
				{
					playerTimerData.endTimeStamp = toggleTime;
					playerTimerData.isStarted = false;
					playerTimerData.lastTimerDuration = (uint)(playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					UnityEvent<int, int> onTimerStopped = this.OnTimerStopped;
					if (onTimerStopped != null)
					{
						onTimerStopped.Invoke(player.ActorNumber, playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					}
				}
				this.playerTimerData[player.ActorNumber] = playerTimerData;
			}
			else
			{
				PlayerTimerManager.PlayerTimerData playerTimerData2 = new PlayerTimerManager.PlayerTimerData
				{
					startTimeStamp = (startTimer ? toggleTime : 0),
					endTimeStamp = (startTimer ? 0 : toggleTime),
					isStarted = startTimer,
					lastTimerDuration = 0U
				};
				this.playerTimerData.TryAdd(player.ActorNumber, playerTimerData2);
				UnityEvent<int> onTimerStartedForPlayer2 = this.OnTimerStartedForPlayer;
				if (onTimerStartedForPlayer2 != null)
				{
					onTimerStartedForPlayer2.Invoke(player.ActorNumber);
				}
				if (player.IsLocal)
				{
					UnityEvent onLocalTimerStarted2 = this.OnLocalTimerStarted;
					if (onLocalTimerStarted2 != null)
					{
						onLocalTimerStarted2.Invoke();
					}
				}
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x0600597C RID: 22908 RVA: 0x001CA3B8 File Offset: 0x001C85B8
		private bool ValidateCallLimits(PlayerTimerManager.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= PlayerTimerManager.RPC.InitTimersMaster && rpcCall < PlayerTimerManager.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x0600597D RID: 22909 RVA: 0x001CA3E8 File Offset: 0x001C85E8
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			base.OnMasterClientSwitched(newMasterClient);
			if (newMasterClient.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", 1, new object[]
				{
					num,
					this.serializedTimerData
				});
				return;
			}
			this.playerTimerData.Clear();
			this.areTimersInitialized = false;
		}

		// Token: 0x0600597E RID: 22910 RVA: 0x001CA448 File Offset: 0x001C8648
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			base.OnPlayerEnteredRoom(newPlayer);
			if (PhotonNetwork.IsMasterClient && !newPlayer.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", newPlayer, new object[]
				{
					num,
					this.serializedTimerData
				});
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x0600597F RID: 22911 RVA: 0x001CA4A4 File Offset: 0x001C86A4
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			this.playerTimerData.Remove(otherPlayer.ActorNumber);
			CallLimiter limiter;
			if (this.timerToggleLimiters.TryGetValue(otherPlayer.ActorNumber, ref limiter))
			{
				this.ReturnCallLimiterToPool(limiter);
				this.timerToggleLimiters.Remove(otherPlayer.ActorNumber);
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06005980 RID: 22912 RVA: 0x001CA500 File Offset: 0x001C8700
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			this.joinedRoom = true;
			if (PhotonNetwork.IsMasterClient)
			{
				this.playerTimerData.Clear();
				foreach (CallLimiter limiter in this.timerToggleLimiters.Values)
				{
					this.ReturnCallLimiterToPool(limiter);
				}
				this.timerToggleLimiters.Clear();
				this.areTimersInitialized = true;
				this.UpdateAllTimerBoards();
				return;
			}
			this.requestSendTime = 0f;
			this.areTimersInitialized = false;
		}

		// Token: 0x06005981 RID: 22913 RVA: 0x001CA5A4 File Offset: 0x001C87A4
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			this.joinedRoom = false;
			this.playerTimerData.Clear();
			foreach (CallLimiter limiter in this.timerToggleLimiters.Values)
			{
				this.ReturnCallLimiterToPool(limiter);
			}
			this.timerToggleLimiters.Clear();
			this.areTimersInitialized = false;
			this.requestSendTime = 0f;
			this.localPlayerRequestedStart = false;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06005982 RID: 22914 RVA: 0x001CA640 File Offset: 0x001C8840
		private void UpdateAllTimerBoards()
		{
			foreach (PlayerTimerBoard board in PlayerTimerManager.timerBoards)
			{
				this.UpdateTimerBoard(board);
			}
		}

		// Token: 0x06005983 RID: 22915 RVA: 0x001CA694 File Offset: 0x001C8894
		private void UpdateTimerBoard(PlayerTimerBoard board)
		{
			board.SetSleepState(this.joinedRoom);
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (!this.joinedRoom)
			{
				if (board.notInRoomText != null)
				{
					board.notInRoomText.gameObject.SetActive(true);
					board.notInRoomText.text = GorillaComputer.instance.offlineTextInitialString;
				}
				for (int i = 0; i < board.lines.Count; i++)
				{
					board.lines[i].ResetData();
				}
				return;
			}
			if (board.notInRoomText != null)
			{
				board.notInRoomText.gameObject.SetActive(false);
			}
			for (int j = 0; j < board.lines.Count; j++)
			{
				PlayerTimerBoardLine playerTimerBoardLine = board.lines[j];
				if (j < PhotonNetwork.PlayerList.Length)
				{
					playerTimerBoardLine.gameObject.SetActive(true);
					playerTimerBoardLine.SetLineData(NetworkSystem.Instance.GetPlayer(PhotonNetwork.PlayerList[j]));
					playerTimerBoardLine.UpdateLine();
				}
				else
				{
					playerTimerBoardLine.ResetData();
					playerTimerBoardLine.gameObject.SetActive(false);
				}
			}
			board.RedrawPlayerLines();
		}

		// Token: 0x04006696 RID: 26262
		public static PlayerTimerManager instance;

		// Token: 0x04006697 RID: 26263
		public PhotonView timerPV;

		// Token: 0x04006698 RID: 26264
		public UnityEvent OnLocalTimerStarted;

		// Token: 0x04006699 RID: 26265
		public UnityEvent<int> OnTimerStartedForPlayer;

		// Token: 0x0400669A RID: 26266
		public UnityEvent<int, int> OnTimerStopped;

		// Token: 0x0400669B RID: 26267
		public const float MAX_DURATION_SECONDS = 3599.99f;

		// Token: 0x0400669C RID: 26268
		private float requestSendTime;

		// Token: 0x0400669D RID: 26269
		private bool localPlayerRequestedStart;

		// Token: 0x0400669E RID: 26270
		private CallLimiter[] callLimiters;

		// Token: 0x0400669F RID: 26271
		private Dictionary<int, CallLimiter> timerToggleLimiters;

		// Token: 0x040066A0 RID: 26272
		private List<CallLimiter> limiterPool;

		// Token: 0x040066A1 RID: 26273
		private bool areTimersInitialized;

		// Token: 0x040066A2 RID: 26274
		private Dictionary<int, PlayerTimerManager.PlayerTimerData> playerTimerData;

		// Token: 0x040066A3 RID: 26275
		private const int MAX_TIMER_INIT_BYTES = 256;

		// Token: 0x040066A4 RID: 26276
		private byte[] serializedTimerData;

		// Token: 0x040066A5 RID: 26277
		private static List<PlayerTimerBoard> timerBoards = new List<PlayerTimerBoard>(10);

		// Token: 0x040066A6 RID: 26278
		private bool joinedRoom;

		// Token: 0x02000DFF RID: 3583
		private enum RPC
		{
			// Token: 0x040066A8 RID: 26280
			InitTimersMaster,
			// Token: 0x040066A9 RID: 26281
			ToggleTimerMaster,
			// Token: 0x040066AA RID: 26282
			Count
		}

		// Token: 0x02000E00 RID: 3584
		public struct PlayerTimerData
		{
			// Token: 0x040066AB RID: 26283
			public int startTimeStamp;

			// Token: 0x040066AC RID: 26284
			public int endTimeStamp;

			// Token: 0x040066AD RID: 26285
			public bool isStarted;

			// Token: 0x040066AE RID: 26286
			public uint lastTimerDuration;
		}
	}
}
