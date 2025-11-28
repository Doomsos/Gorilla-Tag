using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000150 RID: 336
public class SITouchscreenButtonContainer : MonoBehaviour
{
	// Token: 0x04000AE0 RID: 2784
	public SITouchscreenButton.SITouchscreenButtonType type;

	// Token: 0x04000AE1 RID: 2785
	public string buttonTextString;

	// Token: 0x04000AE2 RID: 2786
	public int data;

	// Token: 0x04000AE3 RID: 2787
	public RectTransform backGround;

	// Token: 0x04000AE4 RID: 2788
	public RectTransform backgroundShadow;

	// Token: 0x04000AE5 RID: 2789
	public Image foreGround;

	// Token: 0x04000AE6 RID: 2790
	public TextMeshProUGUI buttonText;

	// Token: 0x04000AE7 RID: 2791
	public ITouchScreenStation station;

	// Token: 0x04000AE8 RID: 2792
	public SITouchscreenButton button;

	// Token: 0x04000AE9 RID: 2793
	[SerializeField]
	private bool autoConfigure = true;
}
