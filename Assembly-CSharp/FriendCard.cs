using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000B78 RID: 2936
public class FriendCard : MonoBehaviour
{
	// Token: 0x170006C8 RID: 1736
	// (get) Token: 0x06004878 RID: 18552 RVA: 0x0017BD18 File Offset: 0x00179F18
	public TextMeshProUGUI NameText
	{
		get
		{
			return this.nameText;
		}
	}

	// Token: 0x170006C9 RID: 1737
	// (get) Token: 0x06004879 RID: 18553 RVA: 0x0017BD20 File Offset: 0x00179F20
	public TextMeshProUGUI RoomText
	{
		get
		{
			return this.roomText;
		}
	}

	// Token: 0x170006CA RID: 1738
	// (get) Token: 0x0600487A RID: 18554 RVA: 0x0017BD28 File Offset: 0x00179F28
	public TextMeshProUGUI ZoneText
	{
		get
		{
			return this.zoneText;
		}
	}

	// Token: 0x170006CB RID: 1739
	// (get) Token: 0x0600487B RID: 18555 RVA: 0x0017BD30 File Offset: 0x00179F30
	public float Width
	{
		get
		{
			return this.width;
		}
	}

	// Token: 0x170006CC RID: 1740
	// (get) Token: 0x0600487C RID: 18556 RVA: 0x0017BD38 File Offset: 0x00179F38
	// (set) Token: 0x0600487D RID: 18557 RVA: 0x0017BD40 File Offset: 0x00179F40
	public float Height { get; private set; } = 0.25f;

	// Token: 0x0600487E RID: 18558 RVA: 0x0017BD49 File Offset: 0x00179F49
	private void Awake()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600487F RID: 18559 RVA: 0x0017BD69 File Offset: 0x00179F69
	private void OnDestroy()
	{
		if (this._button)
		{
			this._button.onPressed -= new Action<GorillaPressableButton, bool>(this.OnButtonPressed);
		}
	}

	// Token: 0x06004880 RID: 18560 RVA: 0x0017BD8F File Offset: 0x00179F8F
	public void Init(FriendDisplay owner)
	{
		this.friendDisplay = owner;
	}

