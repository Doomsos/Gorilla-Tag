using System;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class SpiderDangler : MonoBehaviour
{
	// Token: 0x06000ABE RID: 2750 RVA: 0x0003A3C8 File Offset: 0x000385C8
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.ropeSegLen = magnitude / 6f;
		this.ropeSegs = new SpiderDangler.RopeSegment[6];
		for (int i = 0; i < 6; i++)
		{
			this.ropeSegs[i] = new SpiderDangler.RopeSegment(position);
			position.y -= this.ropeSegLen;
		}
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x0003A44F File Offset: 0x0003864F
	protected void FixedUpdate()
	{
		this.Simulate();
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0003A458 File Offset: 0x00038658
	protected void LateUpdate()
	{
		this.DrawRope();
		Vector3 normalized = (this.ropeSegs[this.ropeSegs.Length - 2].pos - this.ropeSegs[this.ropeSegs.Length - 1].pos).normalized;
		this.endTransform.position = this.ropeSegs[this.ropeSegs.Length - 1].pos;
		this.endTransform.up = normalized;
		Vector4 vector = this.spinSpeeds * Time.time;
		vector..ctor(Mathf.Sin(vector.x), Mathf.Sin(vector.y), Mathf.Sin(vector.z), Mathf.Sin(vector.w));
		vector.Scale(this.spinScales);
		this.endTransform.Rotate(Vector3.up, vector.x + vector.y + vector.z + vector.w);
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0003A55C File Offset: 0x0003875C
	private void Simulate()
	{
		this.ropeSegLenScaled = this.ropeSegLen * base.transform.lossyScale.x;
		Vector3 vector = new Vector3(0f, -0.5f, 0f) * Time.fixedDeltaTime;
		for (int i = 1; i < 6; i++)
		{
			Vector3 vector2 = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			SpiderDangler.RopeSegment[] array = this.ropeSegs;
			int num = i;
			array[num].pos = array[num].pos + vector2 * 0.95f;
			SpiderDangler.RopeSegment[] array2 = this.ropeSegs;
			int num2 = i;
			array2[num2].pos = array2[num2].pos + vector;
		}
		for (int j = 0; j < 8; j++)
		{
			this.ApplyConstraint();
		}
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0003A664 File Offset: 0x00038864
	private void ApplyConstraint()
	{
		this.ropeSegs[0].pos = base.transform.position;
		this.ApplyConstraintSegment(ref this.ropeSegs[0], ref this.ropeSegs[1], 0f, 1f);
		for (int i = 1; i < 5; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], 0.5f, 0.5f);
		}
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0003A6EC File Offset: 0x000388EC
	private void ApplyConstraintSegment(ref SpiderDangler.RopeSegment segA, ref SpiderDangler.RopeSegment segB, float dampenA, float dampenB)
	{
		float num = (segA.pos - segB.pos).magnitude - this.ropeSegLenScaled;
		Vector3 vector = (segA.pos - segB.pos).normalized * num;
		segA.pos -= vector * dampenA;
		segB.pos += vector * dampenB;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0003A778 File Offset: 0x00038978
	private void DrawRope()
	{
		Vector3[] array = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = this.ropeSegs[i].pos;
		}
		this.lineRenderer.positionCount = array.Length;
		this.lineRenderer.SetPositions(array);
	}

	// Token: 0x04000D2A RID: 3370
	public Transform endTransform;

	// Token: 0x04000D2B RID: 3371
	public Vector4 spinSpeeds = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);

	// Token: 0x04000D2C RID: 3372
	public Vector4 spinScales = new Vector4(180f, 90f, 120f, 180f);

	// Token: 0x04000D2D RID: 3373
	private LineRenderer lineRenderer;

	// Token: 0x04000D2E RID: 3374
	private SpiderDangler.RopeSegment[] ropeSegs;

	// Token: 0x04000D2F RID: 3375
	private float ropeSegLen;

	// Token: 0x04000D30 RID: 3376
	private float ropeSegLenScaled;

	// Token: 0x04000D31 RID: 3377
	private const int kSegmentCount = 6;

	// Token: 0x04000D32 RID: 3378
	private const float kVelocityDamper = 0.95f;

	// Token: 0x04000D33 RID: 3379
	private const int kConstraintCalculationIterations = 8;

	// Token: 0x02000191 RID: 401
	public struct RopeSegment
	{
		// Token: 0x06000AC6 RID: 2758 RVA: 0x0003A81D File Offset: 0x00038A1D
		public RopeSegment(Vector3 pos)
		{
			this.pos = pos;
			this.posOld = pos;
		}

		// Token: 0x04000D34 RID: 3380
		public Vector3 pos;

		// Token: 0x04000D35 RID: 3381
		public Vector3 posOld;
	}
}
