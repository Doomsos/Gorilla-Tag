using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020005ED RID: 1517
public class FortuneTeller : MonoBehaviourPunCallbacks
{
	// Token: 0x0600262A RID: 9770 RVA: 0x000CC084 File Offset: 0x000CA284
	private void Awake()
	{
		if (this.changeMaterialsInGreyZone && GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Combine(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Combine(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x000CC0F8 File Offset: 0x000CA2F8
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Remove(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Remove(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x000CC164 File Offset: 0x000CA364
	public override void OnEnable()
	{
		base.OnEnable();
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		if (this.button)
		{
			this.button.onPressed += new Action<GorillaPressableButton, bool>(this.HandlePressedButton);
		}
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x000CC1A2 File Offset: 0x000CA3A2
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.button)
		{
			this.button.onPressed -= new Action<GorillaPressableButton, bool>(this.HandlePressedButton);
		}
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000CC1CE File Offset: 0x000CA3CE
	private void GreyZoneActivated()
	{
		this.boothRenderer.material = this.boothGreyZoneMaterial;
		this.beardRenderer.material = this.beardGreyZoneMaterial;
		this.tellerRenderer.SetMaterials(this.tellerGreyZoneMaterials);
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000CC203 File Offset: 0x000CA403
	private void GreyZoneDeactivated()
	{
		this.boothRenderer.material = this.boothDefaultMaterial;
		this.beardRenderer.material = this.beardDefaultMaterial;
		this.tellerRenderer.SetMaterials(this.tellerDefaultMaterials);
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000CC238 File Offset: 0x000CA438
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			base.photonView.RPC("TriggerUpdateFortuneRPC", newPlayer, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000CC29C File Offset: 0x000CA49C
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000CC29C File Offset: 0x000CA49C
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000CC2AB File Offset: 0x000CA4AB
	private void HandlePressedButton(GorillaPressableButton button, bool isLeft)
	{
		if (base.photonView.IsMine)
		{
			this.SendNewFortune();
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("RequestFortuneRPC", 2, Array.Empty<object>());
		}
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x000CC2E0 File Offset: 0x000CA4E0
	[PunRPC]
	private void RequestFortuneRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestFortune");
		RigContainer rigContainer;
		if (NetworkSystem.Instance.IsMasterClient && info.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			CallLimitType<CallLimiter> callLimitType = rigContainer.Rig.fxSettings.callSettings[(int)this.limiterType];
			if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
			{
				this.SendNewFortune();
			}
		}
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000CC36C File Offset: 0x000CA56C
	private void SendNewFortune()
	{
		if (this.playable.time > 0.0 && this.playable.time < this.playable.duration)
		{
			return;
		}
		this.latestFortune = this.results.GetResult();
		this.UpdateFortune(this.latestFortune, true);
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("TriggerNewFortuneRPC", 1, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000CC40C File Offset: 0x000CA60C
	[PunRPC]
	private void TriggerUpdateFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerUpdateFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerUpdateFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerUpdateFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.UpdateFortune(this.latestFortune, false);
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000CC488 File Offset: 0x000CA688
	[PunRPC]
	private void TriggerNewFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerNewFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerNewFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerNewFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		this.UpdateFortune(this.latestFortune, true);
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000CC514 File Offset: 0x000CA714
	private void StartAttractModeMonitor()
	{
		if (this.attractModeMonitor == null)
		{
			this.attractModeMonitor = base.StartCoroutine(this.AttractModeMonitor());
		}
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000CC530 File Offset: 0x000CA730
	private IEnumerator AttractModeMonitor()
	{
		while (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			if (Time.time >= this.nextAttractAnimTimestamp)
			{
				this.SendAttractAnim();
			}
			yield return new WaitForSeconds(this.nextAttractAnimTimestamp - Time.time);
		}
		this.attractModeMonitor = null;
		yield break;
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x000CC53F File Offset: 0x000CA73F
	private void SendAttractAnim()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
		{
			base.photonView.RPC("TriggerAttractAnimRPC", 0, Array.Empty<object>());
		}
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x000CC568 File Offset: 0x000CA768
	[PunRPC]
	private void TriggerAttractAnimRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TriggerAttractAnim");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			GorillaNot.instance.SendReport("Sent TriggerAttractAnim when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.animator.SetTrigger(this.trigger_attract);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x000CC5E0 File Offset: 0x000CA7E0
	private void UpdateFortune(FortuneResults.FortuneResult result, bool newFortune)
	{
		if (this.results)
		{
			PlayableAsset resultFanfare = this.GetResultFanfare(result.fortuneType);
			if (resultFanfare)
			{
				this.playable.initialTime = (newFortune ? 0.0 : resultFanfare.duration);
				this.playable.Play(resultFanfare, 0);
				this.animator.SetTrigger(this.trigger_prediction);
				this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
			}
		}
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x000CC663 File Offset: 0x000CA863
	public void ApplyFortuneText()
	{
		this.text.text = this.results.GetResultText(this.latestFortune).ToUpper();
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000CC688 File Offset: 0x000CA888
	private PlayableAsset GetResultFanfare(FortuneResults.FortuneCategoryType fortuneType)
	{
		foreach (FortuneTeller.FortuneTellerResultFanfare fortuneTellerResultFanfare in this.resultFanfares)
		{
			if (fortuneTellerResultFanfare.type == fortuneType)
			{
				return fortuneTellerResultFanfare.fanfare;
			}
		}
		return null;
	}

	// Token: 0x04003209 RID: 12809
	[SerializeField]
	private FXType limiterType;

	// Token: 0x0400320A RID: 12810
	[SerializeField]
	private FortuneTellerButton button;

	// Token: 0x0400320B RID: 12811
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x0400320C RID: 12812
	[SerializeField]
	private FortuneResults results;

	// Token: 0x0400320D RID: 12813
	[SerializeField]
	private PlayableDirector playable;

	// Token: 0x0400320E RID: 12814
	[SerializeField]
	private Animator animator;

	// Token: 0x0400320F RID: 12815
	[SerializeField]
	private float waitDurationBeforeAttractAnim;

	// Token: 0x04003210 RID: 12816
	[SerializeField]
	private FortuneTeller.FortuneTellerResultFanfare[] resultFanfares;

	// Token: 0x04003211 RID: 12817
	[Header("Grey Zone Visuals")]
	[SerializeField]
	private bool changeMaterialsInGreyZone;

	// Token: 0x04003212 RID: 12818
	[SerializeField]
	private MeshRenderer boothRenderer;

	// Token: 0x04003213 RID: 12819
	[SerializeField]
	private Material boothDefaultMaterial;

	// Token: 0x04003214 RID: 12820
	[SerializeField]
	private Material boothGreyZoneMaterial;

	// Token: 0x04003215 RID: 12821
	[SerializeField]
	private MeshRenderer beardRenderer;

	// Token: 0x04003216 RID: 12822
	[SerializeField]
	private Material beardDefaultMaterial;

	// Token: 0x04003217 RID: 12823
	[SerializeField]
	private Material beardGreyZoneMaterial;

	// Token: 0x04003218 RID: 12824
	[SerializeField]
	private SkinnedMeshRenderer tellerRenderer;

	// Token: 0x04003219 RID: 12825
	[SerializeField]
	private List<Material> tellerDefaultMaterials;

	// Token: 0x0400321A RID: 12826
	[SerializeField]
	private List<Material> tellerGreyZoneMaterials;

	// Token: 0x0400321B RID: 12827
	private FortuneResults.FortuneResult latestFortune;

	// Token: 0x0400321C RID: 12828
	private CallLimiter triggerNewFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x0400321D RID: 12829
	private CallLimiter triggerUpdateFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x0400321E RID: 12830
	private AnimHashId trigger_attract = "Attract";

	// Token: 0x0400321F RID: 12831
	private AnimHashId trigger_prediction = "Prediction";

	// Token: 0x04003220 RID: 12832
	private float nextAttractAnimTimestamp;

	// Token: 0x04003221 RID: 12833
	private Coroutine attractModeMonitor;

	// Token: 0x020005EE RID: 1518
	[Serializable]
	public struct FortuneTellerResultFanfare
	{
		// Token: 0x04003222 RID: 12834
		public FortuneResults.FortuneCategoryType type;

		// Token: 0x04003223 RID: 12835
		public PlayableAsset fanfare;
	}
}
