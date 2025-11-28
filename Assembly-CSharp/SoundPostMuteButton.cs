using System;
using UnityEngine;

// Token: 0x02000932 RID: 2354
public class SoundPostMuteButton : GorillaPressableButton
{
	// Token: 0x06003C2C RID: 15404 RVA: 0x0013DE2C File Offset: 0x0013C02C
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.IsDummyButton)
		{
			SynchedMusicController[] array = this.musicControllers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MuteAudio(this);
			}
			return;
		}
		if (this._targetMuteButton != null)
		{
			this._targetMuteButton.ButtonActivation();
		}
	}

	// Token: 0x04004CD3 RID: 19667
	public SynchedMusicController[] musicControllers;

	// Token: 0x04004CD4 RID: 19668
	[Tooltip("If true, then this button will passthrough clicks to a connected SoundPostMuteButton.")]
	public bool IsDummyButton;

	// Token: 0x04004CD5 RID: 19669
	[SerializeField]
	[Tooltip("The targetted SoundPostMuteButton if this is a dummy button.")]
	private SoundPostMuteButton _targetMuteButton;
}
