using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000277 RID: 631
public class ParticleEffectsPool : MonoBehaviour
{
	// Token: 0x06001030 RID: 4144 RVA: 0x00055211 File Offset: 0x00053411
	public void Awake()
	{
		this.OnPoolAwake();
		this.Setup();
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnPoolAwake()
	{
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00055220 File Offset: 0x00053420
	private void Setup()
	{
		this.MoveToSceneWorldRoot();
		this._pools = new RingBuffer<ParticleEffect>[this.effects.Length];
		this._effectToPool = new Dictionary<long, int>(this.effects.Length);
		for (int i = 0; i < this.effects.Length; i++)
		{
			ParticleEffect particleEffect = this.effects[i];
			this._pools[i] = this.InitPoolForPrefab(i, this.effects[i]);
			this._effectToPool.TryAdd(particleEffect.effectID, i);
		}
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x0005529F File Offset: 0x0005349F
	private void MoveToSceneWorldRoot()
	{
		Transform transform = base.transform;
		transform.parent = null;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000552D0 File Offset: 0x000534D0
	private RingBuffer<ParticleEffect> InitPoolForPrefab(int index, ParticleEffect prefab)
	{
		RingBuffer<ParticleEffect> ringBuffer = new RingBuffer<ParticleEffect>(this.poolSize);
		string text = prefab.name.Trim();
		for (int i = 0; i < this.poolSize; i++)
		{
			ParticleEffect particleEffect = Object.Instantiate<ParticleEffect>(prefab, base.transform);
			particleEffect.gameObject.SetActive(false);
			particleEffect.pool = this;
			particleEffect.poolIndex = index;
			particleEffect.name = ZString.Concat<string, string, int>(text, "*", i);
			ringBuffer.Push(particleEffect);
		}
		return ringBuffer;
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00055348 File Offset: 0x00053548
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos)
	{
		this.PlayEffect(effect.effectID, worldPos);
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00055357 File Offset: 0x00053557
	public void PlayEffect(ParticleEffect effect, Vector3 worldPos, float delay)
	{
		this.PlayEffect(effect.effectID, worldPos, delay);
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x00055367 File Offset: 0x00053567
	public void PlayEffect(long effectID, Vector3 worldPos)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos);
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x00055377 File Offset: 0x00053577
	public void PlayEffect(long effectID, Vector3 worldPos, float delay)
	{
		this.PlayEffect(this.GetPoolIndex(effectID), worldPos, delay);
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x00055388 File Offset: 0x00053588
	public void PlayEffect(int index, Vector3 worldPos)
	{
		if (index == -1)
		{
			return;
		}
		ParticleEffect particleEffect;
		if (!this._pools[index].TryPop(out particleEffect))
		{
			return;
		}
		particleEffect.transform.localPosition = worldPos;
		particleEffect.Play();
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x000553BE File Offset: 0x000535BE
	public void PlayEffect(int index, Vector3 worldPos, float delay)
	{
		if (delay.Approx(0f, 1E-06f))
		{
			this.PlayEffect(index, worldPos);
			return;
		}
		base.StartCoroutine(this.PlayDelayed(index, worldPos, delay));
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x000553EB File Offset: 0x000535EB
	private IEnumerator PlayDelayed(int index, Vector3 worldPos, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlayEffect(index, worldPos);
		yield break;
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x0005540F File Offset: 0x0005360F
	public void Return(ParticleEffect effect)
	{
		this._pools[effect.poolIndex].Push(effect);
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x00055428 File Offset: 0x00053628
	public int GetPoolIndex(long effectID)
	{
		int result;
		if (this._effectToPool.TryGetValue(effectID, ref result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x04001431 RID: 5169
	public ParticleEffect[] effects = new ParticleEffect[0];

	// Token: 0x04001432 RID: 5170
	[Space]
	public int poolSize = 10;

	// Token: 0x04001433 RID: 5171
	[Space]
	private RingBuffer<ParticleEffect>[] _pools = new RingBuffer<ParticleEffect>[0];

	// Token: 0x04001434 RID: 5172
	private Dictionary<long, int> _effectToPool = new Dictionary<long, int>();
}
