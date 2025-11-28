using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020004F4 RID: 1268
public class SoundEffects : MonoBehaviour
{
	// Token: 0x17000370 RID: 880
	// (get) Token: 0x0600209C RID: 8348 RVA: 0x000AD3FE File Offset: 0x000AB5FE
	public bool isPlaying
	{
		get
		{
			return this._lastClipIndex >= 0 && this._lastClipLength >= 0.0 && this._lastClipElapsedTime < this._lastClipLength;
		}
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x000AD431 File Offset: 0x000AB631
	public void Clear()
	{
		this.audioClips.Clear();
		this._lastClipIndex = -1;
		this._lastClipLength = -1.0;
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x000AD454 File Offset: 0x000AB654
	public void Stop()
	{
		if (this.source)
		{
			this.source.GTStop();
		}
		this._lastClipLength = -1.0;
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x000AD480 File Offset: 0x000AB680
	public void PlayNext(float delayMin, float delayMax, float volMin, float volMax)
	{
		float delay = this._rnd.NextFloat(delayMin, delayMax);
		float volume = this._rnd.NextFloat(volMin, volMax);
		this.PlayNext(delay, volume);
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x000AD4B4 File Offset: 0x000AB6B4
	public void PlayNext(float delay = 0f, float volume = 1f)
	{
		if (!this.source)
		{
			return;
		}
		if (this.audioClips == null || this.audioClips.Count == 0)
		{
			return;
		}
		if (this.source.isPlaying)
		{
			this.source.GTStop();
		}
		int num = this._rnd.NextInt(this.audioClips.Count);
		while (this.distinct && this._lastClipIndex == num)
		{
			num = this._rnd.NextInt(this.audioClips.Count);
		}
		AudioClip audioClip = this.audioClips[num];
		this._lastClipIndex = num;
		this._lastClipLength = (double)audioClip.length;
		float num2 = delay;
		if (num2 < this._minDelay)
		{
			num2 = this._minDelay;
		}
		if (num2 < 0.0001f)
		{
			this.source.GTPlayOneShot(audioClip, volume);
			this._lastClipElapsedTime = 0f;
			return;
		}
		this.source.clip = audioClip;
		this.source.volume = volume;
		this.source.GTPlayDelayed(num2);
		this._lastClipElapsedTime = -num2;
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000AD5C8 File Offset: 0x000AB7C8
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(this.seed))
		{
			this.seed = "0x1337C0D3";
		}
		this._rnd = new SRand(this.seed);
		if (this.audioClips == null)
		{
			this.audioClips = new List<AudioClip>();
		}
	}

	// Token: 0x04002B37 RID: 11063
	public AudioSource source;

	// Token: 0x04002B38 RID: 11064
	[Space]
	public List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04002B39 RID: 11065
	public string seed = "0x1337C0D3";

	// Token: 0x04002B3A RID: 11066
	[Space]
	public bool distinct = true;

	// Token: 0x04002B3B RID: 11067
	[SerializeField]
	private float _minDelay;

	// Token: 0x04002B3C RID: 11068
	[Space]
	[SerializeField]
	private SRand _rnd;

	// Token: 0x04002B3D RID: 11069
	[NonSerialized]
	private int _lastClipIndex = -1;

	// Token: 0x04002B3E RID: 11070
	[NonSerialized]
	private double _lastClipLength = -1.0;

	// Token: 0x04002B3F RID: 11071
	[NonSerialized]
	private TimeSince _lastClipElapsedTime;
}
