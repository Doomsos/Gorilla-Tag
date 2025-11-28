using System;
using UnityEngine;

// Token: 0x020004EA RID: 1258
public class SpitballEvents : SubEmitterListener
{
	// Token: 0x06002057 RID: 8279 RVA: 0x000AB774 File Offset: 0x000A9974
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.GTPlayOneShot(this._sfxHit, 1f);
		}
	}

	// Token: 0x04002ACC RID: 10956
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04002ACD RID: 10957
	[SerializeField]
	private AudioClip _sfxHit;
}
