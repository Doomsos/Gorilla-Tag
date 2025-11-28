using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000759 RID: 1881
internal class GorillaSerializerScene : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x0600309B RID: 12443 RVA: 0x00109E41 File Offset: 0x00108041
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x00109E50 File Offset: 0x00108050
	protected virtual void Start()
	{
		if (!this.targetComponent.IsNull())
		{
			IGorillaSerializeableScene gorillaSerializeableScene = this.targetComponent as IGorillaSerializeableScene;
			if (gorillaSerializeableScene != null)
			{
				gorillaSerializeableScene.OnSceneLinking(this);
				this.serializeTarget = gorillaSerializeableScene;
				this.sceneSerializeTarget = gorillaSerializeableScene;
				this.successfullInstantiate = true;
				this.photonView.AddCallbackTarget(this);
				return;
			}
		}
		Debug.LogError("GorillaSerializerscene: missing target component or invalid target", base.gameObject);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x00109EBE File Offset: 0x001080BE
	private void OnEnable()
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		if (!this.validDisable)
		{
			this.validDisable = true;
			return;
		}
		this.OnValidEnable();
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x00109EDF File Offset: 0x001080DF
	protected virtual void OnValidEnable()
	{
		this.sceneSerializeTarget.OnNetworkObjectEnable();
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x00109EEC File Offset: 0x001080EC
	private void OnDisable()
	{
		if (!this.successfullInstantiate || !this.validDisable)
		{
			return;
		}
		this.OnValidDisable();
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x00109F05 File Offset: 0x00108105
	protected virtual void OnValidDisable()
	{
		this.sceneSerializeTarget.OnNetworkObjectDisable();
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x00109F14 File Offset: 0x00108114
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		GorillaNot.instance.SendReport("bad net obj creation", info.Sender.UserId, info.Sender.NickName);
		if (info.photonView.IsMine)
		{
			PhotonNetwork.Destroy(info.photonView);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x00109F6C File Offset: 0x0010816C
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.validDisable = false;
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x00109F75 File Offset: 0x00108175
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		if (!this.transferrable)
		{
			return info.Sender == PhotonNetwork.MasterClient;
		}
		return base.ValidOnSerialize(stream, info);
	}

	// Token: 0x04003FA3 RID: 16291
	[SerializeField]
	private bool transferrable;

	// Token: 0x04003FA4 RID: 16292
	[SerializeField]
	private MonoBehaviour targetComponent;

	// Token: 0x04003FA5 RID: 16293
	private IGorillaSerializeableScene sceneSerializeTarget;

	// Token: 0x04003FA6 RID: 16294
	protected bool validDisable = true;
}
