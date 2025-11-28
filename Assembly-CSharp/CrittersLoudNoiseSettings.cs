using System;

// Token: 0x0200005D RID: 93
public class CrittersLoudNoiseSettings : CrittersActorSettings
{
	// Token: 0x060001CB RID: 459 RVA: 0x0000AF08 File Offset: 0x00009108
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)this.parentActor;
		crittersLoudNoise.soundVolume = this._soundVolume;
		crittersLoudNoise.soundDuration = this._soundDuration;
		crittersLoudNoise.soundEnabled = this._soundEnabled;
		crittersLoudNoise.disableWhenSoundDisabled = this._disableWhenSoundDisabled;
		crittersLoudNoise.volumeFearAttractionMultiplier = this._volumeFearAttractionMultiplier;
	}

	// Token: 0x0400020F RID: 527
	public float _soundVolume;

	// Token: 0x04000210 RID: 528
	public float _soundDuration;

	// Token: 0x04000211 RID: 529
	public bool _soundEnabled;

	// Token: 0x04000212 RID: 530
	public bool _disableWhenSoundDisabled;

	// Token: 0x04000213 RID: 531
	public float _volumeFearAttractionMultiplier = 1f;
}
