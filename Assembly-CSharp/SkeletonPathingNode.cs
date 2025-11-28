using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class SkeletonPathingNode : MonoBehaviour
{
	// Token: 0x06000A90 RID: 2704 RVA: 0x000396A0 File Offset: 0x000378A0
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000CDC RID: 3292
	public bool ejectionPoint;

	// Token: 0x04000CDD RID: 3293
	public SkeletonPathingNode[] connectedNodes;

	// Token: 0x04000CDE RID: 3294
	public float distanceToExitNode;
}
