using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class SpringyWobbler : MonoBehaviour
{
	// Token: 0x0600109B RID: 4251 RVA: 0x000569E4 File Offset: 0x00054BE4
	private void Start()
	{
		int num = 1;
		Transform transform = base.transform;
		while (transform.childCount > 0)
		{
			transform = transform.GetChild(0);
			num++;
		}
		this.children = new Transform[num];
		transform = base.transform;
		this.children[0] = transform;
		int num2 = 1;
		while (transform.childCount > 0)
		{
			transform = transform.GetChild(0);
			this.children[num2] = transform;
			num2++;
		}
		this.lastEndpointWorldPos = this.children[this.children.Length - 1].transform.position;
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00056A70 File Offset: 0x00054C70
	private void Update()
	{
		float x = base.transform.lossyScale.x;
		Vector3 vector = base.transform.TransformPoint(this.idealEndpointLocalPos);
		this.endpointVelocity += (vector - this.lastEndpointWorldPos) * this.stabilizingForce * x * Time.deltaTime;
		Vector3 vector2 = this.lastEndpointWorldPos + this.endpointVelocity * Time.deltaTime;
		float num = this.maxDisplacement * x;
		if ((vector2 - vector).IsLongerThan(num))
		{
			vector2 = vector + (vector2 - vector).normalized * num;
		}
		this.endpointVelocity = (vector2 - this.lastEndpointWorldPos) * (1f - this.drag) / Time.deltaTime;
		Vector3 vector3 = base.transform.TransformPoint(this.rotateToFaceLocalPos);
		Vector3 vector4 = base.transform.TransformDirection(Vector3.up);
		Vector3 position = base.transform.position;
		Vector3 ctrl = position + base.transform.TransformDirection(this.idealEndpointLocalPos) * this.startStiffness * x;
		Vector3 vector5 = vector2;
		Vector3 ctrl2 = vector5 + (vector3 - vector5).normalized * this.endStiffness * x;
		for (int i = 1; i < this.children.Length; i++)
		{
			float num2 = (float)i / (float)(this.children.Length - 1);
			Vector3 vector6 = BezierUtils.BezierSolve(num2, position, ctrl, ctrl2, vector5);
			Vector3 vector7 = BezierUtils.BezierSolve(num2 + 0.1f, position, ctrl, ctrl2, vector5);
			this.children[i].transform.position = vector6;
			this.children[i].transform.rotation = Quaternion.LookRotation(vector7 - vector6, vector4);
		}
		this.lastIdealEndpointWorldPos = vector;
		this.lastEndpointWorldPos = vector2;
	}

	// Token: 0x040014AC RID: 5292
	[SerializeField]
	private float stabilizingForce;

	// Token: 0x040014AD RID: 5293
	[SerializeField]
	private float drag;

	// Token: 0x040014AE RID: 5294
	[SerializeField]
	private float maxDisplacement;

	// Token: 0x040014AF RID: 5295
	private Transform[] children;

	// Token: 0x040014B0 RID: 5296
	[SerializeField]
	private Vector3 idealEndpointLocalPos;

	// Token: 0x040014B1 RID: 5297
	[SerializeField]
	private Vector3 rotateToFaceLocalPos;

	// Token: 0x040014B2 RID: 5298
	[SerializeField]
	private float startStiffness;

	// Token: 0x040014B3 RID: 5299
	[SerializeField]
	private float endStiffness;

	// Token: 0x040014B4 RID: 5300
	private Vector3 lastIdealEndpointWorldPos;

	// Token: 0x040014B5 RID: 5301
	private Vector3 lastEndpointWorldPos;

	// Token: 0x040014B6 RID: 5302
	private Vector3 endpointVelocity;
}
