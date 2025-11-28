using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009F7 RID: 2551
public class SubEmitterListener : MonoBehaviour
{
	// Token: 0x06004134 RID: 16692 RVA: 0x0015B54C File Offset: 0x0015974C
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.Disable();
			return;
		}
		ParticleSystem.SubEmittersModule subEmitters = this.target.subEmitters;
		if (this.subEmitterIndex < 0)
		{
			this.subEmitterIndex = 0;
		}
		this._canListen = (subEmitters.subEmittersCount > 0 && this.subEmitterIndex <= subEmitters.subEmittersCount - 1);
		if (!this._canListen)
		{
			this.Disable();
			return;
		}
		this.subEmitter = this.target.subEmitters.GetSubEmitterSystem(this.subEmitterIndex);
		ParticleSystem.MainModule main = this.subEmitter.main;
		this.interval = main.startLifetime.constantMax * main.startLifetimeMultiplier;
	}

	// Token: 0x06004135 RID: 16693 RVA: 0x0015B608 File Offset: 0x00159808
	private void OnDisable()
	{
		this._listenOnce = false;
		this._listening = false;
	}

	// Token: 0x06004136 RID: 16694 RVA: 0x0015B618 File Offset: 0x00159818
	public void ListenStart()
	{
		if (this._listening)
		{
			return;
		}
		if (this._canListen)
		{
			this.Enable();
			this._listening = true;
		}
	}

	// Token: 0x06004137 RID: 16695 RVA: 0x0015B638 File Offset: 0x00159838
	public void ListenStop()
	{
		this.Disable();
	}

	// Token: 0x06004138 RID: 16696 RVA: 0x0015B640 File Offset: 0x00159840
	public void ListenOnce()
	{
		if (this._listening)
		{
			return;
		}
		this.Enable();
		if (this._canListen)
		{
			this.Enable();
			this._listenOnce = true;
			this._listening = true;
		}
	}

	// Token: 0x06004139 RID: 16697 RVA: 0x0015B670 File Offset: 0x00159870
	private void Update()
	{
		if (!this._canListen)
		{
			return;
		}
		if (!this._listening)
		{
			return;
		}
		if (this.subEmitter.particleCount > 0 && this._sinceLastEmit >= this.interval * this.intervalScale)
		{
			this._sinceLastEmit = 0f;
			this.OnSubEmit();
			if (this._listenOnce)
			{
				this.Disable();
			}
		}
	}

	// Token: 0x0600413A RID: 16698 RVA: 0x0015B6DB File Offset: 0x001598DB
	protected virtual void OnSubEmit()
	{
		UnityEvent unityEvent = this.onSubEmit;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600413B RID: 16699 RVA: 0x0015B6ED File Offset: 0x001598ED
	public void Enable()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	// Token: 0x0600413C RID: 16700 RVA: 0x0015B6FE File Offset: 0x001598FE
	public void Disable()
	{
		if (base.enabled)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04005222 RID: 21026
	public ParticleSystem target;

	// Token: 0x04005223 RID: 21027
	public ParticleSystem subEmitter;

	// Token: 0x04005224 RID: 21028
	public int subEmitterIndex;

	// Token: 0x04005225 RID: 21029
	public UnityEvent onSubEmit;

	// Token: 0x04005226 RID: 21030
	public float intervalScale = 1f;

	// Token: 0x04005227 RID: 21031
	public float interval;

	// Token: 0x04005228 RID: 21032
	[NonSerialized]
	private bool _canListen;

	// Token: 0x04005229 RID: 21033
	[NonSerialized]
	private bool _listening;

	// Token: 0x0400522A RID: 21034
	[NonSerialized]
	private bool _listenOnce;

	// Token: 0x0400522B RID: 21035
	[NonSerialized]
	private TimeSince _sinceLastEmit;
}
