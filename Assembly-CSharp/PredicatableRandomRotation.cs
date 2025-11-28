using System;
using UnityEngine;

// Token: 0x02000C71 RID: 3185
public class PredicatableRandomRotation : MonoBehaviour
{
	// Token: 0x06004DC1 RID: 19905 RVA: 0x001925E4 File Offset: 0x001907E4
	private void Start()
	{
		if (this.source == null)
		{
			this.source = base.transform;
		}
	}

	// Token: 0x06004DC2 RID: 19906 RVA: 0x00192600 File Offset: 0x00190800
	private void Update()
	{
		float num = (this.source.position.x * this.source.position.x + this.source.position.y * this.source.position.y + this.source.position.z * this.source.position.z) % 1f;
		base.transform.Rotate(this.rot * num);
	}

	// Token: 0x04005CF8 RID: 23800
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	// Token: 0x04005CF9 RID: 23801
	[SerializeField]
	private Transform source;
}
