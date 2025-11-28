using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts.GhostReactor.SoakTasks;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000659 RID: 1625
public class GhostReactorSoak
{
	// Token: 0x060029CA RID: 10698 RVA: 0x000E2010 File Offset: 0x000E0210
	public void Setup(GRPlayer grPlayer)
	{
		this.grPlayer = grPlayer;
		GhostReactorSoak.instance = this;
		if (this.IsSoaking())
		{
			Debug.LogFormat("Soak Setup {0} InRoom {1} Auth {2}", new object[]
			{
				this.state,
				this.grManager != null && this.grManager.IsAuthority(),
				PhotonNetwork.InRoom
			});
		}
		this._soakTasks.Add(new SoakTaskGrabThrow(grPlayer));
		this._soakTasks.Add(new SoakTaskDepositCollectibles(grPlayer));
		this._soakTasks.Add(new SoakTaskBreakable(grPlayer));
		this._soakTasks.Add(new SoakTaskHitEnemy(grPlayer));
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsSoaking()
	{
		return false;
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x000E20C8 File Offset: 0x000E02C8
	public void OnUpdate()
	{
		if (!this.IsSoaking())
		{
			return;
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.grPlayer.gamePlayer.rig.zoneEntity.currentZone);
		if (managerForZone == null)
		{
			return;
		}
		this.grManager = managerForZone.ghostReactorManager;
		if (this.grManager == null)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		switch (this.state)
		{
		case GhostReactorSoak.State.Disconnected:
			if (!PhotonNetwork.InRoom && timeAsDouble > this.reconnectTime)
			{
				this.SetState(GhostReactorSoak.State.Connecting);
				return;
			}
			break;
		case GhostReactorSoak.State.Connecting:
			if (this.grManager.IsZoneActive())
			{
				this.SetState(GhostReactorSoak.State.Active);
				return;
			}
			if (timeAsDouble > this.stateStartTime + 15.0)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
				return;
			}
			break;
		case GhostReactorSoak.State.Active:
			this.UpdateActive();
			if (timeAsDouble > this.disconnectTime)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
				return;
			}
			if (!PhotonNetwork.InRoom)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x000E21B0 File Offset: 0x000E03B0
	private int GetActorNumber()
	{
		if (this.grPlayer.gamePlayer.rig.OwningNetPlayer == null)
		{
			return -1;
		}
		return this.grPlayer.gamePlayer.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x000E21E8 File Offset: 0x000E03E8
	public void SetState(GhostReactorSoak.State newState)
	{
		this.state = newState;
		this.stateStartTime = Time.timeAsDouble;
		Debug.LogFormat("Soak Set State {0} Player {1} InRoom {2} Auth {3}", new object[]
		{
			this.state,
			this.GetActorNumber(),
			this.grManager != null && this.grManager.IsAuthority(),
			PhotonNetwork.InRoom
		});
		switch (this.state)
		{
		case GhostReactorSoak.State.Disconnected:
			this.LeaveRoom();
			this.reconnectTime = this.stateStartTime + (double)Random.Range(3f, 6f);
			return;
		case GhostReactorSoak.State.Connecting:
			this.JoinRoom();
			return;
		case GhostReactorSoak.State.Active:
			this.disconnectTime = this.stateStartTime + (double)Random.Range(5f, 60f);
			return;
		default:
			return;
		}
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x000E22C6 File Offset: 0x000E04C6
	public void JoinRoom()
	{
		Debug.LogFormat("Soak Join Room {0}", new object[]
		{
			"AKJSOAK"
		});
		PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("AKJSOAK", JoinType.Solo);
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x000E22F2 File Offset: 0x000E04F2
	public void LeaveRoom()
	{
		Debug.LogFormat("Soak Leave Room", Array.Empty<object>());
		NetworkSystem.Instance.ReturnToSinglePlayer();
	}

	// Token: 0x060029D1 RID: 10705 RVA: 0x000E2310 File Offset: 0x000E0510
	private void UpdateActive()
	{
		if (this._activeTask != null)
		{
			bool flag = false;
			if (!this._activeTask.Update())
			{
				Debug.LogError(string.Format("Failed to execute soak task of type {0}", this._activeTask.GetType()));
				flag = true;
			}
			if (flag || this._activeTask.Complete)
			{
				this._activeTask.Reset();
				this._activeTask = null;
				return;
			}
		}
		else if (Random.value <= 0.005f)
		{
			int num = Random.Range(0, this._soakTasks.Count);
			this._activeTask = this._soakTasks[num];
		}
	}

	// Token: 0x04003591 RID: 13713
	public static GhostReactorSoak instance;

	// Token: 0x04003592 RID: 13714
	private const string SOAK_ROOM = "AKJSOAK";

	// Token: 0x04003593 RID: 13715
	private const float MIN_CONNECTED_TIME = 5f;

	// Token: 0x04003594 RID: 13716
	private const float MAX_CONNECTED_TIME = 60f;

	// Token: 0x04003595 RID: 13717
	private const float MIN_DISCONNECTED_TIME = 3f;

	// Token: 0x04003596 RID: 13718
	private const float MAX_DISCONNECTED_TIME = 6f;

	// Token: 0x04003597 RID: 13719
	public GRPlayer grPlayer;

	// Token: 0x04003598 RID: 13720
	public GhostReactorManager grManager;

	// Token: 0x04003599 RID: 13721
	public GhostReactorSoak.State state;

	// Token: 0x0400359A RID: 13722
	public double stateStartTime;

	// Token: 0x0400359B RID: 13723
	public double reconnectTime;

	// Token: 0x0400359C RID: 13724
	public double disconnectTime;

	// Token: 0x0400359D RID: 13725
	public const float START_NEW_TASK_ODDS = 0.005f;

	// Token: 0x0400359E RID: 13726
	private IGhostReactorSoakTask _activeTask;

	// Token: 0x0400359F RID: 13727
	private readonly List<IGhostReactorSoakTask> _soakTasks = new List<IGhostReactorSoakTask>();

	// Token: 0x0200065A RID: 1626
	public enum State
	{
		// Token: 0x040035A1 RID: 13729
		Disconnected,
		// Token: 0x040035A2 RID: 13730
		Connecting,
		// Token: 0x040035A3 RID: 13731
		Active
	}
}
