using System;
using System.Collections;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x0200093B RID: 2363
public class CustomMapsLoadRoomMapButton : GorillaPressableButton
{
	// Token: 0x06003C5E RID: 15454 RVA: 0x0013EFB3 File Offset: 0x0013D1B3
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (CustomMapManager.CanLoadRoomMap())
		{
			CustomMapManager.ApproveAndLoadRoomMap();
		}
	}

	// Token: 0x06003C5F RID: 15455 RVA: 0x0013EFD4 File Offset: 0x0013D1D4
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x04004D06 RID: 19718
	[SerializeField]
	private float pressedTime = 0.2f;
}
