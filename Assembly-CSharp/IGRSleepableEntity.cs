using System;
using UnityEngine;

// Token: 0x0200074F RID: 1871
public interface IGRSleepableEntity
{
	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x06003052 RID: 12370
	Vector3 Position { get; }

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x06003053 RID: 12371
	float WakeUpRadius { get; }

	// Token: 0x06003054 RID: 12372
	bool IsSleeping();

	// Token: 0x06003055 RID: 12373
	void WakeUp();

	// Token: 0x06003056 RID: 12374
	void Sleep();
}
