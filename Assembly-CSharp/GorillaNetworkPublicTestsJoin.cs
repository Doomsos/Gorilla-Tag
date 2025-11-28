using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000B8E RID: 2958
public class GorillaNetworkPublicTestsJoin : GorillaTriggerBox, ITickSystemPost
{
	// Token: 0x170006D6 RID: 1750
	// (get) Token: 0x06004923 RID: 18723 RVA: 0x001807E8 File Offset: 0x0017E9E8
	// (set) Token: 0x06004924 RID: 18724 RVA: 0x001807F0 File Offset: 0x0017E9F0
	public bool PostTickRunning { get; set; }

	// Token: 0x06004925 RID: 18725 RVA: 0x001807F9 File Offset: 0x0017E9F9
	public void Awake()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004926 RID: 18726 RVA: 0x00180804 File Offset: 0x0017EA04
	public void PostTick()
	{
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				if ((GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f) && !this.waiting && !GorillaNot.instance.reportedPlayers.Contains(PhotonNetwork.LocalPlayer.UserId))
				{
					base.StartCoroutine(this.GracePeriod());
				}
				float magnitude = (GTPlayer.Instance.transform.position - this.lastPosition).magnitude;
			}
			this.lastPosition = GTPlayer.Instance.transform.position;
		}
		catch
		{
		}
	}

	// Token: 0x06004927 RID: 18727 RVA: 0x00180938 File Offset: 0x0017EB38
	private IEnumerator GracePeriod()
	{
		this.waiting = true;
		yield return new WaitForSeconds(30f);
		try
		{
			if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
			{
				if (GTPlayer.Instance.GetComponent<Rigidbody>().isKinematic)
				{
					GorillaNot.instance.SendReport("gorvity bisdabled", PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GTPlayer.Instance.jumpMultiplier > GorillaGameManager.instance.fastJumpMultiplier * 2f || GTPlayer.Instance.maxJumpSpeed > GorillaGameManager.instance.fastJumpLimit * 2f)
				{
					GorillaNot.instance.SendReport(string.Concat(new string[]
					{
						"jimp 2mcuh.",
						GTPlayer.Instance.jumpMultiplier.ToString(),
						".",
						GTPlayer.Instance.maxJumpSpeed.ToString(),
						"."
					}), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
				if (GorillaTagger.Instance.sphereCastRadius > 0.04f)
				{
					GorillaNot.instance.SendReport("wack rad. " + GorillaTagger.Instance.sphereCastRadius.ToString(), PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
				}
			}
			this.waiting = false;
			yield break;
		}
		catch
		{
			yield break;
		}
		yield break;
	}

	// Token: 0x040059AE RID: 22958
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x040059AF RID: 22959
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x040059B0 RID: 22960
	public string gameModeName;

	// Token: 0x040059B1 RID: 22961
	public PhotonNetworkController photonNetworkController;

	// Token: 0x040059B2 RID: 22962
	public string componentTypeToAdd;

	// Token: 0x040059B3 RID: 22963
	public GameObject componentTarget;

	// Token: 0x040059B4 RID: 22964
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x040059B5 RID: 22965
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x040059B6 RID: 22966
	private Transform tosPition;

	// Token: 0x040059B7 RID: 22967
	private Transform othsTosPosition;

	// Token: 0x040059B8 RID: 22968
	private PhotonView fotVew;

	// Token: 0x040059B9 RID: 22969
	private bool waiting;

	// Token: 0x040059BA RID: 22970
	private Vector3 lastPosition;

	// Token: 0x040059BB RID: 22971
	private VRRig tempRig;
}
