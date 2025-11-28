using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004F3 RID: 1267
public class RubberDuckEvents : MonoBehaviour
{
	// Token: 0x06002096 RID: 8342 RVA: 0x000AD284 File Offset: 0x000AB484
	public void Init(NetPlayer player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool isLocal = player.IsLocal;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (isLocal && instance != null)
			{
				text = instance.GetPlayFabPlayerId();
			}
			else
			{
				text = player.NickName;
			}
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.Dispose();
		this.Activate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Activate"));
		this.Deactivate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Deactivate"));
		this.Activate.reliable = true;
		this.Deactivate.reliable = true;
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x000AD35E File Offset: 0x000AB55E
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

	// Token: 0x06002098 RID: 8344 RVA: 0x000AD381 File Offset: 0x000AB581
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

	// Token: 0x06002099 RID: 8345 RVA: 0x000AD3A4 File Offset: 0x000AB5A4
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x000AD3AC File Offset: 0x000AB5AC
	public void Dispose()
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

	// Token: 0x04002B33 RID: 11059
	public int PlayerId;

	// Token: 0x04002B34 RID: 11060
	public string PlayerIdString;

	// Token: 0x04002B35 RID: 11061
	public PhotonEvent Activate;

	// Token: 0x04002B36 RID: 11062
	public PhotonEvent Deactivate;
}
