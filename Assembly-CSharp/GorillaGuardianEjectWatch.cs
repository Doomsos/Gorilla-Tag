using System;
using GorillaGameModes;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000780 RID: 1920
public class GorillaGuardianEjectWatch : MonoBehaviour
{
	// Token: 0x0600322F RID: 12847 RVA: 0x0010EC88 File Offset: 0x0010CE88
	private void Start()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.AddListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x0010ECB4 File Offset: 0x0010CEB4
	private void OnDestroy()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.RemoveListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x0010ECE0 File Offset: 0x0010CEE0
	private void OnEjectButtonPressed()
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x040040A1 RID: 16545
	[SerializeField]
	private HeldButton ejectButton;
}
