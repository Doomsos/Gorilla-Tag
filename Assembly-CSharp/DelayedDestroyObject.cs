using System;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class DelayedDestroyObject : MonoBehaviour
{
	// Token: 0x06000304 RID: 772 RVA: 0x00012B95 File Offset: 0x00010D95
	private void Start()
	{
		this._timeToDie = Time.time + this.lifetime;
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00012BA9 File Offset: 0x00010DA9
	private void LateUpdate()
	{
		if (Time.time >= this._timeToDie)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040003AB RID: 939
	public float lifetime = 10f;

	// Token: 0x040003AC RID: 940
	private float _timeToDie;
}
