using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Customizations;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009AD RID: 2477
public class VirtualStumpOptionsTerminal : MonoBehaviour, IWssAuthPrompter
{
	// Token: 0x06003F3E RID: 16190 RVA: 0x00153310 File Offset: 0x00151510
	public void Start()
	{
		this.optionList.gameObject.SetActive(true);
		this.mainScreenText.gameObject.SetActive(true);
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoginStarted.AddListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.AddListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
	}

	// Token: 0x06003F3F RID: 16191 RVA: 0x001533CC File Offset: 0x001515CC
	public void OnDestroy()
	{
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoginStarted.RemoveListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.RemoveListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
	}

	// Token: 0x06003F40 RID: 16192 RVA: 0x00153469 File Offset: 0x00151669
	public void OnEnable()
	{
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
	}

	// Token: 0x06003F41 RID: 16193 RVA: 0x00153480 File Offset: 0x00151680
	private void OnKeyPressed(CustomMapKeyboardBinding pressedButton)
	{
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.cachedError = null;
			this.RefreshButtonState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.up)
		{
			int num = this.currentState - VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE;
			if (num < 0)
			{
				num = 1;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.down)
		{
			int num2 = (int)(this.currentState + 1);
			if (num2 >= 2)
			{
				num2 = 0;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num2);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.OnKeyPressed_ModIOAccount(pressedButton);
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.OnKeyPressed_RoomSize(pressedButton);
	}

	// Token: 0x06003F42 RID: 16194 RVA: 0x0015351C File Offset: 0x0015171C
	private void ChangeState(VirtualStumpOptionsTerminal.ETerminalState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		this.currentState = newState;
		this.RefreshButtonState();
	}

	// Token: 0x06003F43 RID: 16195 RVA: 0x00153538 File Offset: 0x00151738
	private void RefreshButtonState()
	{
		for (int i = 0; i < this.contextualButtons.Count; i++)
		{
			if (this.contextualButtons[i].IsNotNull())
			{
				this.contextualButtons[i].SetActive(false);
			}
		}
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.OKButton.SetActive(true);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			for (int j = 0; j < this.buttonsToShow_MODIO.Count; j++)
			{
				if (this.buttonsToShow_MODIO[j].IsNotNull())
				{
					this.buttonsToShow_MODIO[j].SetActive(true);
				}
			}
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		for (int k = 0; k < this.buttonsToShow_ROOMSIZE.Count; k++)
		{
			if (this.buttonsToShow_ROOMSIZE[k].IsNotNull())
			{
				this.buttonsToShow_ROOMSIZE[k].SetActive(true);
			}
		}
	}

	// Token: 0x06003F44 RID: 16196 RVA: 0x00153620 File Offset: 0x00151820
	private void UpdateOptionListForCurrentState()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 2; i++)
		{
			stringBuilder.Append(this.optionStrings[i]);
			if (i == (int)this.currentState)
			{
				stringBuilder.Append(" <-");
			}
			stringBuilder.Append("\n");
		}
		this.optionList.text = stringBuilder.ToString();
	}

	// Token: 0x06003F45 RID: 16197 RVA: 0x00153684 File Offset: 0x00151884
	private void UpdateScreen()
	{
		Debug.Log("[VirtualStumpOptionsTerminal::UpdateScreen] State: " + this.currentState.ToString() + " | CachedError: " + (!this.cachedError.IsNullOrEmpty()).ToString());
		this.mainScreenText.text = "";
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.RefreshButtonState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.cachedError);
			TMP_Text tmp_Text = this.mainScreenText;
			string text = "<color=\"red\">";
			StringBuilder stringBuilder2 = stringBuilder;
			tmp_Text.text = text + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.mainScreenText.text = this.UpdateScreen_ModIOAccount();
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.mainScreenText.text = this.UpdateScreen_RoomSize();
	}

	// Token: 0x06003F46 RID: 16198 RVA: 0x00153757 File Offset: 0x00151957
	private void OnModIOLoginStarted()
	{
		Debug.Log("[VirtualStumpOptionsTerminal::OnModIOLoginStarted]...");
		this.UpdateScreen();
	}

	// Token: 0x06003F47 RID: 16199 RVA: 0x0015376C File Offset: 0x0015196C
	private void OnModIOLoggedIn()
	{
		Debug.Log("[VirtualStumpOptionsTerminal::OnModIOLoggedIn]...");
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
		AssociateMotherhsipAndModIOAccountsRequest associateMotherhsipAndModIOAccountsRequest = new AssociateMotherhsipAndModIOAccountsRequest();
		associateMotherhsipAndModIOAccountsRequest.ModIOId = ModIOManager.GetCurrentUserId();
		associateMotherhsipAndModIOAccountsRequest.ModIOToken = ModIOManager.GetCurrentAuthToken();
		associateMotherhsipAndModIOAccountsRequest.MothershipEnvId = MothershipClientApiUnity.EnvironmentId;
		associateMotherhsipAndModIOAccountsRequest.MothershipPlayerId = MothershipClientContext.MothershipId;
		associateMotherhsipAndModIOAccountsRequest.MothershipToken = MothershipClientContext.Token;
		base.StartCoroutine(ModIOManager.AssociateMothershipAndModIOAccounts(associateMotherhsipAndModIOAccountsRequest, delegate(AssociateMotherhsipAndModIOAccountsResponse response)
		{
			Debug.Log("[VirtualStumpOptionsTerminal::OnModIOLoggedIn]... Mothership Account Association Created/Updated");
		}));
	}

	// Token: 0x06003F48 RID: 16200 RVA: 0x00153823 File Offset: 0x00151A23
	private void OnModIOLoggedOut()
	{
		Debug.Log("[VirtualStumpOptionsTerminal::OnModIOLoggedOut]...");
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
	}

	// Token: 0x06003F49 RID: 16201 RVA: 0x00153852 File Offset: 0x00151A52
	private void OnModIOLoginFailed(string error)
	{
		Debug.Log("[VirtualStumpOptionsTerminal::OnModIOLoginFailed] Error: " + error);
		this.processingAccountLink = false;
		this.cachedError = error;
		this.UpdateScreen();
	}

	// Token: 0x06003F4A RID: 16202 RVA: 0x00153878 File Offset: 0x00151A78
	private void OnModIOUserChanged(User user)
	{
		Debug.Log("[VirtualStumpOptionsTerminal::OnModIOUserChanged] Username: " + ModIOManager.GetCurrentUsername());
		this.UpdateScreen();
	}

	// Token: 0x06003F4B RID: 16203 RVA: 0x00153894 File Offset: 0x00151A94
	private void OnKeyPressed_ModIOAccount(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.option1)
		{
			this.StartAccountLinkingProcess();
		}
		if (pressedButton == CustomMapKeyboardBinding.option2)
		{
			GTDev.Log<string>(string.Format("[VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount] logout {0}", ModIOManager.IsLoggedIn()), null);
			if (ModIOManager.IsLoggedIn())
			{
				ModIOManager.LogoutFromModIO();
			}
		}
		if (pressedButton == CustomMapKeyboardBinding.option3)
		{
			GTDev.Log<string>(string.Format("[VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount] login {0}", ModIOManager.IsLoggedIn()), null);
			if (!ModIOManager.IsLoggedIn())
			{
				ModIOManager.CancelExternalAuthentication();
				try
				{
					ModIOManager.RequestPlatformLogin();
				}
				catch (Exception ex)
				{
					GTDev.Log<string>(string.Format("VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount platform login error: {0}", ex), null);
					throw;
				}
			}
		}
	}

	// Token: 0x06003F4C RID: 16204 RVA: 0x00153930 File Offset: 0x00151B30
	private Task StartAccountLinkingProcess()
	{
		VirtualStumpOptionsTerminal.<StartAccountLinkingProcess>d__40 <StartAccountLinkingProcess>d__;
		<StartAccountLinkingProcess>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAccountLinkingProcess>d__.<>4__this = this;
		<StartAccountLinkingProcess>d__.<>1__state = -1;
		<StartAccountLinkingProcess>d__.<>t__builder.Start<VirtualStumpOptionsTerminal.<StartAccountLinkingProcess>d__40>(ref <StartAccountLinkingProcess>d__);
		return <StartAccountLinkingProcess>d__.<>t__builder.Task;
	}

	// Token: 0x06003F4D RID: 16205 RVA: 0x00153973 File Offset: 0x00151B73
	public void ShowPrompt(string url, string code)
	{
		this.cachedLinkURL = url;
		this.cachedLinkCode = code;
		this.UpdateScreen();
	}

	// Token: 0x06003F4E RID: 16206 RVA: 0x0015398C File Offset: 0x00151B8C
	private string UpdateScreen_ModIOAccount()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (ModIOManager.IsLoggedIn())
		{
			stringBuilder.Append(this.loggedInAsString + "\n");
			stringBuilder.Append("   " + ModIOManager.GetCurrentUsername() + "\n\n");
			if (ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount)
			{
				stringBuilder.Append(this.linkAccountPromptString + "\n");
			}
			else
			{
				stringBuilder.Append(this.alreadyLinkedAccountString + "\n");
			}
		}
		else if (ModIOManager.IsLoggingIn() && !this.processingAccountLink)
		{
			stringBuilder.Append(this.loggingInString);
		}
		else if (ModIOManager.IsLoggingOut())
		{
			stringBuilder.Append(this.loggingOutString);
		}
		else if (this.processingAccountLink)
		{
			stringBuilder.Append(this.linkAccountPromptString + "\n\n");
			stringBuilder.Append(this.urlLabelString + this.cachedLinkURL + "\n");
			stringBuilder.Append(this.linkCodeLabelString + this.cachedLinkCode + "\n");
		}
		else
		{
			stringBuilder.Append(this.notLoggedInString + "\n\n");
			stringBuilder.Append(this.loginPromptString);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003F4F RID: 16207 RVA: 0x00153AD4 File Offset: 0x00151CD4
	private void OnKeyPressed_RoomSize(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.left)
		{
			this.DecrementRoomSize();
		}
		if (pressedButton == CustomMapKeyboardBinding.right)
		{
			this.IncrementRoomSize();
		}
	}

	// Token: 0x06003F50 RID: 16208 RVA: 0x00153AEC File Offset: 0x00151CEC
	private void DecrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() - 1);
		this.UpdateScreen();
	}

	// Token: 0x06003F51 RID: 16209 RVA: 0x00153B01 File Offset: 0x00151D01
	private void IncrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() + 1);
		this.UpdateScreen();
	}

	// Token: 0x06003F52 RID: 16210 RVA: 0x00153B18 File Offset: 0x00151D18
	private string UpdateScreen_RoomSize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.roomSizeDescriptionString + "\n\n");
		stringBuilder.Append(this.roomSizeLabelString + RoomSystem.GetOverridenRoomSize().ToString());
		return stringBuilder.ToString();
	}

	// Token: 0x06003F53 RID: 16211 RVA: 0x00153B68 File Offset: 0x00151D68
	public VirtualStumpOptionsTerminal()
	{
		List<string> list = new List<string>();
		list.Add("MOD.IO");
		list.Add("ROOM SIZE");
		this.optionStrings = list;
		this.loggedInAsString = "LOGGED INTO MOD.IO AS: ";
		this.notLoggedInString = "LOGGED OUT OF MOD.IO";
		this.loginPromptString = "PRESS THE 'PLATFORM LOGIN' OR 'LINK MOD.IO ACCOUNT' BUTTON TO LOGIN";
		this.loggingInString = "LOGGING IN TO MOD.IO...";
		this.loggingOutString = "LOGGING OUT OF MOD.IO...";
		this.linkAccountPromptString = "IF YOU HAVE AN EXISTING MOD.IO ACCOUNT, YOU CAN LINK IT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON.";
		this.alreadyLinkedAccountString = "YOU'VE ALREADY LINKED YOUR MOD.IO ACCOUNT.";
		this.accountLinkingPromptString = "PLEASE GO TO THIS URL IN YOUR BROWSER AND LOG IN TO YOUR MOD.IO ACCOUNT. ONCE LOGGED IN, ENTER THE FOLLOWING CODE TO PROCEED: ";
		this.urlLabelString = "URL: ";
		this.linkCodeLabelString = "CODE: ";
		this.roomSizeDescriptionString = "THIS SETTING WILL CHANGE THE MAXIMUM AMOUNT OF PLAYERS ALLOWED IN PRIVATE ROOMS YOU CREATE. WHEN JOINING A PUBLIC ROOM, THE MAP YOU'VE LOADED WILL CONTROL THE ROOM SIZE.";
		this.roomSizeLabelString = "MAX PLAYERS: ";
		this.contextualButtons = new List<GameObject>();
		this.buttonsToShow_MODIO = new List<GameObject>();
		this.buttonsToShow_ROOMSIZE = new List<GameObject>();
		this.cachedLinkURL = "";
		this.cachedLinkCode = "";
		base..ctor();
	}

	// Token: 0x04005070 RID: 20592
	[SerializeField]
	private TMP_Text optionList;

	// Token: 0x04005071 RID: 20593
	[SerializeField]
	private TMP_Text mainScreenText;

	// Token: 0x04005072 RID: 20594
	[SerializeField]
	private CustomMapsKeyboard keyboard;

	// Token: 0x04005073 RID: 20595
	[SerializeField]
	private List<string> optionStrings;

	// Token: 0x04005074 RID: 20596
	[SerializeField]
	private string loggedInAsString;

	// Token: 0x04005075 RID: 20597
	[SerializeField]
	private string notLoggedInString;

	// Token: 0x04005076 RID: 20598
	[SerializeField]
	private string loginPromptString;

	// Token: 0x04005077 RID: 20599
	[SerializeField]
	private string loggingInString;

	// Token: 0x04005078 RID: 20600
	[SerializeField]
	private string loggingOutString;

	// Token: 0x04005079 RID: 20601
	[SerializeField]
	private string linkAccountPromptString;

	// Token: 0x0400507A RID: 20602
	[SerializeField]
	private string alreadyLinkedAccountString;

	// Token: 0x0400507B RID: 20603
	[SerializeField]
	private string accountLinkingPromptString;

	// Token: 0x0400507C RID: 20604
	[SerializeField]
	private string urlLabelString;

	// Token: 0x0400507D RID: 20605
	[SerializeField]
	private string linkCodeLabelString;

	// Token: 0x0400507E RID: 20606
	[SerializeField]
	private string roomSizeDescriptionString;

	// Token: 0x0400507F RID: 20607
	[SerializeField]
	private string roomSizeLabelString;

	// Token: 0x04005080 RID: 20608
	[SerializeField]
	private GameObject OKButton;

	// Token: 0x04005081 RID: 20609
	[SerializeField]
	private List<GameObject> contextualButtons;

	// Token: 0x04005082 RID: 20610
	[SerializeField]
	private List<GameObject> buttonsToShow_MODIO;

	// Token: 0x04005083 RID: 20611
	[SerializeField]
	private List<GameObject> buttonsToShow_ROOMSIZE;

	// Token: 0x04005084 RID: 20612
	private bool processingAccountLink;

	// Token: 0x04005085 RID: 20613
	private string cachedLinkURL;

	// Token: 0x04005086 RID: 20614
	private string cachedLinkCode;

	// Token: 0x04005087 RID: 20615
	private string cachedError;

	// Token: 0x04005088 RID: 20616
	private VirtualStumpOptionsTerminal.ETerminalState currentState;

	// Token: 0x020009AE RID: 2478
	private enum ETerminalState
	{
		// Token: 0x0400508A RID: 20618
		MODIO_ACCOUNT,
		// Token: 0x0400508B RID: 20619
		ROOM_SIZE,
		// Token: 0x0400508C RID: 20620
		NUM_STATES
	}
}
