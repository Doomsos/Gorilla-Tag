using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200080A RID: 2058
public class FreeHoverboardManager : NetworkSceneObject
{
	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x0600362B RID: 13867 RVA: 0x00125D33 File Offset: 0x00123F33
	// (set) Token: 0x0600362C RID: 13868 RVA: 0x00125D3A File Offset: 0x00123F3A
	public static FreeHoverboardManager instance { get; private set; }

	// Token: 0x0600362D RID: 13869 RVA: 0x00125D44 File Offset: 0x00123F44
	private FreeHoverboardManager.DataPerPlayer GetOrCreatePlayerData(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, ref dataPerPlayer))
		{
			dataPerPlayer = default(FreeHoverboardManager.DataPerPlayer);
			dataPerPlayer.Init(actorNumber, this.freeBoardPool);
			this.perPlayerData.Add(actorNumber, dataPerPlayer);
		}
		return dataPerPlayer;
	}

	// Token: 0x0600362E RID: 13870 RVA: 0x00125D88 File Offset: 0x00123F88
	private void Awake()
	{
		FreeHoverboardManager.instance = this;
		for (int i = 0; i < 20; i++)
		{
			FreeHoverboardInstance freeHoverboardInstance = Object.Instantiate<FreeHoverboardInstance>(this.freeHoverboardPrefab);
			freeHoverboardInstance.gameObject.SetActive(false);
			this.freeBoardPool.Push(freeHoverboardInstance);
		}
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnLeftRoom);
	}

	// Token: 0x0600362F RID: 13871 RVA: 0x00125E10 File Offset: 0x00124010
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (this.perPlayerData.TryGetValue(netPlayer.ActorNumber, ref dataPerPlayer))
		{
			dataPerPlayer.ReturnBoards(this.freeBoardPool);
			this.perPlayerData.Remove(netPlayer.ActorNumber);
		}
	}

	// Token: 0x06003630 RID: 13872 RVA: 0x00125E54 File Offset: 0x00124054
	private void OnLeftRoom()
	{
		foreach (KeyValuePair<int, FreeHoverboardManager.DataPerPlayer> keyValuePair in this.perPlayerData)
		{
			keyValuePair.Value.ReturnBoards(this.freeBoardPool);
		}
		this.perPlayerData.Clear();
	}

	// Token: 0x06003631 RID: 13873 RVA: 0x00125EC0 File Offset: 0x001240C0
	private void SpawnBoard(FreeHoverboardManager.DataPerPlayer playerData, int boardIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardInstance freeHoverboardInstance = (boardIndex == 0) ? playerData.board0 : playerData.board1;
		freeHoverboardInstance.transform.position = position;
		freeHoverboardInstance.transform.rotation = rotation;
		freeHoverboardInstance.Rigidbody.linearVelocity = velocity;
		freeHoverboardInstance.Rigidbody.angularVelocity = avelocity;
		freeHoverboardInstance.SetColor(boardColor);
		freeHoverboardInstance.gameObject.SetActive(true);
		int ownerActorNumber = freeHoverboardInstance.ownerActorNumber;
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		int? num = (localPlayer != null) ? new int?(localPlayer.ActorNumber) : default(int?);
		if (ownerActorNumber == num.GetValueOrDefault() & num != null)
		{
			this.localPlayerLastSpawnedBoardIndex = boardIndex;
		}
	}

	// Token: 0x06003632 RID: 13874 RVA: 0x00125F68 File Offset: 0x00124168
	public void SendDropBoardRPC(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		int num = (!orCreatePlayerData.board0.gameObject.activeSelf) ? 0 : ((!orCreatePlayerData.board1.gameObject.activeSelf) ? 1 : (1 - this.localPlayerLastSpawnedBoardIndex));
		if (PhotonNetwork.InRoom)
		{
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			long num4 = BitPackUtils.PackWorldPosForNetwork(velocity);
			long num5 = BitPackUtils.PackWorldPosForNetwork(avelocity);
			short num6 = BitPackUtils.PackColorForNetwork(boardColor);
			this.photonView.RPC("DropBoard_RPC", 0, new object[]
			{
				num == 1,
				num2,
				num3,
				num4,
				num5,
				num6
			});
			return;
		}
		this.SpawnBoard(orCreatePlayerData, num, position, rotation, velocity, avelocity, boardColor);
	}

	// Token: 0x06003633 RID: 13875 RVA: 0x00126050 File Offset: 0x00124250
	[PunRPC]
	public void DropBoard_RPC(bool boardIndex1, long positionPacked, int rotationPacked, long velocityPacked, long avelocityPacked, short colorPacked, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DropBoard_RPC");
		int boardIndex2 = boardIndex1 ? 1 : 0;
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(info.Sender.ActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(positionPacked);
		if (!rigContainer.Rig.IsPositionInRange(position, 5f))
		{
			return;
		}
		this.SpawnBoard(orCreatePlayerData, boardIndex2, position, BitPackUtils.UnpackQuaternionFromNetwork(rotationPacked), BitPackUtils.UnpackWorldPosFromNetwork(velocityPacked), BitPackUtils.UnpackWorldPosFromNetwork(avelocityPacked), BitPackUtils.UnpackColorFromNetwork(colorPacked));
	}

	// Token: 0x06003634 RID: 13876 RVA: 0x001260FC File Offset: 0x001242FC
	public void SendGrabBoardRPC(FreeHoverboardInstance board)
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("GrabBoard_RPC", 0, new object[]
			{
				board.ownerActorNumber,
				board.boardIndex == 1
			});
			board.gameObject.SetActive(false);
			return;
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06003635 RID: 13877 RVA: 0x00126160 File Offset: 0x00124360
	[PunRPC]
	public void GrabBoard_RPC(int ownerActorNumber, bool boardIndex1, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GrabBoard_RPC");
		int boardIndex2 = boardIndex1 ? 1 : 0;
		if (NetworkSystem.Instance.GetNetPlayerByID(ownerActorNumber) == null)
		{
			return;
		}
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(ownerActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FreeHoverboardInstance board = orCreatePlayerData.GetBoard(boardIndex2);
		if (board.IsNull())
		{
			return;
		}
		if (info.Sender.ActorNumber != ownerActorNumber)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if (!rigContainer.Rig.IsPositionInRange(board.transform.position, 5f))
			{
				return;
			}
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06003636 RID: 13878 RVA: 0x00126218 File Offset: 0x00124418
	public void PreserveMaxHoverboardsConstraint(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, ref dataPerPlayer))
		{
			return;
		}
		if (dataPerPlayer.board0.gameObject.activeSelf && dataPerPlayer.board1.gameObject.activeSelf)
		{
			FreeHoverboardInstance board = dataPerPlayer.GetBoard(1 - this.localPlayerLastSpawnedBoardIndex);
			this.SendGrabBoardRPC(board);
		}
	}

	// Token: 0x04004593 RID: 17811
	[SerializeField]
	private FreeHoverboardInstance freeHoverboardPrefab;

	// Token: 0x04004594 RID: 17812
	private Stack<FreeHoverboardInstance> freeBoardPool = new Stack<FreeHoverboardInstance>(20);

	// Token: 0x04004595 RID: 17813
	private const int NumPlayers = 10;

	// Token: 0x04004596 RID: 17814
	private const int NumFreeBoardsPerPlayer = 2;

	// Token: 0x04004597 RID: 17815
	private int localPlayerLastSpawnedBoardIndex;

	// Token: 0x04004598 RID: 17816
	private Dictionary<int, FreeHoverboardManager.DataPerPlayer> perPlayerData = new Dictionary<int, FreeHoverboardManager.DataPerPlayer>();

	// Token: 0x0200080B RID: 2059
	private struct DataPerPlayer
	{
		// Token: 0x06003638 RID: 13880 RVA: 0x00126294 File Offset: 0x00124494
		public void Init(int actorNumber, Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0 = freeBoardPool.Pop();
			this.board0.ownerActorNumber = actorNumber;
			this.board0.boardIndex = 0;
			this.board1 = freeBoardPool.Pop();
			this.board1.ownerActorNumber = actorNumber;
			this.board1.boardIndex = 1;
			this.spamCheck = new CallLimiterWithCooldown(5f, 10, 1f);
		}

		// Token: 0x06003639 RID: 13881 RVA: 0x00126300 File Offset: 0x00124500
		public void ReturnBoards(Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0.gameObject.SetActive(false);
			freeBoardPool.Push(this.board0);
			this.board1.gameObject.SetActive(false);
			freeBoardPool.Push(this.board1);
		}

		// Token: 0x0600363A RID: 13882 RVA: 0x0012633C File Offset: 0x0012453C
		public FreeHoverboardInstance GetBoard(int boardIndex)
		{
			if (boardIndex != 1)
			{
				return this.board0;
			}
			return this.board1;
		}

		// Token: 0x04004599 RID: 17817
		public FreeHoverboardInstance board0;

		// Token: 0x0400459A RID: 17818
		public FreeHoverboardInstance board1;

		// Token: 0x0400459B RID: 17819
		public CallLimiterWithCooldown spamCheck;
	}
}
