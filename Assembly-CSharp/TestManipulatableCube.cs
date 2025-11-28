using System;
using UnityEngine;

// Token: 0x02000487 RID: 1159
public class TestManipulatableCube : ManipulatableObject
{
	// Token: 0x06001DA0 RID: 7584 RVA: 0x0009BD6E File Offset: 0x00099F6E
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x0009BD92 File Offset: 0x00099F92
	protected override void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
		if (this.applyReleaseVelocity)
		{
			this.velocity = this.localSpace.MultiplyVector(releaseVelocity);
		}
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x0009BDB0 File Offset: 0x00099FB0
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x0009BDF0 File Offset: 0x00099FF0
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(hand.transform.position);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x0009BE88 File Offset: 0x0009A088
	protected override void OnReleasedUpdate()
	{
		if (this.velocity != Vector3.zero)
		{
			Vector3 vector = this.localSpace.MultiplyPoint(base.transform.position);
			vector += this.velocity * Time.deltaTime;
			if (vector.x < this.minXOffset)
			{
				vector.x = this.minXOffset;
				this.velocity.x = 0f;
			}
			else if (vector.x > this.maxXOffset)
			{
				vector.x = this.maxXOffset;
				this.velocity.x = 0f;
			}
			if (vector.y < this.minYOffset)
			{
				vector.y = this.minYOffset;
				this.velocity.y = 0f;
			}
			else if (vector.y > this.maxYOffset)
			{
				vector.y = this.maxYOffset;
				this.velocity.y = 0f;
			}
			if (vector.z < this.minZOffset)
			{
				vector.z = this.minZOffset;
				this.velocity.z = 0f;
			}
			else if (vector.z > this.maxZOffset)
			{
				vector.z = this.maxZOffset;
				this.velocity.z = 0f;
			}
			vector += this.startingPos;
			base.transform.localPosition = vector;
			this.velocity *= 1f - this.releaseDrag * Time.deltaTime;
			if (this.velocity.sqrMagnitude < 0.001f)
			{
				this.velocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x0009C039 File Offset: 0x0009A239
	public Matrix4x4 GetLocalSpace()
	{
		return this.localSpace;
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x0009C044 File Offset: 0x0009A244
	public void SetCubeToSpecificPosition(Vector3 pos)
	{
		Vector3 vector = this.localSpace.MultiplyPoint3x4(pos);
		vector.x = Mathf.Clamp(vector.x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(vector.y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(vector.z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x0009C0D4 File Offset: 0x0009A2D4
	public void SetCubeToSpecificPosition(float x, float y, float z)
	{
		Vector3 vector;
		vector..ctor(0f, 0f, 0f);
		vector.x = Mathf.Clamp(x, this.minXOffset, this.maxXOffset);
		vector.y = Mathf.Clamp(y, this.minYOffset, this.maxYOffset);
		vector.z = Mathf.Clamp(z, this.minZOffset, this.maxZOffset);
		vector += this.startingPos;
		base.transform.localPosition = vector;
	}

	// Token: 0x04002794 RID: 10132
	public float breakDistance = 0.2f;

	// Token: 0x04002795 RID: 10133
	public float maxXOffset;

	// Token: 0x04002796 RID: 10134
	public float minXOffset;

	// Token: 0x04002797 RID: 10135
	public float maxYOffset;

	// Token: 0x04002798 RID: 10136
	public float minYOffset;

	// Token: 0x04002799 RID: 10137
	public float maxZOffset;

	// Token: 0x0400279A RID: 10138
	public float minZOffset;

	// Token: 0x0400279B RID: 10139
	public bool applyReleaseVelocity;

	// Token: 0x0400279C RID: 10140
	public float releaseDrag = 1f;

	// Token: 0x0400279D RID: 10141
	private Matrix4x4 localSpace;

	// Token: 0x0400279E RID: 10142
	private Vector3 startingPos;

	// Token: 0x0400279F RID: 10143
	private Vector3 velocity;
}
