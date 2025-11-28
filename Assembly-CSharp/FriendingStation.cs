using System;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000B7E RID: 2942
public class FriendingStation : MonoBehaviour
{
	// Token: 0x170006CE RID: 1742
	// (get) Token: 0x060048D4 RID: 18644 RVA: 0x0017EA3E File Offset: 0x0017CC3E
	public TextMeshProUGUI Player1Text
	{
		get
		{
			return this.player1Text;
		}
	}

	// Token: 0x170006CF RID: 1743
	// (get) Token: 0x060048D5 RID: 18645 RVA: 0x0017EA46 File Offset: 0x0017CC46
	public TextMeshProUGUI Player2Text
	{
		get
		{
			return this.player2Text;
		}
	}

	// Token: 0x170006D0 RID: 1744
	// (get) Token: 0x060048D6 RID: 18646 RVA: 0x0017EA4E File Offset: 0x0017CC4E
	public TextMeshProUGUI StatusText
	{
		get
		{
			return this.statusText;
		}
	}

	// Token: 0x170006D1 RID: 1745
	// (get) Token: 0x060048D7 RID: 18647 RVA: 0x0017EA56 File Offset: 0x0017CC56
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x060048D8 RID: 18648 RVA: 0x0017EA5E File Offset: 0x0017CC5E
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
	}

	// Token: 0x060048D9 RID: 18649 RVA: 0x0017EA90 File Offset: 0x0017CC90
	private void OnEnable()
	{
		FriendingManager.Instance.RegisterFriendingStation(this);
		if (PhotonNetwork.InRoom)
		{
			this.displayedData.actorNumberA = -1;
			this.displayedData.actorNumberB = -1;
			this.displayedData.state = FriendingManager.FriendStationState.WaitingForPlayers;
		}
		else
		{
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
		}
		this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
		this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
		this.UpdateDisplayedState(this.displayedData.state);
	}

	// Token: 0x060048DA RID: 18650 RVA: 0x0017EB3C File Offset: 0x0017CD3C
	private void OnDisable()
	{
		FriendingManager.Instance.UnregisterFriendingStation(this);
	}

	// Token: 0x060048DB RID: 18651 RVA: 0x0017EB4C File Offset: 0x0017CD4C
	private void UpdatePlayerText(TextMeshProUGUI playerText, int playerId)
	{
		if (playerId == -2)
		{
			playerText.text = "";
			return;
		}
		if (playerId == -1)
		{
			playerText.text = "PLAYER:\nNONE";
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerId);
		if (netPlayerByID != null)
		{
			playerText.text = "PLAYER:\n" + netPlayerByID.SanitizedNickName;
			return;
		}
		playerText.text = "PLAYER:\nNONE";
	}

	// Token: 0x060048DC RID: 18652 RVA: 0x0017EBAC File Offset: 0x0017CDAC
	private void UpdateDisplayedState(FriendingManager.FriendStationState state)
	{
		switch (state)
		{
		case FriendingManager.FriendStationState.NotInRoom:
			this.statusText.text = "JOIN A ROOM TO USE";
			return;
		case FriendingManager.FriendStationState.WaitingForPlayers:
			this.statusText.text = "";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusBoth:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonBoth:
			this.statusText.text = "PRESS [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerA:
			this.statusText.text = "PRESS [       ] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerB:
			this.statusText.text = "READY [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer0:
			this.statusText.text = "READY [       ] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer1:
			this.statusText.text = "READY [-     -] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer2:
			this.statusText.text = "READY [--   --] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer3:
			this.statusText.text = "READY [--- ---] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer4:
			this.statusText.text = "READY [-------] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestBoth:
			this.statusText.text = " SENT [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerA:
			this.statusText.text = " SENT [-------] DONE ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerB:
			this.statusText.text = " DONE [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.RequestFailed:
			this.statusText.text = "FRIEND REQUEST FAILED";
			return;
		case FriendingManager.FriendStationState.Friends:
			this.statusText.text = "\\O/ FRIENDS \\O/";
			return;
		case FriendingManager.FriendStationState.AlreadyFriends:
			this.statusText.text = "ALREADY FRIENDS";
			return;
		default:
			return;
		}
	}

	// Token: 0x060048DD RID: 18653 RVA: 0x0017ED50 File Offset: 0x0017CF50
	private void UpdateAddFriendButton()
	{
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
		{
			this.addFriendButton.isOn = true;
		}
		else
		{
			this.addFriendButton.isOn = false;
		}
		this.addFriendButton.UpdateColor();
	}

	// Token: 0x060048DE RID: 18654 RVA: 0x0017EDF0 File Offset: 0x0017CFF0
	private void UpdateDisplay(ref FriendingManager.FriendStationData data)
	{
		if (this.displayedData.actorNumberA != data.actorNumberA)
		{
			this.UpdatePlayerText(this.player1Text, data.actorNumberA);
		}
		if (this.displayedData.actorNumberB != data.actorNumberB)
		{
			this.UpdatePlayerText(this.player2Text, data.actorNumberB);
		}
		if (this.displayedData.state != data.state)
		{
			this.UpdateDisplayedState(data.state);
		}
		this.displayedData = data;
		this.UpdateAddFriendButton();
	}

	// Token: 0x060048DF RID: 18655 RVA: 0x0017EE78 File Offset: 0x0017D078
	public void UpdateState(FriendingManager.FriendStationData data)
	{
		this.UpdateDisplay(ref data);
	}

	// Token: 0x060048E0 RID: 18656 RVA: 0x0017EE84 File Offset: 0x0017D084
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null && component.OwningNetPlayer != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerEnteredStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x060048E1 RID: 18657 RVA: 0x0017EF5C File Offset: 0x0017D15C
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerExitedStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x060048E2 RID: 18658 RVA: 0x0017F02C File Offset: 0x0017D22C
	public void FriendButtonPressed()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		if (!this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonPressedRPC", 2, new object[]
			{
				this.zone
			});
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if (this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonBoth || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB))
			{
				this.addFriendButton.isOn = true;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x060048E3 RID: 18659 RVA: 0x0017F100 File Offset: 0x0017D300
	public void FriendButtonReleased()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		if (this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonUnpressedRPC", 2, new object[]
			{
				this.zone
			});
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
			{
				this.addFriendButton.isOn = false;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x0400595C RID: 22876
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x0400595D RID: 22877
	[SerializeField]
	private TextMeshProUGUI player1Text;

	// Token: 0x0400595E RID: 22878
	[SerializeField]
	private TextMeshProUGUI player2Text;

	// Token: 0x0400595F RID: 22879
	[SerializeField]
	private TextMeshProUGUI statusText;

	// Token: 0x04005960 RID: 22880
	[SerializeField]
	private GTZone zone;

	// Token: 0x04005961 RID: 22881
	[SerializeField]
	private GorillaPressableButton addFriendButton;

	// Token: 0x04005962 RID: 22882
	private FriendingManager.FriendStationData displayedData;
}
