using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000B79 RID: 2937
public class FriendDisplay : MonoBehaviour
{
	// Token: 0x170006CD RID: 1741
	// (get) Token: 0x06004895 RID: 18581 RVA: 0x0017C7ED File Offset: 0x0017A9ED
	public bool InRemoveMode
	{
		get
		{
			return this.inRemoveMode;
		}
	}

	// Token: 0x06004896 RID: 18582 RVA: 0x0017C7F8 File Offset: 0x0017A9F8
	private void Start()
	{
		this.InitFriendCards();
		this.InitLocalPlayerCard();
		this.UpdateLocalPlayerPrivacyButtons();
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		NetworkSystem.Instance.OnJoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06004897 RID: 18583 RVA: 0x0017C868 File Offset: 0x0017AA68
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnJoinedRoomEvent -= new Action(this.OnJoinedRoom);
		}
		if (this.triggerNotifier != null)
		{
			this.triggerNotifier.TriggerEnterEvent -= this.TriggerEntered;
			this.triggerNotifier.TriggerExitEvent -= this.TriggerExited;
		}
	}

	// Token: 0x06004898 RID: 18584 RVA: 0x0017C8E0 File Offset: 0x0017AAE0
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh += new Action<List<FriendBackendController.Friend>>(this.OnGetFriendsReceived);
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
			this.localPlayerAtDisplay = true;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x06004899 RID: 18585 RVA: 0x0017C940 File Offset: 0x0017AB40
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh -= new Action<List<FriendBackendController.Friend>>(this.OnGetFriendsReceived);
			this.ClearFriendCards();
			this.ClearLocalPlayerCard();
			this.ClearPageButtons();
			this.localPlayerAtDisplay = false;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x0600489A RID: 18586 RVA: 0x0017C99E File Offset: 0x0017AB9E
	private void OnJoinedRoom()
	{
		this.Refresh();
	}

	// Token: 0x0600489B RID: 18587 RVA: 0x0017C9A6 File Offset: 0x0017ABA6
	private void Refresh()
	{
		if (this.localPlayerAtDisplay)
		{
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
		}
	}

	// Token: 0x0600489C RID: 18588 RVA: 0x0017C9C2 File Offset: 0x0017ABC2
	public void LocalPlayerFullyVisiblePress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Visible);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x0600489D RID: 18589 RVA: 0x0017C9DD File Offset: 0x0017ABDD
	public void LocalPlayerPublicOnlyPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.PublicOnly);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x0600489E RID: 18590 RVA: 0x0017C9F8 File Offset: 0x0017ABF8
	public void LocalPlayerFullyHiddenPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Hidden);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x0600489F RID: 18591 RVA: 0x0017CA14 File Offset: 0x0017AC14
	private void UpdateLocalPlayerPrivacyButtons()
	{
		FriendSystem.PlayerPrivacy localPlayerPrivacy = FriendSystem.Instance.LocalPlayerPrivacy;
		this.SetButtonAppearance(this._localPlayerFullyVisibleButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Visible);
		this.SetButtonAppearance(this._localPlayerPublicOnlyButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly);
		this.SetButtonAppearance(this._localPlayerFullyHiddenButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden);
	}

	// Token: 0x060048A0 RID: 18592 RVA: 0x0017CA60 File Offset: 0x0017AC60
	private void UpdatePageButtons(int selectedPage)
	{
		for (int i = 0; i < this.totalPages; i++)
		{
			if (FriendBackendController.Instance.FriendsList.Count > this.cardsPerPage * Mathf.Max(i, 1))
			{
				this.SetPageButtonAppearance(this.PageButtons[i], (i == selectedPage) ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Active);
			}
			else
			{
				this.SetPageButtonAppearance(this.PageButtons[i], false);
			}
		}
	}

	// Token: 0x060048A1 RID: 18593 RVA: 0x0017CAC6 File Offset: 0x0017ACC6
	private void SetButtonAppearance(MeshRenderer buttonRenderer, bool active)
	{
		this.SetButtonAppearance(buttonRenderer, active ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060048A2 RID: 18594 RVA: 0x0017CAD8 File Offset: 0x0017ACD8
	private void SetButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		Material[] sharedMaterials;
		switch (state)
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
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
	}

	// Token: 0x060048A3 RID: 18595 RVA: 0x0017CB30 File Offset: 0x0017AD30
	private void ClearPageButtons()
	{
		for (int i = 0; i < this.PageButtons.Length; i++)
		{
			this.SetPageButtonAppearance(this.PageButtons[i], false);
		}
	}

	// Token: 0x060048A4 RID: 18596 RVA: 0x0017CB5F File Offset: 0x0017AD5F
	private void SetPageButtonAppearance(MeshRenderer buttonRenderer, bool active)
	{
		this.SetPageButtonAppearance(buttonRenderer, active ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060048A5 RID: 18597 RVA: 0x0017CB70 File Offset: 0x0017AD70
	private void SetPageButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		bool enabled;
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			enabled = false;
			break;
		case FriendDisplay.ButtonState.Active:
			enabled = true;
			break;
		case FriendDisplay.ButtonState.Alert:
			enabled = true;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.enabled = enabled;
		Material[] sharedMaterials;
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			sharedMaterials = this._pageButtonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			sharedMaterials = this._pageButtonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			sharedMaterials = this._pageButtonAlerttMaterials;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
		Transform transform = buttonRenderer.transform;
		Vector3 localPosition;
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			localPosition = new Vector3(buttonRenderer.transform.localPosition.x, buttonRenderer.transform.localPosition.y, this.pageButtonInactiveZPos);
			break;
		case FriendDisplay.ButtonState.Active:
			localPosition = new Vector3(buttonRenderer.transform.localPosition.x, buttonRenderer.transform.localPosition.y, this.pageButtonActiveZPos);
			break;
		case FriendDisplay.ButtonState.Alert:
			localPosition = new Vector3(buttonRenderer.transform.localPosition.x, buttonRenderer.transform.localPosition.y, this.pageButtonActiveZPos);
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		transform.localPosition = localPosition;
		BoxCollider component = buttonRenderer.GetComponent<BoxCollider>();
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			enabled = false;
			break;
		case FriendDisplay.ButtonState.Active:
			enabled = true;
			break;
		case FriendDisplay.ButtonState.Alert:
			enabled = true;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		component.enabled = enabled;
	}

	// Token: 0x060048A6 RID: 18598 RVA: 0x0017CD0C File Offset: 0x0017AF0C
	public void ToggleRemoveFriendMode()
	{
		this.inRemoveMode = !this.inRemoveMode;
		FriendCard[] array = this.friendCards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetRemoveEnabled(this.inRemoveMode);
		}
		this.SetButtonAppearance(this._removeFriendButton, this.inRemoveMode ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x060048A7 RID: 18599 RVA: 0x0017CD64 File Offset: 0x0017AF64
	private void InitFriendCards()
	{
		float num = this.gridWidth / (float)this.gridDimension;
		float num2 = this.gridHeight / (float)this.gridDimension;
		Vector3 right = this.gridRoot.right;
		Vector3 vector = -this.gridRoot.up;
		Vector3 vector2 = this.gridRoot.position - right * (this.gridWidth * 0.5f - num * 0.5f) - vector * (this.gridHeight * 0.5f - num2 * 0.5f);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.gridDimension; i++)
		{
			for (int j = 0; j < this.gridDimension; j++)
			{
				FriendCard friendCard = this.friendCards[num4];
				friendCard.gameObject.SetActive(true);
				friendCard.transform.localScale = Vector3.one * (num / friendCard.Width);
				friendCard.transform.position = vector2 + right * num * (float)j + vector * num2 * (float)i;
				friendCard.transform.rotation = this.gridRoot.transform.rotation;
				friendCard.Init(this);
				friendCard.SetButton(this._friendCardButtons[num3++], this._buttonDefaultMaterials, this._buttonActiveMaterials, this._buttonAlertMaterials, this._friendCardButtonText[num4]);
				friendCard.SetEmpty();
				num4++;
			}
		}
	}

	// Token: 0x060048A8 RID: 18600 RVA: 0x0017CF04 File Offset: 0x0017B104
	public void RandomizeFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].Randomize();
		}
	}

	// Token: 0x060048A9 RID: 18601 RVA: 0x0017CF34 File Offset: 0x0017B134
	private void ClearFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
	}

	// Token: 0x060048AA RID: 18602 RVA: 0x0017CF61 File Offset: 0x0017B161
	public void OnGetFriendsReceived(List<FriendBackendController.Friend> friendsList)
	{
		this.PopulateFriendCards(friendsList);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
		this.UpdatePageButtons(0);
	}

	// Token: 0x060048AB RID: 18603 RVA: 0x0017CF80 File Offset: 0x0017B180
	private void PopulateFriendCards(List<FriendBackendController.Friend> friendsList)
	{
		int num = Mathf.Min(this.friendCards.Length, friendsList.Count);
		int num2 = 0;
		while (num2 < num && friendsList[num2] != null)
		{
			this.friendCards[num2].Populate(friendsList[num2]);
			num2++;
		}
	}

	// Token: 0x060048AC RID: 18604 RVA: 0x0017CFCC File Offset: 0x0017B1CC
	public void GoToFriendPage(int currentPage)
	{
		this.UpdatePageButtons(currentPage);
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
		int num = currentPage * this.cardsPerPage;
		Mathf.Min(num + this.cardsPerPage, FriendBackendController.Instance.FriendsList.Count);
		int num2 = 0;
		int num3 = 0;
		while (num3 < this.friendCards.Length && FriendBackendController.Instance.FriendsList.Count > num + num2)
		{
			this.friendCards[num3].Populate(FriendBackendController.Instance.FriendsList[num + num2]);
			num2++;
			num3++;
		}
	}

	// Token: 0x060048AD RID: 18605 RVA: 0x0017D078 File Offset: 0x0017B278
	private void InitLocalPlayerCard()
	{
		this._localPlayerCard.Init(this);
		this.ClearLocalPlayerCard();
	}

	// Token: 0x060048AE RID: 18606 RVA: 0x0017D08C File Offset: 0x0017B28C
	private void PopulateLocalPlayerCard()
	{
		string zone = PhotonNetworkController.Instance.CurrentRoomZone.GetName<GTZone>().ToUpper();
		this._localPlayerCard.SetName(NetworkSystem.Instance.LocalPlayer.NickName.ToUpper());
		if (!PhotonNetwork.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.RoomName) || NetworkSystem.Instance.RoomName.Length <= 0)
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		bool flag = NetworkSystem.Instance.RoomName.get_Chars(0) == '@';
		bool flag2 = !NetworkSystem.Instance.SessionIsPrivate;
		if (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden || (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly && !flag2))
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		if (flag)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.Substring(1).ToUpper());
			this._localPlayerCard.SetZone("CUSTOM");
			return;
		}
		if (!flag2)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
			this._localPlayerCard.SetZone("PRIVATE");
			return;
		}
		this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
		this._localPlayerCard.SetZone(zone);
	}

	// Token: 0x060048AF RID: 18607 RVA: 0x0017D219 File Offset: 0x0017B419
	private void ClearLocalPlayerCard()
	{
		this._localPlayerCard.SetEmpty();
	}

	// Token: 0x060048B0 RID: 18608 RVA: 0x0017D228 File Offset: 0x0017B428
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		float num = this.gridWidth * 0.5f;
		float num2 = this.gridHeight * 0.5f;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, -num4, 0f);
		for (int i = 0; i <= this.gridDimension; i++)
		{
			float num5 = (float)i / (float)this.gridDimension;
			Vector3 vector5 = Vector3.Lerp(vector, vector2, num5);
			Vector3 vector6 = Vector3.Lerp(vector3, vector4, num5);
			Gizmos.DrawLine(vector5, vector6);
			Vector3 vector7 = Vector3.Lerp(vector, vector3, num5);
			Vector3 vector8 = Vector3.Lerp(vector2, vector4, num5);
			Gizmos.DrawLine(vector7, vector8);
		}
	}

	// Token: 0x0400591E RID: 22814
	[FormerlySerializedAs("gridCenter")]
	[SerializeField]
	private FriendCard[] friendCards = new FriendCard[9];

	// Token: 0x0400591F RID: 22815
	[SerializeField]
	private Transform gridRoot;

	// Token: 0x04005920 RID: 22816
	[SerializeField]
	private float gridWidth = 2f;

	// Token: 0x04005921 RID: 22817
	[SerializeField]
	private float gridHeight = 1f;

	// Token: 0x04005922 RID: 22818
	[SerializeField]
	private int gridDimension = 3;

	// Token: 0x04005923 RID: 22819
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04005924 RID: 22820
	[FormerlySerializedAs("_joinButtons")]
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableDelayButton[] _friendCardButtons;

	// Token: 0x04005925 RID: 22821
	[SerializeField]
	private TextMeshProUGUI[] _friendCardButtonText;

	// Token: 0x04005926 RID: 22822
	[SerializeField]
	private MeshRenderer _localPlayerFullyVisibleButton;

	// Token: 0x04005927 RID: 22823
	[SerializeField]
	private MeshRenderer _localPlayerPublicOnlyButton;

	// Token: 0x04005928 RID: 22824
	[SerializeField]
	private MeshRenderer _localPlayerFullyHiddenButton;

	// Token: 0x04005929 RID: 22825
	[SerializeField]
	private MeshRenderer _removeFriendButton;

	// Token: 0x0400592A RID: 22826
	[SerializeField]
	private FriendCard _localPlayerCard;

	// Token: 0x0400592B RID: 22827
	[SerializeField]
	private MeshRenderer[] PageButtons;

	// Token: 0x0400592C RID: 22828
	[SerializeField]
	private Material[] _buttonDefaultMaterials;

	// Token: 0x0400592D RID: 22829
	[SerializeField]
	private Material[] _buttonActiveMaterials;

	// Token: 0x0400592E RID: 22830
	[SerializeField]
	private Material[] _buttonAlertMaterials;

	// Token: 0x0400592F RID: 22831
	[SerializeField]
	private Material[] _pageButtonDefaultMaterials;

	// Token: 0x04005930 RID: 22832
	[SerializeField]
	private Material[] _pageButtonActiveMaterials;

	// Token: 0x04005931 RID: 22833
	[SerializeField]
	private Material[] _pageButtonAlerttMaterials;

	// Token: 0x04005932 RID: 22834
	private int cardsPerPage = 9;

	// Token: 0x04005933 RID: 22835
	private int totalPages = 5;

	// Token: 0x04005934 RID: 22836
	[SerializeField]
	private float pageButtonInactiveZPos;

	// Token: 0x04005935 RID: 22837
	[SerializeField]
	private float pageButtonActiveZPos;

	// Token: 0x04005936 RID: 22838
	private MeshRenderer[] _joinButtonRenderers;

	// Token: 0x04005937 RID: 22839
	private bool inRemoveMode;

	// Token: 0x04005938 RID: 22840
	private bool localPlayerAtDisplay;

	// Token: 0x02000B7A RID: 2938
	public enum ButtonState
	{
		// Token: 0x0400593A RID: 22842
		Default,
		// Token: 0x0400593B RID: 22843
		Active,
		// Token: 0x0400593C RID: 22844
		Alert
	}
}
