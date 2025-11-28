using System;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public class DisableOtherObjectsWhileActive : MonoBehaviour
{
	// Token: 0x06001126 RID: 4390 RVA: 0x0005BC56 File Offset: 0x00059E56
	private void OnEnable()
	{
		this.SetAllActive(false);
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0005BC5F File Offset: 0x00059E5F
	private void OnDisable()
	{
		this.SetAllActive(true);
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x0005BC68 File Offset: 0x00059E68
	private void SetAllActive(bool active)
	{
		foreach (GameObject gameObject in this.otherObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
		foreach (XSceneRef xsceneRef in this.otherXSceneObjects)
		{
			GameObject gameObject2;
			if (xsceneRef.TryResolve(out gameObject2))
			{
				gameObject2.SetActive(active);
			}
		}
	}

	// Token: 0x040015BA RID: 5562
	public GameObject[] otherObjects;

	// Token: 0x040015BB RID: 5563
	public XSceneRef[] otherXSceneObjects;
}
