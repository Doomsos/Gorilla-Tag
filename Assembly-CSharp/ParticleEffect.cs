using System;
using UnityEngine;

// Token: 0x02000276 RID: 630
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffect : MonoBehaviour
{
	// Token: 0x1700018B RID: 395
	// (get) Token: 0x0600102A RID: 4138 RVA: 0x00055183 File Offset: 0x00053383
	public long effectID
	{
		get
		{
			return this._effectID;
		}
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x0600102B RID: 4139 RVA: 0x0005518B File Offset: 0x0005338B
	public bool isPlaying
	{
		get
		{
			return this.system && this.system.isPlaying;
		}
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x000551A7 File Offset: 0x000533A7
	public virtual void Play()
	{
		base.gameObject.SetActive(true);
		this.system.Play(true);
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x000551C1 File Offset: 0x000533C1
	public virtual void Stop()
	{
		this.system.Stop(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x000551DB File Offset: 0x000533DB
	private void OnParticleSystemStopped()
	{
		base.gameObject.SetActive(false);
		if (this.pool)
		{
			this.pool.Return(this);
		}
	}

	// Token: 0x0400142D RID: 5165
	public ParticleSystem system;

	// Token: 0x0400142E RID: 5166
	[SerializeField]
	private long _effectID;

	// Token: 0x0400142F RID: 5167
	public ParticleEffectsPool pool;

	// Token: 0x04001430 RID: 5168
	[NonSerialized]
	public int poolIndex = -1;
}
