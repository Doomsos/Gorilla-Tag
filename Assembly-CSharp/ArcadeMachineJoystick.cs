using System;
using Photon.Pun;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x0200000D RID: 13
public class ArcadeMachineJoystick : HandHold, ISnapTurnOverride, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000021 RID: 33 RVA: 0x00002646 File Offset: 0x00000846
	// (set) Token: 0x06000022 RID: 34 RVA: 0x0000264E File Offset: 0x0000084E
	public bool heldByLocalPlayer { get; private set; }

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000023 RID: 35 RVA: 0x00002657 File Offset: 0x00000857
	public bool IsHeldLeftHanded
	{
		get
		{
			return this.heldByLocalPlayer && this.xrNode == 4;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000024 RID: 36 RVA: 0x0000266C File Offset: 0x0000086C
	// (set) Token: 0x06000025 RID: 37 RVA: 0x00002674 File Offset: 0x00000874
	public ArcadeButtons currentButtonState { get; private set; }

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000026 RID: 38 RVA: 0x0000267D File Offset: 0x0000087D
	// (set) Token: 0x06000027 RID: 39 RVA: 0x00002685 File Offset: 0x00000885
	public int player { get; private set; }

	// Token: 0x06000028 RID: 40 RVA: 0x0000268E File Offset: 0x0000088E
	public void Init(ArcadeMachine machine, int player)
	{
		this.machine = machine;
		this.player = player;
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.guard.AddCallbackTarget(this);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000026B8 File Offset: 0x000008B8
	public void BindController(bool leftHand)
	{
		this.xrNode = (leftHand ? 4 : 5);
		this.heldByLocalPlayer = true;
		if (!leftHand)
		{
			if (!this.snapTurn)
			{
				this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			}
			if (this.snapTurn != null)
			{
				this.snapTurnOverride = true;
				this.snapTurn.SetTurningOverride(this);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.guard.TransferOwnership(PhotonNetwork.LocalPlayer, "");
		}
		else if (!this.guard.isMine)
		{
			this.guard.RequestOwnership(new Action(this.OnOwnershipSuccess), new Action(this.OnOwnershipFail));
		}
		ControllerInputPoller.AddUpdateCallback(new Action(this.OnInputUpdate));
		PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002789 File Offset: 0x00000989
	private void OnOwnershipSuccess()
	{
	}

	// Token: 0x0600002B RID: 43 RVA: 0x0000278B File Offset: 0x0000098B
	private void OnOwnershipFail()
	{
		this.ForceRelease();
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002793 File Offset: 0x00000993
	public void UnbindController()
	{
		this.heldByLocalPlayer = false;
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
		this.OnInputUpdate();
		ControllerInputPoller.RemoveUpdateCallback(new Action(this.OnInputUpdate));
	}

	// Token: 0x0600002D RID: 45 RVA: 0x000027D0 File Offset: 0x000009D0
	private void OnInputUpdate()
	{
		ArcadeButtons arcadeButtons = (ArcadeButtons)0;
		if (this.heldByLocalPlayer)
		{
			arcadeButtons |= ArcadeButtons.GRAB;
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).y > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.UP;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).y < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.DOWN;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).x < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.LEFT;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).x > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.RIGHT;
			}
			if (ControllerInputPoller.PrimaryButtonPress(this.xrNode))
			{
				arcadeButtons |= ArcadeButtons.B0;
			}
			if (ControllerInputPoller.SecondaryButtonPress(this.xrNode))
			{
				arcadeButtons |= ArcadeButtons.B1;
			}
			if (ControllerInputPoller.TriggerFloat(this.xrNode) > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.TRIGGER;
			}
		}
		if (arcadeButtons != this.currentButtonState)
		{
			this.machine.OnJoystickStateChange(this.player, arcadeButtons);
		}
		this.currentButtonState = arcadeButtons;
	}

	// Token: 0x0600002E RID: 46 RVA: 0x000028BC File Offset: 0x00000ABC
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		ArcadeButtons arcadeButtons = (ArcadeButtons)((int)stream.ReceiveNext());
		if (arcadeButtons != this.currentButtonState && this.machine != null)
		{
			this.machine.OnJoystickStateChange(this.player, arcadeButtons);
		}
		this.currentButtonState = arcadeButtons;
		this.machine.ReadPlayerDataPUN(this.player, stream, info);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x0000292C File Offset: 0x00000B2C
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext((int)this.currentButtonState);
		this.machine.WritePlayerDataPUN(this.player, stream, info);
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002789 File Offset: 0x00000989
	public void ReceiveRemoteState(ArcadeButtons newState)
	{
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002952 File Offset: 0x00000B52
	public bool TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x06000032 RID: 50 RVA: 0x0000295A File Offset: 0x00000B5A
	public override bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return !this.machine.IsControllerInUse(this.player);
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002970 File Offset: 0x00000B70
	public void ForceRelease()
	{
		this.heldByLocalPlayer = false;
		this.currentButtonState = (ArcadeButtons)0;
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002980 File Offset: 0x00000B80
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (this.heldByLocalPlayer && (toPlayer == null || !toPlayer.IsLocal))
		{
			this.ForceRelease();
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x0000299B File Offset: 0x00000B9B
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !this.heldByLocalPlayer;
	}

	// Token: 0x06000036 RID: 54 RVA: 0x0000299B File Offset: 0x00000B9B
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return !this.heldByLocalPlayer;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x0400001E RID: 30
	private XRNode xrNode;

	// Token: 0x04000022 RID: 34
	private ArcadeMachine machine;

	// Token: 0x04000023 RID: 35
	private RequestableOwnershipGuard guard;

	// Token: 0x04000024 RID: 36
	private GorillaSnapTurn snapTurn;

	// Token: 0x04000025 RID: 37
	private bool snapTurnOverride;
}
