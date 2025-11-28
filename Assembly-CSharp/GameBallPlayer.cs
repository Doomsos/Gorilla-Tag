using System;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class GameBallPlayer : MonoBehaviour
{
	// Token: 0x060021EE RID: 8686 RVA: 0x000B1A4C File Offset: 0x000AFC4C
	private void Awake()
	{
		this.hands = new GameBallPlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.teamId = -1;
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x000B1A80 File Offset: 0x000AFC80
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

	// Token: 0x060021F0 RID: 8688 RVA: 0x000B1AD4 File Offset: 0x000AFCD4
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

	// Token: 0x060021F1 RID: 8689 RVA: 0x000B1B14 File Offset: 0x000AFD14
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

	// Token: 0x060021F2 RID: 8690 RVA: 0x000B1B4D File Offset: 0x000AFD4D
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000B1B5C File Offset: 0x000AFD5C
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000B1B83 File Offset: 0x000AFD83
	public void SetInGoalZone(bool inZone)
	{
		if (inZone)
		{
			this.inGoalZone++;
			return;
		}
		this.inGoalZone--;
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000B1BA8 File Offset: 0x000AFDA8
	public bool IsHoldingBall()
	{
		return this.GetGameBallId().IsValid();
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000B1BC3 File Offset: 0x000AFDC3
	public GameBallId GetGameBallId(int handIndex)
	{
		return this.hands[handIndex].grabbedGameBallId;
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000B1BD8 File Offset: 0x000AFDD8
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

	// Token: 0x060021F8 RID: 8696 RVA: 0x000B1C14 File Offset: 0x000AFE14
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

	// Token: 0x060021F9 RID: 8697 RVA: 0x000B1C63 File Offset: 0x000AFE63
	public bool IsLocalPlayer()
	{
		return VRRigCache.Instance.localRig.Creator.ActorNumber == this.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000B1C8B File Offset: 0x000AFE8B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000B1C91 File Offset: 0x000AFE91
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000B1C9C File Offset: 0x000AFE9C
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

	// Token: 0x060021FD RID: 8701 RVA: 0x000B1CD8 File Offset: 0x000AFED8
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

	// Token: 0x060021FE RID: 8702 RVA: 0x000B1D04 File Offset: 0x000AFF04
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
