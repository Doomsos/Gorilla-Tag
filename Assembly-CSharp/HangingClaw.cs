using System;
using UnityEngine;

// Token: 0x020006CB RID: 1739
[RequireComponent(typeof(LineRenderer))]
public class HangingClaw : MonoBehaviourPostTick
{
	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06002CA8 RID: 11432 RVA: 0x000F2163 File Offset: 0x000F0363
	// (set) Token: 0x06002CA9 RID: 11433 RVA: 0x000F216B File Offset: 0x000F036B
	public new bool PostTickRunning { get; set; }

	// Token: 0x06002CAA RID: 11434 RVA: 0x000F2174 File Offset: 0x000F0374
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		this.segmentCount = 4;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.segmentCount = Mathf.Max(2, this.segmentCount);
		this.baseSegLen = magnitude / (float)this.segmentCount;
		this.ropeSegs = new HangingClaw.RopeSegment[this.segmentCount];
		this.invMass = new float[this.segmentCount];
		for (int i = 0; i < this.segmentCount; i++)
		{
			Vector3 p = Vector3.Lerp(position, this.endTransform.position, (float)i / (float)(this.segmentCount - 1));
			this.ropeSegs[i] = new HangingClaw.RopeSegment(p);
		}
		this.invMass[0] = 0f;
		for (int j = 1; j < this.segmentCount - 1; j++)
		{
			this.invMass[j] = 1f / Mathf.Max(0.0001f, this.segmentMassKg);
		}
		this.invMass[this.segmentCount - 1] = 1f / Mathf.Max(0.0001f, this.endMassKg);
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x000F22A8 File Offset: 0x000F04A8
	public override void PostTick()
	{
		this.Simulate();
		this.DrawRope();
		int num = this.segmentCount - 1;
		int num2 = this.segmentCount;
		this.endTransform.position = this.ropeSegs[num].pos;
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x000F22F0 File Offset: 0x000F04F0
	private void Simulate()
	{
		float num = this.baseSegLen;
		this.targetSegLenScaled = num * (1f + this.slackFraction);
		float num2 = 0.01111f;
		float num3 = Time.time * 0.5f;
		Vector3 vector = this.gravity * num2 * num2;
		Vector3 topPos = base.transform.position + new Vector3(0f, 0.012f * Mathf.Sin(num3), 0.02f * Mathf.Cos(num3));
		for (int i = 1; i < this.segmentCount; i++)
		{
			Vector3 vector2 = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			HangingClaw.RopeSegment[] array = this.ropeSegs;
			int num4 = i;
			array[num4].pos = array[num4].pos + (vector2 * this.velocityDamping + vector);
		}
		int num5 = 3;
		for (int j = 0; j < num5; j++)
		{
			this.ApplyConstraints(topPos);
		}
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x000F242C File Offset: 0x000F062C
	private void ApplyConstraints(Vector3 topPos)
	{
		this.ropeSegs[0].pos = topPos;
		this.ropeSegs[0].posOld = topPos;
		float stiffness = Mathf.Clamp01(this.ropeStiffness);
		for (int i = 0; i < this.segmentCount - 1; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], this.invMass[i], this.invMass[i + 1], stiffness);
		}
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x000F24B0 File Offset: 0x000F06B0
	private void ApplyConstraintSegment(ref HangingClaw.RopeSegment a, ref HangingClaw.RopeSegment b, float wA, float wB, float stiffness)
	{
		Vector3 vector = b.pos - a.pos;
		float magnitude = vector.magnitude;
		if (magnitude < 1E-06f)
		{
			return;
		}
		float num = magnitude - this.targetSegLenScaled;
		if (Mathf.Abs(num) < 1E-06f)
		{
			return;
		}
		Vector3 vector2 = vector / magnitude;
		float num2 = wA + wB;
		if (num2 <= 0f)
		{
			return;
		}
		Vector3 vector3 = vector2 * (num * stiffness);
		a.pos += vector3 * (wA / num2);
		b.pos += -vector3 * (wB / num2);
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x000F2564 File Offset: 0x000F0764
	private void DrawRope()
	{
		if (this.lineRenderer == null)
		{
			return;
		}
		this.lineRenderer.positionCount = this.segmentCount;
		for (int i = 0; i < this.segmentCount; i++)
		{
			Vector3 pos = this.ropeSegs[i].pos;
			if (this.heightCap && pos.y > this.heightCap.position.y)
			{
				pos.y = this.heightCap.position.y;
			}
			this.lineRenderer.SetPosition(i, this.ropeSegs[i].pos);
		}
	}

	// Token: 0x040039FC RID: 14844
	public Transform endTransform;

	// Token: 0x040039FD RID: 14845
	public Transform heightCap;

	// Token: 0x040039FE RID: 14846
	private int segmentCount = 6;

	// Token: 0x040039FF RID: 14847
	public float segmentMassKg = 1f;

	// Token: 0x04003A00 RID: 14848
	public float endMassKg = 5f;

	// Token: 0x04003A01 RID: 14849
	public float ropeStiffness = 0.9f;

	// Token: 0x04003A02 RID: 14850
	public float slackFraction = 0.02f;

	// Token: 0x04003A03 RID: 14851
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x04003A04 RID: 14852
	public float velocityDamping = 0.98f;

	// Token: 0x04003A05 RID: 14853
	private float maxY;

	// Token: 0x04003A06 RID: 14854
	private LineRenderer lineRenderer;

	// Token: 0x04003A07 RID: 14855
	private HangingClaw.RopeSegment[] ropeSegs;

	// Token: 0x04003A08 RID: 14856
	private float baseSegLen;

	// Token: 0x04003A09 RID: 14857
	private float targetSegLenScaled;

	// Token: 0x04003A0A RID: 14858
	private float[] invMass;

	// Token: 0x020006CC RID: 1740
	public struct RopeSegment
	{
		// Token: 0x06002CB1 RID: 11441 RVA: 0x000F267B File Offset: 0x000F087B
		public RopeSegment(Vector3 p)
		{
			this.pos = p;
			this.posOld = p;
		}

		// Token: 0x04003A0C RID: 14860
		public Vector3 pos;

		// Token: 0x04003A0D RID: 14861
		public Vector3 posOld;
	}
}
