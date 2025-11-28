using System;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009AC RID: 2476
public abstract class CustomMapsTerminalScreen : MonoBehaviour
{
	// Token: 0x06003F39 RID: 16185
	public abstract void Initialize();

	// Token: 0x06003F3A RID: 16186 RVA: 0x00153234 File Offset: 0x00151434
	public virtual void Show()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard != null)
			{
				customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
			}
		}
		this.showTime = Time.time;
	}

	// Token: 0x06003F3B RID: 16187 RVA: 0x00153288 File Offset: 0x00151488
	public virtual void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard != null)
			{
				customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
			}
		}
		this.showTime = 0f;
	}

	// Token: 0x06003F3C RID: 16188 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PressButton(CustomMapKeyboardBinding pressedButton)
	{
	}

	// Token: 0x0400506D RID: 20589
	public CustomMapsKeyboard terminalKeyboard;

	// Token: 0x0400506E RID: 20590
	[SerializeField]
	protected float activationTime = 0.25f;

	// Token: 0x0400506F RID: 20591
	protected float showTime;
}
