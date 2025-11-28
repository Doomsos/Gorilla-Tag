using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003CA RID: 970
[RequireComponent(typeof(PhotonView))]
public class NetworkSceneObject : SimulationBehaviour
{
	// Token: 0x1700025E RID: 606
	// (get) Token: 0x0600176D RID: 5997 RVA: 0x00080C0C File Offset: 0x0007EE0C
	public bool IsMine
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x00080C19 File Offset: 0x0007EE19
	protected virtual void Start()
	{
		if (this.photonView == null)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x00080C35 File Offset: 0x0007EE35
	protected virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x00080C3D File Offset: 0x0007EE3D
	protected virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x00080C48 File Offset: 0x0007EE48
	private void RegisterOnRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.AddGlobal(this);
		}
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x00080C80 File Offset: 0x0007EE80
	private void RemoveFromRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.RemoveGlobal(this);
		}
	}

	// Token: 0x04002144 RID: 8516
	public PhotonView photonView;
}
