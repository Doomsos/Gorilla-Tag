using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005F6 RID: 1526
public class GameAgent : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x14000045 RID: 69
	// (add) Token: 0x0600265B RID: 9819 RVA: 0x000CCB04 File Offset: 0x000CAD04
	// (remove) Token: 0x0600265C RID: 9820 RVA: 0x000CCB3C File Offset: 0x000CAD3C
	public event GameAgent.StateChangedEvent onBodyStateChanged;

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x0600265D RID: 9821 RVA: 0x000CCB74 File Offset: 0x000CAD74
	// (remove) Token: 0x0600265E RID: 9822 RVA: 0x000CCBAC File Offset: 0x000CADAC
	public event GameAgent.StateChangedEvent onBehaviorStateChanged;

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x0600265F RID: 9823 RVA: 0x000CCBE4 File Offset: 0x000CADE4
	// (remove) Token: 0x06002660 RID: 9824 RVA: 0x000CCC1C File Offset: 0x000CAE1C
	public event GameAgent.NavigationLinkReachedEvent onReachedNavigationLink;

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x06002661 RID: 9825 RVA: 0x000CCC54 File Offset: 0x000CAE54
	// (remove) Token: 0x06002662 RID: 9826 RVA: 0x000CCC8C File Offset: 0x000CAE8C
	public event GameAgent.JumpRequestedEvent onJumpRequested;

	// Token: 0x14000049 RID: 73
	// (add) Token: 0x06002663 RID: 9827 RVA: 0x000CCCC4 File Offset: 0x000CAEC4
	// (remove) Token: 0x06002664 RID: 9828 RVA: 0x000CCCFC File Offset: 0x000CAEFC
	public event GameAgent.NavigationFailedEvent onNavigationFailed;

	// Token: 0x06002665 RID: 9829 RVA: 0x000CCD31 File Offset: 0x000CAF31
	public GameAgentManager GetGameAgentManager()
	{
		return this.entity.manager.gameAgentManager;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000CCD43 File Offset: 0x000CAF43
	private void Awake()
	{
		this.agentComponents = new List<IGameAgentComponent>(1);
		base.GetComponentsInChildren<IGameAgentComponent>(this.agentComponents);
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000CCD5D File Offset: 0x000CAF5D
	public void OnEntityInit()
	{
		this.GetGameAgentManager().AddGameAgent(this);
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000CCD6B File Offset: 0x000CAF6B
	public void OnEntityDestroy()
	{
		this.GetGameAgentManager().RemoveGameAgent(this);
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000CCD79 File Offset: 0x000CAF79
	public void OnBehaviorStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBehaviorStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000CCD8C File Offset: 0x000CAF8C
	public void OnBodyStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBodyStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000CCDA0 File Offset: 0x000CAFA0
	public void OnThink(float deltaTime)
	{
		if (!this.pauseEntityThink)
		{
			for (int i = 0; i < this.agentComponents.Count; i++)
			{
				this.agentComponents[i].OnEntityThink(deltaTime);
			}
		}
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000CCDE0 File Offset: 0x000CAFE0
	public void OnUpdate()
	{
		if (this.navAgent.isOnNavMesh)
		{
			this.lastPosOnNavMesh = this.navAgent.transform.position;
		}
		if (!this.navAgent.autoTraverseOffMeshLink && !this.wasOnOffMeshNavLink && this.navAgent.isOnOffMeshLink)
		{
			if (this.entity.IsAuthority())
			{
				if ((this.navAgent.transform.position - this.navAgent.currentOffMeshLinkData.startPos).sqrMagnitude < (this.navAgent.transform.position - this.navAgent.currentOffMeshLinkData.endPos).sqrMagnitude)
				{
					this.GetGameAgentManager().RequestJump(this, this.navAgent.transform.position, this.navAgent.currentOffMeshLinkData.endPos, 1f, 1f);
				}
				else
				{
					this.GetGameAgentManager().RequestJump(this, this.navAgent.transform.position, this.navAgent.currentOffMeshLinkData.startPos, 1f, 1f);
				}
			}
			GameAgent.NavigationLinkReachedEvent navigationLinkReachedEvent = this.onReachedNavigationLink;
			if (navigationLinkReachedEvent != null)
			{
				navigationLinkReachedEvent(this.navAgent.currentOffMeshLinkData);
			}
		}
		this.wasOnOffMeshNavLink = this.navAgent.isOnOffMeshLink;
		if (!this.hasNotifiedNavigationFailure && !this.navAgent.pathPending && (this.navAgent.pathStatus == 1 || this.navAgent.pathStatus == 2))
		{
			GameAgent.NavigationFailedEvent navigationFailedEvent = this.onNavigationFailed;
			if (navigationFailedEvent != null)
			{
				navigationFailedEvent(this.navAgent.pathStatus, this.navAgent.destination, this.navAgent.remainingDistance);
			}
			this.hasNotifiedNavigationFailure = true;
		}
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000CCFBA File Offset: 0x000CB1BA
	public void OnJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		GameAgent.JumpRequestedEvent jumpRequestedEvent = this.onJumpRequested;
		if (jumpRequestedEvent == null)
		{
			return;
		}
		jumpRequestedEvent(start, end, heightScale, speedScale);
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000CCFD1 File Offset: 0x000CB1D1
	public bool IsOnNavMesh()
	{
		return this.navAgent != null && this.navAgent.isOnNavMesh;
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000CCFEE File Offset: 0x000CB1EE
	public Vector3 GetLastPosOnNavMesh()
	{
		return this.lastPosOnNavMesh;
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000CCFF8 File Offset: 0x000CB1F8
	public void RequestDestination(Vector3 dest)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!this.IsOnNavMesh())
		{
			dest = this.lastPosOnNavMesh;
		}
		if (Vector3.Distance(this.lastRequestedDest, dest) < 0.5f)
		{
			return;
		}
		this.lastRequestedDest = dest;
		if (this.entity.IsAuthority())
		{
			this.GetGameAgentManager().RequestDestination(this, dest);
		}
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000CD058 File Offset: 0x000CB258
	public void RequestBehaviorChange(byte behavior)
	{
		this.GetGameAgentManager().RequestBehavior(this, behavior);
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000CD067 File Offset: 0x000CB267
	public void RequestStateChange(byte state)
	{
		this.GetGameAgentManager().RequestState(this, state);
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000CD076 File Offset: 0x000CB276
	public void RequestTarget(NetPlayer targetPlayer)
	{
		this.GetGameAgentManager().RequestTarget(this, targetPlayer);
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000CD088 File Offset: 0x000CB288
	public void ApplyDestination(Vector3 dest)
	{
		NavMeshHit navMeshHit;
		if (!NavMesh.SamplePosition(dest, ref navMeshHit, 1.5f, -1))
		{
			return;
		}
		dest = navMeshHit.position;
		this.lastReceivedDest = dest;
		this.hasNotifiedNavigationFailure = false;
		if (this.navAgent.isOnNavMesh)
		{
			this.navAgent.destination = dest;
		}
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000CD0D6 File Offset: 0x000CB2D6
	public void SetDisableNetworkSync(bool disable)
	{
		this.disableNetworkSync = disable;
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000CD0DF File Offset: 0x000CB2DF
	public void SetIsPathing(bool isPathing, bool ignoreRigiBody = false)
	{
		this.navAgent.enabled = isPathing;
		if (!ignoreRigiBody && this.rigidBody != null)
		{
			this.rigidBody.isKinematic = isPathing;
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000CD10A File Offset: 0x000CB30A
	public void SetSpeed(float speed)
	{
		this.navAgent.speed = speed;
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000CD118 File Offset: 0x000CB318
	public void ApplyNetworkUpdate(Vector3 position, Quaternion rotation)
	{
		if (this.disableNetworkSync)
		{
			return;
		}
		if ((base.transform.position - position).sqrMagnitude > this.networkPositionCorrectionDist * this.networkPositionCorrectionDist)
		{
			this.navAgent.Warp(position);
			this.navAgent.destination = this.lastReceivedDest;
		}
		base.transform.rotation = rotation;
		if (this.rigidBody != null)
		{
			this.rigidBody.rotation = rotation;
		}
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000CD19C File Offset: 0x000CB39C
	public static void UpdateFacing(Transform transform, NavMeshAgent navAgent, NetPlayer targetPlayer, float turnspeed = 3600f)
	{
		Transform target = null;
		Vector3 forward = transform.forward;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				target = grplayer.transform;
			}
		}
		GameAgent.UpdateFacingTarget(transform, navAgent, target, turnspeed);
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000CD1E4 File Offset: 0x000CB3E4
	public static void UpdateFacingTarget(Transform transform, NavMeshAgent navAgent, Transform target, float turnspeed = 3600f)
	{
		Vector3 vector = transform.forward;
		if (target != null)
		{
			Vector3 position = target.position;
			Vector3 position2 = transform.position;
			Vector3 vector2 = position - position2;
			vector2.y = 0f;
			float magnitude = vector2.magnitude;
			if (magnitude > 0f)
			{
				vector = vector2 / magnitude;
			}
		}
		else
		{
			Vector3 desiredVelocity = navAgent.desiredVelocity;
			desiredVelocity.y = 0f;
			float magnitude2 = desiredVelocity.magnitude;
			if (magnitude2 > 0f)
			{
				vector = desiredVelocity / magnitude2;
			}
		}
		Quaternion quaternion = Quaternion.LookRotation(vector);
		if (navAgent.speed > 0f)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Mathf.Clamp(turnspeed * navAgent.speed / Quaternion.Angle(transform.rotation, quaternion) * Time.deltaTime, 0f, 1f));
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Mathf.Clamp(turnspeed / Quaternion.Angle(transform.rotation, quaternion) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000CD2F4 File Offset: 0x000CB4F4
	public static void UpdateFacingForward(Transform transform, NavMeshAgent navAgent, float turnspeed = 3600f)
	{
		Vector3 desiredVelocity = navAgent.desiredVelocity;
		desiredVelocity.y = 0f;
		float magnitude = desiredVelocity.magnitude;
		if (magnitude <= 0f)
		{
			return;
		}
		Vector3 facingDir = desiredVelocity / magnitude;
		GameAgent.UpdateFacingDir(transform, navAgent, facingDir, turnspeed);
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000CD338 File Offset: 0x000CB538
	public static void UpdateFacingPos(Transform transform, NavMeshAgent navAgent, Vector3 facingPos, float turnspeed = 3600f)
	{
		Vector3 facingDir = facingPos - transform.position;
		facingDir.y = 0f;
		facingDir.Normalize();
		GameAgent.UpdateFacingDir(transform, navAgent, facingDir, turnspeed);
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x000CD370 File Offset: 0x000CB570
	public static void UpdateFacingDir(Transform transform, NavMeshAgent navAgent, Vector3 facingDir, float turnspeed = 3600f)
	{
		Quaternion quaternion = Quaternion.LookRotation(facingDir);
		transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Mathf.Clamp(turnspeed * navAgent.speed / Quaternion.Angle(transform.rotation, quaternion) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x0400325B RID: 12891
	public GameEntity entity;

	// Token: 0x0400325C RID: 12892
	public NavMeshAgent navAgent;

	// Token: 0x0400325D RID: 12893
	public Rigidbody rigidBody;

	// Token: 0x0400325E RID: 12894
	public float networkPositionCorrectionDist = 2.5f;

	// Token: 0x0400325F RID: 12895
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003260 RID: 12896
	private bool disableNetworkSync;

	// Token: 0x04003261 RID: 12897
	private Vector3 lastPosOnNavMesh;

	// Token: 0x04003262 RID: 12898
	private Vector3 lastRequestedDest;

	// Token: 0x04003263 RID: 12899
	private Vector3 lastReceivedDest;

	// Token: 0x04003269 RID: 12905
	private bool hasNotifiedNavigationFailure;

	// Token: 0x0400326A RID: 12906
	private List<IGameAgentComponent> agentComponents;

	// Token: 0x0400326B RID: 12907
	private bool wasOnOffMeshNavLink;

	// Token: 0x0400326C RID: 12908
	[ReadOnly]
	public bool pauseEntityThink;

	// Token: 0x020005F7 RID: 1527
	// (Invoke) Token: 0x06002681 RID: 9857
	public delegate void StateChangedEvent(byte newState);

	// Token: 0x020005F8 RID: 1528
	// (Invoke) Token: 0x06002685 RID: 9861
	public delegate void NavigationLinkReachedEvent(OffMeshLinkData linkData);

	// Token: 0x020005F9 RID: 1529
	// (Invoke) Token: 0x06002689 RID: 9865
	public delegate void JumpRequestedEvent(Vector3 start, Vector3 end, float heightScale, float speedScale);

	// Token: 0x020005FA RID: 1530
	// (Invoke) Token: 0x0600268D RID: 9869
	public delegate void NavigationFailedEvent(NavMeshPathStatus status, Vector3 destination, float remainingDistance);
}
