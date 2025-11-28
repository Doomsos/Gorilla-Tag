using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B7 RID: 183
public class SteeringWheelCosmetic : MonoBehaviour
{
	// Token: 0x0600048B RID: 1163 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001A052 File Offset: 0x00018252
	public void TryHornHit()
	{
		if (Time.time > this.lastHornTime + this.cooldown)
		{
			this.lastHornTime = Time.time;
			UnityEvent unityEvent = this.onHornHit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0001A084 File Offset: 0x00018284
	private void Update()
	{
		float z = base.transform.localEulerAngles.z;
		if (Mathf.Abs(Mathf.DeltaAngle(this.lastZAngle, z)) >= this.dramaticTurnThreshold)
		{
			UnityEvent unityEvent = this.onDramaticTurn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.lastZAngle = z;
	}

	// Token: 0x04000547 RID: 1351
	[SerializeField]
	private float cooldown = 1.5f;

	// Token: 0x04000548 RID: 1352
	[SerializeField]
	private float dramaticTurnThreshold = 35f;

	// Token: 0x04000549 RID: 1353
	[SerializeField]
	private UnityEvent onHornHit;

	// Token: 0x0400054A RID: 1354
	[SerializeField]
	private UnityEvent onDramaticTurn;

	// Token: 0x0400054B RID: 1355
	private float lastHornTime = -999f;

	// Token: 0x0400054C RID: 1356
	private float lastZAngle;
}
