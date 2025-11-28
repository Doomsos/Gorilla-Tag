using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200059D RID: 1437
public class BuilderRoom : MonoBehaviour
{
	// Token: 0x04002FA5 RID: 12197
	public List<GameObject> disableColliderRoots;

	// Token: 0x04002FA6 RID: 12198
	public List<GameObject> disableRenderRoots;

	// Token: 0x04002FA7 RID: 12199
	public List<GameObject> disableGameObjectsForScene;

	// Token: 0x04002FA8 RID: 12200
	public List<GameObject> disableObjectsForPersistent;

	// Token: 0x04002FA9 RID: 12201
	public List<MeshRenderer> disabledRenderersForPersistent;

	// Token: 0x04002FAA RID: 12202
	public List<Collider> disabledCollidersForScene;
}
