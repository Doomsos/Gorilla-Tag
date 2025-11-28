using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x02000475 RID: 1141
public class CoconutMystic : MonoBehaviour
{
	// Token: 0x06001CF9 RID: 7417 RVA: 0x000993D0 File Offset: 0x000975D0
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x000993DE File Offset: 0x000975DE
	private void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += new Action<EventData>(this.OnPhotonEvent);
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x000993F6 File Offset: 0x000975F6
	private void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= new Action<EventData>(this.OnPhotonEvent);
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x00099410 File Offset: 0x00097610
	private void OnPhotonEvent(EventData evData)
	{
		if (evData.Code != 176)
		{
			return;
		}
		object[] array = (object[])evData.CustomData;
		object obj = array[0];
		if (!(obj is int))
		{
			return;
		}
		int num = (int)obj;
		if (num != CoconutMystic.kUpdateLabelEvent)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(evData.Sender);
		NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
		if (player != owningNetPlayer)
		{
			return;
		}
		int index = (int)array[1];
		this.label.text = this.answers.GetItem(index).GetLocalizedString();
		this.soundPlayer.Play();
		this.breakEffect.Play();
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x000994B4 File Offset: 0x000976B4
	public void UpdateLabel()
	{
		bool flag = this.geodeItem.currentState == TransferrableObject.PositionState.InLeftHand;
		this.label.rectTransform.localRotation = Quaternion.Euler(0f, flag ? 270f : 90f, 0f);
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x00099500 File Offset: 0x00097700
	public void ShowAnswer()
	{
		this.answers.distinct = this.distinct;
		this.label.text = this.answers.NextItem().GetLocalizedString();
		this.soundPlayer.Play();
		this.breakEffect.Play();
		object obj = new object[]
		{
			CoconutMystic.kUpdateLabelEvent,
			this.answers.lastItemIndex
		};
		PhotonNetwork.RaiseEvent(176, obj, RaiseEventOptions.Default, SendOptions.SendReliable);
	}

	// Token: 0x040026EA RID: 9962
	public VRRig rig;

	// Token: 0x040026EB RID: 9963
	public GeodeItem geodeItem;

	// Token: 0x040026EC RID: 9964
	public SoundBankPlayer soundPlayer;

	// Token: 0x040026ED RID: 9965
	public ParticleSystem breakEffect;

	// Token: 0x040026EE RID: 9966
	public RandomLocalizedStrings answers;

	// Token: 0x040026EF RID: 9967
	public TMP_Text label;

	// Token: 0x040026F0 RID: 9968
	public bool distinct;

	// Token: 0x040026F1 RID: 9969
	private static readonly int kUpdateLabelEvent = "CoconutMystic.UpdateLabel".GetStaticHash();
}
