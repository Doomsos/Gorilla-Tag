using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000088 RID: 136
public class ReleaseCageWhenUpsideDown : MonoBehaviour
{
	// Token: 0x0600035F RID: 863 RVA: 0x0001407A File Offset: 0x0001227A
	private void Awake()
	{
		this.cage = base.GetComponentInChildren<CrittersCage>();
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00014088 File Offset: 0x00012288
	private void Update()
	{
		this.cage.inReleasingPosition = (Vector3.Angle(base.transform.up, Vector3.down) < this.releaseCritterThreshold);
	}

	// Token: 0x040003ED RID: 1005
	public CrittersCage cage;

	// Token: 0x040003EE RID: 1006
	[FormerlySerializedAs("dumpThreshold")]
	[FormerlySerializedAs("angle")]
	public float releaseCritterThreshold = 30f;
}
