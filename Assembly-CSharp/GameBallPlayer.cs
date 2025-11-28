using System;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class GameBallPlayer : MonoBehaviour
{
	// Token: 0x060021EE RID: 8686 RVA: 0x000B1A6C File Offset: 0x000AFC6C
	private void Awake()
	{
		this.hands = new GameBallPlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.teamId = -1;
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x000B1AA0 File Offset: 0x000AFCA0
	public void CleanupPlayer()
	{
		MonkeBallPlayer component = base.GetComponent<MonkeBallPlayer>();
		if (component != null)
		{
			component.currGoalZone = null;
			for (int i = 0; i < MonkeBallGame.Instance.goalZones.Count; i++)
			{
				MonkeBallGame.Instance.goalZones[i].CleanupPlayer(component);
			}
		}
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x000B1AF4 File Offset: 0x000AFCF4
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GameBallPlayer.HandData handData = this.hands[handIndex];
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000B1B34 File Offset: 0x000AFD34
	public void ClearGrabbedIfHeld(GameBallId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x000B1B6D File Offset: 0x000AFD6D
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000B1B7C File Offset: 0x000AFD7C
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000B1BA3 File Offset: 0x000AFDA3
	public void SetInGoalZone(bool inZone)
	{
		if (inZone)
		{
			this.inGoalZone++;
			return;
		}
		this.inGoalZone--;
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000B1BC8 File Offset: 0x000AFDC8
	public bool IsHoldingBall()
	{
		return this.GetGameBallId().IsValid();
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000B1BE3 File Offset: 0x000AFDE3
	public GameBallId GetGameBallId(int handIndex)
	{
		return this.hands[handIndex].grabbedGameBallId;
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000B1BF8 File Offset: 0x000AFDF8
	public int FindHandIndex(GameBallId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000B1C34 File Offset: 0x000AFE34
	public GameBallId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId.IsValid())
			{
				return this.hands[i].grabbedGameBallId;
			}
		}
		return GameBallId.Invalid;
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000B1C83 File Offset: 0x000AFE83
	public bool IsLocalPlayer()
	{
		return VRRigCache.Instance.localRig.Creator.ActorNumber == this.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000B1CAB File Offset: 0x000AFEAB
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000B1CB1 File Offset: 0x000AFEB1
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000B1CBC File Offset: 0x000AFEBC
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || player.IsNull || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000B1CF8 File Offset: 0x000AFEF8
	public static GameBallPlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		VRRig vrrig = GameBallPlayer.GetRig(actorNumber);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.GetComponent<GameBallPlayer>();
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x000B1D24 File Offset: 0x000AFF24
	public static GameBallPlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameBallPlayer component = transform.GetComponent<GameBallPlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x04002C99 RID: 11417
	public VRRig rig;

	// Token: 0x04002C9A RID: 11418
	public int teamId;

	// Token: 0x04002C9B RID: 11419
	private GameBallPlayer.HandData[] hands;

	// Token: 0x04002C9C RID: 11420
	public const int MAX_HANDS = 2;

	// Token: 0x04002C9D RID: 11421
	public const int LEFT_HAND = 0;

	// Token: 0x04002C9E RID: 11422
	public const int RIGHT_HAND = 1;

	// Token: 0x04002C9F RID: 11423
	private int inGoalZone;

	// Token: 0x02000543 RID: 1347
	private struct HandData
	{
		// Token: 0x04002CA0 RID: 11424
		public GameBallId grabbedGameBallId;
	}
}
