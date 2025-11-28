using System;
using UnityEngine;

// Token: 0x020009FA RID: 2554
[Serializable]
public class VelocityHelper
{
	// Token: 0x0600416B RID: 16747 RVA: 0x0015BAE9 File Offset: 0x00159CE9
	public VelocityHelper(int historySize = 12)
	{
		this._size = historySize;
		this._samples = new float[historySize * 4];
	}

	// Token: 0x0600416C RID: 16748 RVA: 0x0015BB08 File Offset: 0x00159D08
	public void SamplePosition(Transform target, float dt)
	{
		Vector3 position = target.position;
		if (!this._initialized)
		{
			this._InitSamples(position, dt);
		}
		this._SetSample(this._latest, position, dt);
		this._latest = (this._latest + 1) % this._size;
	}

	// Token: 0x0600416D RID: 16749 RVA: 0x0015BB50 File Offset: 0x00159D50
	private void _InitSamples(Vector3 position, float dt)
	{
		for (int i = 0; i < this._size; i++)
		{
			this._SetSample(i, position, dt);
		}
		this._initialized = true;
	}

	// Token: 0x0600416E RID: 16750 RVA: 0x0015BB7E File Offset: 0x00159D7E
	private void _SetSample(int i, Vector3 position, float dt)
	{
		this._samples[i] = position.x;
		this._samples[i + 1] = position.y;
		this._samples[i + 2] = position.z;
		this._samples[i + 3] = dt;
	}

	// Token: 0x04005235 RID: 21045
	private float[] _samples;

	// Token: 0x04005236 RID: 21046
	private int _latest;

	// Token: 0x04005237 RID: 21047
	private int _size;

	// Token: 0x04005238 RID: 21048
	private bool _initialized;
}
