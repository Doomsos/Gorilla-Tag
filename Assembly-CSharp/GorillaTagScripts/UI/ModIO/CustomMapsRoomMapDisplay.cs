using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000E2D RID: 3629
	public class CustomMapsRoomMapDisplay : MonoBehaviour
	{
		// Token: 0x06005A8E RID: 23182 RVA: 0x001D07CC File Offset: 0x001CE9CC
		public void Start()
		{
			this.roomMapNameText.text = this.noRoomMapString;
			this.roomMapStatusText.text = this.notLoadedStatusString;
			this.roomMapLabelText.gameObject.SetActive(true);
			this.roomMapNameText.gameObject.SetActive(true);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnDisconnectedFromRoom);
			CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		// Token: 0x06005A8F RID: 23183 RVA: 0x001D08C4 File Offset: 0x001CEAC4
		public void OnDestroy()
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnDisconnectedFromRoom);
			CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		}

		// Token: 0x06005A90 RID: 23184 RVA: 0x001D0929 File Offset: 0x001CEB29
		private void OnJoinedRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06005A91 RID: 23185 RVA: 0x001D0929 File Offset: 0x001CEB29
		private void OnDisconnectedFromRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06005A92 RID: 23186 RVA: 0x001D0929 File Offset: 0x001CEB29
		private void OnRoomMapChanged(ModId roomMapModId)
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06005A93 RID: 23187 RVA: 0x001D0934 File Offset: 0x001CEB34
		private Task UpdateRoomMap()
		{
			CustomMapsRoomMapDisplay.<UpdateRoomMap>d__18 <UpdateRoomMap>d__;
			<UpdateRoomMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateRoomMap>d__.<>4__this = this;
			<UpdateRoomMap>d__.<>1__state = -1;
			<UpdateRoomMap>d__.<>t__builder.Start<CustomMapsRoomMapDisplay.<UpdateRoomMap>d__18>(ref <UpdateRoomMap>d__);
			return <UpdateRoomMap>d__.<>t__builder.Task;
		}

		// Token: 0x06005A94 RID: 23188 RVA: 0x001D0978 File Offset: 0x001CEB78
		private void OnMapLoadComplete(bool success)
		{
			if (success)
			{
				this.roomMapStatusText.text = this.readyToPlayStatusString;
				this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				return;
			}
			this.roomMapStatusText.text = this.loadFailedStatusString;
			this.roomMapStatusText.color = this.loadFailedStatusStringColor;
		}

		// Token: 0x06005A95 RID: 23189 RVA: 0x001D09CD File Offset: 0x001CEBCD
		private void OnMapLoadProgress(MapLoadStatus status, int progress, string message)
		{
			if (status - MapLoadStatus.Downloading <= 1)
			{
				this.roomMapStatusText.text = this.loadingStatusString;
				this.roomMapStatusText.color = this.loadingStatusStringColor;
			}
		}

		// Token: 0x040067C5 RID: 26565
		[SerializeField]
		private TMP_Text roomMapLabelText;

		// Token: 0x040067C6 RID: 26566
		[SerializeField]
		private TMP_Text roomMapNameText;

		// Token: 0x040067C7 RID: 26567
		[SerializeField]
		private TMP_Text roomMapStatusLabelText;

		// Token: 0x040067C8 RID: 26568
		[SerializeField]
		private TMP_Text roomMapStatusText;

		// Token: 0x040067C9 RID: 26569
		[SerializeField]
		private string noRoomMapString = "NONE";

		// Token: 0x040067CA RID: 26570
		[SerializeField]
		private string notLoadedStatusString = "NOT LOADED";

		// Token: 0x040067CB RID: 26571
		[SerializeField]
		private string loadingStatusString = "LOADING...";

		// Token: 0x040067CC RID: 26572
		[SerializeField]
		private string readyToPlayStatusString = "READY!";

		// Token: 0x040067CD RID: 26573
		[SerializeField]
		private string loadFailedStatusString = "LOAD FAILED";

		// Token: 0x040067CE RID: 26574
		[SerializeField]
		private Color notLoadedStatusStringColor = Color.red;

		// Token: 0x040067CF RID: 26575
		[SerializeField]
		private Color loadingStatusStringColor = Color.yellow;

		// Token: 0x040067D0 RID: 26576
		[SerializeField]
		private Color readyToPlayStatusStringColor = Color.green;

		// Token: 0x040067D1 RID: 26577
		[SerializeField]
		private Color loadFailedStatusStringColor = Color.red;
	}
}
