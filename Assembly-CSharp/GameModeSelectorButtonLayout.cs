using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameModeSelectorButtonLayout : MonoBehaviour
{
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += new Action(this.SetupButtons);
	}

	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= new Action(this.SetupButtons);
	}

	public virtual void SetupButtons()
	{
		GameModeSelectorButtonLayout.<SetupButtons>d__6 <SetupButtons>d__;
		<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetupButtons>d__.<>4__this = this;
		<SetupButtons>d__.<>1__state = -1;
		<SetupButtons>d__.<>t__builder.Start<GameModeSelectorButtonLayout.<SetupButtons>d__6>(ref <SetupButtons>d__);
	}

	[SerializeField]
	protected ModeSelectButton pf_button;

	[SerializeField]
	protected GTZone zone;

	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	protected List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
