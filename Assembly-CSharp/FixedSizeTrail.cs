using System;
using UnityEngine;

// Token: 0x020005DF RID: 1503
[RequireComponent(typeof(LineRenderer))]
public class FixedSizeTrail : MonoBehaviour
{
	// Token: 0x170003CC RID: 972
	// (get) Token: 0x060025D9 RID: 9689 RVA: 0x000CA522 File Offset: 0x000C8722
	public LineRenderer renderer
	{
		get
		{
			return this._lineRenderer;
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x060025DA RID: 9690 RVA: 0x000CA52A File Offset: 0x000C872A
	// (set) Token: 0x060025DB RID: 9691 RVA: 0x000CA532 File Offset: 0x000C8732
	public float length
	{
		get
		{
			return this._length;
		}
		set
		{
			this._length = Math.Clamp(value, 0f, 128f);
		}
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x060025DC RID: 9692 RVA: 0x000CA54A File Offset: 0x000C874A
	public Vector3[] points
	{
		get
		{
			return this._points;
		}
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000CA552 File Offset: 0x000C8752
	private void Reset()
	{
		this.Setup();
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000CA552 File Offset: 0x000C8752
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000CA552 File Offset: 0x000C8752
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000CA55C File Offset: 0x000C875C
	public void Setup()
	{
		this._transform = base.transform;
		if (this._lineRenderer == null)
		{
			this._lineRenderer = base.GetComponent<LineRenderer>();
		}
		if (!this._lineRenderer)
		{
			return;
		}
		this._lineRenderer.useWorldSpace = true;
		Vector3 position = this._transform.position;
		Vector3 forward = this._transform.forward;
		int num = this._segments + 1;
		this._points = new Vector3[num];
		float num2 = this._length / (float)this._segments;
		for (int i = 0; i < num; i++)
		{
			this._points[i] = position - forward * num2 * (float)i;
		}
		this._lineRenderer.positionCount = num;
		this._lineRenderer.SetPositions(this._points);
		this.Update();
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x000CA63A File Offset: 0x000C883A
	private void Update()
	{
		if (!this.manualUpdate)
		{
			this.Update(Time.deltaTime);
		}
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x000CA650 File Offset: 0x000C8850
	private void FixedUpdate()
	{
		if (!this.applyPhysics)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = this._points.Length - 1;
		float num2 = this._length / (float)num;
		for (int i = 1; i < num; i++)
		{
			float num3 = (float)(i - 1) / (float)num;
			float num4 = this.gravityCurve.Evaluate(num3);
			Vector3 vector = this.gravity * (num4 * deltaTime);
			this._points[i] += vector;
			this._points[i + 1] += vector;
		}
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x000CA6F4 File Offset: 0x000C88F4
	public void Update(float dt)
	{
		float num = this._length / (float)(this._segments - 1);
		Vector3 position = this._transform.position;
		this._points[0] = position;
		float num2 = Vector3.Distance(this._points[0], this._points[1]);
		float num3 = num - num2;
		if (num2 > num)
		{
			Array.Copy(this._points, 0, this._points, 1, this._points.Length - 1);
		}
		for (int i = 0; i < this._points.Length - 1; i++)
		{
			Vector3 vector = this._points[i];
			Vector3 vector2 = this._points[i + 1] - vector;
			if (vector2.sqrMagnitude > num * num)
			{
				this._points[i + 1] = vector + vector2.normalized * num;
			}
		}
		if (num3 > 0f)
		{
			int num4 = this._points.Length - 1;
			int num5 = num4 - 1;
			Vector3 vector3 = this._points[num4] - this._points[num5];
			Vector3 vector4 = vector3.normalized;
			if (this.applyPhysics)
			{
				Vector3 normalized = (this._points[num5] - this._points[num5 - 1]).normalized;
				vector4 = Vector3.Lerp(vector4, normalized, 0.5f);
			}
			this._points[num4] = this._points[num5] + vector4 * Math.Min(vector3.magnitude, num3);
		}
		this._lineRenderer.SetPositions(this._points);
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x000CA8AC File Offset: 0x000C8AAC
	private static float CalcLength(in Vector3[] positions)
	{
		float num = 0f;
		for (int i = 0; i < positions.Length - 1; i++)
		{
			num += Vector3.Distance(positions[i], positions[i + 1]);
		}
		return num;
	}

	// Token: 0x040031A5 RID: 12709
	[SerializeField]
	private Transform _transform;

	// Token: 0x040031A6 RID: 12710
	[SerializeField]
	private LineRenderer _lineRenderer;

	// Token: 0x040031A7 RID: 12711
	[SerializeField]
	[Range(1f, 128f)]
	private int _segments = 8;

	// Token: 0x040031A8 RID: 12712
	[SerializeField]
	private float _length = 8f;

	// Token: 0x040031A9 RID: 12713
	public bool manualUpdate;

	// Token: 0x040031AA RID: 12714
	[Space]
	public bool applyPhysics;

	// Token: 0x040031AB RID: 12715
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x040031AC RID: 12716
	public AnimationCurve gravityCurve = AnimationCurves.EaseInCubic;

	// Token: 0x040031AD RID: 12717
	[Space]
	private Vector3[] _points = new Vector3[8];
}
