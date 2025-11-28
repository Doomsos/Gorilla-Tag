using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000CA0 RID: 3232
public class SnapTurnOverrideOnEnable : MonoBehaviour, ISnapTurnOverride
{
	// Token: 0x06004EEF RID: 20207 RVA: 0x00198448 File Offset: 0x00196648
	private void OnEnable()
	{
		if (this.snapTurn == null && GorillaTagger.Instance != null)
		{
			this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
		}
		if (this.snapTurn != null)
		{
			this.snapTurnOverride = true;
			this.snapTurn.SetTurningOverride(this);
		}
	}

	// Token: 0x06004EF0 RID: 20208 RVA: 0x001984A1 File Offset: 0x001966A1
	private void OnDisable()
	{
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
	}

	// Token: 0x06004EF1 RID: 20209 RVA: 0x001984BE File Offset: 0x001966BE
	bool ISnapTurnOverride.TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x04005DA2 RID: 23970
	private GorillaSnapTurn snapTurn;

	// Token: 0x04005DA3 RID: 23971
	private bool snapTurnOverride;
}
