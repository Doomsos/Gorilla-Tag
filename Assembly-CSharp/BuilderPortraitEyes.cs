using System;
using UnityEngine;

// Token: 0x02000561 RID: 1377
public class BuilderPortraitEyes : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060022C3 RID: 8899 RVA: 0x000B5C5E File Offset: 0x000B3E5E
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.scale = base.transform.lossyScale.x;
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000B5C7D File Offset: 0x000B3E7D
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.eyes.transform.position = this.eyeCenter.transform.position;
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x000B5CA8 File Offset: 0x000B3EA8
	public void SliceUpdate()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		Vector3 vector = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(GorillaTagger.Instance.headCollider.transform.position - this.eyeCenter.position, this.eyeCenter.forward), this.moveRadius * this.scale);
		this.eyes.transform.position = this.eyeCenter.position + vector;
	}

	// Token: 0x04002D6A RID: 11626
	[SerializeField]
	private Transform eyeCenter;

	// Token: 0x04002D6B RID: 11627
	[SerializeField]
	private GameObject eyes;

	// Token: 0x04002D6C RID: 11628
	[SerializeField]
	private float moveRadius = 0.5f;

	// Token: 0x04002D6D RID: 11629
	private float scale = 1f;
}
