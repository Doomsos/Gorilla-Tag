using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000AB8 RID: 2744
public class KIDUI_SendUpgradeEmailScreen : MonoBehaviour
{
	// Token: 0x060044CD RID: 17613 RVA: 0x0016C9D4 File Offset: 0x0016ABD4
	public Task SendUpgradeEmail(List<string> requestedPermissions)
	{
		KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4 <SendUpgradeEmail>d__;
		<SendUpgradeEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SendUpgradeEmail>d__.<>4__this = this;
		<SendUpgradeEmail>d__.requestedPermissions = requestedPermissions;
		<SendUpgradeEmail>d__.<>1__state = -1;
		<SendUpgradeEmail>d__.<>t__builder.Start<KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4>(ref <SendUpgradeEmail>d__);
		return <SendUpgradeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x060044CE RID: 17614 RVA: 0x0016CA1F File Offset: 0x0016AC1F
	public void OnCancel()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x060044CF RID: 17615 RVA: 0x0016CA39 File Offset: 0x0016AC39
	private void OnSuccess()
	{
		base.gameObject.SetActive(false);
		this._successScreen.Show(null);
	}

	// Token: 0x060044D0 RID: 17616 RVA: 0x0016CA53 File Offset: 0x0016AC53
	private void OnFailure(string errorMessage)
	{
		base.gameObject.SetActive(false);
		this._errorScreen.Show(errorMessage);
	}

	// Token: 0x04005681 RID: 22145
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005682 RID: 22146
	[SerializeField]
	private KIDUI_MessageScreen _successScreen;

	// Token: 0x04005683 RID: 22147
	[SerializeField]
	private KIDUI_MessageScreen _errorScreen;

	// Token: 0x04005684 RID: 22148
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
