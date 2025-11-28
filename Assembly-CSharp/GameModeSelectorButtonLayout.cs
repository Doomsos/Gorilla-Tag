using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class GameModeSelectorButtonLayout : MonoBehaviour
{
	// Token: 0x060003AC RID: 940 RVA: 0x000168E7 File Offset: 0x00014AE7
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += new Action(this.SetupButtons);
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00016911 File Offset: 0x00014B11
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= new Action(this.SetupButtons);
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00016938 File Offset: 0x00014B38
	public virtual void SetupButtons()
	{
		GameModeSelectorButtonLayout.<SetupButtons>d__6 <SetupButtons>d__;
		<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetupButtons>d__.<>4__this = this;
		<SetupButtons>d__.<>1__state = -1;
		<SetupButtons>d__.<>t__builder.Start<GameModeSelectorButtonLayout.<SetupButtons>d__6>(ref <SetupButtons>d__);
	}

	// Token: 0x04000423 RID: 1059
	[SerializeField]
	protected ModeSelectButton pf_button;

	// Token: 0x04000424 RID: 1060
	[SerializeField]
	protected GTZone zone;

	// Token: 0x04000425 RID: 1061
	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	// Token: 0x04000426 RID: 1062
	protected List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
