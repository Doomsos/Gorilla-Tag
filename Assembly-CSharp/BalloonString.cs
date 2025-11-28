using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200040C RID: 1036
public class BalloonString : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600196B RID: 6507 RVA: 0x0008846C File Offset: 0x0008666C
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.vertices = new List<Vector3>(this.numSegments + 1);
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.vertices.Add(this.startPositionXf.position);
			int num = this.vertices.Count - 2;
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)((i + 1) / (this.vertices.Count - 1));
				Vector3 vector = Vector3.Lerp(this.startPositionXf.position, this.endPositionXf.position, num2);
				this.vertices.Add(vector);
			}
			this.vertices.Add(this.endPositionXf.position);
		}
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x0008853C File Offset: 0x0008673C
	private void UpdateDynamics()
	{
		this.vertices[0] = this.startPositionXf.position;
		this.vertices[this.vertices.Count - 1] = this.endPositionXf.position;
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x00088578 File Offset: 0x00086778
	private void UpdateRenderPositions()
	{
		this.lineRenderer.SetPosition(0, this.startPositionXf.transform.position);
		this.lineRenderer.SetPosition(1, this.endPositionXf.transform.position);
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x000885B2 File Offset: 0x000867B2
	public void SliceUpdate()
	{
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.UpdateDynamics();
			this.UpdateRenderPositions();
		}
	}

	// Token: 0x040022D6 RID: 8918
	public Transform startPositionXf;

	// Token: 0x040022D7 RID: 8919
	public Transform endPositionXf;

	// Token: 0x040022D8 RID: 8920
	private List<Vector3> vertices;

	// Token: 0x040022D9 RID: 8921
	public int numSegments = 1;

	// Token: 0x040022DA RID: 8922
	private LineRenderer lineRenderer;
}
