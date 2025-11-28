using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000934 RID: 2356
[DisallowMultipleComponent]
public class VerletLine : MonoBehaviour
{
	// Token: 0x06003C32 RID: 15410 RVA: 0x0013DF0C File Offset: 0x0013C10C
	private void Awake()
	{
		this._nodes = new VerletLine.LineNode[this.segmentNumber];
		this._positions = new Vector3[this.segmentNumber];
		for (int i = 0; i < this.segmentNumber; i++)
		{
			float num = (float)i / (float)(this.segmentNumber - 1);
			Vector3 vector = Vector3.Lerp(this.lineStart.position, this.lineEnd.position, num);
			this._nodes[i] = new VerletLine.LineNode
			{
				position = vector,
				lastPosition = vector,
				acceleration = this.gravity
			};
		}
		this.line.positionCount = this._nodes.Length;
		this.endRigidbody = this.lineEnd.GetComponent<Rigidbody>();
		if (this.endRigidbody)
		{
			this.endRigidbody.maxLinearVelocity = this.endMaxSpeed;
			this.endRigidbodyParent = this.endRigidbody.transform.parent;
			this.rigidBodyStartingLocalPosition = this.endRigidbody.transform.localPosition;
			this.endRigidbody.transform.parent = null;
			this.endRigidbody.gameObject.SetActive(false);
		}
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06003C33 RID: 15411 RVA: 0x0013E04C File Offset: 0x0013C24C
	private void OnEnable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(true);
			this.endRigidbody.transform.localPosition = this.endRigidbodyParent.TransformPoint(this.rigidBodyStartingLocalPosition);
		}
	}

	// Token: 0x06003C34 RID: 15412 RVA: 0x0013E098 File Offset: 0x0013C298
	private void OnDisable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003C35 RID: 15413 RVA: 0x0013E0B8 File Offset: 0x0013C2B8
	public void SetLength(float total, float delay = 0f)
	{
		this.segmentTargetLength = total / (float)this.segmentNumber;
		if (this.segmentTargetLength < this.segmentMinLength)
		{
			this.segmentTargetLength = this.segmentMinLength;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06003C36 RID: 15414 RVA: 0x0013E120 File Offset: 0x0013C320
	public void AddSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength + amount;
		if (this.segmentTargetLength <= 0f)
		{
			return;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06003C37 RID: 15415 RVA: 0x0013E17C File Offset: 0x0013C37C
	public void RemoveSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength - amount;
		if (this.segmentTargetLength <= this.segmentMinLength)
		{
			this.segmentTargetLength = (this.segmentLength = this.segmentMinLength);
			return;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06003C38 RID: 15416 RVA: 0x0013E1D1 File Offset: 0x0013C3D1
	private IEnumerator ResizeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield break;
	}

	// Token: 0x06003C39 RID: 15417 RVA: 0x0013E1E0 File Offset: 0x0013C3E0
	private void Update()
	{
		if (this.segmentLength.Approx(this.segmentTargetLength, 0.1f))
		{
			this.segmentLength = this.segmentTargetLength;
			return;
		}
		this.segmentLength = Mathf.Lerp(this.segmentLength, this.segmentTargetLength, this.resizeSpeed * this.resizeScale * Time.deltaTime);
		if (this.scaleLineWidth)
		{
			this.line.widthMultiplier = base.transform.lossyScale.x;
		}
	}

	// Token: 0x06003C3A RID: 15418 RVA: 0x0013E260 File Offset: 0x0013C460
	public void ForceTotalLength(float totalLength)
	{
		float num = totalLength / (float)((this.segmentNumber < 1) ? 1 : this.segmentNumber);
		this.segmentLength = (this.segmentTargetLength = num);
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06003C3B RID: 15419 RVA: 0x0013E2A8 File Offset: 0x0013C4A8
	private void FixedUpdate()
	{
		for (int i = 0; i < this._nodes.Length; i++)
		{
			VerletLine.Simulate(ref this._nodes[i], Time.fixedDeltaTime);
		}
		for (int j = 0; j < this.simIterations; j++)
		{
			for (int k = 0; k < this._nodes.Length - 1; k++)
			{
				VerletLine.LimitDistance(ref this._nodes[k], ref this._nodes[k + 1], this.segmentLength);
			}
		}
		this._nodes[0].position = this.lineStart.position;
		if (this.endRigidbody)
		{
			if (this.onlyPullAtEdges)
			{
				if ((this.endRigidbody.transform.position - this.lineStart.position).IsLongerThan(this.totalLineLength))
				{
					Vector3 vector = this.lineStart.position + (this.endRigidbody.transform.position - this.lineStart.position).normalized * this.totalLineLength;
					this.endRigidbody.linearVelocity += (vector - this.endRigidbody.transform.position) / Time.fixedDeltaTime;
					if (this.endRigidbody.linearVelocity.IsLongerThan(this.endMaxSpeed))
					{
						this.endRigidbody.linearVelocity = this.endRigidbody.linearVelocity.normalized * this.endMaxSpeed;
					}
				}
			}
			else
			{
				VerletLine.LineNode[] nodes = this._nodes;
				Vector3 vector2 = (nodes[nodes.Length - 1].position - this.lineEnd.position) * (this.tension * this.tensionScale);
				Quaternion rotation = this.endRigidbody.rotation;
				VerletLine.LineNode[] nodes2 = this._nodes;
				Vector3 position = nodes2[nodes2.Length - 1].position;
				VerletLine.LineNode[] nodes3 = this._nodes;
				Quaternion.LookRotation(position - nodes3[nodes3.Length - 2].position);
				if (!this.endRigidbody.isKinematic)
				{
					this.endRigidbody.AddForceAtPosition(vector2, this.endRigidbody.transform.TransformPoint(this.endLineAnchorLocalPosition));
				}
			}
		}
		VerletLine.LineNode[] nodes4 = this._nodes;
		nodes4[nodes4.Length - 1].position = this.lineEnd.position;
		for (int l = 0; l < this._nodes.Length; l++)
		{
			this._positions[l] = this._nodes[l].position;
		}
		this.line.SetPositions(this._positions);
	}

	// Token: 0x06003C3C RID: 15420 RVA: 0x0013E568 File Offset: 0x0013C768
	private static void Simulate(ref VerletLine.LineNode p, float dt)
	{
		Vector3 position = p.position;
		p.position += p.position - p.lastPosition + p.acceleration * (dt * dt);
		p.lastPosition = position;
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x0013E5C0 File Offset: 0x0013C7C0
	private static void LimitDistance(ref VerletLine.LineNode p1, ref VerletLine.LineNode p2, float restLength)
	{
		Vector3 vector = p2.position - p1.position;
		float num = vector.magnitude + 1E-05f;
		float num2 = (num - restLength) / num;
		p1.position += vector * (num2 * 0.5f);
		p2.position -= vector * (num2 * 0.5f);
	}

	// Token: 0x04004CD8 RID: 19672
	public Transform lineStart;

	// Token: 0x04004CD9 RID: 19673
	public Transform lineEnd;

	// Token: 0x04004CDA RID: 19674
	[Space]
	public LineRenderer line;

	// Token: 0x04004CDB RID: 19675
	public Rigidbody endRigidbody;

	// Token: 0x04004CDC RID: 19676
	public Transform endRigidbodyParent;

	// Token: 0x04004CDD RID: 19677
	public Vector3 endLineAnchorLocalPosition;

	// Token: 0x04004CDE RID: 19678
	private Vector3 rigidBodyStartingLocalPosition;

	// Token: 0x04004CDF RID: 19679
	[Space]
	public int segmentNumber = 10;

	// Token: 0x04004CE0 RID: 19680
	public float segmentLength = 0.03f;

	// Token: 0x04004CE1 RID: 19681
	public float segmentTargetLength = 0.03f;

	// Token: 0x04004CE2 RID: 19682
	public float segmentMaxLength = 0.03f;

	// Token: 0x04004CE3 RID: 19683
	public float segmentMinLength = 0.03f;

	// Token: 0x04004CE4 RID: 19684
	[Space]
	public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

	// Token: 0x04004CE5 RID: 19685
	public int simIterations = 6;

	// Token: 0x04004CE6 RID: 19686
	public float tension = 10f;

	// Token: 0x04004CE7 RID: 19687
	public float tensionScale = 1f;

	// Token: 0x04004CE8 RID: 19688
	public float endMaxSpeed = 48f;

	// Token: 0x04004CE9 RID: 19689
	[FormerlySerializedAs("lerpSpeed")]
	[Space]
	public float resizeSpeed = 1f;

	// Token: 0x04004CEA RID: 19690
	public float resizeScale = 1f;

	// Token: 0x04004CEB RID: 19691
	[NonSerialized]
	private VerletLine.LineNode[] _nodes = new VerletLine.LineNode[0];

	// Token: 0x04004CEC RID: 19692
	[NonSerialized]
	private Vector3[] _positions = new Vector3[0];

	// Token: 0x04004CED RID: 19693
	private float totalLineLength;

	// Token: 0x04004CEE RID: 19694
	[SerializeField]
	private bool onlyPullAtEdges;

	// Token: 0x04004CEF RID: 19695
	[SerializeField]
	private bool scaleLineWidth = true;

	// Token: 0x02000935 RID: 2357
	[Serializable]
	public struct LineNode
	{
		// Token: 0x04004CF0 RID: 19696
		public Vector3 position;

		// Token: 0x04004CF1 RID: 19697
		public Vector3 lastPosition;

		// Token: 0x04004CF2 RID: 19698
		public Vector3 acceleration;
	}
}
