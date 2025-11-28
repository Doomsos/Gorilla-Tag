using System;
using UnityEngine;

// Token: 0x02000816 RID: 2070
public class LerpTask<T>
{
	// Token: 0x06003670 RID: 13936 RVA: 0x00126C4E File Offset: 0x00124E4E
	public void Reset()
	{
		this.onLerp.Invoke(this.lerpFrom, this.lerpTo, 0f);
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x00126C7E File Offset: 0x00124E7E
	public void Start(T from, T to, float duration)
	{
		this.lerpFrom = from;
		this.lerpTo = to;
		this.duration = duration;
		this.elapsed = 0f;
		this.active = true;
	}

	// Token: 0x06003672 RID: 13938 RVA: 0x00126CA8 File Offset: 0x00124EA8
	public void Finish()
	{
		this.onLerp.Invoke(this.lerpFrom, this.lerpTo, 1f);
		Action action = this.onLerpEnd;
		if (action != null)
		{
			action.Invoke();
		}
		this.active = false;
		this.elapsed = 0f;
	}

	// Token: 0x06003673 RID: 13939 RVA: 0x00126CF4 File Offset: 0x00124EF4
	public void Update()
	{
		if (!this.active)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.elapsed < this.duration)
		{
			float num = (this.elapsed + deltaTime >= this.duration) ? 1f : (this.elapsed / this.duration);
			this.onLerp.Invoke(this.lerpFrom, this.lerpTo, num);
			this.elapsed += deltaTime;
			return;
		}
		this.Finish();
	}

	// Token: 0x040045C5 RID: 17861
	public float elapsed;

	// Token: 0x040045C6 RID: 17862
	public float duration;

	// Token: 0x040045C7 RID: 17863
	public T lerpFrom;

	// Token: 0x040045C8 RID: 17864
	public T lerpTo;

	// Token: 0x040045C9 RID: 17865
	public Action<T, T, float> onLerp;

	// Token: 0x040045CA RID: 17866
	public Action onLerpEnd;

	// Token: 0x040045CB RID: 17867
	public bool active;
}
