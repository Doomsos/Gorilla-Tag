using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x06000F03 RID: 3843 RVA: 0x0004FA3D File Offset: 0x0004DC3D
	private void Reset()
	{
		this._source = base.GetComponent<AudioSource>();
		if (this._source)
		{
			this._source.playOnAwake = false;
		}
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0004FA64 File Offset: 0x0004DC64
	private void OnEnable()
	{
		this.Play();
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0004FA6C File Offset: 0x0004DC6C
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0004FA74 File Offset: 0x0004DC74
	public void Play()
	{
		if (this._loop && this._clips.Length == 1 && this._loopDelay == Vector2.zero)
		{
			this._source.clip = this._clips[0];
			this._source.loop = true;
			this._source.GTPlay();
			return;
		}
		this._source.loop = false;
		if (this._loop)
		{
			base.StartCoroutine(this.DoLoop());
			return;
		}
		this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
		this._source.GTPlay();
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x0004FB1E File Offset: 0x0004DD1E
	private IEnumerator DoLoop()
	{
		while (base.enabled)
		{
			this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
			this._source.GTPlay();
			while (this._source.isPlaying)
			{
				yield return null;
			}
			float num = Random.Range(this._loopDelay.x, this._loopDelay.y);
			if (num > 0f)
			{
				float waitEndTime = Time.time + num;
				while (Time.time < waitEndTime)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0004FB2D File Offset: 0x0004DD2D
	public void Stop()
	{
		this._source.GTStop();
		this._source.loop = false;
	}

	// Token: 0x04001257 RID: 4695
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04001258 RID: 4696
	[SerializeField]
	private AudioClip[] _clips;

	// Token: 0x04001259 RID: 4697
	[SerializeField]
	private bool _loop;

	// Token: 0x0400125A RID: 4698
	[SerializeField]
	private Vector2 _loopDelay;
}
