using System;
using UnityEngine;

// Token: 0x0200085B RID: 2139
public class PitchShiftAudioPlayer : MonoBehaviour
{
	// Token: 0x06003859 RID: 14425 RVA: 0x0012D766 File Offset: 0x0012B966
	private void Awake()
	{
		if (this._source == null)
		{
			this._source = base.GetComponent<AudioSource>();
		}
		if (this._pitch == null)
		{
			this._pitch = base.GetComponent<RangedFloat>();
		}
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x0012D79C File Offset: 0x0012B99C
	private void OnEnable()
	{
		this._pitchMixVars.Rent(out this._pitchMix);
		this._source.outputAudioMixerGroup = this._pitchMix.group;
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x0012D7C6 File Offset: 0x0012B9C6
	private void OnDisable()
	{
		this._source.Stop();
		this._source.outputAudioMixerGroup = null;
		AudioMixVar pitchMix = this._pitchMix;
		if (pitchMix == null)
		{
			return;
		}
		pitchMix.ReturnToPool();
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x0012D7EF File Offset: 0x0012B9EF
	private void Update()
	{
		if (this.apply)
		{
			this.ApplyPitch();
		}
	}

	// Token: 0x0600385D RID: 14429 RVA: 0x0012D7FF File Offset: 0x0012B9FF
	private void ApplyPitch()
	{
		this._pitchMix.value = this._pitch.curved;
	}

	// Token: 0x04004764 RID: 18276
	public bool apply = true;

	// Token: 0x04004765 RID: 18277
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04004766 RID: 18278
	[SerializeField]
	private AudioMixVarPool _pitchMixVars;

	// Token: 0x04004767 RID: 18279
	[SerializeReference]
	private AudioMixVar _pitchMix;

	// Token: 0x04004768 RID: 18280
	[SerializeField]
	private RangedFloat _pitch;
}
