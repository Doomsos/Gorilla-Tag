using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200014D RID: 333
public class SITouchscreenButton : MonoBehaviour, IClickable
{
	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x060008DB RID: 2267 RVA: 0x0002FF11 File Offset: 0x0002E111
	private bool IsUsable
	{
		get
		{
			if (!this._screenRegion)
			{
				return Time.time - this._enableTime >= 0.2f;
			}
			return !this._screenRegion.HasPressedButton;
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0002FF48 File Offset: 0x0002E148
	private void Awake()
	{
		ITouchScreenStation componentInParent = base.GetComponentInParent<ITouchScreenStation>();
		if (componentInParent != null)
		{
			this._screenRegion = componentInParent.ScreenRegion;
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x0002FF6B File Offset: 0x0002E16B
	private void OnEnable()
	{
		this._enableTime = Time.time;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0002FF78 File Offset: 0x0002E178
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton();
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002FFC4 File Offset: 0x0002E1C4
	public void PressButton()
	{
		if (!this.IsUsable)
		{
			return;
		}
		if (this._screenRegion)
		{
			this._screenRegion.RegisterButtonPress();
		}
		this.buttonPressed.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (this._pressSound != null)
		{
			GTAudioOneShot.Play(this._pressSound, base.transform.position, this._pressSoundVolume, 1f);
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00030047 File Offset: 0x0002E247
	public void Click(bool leftHand = false)
	{
		this.PressButton();
	}

	// Token: 0x04000AC8 RID: 2760
	public SITouchscreenButton.SITouchscreenButtonType buttonType;

	// Token: 0x04000AC9 RID: 2761
	public int data;

	// Token: 0x04000ACA RID: 2762
	[SerializeField]
	private AudioClip _pressSound;

	// Token: 0x04000ACB RID: 2763
	[SerializeField]
	private float _pressSoundVolume = 0.1f;

	// Token: 0x04000ACC RID: 2764
	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int> buttonPressed;

	// Token: 0x04000ACD RID: 2765
	private SIScreenRegion _screenRegion;

	// Token: 0x04000ACE RID: 2766
	private const float DEBOUNCE_TIME = 0.2f;

	// Token: 0x04000ACF RID: 2767
	private float _enableTime;

	// Token: 0x0200014E RID: 334
	public enum SITouchscreenButtonType
	{
		// Token: 0x04000AD1 RID: 2769
		Back,
		// Token: 0x04000AD2 RID: 2770
		Next,
		// Token: 0x04000AD3 RID: 2771
		Exit,
		// Token: 0x04000AD4 RID: 2772
		Help,
		// Token: 0x04000AD5 RID: 2773
		Select,
		// Token: 0x04000AD6 RID: 2774
		Dispense,
		// Token: 0x04000AD7 RID: 2775
		Research,
		// Token: 0x04000AD8 RID: 2776
		Collect,
		// Token: 0x04000AD9 RID: 2777
		Debug,
		// Token: 0x04000ADA RID: 2778
		PageSelect,
		// Token: 0x04000ADB RID: 2779
		Purchase,
		// Token: 0x04000ADC RID: 2780
		Confirm,
		// Token: 0x04000ADD RID: 2781
		Cancel,
		// Token: 0x04000ADE RID: 2782
		OverrideFailure,
		// Token: 0x04000ADF RID: 2783
		None
	}
}
