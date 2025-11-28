using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003F3 RID: 1011
public class PUNCallbackNotifier : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x060018C6 RID: 6342 RVA: 0x000855EA File Offset: 0x000837EA
	private void Start()
	{
		this.parentSystem = base.GetComponent<NetworkSystemPUN>();
	}

	// Token: 0x060018C7 RID: 6343 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x000855F8 File Offset: 0x000837F8
	public override void OnConnectedToMaster()
	{
		this.parentSystem.OnConnectedtoMaster();
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x00085605 File Offset: 0x00083805
	public override void OnJoinedRoom()
	{
		this.parentSystem.OnJoinedRoom();
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x00085612 File Offset: 0x00083812
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x00085612 File Offset: 0x00083812
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x00085621 File Offset: 0x00083821
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnCreateRoomFailed(returnCode, message);
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x00085630 File Offset: 0x00083830
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.parentSystem.OnPlayerEnteredRoom(newPlayer);
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x0008563E File Offset: 0x0008383E
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.parentSystem.OnPlayerLeftRoom(otherPlayer);
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x0008564C File Offset: 0x0008384C
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnect callback, cause:" + cause.ToString());
		this.parentSystem.OnDisconnected(cause);
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x00085676 File Offset: 0x00083876
	public void OnEvent(EventData photonEvent)
	{
		this.parentSystem.RaiseEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x00085695 File Offset: 0x00083895
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		this.parentSystem.OnMasterClientSwitched(newMasterClient);
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x000856A3 File Offset: 0x000838A3
	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		base.OnCustomAuthenticationResponse(data);
		NetworkSystem.Instance.CustomAuthenticationResponse(data);
	}

	// Token: 0x04002239 RID: 8761
	private NetworkSystemPUN parentSystem;
}
