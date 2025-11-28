using System;
using System.Collections;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000B8C RID: 2956
public class GorillaNetworkPublicTestJoin2 : GorillaTriggerBox
{
	// Token: 0x06004919 RID: 18713 RVA: 0x00002789 File Offset: 0x00000989
	public void Awake()
	{
	}

	// Token: 0x0600491A RID: 18714 RVA: 0x001804F0 File Offset: 0x0017E6F0
	public void LateUpdate()
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

	// Token: 0x0600491B RID: 18715 RVA: 0x00180624 File Offset: 0x0017E824
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

	// Token: 0x0400599D RID: 22941
	public GameObject[] makeSureThisIsDisabled;

	// Token: 0x0400599E RID: 22942
	public GameObject[] makeSureThisIsEnabled;

	// Token: 0x0400599F RID: 22943
	public string gameModeName;

	// Token: 0x040059A0 RID: 22944
	public PhotonNetworkController photonNetworkController;

	// Token: 0x040059A1 RID: 22945
	public string componentTypeToAdd;

	// Token: 0x040059A2 RID: 22946
	public GameObject componentTarget;

	// Token: 0x040059A3 RID: 22947
	public GorillaLevelScreen[] joinScreens;

	// Token: 0x040059A4 RID: 22948
	public GorillaLevelScreen[] leaveScreens;

	// Token: 0x040059A5 RID: 22949
	private Transform tosPition;

	// Token: 0x040059A6 RID: 22950
	private Transform othsTosPosition;

	// Token: 0x040059A7 RID: 22951
	private PhotonView fotVew;

	// Token: 0x040059A8 RID: 22952
	private bool waiting;

	// Token: 0x040059A9 RID: 22953
	private Vector3 lastPosition;

	// Token: 0x040059AA RID: 22954
	private VRRig tempRig;
}
