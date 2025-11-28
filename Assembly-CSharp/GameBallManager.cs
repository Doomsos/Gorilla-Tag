using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000540 RID: 1344
[NetworkBehaviourWeaved(0)]
public class GameBallManager : NetworkComponent
{
	// Token: 0x060021CF RID: 8655 RVA: 0x000B0770 File Offset: 0x000AE970
	protected override void Awake()
	{
		base.Awake();
		GameBallManager.Instance = this;
		this.gameBalls = new List<GameBall>(64);
		this.gameBallData = new List<GameBallData>(64);
		this._callLimiters = new CallLimiter[8];
		this._callLimiters[0] = new CallLimiter(50, 1f, 0.5f);
		this._callLimiters[1] = new CallLimiter(50, 1f, 0.5f);
		this._callLimiters[2] = new CallLimiter(25, 1f, 0.5f);
		this._callLimiters[3] = new CallLimiter(25, 1f, 0.5f);
		this._callLimiters[4] = new CallLimiter(10, 1f, 0.5f);
		this._callLimiters[5] = new CallLimiter(10, 1f, 0.5f);
		this._callLimiters[6] = new CallLimiter(10, 1f, 0.5f);
		this._callLimiters[7] = new CallLimiter(25, 1f, 0.5f);
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000B087C File Offset: 0x000AEA7C
	private bool ValidateCallLimits(GameBallManager.RPC rpcCall, PhotonMessageInfo info)
	{
		if (rpcCall < GameBallManager.RPC.RequestGrabBall || rpcCall >= GameBallManager.RPC.Count)
		{
			return false;
		}
		bool flag = this._callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		if (!flag)
		{
			this.ReportRPCCall(rpcCall, info, "Too many RPC Calls!");
		}
		return flag;
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000B08B7 File Offset: 0x000AEAB7
	private void ReportRPCCall(GameBallManager.RPC rpcCall, PhotonMessageInfo info, string susReason)
	{
		GorillaNot.instance.SendReport(string.Format("Reason: {0}   RPC: {1}", susReason, rpcCall), info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x000B08EC File Offset: 0x000AEAEC
	public GameBallId AddGameBall(GameBall gameBall)
	{
		int count = this.gameBallData.Count;
		this.gameBalls.Add(gameBall);
		GameBallData gameBallData = default(GameBallData);
		this.gameBallData.Add(gameBallData);
		gameBall.id = new GameBallId(count);
		return gameBall.id;
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000B0938 File Offset: 0x000AEB38
	public GameBall GetGameBall(GameBallId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		int index = id.index;
		return this.gameBalls[index];
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000B0964 File Offset: 0x000AEB64
	public GameBallId TryGrabLocal(Vector3 handPosition, int teamId)
	{
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		GameBallId result = GameBallId.Invalid;
		float num = float.MaxValue;
		for (int i = 0; i < this.gameBalls.Count; i++)
		{
			if (this.gameBalls[i].onlyGrabTeamId == -1 || this.gameBalls[i].onlyGrabTeamId == teamId)
			{
				float sqrMagnitude = this.gameBalls[i].GetVelocity().sqrMagnitude;
				double num2 = 0.0625;
				if (sqrMagnitude > 2f)
				{
					num2 = 0.25;
				}
				float sqrMagnitude2 = (handPosition - this.gameBalls[i].transform.position).sqrMagnitude;
				if ((double)sqrMagnitude2 < num2 && sqrMagnitude2 < num)
				{
					result = this.gameBalls[i].id;
					num = sqrMagnitude2;
				}
			}
		}
		return result;
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000B0A4C File Offset: 0x000AEC4C
	public void RequestGrabBall(GameBallId ballId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		this.GrabBall(ballId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
		this.photonView.RPC("RequestGrabBallRPC", 2, new object[]
		{
			ballId.index,
			isLeftHand,
			num
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000B0AB4 File Offset: 0x000AECB4
	[PunRPC]
	private void RequestGrabBallRPC(int gameBallIndex, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestGrabBallRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestGrabBall, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex > this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBallIndex out of array.");
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!vector.IsValid(num) || !quaternion.IsValid())
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "localPosition or localRotation is invalid.");
			return;
		}
		bool flag = true;
		GameBall gameBall = this.gameBalls[gameBallIndex];
		if (gameBall != null)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				if (!rigContainer.Rig.IsPositionInRange(gameBall.transform.position, 25f))
				{
					flag = false;
					this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBall exceeds max catch distance.");
				}
			}
			else
			{
				flag = false;
				this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "Cannot find VRRig for grabber.");
			}
			if (vector.sqrMagnitude > 25f)
			{
				flag = false;
				this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBall exceeds max catch distance.");
			}
		}
		else
		{
			flag = false;
			this.ReportRPCCall(GameBallManager.RPC.RequestGrabBall, info, "gameBall does not exist.");
		}
		if (flag)
		{
			this.photonView.RPC("GrabBallRPC", 0, new object[]
			{
				gameBallIndex,
				isLeftHand,
				packedPosRot,
				info.Sender
			});
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000B0C18 File Offset: 0x000AEE18
	[PunRPC]
	private void GrabBallRPC(int gameBallIndex, bool isLeftHand, long packedPosRot, Player grabbedBy, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "GrabBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.GrabBall, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex > this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.GrabBall, info, "gameBallIndex out of array.");
			return;
		}
		Vector3 localPosition;
		Quaternion localRotation;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
		float num = 10000f;
		if (!localPosition.IsValid(num) || !localRotation.IsValid())
		{
			this.ReportRPCCall(GameBallManager.RPC.GrabBall, info, "localPosition or localRotation is invalid.");
			return;
		}
		this.GrabBall(new GameBallId(gameBallIndex), isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedBy));
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000B0CB8 File Offset: 0x000AEEB8
	private void GrabBall(GameBallId gameBallId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(grabbedByPlayer, out rigContainer))
		{
			return;
		}
		GameBallData gameBallData = this.gameBallData[gameBallId.index];
		GameBall gameBall = this.gameBalls[gameBallId.index];
		GameBallPlayer gameBallPlayer = (gameBall.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(gameBall.heldByActorNumber);
		int num = (gameBallPlayer == null) ? -1 : gameBallPlayer.FindHandIndex(gameBallId);
		bool flag = gameBall.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int num2 = -1;
		if (gameBallPlayer != null)
		{
			gameBallPlayer.ClearGrabbedIfHeld(gameBallId);
			num2 = gameBallPlayer.teamId;
			if (num != -1 && flag)
			{
				GameBallPlayerLocal.instance.ClearGrabbed(num);
			}
		}
		BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
		Transform parent = isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
		if (!grabbedByPlayer.IsLocal)
		{
			gameBall.SetVisualOffset(true);
		}
		gameBall.transform.SetParent(parent);
		gameBall.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(grabbedByPlayer.ActorNumber);
		bool flag2 = num2 == gamePlayer.teamId;
		bool flag3 = gameBall.lastHeldByActorNumber != grabbedByPlayer.ActorNumber;
		MonkeBall component2 = gameBall.GetComponent<MonkeBall>();
		if (component2 != null)
		{
			component2.OnGrabbed();
			if (!flag2 && flag3)
			{
				component2.OnSwitchHeldByTeam(gamePlayer.teamId);
			}
		}
		gameBall.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameBall.lastHeldByActorNumber = gameBall.heldByActorNumber;
		gameBall.SetHeldByTeamId(gamePlayer.teamId);
		int handIndex = GameBallPlayer.GetHandIndex(isLeftHand);
		gamePlayer.SetGrabbed(gameBallId, handIndex);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GameBallPlayerLocal.instance.SetGrabbed(gameBallId, GameBallPlayer.GetHandIndex(isLeftHand));
			GameBallPlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		gameBall.PlayCatchFx();
		if (component2 != null)
		{
			MonkeBallGame.Instance.OnBallGrabbed(gameBallId);
		}
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000B0EC0 File Offset: 0x000AF0C0
	public void RequestThrowBall(GameBallId ballId, bool isLeftHand, Vector3 velocity, Vector3 angVelocity)
	{
		GameBall gameBall = this.GetGameBall(ballId);
		if (gameBall == null)
		{
			return;
		}
		Vector3 position = gameBall.transform.position;
		Quaternion rotation = gameBall.transform.rotation;
		this.ThrowBall(ballId, isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		this.photonView.RPC("RequestThrowBallRPC", 2, new object[]
		{
			ballId.index,
			isLeftHand,
			position,
			rotation,
			velocity,
			angVelocity
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x000B0F68 File Offset: 0x000AF168
	[PunRPC]
	private void RequestThrowBallRPC(int gameBallIndex, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestThrowBallRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestThrowBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, angVelocity))
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		if (this.gameBalls[gameBallIndex].heldByActorNumber != info.Sender.ActorNumber && this.gameBalls[gameBallIndex].lastHeldByActorNumber != info.Sender.ActorNumber && (this.gameBalls[gameBallIndex].heldByActorNumber != -1 || this.gameBalls[gameBallIndex].lastHeldByActorNumber != -1))
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "gameBall is not held by the thrower.");
			return;
		}
		bool flag = true;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer))
		{
			if ((rigContainer.Rig.transform.position - position).sqrMagnitude > 6.25f)
			{
				flag = false;
				this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "gameBall distance exceeds max distance from hand.");
			}
		}
		else
		{
			flag = false;
			this.ReportRPCCall(GameBallManager.RPC.RequestThrowBall, info, "Player rig cannot be found for thrower.");
		}
		if (flag)
		{
			this.photonView.RPC("ThrowBallRPC", 0, new object[]
			{
				gameBallIndex,
				isLeftHand,
				position,
				rotation,
				velocity,
				angVelocity,
				info.Sender,
				info.SentServerTime
			});
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x000B1108 File Offset: 0x000AF308
	[PunRPC]
	private void ThrowBallRPC(int gameBallIndex, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownBy, double throwTime, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "ThrowBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.ThrowBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, angVelocity))
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		if ((base.transform.position - position).sqrMagnitude > 6400f)
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "gameBall distance exceeds max distance from arena.");
			return;
		}
		if (double.IsNaN(throwTime) || double.IsInfinity(throwTime))
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "throwTime is not a valid value.");
			return;
		}
		float num = (float)(PhotonNetwork.Time - throwTime);
		if (num < -3f || num > 3f)
		{
			this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "Throw time delta exceeds range.");
			return;
		}
		GameBall gameBall = this.gameBalls[gameBallIndex];
		position = 0.5f * Physics.gravity * gameBall.gravityMult * num * num + velocity * num + position;
		velocity = Physics.gravity * gameBall.gravityMult * num + velocity;
		rotation *= Quaternion.Euler(angVelocity * Time.deltaTime);
		this.ThrowBall(new GameBallId(gameBallIndex), isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(thrownBy));
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x000B1270 File Offset: 0x000AF470
	private bool ValidateThrowBallParams(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
	{
		if (gameBallIndex < 0 || gameBallIndex >= this.gameBalls.Count)
		{
			return false;
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
					return velocity.sqrMagnitude <= 1600f;
				}
			}
		}
		return false;
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x000B12E0 File Offset: 0x000AF4E0
	private void ThrowBall(GameBallId gameBallId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		GameBall gameBall = this.gameBalls[gameBallId.index];
		if (!thrownByPlayer.IsLocal)
		{
			gameBall.SetVisualOffset(true);
		}
		gameBall.transform.SetParent(null);
		gameBall.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.linearVelocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameBall.heldByActorNumber = -1;
		MonkeBall monkeBall = MonkeBall.Get(gameBall);
		if (monkeBall != null)
		{
			monkeBall.ClearCannotGrabTeamId();
		}
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GameBallPlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GameBallPlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GameBallPlayerLocal.instance.ClearGrabbed(handIndex);
			GameBallPlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GameBallPlayer component2 = rigContainer.Rig.GetComponent<GameBallPlayer>();
			if (component2 != null)
			{
				component2.ClearGrabbedIfHeld(gameBallId);
			}
		}
		gameBall.PlayThrowFx();
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000B140C File Offset: 0x000AF60C
	public void RequestLaunchBall(GameBallId ballId, Vector3 velocity)
	{
		GameBall gameBall = this.GetGameBall(ballId);
		if (gameBall == null)
		{
			return;
		}
		Vector3 position = gameBall.transform.position;
		Quaternion rotation = gameBall.transform.rotation;
		this.LaunchBall(ballId, position, rotation, velocity);
		this.photonView.RPC("RequestLaunchBallRPC", 2, new object[]
		{
			ballId.index,
			position,
			rotation,
			velocity
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000B1494 File Offset: 0x000AF694
	[PunRPC]
	private void RequestLaunchBallRPC(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestLaunchBallRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestLaunchBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, Vector3.zero))
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestLaunchBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		bool flag = true;
		if ((MonkeBallGame.Instance.BallLauncher.position - position).sqrMagnitude > 1f)
		{
			flag = false;
			this.ReportRPCCall(GameBallManager.RPC.RequestLaunchBall, info, "gameBall distance exceeds max distance from launcher.");
		}
		if (flag)
		{
			this.photonView.RPC("LaunchBallRPC", 0, new object[]
			{
				gameBallIndex,
				position,
				rotation,
				velocity,
				info.SentServerTime
			});
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000B156C File Offset: 0x000AF76C
	[PunRPC]
	private void LaunchBallRPC(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, double throwTime, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "LaunchBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.ThrowBall, info))
		{
			return;
		}
		if (!this.ValidateThrowBallParams(gameBallIndex, position, rotation, velocity, Vector3.zero))
		{
			this.ReportRPCCall(GameBallManager.RPC.LaunchBall, info, "ValidateThrowBallParams are invalid.");
			return;
		}
		float num = (float)(PhotonNetwork.Time - throwTime);
		if (num < -3f || num > 3f)
		{
			this.ReportRPCCall(GameBallManager.RPC.LaunchBall, info, "Throw time delta exceeds range.");
			return;
		}
		GameBall gameBall = this.gameBalls[gameBallIndex];
		position = 0.5f * Physics.gravity * gameBall.gravityMult * num * num + velocity * num + position;
		velocity = Physics.gravity * gameBall.gravityMult * num + velocity;
		this.LaunchBall(new GameBallId(gameBallIndex), position, rotation, velocity);
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000B1664 File Offset: 0x000AF864
	private void LaunchBall(GameBallId gameBallId, Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		GameBall gameBall = this.gameBalls[gameBallId.index];
		gameBall.transform.SetParent(null);
		gameBall.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.linearVelocity = velocity;
			component.angularVelocity = Vector3.zero;
		}
		gameBall.heldByActorNumber = -1;
		gameBall.lastHeldByActorNumber = -1;
		gameBall.WasLaunched();
		MonkeBall monkeBall = MonkeBall.Get(gameBall);
		if (monkeBall != null)
		{
			monkeBall.ClearCannotGrabTeamId();
			monkeBall.TriggerDelayedResync();
		}
		gameBall.PlayThrowFx();
		GameBallPlayerLocal.instance.gamePlayer.ClearAllGrabbed();
		GameBallPlayerLocal.instance.ClearAllGrabbed();
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000B1728 File Offset: 0x000AF928
	public void RequestTeleportBall(GameBallId id, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.photonView.RPC("TeleportBallRPC", 0, new object[]
		{
			id.index,
			position,
			rotation,
			velocity,
			angularVelocity
		});
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000B1788 File Offset: 0x000AF988
	[PunRPC]
	private void TeleportBallRPC(int gameBallIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "TeleportBallRPC");
		if (!this.ValidateCallLimits(GameBallManager.RPC.TeleportBall, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.TeleportBall, info, "gameBallIndex is out of range.");
			return;
		}
		float num = 10000f;
		if (position.IsValid(num) && rotation.IsValid())
		{
			float num2 = 10000f;
			if (velocity.IsValid(num2))
			{
				float num3 = 10000f;
				if (angularVelocity.IsValid(num3))
				{
					if ((base.transform.position - position).sqrMagnitude > 6400f)
					{
						this.ReportRPCCall(GameBallManager.RPC.ThrowBall, info, "gameBall distance exceeds max distance from arena.");
						return;
					}
					GameBallId gameBallId = new GameBallId(gameBallIndex);
					this.TeleportBall(gameBallId, position, rotation, velocity, angularVelocity);
					return;
				}
			}
		}
		this.ReportRPCCall(GameBallManager.RPC.TeleportBall, info, "Ball params are invalid.");
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000B1870 File Offset: 0x000AFA70
	private void TeleportBall(GameBallId gameBallId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		int index = gameBallId.index;
		if (index < 0 || index >= this.gameBallData.Count)
		{
			return;
		}
		GameBallData gameBallData = this.gameBallData[index];
		GameBall gameBall = this.gameBalls[index];
		if (gameBall == null)
		{
			return;
		}
		gameBall.SetVisualOffset(false);
		gameBall.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.linearVelocity = velocity;
			component.angularVelocity = angularVelocity;
		}
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000B1904 File Offset: 0x000AFB04
	public void RequestSetBallPosition(GameBallId ballId)
	{
		if (this.GetGameBall(ballId) == null)
		{
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		this.photonView.RPC("RequestSetBallPositionRPC", 2, new object[]
		{
			ballId.index
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000B1958 File Offset: 0x000AFB58
	[PunRPC]
	private void RequestSetBallPositionRPC(int gameBallIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestSetBallPositionRPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.ValidateCallLimits(GameBallManager.RPC.RequestSetBallPosition, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.gameBalls.Count)
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestSetBallPosition, info, "gameBallIndex is out of range.");
			return;
		}
		GameBall gameBall = this.gameBalls[gameBallIndex];
		if (gameBall == null)
		{
			return;
		}
		if ((gameBall.transform.position - base.transform.position).sqrMagnitude > 6400f)
		{
			this.ReportRPCCall(GameBallManager.RPC.RequestSetBallPosition, info, "Ball position is outside of arena.");
			return;
		}
		Rigidbody component = gameBall.GetComponent<Rigidbody>();
		if (component == null)
		{
			return;
		}
		this.photonView.RPC("TeleportBallRPC", info.Sender, new object[]
		{
			gameBallIndex,
			gameBall.transform.position,
			gameBall.transform.rotation,
			component.linearVelocity,
			component.angularVelocity
		});
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x00002789 File Offset: 0x00000989
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002C85 RID: 11397
	[OnEnterPlay_SetNull]
	public static volatile GameBallManager Instance;

	// Token: 0x04002C86 RID: 11398
	public PhotonView photonView;

	// Token: 0x04002C87 RID: 11399
	private List<GameBall> gameBalls;

	// Token: 0x04002C88 RID: 11400
	private List<GameBallData> gameBallData;

	// Token: 0x04002C89 RID: 11401
	public const float MAX_LOCAL_MAGNITUDE_SQR = 6400f;

	// Token: 0x04002C8A RID: 11402
	private const float MAX_LAUNCHER_DISTANCE_SQR = 1f;

	// Token: 0x04002C8B RID: 11403
	public const float MAX_CATCH_DISTANCE_FROM_HAND_SQR = 25f;

	// Token: 0x04002C8C RID: 11404
	public const float MAX_DISTANCE_FROM_HAND_SQR = 6.25f;

	// Token: 0x04002C8D RID: 11405
	public const float MAX_THROW_VELOCITY_SQR = 1600f;

	// Token: 0x04002C8E RID: 11406
	private CallLimiter[] _callLimiters;

	// Token: 0x02000541 RID: 1345
	private enum RPC
	{
		// Token: 0x04002C90 RID: 11408
		RequestGrabBall,
		// Token: 0x04002C91 RID: 11409
		GrabBall,
		// Token: 0x04002C92 RID: 11410
		RequestThrowBall,
		// Token: 0x04002C93 RID: 11411
		ThrowBall,
		// Token: 0x04002C94 RID: 11412
		RequestLaunchBall,
		// Token: 0x04002C95 RID: 11413
		LaunchBall,
		// Token: 0x04002C96 RID: 11414
		TeleportBall,
		// Token: 0x04002C97 RID: 11415
		RequestSetBallPosition,
		// Token: 0x04002C98 RID: 11416
		Count
	}
}
