using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000137 RID: 311
public class SIResourceDeposit : MonoBehaviour, ISIResourceDeposit
{
	// Token: 0x17000096 RID: 150
	// (get) Token: 0x0600084D RID: 2125 RVA: 0x0002D076 File Offset: 0x0002B276
	public bool IsAuthority
	{
		get
		{
			return this.SIManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x0600084E RID: 2126 RVA: 0x0002D088 File Offset: 0x0002B288
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.superInfection.siManager;
		}
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002D098 File Offset: 0x0002B298
	private void OnEnable()
	{
		if (this._displayResources == null || this._displayResources.Count == 0)
		{
			List<SIResource> resourcePrefabs = this.superInfection.ResourcePrefabs;
			if (resourcePrefabs != null && resourcePrefabs.Count > 0)
			{
				this._displayResources = new List<GameObject>();
				for (int i = 0; i < Mathf.Min(resourcePrefabs.Count, this.resourceDisplays.Length); i++)
				{
					GameObject gameObject = resourcePrefabs[i].gameObject;
					bool activeSelf = gameObject.activeSelf;
					try
					{
						if (activeSelf)
						{
							gameObject.SetActive(false);
						}
						GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, this.resourceDisplays[i].transform);
						gameObject2.transform.localScale = new Vector3(0.27f, 0.27f, 0.27f);
						this._displayResources.Add(gameObject2);
						foreach (MonoBehaviour monoBehaviour in gameObject2.GetComponentsInChildren<MonoBehaviour>(true))
						{
							monoBehaviour.enabled = false;
							Object.Destroy(monoBehaviour);
						}
						Rigidbody component = gameObject2.GetComponent<Rigidbody>();
						if (component != null)
						{
							Object.Destroy(component);
						}
						gameObject2.SetLayerRecursively(UnityLayer.Default);
						gameObject2.SetActive(true);
					}
					finally
					{
						if (activeSelf)
						{
							gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002D1DC File Offset: 0x0002B3DC
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.netPlayer != null)
		{
			stream.SendNext(this.netPlayer.ActorNr);
		}
		else
		{
			stream.SendNext(-1);
		}
		stream.SendNext((int)this.netResourceType);
		stream.SendNext((int)this.netLimitedDepositType);
		stream.SendNext(this.netShowPopup);
		this.netShowPopup = false;
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0002D258 File Offset: 0x0002B458
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.netPlayer = SIPlayer.Get((int)stream.ReceiveNext());
		this.netResourceType = (SIResource.ResourceType)((int)stream.ReceiveNext());
		this.netLimitedDepositType = (SIResource.LimitedDepositType)((int)stream.ReceiveNext());
		if ((bool)stream.ReceiveNext())
		{
			this.LocalShowPopup(this.netPlayer, this.netResourceType, this.netLimitedDepositType);
		}
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0002D2C4 File Offset: 0x0002B4C4
	private void LocalShowPopup(SIPlayer player, SIResource.ResourceType resourceType, SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			this.depositBin.SetActive(true);
		}
		this.popupScreen.EnableAndResetTimer();
		this.depositText.text = string.Format("{0} COLLECTED {1}\n(TOTAL {2})", player.gamePlayer.rig.Creator.SanitizedNickName, resourceType.GetName<SIResource.ResourceType>(), player.GetResourceAmount(resourceType));
		this.depositImage.sprite = ((resourceType == SIResource.ResourceType.TechPoint) ? this.resourceImageSprites[0] : this.resourceImageSprites[1]);
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002D348 File Offset: 0x0002B548
	public void ResourceDeposited(SIResource resource)
	{
		bool flag = false;
		if (resource.lastPlayerHeld.gamePlayer.IsLocal() && !resource.localDeposited)
		{
			this.AuthShowPopup(resource);
			resource.HandleDepositLocal(resource.lastPlayerHeld);
			resource.lastPlayerHeld.GatherResource(resource.type, resource.limitedDepositType, 1);
			this.superInfection.siManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.ResourceDepositDeposited, new object[]
			{
				resource.myGameEntity.GetNetId(),
				this.index
			});
			flag = true;
		}
		if (this.superInfection.siManager.gameEntityManager.IsAuthority())
		{
			resource.HandleDepositAuth(resource.lastPlayerHeld);
			this.superInfection.siManager.gameEntityManager.RequestDestroyItem(resource.myGameEntity.id);
			this.AuthShowPopup(resource);
			flag = true;
		}
		if (flag)
		{
			this.LocalShowPopup(resource.lastPlayerHeld, resource.type, resource.limitedDepositType);
		}
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002D43D File Offset: 0x0002B63D
	private void AuthShowPopup(SIResource resource)
	{
		this.netPlayer = resource.lastPlayerHeld;
		this.netResourceType = resource.type;
		this.netLimitedDepositType = resource.limitedDepositType;
		this.netShowPopup = true;
	}

	// Token: 0x04000A29 RID: 2601
	public int index;

	// Token: 0x04000A2A RID: 2602
	public Text depositText;

	// Token: 0x04000A2B RID: 2603
	public Image depositImage;

	// Token: 0x04000A2C RID: 2604
	public DisableGameObjectDelayed popupScreen;

	// Token: 0x04000A2D RID: 2605
	public SuperInfection superInfection;

	// Token: 0x04000A2E RID: 2606
	public Sprite[] resourceImageSprites;

	// Token: 0x04000A2F RID: 2607
	public GameObject depositBin;

	// Token: 0x04000A30 RID: 2608
	[SerializeField]
	private Transform[] resourceDisplays;

	// Token: 0x04000A31 RID: 2609
	public SIPlayer netPlayer;

	// Token: 0x04000A32 RID: 2610
	public SIResource.ResourceType netResourceType;

	// Token: 0x04000A33 RID: 2611
	public SIResource.LimitedDepositType netLimitedDepositType;

	// Token: 0x04000A34 RID: 2612
	private bool netShowPopup;

	// Token: 0x04000A35 RID: 2613
	public List<SIUIPlayerQuestDisplay> questDisplays;

	// Token: 0x04000A36 RID: 2614
	private List<GameObject> _displayResources;
}
