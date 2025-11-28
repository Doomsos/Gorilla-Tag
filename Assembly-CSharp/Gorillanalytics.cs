using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;

// Token: 0x020007A1 RID: 1953
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x0600330A RID: 13066 RVA: 0x0011393E File Offset: 0x00111B3E
	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			double num;
			if (double.TryParse(s, ref num))
			{
				this.oneOverChance = num;
			}
		}, delegate(PlayFabError e)
		{
		}, false);
		for (;;)
		{
			yield return new WaitForSeconds(this.interval);
			if ((double)Random.Range(0f, 1f) < 1.0 / this.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				this.UploadGorillanalytics();
			}
		}
		yield break;
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x00113950 File Offset: 0x00111B50
	private void UploadGorillanalytics()
	{
		try
		{
			string map;
			string mode;
			string queue;
			this.GetMapModeQueue(out map, out mode, out queue);
			Vector3 position = GTPlayer.Instance.headCollider.transform.position;
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			this.uploadData.version = NetworkSystemConfig.AppVersion;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = map;
			this.uploadData.mode = mode;
			this.uploadData.queue = queue;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = averagedVelocity.x;
			this.uploadData.vel_y = averagedVelocity.y;
			this.uploadData.vel_z = averagedVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", Enumerable.Select<CosmeticsController.CosmeticItem, string>(CosmeticsController.instance.unlockedCosmetics, (CosmeticsController.CosmeticItem c) => c.itemName));
			this.uploadData.cosmetics_worn = string.Join(";", Enumerable.Select<CosmeticsController.CosmeticItem, string>(CosmeticsController.instance.currentWornSet.items, (CosmeticsController.CosmeticItem c) => c.itemName));
			GorillaServer.Instance.UploadGorillanalytics(this.uploadData);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x00113B20 File Offset: 0x00111D20
	private void GetMapModeQueue(out string map, out string mode, out string queue)
	{
		if (!PhotonNetwork.InRoom)
		{
			map = "none";
			mode = "none";
			queue = "none";
			return;
		}
		object obj = null;
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null)
		{
			currentRoom.CustomProperties.TryGetValue("gameMode", ref obj);
		}
		string gameMode = ((obj != null) ? obj.ToString() : null) ?? "";
		GTZone gtzone = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone;
		if (gtzone == GTZone.cityNoBuildings || gtzone == GTZone.cityWithSkyJungle || gtzone == GTZone.mall)
		{
			gtzone = GTZone.city;
		}
		if (gtzone == GTZone.tutorial)
		{
			gtzone = GTZone.forest;
		}
		if (gtzone == GTZone.ghostReactorTunnel)
		{
			gtzone = GTZone.ghostReactor;
		}
		map = gtzone.ToString().ToLower();
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			map += "private";
		}
		mode = (Enumerable.FirstOrDefault<string>(this.modes, (string s) => gameMode.Contains(s)) ?? "unknown");
		queue = (Enumerable.FirstOrDefault<string>(this.queues, (string s) => gameMode.Contains(s)) ?? "unknown");
	}

	// Token: 0x04004185 RID: 16773
	public float interval = 60f;

	// Token: 0x04004186 RID: 16774
	public double oneOverChance = 4320.0;

	// Token: 0x04004187 RID: 16775
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04004188 RID: 16776
	public GameModeZoneMapping gameModeData;

	// Token: 0x04004189 RID: 16777
	public List<string> maps;

	// Token: 0x0400418A RID: 16778
	public List<string> modes;

	// Token: 0x0400418B RID: 16779
	public List<string> queues;

	// Token: 0x0400418C RID: 16780
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x020007A2 RID: 1954
	private class UploadData
	{
		// Token: 0x0400418D RID: 16781
		public string version;

		// Token: 0x0400418E RID: 16782
		public double upload_chance;

		// Token: 0x0400418F RID: 16783
		public string map;

		// Token: 0x04004190 RID: 16784
		public string mode;

		// Token: 0x04004191 RID: 16785
		public string queue;

		// Token: 0x04004192 RID: 16786
		public int player_count;

		// Token: 0x04004193 RID: 16787
		public float pos_x;

		// Token: 0x04004194 RID: 16788
		public float pos_y;

		// Token: 0x04004195 RID: 16789
		public float pos_z;

		// Token: 0x04004196 RID: 16790
		public float vel_x;

		// Token: 0x04004197 RID: 16791
		public float vel_y;

		// Token: 0x04004198 RID: 16792
		public float vel_z;

		// Token: 0x04004199 RID: 16793
		public string cosmetics_owned;

		// Token: 0x0400419A RID: 16794
		public string cosmetics_worn;
	}
}
