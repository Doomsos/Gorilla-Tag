using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class JoinTriggerUI : MonoBehaviour
{
	// Token: 0x060011E3 RID: 4579 RVA: 0x0005E35F File Offset: 0x0005C55F
	private void Awake()
	{
		this.joinTrigger_isRefResolved = (this.joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.joinTrigger) && this.joinTrigger != null);
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0005E389 File Offset: 0x0005C589
	private void Start()
	{
		this.didStart = true;
		this.OnEnable();
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0005E398 File Offset: 0x0005C598
	private void OnEnable()
	{
		if (this.didStart && this._IsValid())
		{
			this.joinTrigger.RegisterUI(this);
		}
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0005E3B6 File Offset: 0x0005C5B6
	private void OnDisable()
	{
		if (this._IsValid())
		{
			this.joinTrigger.UnregisterUI(this);
		}
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0005E3CC File Offset: 0x0005C5CC
	public void SetState(JoinTriggerVisualState state, Func<string> oldZone, Func<string> newZone, Func<string> oldGameMode, Func<string> newGameMode)
	{
		switch (state)
		{
		case JoinTriggerVisualState.ConnectionError:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_Error;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_Error;
			this.screenText.text = (this.template.showFullErrorMessages ? GorillaScoreboardTotalUpdater.instance.offlineTextErrorString : this.template.ScreenText_Error);
			return;
		case JoinTriggerVisualState.AlreadyInRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AlreadyInRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AlreadyInRoom;
			this.screenText.text = this.template.ScreenText_AlreadyInRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.InPrivateRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_InPrivateRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_InPrivateRoom;
			this.screenText.text = this.template.ScreenText_InPrivateRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.NotConnectedSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_NotConnectedSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_NotConnectedSoloJoin;
			this.screenText.text = this.template.ScreenText_NotConnectedSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndSoloJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndPartyJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndGroupJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndGroupJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndGroupJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.AbandonPartyAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AbandonPartyAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AbandonPartyAndSoloJoin;
			this.screenText.text = this.template.ScreenText_AbandonPartyAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.ChangingGameModeSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_ChangingGameModeSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_ChangingGameModeSoloJoin;
			this.screenText.text = this.template.ScreenText_ChangingGameModeSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		default:
			return;
		}
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0005E680 File Offset: 0x0005C880
	private bool _IsValid()
	{
		if (!this.joinTrigger_isRefResolved)
		{
			if (this.joinTriggerRef.TargetID == 0)
			{
				Debug.LogError("ERROR!!!  JoinTriggerUI: XSceneRef `joinTriggerRef` is not assigned so could not resolve. Path=" + base.transform.GetPathQ(), this);
			}
			else
			{
				Debug.LogError("ERROR!!!  JoinTriggerUI: XSceneRef `joinTriggerRef` could not be resolved. Path=" + base.transform.GetPathQ(), this);
			}
		}
		return this.joinTrigger_isRefResolved;
	}

	// Token: 0x04001664 RID: 5732
	[SerializeField]
	private XSceneRef joinTriggerRef;

	// Token: 0x04001665 RID: 5733
	private GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04001666 RID: 5734
	private bool joinTrigger_isRefResolved;

	// Token: 0x04001667 RID: 5735
	[SerializeField]
	private MeshRenderer milestoneRenderer;

	// Token: 0x04001668 RID: 5736
	[SerializeField]
	private MeshRenderer screenBGRenderer;

	// Token: 0x04001669 RID: 5737
	[SerializeField]
	private TextMeshPro screenText;

	// Token: 0x0400166A RID: 5738
	[SerializeField]
	private JoinTriggerUITemplate template;

	// Token: 0x0400166B RID: 5739
	private bool didStart;
}
