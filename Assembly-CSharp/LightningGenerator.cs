using System;
using UnityEngine;

// Token: 0x02000CBD RID: 3261
public class LightningGenerator : MonoBehaviour
{
	// Token: 0x06004F94 RID: 20372 RVA: 0x00199B4C File Offset: 0x00197D4C
	private void Awake()
	{
		this.strikes = new LightningStrike[this.maxConcurrentStrikes];
		for (int i = 0; i < this.strikes.Length; i++)
		{
			if (i == 0)
			{
				this.strikes[i] = this.prototype;
			}
			else
			{
				this.strikes[i] = Object.Instantiate<LightningStrike>(this.prototype, base.transform);
			}
			this.strikes[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06004F95 RID: 20373 RVA: 0x00199BBC File Offset: 0x00197DBC
	private void OnEnable()
	{
		LightningDispatcher.RequestLightningStrike += this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x06004F96 RID: 20374 RVA: 0x00199BCF File Offset: 0x00197DCF
	private void OnDisable()
	{
		LightningDispatcher.RequestLightningStrike -= this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x06004F97 RID: 20375 RVA: 0x00199BE2 File Offset: 0x00197DE2
	private LightningStrike LightningDispatcher_RequestLightningStrike(Vector3 t1, Vector3 t2)
	{
		this.index = (this.index + 1) % this.strikes.Length;
		return this.strikes[this.index];
	}

	// Token: 0x04005E0F RID: 24079
	[SerializeField]
	private uint maxConcurrentStrikes = 10U;

	// Token: 0x04005E10 RID: 24080
	[SerializeField]
	private LightningStrike prototype;

	// Token: 0x04005E11 RID: 24081
	private LightningStrike[] strikes;

	// Token: 0x04005E12 RID: 24082
	private int index;
}
