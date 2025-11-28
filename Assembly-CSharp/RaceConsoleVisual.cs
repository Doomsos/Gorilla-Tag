using System;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class RaceConsoleVisual : MonoBehaviour
{
	// Token: 0x0600124E RID: 4686 RVA: 0x0006012C File Offset: 0x0005E32C
	public void ShowRaceInProgress(int laps)
	{
		this.button1.sharedMaterial = this.inactiveButton;
		this.button3.sharedMaterial = this.inactiveButton;
		this.button5.sharedMaterial = this.inactiveButton;
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		switch (laps)
		{
		default:
			this.button1.sharedMaterial = this.selectedButton;
			this.button1.transform.localPosition = this.buttonPressedOffset;
			return;
		case 3:
			this.button3.sharedMaterial = this.selectedButton;
			this.button3.transform.localPosition = this.buttonPressedOffset;
			return;
		case 5:
			this.button5.sharedMaterial = this.selectedButton;
			this.button5.transform.localPosition = this.buttonPressedOffset;
			return;
		}
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x00060240 File Offset: 0x0005E440
	public void ShowCanStartRace()
	{
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		this.button1.sharedMaterial = this.pressableButton;
		this.button3.sharedMaterial = this.pressableButton;
		this.button5.sharedMaterial = this.pressableButton;
	}

	// Token: 0x040016E4 RID: 5860
	[SerializeField]
	private MeshRenderer button1;

	// Token: 0x040016E5 RID: 5861
	[SerializeField]
	private MeshRenderer button3;

	// Token: 0x040016E6 RID: 5862
	[SerializeField]
	private MeshRenderer button5;

	// Token: 0x040016E7 RID: 5863
	[SerializeField]
	private Vector3 buttonPressedOffset;

	// Token: 0x040016E8 RID: 5864
	[SerializeField]
	private Material pressableButton;

	// Token: 0x040016E9 RID: 5865
	[SerializeField]
	private Material selectedButton;

	// Token: 0x040016EA RID: 5866
	[SerializeField]
	private Material inactiveButton;
}
