using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020004DA RID: 1242
public class GumBubble : LerpComponent
{
	// Token: 0x06001FED RID: 8173 RVA: 0x000A9BED File Offset: 0x000A7DED
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000A9C02 File Offset: 0x000A7E02
	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000A9C10 File Offset: 0x000A7E10
	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000A9C30 File Offset: 0x000A7E30
	public void Inflate()
	{
		base.gameObject.SetActive(true);
		base.enabled = true;
		if (this._animating)
		{
			return;
		}
		this._animating = true;
		this._sinceInflate = 0f;
		if (this.audioSource != null && this._sfxInflate != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxInflate, 1f);
		}
		UnityEvent unityEvent = this.onInflate;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000A9CB4 File Offset: 0x000A7EB4
	public void Pop()
	{
		this._lerp = 0f;
		base.RenderLerp();
		if (this.audioSource != null && this._sfxPop != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxPop, 1f);
		}
		UnityEvent unityEvent = this.onPop;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this._done = false;
		this._animating = false;
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000A9D38 File Offset: 0x000A7F38
	private void Update()
	{
		float num = Mathf.Clamp01(this._sinceInflate / this._lerpLength);
		this._lerp = Mathf.Lerp(0f, 1f, num);
		if (this._lerp <= 1f && !this._done)
		{
			base.RenderLerp();
			if (Mathf.Approximately(this._lerp, 1f))
			{
				this._done = true;
			}
		}
		float num2 = this._lerpLength + this._delayPop;
		if (this._sinceInflate >= num2)
		{
			this.Pop();
		}
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000A9DCC File Offset: 0x000A7FCC
	protected override void OnLerp(float t)
	{
		if (!this.target)
		{
			return;
		}
		if (this._lerpCurve == null)
		{
			GTDev.LogError<string>("[GumBubble] Missing lerp curve", this, null);
			return;
		}
		this.target.localScale = this.targetScale * this._lerpCurve.Evaluate(t);
	}

	// Token: 0x04002A43 RID: 10819
	public Transform target;

	// Token: 0x04002A44 RID: 10820
	public Vector3 targetScale = Vector3.one;

	// Token: 0x04002A45 RID: 10821
	[SerializeField]
	private AnimationCurve _lerpCurve;

	// Token: 0x04002A46 RID: 10822
	public AudioSource audioSource;

	// Token: 0x04002A47 RID: 10823
	[SerializeField]
	private AudioClip _sfxInflate;

	// Token: 0x04002A48 RID: 10824
	[SerializeField]
	private AudioClip _sfxPop;

	// Token: 0x04002A49 RID: 10825
	[SerializeField]
	private float _delayInflate = 1.16f;

	// Token: 0x04002A4A RID: 10826
	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	// Token: 0x04002A4B RID: 10827
	[SerializeField]
	private bool _animating;

	// Token: 0x04002A4C RID: 10828
	public UnityEvent onPop;

	// Token: 0x04002A4D RID: 10829
	public UnityEvent onInflate;

	// Token: 0x04002A4E RID: 10830
	[NonSerialized]
	private bool _done;

	// Token: 0x04002A4F RID: 10831
	[NonSerialized]
	private TimeSince _sinceInflate;
}
