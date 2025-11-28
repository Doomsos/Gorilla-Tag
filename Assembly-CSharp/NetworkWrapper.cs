using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003DE RID: 990
public class NetworkWrapper : MonoBehaviour
{
	// Token: 0x06001840 RID: 6208 RVA: 0x00081E63 File Offset: 0x00080063
	[RuntimeInitializeOnLoadMethod(1)]
	public static void AutoInstantiate()
	{
		Object.DontDestroyOnLoad(Object.Instantiate<GameObject>(Resources.Load<GameObject>("P_NetworkWrapper")));
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x00081E7C File Offset: 0x0008007C
	private void Awake()
	{
		if (this.titleRef != null)
		{
			this.titleRef.text = "PUN";
		}
		this.activeNetworkSystem = base.gameObject.AddComponent<NetworkSystemPUN>();
		this.activeNetworkSystem.AddVoiceSettings(this.VoiceSettings);
		this.activeNetworkSystem.config = this.netSysConfig;
		this.activeNetworkSystem.regionNames = this.networkRegionNames;
		this.activeNetworkSystem.OnPlayerJoined += new Action<NetPlayer>(this.UpdatePlayerCountWrapper);
		this.activeNetworkSystem.OnPlayerLeft += new Action<NetPlayer>(this.UpdatePlayerCountWrapper);
		this.activeNetworkSystem.OnMultiplayerStarted += new Action(this.UpdatePlayerCount);
		this.activeNetworkSystem.OnReturnedToSinglePlayer += new Action(this.UpdatePlayerCount);
		Debug.Log("<color=green>initialize Network System</color>");
		this.activeNetworkSystem.Initialise();
	}

	// Token: 0x06001842 RID: 6210 RVA: 0x00081F88 File Offset: 0x00080188
	private void UpdatePlayerCountWrapper(NetPlayer player)
	{
		this.UpdatePlayerCount();
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x00081F90 File Offset: 0x00080190
	private void UpdatePlayerCount()
	{
		if (this.playerCountTextRef == null)
		{
			return;
		}
		if (!this.activeNetworkSystem.IsOnline)
		{
			this.playerCountTextRef.text = string.Format("0/{0}", this.netSysConfig.MaxPlayerCount);
			Debug.Log("Player count updated");
			return;
		}
		Debug.Log("Player count not updated");
		this.playerCountTextRef.text = string.Format("{0}/{1}", this.activeNetworkSystem.AllNetPlayers.Length, this.netSysConfig.MaxPlayerCount);
	}

	// Token: 0x040021A6 RID: 8614
	[HideInInspector]
	public NetworkSystem activeNetworkSystem;

	// Token: 0x040021A7 RID: 8615
	public Text titleRef;

	// Token: 0x040021A8 RID: 8616
	[Header("NetSys settings")]
	public NetworkSystemConfig netSysConfig;

	// Token: 0x040021A9 RID: 8617
	public string[] networkRegionNames;

	// Token: 0x040021AA RID: 8618
	public string[] devNetworkRegionNames;

	// Token: 0x040021AB RID: 8619
	[Header("Debug output refs")]
	public Text stateTextRef;

	// Token: 0x040021AC RID: 8620
	public Text playerCountTextRef;

	// Token: 0x040021AD RID: 8621
	[SerializeField]
	private SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x040021AE RID: 8622
	private const string WrapperResourcePath = "P_NetworkWrapper";
}
