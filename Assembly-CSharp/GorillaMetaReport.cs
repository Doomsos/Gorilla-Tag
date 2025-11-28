using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Oculus.Platform;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// Token: 0x02000402 RID: 1026
public class GorillaMetaReport : MonoBehaviour
{
	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06001904 RID: 6404 RVA: 0x00085C54 File Offset: 0x00083E54
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x00085C5B File Offset: 0x00083E5B
	private void Start()
	{
		this.localPlayer.inOverlay = false;
		MothershipClientApiUnity.OnMessageNotificationSocket += new Action<NotificationsMessageResponse, IntPtr>(this.OnNotification);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x00085C86 File Offset: 0x00083E86
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.localPlayer.inOverlay = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x00085CA4 File Offset: 0x00083EA4
	private void OnReportButtonIntentNotif(Message<string> message)
	{
		if (message.IsError)
		{
			AbuseReport.ReportRequestHandled(2);
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			this.ReportText.SetActive(true);
			AbuseReport.ReportRequestHandled(1);
			this.StartOverlay(false);
			return;
		}
		if (!message.IsError)
		{
			AbuseReport.ReportRequestHandled(1);
			this.StartOverlay(false);
		}
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x00085CFC File Offset: 0x00083EFC
	private void OnNotification(NotificationsMessageResponse notification, [NativeInteger] IntPtr _)
	{
		string title = notification.Title;
		if (title == "Warning")
		{
			this.OnWarning(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Warning");
			return;
		}
		if (title == "Mute")
		{
			this.OnMuteSanction(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Mute");
			return;
		}
		if (!(title == "Unmute"))
		{
			return;
		}
		if (GorillaTagger.hasInstance)
		{
			GorillaTagger.moderationMutedTime = -1f;
		}
		GorillaTelemetry.PostNotificationEvent("Unmute");
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x00085D84 File Offset: 0x00083F84
	private void OnWarning(string warningNotification)
	{
		string[] array = warningNotification.Split('|', 0);
		if (array.Length != 2)
		{
			Debug.LogError("Invalid warning notification");
			return;
		}
		string text = array[0];
		string[] array2 = array[1].Split(',', 0);
		if (array2.Length == 0)
		{
			Debug.LogError("Missing warning notification reasons");
			return;
		}
		string text2 = GorillaMetaReport.FormatListToString(array2);
		this.ReportText.GetComponent<Text>().text = text.ToUpper() + " WARNING FOR " + text2.ToUpper();
		this.StartOverlay(true);
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x00085E00 File Offset: 0x00084000
	private void OnMuteSanction(string muteNotification)
	{
		string[] array = muteNotification.Split('|', 0);
		if (array.Length != 3)
		{
			Debug.LogError("Invalid mute notification");
			return;
		}
		if (!array[0].Equals("voice", 5))
		{
			return;
		}
		int num;
		if (array[2].Length > 0 && int.TryParse(array[2], ref num))
		{
			int num2 = num / 60;
			this.ReportText.GetComponent<Text>().text = string.Format("MUTED FOR {0} MINUTES\nBAD MONKE", num2);
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = (float)num;
			}
		}
		else
		{
			this.ReportText.GetComponent<Text>().text = "MUTED FOREVER";
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = float.PositiveInfinity;
			}
		}
		this.StartOverlay(true);
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x00085EB4 File Offset: 0x000840B4
	private static string FormatListToString(in string[] list)
	{
		int num = list.Length;
		string result;
		if (num != 1)
		{
			if (num != 2)
			{
				string text = RuntimeHelpers.GetSubArray<string>(list, Range.EndAt(new Index(1, true))).Join(", ");
				string text2 = ", AND ";
				string[] array = list;
				result = text + text2 + array[array.Length - 1];
			}
			else
			{
				result = list[0] + " AND " + list[1];
			}
		}
		else
		{
			result = list[0];
		}
		return result;
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x00085F1D File Offset: 0x0008411D
	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		this.Teardown();
		yield break;
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x00085F2C File Offset: 0x0008412C
	private void DuplicateScoreboard()
	{
		this.currentScoreboard.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(this.currentScoreboard);
		}
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		this.currentScoreboard.transform.SetPositionAndRotation(vector, quaternion);
		this.reportScoreboard.transform.SetPositionAndRotation(vector, quaternion);
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x00085F98 File Offset: 0x00084198
	private void ToggleLevelVisibility(bool state)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (state)
		{
			component.cullingMask = this.savedCullingLayers;
			return;
		}
		this.savedCullingLayers = component.cullingMask;
		component.cullingMask = this.visibleLayers;
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x00085FE4 File Offset: 0x000841E4
	private void Teardown()
	{
		this.ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		this.ReportText.SetActive(false);
		this.localPlayer.inOverlay = false;
		this.localPlayer.disableMovement = false;
		this.closeButton.selected = false;
		this.closeButton.isOn = false;
		this.closeButton.UpdateColor();
		this.localPlayer.InReportMenu = false;
		this.ToggleLevelVisibility(true);
		base.gameObject.SetActive(false);
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			gorillaPlayerScoreboardLine.doneReporting = false;
		}
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x000860C0 File Offset: 0x000842C0
	private void CheckReportSubmit()
	{
		if (this.currentScoreboard == null)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			if (gorillaPlayerScoreboardLine.doneReporting)
			{
				this.ReportText.SetActive(true);
				this.ReportText.GetComponent<Text>().text = "REPORTED " + gorillaPlayerScoreboardLine.playerNameVisible;
				this.currentScoreboard.gameObject.SetActive(false);
				base.StartCoroutine(this.Submitted());
			}
		}
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x00086174 File Offset: 0x00084374
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * this.playerLocalScreenPosition * scale.x;
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x00086200 File Offset: 0x00084400
	private void StartOverlay(bool isSanction = false)
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		this.currentScoreboard.transform.localScale = vector2 * 2f;
		this.reportScoreboard.transform.localScale = vector2;
		this.leftHandObject.transform.localScale = vector2;
		this.rightHandObject.transform.localScale = vector2;
		this.occluder.transform.localScale = vector2;
		if (this.localPlayer.InReportMenu && !PhotonNetwork.InRoom)
		{
			return;
		}
		this.localPlayer.InReportMenu = true;
		this.localPlayer.disableMovement = true;
		this.localPlayer.inOverlay = true;
		base.gameObject.SetActive(true);
		if (PhotonNetwork.InRoom && !isSanction)
		{
			this.DuplicateScoreboard();
		}
		else
		{
			this.ReportText.SetActive(true);
			this.reportScoreboard.transform.SetPositionAndRotation(vector, quaternion);
			this.currentScoreboard.transform.SetPositionAndRotation(vector, quaternion);
		}
		this.ToggleLevelVisibility(false);
		Transform controllerTransform = this.localPlayer.GetControllerTransform(true);
		Transform controllerTransform2 = this.localPlayer.GetControllerTransform(false);
		this.rightHandObject.transform.SetPositionAndRotation(controllerTransform2.position, controllerTransform2.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(controllerTransform.position, controllerTransform.rotation);
		if (isSanction)
		{
			this.currentScoreboard.gameObject.SetActive(false);
			return;
		}
		this.currentScoreboard.gameObject.SetActive(true);
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00086384 File Offset: 0x00084584
	private void CheckDistance()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		float num = Vector3.Distance(this.reportScoreboard.transform.position, vector);
		float num2 = 1f;
		if (num > num2 && !this.isMoving)
		{
			this.isMoving = true;
			this.movementTime = 0f;
		}
		if (this.isMoving)
		{
			this.movementTime += Time.deltaTime;
			float num3 = this.movementTime;
			this.reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.reportScoreboard.transform.position, vector, num3), Quaternion.Lerp(this.reportScoreboard.transform.rotation, quaternion, num3));
			if (this.currentScoreboard != null)
			{
				this.currentScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.currentScoreboard.transform.position, vector, num3), Quaternion.Lerp(this.currentScoreboard.transform.rotation, quaternion, num3));
			}
			if (num3 >= 1f)
			{
				this.isMoving = false;
				this.movementTime = 0f;
			}
		}
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000864A4 File Offset: 0x000846A4
	private void Update()
	{
		if (this.blockButtonsUntilTimestamp > Time.time)
		{
			return;
		}
		if (SteamVR_Actions.gorillaTag_System.GetState(1) && this.localPlayer.InReportMenu)
		{
			this.Teardown();
			this.blockButtonsUntilTimestamp = Time.time + 0.75f;
		}
		if (this.localPlayer.InReportMenu)
		{
			this.localPlayer.inOverlay = true;
			this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
			Transform controllerTransform = this.localPlayer.GetControllerTransform(true);
			Transform controllerTransform2 = this.localPlayer.GetControllerTransform(false);
			this.rightHandObject.transform.SetPositionAndRotation(controllerTransform2.position, controllerTransform2.rotation);
			this.leftHandObject.transform.SetPositionAndRotation(controllerTransform.position, controllerTransform.rotation);
			this.CheckDistance();
			this.CheckReportSubmit();
		}
		if (this.closeButton.selected)
		{
			this.Teardown();
		}
		if (this.testPress)
		{
			this.testPress = false;
			this.StartOverlay(false);
		}
	}

	// Token: 0x0400225A RID: 8794
	[SerializeField]
	private GameObject occluder;

	// Token: 0x0400225B RID: 8795
	[SerializeField]
	private GameObject reportScoreboard;

	// Token: 0x0400225C RID: 8796
	[SerializeField]
	private GameObject ReportText;

	// Token: 0x0400225D RID: 8797
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x0400225E RID: 8798
	[SerializeField]
	private GorillaReportButton closeButton;

	// Token: 0x0400225F RID: 8799
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x04002260 RID: 8800
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x04002261 RID: 8801
	[SerializeField]
	private Vector3 playerLocalScreenPosition;

	// Token: 0x04002262 RID: 8802
	private float blockButtonsUntilTimestamp;

	// Token: 0x04002263 RID: 8803
	[SerializeField]
	private GorillaScoreBoard currentScoreboard;

	// Token: 0x04002264 RID: 8804
	private int savedCullingLayers;

	// Token: 0x04002265 RID: 8805
	public bool testPress;

	// Token: 0x04002266 RID: 8806
	public bool isMoving;

	// Token: 0x04002267 RID: 8807
	private float movementTime;
}
