using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200053A RID: 1338
[Serializable]
public class AudioMixVar
{
	// Token: 0x1700038F RID: 911
	// (get) Token: 0x060021AD RID: 8621 RVA: 0x000B01C8 File Offset: 0x000AE3C8
	// (set) Token: 0x060021AE RID: 8622 RVA: 0x000B0217 File Offset: 0x000AE417
	public float value
	{
		get
		{
			if (!this.group)
			{
				return 0f;
			}
			if (!this.mixer)
			{
				return 0f;
			}
			float result;
			if (!this.mixer.GetFloat(this.name, ref result))
			{
				return 0f;
			}
			return result;
		}
		set
		{
			if (this.mixer)
			{
				this.mixer.SetFloat(this.name, value);
			}
		}
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000B0239 File Offset: 0x000AE439
	public void ReturnToPool()
	{
		if (this._pool != null)
		{
			this._pool.Return(this);
		}
	}

	// Token: 0x04002C58 RID: 11352
	public AudioMixerGroup group;

	// Token: 0x04002C59 RID: 11353
	public AudioMixer mixer;

	// Token: 0x04002C5A RID: 11354
	public string name;

	// Token: 0x04002C5B RID: 11355
	[NonSerialized]
	public bool taken;

	// Token: 0x04002C5C RID: 11356
	[SerializeField]
	private AudioMixVarPool _pool;
}
