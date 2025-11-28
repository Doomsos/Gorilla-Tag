using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class UseableObjectEvents : MonoBehaviour
{
	// Token: 0x060020BB RID: 8379 RVA: 0x000AD990 File Offset: 0x000ABB90
	public void Init(NetPlayer player)
	{
		bool isLocal = player.IsLocal;
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string text;
		if (isLocal && instance != null)
		{
			text = instance.GetPlayFabPlayerId();
		}
		else
		{
			text = player.NickName;
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.DisposeEvents();
		this.Activate = new PhotonEvent(this.PlayerId.ToString() + ".Activate");
		this.Deactivate = new PhotonEvent(this.PlayerId.ToString() + ".Deactivate");
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x000ADA51 File Offset: 0x000ABC51
	private void OnEnable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Enable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Enable();
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x000ADA74 File Offset: 0x000ABC74
	private void OnDisable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Disable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Disable();
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x000ADA97 File Offset: 0x000ABC97
	private void OnDestroy()
	{
		this.DisposeEvents();
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x000ADA9F File Offset: 0x000ABC9F
	private void DisposeEvents()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Dispose();
		}
		this.Activate = null;
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate != null)
		{
			deactivate.Dispose();
		}
		this.Deactivate = null;
	}

	// Token: 0x04002B53 RID: 11091
	[NonSerialized]
	private string PlayerIdString;

	// Token: 0x04002B54 RID: 11092
	[NonSerialized]
	private int PlayerId;

	// Token: 0x04002B55 RID: 11093
	public PhotonEvent Activate;

	// Token: 0x04002B56 RID: 11094
	public PhotonEvent Deactivate;
}
