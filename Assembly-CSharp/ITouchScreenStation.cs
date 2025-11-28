using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public interface ITouchScreenStation
{
	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x060008E2 RID: 2274
	GameObject gameObject { get; }

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x060008E3 RID: 2275
	SIScreenRegion ScreenRegion { get; }

	// Token: 0x060008E4 RID: 2276
	void AddButton(SITouchscreenButton button, bool isPopupButton = false);

	// Token: 0x060008E5 RID: 2277
	void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr);
}
