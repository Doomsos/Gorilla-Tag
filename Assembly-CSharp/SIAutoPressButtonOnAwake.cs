using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class SIAutoPressButtonOnAwake : MonoBehaviour
{
	// Token: 0x060006F1 RID: 1777 RVA: 0x00026310 File Offset: 0x00024510
	private void Awake()
	{
		this.button = base.GetComponent<SITouchscreenButton>();
		this.terminalParent = this.button.GetComponentInParent<SICombinedTerminal>();
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0002632F File Offset: 0x0002452F
	private void OnEnable()
	{
		if (this.button == null)
		{
			return;
		}
		this.awakeTime = Time.time;
		this.buttonPressed = false;
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00026354 File Offset: 0x00024554
	private void Update()
	{
		if (this.buttonPressed || Time.time < this.awakeTime + this.delay)
		{
			return;
		}
		if (this.terminalParent.activePlayer.ActorNr == SIPlayer.LocalPlayer.ActorNr)
		{
			this.button.PressButton();
		}
		this.buttonPressed = true;
	}

	// Token: 0x040008C3 RID: 2243
	private SICombinedTerminal terminalParent;

	// Token: 0x040008C4 RID: 2244
	private SITouchscreenButton button;

	// Token: 0x040008C5 RID: 2245
	private float awakeTime;

	// Token: 0x040008C6 RID: 2246
	private bool buttonPressed;

	// Token: 0x040008C7 RID: 2247
	public float delay = 2f;
}