	// Token: 0x06004881 RID: 18561 RVA: 0x0017BD98 File Offset: 0x00179F98
	private void UpdateComponentStates()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(this.canRemove);
		}
		if (this.canRemove)
		{
			this.SetButtonState((this.currentFriend != null) ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
			return;
		}
		if (this.joinable)
		{
			this.SetButtonState(FriendDisplay.ButtonState.Active);
			return;
		}
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06004882 RID: 18562 RVA: 0x0017BDFC File Offset: 0x00179FFC
	private void SetButtonState(FriendDisplay.ButtonState newState)
	{
		if (this._button == null)
		{
			return;
		}
		if (this._buttonState == newState)
		{
			return;
		}
		this._buttonState = newState;
		MeshRenderer buttonRenderer = this._button.buttonRenderer;
		FriendDisplay.ButtonState buttonState = this._buttonState;
		Material[] sharedMaterials;
		switch (buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			sharedMaterials = this._buttonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			sharedMaterials = this._buttonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			sharedMaterials = this._buttonAlertMaterials;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(buttonState);
			break;
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
		this._button.delayTime = (float)((this._buttonState == FriendDisplay.ButtonState.Alert) ? 3 : 0);
	}

	// Token: 0x06004883 RID: 18563 RVA: 0x0017BE98 File Offset: 0x0017A098
	public void Populate(FriendBackendController.Friend friend)
	{
		this.SetEmpty();
		if (friend != null && friend.Presence != null)
		{
			if (friend.Presence.UserName != null)
			{
				this.SetName(friend.Presence.UserName.ToUpper());
			}
			if (!string.IsNullOrEmpty(friend.Presence.RoomId) && friend.Presence.RoomId.Length > 0)
			{
				bool? isPublic = friend.Presence.IsPublic;
				bool flag = true;
				bool flag2 = isPublic.GetValueOrDefault() == flag & isPublic != null;
				bool flag3 = friend.Presence.RoomId.get_Chars(0) == '@';
				bool flag4 = friend.Presence.RoomId.Equals(NetworkSystem.Instance.RoomName);
				bool flag5 = false;
				if (!flag4 && flag2 && !friend.Presence.Zone.IsNullOrEmpty())
				{
					string text = friend.Presence.Zone.ToLower();
					foreach (GTZone e in ZoneManagement.instance.activeZones)
					{
						if (text.Contains(e.GetName<GTZone>().ToLower()))
						{
							flag5 = true;
						}
					}
				}
				this.joinable = (!flag3 && !flag4 && (!flag2 || flag5) && this.HasKIDPermissionToJoinPrivateRooms());
				if (flag3)
				{
					this.SetRoom(friend.Presence.RoomId.Substring(1).ToUpper());
					this.SetZone("CUSTOM");
				}
				else if (!flag2)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone("PRIVATE");
				}
				else if (friend.Presence.Zone != null)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone(friend.Presence.Zone.ToUpper());
				}
			}
			else
			{
				this.joinable = false;
				this.SetRoom("OFFLINE");
			}
			this.currentFriend = friend;
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06004884 RID: 18564 RVA: 0x0017C0B4 File Offset: 0x0017A2B4
	public void SetName(string friendName)
	{
		TMP_Text tmp_Text = this.nameText;
		this._friendName = friendName;
		tmp_Text.text = friendName;
	}

	// Token: 0x06004885 RID: 18565 RVA: 0x0017C0D8 File Offset: 0x0017A2D8
	public void SetRoom(string friendRoom)
	{
		TMP_Text tmp_Text = this.roomText;
		this._friendRoom = friendRoom;
		tmp_Text.text = friendRoom;
	}

	// Token: 0x06004886 RID: 18566 RVA: 0x0017C0FC File Offset: 0x0017A2FC
	public void SetZone(string friendZone)
	{
		TMP_Text tmp_Text = this.zoneText;
		this._friendZone = friendZone;
		tmp_Text.text = friendZone;
	}

	// Token: 0x06004887 RID: 18567 RVA: 0x0017C120 File Offset: 0x0017A320
	public void Randomize()
	{
		this.SetEmpty();
		int num = Random.Range(0, this.randomNames.Length);
		this.SetName(this.randomNames[num].ToUpper());
		this.SetRoom(string.Format("{0}{1}{2}{3}", new object[]
		{
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91)
		}));
		bool flag = Random.Range(0f, 1f) > 0.5f;
		this.joinable = (flag && Random.Range(0f, 1f) > 0.5f);
		if (flag)
		{
			int num2 = Random.Range(0, 17);
			GTZone gtzone = (GTZone)num2;
			this.SetZone(gtzone.ToString().ToUpper());
		}
		else
		{
			this.SetZone(this.privateString);
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06004888 RID: 18568 RVA: 0x0017C226 File Offset: 0x0017A426
	public void SetEmpty()
	{
		this.SetName(this.emptyString);
		this.SetRoom(this.emptyString);
		this.SetZone(this.emptyString);
		this.joinable = false;
		this.currentFriend = null;
		this.UpdateComponentStates();
	}

	// Token: 0x06004889 RID: 18569 RVA: 0x0017C260 File Offset: 0x0017A460
	public void SetRemoveEnabled(bool enabled)
	{
		this.canRemove = enabled;
		this.UpdateComponentStates();
	}

	// Token: 0x0600488A RID: 18570 RVA: 0x0017C270 File Offset: 0x0017A470
	private void JoinButtonPressed()
	{
		if (this.joinable && this.currentFriend != null && this.currentFriend.Presence != null)
		{
			bool? isPublic = this.currentFriend.Presence.IsPublic;
			bool flag = true;
			JoinType roomJoinType = (isPublic.GetValueOrDefault() == flag & isPublic != null) ? JoinType.FriendStationPublic : JoinType.FriendStationPrivate;
			GorillaComputer.instance.roomToJoin = this._friendRoom;
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this._friendRoom, roomJoinType);
			this.joinable = false;
			this.UpdateComponentStates();
		}
	}

	// Token: 0x0600488B RID: 18571 RVA: 0x0017C2F8 File Offset: 0x0017A4F8
	private void RemoveFriendButtonPressed()
	{
		if (this.friendDisplay.InRemoveMode)
		{
			FriendSystem.Instance.RemoveFriend(this.currentFriend, null);
			this.SetEmpty();
		}
	}

	// Token: 0x0600488C RID: 18572 RVA: 0x0017C320 File Offset: 0x0017A520
	private void OnDrawGizmosSelected()
	{
		float num = this.width * 0.5f * base.transform.lossyScale.x;
		float num2 = this.Height * 0.5f * base.transform.lossyScale.y;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = base.transform.position + base.transform.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = base.transform.position + base.transform.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = base.transform.position + base.transform.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = base.transform.position + base.transform.rotation * new Vector3(num3, -num4, 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector4, vector3);
		Gizmos.DrawLine(vector3, vector);
	}

	// Token: 0x0600488D RID: 18573 RVA: 0x0017C454 File Offset: 0x0017A654
	public void SetButton(GorillaPressableDelayButton friendCardButton, Material[] normalMaterials, Material[] activeMaterials, Material[] alertMaterials, TextMeshProUGUI buttonText)
	{
		this._button = friendCardButton;
		this._button.SetFillBar(this.removeProgressBar);
		this._button.onPressBegin += new Action(this.OnButtonPressBegin);
		this._button.onPressAbort += new Action(this.OnButtonPressAbort);
		this._button.onPressed += new Action<GorillaPressableButton, bool>(this.OnButtonPressed);
		this._buttonDefaultMaterials = normalMaterials;
		this._buttonActiveMaterials = activeMaterials;
		this._buttonAlertMaterials = alertMaterials;
		this._buttonText = buttonText;
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x0600488E RID: 18574 RVA: 0x0017C4E3 File Offset: 0x0017A6E3
	private void OnRemoveFriendBegin()
	{
		this.nameText.text = "REMOVING";
		this.roomText.text = "FRIEND";
		this.zoneText.text = this.emptyString;
	}

	// Token: 0x0600488F RID: 18575 RVA: 0x0017C516 File Offset: 0x0017A716
	private void OnRemoveFriendEnd()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x06004890 RID: 18576 RVA: 0x0017C54C File Offset: 0x0017A74C
	private void OnButtonPressBegin()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendBegin();
			break;
		default:
			return;
		}
	}

	// Token: 0x06004891 RID: 18577 RVA: 0x0017C57C File Offset: 0x0017A77C
	private void OnButtonPressAbort()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendEnd();
			break;
		default:
			return;
		}
	}

	// Token: 0x06004892 RID: 18578 RVA: 0x0017C5AC File Offset: 0x0017A7AC
	private void OnButtonPressed(GorillaPressableButton button, bool isLeftHand)
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			break;
		case FriendDisplay.ButtonState.Active:
			this.JoinButtonPressed();
			return;
		case FriendDisplay.ButtonState.Alert:
			this.RemoveFriendButtonPressed();
			break;
		default:
			return;
		}
	}

	// Token: 0x06004893 RID: 18579 RVA: 0x0017C5E0 File Offset: 0x0017A7E0
	private bool HasKIDPermissionToJoinPrivateRooms()
	{
		return !KIDManager.KidEnabled || (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer));
	}

	// Token: 0x04005908 RID: 22792
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x04005909 RID: 22793
	[SerializeField]
	private TextMeshProUGUI roomText;

	// Token: 0x0400590A RID: 22794
	[SerializeField]
	private TextMeshProUGUI zoneText;

	// Token: 0x0400590B RID: 22795
	[SerializeField]
	private Transform removeProgressBar;

	// Token: 0x0400590C RID: 22796
	[SerializeField]
	private float width = 0.25f;

	// Token: 0x0400590E RID: 22798
	private string emptyString = "";

	// Token: 0x0400590F RID: 22799
	private string privateString = "PRIVATE";

	// Token: 0x04005910 RID: 22800
	private bool joinable;

	// Token: 0x04005911 RID: 22801
	private bool canRemove;

	// Token: 0x04005912 RID: 22802
	private GorillaPressableDelayButton _button;

	// Token: 0x04005913 RID: 22803
	private TextMeshProUGUI _buttonText;

	// Token: 0x04005914 RID: 22804
	private string _friendName;

	// Token: 0x04005915 RID: 22805
	private string _friendRoom;

	// Token: 0x04005916 RID: 22806
	private string _friendZone;

	// Token: 0x04005917 RID: 22807
	private FriendBackendController.Friend currentFriend;

	// Token: 0x04005918 RID: 22808
	private FriendDisplay friendDisplay;

	// Token: 0x04005919 RID: 22809
	private string[] randomNames = new string[]
	{
		"Veronica",
		"Roman",
		"Janiyah",
		"Dalton",
		"Bellamy",
		"Eithan",
		"Celeste",
		"Isaac",
		"Astrid",
		"Azariah",
		"Keilani",
		"Zeke",
		"Jayleen",
		"Yosef",
		"Jaylee",
		"Bodie",
		"Greta",
		"Cain",
		"Ella",
		"Everly",
		"Finnley",
		"Paisley",
		"Kaison",
		"Luna",
		"Nina",
		"Maison",
		"Monroe",
		"Ricardo",
		"Zariyah",
		"Travis",
		"Lacey",
		"Elian",
		"Frankie",
		"Otis",
		"Adele",
		"Edison",
		"Amira",
		"Ivan",
		"Raelynn",
		"Eliel",
		"Aliana",
		"Beckett",
		"Mylah",
		"Melvin",
		"Magdalena",
		"Leroy",
		"Madeleine"
	};

	// Token: 0x0400591A RID: 22810
	private FriendDisplay.ButtonState _buttonState = (FriendDisplay.ButtonState)(-1);

	// Token: 0x0400591B RID: 22811
	private Material[] _buttonDefaultMaterials;

	// Token: 0x0400591C RID: 22812
	private Material[] _buttonActiveMaterials;

	// Token: 0x0400591D RID: 22813
	private Material[] _buttonAlertMaterials;
}
